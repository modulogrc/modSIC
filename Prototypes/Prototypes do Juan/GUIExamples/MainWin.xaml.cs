using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Tamir.SharpSsh;
using FrameworkNG.SshHelpers;
using MinimalisticTelnet;

namespace FrameworkNG
{
    /// <summary>
    /// Interaction logic for MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        private SshExec exec = null;

        private void ButtonsEnabled(bool isEnabled)
        {
            doRun.IsEnabled = isEnabled;
            doInfo.IsEnabled = isEnabled;
            doSysinfo.IsEnabled = isEnabled;
            doPkgInfo.IsEnabled = isEnabled;
            doServersInfo.IsEnabled = isEnabled;
            doProcsInfo.IsEnabled = isEnabled;
            doPattern.IsEnabled = isEnabled;
            doEnvVars.IsEnabled = isEnabled;
            doInetd.IsEnabled = isEnabled;
            doPasswd.IsEnabled = isEnabled;
            doAIXFilesets.IsEnabled = isEnabled;
            doRunlevel.IsEnabled = isEnabled;
            doXinetd.IsEnabled = isEnabled;
            doSmf.IsEnabled = isEnabled;
            doApache.IsEnabled = isEnabled;
        }

        private static void NonBlockingPrint(System.Windows.Controls.TextBox myBox, string myStr, params object[] args)
        {
            myBox.Text += String.Format(myStr, args);
            myBox.ScrollToEnd();
            System.Windows.Forms.Application.DoEvents();
        }

        public MainWin()
        {
            InitializeComponent();
        }

        private void ciscoLogin(TelnetConnection tc, string userName, string passWord)
        {
            string retString = tc.Login(tbUser.Text, tbPass.Password);
            if (!retString.EndsWith(">"))
                throw new ApplicationException("Failed to login: no '>' prompt received after login");

            char promptChar;
            ciscoCommand(tc, "terminal length 0", out promptChar);
            if (promptChar != '>')
                throw new ApplicationException("Failed to login: no '>' prompt received after terminal setup");
        }

        private void ciscoEnable(TelnetConnection tc, string enaPassword)
        {
            string output;

            tc.WriteLine("enable");
            output = tc.Read(tc.TimeOutLoginMs, ".*: *");
            if (!output.TrimEnd().EndsWith(":"))
                throw new ApplicationException("Failed to enable: unexpected output");

            tc.WriteLine(enaPassword);
            output = tc.Read(tc.TimeOutLoginMs, ".*[:#] *");
            if (!output.TrimEnd().EndsWith("#"))
                throw new ApplicationException("Failed to enable: authentication failure");
        }

        private string ciscoCommand(TelnetConnection tc, string cmd, out char promptChar)
        {
            promptChar = '\0';
            tc.WriteLine(cmd);
            // NonBlockingPrint(tbOutput, "DEBUG: Wrote line input:\n----------\n{0}\n----------\n", cmd);
            string output = tc.Read(tc.TimeOutReadMs, ".*[#>] *");
            // NonBlockingPrint(tbOutput, "DEBUG: Full output:\n----------\n{0}\n----------\n", output);

            // Parse away command echo
            if (output.StartsWith(cmd))
            {
                int startpos = cmd.Length;
                while ((startpos < output.Length) && ((output[startpos] == '\r') || (output[startpos] == '\n')))
                    startpos++;
                output = output.Substring(startpos);
            }

            // Parse away ending prompt
            int endpos = output.Length - 1;
            while (endpos >= 0)
            {
                if ((output[endpos] == ' ') || (output[endpos] == '\t'))
                    endpos--;
                else
                    break;
            }
            if (endpos >= 0)
            {
                promptChar = output[endpos];
                // NonBlockingPrint(tbOutput, "DEBUG: Setting promptChar to '{0}'\n", promptChar);
                while ((endpos >= 0) && (output[endpos] != '\r') && (output[endpos] != '\n'))
                    endpos--;
                output = output.Substring(0, endpos + 1);
            }

            return output;
        }

        private void doCisco_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(tbOutput, "----- Cisco BEGIN -----\n");
            try
            {
                CiscoIOSVersion myVer = new CiscoIOSVersion();
                myVer.VersionString = "11.7";
                myVer.VersionString = "11.7(7)";
                myVer.VersionString = "11.7(7xy)";
                myVer.VersionString = "11.7(7xy)T";
                myVer.VersionString = "11.7(7xy)T19";
                myVer.VersionString = "11.7(7xy)T7b";

                TelnetConnection tc = new TelnetConnection(tbHost.Text, int.Parse(tbPort.Text));
                tc.TimeOutLoginMs = 1000;
                tc.TimeOutReadMs = 30000;

                ciscoLogin(tc, tbUser.Text, tbPass.Password);
                NonBlockingPrint(tbOutput, "Logged in.\n");

                myVer = CiscoIOSHelper.CiscoGetVersion(tc);
                NonBlockingPrint(tbOutput, "-----\nVersion string: {0}\n", myVer);

                ciscoEnable(tc, tbPass.Password);
                NonBlockingPrint(tbOutput, "-----\nEnable succeeded.\n");
                if (!String.IsNullOrEmpty(tbCmd.Text))
                {
                    char promptChar;
                    NonBlockingPrint(tbOutput, "-----\n{0}", ciscoCommand(tc, tbCmd.Text, out promptChar));
                    NonBlockingPrint(tbOutput, "-----\nPrompt char: {0}\n", promptChar);
                }
                tc.Close();
            }
            catch (Exception excp)
            {
                NonBlockingPrint(tbOutput, "A {0} occurred while communicating: {1}\n", excp.GetType(), excp.Message);
            }
            finally
            {
                NonBlockingPrint(tbOutput, "----- Cisco END -----\n");
            }
        }

        private void doConnect_Click(object sender, RoutedEventArgs e)
        {
            if ((exec != null) && (exec.Connected))
            {
                if (Interaction.MsgBox("Session is connected, disconnect first?", MsgBoxStyle.YesNo, "Already Connected") == MsgBoxResult.Yes)
                {
                    exec.Close();
                    exec = null;
                    ButtonsEnabled(false);
                }
                else
                    return;
            }
            int sshTCPPort = int.Parse(tbPort.Text);
            exec = new SshExec(tbHost.Text, tbUser.Text, tbPass.Password);
            try
            {
                exec.Connect(sshTCPPort);
                ButtonsEnabled(true);
                NonBlockingPrint(tbOutput, "Connected to {0}:{1}\n", tbHost.Text, sshTCPPort);
            }
            catch (Exception excp)
            {
                NonBlockingPrint(tbOutput, "A {0} occurred while connecting: {1}\n", excp.GetType(), excp.Message);
            }
        }

        private void doRun_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbCmd.Text))
            {
                Interaction.MsgBox("Please enter a command to run.", MsgBoxStyle.OkOnly, "Invalid Input");
                tbCmd.Focus();
                return;
            }
            string output = exec.RunCommand(tbCmd.Text);
            NonBlockingPrint(tbOutput, "{0}\n", output);
        }

        private void doDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if ((exec != null) && (exec.Connected))
            {
                exec.Close();
                exec = null;
                ButtonsEnabled(false);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((exec != null) && (exec.Connected))
            {
                exec.Close();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tbPass.Focus();
        }

        private void doSysinfo_Click(object sender, RoutedEventArgs e)
        {
            SysInfo mySysInfo = SysInfoColllector.getSysInfo(exec);

            NonBlockingPrint(tbOutput, "{0}\n", mySysInfo);
            foreach (InterfaceState thisNIC in mySysInfo.Interfaces)
            {
                NonBlockingPrint(tbOutput, String.Format("Interface: {0}\n", thisNIC));
                foreach (InterfaceState.IPInfo thisIP in thisNIC.InetAddr)
                {
                    NonBlockingPrint(tbOutput, String.Format("    {0}/{1}; Broadcast {2}; {3}\n", thisIP.IPAddr, thisIP.IPMask, thisIP.IPBcast, thisIP.AddrType));
                }
            }

            UnameInfo myUnameInfo = UnameInfoCollector.getUnameInfo(exec);
            NonBlockingPrint(tbOutput, String.Format("Uname info: {0}\n", myUnameInfo));
        }

        private void doInfo_Click(object sender, RoutedEventArgs e)
        {
            int nFiles = 0;
            List<GenericFileInfo> myInfoList = FileInfoCollector.getFileInfo(exec, tbFile.Text);

            foreach (GenericFileInfo myInfo in myInfoList)
            {
                NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
                NonBlockingPrint(tbOutput, "{0}\n", myInfo);
                nFiles++;
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            NonBlockingPrint(tbOutput, "Total files: {0}\n", nFiles);

            nFiles = 0;
            List<FileHashInfo> myHashList = FileHashCollector.getFileHash(exec, tbFile.Text);

            foreach (FileHashInfo myInfo in myHashList)
            {
                NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
                NonBlockingPrint(tbOutput, "{0}\n", myInfo);
                nFiles++;
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            NonBlockingPrint(tbOutput, "Total file hashes: {0}\n", nFiles);
        }

        private void doPkgInfo_Click(object sender, RoutedEventArgs e)
        {
            int nFiles = 0;
            List<LinuxPackageInfo> myPgkList;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            if (String.IsNullOrEmpty(tbFile.Text))
                myPgkList = PackageInfoCollector.getPackageInfo(exec);
            else
            {
                myPgkList = new List<LinuxPackageInfo>();
                LinuxPackageInfo myPkg = PackageInfoCollector.getPackageInfo(exec, tbFile.Text);
                if (myPkg != null)
                    if (myPkg.Name != null)
                        myPgkList.Add(myPkg);
            }
            foreach (LinuxPackageInfo myPkg in myPgkList)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myPkg);
                nFiles++;
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            NonBlockingPrint(tbOutput, "Total packages: {0}\n", nFiles);
        }

        private void doServersInfo_Click(object sender, RoutedEventArgs e)
        {
            List<InetServerInfo> myServers;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myServers = ServersInfoCollector.getInetServerInfo(exec);
            foreach (InetServerInfo myServer in myServers)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myServer);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doProcsInfo_Click(object sender, RoutedEventArgs e)
        {
            List<UnixProcessInfo> myProcs;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myProcs = ProcessInfoCollector.getProcessInfo(exec);
            foreach (UnixProcessInfo myProc in myProcs)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myProc);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doPattern_Click(object sender, RoutedEventArgs e)
        {
            List<TextFileContent> myItems;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myItems = FileContentCollector.getTextFileContent(exec, tbFile.Text, tbPattern.Text);
            foreach (TextFileContent myItem in myItems)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myItem);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            // Como filtrar do lado do Unix: awk '/pattern/ {print}' <arquivo
            // Trocar todos os ' por '"'"'
            // Trocar todos os / por \/
            // Trocar todos os \ por \\
            /*
            Regex myRegex = new Regex("(LAUREL|HARDY) beats (GROUCHO|HARPO|ZEPPO)");
            MatchCollection myMatches = myRegex.Matches("....HARDY beats HARPO........LAUREL beats ZEPPO...");
            foreach (Match myMatch in myMatches)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myMatch);
                NonBlockingPrint(tbOutput, "Groups:\n");
                foreach (Group myGroup in myMatch.Groups)
                {
                    NonBlockingPrint(tbOutput, "        {0}\n", myGroup);
                    NonBlockingPrint(tbOutput, "        Captures:\n");
                    foreach (Capture myCap in myGroup.Captures)
                    {
                        NonBlockingPrint(tbOutput, "                {0}\n", myCap);
                    }
                }
            }
            */
        }

        private void doEnvVars_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> myVars;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myVars = EnvVarsCollector.getEnvVarsInfo(exec);
            foreach (KeyValuePair<string, string> myVar in myVars)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myVar);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doInetd_Click(object sender, RoutedEventArgs e)
        {
            List<InetdInfo> myServers;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myServers = InetdCollector.getInetdInfo(exec);
            foreach (InetdInfo myServer in myServers)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myServer);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doPasswd_Click(object sender, RoutedEventArgs e)
        {
            List<PasswdInfo> myPasswds;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            NonBlockingPrint(tbOutput, "PASSWD:\n");
            myPasswds = PasswdCollector.getPasswdInfo(exec);
            foreach (PasswdInfo myInfo in myPasswds)
            {
                NonBlockingPrint(tbOutput, "        {0}\n", myInfo);
            }
            NonBlockingPrint(tbOutput, "SHADOW:\n");
            List<ShadowInfo> myShadows;
            myShadows = ShadowCollector.getShadowInfo(exec);
            foreach (ShadowInfo myInfo in myShadows)
            {
                NonBlockingPrint(tbOutput, "        {0}\n", myInfo);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doAIXFilesets_Click(object sender, RoutedEventArgs e)
        {
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            NonBlockingPrint(tbOutput, "OS Level: {0}\n", AIXOSLevelCollector.getAIXOSLevel(exec));
            List<AIXFileset> myFilesets;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myFilesets = AIXFilesetCollector.getAIXFileset(exec);
            foreach (AIXFileset myFileset in myFilesets)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myFileset);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doRunlevel_Click(object sender, RoutedEventArgs e)
        {
            List<RunLevelInfo> myInfo;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            if (string.IsNullOrEmpty(tbFile.Text))
                myInfo = RunLevelCollector.getRunLevelInfo(exec, "3");
            else
                myInfo = RunLevelCollector.getRunLevelInfo(exec, tbFile.Text);
            foreach (RunLevelInfo myServer in myInfo)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myServer);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doXinetd_Click(object sender, RoutedEventArgs e)
        {
            List<XinetdInfo> myServers;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            XinetdCollector myCollector = new XinetdCollector(exec);
            myServers = myCollector.getXinetdInfo();
            foreach (XinetdInfo myServer in myServers)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myServer);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doSmf_Click(object sender, RoutedEventArgs e)
        {
            SolarisSmfInfo myInfo;
            SolarisPkgInfo myPkg;
            SolarisIsaInfo myIsa;
            Dictionary<ulong, SolarisPatchInfo> myPatches;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myIsa = SolarisIsaInfoCollector.getIsaInfo(exec);
            NonBlockingPrint(tbOutput, "{0}\n", myIsa);
            myPkg = SolarisPkgInfoCollector.getPkgInfo(exec, tbPattern.Text);
            NonBlockingPrint(tbOutput, "{0}\n", myPkg);
            myInfo = SolarisSvcPropCollector.getSmfInfo(exec, tbFile.Text);
            NonBlockingPrint(tbOutput, "{0}\n", myInfo);
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myPatches = SolarisPatchCollector.getPatchInfo(exec);
            foreach (KeyValuePair<ulong, SolarisPatchInfo> myPatch in myPatches)
            {
                NonBlockingPrint(tbOutput, "Patch: {0}\n", myPatch.Value);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }

        private void doApache_Click(object sender, RoutedEventArgs e)
        {
            List<ApacheInfo> myInfoList;
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
            myInfoList = ApacheCollector.getApacheInfo(exec);
            foreach (ApacheInfo myInfo in myInfoList)
            {
                NonBlockingPrint(tbOutput, "{0}\n", myInfo);
            }
            NonBlockingPrint(tbOutput, "------------------------------------------------------------\n");
        }
    }
}
