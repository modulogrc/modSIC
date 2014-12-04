/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using System.Diagnostics;
using System.IO;

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
            var policyData = GetPolAdtEv(targetInfo);
            var structure = GetPositionValue(policyData, 8);
            Dictionary<AuditEventSubcategories, int> positionsMap;

            switch (structure)
            {
                // Windows 2008 x86 & Vista
                case 0x76:
                    positionsMap = GetStructure76SubcategoriesPositions();
                    break;
                // Windows 7 & 2008 x64
                case 0x78:
                    positionsMap = GetStructure78SubcategoriesPositions();
                    break;
                // Windows 8
                case 0x7E:
                    positionsMap = GetStructure7ESubcategoriesPositions();
                    break;
                default:
                    throw new Exception("Unexpected subcategories data");
            }

            return GetSubcategoriesStatus(policyData, positionsMap);
        }

        private string GetPolAdtEv(TargetInfo targetInfo)
        {
            string build = @"QUERY \\" + targetInfo.GetAddress() + @"\HKLM\Security\Policy\PolAdtEv\";
            string parms = @build;
            string output = string.Empty;
            string error = string.Empty;

            ProcessStartInfo psi = new ProcessStartInfo("reg.exe", parms);

            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            var reg = Process.Start(psi);
            using (StreamReader myOutput = reg.StandardOutput)
            {
                output = myOutput.ReadToEnd();
            }
            using (StreamReader myError = reg.StandardError)
            {
                error = myError.ReadToEnd();
            }

            if (error != string.Empty)
                throw new Exception(error);

            output = output.Replace(@"HKEY_LOCAL_MACHINE\Security\Policy\PolAdtEv", string.Empty);
            output = output.Replace("(Default)", string.Empty);
            output = output.Replace("REG_NONE", string.Empty);
            output = output.Replace(Environment.NewLine, string.Empty);

            return output.Trim();
        }

        private short GetPositionValue(string policyData, int position)
        {
            const int chunkSize = 2;
            var strVal = policyData.Substring(position * chunkSize, chunkSize);
            return Convert.ToInt16(strVal, 16);
        }

        private Dictionary<AuditEventSubcategories, int> GetStructure76SubcategoriesPositions()
        {
            return new Dictionary<AuditEventSubcategories, int>
            {
                { AuditEventSubcategories.security_state_change, 12 },
                { AuditEventSubcategories.security_system_extension, 14 },
                { AuditEventSubcategories.system_integrity, 16 },
                { AuditEventSubcategories.ipsec_driver, 18 },
                { AuditEventSubcategories.other_system_events, 20 },
                { AuditEventSubcategories.logon, 22 },
                { AuditEventSubcategories.logoff, 24 },
                { AuditEventSubcategories.account_lockout, 26 },
                { AuditEventSubcategories.ipsec_main_mode, 28 },
                { AuditEventSubcategories.special_logon, 30 },
                { AuditEventSubcategories.ipsec_quick_mode, 32 },
                { AuditEventSubcategories.ipsec_extended_mode, 34 },
                { AuditEventSubcategories.other_logon_logoff_events, 36 },
                { AuditEventSubcategories.network_policy_server, 38 },
                { AuditEventSubcategories.file_system, 40 },
                { AuditEventSubcategories.registry, 42 },
                { AuditEventSubcategories.kernel_object, 44 },
                { AuditEventSubcategories.sam, 46 },
                { AuditEventSubcategories.other_object_access_events, 48 },
                { AuditEventSubcategories.certification_services, 50 },
                { AuditEventSubcategories.application_generated, 52 },
                { AuditEventSubcategories.handle_manipulation, 54 },
                { AuditEventSubcategories.file_share, 56 },
                { AuditEventSubcategories.filtering_platform_packet_drop, 58 },
                { AuditEventSubcategories.filtering_platform_connection, 60 },

                { AuditEventSubcategories.sensitive_privilege_use, 62 },
                { AuditEventSubcategories.non_sensitive_privilege_use, 64 },
                { AuditEventSubcategories.other_privilege_use_events, 66 },
                { AuditEventSubcategories.process_creation, 68 },
                { AuditEventSubcategories.process_termination, 70 },
                { AuditEventSubcategories.dpapi_activity, 72 },
                { AuditEventSubcategories.rpc_events, 74 },
                { AuditEventSubcategories.audit_policy_change, 76 },
                { AuditEventSubcategories.authentication_policy_change, 78 },
                { AuditEventSubcategories.authorization_policy_change, 80 },
                { AuditEventSubcategories.mpssvc_rule_level_policy_change, 82 },
                { AuditEventSubcategories.filtering_platform_policy_change, 84 },
                { AuditEventSubcategories.other_policy_change_events, 86 },
                { AuditEventSubcategories.user_account_management, 88 },
                { AuditEventSubcategories.computer_account_management, 90 },
                { AuditEventSubcategories.security_group_management, 92 },
                { AuditEventSubcategories.distribution_group_management, 94 },
                { AuditEventSubcategories.application_group_management, 96 },
                { AuditEventSubcategories.other_account_management_events, 98 },
                { AuditEventSubcategories.directory_service_access, 100 },
                { AuditEventSubcategories.directory_service_changes, 102 },
                { AuditEventSubcategories.directory_service_replication, 104 },
                { AuditEventSubcategories.detailed_directory_service_replication, 106 },
                { AuditEventSubcategories.credential_validation, 108 },
                { AuditEventSubcategories.kerberos_service_ticket_operations, 110 },
                { AuditEventSubcategories.other_account_logon_events, 112 },
                { AuditEventSubcategories.kerberos_authentication_service, 114 },
            };
        }

        private Dictionary<AuditEventSubcategories, int> GetStructure78SubcategoriesPositions()
        {
            return new Dictionary<AuditEventSubcategories, int>
            {
                { AuditEventSubcategories.security_state_change, 12 },
                { AuditEventSubcategories.security_system_extension, 14 },
                { AuditEventSubcategories.system_integrity, 16 },
                { AuditEventSubcategories.ipsec_driver, 18 },
                { AuditEventSubcategories.other_system_events, 20 },
                { AuditEventSubcategories.logon, 22 },
                { AuditEventSubcategories.logoff, 24 },
                { AuditEventSubcategories.account_lockout, 26 },
                { AuditEventSubcategories.ipsec_main_mode, 28 },
                { AuditEventSubcategories.special_logon, 30 },
                { AuditEventSubcategories.ipsec_quick_mode, 32 },
                { AuditEventSubcategories.ipsec_extended_mode, 34 },
                { AuditEventSubcategories.other_logon_logoff_events, 36 },
                { AuditEventSubcategories.network_policy_server, 38 },
                { AuditEventSubcategories.file_system, 40 },
                { AuditEventSubcategories.registry, 42 },
                { AuditEventSubcategories.kernel_object, 44 },
                { AuditEventSubcategories.sam, 46 },
                { AuditEventSubcategories.other_object_access_events, 48 },
                { AuditEventSubcategories.certification_services, 50 },
                { AuditEventSubcategories.application_generated, 52 },
                { AuditEventSubcategories.handle_manipulation, 54 },
                { AuditEventSubcategories.file_share, 56 },
                { AuditEventSubcategories.filtering_platform_packet_drop, 58 },
                { AuditEventSubcategories.filtering_platform_connection, 60 },

                { AuditEventSubcategories.detailed_file_share, 62 },

                { AuditEventSubcategories.sensitive_privilege_use, 64 },
                { AuditEventSubcategories.non_sensitive_privilege_use, 66 },
                { AuditEventSubcategories.other_privilege_use_events, 68 },
                { AuditEventSubcategories.process_creation, 70 },
                { AuditEventSubcategories.process_termination, 72 },
                { AuditEventSubcategories.dpapi_activity, 74 },
                { AuditEventSubcategories.rpc_events, 76 },
                { AuditEventSubcategories.audit_policy_change, 78 },
                { AuditEventSubcategories.authentication_policy_change, 80 },
                { AuditEventSubcategories.authorization_policy_change, 82 },
                { AuditEventSubcategories.mpssvc_rule_level_policy_change, 84 },
                { AuditEventSubcategories.filtering_platform_policy_change, 86 },
                { AuditEventSubcategories.other_policy_change_events, 88 },
                { AuditEventSubcategories.user_account_management, 90 },
                { AuditEventSubcategories.computer_account_management, 92 },
                { AuditEventSubcategories.security_group_management, 94 },
                { AuditEventSubcategories.distribution_group_management, 96 },
                { AuditEventSubcategories.application_group_management, 98 },
                { AuditEventSubcategories.other_account_management_events, 100 },
                { AuditEventSubcategories.directory_service_access, 102 },
                { AuditEventSubcategories.directory_service_changes, 104 },
                { AuditEventSubcategories.directory_service_replication, 106 },
                { AuditEventSubcategories.detailed_directory_service_replication, 108 },
                { AuditEventSubcategories.credential_validation, 110 },
                { AuditEventSubcategories.kerberos_service_ticket_operations, 112 },
                { AuditEventSubcategories.other_account_logon_events, 114 },
                { AuditEventSubcategories.kerberos_authentication_service, 116 },
            };
        }

        private Dictionary<AuditEventSubcategories, int> GetStructure7ESubcategoriesPositions()
        {
            return new Dictionary<AuditEventSubcategories, int>
            {
                { AuditEventSubcategories.security_state_change, 12 },
                { AuditEventSubcategories.security_system_extension, 14 },
                { AuditEventSubcategories.system_integrity, 16 },
                { AuditEventSubcategories.ipsec_driver, 18 },
                { AuditEventSubcategories.other_system_events, 20 },
                { AuditEventSubcategories.logon, 22 },
                { AuditEventSubcategories.logoff, 24 },
                { AuditEventSubcategories.account_lockout, 26 },
                { AuditEventSubcategories.ipsec_main_mode, 28 },
                { AuditEventSubcategories.special_logon, 30 },
                { AuditEventSubcategories.ipsec_quick_mode, 32 },
                { AuditEventSubcategories.ipsec_extended_mode, 34 },
                { AuditEventSubcategories.other_logon_logoff_events, 36 },
                { AuditEventSubcategories.network_policy_server, 38 },
                // 40: User / Device Claims (Win 8)
                { AuditEventSubcategories.file_system, 42 },
                { AuditEventSubcategories.registry, 44 },
                { AuditEventSubcategories.kernel_object, 46 },
                { AuditEventSubcategories.sam, 48 },
                { AuditEventSubcategories.other_object_access_events, 50 },
                { AuditEventSubcategories.certification_services, 52 },
                { AuditEventSubcategories.application_generated, 54 },
                { AuditEventSubcategories.handle_manipulation, 56 },
                { AuditEventSubcategories.file_share, 58 },
                { AuditEventSubcategories.filtering_platform_packet_drop, 60 },
                { AuditEventSubcategories.filtering_platform_connection, 62 },
                { AuditEventSubcategories.detailed_file_share, 64 },
                // 66: Removable Storage (Win 8)
                // 68: Central Access Policy Staging (Win 8)
                { AuditEventSubcategories.sensitive_privilege_use, 70 },
                { AuditEventSubcategories.non_sensitive_privilege_use, 72 },
                { AuditEventSubcategories.other_privilege_use_events, 74 },
                { AuditEventSubcategories.process_creation, 76 },
                { AuditEventSubcategories.process_termination, 78 },
                { AuditEventSubcategories.dpapi_activity, 80 },
                { AuditEventSubcategories.rpc_events, 82 },
                { AuditEventSubcategories.audit_policy_change, 84 },
                { AuditEventSubcategories.authentication_policy_change, 86 },
                { AuditEventSubcategories.authorization_policy_change, 88 },
                { AuditEventSubcategories.mpssvc_rule_level_policy_change, 90 },
                { AuditEventSubcategories.filtering_platform_policy_change, 92 },
                { AuditEventSubcategories.other_policy_change_events, 94 },
                { AuditEventSubcategories.user_account_management, 96 },
                { AuditEventSubcategories.computer_account_management, 98 },
                { AuditEventSubcategories.security_group_management, 100 },
                { AuditEventSubcategories.distribution_group_management, 102 },
                { AuditEventSubcategories.application_group_management, 104 },
                { AuditEventSubcategories.other_account_management_events, 106 },
                { AuditEventSubcategories.directory_service_access, 108 },
                { AuditEventSubcategories.directory_service_changes, 110 },
                { AuditEventSubcategories.directory_service_replication, 112 },
                { AuditEventSubcategories.detailed_directory_service_replication, 114 },
                { AuditEventSubcategories.credential_validation, 116 },
                { AuditEventSubcategories.kerberos_service_ticket_operations, 118 },
                { AuditEventSubcategories.other_account_logon_events, 120 },
                { AuditEventSubcategories.kerberos_authentication_service, 122 },
            };
        }

        public Dictionary<AuditEventSubcategories, AuditEventStatus> GetSubcategoriesStatus(string policyData, Dictionary<AuditEventSubcategories, int> positionsMap)
        {
            var items = new Dictionary<AuditEventSubcategories, AuditEventStatus>();

            foreach (var positionItem in positionsMap)
            {
                var status = (AuditEventStatus) GetPositionValue(policyData, positionItem.Value);
                items.Add(positionItem.Key, status);
            }

            return items;
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
        user_account_management,
        detailed_file_share,
        network_policy_server,
        kerberos_authentication_service,
        kerberos_service_ticket_operations,
    }
}
