/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Windows.WMI;
using System.DirectoryServices.AccountManagement;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Extensions;
using Modulo.Collect.Probe.Windows.Helpers;
using System.Security.Principal;

namespace Modulo.Collect.Probe.Windows.Providers
{
    public enum AccountType { User, Group }

    public enum AccountSearchReturnType { Name, SID };

    public class WindowsUsersProvider
    {
        private  WmiDataProvider WmiProvider;
        private TargetInfo TargetInfo;
        private int TargetDomainRole;
        
        public WindowsUsersProvider(WmiDataProvider wmiDataProvider, TargetInfo targetInfo)
        {
            this.WmiProvider = wmiDataProvider;
            this.TargetInfo = targetInfo;
        }

        /// <summary>
        /// Gets all groups and users from target.
        /// </summary>
        /// <returns>
        /// It returns a list of WindowsAccount struct where each element represents a user or group account.
        /// </returns>
        public virtual IEnumerable<WindowsAccount> GetAllGroupsAndUsers()
        {
            var allUsers = new Dictionary<string, WindowsAccount>();
            var allLocalGroups = GetAllUsersByGroup();

            foreach (var group in allLocalGroups)
            {
                allUsers.Add(group.Name, group);

                if (group.Members == null)
                    continue;

                foreach (var user in group.Members)
                {
                    if (!allUsers.ContainsKey(user.Name))
                    {
                        var newUserAccount = new WindowsAccount(user.Name, user.Enabled, user.AccountSID, AccountType.User);
                        allUsers.Add(user.Name, newUserAccount);
                    }
                }
            }

            var allBuiltinAccounts = this.GetAllBuiltinAccounts();
            foreach (var builtinAccount in allBuiltinAccounts)
                if (!allUsers.ContainsKey(builtinAccount.Name))
                    allUsers.Add(builtinAccount.Name, builtinAccount);

            return allUsers.Select(user => user.Value);
        }

        /// <summary>
        /// It gets all local users (and its groups) from target.
        /// </summary>
        /// <returns>
        /// It returns a list of WindowsAccount struct where each element represents a user account.
        /// In order to access user groups, use the Members property.
        /// </returns>
        public virtual IEnumerable<WindowsAccount> GetAllGroupByUsers()
        {
            var allUsers = new Dictionary<string, WindowsAccount>();

            var allUsersByGroup = GetAllUsersByGroup();
            foreach (var group in allUsersByGroup)
            {
                if (group.Members == null)
                    continue;

                foreach (var user in group.Members)
                {
                    if (allUsers.ContainsKey(user.Name))
                    {
                        allUsers[user.Name].AddMember(group.Name, group.Enabled, group.AccountSID);
                    }
                    else 
                    {
                        var newUserAccount = new WindowsAccount(user.Name, user.Enabled, user.AccountSID, AccountType.User);
                        newUserAccount.AddMember(group.Name, group.Enabled, group.AccountSID);
                        allUsers.Add(user.Name, newUserAccount);
                    }
                }
            }

            var allBuiltinAccounts = this.GetAllBuiltinAccounts();
            foreach (var builtinAccount in allBuiltinAccounts)
                if (!allUsers.ContainsKey(builtinAccount.Name))
                    allUsers.Add(builtinAccount.Name, builtinAccount);

            return allUsers.Select(user => user.Value);
        }

        /// <summary>
        /// It gets a all local groups (and its members) from target.
        /// </summary>
        /// <returns>
        /// It returns a list of WindowsAccount struct where each element represents a group account.
        /// In order to access group members, use the Members property.
        /// </returns>
        public virtual IEnumerable<WindowsAccount> GetAllUsersByGroup()
        {
            var computerSystem = GetComputerSystemFromTarget();

            var allUsersByGroup = GetAllLocalGroups();
            foreach (var group in allUsersByGroup)
            {
                var allGroupUsers = GetAllUsersFromGroup(computerSystem, group.Name);
                foreach (var user in allGroupUsers)
                {
                    var accountSID = GetUserSID(computerSystem, user.Key);
                    group.AddMember(user.Key, user.Value, accountSID);
                }
            }

            return allUsersByGroup;
        }

        /// <summary>
        /// It looks for a user in the target.
        /// </summary>
        /// <param name="username">The user complete name. 
        /// For local users, use [targetName]\[username] format.
        /// For domain users, use [domain]\[username] format. 
        /// For system built-in account, use the account name only. Example: "Everyone"</param>
        /// <returns></returns>
        public virtual WindowsAccount GetUserByName(string username)
        {
            var wqlToGetUserDetails = GetWqlToGetUserDetails(username);

            var collectedUser = this.WmiProvider.ExecuteWQL(wqlToGetUserDetails);

            if (collectedUser.IsEmpty())
                throw new WindowsUserNotFound(username);

            var accountName = collectedUser.First().GetFieldValueAsString("Name");
            var disabledProperty = (bool?)collectedUser.First().GetValueOf("Disabled");
            var accountEnabled = ((disabledProperty == null) || ((bool)!disabledProperty));
            return new WindowsAccount(accountName, accountEnabled, string.Empty, AccountType.User);
        }

        /// <summary>
        /// Gets the groups the user belongs 
        /// </summary>
        /// <param name="username">The user name in FQDN format. For SYSTEM accounts do not use FQDN.</param>
        /// <param name="returnType">Defines wether will be returned the group name ou SID.</param>
        /// <returns>A list of group names or group SIDs according with returnType argument.</returns>
        public virtual IEnumerable<String> GetUserGroups(string username, AccountSearchReturnType returnType)
        {
            var address = this.TargetInfo.GetAddress();
            var credentials = this.TargetInfo.credentials;
            var principalContext = AccManUtils.accManConnect(address, credentials.GetUserName(), credentials.GetPassword());

            Principal user = null;
            var isSystemAccount = !username.Contains("\\");

            if (isSystemAccount)
            {
                var wqlGetUserSID = GetWqlToGetUserDetails(username);
                var wqlResult = this.WmiProvider.ExecuteWQL(wqlGetUserSID).FirstOrDefault();
                if (wqlResult != null)
                {
                    var userSID = wqlResult.GetFieldValueAsString("SID");
                    user = Principal.FindByIdentity(principalContext, IdentityType.Sid, userSID);
                }
            }
            else
            {
                user = Principal.FindByIdentity(principalContext, IdentityType.SamAccountName, username);
                if (user == null)
                    user = Principal.FindByIdentity(principalContext, IdentityType.Name, username);
            }

            var groups = new List<String>();
            if (user == null)
                return groups;

            var foundGroups = user.GetGroups();
            if (foundGroups == null)
                return groups;

            if (returnType.Equals(AccountSearchReturnType.SID))
                return foundGroups.Select(g => g.Sid.Value);

            return foundGroups.Select(g => g.Name);
        }


        private IEnumerable<WindowsAccount> GetAllBuiltinAccounts()
        {
            var wqlAllGetBuiltinAccounts = new WQLBuilder().WithWmiClass("Win32_SystemAccount").Build();
            
            var wqlResult = this.WmiProvider.ExecuteWQL(wqlAllGetBuiltinAccounts);
            
            if (wqlResult != null && wqlResult.Count() > 0)
            {
                return
                    wqlResult.Select(
                        wql =>
                            new WindowsAccount(
                                wql.GetFieldValueAsString("Name"),
                                (bool?)true,
                                wql.GetFieldValueAsString("SID"),
                                AccountType.User));
            }

            return new WindowsAccount[] { };
        }

        private string GetUserSID(string domain, string accountName)
        {
            var wqlGetAccountSid =
                new WQLBuilder()
                    .WithWmiClass("Win32_Account")
                        .AddParameter("Domain", domain)
                        .AddParameter("Name", accountName)
                .Build();

            var wqlResult = WmiProvider.ExecuteWQL(wqlGetAccountSid);
            
            if (wqlResult.HasItems())
                return wqlResult.First().GetFieldValueAsString("SID");

            if (this.TargetDomainRole > 3)
            {
                var wqlGetDomain = "select * from Win32_NTDomain";
                var domainName = WmiProvider.ExecuteWQL(wqlGetDomain).First().GetFieldValueAsString("DomainName");
                wqlGetAccountSid =
                    new WQLBuilder()
                        .WithWmiClass("Win32_Account")
                            .AddParameter("Domain", domainName)
                            .AddParameter("Name", accountName)
                        .Build();
                wqlResult = WmiProvider.ExecuteWQL(wqlGetAccountSid);
                if (wqlResult != null && wqlResult.Count() > 0)
                    return wqlResult.First().GetFieldValueAsString("SID");
            }

            return string.Empty;
        }

        private string GetWqlToGetUserDetails(string sourceUsername)
        {
            var usernameParts = sourceUsername.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (String.IsNullOrWhiteSpace(sourceUsername) || usernameParts.Count() > 2)
                throw new InvalidUsernameFormat(sourceUsername);

            if (usernameParts.Count() == 2)
            {
                var domain = usernameParts.First();
                var username = usernameParts.Last();
                
                var wqlFilter = new Dictionary<String, String>();
                wqlFilter.Add("Domain", domain);
                wqlFilter.Add("Name", username);
                
                return new WQLBuilder().WithWmiClass("Win32_UserAccount").AddParameters(wqlFilter).Build();
            }

            return new WQLBuilder().WithWmiClass("Win32_SystemAccount").AddParameter("Name", sourceUsername).Build();
        }

        private string GetComputerSystemFromTarget()
        {
            var wqlToGetComputerName = new WQLBuilder().WithWmiClass("Win32_ComputerSystem").Build();
            var computerSystem = WmiProvider.ExecuteWQL(wqlToGetComputerName).First();
            this.TargetDomainRole = int.Parse(computerSystem.GetFieldValueAsString("DomainRole"));
            
            return computerSystem.GetFieldValueAsString("Name");
        }

        private IEnumerable<WindowsAccount> GetAllLocalGroups()
        {
            var wqlToGetAllLocalGroups =
                new WQLBuilder()
                    .WithWmiClass("Win32_Group")
                    .AddParameter("localaccount", "1")
                .Build();

            var wqlResult = this.WmiProvider.ExecuteWQL(wqlToGetAllLocalGroups);

            return 
                wqlResult
                    .Select(g => 
                        new WindowsAccount(
                            g.GetFieldValueAsString("Name"), 
                            (bool?)g.GetValueOf("Disabled"), 
                            g.GetFieldValueAsString("SID"),
                            AccountType.Group)
                 ).ToList();
        }

        private Dictionary<string, bool?> GetAllUsersFromGroup(string computerName, string groupName)
        {
            var allGroupUsers = new Dictionary<string, bool?>();

            var wqlToGetUsersFromGroup = BuildWqlToGetUsersFromGroup(computerName, groupName);
            var wqlResult = this.WmiProvider.ExecuteWQL(wqlToGetUsersFromGroup);
            if (wqlResult.IsEmpty())
                return allGroupUsers;

            var allPartComponents = wqlResult.Select(gu => gu.GetFieldValueAsString("PartComponent"));
            foreach (var partComponent in allPartComponents)
            {
                var indexOfDomainPart = partComponent.IndexOf("Win32_UserAccount.Domain", 0, StringComparison.InvariantCultureIgnoreCase);
                if (indexOfDomainPart < 0)
                    continue;

                var domainComponentPart = partComponent.Substring(indexOfDomainPart);
                var userFilter = GetUserDomainFilterParts(domainComponentPart);
                var wqlGetUserDetails = BuildWqlToGetUserDetails(userFilter.Key, userFilter.Value);
                var userDetails = this.WmiProvider.ExecuteWQL(wqlGetUserDetails);

                if (userDetails.IsEmpty())
                {
                    var username = ExtractUsernameFromDomainComponentPart(userFilter.Key);
                    allGroupUsers.Add(username, null);
                }
                else
                {
                    var userDetailsRecord = userDetails.FirstOrDefault();
                    var wqlUsernameFieldValue = userDetailsRecord.GetFieldValueAsString("Name");
                    var wqlDisabledFieldValue = (bool?)userDetailsRecord.GetValueOf("Disabled");
                    var userEnabled = wqlDisabledFieldValue == null ? false : !wqlDisabledFieldValue;
                    allGroupUsers.Add(wqlUsernameFieldValue, userEnabled);
                }
            }

            return allGroupUsers;
        }
        
        private string BuildWqlToGetUserDetails(string userCriteria, string domainCriteria)
        {
            var wqlGetUserWithoutFilter = new WQLBuilder().WithWmiClass("Win32_Account").Build();
            var wqlGetUserDetailsBuilder = new StringBuilder(wqlGetUserWithoutFilter);
            var wqlFilter = String.Format(" WHERE {0} and {1} ", domainCriteria, userCriteria); 
            wqlGetUserDetailsBuilder.AppendLine(wqlFilter);

            return wqlGetUserDetailsBuilder.ToString();
        }

        private string ExtractUsernameFromDomainComponentPart(string domainComponentPart)
        {
            try
            {
                var userFilter = GetUserDomainFilterParts(domainComponentPart);
                return System.Text.RegularExpressions.Regex.Match(userFilter.Key, "(Name=\")(.*)(\")").Groups[2].Value;
            }
            catch(Exception)
            {
                return "<Unable to get username>";
            }
        }

        /// <summary>
        /// Get Domain and User filter criterias from component part.
        /// </summary>
        /// <param name="domainUserFilter">
        ///     Component Part Format: Win32_UserAccount.Domain="[DOMAIN_NAME]",Name="[USER_NAME]"
        ///     Example: Win32_UserAccount.Domain="OVAL-W2K3-C-DC0",Name="oval"
        /// </param>
        /// <returns>A key/value pair which on key is user criteria and the value is domain criteria.</returns>
        private KeyValuePair<string, string> GetUserDomainFilterParts(string domainUserFilter)
        {
            var splittedFilter = domainUserFilter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var domainFilter = splittedFilter.First().Replace("Win32_UserAccount.", string.Empty);
            var userFilter = splittedFilter.Last();

            return new KeyValuePair<string, string>(userFilter, domainFilter);
        }

        private string BuildWqlToGetUsersFromGroup(string computerName, string groupName)
        {
             var groupComponentFilter = 
                string.Format(
                    "\\\\\\\\{0}\\\\root\\\\cimv2:Win32_Group.Domain=\"{0}\",Name=\"{1}\"",
                    computerName,
                    groupName); 

            return
                new WQLBuilder()
                    .WithWmiClass("Win32_GroupUser")
                    .AddParameter("GroupComponent", groupComponentFilter)
                .Build();
        }
    }

    public class WindowsAccount
    {
        public String Name { get; private set; }
        public Boolean? Enabled { get; private set; }
        public String AccountSID { get; private set; }
        public AccountType AccountType { get; private set; }
        public IEnumerable<WindowsAccount> Members { get; private set; }

        public WindowsAccount(string name, bool? enabled, string accountSID, AccountType type)
        {
            this.Name = name;
            this.Enabled = enabled;// ?? true;
            this.AccountSID = accountSID;
            this.AccountType = type;
        }

        public void AddMember(string accountName, bool? accountEnabled, string accountSID)
        {
            if (this.Members == null)
                this.Members = new List<WindowsAccount>();

            if (this.HasMember(accountName))
                return;

            var newMember = new WindowsAccount(accountName, accountEnabled, accountSID, GetMemberAccount());
            ((List<WindowsAccount>)this.Members).Add(newMember);
        }

        private Boolean HasMember(string memberName)
        {
            if (Members == null)
                return false;

            return
                Members
                    .Where(m => m.Name.Equals(memberName, StringComparison.InvariantCultureIgnoreCase))
                    .Count() > 0;
        }

        private AccountType GetMemberAccount()
        {
            if (this.AccountType == Providers.AccountType.User)
                return Providers.AccountType.Group;

            return Providers.AccountType.User;
        }
    }

    public class InvalidUsernameFormat : Exception 
    {
        public InvalidUsernameFormat(string givenUsername)
            : base(String.Format("Invalid format for give username: '{0}'", givenUsername)) { }
    }

    public class WindowsUserNotFound : Exception
    {
        public WindowsUserNotFound(string givenUsername)
            : base(String.Format("User '{0}' was not found in target system", givenUsername)) { }
    }
}
