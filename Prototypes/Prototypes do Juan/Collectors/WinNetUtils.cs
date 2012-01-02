using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text.RegularExpressions;

namespace FrameworkNG
{
    public static class WinNetUtils
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

        public enum AuditEventStatus
        {
            AUDIT_NONE = 0,
            AUDIT_SUCCESS = 1,
            AUDIT_FAILURE = 2,
            AUDIT_SUCCESS_FAILURE = 3,
            //AUDIT_NONE = 4,
            EMPTY = 8
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

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern UInt32 LsaNtStatusToWinError(
            long Status);

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
        public static extern long LsaOpenPolicy(
            ref LSA_UNICODE_STRING SystemName,
            ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            Int32 DesiredAccess,
            out IntPtr PolicyHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern long LsaClose(IntPtr PolicyHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern long LsaFreeMemory(IntPtr Buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern void AuditFree(IntPtr Buffer);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern long LsaEnumerateAccountRights(
            IntPtr PolicyHandle, IntPtr AccountSid,
            out /* LSA_UNICODE_STRING[] */ IntPtr UserRights,
            out ulong CountOfRights);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern long LsaQueryInformationPolicy(
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
            out ulong pCountReturned);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern bool AuditQuerySystemPolicy(
            IntPtr pSubCategoryGuids,
            ulong PolicyCount,
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

        public struct UMInfo
        {
            public USER_MODALS_INFO_0 u0;
            public USER_MODALS_INFO_1 u1;
            public USER_MODALS_INFO_2 u2;
            public USER_MODALS_INFO_3 u3;
        }

        public static void connectToRemote(string remoteUNC, string username, string password)
        {
            connectToRemote(remoteUNC, username, password, false);
        }

        public static void connectToRemote(string remoteUNC, string username, string password, bool promptUser)
        {
            NETRESOURCE nr = new NETRESOURCE();
            nr.dwType = RESOURCETYPE_DISK;
            nr.lpRemoteName = remoteUNC;

            int ret;
            if (promptUser)
                ret = WNetUseConnection(IntPtr.Zero, nr, "", "", CONNECT_INTERACTIVE | CONNECT_PROMPT, null, null, null);
            else
                ret = WNetUseConnection(IntPtr.Zero, nr, password, username, 0, null, null, null);

            if (ret != 0)
                throw new System.ComponentModel.Win32Exception(ret);
        }

        public static void disconnectRemote(string remoteUNC)
        {
            int ret = WNetCancelConnection2(remoteUNC, CONNECT_UPDATE_PROFILE, false);
            // if (ret != 0)
            //    throw new System.ComponentModel.Win32Exception(ret);
        }

        public static List<TextFileContent> parseMatches(string dir, string fname, string line, string pattern, ref uint instance)
        {
            List<TextFileContent> retList = new List<TextFileContent>();
            Regex myRegex = new Regex(pattern);
            MatchCollection myMatches = myRegex.Matches(line);
            foreach (Match myMatch in myMatches)
            {
                TextFileContent retItem = new TextFileContent();
                retItem.SubExpressions = new List<string>();

                retItem.Line = line;
                retItem.Pattern = pattern;
                retItem.Text = myMatch.ToString();
                retItem.Path = dir;
                retItem.FileName = fname;
                retItem.Instance = ++instance;
                for (int i = 1; i < myMatch.Groups.Count; i++)
                {
                    retItem.SubExpressions.Add(myMatch.Groups[i].ToString());
                }
                retList.Add(retItem);
            }
            return retList;
        }

        public static List<TextFileContent> getWinTextFileContent(string remoteUNC, string pathspec, string pattern)
        {
            List<TextFileContent> retList = new List<TextFileContent>();

            if ((pathspec[1] != ':') || (pathspec[2] != '\\'))
                throw new ArgumentException("Pathspec must be of the form 'X:\\path\\filename[.ext]'", pathspec);

            char drive = pathspec[0];
            string restofpath = pathspec.Substring(2);
            int whereFile = pathspec.LastIndexOf('\\');
            string dirName, fName;
            if (whereFile > 0)
            {
                dirName = pathspec.Substring(0, whereFile);
                fName = pathspec.Substring(whereFile + 1);
            }
            else
            {
                dirName = "\\";
                fName = pathspec.Substring(1);
            }

            TextReader tr = new StreamReader(remoteUNC + "\\" + drive + "$" + restofpath);
            uint instance = 0;
            for (; ; )
            {
                string linha = tr.ReadLine();
                if (linha == null)
                    break;
                retList.AddRange(parseMatches(dirName, fName, linha, pattern, ref instance));
            }
            tr.Close();

            return retList;
        }

        public static UMInfo getLoginPolicies(string myHost)
        {
            uint retVal;
            IntPtr myBuf;
            UMInfo retData = new UMInfo();

            retVal = NetUserModalsGet(myHost, 0, out myBuf);
            if (retVal == 0)
                retData.u0 = (USER_MODALS_INFO_0)Marshal.PtrToStructure(myBuf, typeof(USER_MODALS_INFO_0));
            else
                throw new System.ComponentModel.Win32Exception((int)retVal);
            retVal = NetApiBufferFree(myBuf);

            retVal = NetUserModalsGet(myHost, 1, out myBuf);
            if (retVal == 0)
                retData.u1 = (USER_MODALS_INFO_1)Marshal.PtrToStructure(myBuf, typeof(USER_MODALS_INFO_1));
            else
                throw new System.ComponentModel.Win32Exception((int)retVal);
            retVal = NetApiBufferFree(myBuf);

            retVal = NetUserModalsGet(myHost, 2, out myBuf);
            if (retVal == 0)
                retData.u2 = (USER_MODALS_INFO_2)Marshal.PtrToStructure(myBuf, typeof(USER_MODALS_INFO_2));
            else
                throw new System.ComponentModel.Win32Exception((int)retVal);
            retVal = NetApiBufferFree(myBuf);

            retVal = NetUserModalsGet(myHost, 3, out myBuf);
            if (retVal == 0)
                retData.u3 = (USER_MODALS_INFO_3)Marshal.PtrToStructure(myBuf, typeof(USER_MODALS_INFO_3));
            else
                throw new System.ComponentModel.Win32Exception((int)retVal);
            retVal = NetApiBufferFree(myBuf);

            return retData;
        }

        public static LSA_UNICODE_STRING string2LSAUS(string myString)
        {
            LSA_UNICODE_STRING retStr = new LSA_UNICODE_STRING();
            retStr.Buffer = Marshal.StringToHGlobalUni(myString);
            retStr.Length = (UInt16)((myString.Length + 1) * UnicodeEncoding.CharSize);
            retStr.MaximumLength = (UInt16)((myString.Length + 1) * UnicodeEncoding.CharSize);
            return retStr;
        }

        public static string LSAUS2string(LSA_UNICODE_STRING lsaus)
        {
            char[] cvt = new char[lsaus.Length / UnicodeEncoding.CharSize];
            Marshal.Copy(lsaus.Buffer, cvt, 0, lsaus.Length / UnicodeEncoding.CharSize);
            return new string(cvt);
        }

        public static Dictionary<string, AuditEventStatus> getAuditPolicy(string targetHost)
        {
            Dictionary<string, AuditEventStatus> retList = new Dictionary<string, AuditEventStatus>();

            LSA_UNICODE_STRING systemName = string2LSAUS(@"\\" + targetHost);
            LSA_OBJECT_ATTRIBUTES objAttrs = new LSA_OBJECT_ATTRIBUTES();

            IntPtr policyHandle = IntPtr.Zero;
            IntPtr pAuditEventsInfo = IntPtr.Zero;
            IntPtr pAuditCategoryId = IntPtr.Zero;
            IntPtr pAuditSubCategoryGuids = IntPtr.Zero;
            IntPtr pAuditPolicies = IntPtr.Zero;

            long lretVal = LsaOpenPolicy(ref systemName, ref objAttrs, POLICY_VIEW_AUDIT_INFORMATION, out policyHandle);
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

                POLICY_AUDIT_EVENTS_INFO myAuditEventsInfo = new POLICY_AUDIT_EVENTS_INFO();
                myAuditEventsInfo = (POLICY_AUDIT_EVENTS_INFO)Marshal.PtrToStructure(pAuditEventsInfo, myAuditEventsInfo.GetType());

                for (ulong policyAuditEventType = 0; policyAuditEventType < myAuditEventsInfo.MaximumAuditEventCount; policyAuditEventType++)
                {
                    pAuditCategoryId = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GUID)));
                    if (!AuditLookupCategoryGuidFromCategoryId((POLICY_AUDIT_EVENT_TYPE)policyAuditEventType, pAuditCategoryId))
                    {
                        int causingError = GetLastError();
                        throw new System.ComponentModel.Win32Exception(causingError);
                    }

                    ulong nSubCats = 0;
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

                        for (ulong subcategoryIndex = 0; subcategoryIndex < nSubCats; subcategoryIndex++)
                        {
                            AUDIT_POLICY_INFORMATION currentPolicy = new AUDIT_POLICY_INFORMATION();

                            IntPtr itemPtr = new IntPtr(pAuditPolicies.ToInt64() + (Int64)subcategoryIndex * (Int64)Marshal.SizeOf(currentPolicy));
                            currentPolicy = (AUDIT_POLICY_INFORMATION)Marshal.PtrToStructure(itemPtr, currentPolicy.GetType());

                            String subCategoryName = String.Empty;
                            Marshal.StructureToPtr(currentPolicy, itemPtr, true);
                            AuditLookupSubCategoryName(itemPtr, ref subCategoryName);

                            retList.Add(subCategoryName, (AuditEventStatus)(currentPolicy.AuditingInformation & 0x3));
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

            return retList;
        }

        public static Dictionary<string, AuditEventStatus> GetAuditEventPolicies(string targetHost)
        {
            Dictionary<string, AuditEventStatus> retList = new Dictionary<string, AuditEventStatus>();

            LSA_UNICODE_STRING systemName = string2LSAUS(@"\\" + targetHost);
            LSA_OBJECT_ATTRIBUTES objAttrs = new LSA_OBJECT_ATTRIBUTES();

            IntPtr policyHandle = IntPtr.Zero;
            IntPtr pAuditEventsInfo = IntPtr.Zero;
            IntPtr pAuditCategoryGuid = IntPtr.Zero;
            IntPtr pAuditSubCategoryGuids = IntPtr.Zero;
            IntPtr pAuditPolicies = IntPtr.Zero;

            long lretVal = LsaOpenPolicy(ref systemName, ref objAttrs, POLICY_VIEW_AUDIT_INFORMATION, out policyHandle);
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

                for (ulong policyAuditEventType = 0; policyAuditEventType < myAuditEventsInfo.MaximumAuditEventCount; policyAuditEventType++)
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
                    retList.Add(categoryName, (AuditEventStatus)(status & 0x3));

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

        public static List<string> getAccessTokens(string targetHost, string entityName)
        {
            List<string> retList = new List<string>();

            IntPtr sid = IntPtr.Zero;
            int sidSize = 0;
            StringBuilder domainName = new StringBuilder();
            int nameSize = 0;
            int accountType = 0;
            long lretVal;
            uint retVal;

            LookupAccountName("\\\\" + targetHost, entityName, sid, ref sidSize,
                domainName, ref nameSize, ref accountType);
            domainName = new StringBuilder(nameSize);
            sid = Marshal.AllocHGlobal(sidSize);

            bool result = LookupAccountName("\\\\" + targetHost, entityName, sid, ref sidSize,
                domainName, ref nameSize, ref accountType);

            if (!result)
            {
                Marshal.FreeHGlobal(sid);
                throw new System.ComponentModel.Win32Exception(GetLastError());
            }

            LSA_UNICODE_STRING systemName = string2LSAUS("\\\\" + targetHost);
            IntPtr policyHandle = IntPtr.Zero;
            LSA_OBJECT_ATTRIBUTES objAttrs = new LSA_OBJECT_ATTRIBUTES();
            lretVal = LsaOpenPolicy(ref systemName, ref objAttrs,
                                POLICY_LOOKUP_NAMES | POLICY_VIEW_LOCAL_INFORMATION, out policyHandle);
            retVal = LsaNtStatusToWinError(lretVal);

            if (retVal != 0)
            {
                Marshal.FreeHGlobal(sid);
                throw new System.ComponentModel.Win32Exception((int)retVal);
            }

            IntPtr rightsArray = IntPtr.Zero;
            ulong rightsCount = 0;
            lretVal = LsaEnumerateAccountRights(policyHandle, sid, out rightsArray, out rightsCount);
            retVal = LsaNtStatusToWinError(lretVal);
            Marshal.FreeHGlobal(sid);

            if (retVal == 2)
            {
                LsaClose(policyHandle);
                return retList;
            }
            if (retVal != 0)
            {
                LsaClose(policyHandle);
                throw new System.ComponentModel.Win32Exception((int)retVal);
            }

            LSA_UNICODE_STRING myLsaus = new LSA_UNICODE_STRING();
            for (ulong i = 0; i < rightsCount; i++)
            {
                IntPtr itemAddr = new IntPtr(rightsArray.ToInt64() + (long)(i * (ulong)Marshal.SizeOf(myLsaus)));
                myLsaus = (LSA_UNICODE_STRING)Marshal.PtrToStructure(itemAddr, myLsaus.GetType());
                string thisRight = LSAUS2string(myLsaus);
                retList.Add(thisRight);
            }

            lretVal = LsaClose(policyHandle);
            retVal = LsaNtStatusToWinError(lretVal);
            if (retVal != 0)
                throw new System.ComponentModel.Win32Exception((int)retVal);

            return retList;
        }
    }
}
