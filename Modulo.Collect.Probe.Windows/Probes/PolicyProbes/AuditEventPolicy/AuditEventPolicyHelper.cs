/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows.AuditEventPolicy
{
    public class AuditEventPolicyHelper
    {
        #region Consts and Enums
        const int RESOURCE_CONNECTED = 0x00000001;
        const int RESOURCE_GLOBALNET = 0x00000002;
        const int RESOURCE_REMEMBERED = 0x00000003;

        const int RESOURCETYPE_ANY = 0x00000000;
        const int RESOURCETYPE_DISK = 0x00000001;
        const int RESOURCETYPE_PRINT = 0x00000002;

        const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
        const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
        const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;

        const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        const int RESOURCEUSAGE_CONTAINER = 0x00000002;

        const int CONNECT_INTERACTIVE = 0x00000008;
        const int CONNECT_PROMPT = 0x00000010;
        const int CONNECT_REDIRECT = 0x00000080;
        const int CONNECT_UPDATE_PROFILE = 0x00000001;
        const int CONNECT_COMMANDLINE = 0x00000800;
        const int CONNECT_CMD_SAVECRED = 0x00001000;
        const int CONNECT_LOCALDRIVE = 0x00000100;

        public const int POLICY_VIEW_LOCAL_INFORMATION = 0x00000001;
        public const int POLICY_VIEW_AUDIT_INFORMATION = 0x00000002;
        public const int POLICY_GET_PRIVATE_INFORMATION = 0x00000004;
        public const int POLICY_TRUST_ADMIN = 0x00000008;
        public const int POLICY_CREATE_ACCOUNT = 0x00000010;
        public const int POLICY_CREATE_SECRET = 0x00000020;
        public const int POLICY_CREATE_PRIVILEGE = 0x00000040;
        public const int POLICY_SET_DEFAULT_QUOTA_LIMITS = 0x00000080;
        public const int POLICY_SET_AUDIT_REQUIREMENTS = 0x00000100;
        public const int POLICY_AUDIT_LOG_ADMIN = 0x00000200;
        public const int POLICY_SERVER_ADMIN = 0x00000400;
        public const int POLICY_LOOKUP_NAMES = 0x00000800;
        public const int DELETE = 0x00010000;
        public const int READ_CONTROL = 0x00020000;
        public const int WRITE_DAC = 0x00040000;
        public const int WRITE_OWNER = 0x00080000;
        public const int SYNCHRONIZE = 0x00100000;
        public const int STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const int STANDARD_RIGHTS_READ = READ_CONTROL;
        public const int STANDARD_RIGHTS_WRITE = READ_CONTROL;
        public const int STANDARD_RIGHTS_EXECUTE = READ_CONTROL;
        public const int STANDARD_RIGHTS_ALL = 0x001F0000;
        public const int SPECIFIC_RIGHTS_ALL = 0x0000FFFF;

        public const int POLICY_AUDIT_EVENT_UNCHANGED = 0x00000000;
        public const int POLICY_AUDIT_EVENT_SUCCESS = 0x00000001;
        public const int POLICY_AUDIT_EVENT_FAILURE = 0x00000002;
        public const int POLICY_AUDIT_EVENT_NONE = 0x00000004;

        public enum POLICY_INFORMATION_CLASS
        {
            PolicyAuditLogInformation = 1,
            PolicyAuditEventsInformation,
            PolicyPrimaryDomainInformation,
            PolicyPdAccountInformation,
            PolicyAccountDomainInformation,
            PolicyLsaServerRoleInformation,
            PolicyReplicaSourceInformation,
            PolicyDefaultQuotaInformation,
            PolicyModificationInformation,
            PolicyAuditFullSetInformation,
            PolicyAuditFullQueryInformation,
            PolicyDnsDomainInformation
        }

        public enum POLICY_AUDIT_EVENT_TYPE
        {
            AuditCategorySystem,
            AuditCategoryLogon,
            AuditCategoryObjectAccess,
            AuditCategoryPrivilegeUse,
            AuditCategoryDetailedTracking,
            AuditCategoryPolicyChange,
            AuditCategoryAccountManagement,
            AuditCategoryDirectoryServiceAccess,
            AuditCategoryAccountLogon
        }

        #endregion

        #region Remote Connection Imports
        [DllImport("Mpr.dll")]
        private static extern int WNetUseConnection(
            IntPtr hwndOwner,
            NETRESOURCE lpNetResource,
            string lpPassword,
            string lpUserID,
            int dwFlags,
            string lpAccessName,
            string lpBufferSize,
            string lpResult
            );

        [DllImport("Mpr.dll")]
        private static extern int WNetCancelConnection2(
            string lpName,
            int dwFlags,
            bool fForce
            );

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public int dwScope = 0;
            public int dwType = 0;
            public int dwDisplayType = 0;
            public int dwUsage = 0;
            public string lpLocalName = "";
            public string lpRemoteName = "";
            public string lpComment = "";
            public string lpProvider = "";
        }

        #endregion

        #region Policy Imports
        [DllImport("netapi32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern uint NetUserModalsGet(
            string server,
            int level,
            out IntPtr BufPtr
        );

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern uint NetApiBufferFree(
          IntPtr bufptr
        );

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_MODALS_INFO_0
        {
            public uint usrmod0_min_passwd_len;
            public uint usrmod0_max_passwd_age;
            public uint usrmod0_min_passwd_age;
            public uint usrmod0_force_logoff;
            public uint usrmod0_password_hist_len;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_MODALS_INFO_1
        {
            public uint usrmod1_role;
            public string usrmod1_primary;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_MODALS_INFO_2
        {
            public string usrmod2_domain_name;
            public IntPtr usrmod2_domain_id;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_MODALS_INFO_3
        {
            public uint usrmod3_lockout_duration;
            public uint usrmod3_lockout_observation_window;
            public uint usrmod3_lockout_threshold;
        };

        #endregion

        #region LSA Imports
        [DllImport("kernel32.dll")]
        extern static int GetLastError();

        //[DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        //public static extern UInt32 LsaNtStatusToWinError(UInt32 Status);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern UInt32 LsaNtStatusToWinError([In] UInt32 status);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        public static extern bool ConvertStringSidToSid(
            string StringSid, out IntPtr pSid);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        public static extern bool LookupAccountName(
            string lpSystemName, string lpAccountName,
            IntPtr psid, ref int cbsid,
            StringBuilder domainName, ref int cbdomainLength,
            ref int use);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern UInt32 LsaOpenPolicy(
            ref LSA_UNICODE_STRING SystemName,
            ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            Int32 DesiredAccess,
            out IntPtr PolicyHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern UInt32 LsaClose(IntPtr PolicyHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern UInt32 LsaFreeMemory(IntPtr Buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern void AuditFree(IntPtr Buffer);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern UInt32 LsaEnumerateAccountRights(
            IntPtr PolicyHandle, IntPtr AccountSid,
            out /* LSA_UNICODE_STRING[] */ IntPtr UserRights,
            out UInt32 CountOfRights);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern UInt32 LsaQueryInformationPolicy(
            IntPtr PolicyHandle, POLICY_INFORMATION_CLASS InformationClass,
            out IntPtr Buffer);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern bool AuditLookupCategoryGuidFromCategoryId(
            POLICY_AUDIT_EVENT_TYPE AuditCategoryId,
            IntPtr pAuditCategoryGuid);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern bool AuditEnumerateSubCategories(
            IntPtr pAuditCategoryGuid,
            bool bRetrieveAllSubCategories,
            ref IntPtr pAuditSubCategoriesArray,
            out UInt32 pCountReturned);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern bool AuditQuerySystemPolicy(
            IntPtr pSubCategoryGuids,
            UInt32 PolicyCount,
            out IntPtr pAuditPolicies);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern bool AuditLookupSubCategoryName(
            IntPtr pAuditSubCategoryGuid,   //  GUID *
            ref String ppszSubCategoryName
        );

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern bool AuditLookupCategoryName(
            IntPtr pAuditCategoryGuid,   //  GUID *
            ref String ppszCategoryName
        );

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LSA_UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LSA_OBJECT_ATTRIBUTES
        {
            public IntPtr RootDirectory;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
            public LSA_UNICODE_STRING ObjectName;
            public UInt32 Attributes;
            public UInt32 Length;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct POLICY_AUDIT_EVENTS_INFO
        {
            public bool AuditingMode;
            public IntPtr EventAuditingOptions;
            public UInt32 MaximumAuditEventCount;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct GUID
        {
            public UInt32 Data1;
            public UInt16 Data2;
            public UInt16 Data3;
            public Byte Data4a;
            public Byte Data4b;
            public Byte Data4c;
            public Byte Data4d;
            public Byte Data4e;
            public Byte Data4f;
            public Byte Data4g;
            public Byte Data4h;

            public override string ToString()
            {
                return Data1.ToString("x8") + "-" + Data2.ToString("x4") + "-" + Data3.ToString("x4") + "-"
                      + Data4a.ToString("x2") + Data4b.ToString("x2") + "-"
                      + Data4c.ToString("x2") + Data4d.ToString("x2") + Data4e.ToString("x2") + Data4f.ToString("x2") + Data4g.ToString("x2") + Data4h.ToString("x2");
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct AUDIT_POLICY_INFORMATION
        {
            public GUID AuditSubCategoryGuid;
            public UInt32 AuditingInformation;
            public GUID AuditCategoryGuid;
        }

        #endregion

        #region Subcategories Dictionary
        private Dictionary<string, AuditEventSubcategories> subcategoriesDictionary = new Dictionary<string, AuditEventSubcategories>()
        {  
            { "Account Lockout", AuditEventSubcategories.account_lockout },
            { "Application Generated", AuditEventSubcategories.application_generated },
            { "Application Group Management", AuditEventSubcategories.application_group_management },
            { "Audit Policy Change", AuditEventSubcategories.audit_policy_change },
            { "Authentication Policy Change", AuditEventSubcategories.authentication_policy_change },
            { "Authorization Policy Change", AuditEventSubcategories.authorization_policy_change },
            { "Certification Services", AuditEventSubcategories.certification_services },
            { "Computer Account Management", AuditEventSubcategories.computer_account_management },
            { "Credential Validation", AuditEventSubcategories.credential_validation },
            { "Detailed Directory Service Replication", AuditEventSubcategories.detailed_directory_service_replication },
            { "Directory Service Access", AuditEventSubcategories.directory_service_access },
            { "Directory Service Changes", AuditEventSubcategories.directory_service_changes },
            { "Directory Service Replication", AuditEventSubcategories.directory_service_replication },
            { "Distribution Group Management", AuditEventSubcategories.distribution_group_management },
            { "DPAPI Activity", AuditEventSubcategories.dpapi_activity },
            { "File Share", AuditEventSubcategories.file_share },
            { "File System", AuditEventSubcategories.file_system },
            { "Filtering Platform Connection", AuditEventSubcategories.filtering_platform_connection },
            { "Filtering Platform Packet Drop", AuditEventSubcategories.filtering_platform_packet_drop },
            { "Filtering Platform Policy Change", AuditEventSubcategories.filtering_platform_policy_change },
            { "Handle Manipulation", AuditEventSubcategories.handle_manipulation },
            { "IPsec Driver", AuditEventSubcategories.ipsec_driver },
            { "IPsec Extended Mode", AuditEventSubcategories.ipsec_extended_mode },
            { "IPsec Main Mode", AuditEventSubcategories.ipsec_main_mode },
            { "IPsec Quick Mode", AuditEventSubcategories.ipsec_quick_mode },
            { "Kerberos Service Ticket Operations", AuditEventSubcategories.kerberos_ticket_events },
            { "Kernel Object", AuditEventSubcategories.kernel_object },
            { "Logoff", AuditEventSubcategories.logoff },
            { "Logon", AuditEventSubcategories.logon },
            { "MPSSVC Rule-Level Policy Change", AuditEventSubcategories.mpssvc_rule_level_policy_change },
            { "Non Sensitive Privilege Use", AuditEventSubcategories.non_sensitive_privilege_use },
            { "Other Account Logon Events", AuditEventSubcategories.other_account_logon_events },
            { "Other Account Management Events", AuditEventSubcategories.other_account_management_events },
            { "Other Logon/Logoff Events", AuditEventSubcategories.other_logon_logoff_events },
            { "Other Object Access Events", AuditEventSubcategories.other_object_access_events },
            { "Other Policy Change Events", AuditEventSubcategories.other_policy_change_events },
            { "Other Privilege Use Events", AuditEventSubcategories.other_privilege_use_events },
            { "Other System Events", AuditEventSubcategories.other_system_events },
            { "Process Creation", AuditEventSubcategories.process_creation },
            { "Process Termination", AuditEventSubcategories.process_termination },
            { "Registry", AuditEventSubcategories.registry },
            { "RPC Events", AuditEventSubcategories.rpc_events },
            { "SAM", AuditEventSubcategories.sam },
            { "Security Group Management", AuditEventSubcategories.security_group_management },
            { "Security State Change", AuditEventSubcategories.security_state_change },
            { "Security System Extension", AuditEventSubcategories.security_system_extension },
            { "Sensitive Privilege Use", AuditEventSubcategories.sensitive_privilege_use },
            { "Special Logon", AuditEventSubcategories.special_logon },
            { "System Integrity", AuditEventSubcategories.system_integrity },
            { "User Account Management", AuditEventSubcategories.user_account_management }
        };
        #endregion

        public static LSA_UNICODE_STRING string2LSAUS(string myString)
        {
            LSA_UNICODE_STRING retStr = new LSA_UNICODE_STRING();
            retStr.Buffer = Marshal.StringToHGlobalUni(myString);
            retStr.Length = (UInt16)((myString.Length + 1) * UnicodeEncoding.CharSize);
            retStr.MaximumLength = retStr.Length;
            return retStr;
        }

        public static string LSAUS2string(LSA_UNICODE_STRING lsaus)
        {
            char[] cvt = new char[lsaus.Length / UnicodeEncoding.CharSize];
            Marshal.Copy(lsaus.Buffer, cvt, 0, lsaus.Length / UnicodeEncoding.CharSize);
            return new string(cvt);
        }

        public virtual Dictionary<AuditEventPolicies, AuditEventStatus> GetAuditEventPolicies(TargetInfo targetInfo)
        {
            Dictionary<AuditEventPolicies, AuditEventStatus> retList = new Dictionary<AuditEventPolicies, AuditEventStatus>();

            LSA_UNICODE_STRING systemName = string2LSAUS(targetInfo.GetAddress());
            LSA_OBJECT_ATTRIBUTES objAttrs = new LSA_OBJECT_ATTRIBUTES();

            IntPtr policyHandle = IntPtr.Zero;
            IntPtr pAuditEventsInfo = IntPtr.Zero;
            IntPtr pAuditCategoryGuid = IntPtr.Zero;
            IntPtr pAuditSubCategoryGuids = IntPtr.Zero;
            IntPtr pAuditPolicies = IntPtr.Zero;

            UInt32 lretVal = LsaOpenPolicy(ref systemName, ref objAttrs, POLICY_VIEW_AUDIT_INFORMATION, out policyHandle);
            uint retVal = LsaNtStatusToWinError(lretVal);
            if (retVal != 0)
            {
                throw new System.ComponentModel.Win32Exception((int)retVal);
            }

            try
            {
                lretVal = LsaQueryInformationPolicy(policyHandle, POLICY_INFORMATION_CLASS.PolicyAuditEventsInformation, out pAuditEventsInfo);
                retVal = LsaNtStatusToWinError(lretVal);
                if (retVal != 0)
                {
                    throw new System.ComponentModel.Win32Exception((int)retVal);
                }

                //  EventAuditingOptions: The index of each array element corresponds to an audit event type value in the POLICY_AUDIT_EVENT_TYPE enumeration type.
                //  Each POLICY_AUDIT_EVENT_OPTIONS variable in the array can specify the following auditing options. 
                //  POLICY_AUDIT_EVENT_UNCHANGED, POLICY_AUDIT_EVENT_SUCCESS, POLICY_AUDIT_EVENT_FAILURE, POLICY_AUDIT_EVENT_NONE
                POLICY_AUDIT_EVENTS_INFO myAuditEventsInfo = new POLICY_AUDIT_EVENTS_INFO();
                myAuditEventsInfo = (POLICY_AUDIT_EVENTS_INFO)Marshal.PtrToStructure(pAuditEventsInfo, myAuditEventsInfo.GetType());

                for (UInt32 policyAuditEventType = 0; policyAuditEventType < myAuditEventsInfo.MaximumAuditEventCount; policyAuditEventType++)
                {

                    pAuditCategoryGuid = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GUID)));
                    if (!AuditLookupCategoryGuidFromCategoryId((POLICY_AUDIT_EVENT_TYPE)policyAuditEventType, pAuditCategoryGuid))
                    {
                        int causingError = GetLastError();
                        throw new System.ComponentModel.Win32Exception(causingError);
                    }

                    String categoryName = String.Empty;
                    AuditLookupCategoryName(pAuditCategoryGuid, ref categoryName);

                    UInt32 status = 0;
                    IntPtr itemPtr = new IntPtr(myAuditEventsInfo.EventAuditingOptions.ToInt64() + (Int64)policyAuditEventType * (Int64)Marshal.SizeOf(typeof(UInt32)));
                    status = (UInt32)Marshal.PtrToStructure(itemPtr, status.GetType());
                    retList.Add((AuditEventPolicies)policyAuditEventType, (AuditEventStatus)(status & 0x3));

                    Marshal.FreeHGlobal(pAuditCategoryGuid);
                    pAuditCategoryGuid = IntPtr.Zero;
                }
            }
            finally
            {
                if (pAuditEventsInfo != IntPtr.Zero)
                {
                    LsaFreeMemory(pAuditEventsInfo);
                }

                if (pAuditCategoryGuid != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pAuditCategoryGuid);
                }

                LsaClose(policyHandle);
            }

            return retList;
        }

        public virtual Dictionary<AuditEventSubcategories, AuditEventStatus> GetAuditEventSubcategoriesPolicy(TargetInfo targetInfo)
        {
            Dictionary<AuditEventSubcategories, AuditEventStatus> retList = new Dictionary<AuditEventSubcategories, AuditEventStatus>();

            string target = @"\\" + targetInfo.GetAddress();
            LSA_UNICODE_STRING systemName = string2LSAUS(target);
            LSA_OBJECT_ATTRIBUTES objAttrs = new LSA_OBJECT_ATTRIBUTES();

            IntPtr policyHandle = IntPtr.Zero;
            IntPtr pAuditEventsInfo = IntPtr.Zero;
            IntPtr pAuditCategoryId = IntPtr.Zero;
            IntPtr pAuditSubCategoryGuids = IntPtr.Zero;
            IntPtr pAuditPolicies = IntPtr.Zero;

            UInt32 lretVal = LsaOpenPolicy(ref systemName, ref objAttrs, POLICY_VIEW_AUDIT_INFORMATION, out policyHandle);
            UInt32 retVal = LsaNtStatusToWinError(lretVal);

            if (retVal == (UInt32)0)
            {
                try
                {
                    lretVal = LsaQueryInformationPolicy(policyHandle, POLICY_INFORMATION_CLASS.PolicyAuditEventsInformation, out pAuditEventsInfo);
                    retVal = LsaNtStatusToWinError(lretVal);

                    if (retVal == 0)
                    {
                        POLICY_AUDIT_EVENTS_INFO myAuditEventsInfo = new POLICY_AUDIT_EVENTS_INFO();
                        myAuditEventsInfo = (POLICY_AUDIT_EVENTS_INFO)Marshal.PtrToStructure(pAuditEventsInfo, myAuditEventsInfo.GetType());

                        for (var policyAuditEventType = 0; policyAuditEventType < myAuditEventsInfo.MaximumAuditEventCount; policyAuditEventType++)
                        {
                            pAuditCategoryId = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GUID)));
                            if (!AuditLookupCategoryGuidFromCategoryId((POLICY_AUDIT_EVENT_TYPE)policyAuditEventType, pAuditCategoryId))
                            {
                                int causingError = GetLastError();
                                throw new System.ComponentModel.Win32Exception(causingError);
                            }

                            UInt32 nSubCats = 0;
                            pAuditSubCategoryGuids = IntPtr.Zero;
                            if (!AuditEnumerateSubCategories(pAuditCategoryId, false, ref pAuditSubCategoryGuids, out nSubCats))
                            {
                                int causingError = GetLastError();
                                throw new System.ComponentModel.Win32Exception(causingError);
                            }

                            Marshal.FreeHGlobal(pAuditCategoryId);
                            pAuditCategoryId = IntPtr.Zero;

                            pAuditPolicies = IntPtr.Zero;
                            if (nSubCats > 0)
                            {

                                if (!AuditQuerySystemPolicy(pAuditSubCategoryGuids, nSubCats, out pAuditPolicies))
                                {
                                    int causingError = GetLastError();
                                    throw new System.ComponentModel.Win32Exception(causingError);
                                }

                                for (var subcategoryIndex = 0; subcategoryIndex < nSubCats; subcategoryIndex++)
                                {
                                    AUDIT_POLICY_INFORMATION currentPolicy = new AUDIT_POLICY_INFORMATION();

                                    IntPtr itemPtr = new IntPtr(pAuditPolicies.ToInt64() + (Int64)subcategoryIndex * (Int64)Marshal.SizeOf(currentPolicy));
                                    currentPolicy = (AUDIT_POLICY_INFORMATION)Marshal.PtrToStructure(itemPtr, currentPolicy.GetType());

                                    String subCategoryName = String.Empty;
                                    Marshal.StructureToPtr(currentPolicy, itemPtr, true);
                                    AuditLookupSubCategoryName(itemPtr, ref subCategoryName);

                                    AuditEventSubcategories value;
                                    if (subcategoriesDictionary.TryGetValue(subCategoryName, out value))
                                    {
                                        retList.Add(value, (AuditEventStatus)(currentPolicy.AuditingInformation & 0x3));
                                    }
                                }

                                if (pAuditPolicies != IntPtr.Zero)
                                {
                                    AuditFree(pAuditPolicies);
                                    pAuditPolicies = IntPtr.Zero;
                                }
                            }

                            if (pAuditSubCategoryGuids != IntPtr.Zero)
                            {
                                AuditFree(pAuditSubCategoryGuids);
                                pAuditSubCategoryGuids = IntPtr.Zero;
                            }

                            nSubCats = 0;
                        }
                    }
                    else
                    {
                        throw new System.ComponentModel.Win32Exception((int)retVal);
                    }
                }
                finally
                {
                    if (pAuditPolicies != IntPtr.Zero)
                    {
                        AuditFree(pAuditPolicies);
                        pAuditPolicies = IntPtr.Zero;
                    }

                    if (pAuditSubCategoryGuids != IntPtr.Zero)
                    {
                        AuditFree(pAuditSubCategoryGuids);
                        pAuditSubCategoryGuids = IntPtr.Zero;
                    }

                    if (pAuditEventsInfo != IntPtr.Zero)
                    {
                        LsaFreeMemory(pAuditEventsInfo);
                    }

                    if (pAuditCategoryId != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pAuditCategoryId);
                    }

                    LsaClose(policyHandle);
                }
            }
            else
            {
                throw new System.ComponentModel.Win32Exception((int)retVal);
            }

            return retList;
        }

        public TargetInfo TargetInfo { get; private set; }

        public AuditEventPolicyHelper(TargetInfo targetInfo)
        {
            this.TargetInfo = targetInfo;
        }
    }

    public enum AuditEventStatus
    {
        AUDIT_NONE = 0,
        AUDIT_SUCCESS = 1,
        AUDIT_FAILURE = 2,
        AUDIT_SUCCESS_FAILURE = 3,
        EMPTY = 8
    }

    public enum AuditEventPolicies
    {
        system = 0,
        account_logon = 1,
        object_access = 2,
        privilege_use = 3,
        detailed_tracking = 4,
        policy_change = 5,
        account_management = 6,
        directory_service_access = 7,
        logon = 8
    }

    public enum AuditEventSubcategories
    {
        account_lockout,
        application_generated,
        application_group_management,
        audit_policy_change,
        authentication_policy_change,
        authorization_policy_change,
        certification_services,
        computer_account_management,
        credential_validation,
        detailed_directory_service_replication,
        directory_service_access,
        directory_service_changes,
        directory_service_replication,
        distribution_group_management,
        dpapi_activity,
        file_share,
        file_system,
        filtering_platform_connection,
        filtering_platform_packet_drop,
        filtering_platform_policy_change,
        handle_manipulation,
        ipsec_driver,
        ipsec_extended_mode,
        ipsec_main_mode,
        ipsec_quick_mode,
        kerberos_ticket_events,
        kernel_object,
        logoff,
        logon,
        mpssvc_rule_level_policy_change,
        non_sensitive_privilege_use,
        other_account_logon_events,
        other_account_management_events,
        other_logon_logoff_events,
        other_object_access_events,
        other_policy_change_events,
        other_privilege_use_events,
        other_system_events,
        process_creation,
        process_termination,
        registry,
        rpc_events,
        sam,
        security_group_management,
        security_state_change,
        security_system_extension,
        sensitive_privilege_use,
        special_logon,
        system_integrity,
        user_account_management
    }
}
