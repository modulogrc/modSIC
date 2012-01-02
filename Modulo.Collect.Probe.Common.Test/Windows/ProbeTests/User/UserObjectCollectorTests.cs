using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.User;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.Providers;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.User
{
    [TestClass]
    public class UserObjectCollectorTests
    {
        private const string EVERYONE_USER = "Everyone";

        [TestMethod, Owner("lfernandes")]
        public void The_Object_Collector_must_be_capable_to_handle_a_user_item_already_collected()
        {
            var fakeUserItem = CreateUserItem(StatusEnumeration.exists);
            
            var collectedItems = new UserObjectCollector(null).CollectDataForSystemItem(fakeUserItem);
            
            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(user_item));
            var collectedUserItem  = (user_item)collectedItems.First().ItemType;
            AssertCollectedUserItemEntities(collectedUserItem, EVERYONE_USER, "1", new string[] { "g1" });
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_user_item_did_not_collected_yet()
        {
            var fakeUserItem = CreateUserItem(StatusEnumeration.notcollected);
            var objectCollector = CreateUserObjectCollectorMock();

            var collectedItems = objectCollector.CollectDataForSystemItem(fakeUserItem);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(user_item), true);
            var collectedUserItem = (user_item)collectedItems.Single().ItemType;
            AssertCollectedUserItemEntities(collectedUserItem, EVERYONE_USER, "1", new string[] { "Fake Group 1", "Fake Group 2" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_user_item_with_not_exists_status_when_no_user_were_found()
        {
            var fakeItemToCollect = CreateUserItem(StatusEnumeration.notcollected);
            var objectCollector = CreateUserObjectCollectorMockWithUserNotFoundBehavior();

            var collectedItems = objectCollector.CollectDataForSystemItem(fakeItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(user_item), false);
            var collectedUserItem = (user_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedUserItem.status, "The collected user item status must be 'does not exist'.");
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedUserItem.user.status, "The collected item user entity must be 'does not exist'.");
        }

        private user_item CreateUserItem(StatusEnumeration itemStatus)
        {
            var newUserItem = 
                new user_item() 
                { 
                    status = itemStatus, 
                    user = OvalHelper.CreateItemEntityWithStringValue(EVERYONE_USER) 
                };

            if (itemStatus.Equals(StatusEnumeration.exists))
            {
                newUserItem.enabled = OvalHelper.CreateBooleanEntityItemFromBoolValue(true);
                newUserItem.group = new EntityItemStringType[] { OvalHelper.CreateItemEntityWithStringValue("g1") };
            }

            return newUserItem;
        }

        private UserObjectCollector CreateUserObjectCollectorMock()
        {
            var mocks = new MockRepository();
            var fakeAccountProvider = mocks.DynamicMock<WindowsUsersProvider>(new object[] { null, null });
            Expect.Call(fakeAccountProvider.GetUserByName(EVERYONE_USER)).Return(new WindowsAccount(EVERYONE_USER, (bool?)true, "", AccountType.User));
            Expect.Call(fakeAccountProvider.GetUserGroups(EVERYONE_USER, AccountSearchReturnType.Name)).Return(new string[] { "Fake Group 1", "Fake Group 2" });
            mocks.ReplayAll();

            return new UserObjectCollector(fakeAccountProvider);
        }


        private UserObjectCollector CreateUserObjectCollectorMockWithUserNotFoundBehavior()
        {
            var mocks = new MockRepository();
            var fakeAccountProvider = mocks.DynamicMock<WindowsUsersProvider>(new object[] { null, null });
            Expect.Call(fakeAccountProvider.GetUserByName(EVERYONE_USER)).Throw(new WindowsUserNotFound(EVERYONE_USER));
            mocks.ReplayAll();

            return new UserObjectCollector(fakeAccountProvider);
        }

        private void AssertCollectedUserItemEntities(
            user_item collectedItem, string expectedName, string expectedEnabled, params string[] expectedGroups)
        {
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.user, expectedName, "user");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.enabled, expectedEnabled, "enabled");
            Assert.AreEqual(expectedGroups.Count(), collectedItem.group.Count(), "Unexpected amount of group was found.");
            for (int i = 0; i < expectedGroups.Count(); i++)
                ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.group.ElementAt(i), expectedGroups.ElementAt(i), "group");
        }


    }
}
