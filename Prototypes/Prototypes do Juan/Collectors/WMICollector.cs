using System;
using System.Net;
using System.IO;
using System.Management;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace FrameworkNG
{
    /// <summary>
    /// === WMICollector ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: Makes data collections by WMI queries on remote machines.
    /// How to Use: N/A
    /// Exceptions: ColectorException
    /// Hypoteses: Should be used with subclasses of WMIControlSpec.
    /// Example: N/A
    /// </summary>
    public class WMICollector : Collector
    {
        [DllImport("advapi32.dll")]
        public static extern int LogonUser(String lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        private bool isconnected;
        private string wmi_namespace = "root\\CIMV2";
        protected WindowsImpersonationContext myContext = null;
        protected ManagementScope scope;
        protected ConnectionOptions options;

        public static object GetWMIProp(ManagementObject queryObj, string propName)
        {
            object retVal;

            try
            {
                retVal = queryObj[propName];
            }
            catch (ManagementException)
            {
                retVal = null;
            }

            return retVal;
        }

        public ManagementScope GetScope()
        {
            return scope;
        }

        public WindowsImpersonationContext WearDrag(string Username, string Password, string DomainOrMachine)
        {
            WindowsImpersonationContext impersonationContext;
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUser(Username, DomainOrMachine, Password,
                        LOGON32_LOGON_NEW_CREDENTIALS,
                        LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    tempWindowsIdentity = new WindowsIdentity(token);
                    impersonationContext = tempWindowsIdentity.Impersonate();
                    if (impersonationContext != null)
                    {
                        CloseHandle(token);
                        return impersonationContext;
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            return null;
        }

        private ManagementObjectSearcher MyGetSearcher(string myquery)
        {
            EnumerationOptions options = new EnumerationOptions();
            options.Rewindable = false;
            options.ReturnImmediately = true;

            return new ManagementObjectSearcher(scope, new ObjectQuery(myquery), options);
        }

        public WMICollector(string wminamespace)
        {
            wmi_namespace = wminamespace;
        }

        public WMICollector(CollectorAuth authinfo, string wminamespace)
            : base(authinfo)
        {
            wmi_namespace = wminamespace;
        }
        
        public WMICollector()
        {
        }

        public WMICollector(CollectorAuth authinfo)
            : base(authinfo)
        {
        }

        public string Namespace
        {
            get { return wmi_namespace; }
            protected set
            {
                if (isconnected)
                    throw new CollectorException(
                        String.Format("Can't set WMI namespace to '{0}'; already connected to '{1}'", value, wmi_namespace)
                        );
                wmi_namespace = value;
            }
        }

        public override void Connect(string hostname)
        {
            // if (myContext != null)
            //     myContext.Undo();
            // myContext = WearDrag(Auth.Username, Auth.Password, hostname);

            options = new ConnectionOptions();
            options.Username = Auth.Username;
            options.Password = Auth.Password;

            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;

            if ((Auth.Domain != null) && (Auth.Domain != ""))
                options.Authority = String.Format("{0}:{1}", Auth.AuthStyle, Auth.Domain);
            scope = new ManagementScope(String.Format("\\\\{0}\\{1}", hostname, Namespace), options);
            scope.Connect();
            isconnected = true;
        }

        public override CollectResult Collect(ControlSpec spec)
        {
            WMIControlSpec myspec = (WMIControlSpec)spec;
            ManagementObjectSearcher searcher = MyGetSearcher(myspec.WMIQuery);
            CollectResult retval = new CollectResult();

            retval.Data = searcher.Get();
            return retval;
        }

        public SysInfo CollectSysInfo()
        {
            SysInfo retVal = new SysInfo();
            ManagementObjectSearcher searcher;
            string arch = null;
            string hostname = null;
            string os = null;
            string osversion = null;

            searcher = MyGetSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                arch = GetWMIProp(queryObj, "SystemType").ToString();
                switch (arch)
                {
                    case "X86-based PC":
                    case "X86-Nec98 PC":
                        arch = "INTEL32";
                        break;
                    case "MIPS-based PC":
                        arch = "MIPS";
                        break;
                    case "Alpha-based PC":
                        arch = "ALPHA32";
                        break;
                    case "Power PC":
                        arch = "POWERPC32";
                        break;
                    case "SH-x PC":
                        arch = "SUPERH";
                        break;
                    case "StrongARM PC":
                        arch = "STRONGARM";
                        break;
                    case "64-bit Intel PC":
                        arch = "INTEL64";
                        break;
                    case "64-bit Alpha PC":
                        arch = "ALPHA64";
                        break;
                    default:
                        arch = "UNKNOWN";
                        break;
                }
                hostname = (string) GetWMIProp(queryObj, "DNSHostName");
                if (hostname == null)
                    hostname = ((string) GetWMIProp(queryObj, "Name")).ToLower();
                if ((bool)GetWMIProp(queryObj, "PartOfDomain"))
                    hostname += "." + GetWMIProp(queryObj, "Domain");
            }

            searcher = MyGetSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                osversion = (string)GetWMIProp(queryObj, "Version");
                os = (string)GetWMIProp(queryObj, "Name");
                os = os.Split('|')[0].Trim();
                os = os.Replace("®", "");
                os = os.Replace("™", "");
                ushort spmajor = (ushort)GetWMIProp(queryObj, "ServicePackMajorVersion");
                ushort spminor = (ushort)GetWMIProp(queryObj, "ServicePackMinorVersion");
                if (spmajor > 0)
                {
                    os += " SP" + spmajor.ToString();
                    if (spminor > 0)
                        os += "." + spminor.ToString();
                }
            }

            retVal.Interfaces = CollectNICInfo();
            retVal.Hostname = hostname;
            retVal.Architecture = arch;
            retVal.OS = os;
            retVal.OSVersion = osversion;

            return retVal;
        }

        /// <summary>Obtains information from the remote host's network interfaces.</summary>
        /// <returns>File info</returns>
        public List<InterfaceState> CollectNICInfo()
        {
            List<InterfaceState> retVal = new List<InterfaceState>();
            ManagementObjectSearcher searcher;
            string ifaceWhere = "";

            searcher = MyGetSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = True");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                InterfaceState thisNIC = new InterfaceState();

                thisNIC.Index = (uint)queryObj["Index"];
                thisNIC.InterfaceIndex = (uint)queryObj["InterfaceIndex"];
                thisNIC.Name = (string)queryObj["Description"];
                thisNIC.HWAddr = (string)queryObj["MACAddress"];

                if (ifaceWhere == "")
                    ifaceWhere = "Index = " + thisNIC.Index.ToString();
                else
                    ifaceWhere += " OR Index = " + thisNIC.Index.ToString();

                bool useDHCP = (queryObj["DHCPEnabled"] == null) ? false : (bool)queryObj["DHCPEnabled"];
                thisNIC.InetAddr = new List<InterfaceState.IPInfo>();
                string[] ipAddrs = (string[])queryObj["IPAddress"];
                if (ipAddrs != null)
                {
                    string[] ipMasks = (string[])queryObj["IPSubnet"];
                    for (int i = 0; i <= ipAddrs.GetUpperBound(0); i++)
                    {
                        if (ipAddrs[i].Contains(":"))   // Ignore IPv6
                            continue;
                        byte[] thisAddr = IPAddress.Parse(ipAddrs[i]).GetAddressBytes();
                        byte[] thisMask = IPAddress.Parse(ipMasks[i]).GetAddressBytes();
                        byte[] thisBcast = (byte[])thisAddr.Clone();
                        bool zeroBcast = (queryObj["IPUseZeroBroadcast"] == null) ? false : (bool)queryObj["IPUseZeroBroadcast"];
                        for (int ipbyte = 0; ipbyte <= thisAddr.GetUpperBound(0); ipbyte++)
                        {
                            if (zeroBcast)
                            {
                                thisBcast[ipbyte] = (byte)(thisBcast[ipbyte] & thisMask[ipbyte]);
                            }
                            else
                            {
                                thisBcast[ipbyte] = (byte)(thisBcast[ipbyte] | ~thisMask[ipbyte]);
                            }
                        }

                        InterfaceState.IPInfo inetAddrObj = new InterfaceState.IPInfo();
                        inetAddrObj.IPBcast = new IPAddress(thisBcast).ToString();
                        inetAddrObj.IPMask = ipMasks[i];
                        inetAddrObj.IPAddr = ipAddrs[i];
                        if (useDHCP)
                            inetAddrObj.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_DYNAMIC;
                        else if (i == 0)
                            inetAddrObj.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_PRIMARY;
                        else
                            inetAddrObj.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_TRANSIENT;
                        thisNIC.InetAddr.Add(inetAddrObj);
                    }
                }
                retVal.Add(thisNIC);
            }

            searcher = MyGetSearcher("SELECT * FROM Win32_NetworkAdapter WHERE " + ifaceWhere);
            foreach (ManagementObject queryObj in searcher.Get())
            {
                uint thisIndex = (uint)queryObj["Index"];
                InterfaceState thisIface = null;
                foreach (InterfaceState istate in retVal)
                {
                    if (istate.Index == thisIndex)
                    {
                        thisIface = istate;
                        break;
                    }
                }
                if (thisIface == null)
                    continue;

                thisIface.IsPhysical = (queryObj["PhysicalAdapter"] == null) ? false : (bool)queryObj["PhysicalAdapter"];
                object thisTypeIdObj = queryObj["AdapterTypeId"];
                ushort thisTypeId = (thisTypeIdObj == null) ? (ushort)65535 : (ushort)thisTypeIdObj;
                switch (thisTypeId)
                {
                    case 0:
                        thisIface.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_ETHERNET;
                        break;
                    case 1:
                        thisIface.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_TOKENRING;
                        break;
                    case 2:
                        thisIface.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_FDDI;
                        break;
                    default:
                        thisIface.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_OTHER;
                        break;
                }
            }

            return retVal;
        }

        public List<WMIWinPatchPackage> CollectPatches()
        {
            List<WMIWinPatchPackage> retList = new List<WMIWinPatchPackage>();
            ManagementObjectSearcher searcher;

            searcher = MyGetSearcher("SELECT * FROM Win32_QuickFixEngineering");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                WMIWinPatchPackage thisfix = new WMIWinPatchPackage();
                object thisProp;

                thisfix.Caption = GetWMIProp(queryObj, "Caption").ToString();
                thisfix.Description = GetWMIProp(queryObj, "Description").ToString();
                thisfix.HotFixID = GetWMIProp(queryObj, "HotFixID").ToString();
                thisfix.FixComments = GetWMIProp(queryObj, "FixComments").ToString();
                thisfix.InstalledBy = GetWMIProp(queryObj, "InstalledBy").ToString();

                thisProp = GetWMIProp(queryObj, "InstalledOn");
                if (thisProp != null)
                    thisfix.InstalledOn = thisProp.ToString();

                retList.Add(thisfix);
            }
            return retList;
        }

        /// <summary>Obtains information from a remote file.</summary>
        /// <param name="path">Remote file or directory's fully qualified path.</param>
        /// <returns>File info</returns>
        public WMIFileInfo CollectFileInfo(string path)
        {
            WMIFileInfo retVal = new WMIFileInfo();

            // Atributes from from CIM_DataFile (creation date, size, etc)
            string pathDrive = Path.GetPathRoot(path);
            string pathPath = Path.GetDirectoryName(path);
            string pathFilename = Path.GetFileNameWithoutExtension(path);
            string pathExtension = Path.GetExtension(path);

            if (pathDrive[1] != ':')
                throw new CollectorException(String.Format("Invalid path '{0}': must be a full path with drive letter", path));
            pathDrive = pathDrive.Substring(0, 2);
            pathPath = pathPath.Substring(2);
            if (pathPath[pathPath.Length - 1] != '\\')
            {
                pathPath += '\\';
            }
            pathPath = pathPath.Replace("\\", "\\\\");
            if (Path.HasExtension(path))
                pathExtension = pathExtension.Substring(1);

            try
            {
                ManagementObject queryObj = null;
                retVal.Query = String.Format("SELECT * FROM CIM_LogicalFile WHERE Drive = '{0}' AND Path = '{1}' AND FileName = '{2}' AND Extension = '{3}'", pathDrive, pathPath, pathFilename, pathExtension);
                ManagementObjectSearcher searcher = MyGetSearcher(retVal.Query);
                foreach (ManagementObject tempQueryObj in searcher.Get())
                {
                    queryObj = tempQueryObj;
                    break;
                }
                if (queryObj == null)
                {
                    retVal.Query = String.Format("SELECT * FROM CIM_LogicalFile WHERE Drive = '{0}' AND Path = '{1}' AND FileName = '{2}.{3}' AND Extension = ''", pathDrive, pathPath, pathFilename, pathExtension);
                    searcher = MyGetSearcher(retVal.Query);
                    foreach (ManagementObject tempQueryObj in searcher.Get())
                    {
                        queryObj = tempQueryObj;
                        break;
                    }
                }

                if (queryObj == null)
                {
                    retVal.Found = false;
                    retVal.ErrorMsg = "File not found";
                }
                else
                {
                    retVal.Found = true;

                    retVal.Drive = (string)queryObj["Drive"];
                    retVal.Path = (string)queryObj["Path"];
                    retVal.Extension = (string)queryObj["Extension"];
                    retVal.FileName = (string)queryObj["FileName"];
                    retVal.Name = (string)queryObj["Name"];

                    retVal.Archive = (bool)queryObj["Archive"];
                    retVal.Compressed = (bool)queryObj["Compressed"];
                    retVal.EightDotThreeFileName = (string)queryObj["EightDotThreeFileName"];
                    retVal.Encrypted = (bool)queryObj["Encrypted"];
                    retVal.FileType = (string)queryObj["FileType"];
                    retVal.Hidden = (bool)queryObj["Hidden"];
                    retVal.System = (bool)queryObj["System"];

                    retVal.Writeable = (bool)queryObj["Writeable"];
                    retVal.CreationDate = DateTime.ParseExact(((string)queryObj["CreationDate"]).Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);
                    retVal.InstallDate = DateTime.ParseExact(((string)queryObj["InstallDate"]).Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);
                    retVal.LastAccessed = DateTime.ParseExact(((string)queryObj["LastAccessed"]).Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);
                    retVal.LastModified = DateTime.ParseExact(((string)queryObj["LastModified"]).Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);

                    string wtfIsThis = queryObj.ClassPath.ClassName;
                    switch (wtfIsThis)
                    {
                        case "Win32_Directory":
                            retVal.IsDirectory = true;
                            break;
                        case "CIM_DataFile":
                            retVal.IsDirectory = false;
                            retVal.FileSize = (ulong)queryObj["FileSize"];
                            retVal.Manufacturer = (string)queryObj["Manufacturer"];
                            retVal.Version = (string)queryObj["Version"];
                            break;
                        default:
                            retVal.IsDirectory = false;
                            retVal.ErrorMsg = "Unexpected obect type '" + queryObj.ClassPath.ClassName + "'";
                            break;
                    }
                }

                // Attributes from Win32_LogicalFileSecuritySetting (Owner, Group, ACLs)
                ManagementObject mgmt = new ManagementObject(scope, new ManagementPath(String.Format("Win32_LogicalFileSecuritySetting.path='{0}'", path)), null);
                ManagementBaseObject secDesc = mgmt.InvokeMethod("GetSecurityDescriptor", null, null);
                ManagementBaseObject descriptor = secDesc.Properties["Descriptor"].Value as ManagementBaseObject;

                ManagementBaseObject owner = descriptor.Properties["Owner"].Value as ManagementBaseObject;
                retVal.Owner  = String.Format("{0}\\{1}", owner.Properties["Domain"].Value, owner.Properties["Name"].Value);

                ManagementBaseObject group = descriptor.Properties["Group"].Value as ManagementBaseObject;
                retVal.Group = String.Format("{0}\\{1}", group.Properties["Domain"].Value, group.Properties["Name"].Value);

                retVal.SACL = new List<WMIWinACE>();
                ManagementBaseObject[] acls = descriptor.Properties["SACL"].Value as ManagementBaseObject[];
                if (acls != null)
                {
                    foreach (ManagementBaseObject thisacl in acls)
                    {
                        WMIWinACE thisace = new WMIWinACE();
                        thisace.IsDirectory = retVal.IsDirectory;
                        thisace.AccessMask = (UInt32)thisacl.Properties["AccessMask"].Value;
                        thisace.AceFlags = (UInt32)thisacl.Properties["AceFlags"].Value;
                        thisace.AceType = (UInt32)thisacl.Properties["AceType"].Value;
                        thisace.GuidInheritedObjectType = thisacl.Properties["GuidInheritedObjectType"].Value as string;
                        thisace.GuidObjectType = thisacl.Properties["GuidObjectType"].Value as string;

                        thisace.Trustee = new WMIWinTrustee();
                        ManagementBaseObject trustee = thisacl.Properties["Trustee"].Value as ManagementBaseObject;
                        thisace.Trustee.Domain = trustee.Properties["Domain"].Value as string;
                        thisace.Trustee.Name = trustee.Properties["Name"].Value as string;
                        thisace.Trustee.SID = trustee.Properties["SID"].Value as Byte[];
                        thisace.Trustee.SidLength = (UInt32)trustee.Properties["SidLength"].Value;
                        thisace.Trustee.SIDString = trustee.Properties["SIDString"].Value as string;

                        retVal.SACL.Add(thisace);
                    }
                }

                retVal.DACL = new List<WMIWinACE>();
                acls = descriptor.Properties["DACL"].Value as ManagementBaseObject[];
                if (acls != null)
                {
                    foreach (ManagementBaseObject thisacl in acls)
                    {
                        WMIWinACE thisace = new WMIWinACE();
                        thisace.IsDirectory = retVal.IsDirectory;
                        thisace.AccessMask = (UInt32)thisacl.Properties["AccessMask"].Value;
                        thisace.AceFlags = (UInt32)thisacl.Properties["AceFlags"].Value;
                        thisace.AceType = (UInt32)thisacl.Properties["AceType"].Value;
                        thisace.GuidInheritedObjectType = thisacl.Properties["GuidInheritedObjectType"].Value as string;
                        thisace.GuidObjectType = thisacl.Properties["GuidObjectType"].Value as string;

                        thisace.Trustee = new WMIWinTrustee();
                        ManagementBaseObject trustee = thisacl.Properties["Trustee"].Value as ManagementBaseObject;
                        thisace.Trustee.Domain = trustee.Properties["Domain"].Value as string;
                        thisace.Trustee.Name = trustee.Properties["Name"].Value as string;
                        thisace.Trustee.SID = trustee.Properties["SID"].Value as Byte[];
                        thisace.Trustee.SidLength = (UInt32)trustee.Properties["SidLength"].Value;
                        thisace.Trustee.SIDString = trustee.Properties["SIDString"].Value as string;

                        retVal.DACL.Add(thisace);
                    }
                }
            }
            catch (Exception excp)
            {
                retVal.ErrorMsg = String.Format("{0}: {1}", excp.GetType(), excp.Message);
            }

            return retVal;
        }
    }
}