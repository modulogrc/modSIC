using System;
using System.Windows;
using System.Management;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using System.DirectoryServices.AccountManagement;
using FrameworkNG;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.DirectoryServices;
using System.Security.Principal;

namespace CollectorsTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_PROVIDER_WINNT50 = 3;
        const int LOGON32_PROVIDER_WINNT40 = 2;
        const int LOGON32_PROVIDER_WINNT35 = 1;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int LogonUser(String lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        extern static int GetLastError();

        FrameworkNG.WMICollector mycollector;
        FrameworkNG.WinRegCollector myrcollector;
        PrincipalContext accManContext;
        string myUNC;

        public MainWin()
        {
            InitializeComponent();
        }

        private static void NonBlockingPrint(System.Windows.Controls.TextBox myBox, string myStr, params object[] args)
        {
            myBox.Text += String.Format(myStr, args);
            myBox.ScrollToEnd();
            System.Windows.Forms.Application.DoEvents();
        }

        private static ManagementObjectSearcher MyGetSearcher(ManagementScope scope, string myquery)
        {
            if (scope == null)
                return new ManagementObjectSearcher("root\\CIMV2", myquery);
            else
                return new ManagementObjectSearcher(scope, new ObjectQuery(myquery));
        }

        public string getWMIPropFromString(string path, string propName)
        {
            int crudLeft = propName.Length + 3;
            int whereProp = path.IndexOf("." + propName + "=\"");
            if (whereProp < 0)
                whereProp = path.IndexOf("," + propName + "=\"");
            if (whereProp < 0)
                return "";
            int endProp = path.IndexOf('"', whereProp + crudLeft);
            if (endProp < 0)
                return path.Substring(whereProp + crudLeft);
            else
                return path.Substring(whereProp + crudLeft, endProp - whereProp - crudLeft);
        }

        private void doWmi_Click(object sender, RoutedEventArgs e)
        {
            // http://msdn.microsoft.com/en-us/library/ms257337.aspx

            NonBlockingPrint(wmiResults, "---------- WMI Start ----------\n");
            try
            {
                ManagementScope scope = null;
                ConnectionOptions options = null;

                if (tbHost.Text.Length > 0)
                {
                    options = new ConnectionOptions();
                    options.Username = tbUsername.Text;
                    options.Password = tbPassword.Password;
                    // options.Impersonation = ImpersonationLevel.Impersonate;
                    // options.Authentication = System.Management.AuthenticationLevel.Default;
                    // options.EnablePrivileges = true;
                    if (cbAuth.Text == "NT")
                        options.Authority = String.Format("ntlmdomain:{0}", tbDomain.Text);
                    else
                        options.Authority = String.Format("kerberos:{0}", tbDomain.Text);
                    scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", tbHost.Text), options);
                    scope.Connect();
                }

                ManagementObjectSearcher searcher;
#if false
                searcher = MyGetSearcher(scope, "SELECT * FROM Win32_Environment");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "Environment Variable:\n{0}={1}\n",
                        WMICollector.GetWMIProp(queryObj, "Name"),
                        WMICollector.GetWMIProp(queryObj, "VariableValue"));
                    NonBlockingPrint(wmiResults, "    Caption={0}\n", WMICollector.GetWMIProp(queryObj, "Caption"));
                    NonBlockingPrint(wmiResults, "    Description={0}\n", WMICollector.GetWMIProp(queryObj, "Description"));
                    NonBlockingPrint(wmiResults, "    InstallDate={0}\n", WMICollector.GetWMIProp(queryObj, "InstallDate"));
                    NonBlockingPrint(wmiResults, "    Status={0}\n", WMICollector.GetWMIProp(queryObj, "Status"));
                    NonBlockingPrint(wmiResults, "    SystemVariable={0}\n", WMICollector.GetWMIProp(queryObj, "SystemVariable"));
                    NonBlockingPrint(wmiResults, "    UserName={0}\n", WMICollector.GetWMIProp(queryObj, "UserName"));
                }

                searcher = MyGetSearcher(scope, "SELECT * FROM Win32_OperatingSystem");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "OS Name: {0}\n", WMICollector.GetWMIProp(queryObj, "Name"));
                    NonBlockingPrint(wmiResults, "OS Version: {0}\n", WMICollector.GetWMIProp(queryObj, "Version"));
                    NonBlockingPrint(wmiResults, "OS Service Pack Major Version: {0}\n", WMICollector.GetWMIProp(queryObj, "ServicePackMajorVersion"));
                    NonBlockingPrint(wmiResults, "OS Service Pack Minor Version: {0}\n", WMICollector.GetWMIProp(queryObj, "ServicePackMinorVersion"));
                    NonBlockingPrint(wmiResults, "OS Other Type Description: {0}\n", WMICollector.GetWMIProp(queryObj, "OtherTypeDescription"));
                    NonBlockingPrint(wmiResults, "OS Architecture: {0}\n", WMICollector.GetWMIProp(queryObj, "OSArchitecture"));
                }

                searcher = MyGetSearcher(scope, "SELECT * FROM Win32_BIOS");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "BIOS Name: {0}\n", WMICollector.GetWMIProp(queryObj, "Name"));
                }
#endif
                string theHost = "";
                searcher = MyGetSearcher(scope, "SELECT * FROM Win32_ComputerSystem");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    theHost = WMICollector.GetWMIProp(queryObj, "Name").ToString();
                    NonBlockingPrint(wmiResults, "Computer Name: {0}\n", theHost);
                    /* NonBlockingPrint(wmiResults, "Computer System Type: {0}\n", WMICollector.GetWMIProp(queryObj, "SystemType"));
                    NonBlockingPrint(wmiResults, "Computer DNS Host Name: {0}\n", WMICollector.GetWMIProp(queryObj, "DNSHostName"));
                    NonBlockingPrint(wmiResults, "Computer Part of Domain: {0}\n", WMICollector.GetWMIProp(queryObj, "PartOfDomain"));
                    NonBlockingPrint(wmiResults, "Computer Domain: {0}\n", WMICollector.GetWMIProp(queryObj, "Domain"));
                    NonBlockingPrint(wmiResults, "Computer Workgroup: {0}\n", WMICollector.GetWMIProp(queryObj, "Workgroup")); */
                }

                char[] backSlash = { '\\' };
                string[] domUser = tbRemoteFile.Text.Split(backSlash, 2);
                string theSID = "";
                string theDomain = "";
                string theName = "";
                string theGroupDomain = "";
                string theGroupName = "";
                bool isDisabled = true;
                if (domUser.GetUpperBound(0) >= 1)
                {
                    theDomain = domUser[0];
                    theName = domUser[1];
                }
                else
                {
                    theDomain = theHost;
                    theName = domUser[0];
                }

                searcher = MyGetSearcher(scope, "SELECT GroupComponent FROM Win32_GroupUser WHERE PartComponent = \"Win32_UserAccount.Domain='" + theDomain + "',Name='" + theName + "'\"");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    string isInThisGroupObj = WMICollector.GetWMIProp(queryObj, "GroupComponent").ToString();
                    ManagementObject isInThisGroup = new ManagementObject(isInThisGroupObj);
                    theGroupDomain = WMICollector.GetWMIProp(isInThisGroup, "Domain").ToString();
                    theGroupName = WMICollector.GetWMIProp(isInThisGroup, "Name").ToString();
                    NonBlockingPrint(wmiResults, "Group: {0}\\{1}\n", theGroupDomain, theGroupName);
                }

                searcher = MyGetSearcher(scope, "SELECT * FROM Win32_Account WHERE Domain = '" + theDomain + "' AND Name='" + theName + "'");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    try
                    {
                        isDisabled = (bool)WMICollector.GetWMIProp(queryObj, "Disabled");
                    }
                    catch
                    {
                        isDisabled = false;
                    }
                    theSID = WMICollector.GetWMIProp(queryObj, "SID").ToString();
                    NonBlockingPrint(wmiResults, "SID: {0}; Disabled = {1}\n", theSID, isDisabled);
                }

                if (theGroupName != "")
                {
                    searcher = MyGetSearcher(scope, "SELECT PartComponent FROM Win32_GroupUser WHERE GroupComponent = \"Win32_Group.Domain='" + theGroupDomain + "',Name='" + theGroupName + "'\"");
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        string isInThisUserObj = WMICollector.GetWMIProp(queryObj, "PartComponent").ToString();
                        string theUserDomain;
                        string theUserName;

                        theUserDomain = getWMIPropFromString(isInThisUserObj, "Domain");
                        theUserName = getWMIPropFromString(isInThisUserObj, "Name");
                        NonBlockingPrint(wmiResults, "Is a member of {0}: {1}{2}{3}\n", theGroupName, theUserDomain, (theUserDomain == "") ? "" : "\\", theUserName);
                    }
                }

                searcher = MyGetSearcher(scope, "SELECT GroupComponent FROM Win32_GroupUser WHERE PartComponent = \"Win32_Account.SID='" + theSID + "''\"");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    object isInThisGroupObj = WMICollector.GetWMIProp(queryObj, "GroupComponent");
                    ManagementObject isInThisGroup = new ManagementObject(isInThisGroupObj.ToString());
                    theGroupDomain = WMICollector.GetWMIProp(isInThisGroup, "Domain").ToString();
                    theGroupName = WMICollector.GetWMIProp(isInThisGroup, "Name").ToString();
                    NonBlockingPrint(wmiResults, "Searched Group by Account SID: {0}\\{1}\n", theGroupDomain, theGroupName);
                }

            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "A ManagementException occurred while querying for WMI data: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for WMI data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- WMI End ----------\n");
            }
        }

        private void doQueryUsers_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- WMI Accounts Start ----------\n");
            try
            {
                ManagementObjectSearcher searcher;

                //searcher = MyGetSearcher(mycollector.GetScope(), String.Format(String.Format("SELECT * FROM Win32_Account WHERE Domain = '{0}' AND Name = '{1}'",
                //                                                tbHost.Text, tbRemoteFile.Text)));
                searcher = MyGetSearcher(mycollector.GetScope(), String.Format("SELECT * FROM Win32_Account"));
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "\n");
                    NonBlockingPrint(wmiResults, "Domain: {0}\n", WMICollector.GetWMIProp(queryObj, "Domain"));
                    NonBlockingPrint(wmiResults, "Name: {0}\n", WMICollector.GetWMIProp(queryObj, "Name"));
                    NonBlockingPrint(wmiResults, "Caption: {0}\n", WMICollector.GetWMIProp(queryObj, "Caption"));
                    NonBlockingPrint(wmiResults, "Description: {0}\n", WMICollector.GetWMIProp(queryObj, "Description"));
                    NonBlockingPrint(wmiResults, "InstallDate: {0}\n", WMICollector.GetWMIProp(queryObj, "InstallDate"));
                    NonBlockingPrint(wmiResults, "LocalAccount: {0}\n", WMICollector.GetWMIProp(queryObj, "LocalAccount"));
                    NonBlockingPrint(wmiResults, "SID: {0}\n", WMICollector.GetWMIProp(queryObj, "SID"));
                    NonBlockingPrint(wmiResults, "SIDType: {0}\n", WMICollector.GetWMIProp(queryObj, "SIDType"));
                    NonBlockingPrint(wmiResults, "Status: {0}\n", WMICollector.GetWMIProp(queryObj, "Status"));
                }
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "A ManagementException occurred while querying for WMI data: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for WMI data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- WMI Accounts End ----------\n");
            }
        }

        private void doOtherWMI_Click(object sender, RoutedEventArgs e)
        {
            doOtherWMI.IsEnabled = false;
            wmiResults.Text += "---------- Other WMI Start ----------\n";
            System.Windows.Forms.Application.DoEvents();
            try
            {
                FrameworkNG.WMI.Registry.RegistryObject SysRegistry =
                                new FrameworkNG.WMI.Registry.RegistryRemote(tbUsername.Text,
                                                            tbPassword.Password,
                                                            tbDomain.Text,
                                                            tbHost.Text);
                string registryKey = @"HARDWARE\Description\System\BIOS";

                wmiResults.Text += String.Format("Values Under: {0}\n", registryKey);
                foreach (string valueName in SysRegistry.EnumerateValues(FrameworkNG.WMI.Registry.baseKey.HKEY_LOCAL_MACHINE, registryKey))
                {
                    string valueValue = SysRegistry.GetValue(FrameworkNG.WMI.Registry.baseKey.HKEY_LOCAL_MACHINE,
                        registryKey, valueName, FrameworkNG.WMI.Registry.valueType.STRING);
                    wmiResults.Text += String.Format(">>> {0} = {1}\n", valueName, valueValue);
                    wmiResults.ScrollToEnd();
                    System.Windows.Forms.Application.DoEvents();
                }

                wmiResults.Text += String.Format("SubKeys Under: {0}\n", registryKey);
                foreach (string subKey in SysRegistry.EnumerateKeys(
                              FrameworkNG.WMI.Registry.baseKey.HKEY_LOCAL_MACHINE, registryKey))
                {
                    wmiResults.Text += String.Format(">>> {0}\n", subKey);
                    wmiResults.ScrollToEnd();
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (ManagementException excp)
            {
                wmiResults.Text += string.Format("An error occurred while querying for WMI data: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                wmiResults.Text += string.Format("An error occurred while querying for WMI data: {0}\n", excp.Message);
            }
            finally
            {
                wmiResults.Text += "---------- Other WMI End ----------\n";
                wmiResults.ScrollToEnd();
                doOtherWMI.IsEnabled = true;
            }
        }

        private void doWMIConnect_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- Collector Connect start ----------\n");
            try
            {
                FrameworkNG.CollectorAuth myauth = new FrameworkNG.CollectorAuth(tbUsername.Text, tbPassword.Password, tbDomain.Text);
                if (cbAuth.Text == "NT")
                    myauth.AuthStyle = "ntlmdomain";
                else
                    myauth.AuthStyle = "kerberos";
                mycollector = new FrameworkNG.WMICollector(myauth);
                mycollector.Connect(tbHost.Text);
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- Collector Connect end ----------\n");
            }
        }

        private void doFileInfo_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- File Info test start: {0} ----------\n", tbRemoteFile.Text);
            try
            {
                FrameworkNG.WMIFileInfo props = mycollector.CollectFileInfo(tbRemoteFile.Text);
                NonBlockingPrint(wmiResults, String.Format("Props:\n{0}\n", props));
                NonBlockingPrint(wmiResults, "\nDACL:\n");
                foreach (WMIWinACE thisacl in props.DACL)
                {
                    NonBlockingPrint(wmiResults, "ACE: {0}\n", thisacl);
                }

                string thisSID = "";
                IntPtr nuclearACL = WinACL.MakeAuditACL(props.SACL.Count);
                NonBlockingPrint(wmiResults, "\nSACL:\n");
                foreach (WMIWinACE thisacl in props.SACL)
                {
                    thisSID = thisacl.Trustee.SIDString;
                    WinACL.AddACEToAuditACL(nuclearACL, thisacl.AceFlags, thisacl.AccessMask, thisSID);
                    NonBlockingPrint(wmiResults, "ACE: {0}\n", thisacl);
                }

                UInt32 successAudit = 0;
                UInt32 failAudit = 0;
                WinACL.GetAuditedPermissions(nuclearACL, thisSID, out successAudit, out failAudit);
                Marshal.FreeHGlobal(nuclearACL);
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- File Info test end ----------\n");
            }
        }

        private void doGoodBIOS_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- GoodBIOS check start ----------\n");
            try
            {
                FrameworkNG.GoodBIOSControlSpec myspec = new FrameworkNG.GoodBIOSControlSpec();
                FrameworkNG.CollectResult myres = mycollector.Collect(myspec);
                NonBlockingPrint(wmiResults, "BIOS probe decision: {0}\n", myspec.Decide(myres));
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- GoodBIOS check end ----------\n");
            }
        }

        private void doWinRegConnect_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- Collector Connect start ----------\n");
            try
            {
                FrameworkNG.CollectorAuth myauth = new FrameworkNG.CollectorAuth(tbUsername.Text, tbPassword.Password, tbDomain.Text);
                if (cbAuth.Text == "NT")
                    myauth.AuthStyle = "ntlmdomain";
                else
                    myauth.AuthStyle = "kerberos";
                myrcollector = new FrameworkNG.WinRegCollector(myauth);
                myrcollector.Connect(tbHost.Text);
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- Collector Connect end ----------\n");
            }
        }

        private void doAutoUpdate_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- AutoUpdate check start ----------\n");
            try
            {
                FrameworkNG.AutoUpdateControlSpec myspec = new FrameworkNG.AutoUpdateControlSpec();
                FrameworkNG.CollectResult myres = myrcollector.Collect(myspec);
                NonBlockingPrint(wmiResults, "AutoUpdate probe decision: {0}\n", myspec.Decide(myres));
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- AutoUpdate check end ----------\n");
            }
        }

        private void doOSFingerprint_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- OS Fingerprinting START ----------\n");
            try
            {
                FrameworkNG.OSFPCollector myoscollector = new FrameworkNG.OSFPCollector();
                FrameworkNG.OSScanResult myRes = myoscollector.CollectOS(tbHost.Text);

                if (myRes.Best != null)
                {
                    NonBlockingPrint(wmiResults, String.Format("Best Match: {0}\n", myRes.Best.ToString()));
                    if (chShowAllMatches.IsChecked ?? false)
                    {
                        NonBlockingPrint(wmiResults, "    All Matches:\n");
                        for (int i = 0; i < myRes.Guesses.Count; i++)
                        {
                            NonBlockingPrint(wmiResults, String.Format("        Match #{0}: {1}\n", i + 1, myRes.Guesses[i].ToString()));
                        }
                    }
                }
                else
                    NonBlockingPrint(wmiResults, String.Format("No response from {0}\n", tbHost.Text));
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- OS Fingerprinting END ----------\n");
            }
        }

        public string GetFQDN()
        {
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();
            string fqdn = "";
            if (!String.IsNullOrEmpty(domainName) && !hostName.Contains(domainName))
                fqdn = hostName + "." + domainName;
            else
                fqdn = hostName;

            return fqdn.ToLower();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbPassword.Focus();

            foreach (var hive in Enum.GetValues(typeof(FrameworkNG.WMI.Registry.baseKey)))
                lbHive.Items.Add(hive);

            this.Title = "Experiências - " + GetFQDN();
        }

        private string SSID2Name(string ssid)
        {
            try
            {
                return new System.Security.Principal.SecurityIdentifier(ssid).Translate(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            catch
            {
                return "";
            }
        }

        private void doKeySearch_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- Registry Regex Search START ----------\n");
            try
            {
                if (tbVnamePattern.Text == "")
                {
                    ArrayList found = myrcollector.SearchKeys((FrameworkNG.WMI.Registry.baseKey)lbHive.SelectedItem, tbKeyPattern.Text);
                    foreach (string keymatch in found)
                    {
                        NonBlockingPrint(wmiResults, "Found: {0}\n", keymatch);
                    }
                }
                else
                {
                    ArrayList found = myrcollector.SearchKeys((FrameworkNG.WMI.Registry.baseKey)lbHive.SelectedItem, tbKeyPattern.Text, tbVnamePattern.Text);
                    foreach (FrameworkNG.WMI.Registry.RegKeyValue rkv in found)
                    {
                        NonBlockingPrint(wmiResults, "Found Key: {0} ValueName: {1}\n", rkv.KeyName, rkv.ValueName);
                    }
                }
                if ((FrameworkNG.WMI.Registry.baseKey)lbHive.SelectedItem == FrameworkNG.WMI.Registry.baseKey.HKEY_USERS)
                {
                    string ssid = tbKeyPattern.Text.Split('\\')[0];
                    NonBlockingPrint(wmiResults, "Account: {0}\n", SSID2Name(ssid));
                }
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- Registry Regex Search END ----------\n");
            }
        }

        private void doNICs_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- NIC Info test START ----------\n");
            try
            {
                List<FrameworkNG.InterfaceState> myNICs = mycollector.CollectNICInfo();
                foreach (FrameworkNG.InterfaceState thisNIC in myNICs)
                {
                    NonBlockingPrint(wmiResults, String.Format("Interface: {0}\n", thisNIC));
                    foreach (FrameworkNG.InterfaceState.IPInfo thisIP in thisNIC.InetAddr)
                    {
                        NonBlockingPrint(wmiResults, String.Format("    {0}/{1}; Broadcast {2}; {3}\n", thisIP.IPAddr, thisIP.IPMask, thisIP.IPBcast, thisIP.AddrType));
                    }
                }
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- NIC Info test END ----------\n");
            }
        }

        private void doSysInfo_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- System Info test START ----------\n");
            try
            {
                FrameworkNG.SysInfo myInfo = mycollector.CollectSysInfo();
                NonBlockingPrint(wmiResults, "{0}\n", myInfo);
                foreach (FrameworkNG.InterfaceState thisNIC in myInfo.Interfaces)
                {
                    NonBlockingPrint(wmiResults, String.Format("Interface: {0}\n", thisNIC));
                    foreach (FrameworkNG.InterfaceState.IPInfo thisIP in thisNIC.InetAddr)
                    {
                        NonBlockingPrint(wmiResults, String.Format("    {0}/{1}; Broadcast {2}; {3}\n", thisIP.IPAddr, thisIP.IPMask, thisIP.IPBcast, thisIP.AddrType));
                    }
                }
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- System Info test END ----------\n");
            }
        }

        private void doWinNetConnect_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- Windows UNC Connect START ----------\n");
            try
            {
                this.ConnectViaWindowsShare();
            }
            catch (System.ComponentModel.Win32Exception excp)
            {
                NonBlockingPrint(wmiResults, "Windows API error {0}: {1}\n", excp.ErrorCode, excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- Windows UNC Connect END ----------\n");
            }
        }

        private void ConnectViaWindowsShare()
        {
            myUNC = "\\\\" + tbHost.Text;
            WinNetUtils.connectToRemote(myUNC, tbDomain.Text + "\\" + tbUsername.Text, tbPassword.Password);
        }

        private void doSearchFile_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- Windows File Content START ----------\n");
            List<TextFileContent> fMatches = WinNetUtils.getWinTextFileContent(myUNC, tbRemoteFile.Text, tbKeyPattern.Text);
            foreach (TextFileContent fMatch in fMatches)
            {
                NonBlockingPrint(wmiResults, "{0}\n", fMatch);
            }
            NonBlockingPrint(wmiResults, "---------- Windows File Content END ----------\n");
        }

        private string BArrayToString(byte[] myArray)
        {
            string retVal = "";
            if (myArray == null)
                retVal = "Null";
            else
            {
                foreach (byte myByte in myArray)
                {
                    retVal += myByte.ToString("X2");
                }
            }
            return retVal;
        }

        private void doRSOP_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- RSOP Start ----------\n");
            try
            {
                ManagementScope scope = null;
                ConnectionOptions options = null;

                if (tbHost.Text.Length > 0)
                {
                    options = new ConnectionOptions();
                    options.Username = tbUsername.Text;
                    options.Password = tbPassword.Password;
                    // options.Impersonation = ImpersonationLevel.Impersonate;
                    // options.Authentication = System.Management.AuthenticationLevel.Default;
                    // options.EnablePrivileges = true;
                    if (cbAuth.Text == "NT")
                        options.Authority = String.Format("ntlmdomain:{0}", tbDomain.Text);
                    else
                        options.Authority = String.Format("kerberos:{0}", tbDomain.Text);
                    scope = new ManagementScope(String.Format("\\\\{0}\\root\\RSOP\\Computer", tbHost.Text), options);
                    scope.Connect();
                }

                ManagementObjectSearcher searcher;

                searcher = MyGetSearcher(scope, "SELECT * FROM RSOP_RegistryPolicySetting");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "\ncommand={0}\n", WMICollector.GetWMIProp(queryObj, "command"));
                    NonBlockingPrint(wmiResults, "creationTime={0}\n", WMICollector.GetWMIProp(queryObj, "creationTime"));
                    NonBlockingPrint(wmiResults, "deleted={0}\n", WMICollector.GetWMIProp(queryObj, "deleted"));
                    NonBlockingPrint(wmiResults, "GPOID={0}\n", WMICollector.GetWMIProp(queryObj, "GPOID"));
                    NonBlockingPrint(wmiResults, "id={0}\n", WMICollector.GetWMIProp(queryObj, "id"));
                    NonBlockingPrint(wmiResults, "name={0}\n", WMICollector.GetWMIProp(queryObj, "name"));
                    NonBlockingPrint(wmiResults, "precedence={0}\n", WMICollector.GetWMIProp(queryObj, "precedence"));
                    NonBlockingPrint(wmiResults, "registryKey={0}\n", WMICollector.GetWMIProp(queryObj, "registryKey"));
                    NonBlockingPrint(wmiResults, "SOMID={0}\n", WMICollector.GetWMIProp(queryObj, "SOMID"));
                    NonBlockingPrint(wmiResults, "value={0}\n", BArrayToString((byte[])WMICollector.GetWMIProp(queryObj, "value")));
                    NonBlockingPrint(wmiResults, "valueName={0}\n", WMICollector.GetWMIProp(queryObj, "valueName"));
                    NonBlockingPrint(wmiResults, "valueType={0}\n", WMICollector.GetWMIProp(queryObj, "valueType"));
                }

                searcher = MyGetSearcher(scope, "SELECT * FROM RSOP_RegistryKey");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "\nMode={0}\n", WMICollector.GetWMIProp(queryObj, "Mode"));
                    NonBlockingPrint(wmiResults, "Path={0}\n", WMICollector.GetWMIProp(queryObj, "Path"));
                    NonBlockingPrint(wmiResults, "precedence={0}\n", WMICollector.GetWMIProp(queryObj, "precedence"));
                    NonBlockingPrint(wmiResults, "SDDLString={0}\n", WMICollector.GetWMIProp(queryObj, "SDDLString"));
                }

                searcher = MyGetSearcher(scope, "SELECT * FROM RSOP_SecuritySettingString");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "\nKeyName={0}\n", WMICollector.GetWMIProp(queryObj, "KeyName"));
                    NonBlockingPrint(wmiResults, "precedence={0}\n", WMICollector.GetWMIProp(queryObj, "precedence"));
                    NonBlockingPrint(wmiResults, "Setting={0}\n", WMICollector.GetWMIProp(queryObj, "Setting"));
                }

                searcher = MyGetSearcher(scope, "SELECT * FROM RSOP_SecuritySettingNumeric");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "\nKeyName={0}\n", WMICollector.GetWMIProp(queryObj, "KeyName"));
                    NonBlockingPrint(wmiResults, "precedence={0}\n", WMICollector.GetWMIProp(queryObj, "precedence"));
                    NonBlockingPrint(wmiResults, "Setting={0}\n", WMICollector.GetWMIProp(queryObj, "Setting"));
                }

                searcher = MyGetSearcher(scope, "SELECT * FROM RSOP_SecuritySettingBoolean");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "\nKeyName={0}\n", WMICollector.GetWMIProp(queryObj, "KeyName"));
                    NonBlockingPrint(wmiResults, "precedence={0}\n", WMICollector.GetWMIProp(queryObj, "precedence"));
                    NonBlockingPrint(wmiResults, "Setting={0}\n", WMICollector.GetWMIProp(queryObj, "Setting"));
                }

                searcher = MyGetSearcher(scope, "SELECT * FROM RSOP_ScriptPolicySetting");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    NonBlockingPrint(wmiResults, "\nid={0}\n", WMICollector.GetWMIProp(queryObj, "id"));
                    NonBlockingPrint(wmiResults, "precedence={0}\n", WMICollector.GetWMIProp(queryObj, "precedence"));
                    NonBlockingPrint(wmiResults, "scriptList={0}\n", WMICollector.GetWMIProp(queryObj, "scriptList"));
                    NonBlockingPrint(wmiResults, "scriptorder={0}\n", WMICollector.GetWMIProp(queryObj, "scriptorder"));
                    NonBlockingPrint(wmiResults, "scriptType={0}\n", WMICollector.GetWMIProp(queryObj, "scriptType"));
                }
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "A ManagementException occurred while querying for WMI data: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for WMI data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- RSOP End ----------\n");
            }
        }

        private void doPolicy_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- Policy Begin ----------\n");
            try
            {
                WinNetUtils.UMInfo myInfo = WinNetUtils.getLoginPolicies(tbHost.Text);
                NonBlockingPrint(wmiResults, "usrmod0_force_logoff={0}\nusrmod0_max_passwd_age={1}\nusrmod0_min_passwd_age={2}\nusrmod0_min_passwd_len={3}\nusrmod0_password_hist_len={4}\n",
                    myInfo.u0.usrmod0_force_logoff.ToString("X8"),
                    myInfo.u0.usrmod0_max_passwd_age.ToString("X8"),
                    myInfo.u0.usrmod0_min_passwd_age.ToString("X8"),
                    myInfo.u0.usrmod0_min_passwd_len.ToString("X8"),
                    myInfo.u0.usrmod0_password_hist_len.ToString("X8")
                    );

                NonBlockingPrint(wmiResults, "usrmod1_primary={0}\nusrmod1_role={1}\n",
                    myInfo.u1.usrmod1_primary,
                    myInfo.u1.usrmod1_role.ToString("X8")
                    );

                NonBlockingPrint(wmiResults, "usrmod2_domain_id={0}\nusrmod2_domain_name={1}\n",
                    myInfo.u2.usrmod2_domain_id.ToString("X8"),
                    myInfo.u2.usrmod2_domain_name
                    );

                NonBlockingPrint(wmiResults, "usrmod3_lockout_duration={0}\nusrmod3_lockout_observation_window={1}\nusrmod3_lockout_threshold={2}\n",
                    myInfo.u3.usrmod3_lockout_duration.ToString("X8"),
                    myInfo.u3.usrmod3_lockout_observation_window.ToString("X8"),
                    myInfo.u3.usrmod3_lockout_threshold.ToString("X8")
                    );
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "A ManagementException occurred while querying for policy data: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for policy data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- Policy End ----------\n");
            }
        }

        private void doLSA_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- LSA Begin ----------\n");
            try
            {
                string fullUsername;
                if (tbDomain.Text == "")
                    fullUsername = tbUsername.Text;
                else
                    fullUsername = tbDomain.Text + "\\" + tbUsername.Text;

                int i = 0;
                List<string> tokens = WinNetUtils.getAccessTokens(tbHost.Text, tbRemoteFile.Text);
                foreach (string thisRight in tokens)
                {
                    NonBlockingPrint(wmiResults, "Right #{0}: {1}\n", ++i, thisRight);
                }
#if false
                IntPtr sid = IntPtr.Zero;
                int sidSize = 0;
                StringBuilder domainName = new StringBuilder();
                int nameSize = 0;
                int accountType = 0;

                WinNetUtils.LookupAccountName("\\\\" + tbHost.Text, fullUsername, sid, ref sidSize,
                    domainName, ref nameSize, ref accountType);
                domainName = new StringBuilder(nameSize);
                sid = Marshal.AllocHGlobal(sidSize);

                bool result = WinNetUtils.LookupAccountName("\\\\" + tbHost.Text, fullUsername, sid, ref sidSize,
                    domainName, ref nameSize, ref accountType);

                NonBlockingPrint(wmiResults, "LookupAccountName(): Result {0}, SID {1}\n", result, sid);
                
                WinNetUtils.LSA_UNICODE_STRING systemName = WinNetUtils.string2LSAUS("\\\\" + tbHost.Text);
                IntPtr policyHandle = IntPtr.Zero;
                WinNetUtils.LSA_OBJECT_ATTRIBUTES objAttrs = new WinNetUtils.LSA_OBJECT_ATTRIBUTES();
                uint retVal = WinNetUtils.LsaOpenPolicy(ref systemName, ref objAttrs,
                                    WinNetUtils.POLICY_LOOKUP_NAMES | WinNetUtils.POLICY_VIEW_LOCAL_INFORMATION, out policyHandle);

                NonBlockingPrint(wmiResults, "LsaOpenPolicy(): Result {0}, Policy Handle {1}\n", retVal, policyHandle);

                IntPtr rightsArray = IntPtr.Zero;
                ulong rightsCount = 0;
                long lretVal = WinNetUtils.LsaEnumerateAccountRights(policyHandle, sid, out rightsArray, out rightsCount);
                retVal = WinNetUtils.LsaNtStatusToWinError(lretVal);

                if (retVal != 0)
                    throw new System.ComponentModel.Win32Exception((int)retVal);

                NonBlockingPrint(wmiResults, "LsaEnumerateAccountRights(): Result {0}, RightsArray {1}, Count {2}\n",
                    retVal, rightsArray, rightsCount);

                WinNetUtils.LSA_UNICODE_STRING myLsaus = new WinNetUtils.LSA_UNICODE_STRING();

                for (ulong i = 0; i < rightsCount; i++)
                {
                    IntPtr itemAddr = new IntPtr(rightsArray.ToInt64() + (long)(i * (ulong) Marshal.SizeOf(myLsaus)));
                    myLsaus = (WinNetUtils.LSA_UNICODE_STRING)Marshal.PtrToStructure(itemAddr, myLsaus.GetType());
                    string thisRight = WinNetUtils.LSAUS2string(myLsaus);
                    NonBlockingPrint(wmiResults, "Right #{0}: {1}\n", i+1, thisRight);
                }
#endif
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "A ManagementException occurred while querying for security data: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for security data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- LSA End ----------\n");
            }
        }

        private void doUpdatesLocal_Click(object sender, RoutedEventArgs e)
        {
            /* NonBlockingPrint(wmiResults, "---------- Patch List BEGIN ----------\n");
            try
            { */
                var fixes = mycollector.CollectPatches();
                foreach (var entry in fixes)
                {
                    NonBlockingPrint(wmiResults, "{0}\n", entry);
                }
            /* }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "ManagementException occurred: {0}\n", excp.Message);
            }
            catch (FrameworkNG.CollectorException excp)
            {
                NonBlockingPrint(wmiResults, "CollectorException occurred: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "Exception of type {0} occurred: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- Patch List END ----------\n");
            } */
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- AccMan Begin ----------\n");
            try
            {
                string theGroupName = "", theGroupSid = "", theUserName = "", theUserSid = "";

                List<Principal> groups = AccManUtils.getGroupsByUser(accManContext, tbRemoteFile.Text);
                foreach (Principal group in groups)
                {
                    NonBlockingPrint(wmiResults, "Member of {0} (PrincipalName '{1}', SID {2})\n", group.Name, group.UserPrincipalName, group.Sid);
                    theGroupName = group.Name;
                    theGroupSid = group.Sid.ToString();
                }

                List<Principal> users = AccManUtils.getMembersOfGroup(accManContext, "Administrators");
                foreach (Principal user in users)
                {
                    NonBlockingPrint(wmiResults, "Administrators has {0} (PrincipalName '{1}', SID {2})\n", user.Name, user.UserPrincipalName, user.Sid);
                    if (theUserName == "")
                    {
                        theUserName = user.Name;
                        theUserSid = user.Sid.ToString();
                    }
                }

                if (theGroupSid != "")
                {
                    NonBlockingPrint(wmiResults, "\nSearching by Group SID {0}:\n", theGroupSid);
                    List<Principal> usersBySid = AccManUtils.getMembersOfGroupSid(accManContext, theGroupSid);
                    foreach (Principal user in usersBySid)
                    {
                        NonBlockingPrint(wmiResults, "Group {0}, with SID {1} has {2} (PrincipalName '{3}', SID {4})\n", theGroupName, theGroupSid, user.Name, user.UserPrincipalName, user.Sid);
                    }
                }

                if (theUserSid != "")
                {
                    NonBlockingPrint(wmiResults, "\nSearching by User SID {0}:\n", theUserSid);
                    List<Principal> groupsBySid = AccManUtils.getGroupsByUserSid(accManContext, theUserSid);
                    foreach (Principal group in groupsBySid)
                    {
                        NonBlockingPrint(wmiResults, "User {0}, with SID {1} belongs to {2} (SID {3})\n", theUserName, theUserSid, group.Name, group.Sid);
                    }
                }
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for security data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- AccMan End ----------\n");
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- AccMan Connect Begin ----------\n");
            try
            {
                accManContext = AccManUtils.accManConnect(tbHost.Text, tbDomain.Text + "\\" + tbUsername.Text, tbPassword.Password);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for security data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- AccMan Connect End ----------\n");
            }
        }

        private void doLSA2_Click(object sender, RoutedEventArgs e)
        {           
            NonBlockingPrint(wmiResults, "---------- LSA 2 Begin ----------\n");
            try
            {
                //this.ConnectViaWindowsShare();
                Dictionary<string, WinNetUtils.AuditEventStatus> tokens = WinNetUtils.getAuditPolicy(tbHost.Text);                
                foreach (var item in tokens)
                {
                    NonBlockingPrint(wmiResults, "{0}: {1}\n", item.Key, ((WinNetUtils.AuditEventStatus)item.Value).ToString());
                }
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "A ManagementException occurred while querying for security data: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for security data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- LSA 2 End ----------\n");
            }
        }

        private void doLSA3_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- LSA 3 Begin ----------\n");
            try
            {
                Dictionary<string, WinNetUtils.AuditEventStatus> tokens = WinNetUtils.GetAuditEventPolicies(tbHost.Text);
                foreach (var item in tokens)
                {
                    NonBlockingPrint(wmiResults, "{0}: {1}\n", item.Key, ((WinNetUtils.AuditEventStatus)item.Value).ToString());
                }
            }
            catch (ManagementException excp)
            {
                NonBlockingPrint(wmiResults, "A ManagementException occurred while querying for security data: {0}\n", excp.Message);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while querying for security data: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- LSA 3 End ----------\n");
            }
        }

        private void doSQL_Click(object sender, RoutedEventArgs e)
        {
            string myQuery = "SELECT * FROM useless";
            string driverStr = "PostgreSQL Unicode"; // "MySQL ODBC 5.1 Driver";

            NonBlockingPrint(wmiResults, "---------- SQL Begin ----------\n");
            try
            {
                string[] fldNames;
                List<object[]> sqlResults = SQLCollector.sqlDoSelect(driverStr, tbHost.Text, tbRemoteFile.Text, tbUsername.Text, tbPassword.Password, myQuery, out fldNames);

                string rowStr = "";
                foreach (string fldName in fldNames)
                {
                    if (rowStr == "")
                        rowStr = fldName;
                    else
                        rowStr += "," + fldName;
                }
                NonBlockingPrint(wmiResults, "{0}\n", rowStr);
                NonBlockingPrint(wmiResults, "{0}\n", new string('-', rowStr.Length));

                foreach (object[] thisRow in sqlResults)
                {
                    rowStr = "";
                    foreach (object thisVal in thisRow)
                    {
                        if (rowStr == "")
                            rowStr = thisVal.ToString();
                        else
                            rowStr += "," + thisVal.ToString();
                    }
                    NonBlockingPrint(wmiResults, "{0}\n", rowStr);
                }
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while making a SQL query: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(wmiResults, "---------- SQL End ----------\n");
            }
        }

        private void doMetabase_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(wmiResults, "---------- Metabase Start ----------\n");

#if false
            IntPtr myToken = IntPtr.Zero;
            DirectoryEntry myDirEntry = null;
            WindowsImpersonationContext myMission = null;
#endif
            try
            {
                var site /* entry */ = new DirectoryEntry("IIS://" + tbHost.Text + "/W3SVC/1", tbUsername.Text, tbPassword.Password);
                /* foreach (DirectoryEntry site in entry.Children)
                { */
                    NonBlockingPrint(wmiResults, "    Site {0}\n", site.Name);
                    foreach (PropertyValueCollection prop in site.Properties)
                    {
                        NonBlockingPrint(wmiResults, "        {0} (Type {1})\n", prop.PropertyName, prop.Value.GetType());
                        foreach (var thingInProp in prop)
                        {
                            NonBlockingPrint(wmiResults, "            {0}\n", thingInProp);
                        }
                    }
                /* } */


#if false
                // WinNetUtils.connectToRemote("\\\\" + tbHost.Text, tbUsername.Text, tbPassword.Password);

                if (LogonUser(tbUsername.Text, tbHost.Text, tbPassword.Password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref myToken) == 0)
                {
                    int causingError = GetLastError();
                    throw new System.ComponentModel.Win32Exception(causingError);
                }

                myMission = WindowsIdentity.Impersonate(myToken);

                string mbUri = "IIS://" + tbHost.Text + "/MimeMap";
                myDirEntry = new DirectoryEntry(mbUri);
                NonBlockingPrint(wmiResults, "{0}\n", myDirEntry.Properties["KeyType"]);

                myDirEntry.Close();
                myMission.Undo();
                if (myToken != IntPtr.Zero)
                    CloseHandle(myToken);

                // WinNetUtils.disconnectRemote("\\\\" + tbHost.Text);
#endif
            }
            catch (Exception excp)
            {
                NonBlockingPrint(wmiResults, "A {0} occurred while accessing a Metabase: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
#if false
                if (myDirEntry != null)
                    myDirEntry.Close();
                if (myMission != null)
                    myMission.Undo();
                if (myToken != IntPtr.Zero)
                    CloseHandle(myToken);
#endif
                NonBlockingPrint(wmiResults, "---------- Metabase End ----------\n");
            }
        }
    }
}
