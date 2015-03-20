/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
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
 * */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.Helpers;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace Modulo.Collect.Probe.Windows
{
    /// <summary>
    /// A provider to get object (local or remote) ACLs.
    /// </summary>
    public class AccessControlListProvider
    {
        private const string MSG_ERROR_GETTING_EFFECTIVE_RIGHTS = "An error occurred while trying to get effective rights for registry key '{0}' on {1}: '{2}'.";

        #region imports
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
        int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto,SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto,SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr existingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, ref IntPtr duplicateTokenHandle);
        #endregion
        
        #region logon consts
        // logon types 
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;        
        
        // logon providers 
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_PROVIDER_WINNT50 = 3;
        const int LOGON32_PROVIDER_WINNT40 = 2;
        const int LOGON32_PROVIDER_WINNT35 = 1;
        #endregion 

        private static AccessControlListProvider Instance;

        private Dictionary<string, bool> TargetRegistryKeyUsers;

        public AccessControlListProvider()
        {
            this.TargetRegistryKeyUsers = new Dictionary<string, bool>();
        }

        public static AccessControlListProvider CreateInstance()
        {
            if (Instance == null)
                Instance = new AccessControlListProvider();
            
            return Instance;
        }
        
        /// <summary>
        /// Returns the ACL Access Mask for a user on registry key.
        /// </summary>
        /// <param name="targetInfo">The target info object. For Windows Authentication, the credentials must be null or empty.</param>
        /// <param name="hive">The hive of registry key.</param>
        /// <param name="registryKey">The path of registry key without hive value.</param>
        /// <param name="userSID">The unique user security identifier.</param>
        /// <returns>Access Mask as unassigned integer.</returns>
        public virtual uint GetRegistryKeyEffectiveRights(
            TargetInfo targetInfo, RegistryHive hive, string registryKey, string userSID)
        {
            IntPtr token = LogOn(targetInfo);
            var person = WindowsIdentity.Impersonate(token);
            
            var result = this.GetUserDaclForRegistryKeyOnTarget(
                targetInfo, hive.ToString(), registryKey, userSID);
            
            person.Undo();
            return result;
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="regHive"></param>
        /// <param name="regKey"></param>
        /// <param name="userSID"></param>
        /// <returns></returns>
        public virtual bool IsThereDACLOnRegistryKeyForUser(
            TargetInfo targetInfo, string registryHive, string registryKey, string userSID)
        {
            var queryHash = GetHashCodeForQuery(targetInfo.GetAddress(), registryHive, registryKey, userSID);
            if (this.TargetRegistryKeyUsers.ContainsKey(queryHash))
                return this.TargetRegistryKeyUsers[queryHash];

            IntPtr token = LogOn(targetInfo);
            var person = WindowsIdentity.Impersonate(token);
            this.TargetRegistryKeyUsers.Add(queryHash, true);
            try
            {
                GetUserDaclForRegistryKeyOnTarget(targetInfo, registryHive, registryKey, userSID);
                return true;
            }
            catch (UserDACLNotFoundOnRegistryException)
            {
                this.TargetRegistryKeyUsers[queryHash] = false;
                return false;
            }
            finally
            {
                person.Undo();
            }
        }

        private string GetHashCodeForQuery(string targetAddress, string hive, string regKey, string userSID)
        {
            return String.Format("{0};{1};{2};{3}", targetAddress, hive, regKey, userSID);
        }
    
        private IntPtr LogOn(TargetInfo targetInfo)
        {
            IntPtr token = IntPtr.Zero;
            var logonMode = LOGON32_LOGON_NEW_CREDENTIALS; // LOGON32_LOGON_INTERACTIVE
            // Getting Credentials...
            var username = targetInfo.credentials.GetUserName();
            var password = targetInfo.credentials.GetPassword();
            var domain = targetInfo.credentials.GetDomain();
            if (string.IsNullOrWhiteSpace(domain))
                domain = targetInfo.GetAddress();


            bool isSuccess = LogonUser(username, domain, password, logonMode, LOGON32_PROVIDER_DEFAULT, ref token);
            if (!isSuccess)
                throw new WinLogonAccessDenied(targetInfo.GetAddress());
            return token;
        }

        public virtual Dictionary<string, uint> GetRegKeyDACLs(TargetInfo targetInfo, string regHive, string regKey)
        {
            IntPtr token = LogOn(targetInfo);
            var person = WindowsIdentity.Impersonate(token);
            RegistryKey remoteKey = null;
            try
            {
                remoteKey = this.TryToOpenRemoteKey(targetInfo, regHive, regKey);
            }
            finally
            {
                person.Undo();
            }

            try
            {
                var regKeyAccessControl = remoteKey.GetAccessControl(AccessControlSections.Access);
                var typeOfSID = typeof(System.Security.Principal.SecurityIdentifier);
                var DACLs = regKeyAccessControl.GetAccessRules(true, true, typeOfSID);

                var result = new Dictionary<string, uint>();
                foreach (RegistryAccessRule dacl in DACLs)
                {
                    var userDACL = (uint)dacl.RegistryRights;
                    if (userDACL <= Int32.MaxValue)
                    {
                        var userSid = dacl.IdentityReference.Value;
                        if (result.ContainsKey(userSid))
                            result[userSid] += userDACL;
                        else
                            result.Add(userSid, userDACL);
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occurred while trying to get effective rights on target '{0}':\r\n'{1}'", targetInfo.GetAddress(), ex.Message));
            }
            //finally
            //{
            //    person.Undo();
            //}          
        }

        private uint GetUserDaclForRegistryKeyOnTarget(TargetInfo address, string regHive, string regKey, string userSID)
        {
            RegistryKey remoteKey = this.TryToOpenRemoteKey(address, regHive, regKey);

            AuthorizationRuleCollection DACLs = null;
            try
            {
                DACLs = remoteKey.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));
            }
            catch (ArgumentException)
            {
                //throw new Exception(string.Format("An error occurred while trying to get effective rights on target '{0}':\r\n'{1}'", address, ex.Message));
                throw new Exception(string.Format("User \"{0}\" do not have access to the registry key permissions", address.credentials.GetFullyQualifiedUsername()));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occurred while trying to get effective rights on target \"{0}\":\r\n'{1}'", address, ex.Message));
            }
            
            uint userDACL = this.GetUserDACLfromAllRegistryKeyDACLs(DACLs, userSID);
            if (userDACL == 0)
                this.RaiseDACLNotFoundException(address.GetAddress(), userSID, regHive.ToString(), regKey);
            
            return userDACL;
        }

        private uint GetUserDACLfromAllRegistryKeyDACLs(AuthorizationRuleCollection DACLs, string userSID)
        {
            foreach (RegistryAccessRule dacl in DACLs)
                if (dacl.IdentityReference.Value.Equals(userSID))
                    return (uint)dacl.RegistryRights;

            return 0;
        }

        private RegistryKey TryToOpenRemoteKey(TargetInfo targetInfo, string registryHiveAsString, string registryKey)
        {
            var keyPath = this.CombineHiveAndKey(registryHiveAsString, registryKey);

            RegistryKey registryHive = null;
            try
            {
                var hive = (RegistryHive)Enum.Parse(typeof(RegistryHive), registryHiveAsString);
                var isLocal = targetInfo.GetAddress().Equals("127.0.0.1") || targetInfo.GetAddress().Trim().ToLower().Equals("localhost");
                registryHive = RegistryKey.OpenRemoteBaseKey(hive, isLocal ? string.Empty : targetInfo.GetAddress());
                //RegistrySecurity rs = registryHive.GetAccessControl(AccessControlSections.Access);
                //rs.SetAccessRuleProtection(true, true); //this line you need to set  ACL on a remote machines registry key.
            }
            catch (UnauthorizedAccessException)
            {
                throw new RegistryKeyEffectiveRightsAccessDenied(targetInfo.GetAddress(), keyPath);
            }

            
            var remoteKey = string.IsNullOrEmpty(registryKey) ? registryHive : registryHive.OpenSubKey(registryKey);
            if (remoteKey == null)
                throw new RegistryKeyEffectiveRightsNotFoundException(targetInfo.GetAddress(), keyPath);

            return remoteKey;
        }

        private string CombineHiveAndKey(string hive, string key)
        {
            return string.Format("{0}\\{1}", hive, key);
        }

        private void RaiseDACLNotFoundException(string address, string userSID, string hiveName, string registryKey)
        {
            string completeKeyName = string.Format(@"{0}\{1}", hiveName, registryKey);
            throw new UserDACLNotFoundOnRegistryException(userSID, completeKeyName, address);
        }

        private void ConnectToTarget(TargetInfo targetInfo)
        {
            if (targetInfo.IsLocalTarget())
                return;
            
            try
            {
                string remoteUNC = string.Format(@"\\{0}", targetInfo.GetAddress());
                string userName = targetInfo.IsThereCredential() ? targetInfo.credentials.GetFullyQualifiedUsername() : string.Empty;
                string password = string.IsNullOrEmpty(userName) ? string.Empty : targetInfo.credentials.GetPassword();

                WinNetUtils.connectToRemote(remoteUNC, userName, password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class WinLogonAccessDenied : Exception
    {
        const string MSG_WINLOGON_ACCESS_DENIED = "It was not possible to log on '{0}'. The remote machine did not accept the connection attempt.";

        public WinLogonAccessDenied(string machineName)
            : base(string.Format(MSG_WINLOGON_ACCESS_DENIED, machineName)) { }
    }

    public class RegistryKeyEffectiveRightsAccessDenied : Exception
    {
        const string MSG_REGKEY_ACCESS_DENIED= "The access to registry key which path '{0}' is denied on '{1}'.";

        public RegistryKeyEffectiveRightsAccessDenied(string machineName, string registryKeyPath)
            : base(string.Format(MSG_REGKEY_ACCESS_DENIED, registryKeyPath, machineName)) { }
    }

    public class RegistryKeyEffectiveRightsNotFoundException : Exception
    {
        private const string MSG_REGKEY_NOT_FOUND = "The registry key which path '{0}' was not found on '{1}'.";

        public RegistryKeyEffectiveRightsNotFoundException(string machineName, string registryKeyPath)
            :base(string.Format(MSG_REGKEY_NOT_FOUND, registryKeyPath, machineName)) { }
    }

    public class UserDACLNotFoundOnRegistryException : Exception
    {
        private const string MSG_DACL_NOT_FOUND = "There is no DACL for user '{0}' on registry key '{1}' at '{2}'";

        public UserDACLNotFoundOnRegistryException(string trustee, string registryKey, string hostname)
            : base(string.Format(MSG_DACL_NOT_FOUND, trustee, registryKey, hostname)) { }
    }
}
