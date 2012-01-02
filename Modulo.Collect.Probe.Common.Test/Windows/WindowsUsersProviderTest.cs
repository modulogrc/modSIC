using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.Providers;
using System;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass]
    public class WindowsUsersProviderTest
    {
        private const string EVERYONE_USERNAME = "Everyone";
        private const string FAKE_COMPUTER_NAME = "WORKSTATION";
        private const string FAKE_USERNAME = "oval";
        
        private string ExpectedWqlToGetBuiltinAccount;
        private string ExpectedWqlToGetAnyAccount;
        
        public WindowsUsersProviderTest()
        {
            this.ExpectedWqlToGetBuiltinAccount =
                 new WQLBuilder()
                    .WithWmiClass("Win32_SystemAccount")
                        .AddParameter("Name", EVERYONE_USERNAME)
                 .Build();

            this.ExpectedWqlToGetAnyAccount =
                new WQLBuilder()
                    .WithWmiClass("Win32_UserAccount")
                        .AddParameter("Domain", FAKE_COMPUTER_NAME)
                        .AddParameter("Name", FAKE_USERNAME)
                .Build();

        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_builtin_account()
        {
            var fakeWmiProvider = GetWmiProviderMock(EVERYONE_USERNAME, false, new string[] { "group1", "group1" });
            
            var result = new WindowsUsersProvider(fakeWmiProvider, null).GetUserByName(EVERYONE_USERNAME);

            AssertCollectedWindowsAccount(result, EVERYONE_USERNAME, true);
            CheckIfWmiProviderWasUsedProperly(fakeWmiProvider, ExpectedWqlToGetBuiltinAccount);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_local_or_domain_account()
        {
            var fakeUsername = String.Format("{0}\\{1}", FAKE_COMPUTER_NAME, FAKE_USERNAME);
            var fakeWmiProvider = GetWmiProviderMock(fakeUsername, true, null);

            var result = new WindowsUsersProvider(fakeWmiProvider, null).GetUserByName(fakeUsername);

            AssertCollectedWindowsAccount(result, fakeUsername, false);
            CheckIfWmiProviderWasUsedProperly(fakeWmiProvider, ExpectedWqlToGetAnyAccount);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_not_possible_to_get_user_details_through_wmi_only_the_username_should_be_returned()
        {
            var fakeWmiProvider = GetWmiProviderMockWithNoWmiResultBehavior();

            var result = new WindowsUsersProvider(fakeWmiProvider, null).GetAllGroupByUsers();

            Assert.AreEqual(1, result.Count());
            var userAccountToAssert = result.Single();
            AssertCollectedWindowsAccount(userAccountToAssert, "fakeUser");
            
            var groups = userAccountToAssert.Members;
            Assert.AreEqual(3, groups.Count());
            var firstGroup = groups.ElementAt(0);
            Assert.AreEqual("Administrators", firstGroup.Name);
            Assert.IsNull(firstGroup.Enabled);
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(WindowsUserNotFound))]
        public void Should_be_possible_to_handle_not_founded_users()
        {
            var fakeWmiProvider = GetWmiProviderMock(null, true, null);

            var result = new WindowsUsersProvider(fakeWmiProvider, null).GetUserByName("Ghost");
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(InvalidUsernameFormat))]
        public void An_exception_must_be_thrown_if_an_invalid_username_is_passed()
        {
            new WindowsUsersProvider(null, null).GetUserByName("domain\\user\\user");
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(InvalidUsernameFormat))]
        public void An_exception_must_be_thrown_if_an_empty_username_is_passed()
        {
            new WindowsUsersProvider(null, null).GetUserByName(" ");
        }

        private WmiDataProvider GetWmiProviderMock(string usernameReturn, bool userDisabledReturn, params string[] groupNamesReturn)
        {
            var fakeWqlResult = new List<WmiObject>();
            if (usernameReturn != null)
            {
                var fakeWmiObject = new WmiObject();
                fakeWmiObject.Add("Domain", "fakeDomainOrComputername");
                fakeWmiObject.Add("Name", usernameReturn);
                fakeWmiObject.Add("Disabled", userDisabledReturn);
                fakeWqlResult.Add(fakeWmiObject);
            }

            var mocks = new MockRepository();
            var fakeWmiProvider = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeWmiProvider.ExecuteWQL(null)).IgnoreArguments().Return(fakeWqlResult);
            mocks.ReplayAll();

            return fakeWmiProvider;
        }

        private WmiDataProvider GetWmiProviderMockWithNoWmiResultBehavior()
        {
            // This mocked WmiDataProvider must be used in order to test GetAllGroupsByUser method.
            var mocks = new MockRepository();
            var fakeWmiProvider = mocks.StrictMock<WmiDataProvider>();
            
            // Create expectation for first calling (to get computer name).
            var wqlToGetComputerName = new WQLBuilder().WithWmiClass("Win32_ComputerSystem").Build();
            var fakeWin32ComputerSystemRecord = new WmiObject();
            fakeWin32ComputerSystemRecord.Add("Name", FAKE_COMPUTER_NAME);
            fakeWin32ComputerSystemRecord.Add("DomainRole", 1);
            Expect.Call(fakeWmiProvider.ExecuteWQL(wqlToGetComputerName)).Return(new[] { fakeWin32ComputerSystemRecord });
            
            // Create expectation for second calling (to get all local groups);
            var wqlToGetAllLocalGroups = new WQLBuilder().WithWmiClass("Win32_Group").AddParameter("localaccount", "1").Build();
            var fakeWin32GroupRecords = new List<WmiObject>();
            fakeWin32GroupRecords.Add(NewWmiObjectForFakeLocalGroup("Administrators", "S-1-5-32-544"));
            fakeWin32GroupRecords.Add(NewWmiObjectForFakeLocalGroup("Backup Operators", "S-1-5-32-551"));
            fakeWin32GroupRecords.Add(NewWmiObjectForFakeLocalGroup("Users", "S-1-5-32-545"));
            Expect.Call(fakeWmiProvider.ExecuteWQL(wqlToGetAllLocalGroups)).Return(fakeWin32GroupRecords);
            
            // Create expectation for each get group component calling...
            CreateExpectationForGetGroupComponent(fakeWmiProvider, "Administrators");
            CreateExpectationForGetGroupComponent(fakeWmiProvider, "Backup Operators");
            CreateExpectationForGetGroupComponent(fakeWmiProvider, "Users");

            // Create expectation for each get user details calling (3 first times) and get user SID (3 last times)
            Expect.Call(fakeWmiProvider.ExecuteWQL(null)).IgnoreArguments().Repeat.Times(6).Return(null);

            var wqlAllGetBuiltinAccounts = new WQLBuilder().WithWmiClass("Win32_SystemAccount").Build();
            Expect.Call(fakeWmiProvider.ExecuteWQL(wqlAllGetBuiltinAccounts)).Return(null);

            mocks.ReplayAll();
            
            return fakeWmiProvider;
        }

        private void CreateExpectationForGetGroupComponent(WmiDataProvider fakeWmiProvider, string groupName)
        {
            var fakeWin32GroupUserRecords = new List<WmiObject>();
            var groupComponent = GroupComponent(FAKE_COMPUTER_NAME, groupName);
            var wqlAdministratorsUsers = new WQLBuilder().WithWmiClass("Win32_GroupUser").AddParameter("GroupComponent", groupComponent).Build();
            Expect.Call(fakeWmiProvider.ExecuteWQL(wqlAdministratorsUsers)).Return(new[] { NewWmiObjectForFakeGroupUser("fakeUser") });
        }

        private string GroupComponent(string domain, string name)
        {
            return String.Format("\\\\\\\\{0}\\\\root\\\\cimv2:Win32_Group.Domain=\"{0}\",Name=\"{1}\"", domain, name);
        }

        private WmiObject NewWmiObjectForFakeLocalGroup(string groupName, string sid)
        {
            var newWmiObject = new WmiObject();
            newWmiObject.Add("Name", groupName);
            newWmiObject.Add("SID", sid);
            return newWmiObject;
        }

        private WmiObject NewWmiObjectForFakeGroupUser(string name)
        {
            var partComponent = String.Format("\\\\{0}\\root\\cimv2:Win32_UserAccount.Domain=\"{0}\",Name=\"{1}\"", FAKE_COMPUTER_NAME, name);
            var newWmiObject = new WmiObject();
            newWmiObject.Add("PartComponent", partComponent);
            return newWmiObject;
        }

        private void AssertCollectedWindowsAccount(WindowsAccount accountToAssert, string expectedAccountName, bool? accountMustBeEnabled = null)
        {
            Assert.IsNotNull(accountToAssert, "The result cannot be null.");
            Assert.AreEqual(AccountType.User, accountToAssert.AccountType, "Unexpected windows user account type was found.");
            Assert.AreEqual(expectedAccountName, accountToAssert.Name, "Unexpected user account name was found.");
            Assert.AreEqual(accountMustBeEnabled, accountToAssert.Enabled, "Unexpected user account status.");
        }

        private void CheckIfWmiProviderWasUsedProperly(WmiDataProvider usedWmiProvider, string wqlThatShouldBeUsed)
        {
            usedWmiProvider.AssertWasCalled<WmiDataProvider>(wmi => wmi.ExecuteWQL(wqlThatShouldBeUsed));
        }

        private void WmiProviderMustNotHaveBeenUsed(WmiDataProvider wmiProvider)
        {
            wmiProvider.AssertWasNotCalled<WmiDataProvider>(wmi => wmi.ExecuteWQL(null));
        }

    }
}
