//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.IO;
//using System.Windows.Forms;
//using System.Xml;
//using System.Text;
//using System.Xml.Serialization;

//using Modulo.Collect.Service.Contract;
//using System.Runtime.InteropServices;
//using ActiveDs;
//using Modulo.Collect.Probe.Windows.FileContent.Helpers;
//using System.Collections;
//using Modulo.Collect.Probe.Common;
//using Modulo.Collect.Probe.Windows;
//using System.ServiceModel;
//using Microsoft.Win32;
//using System.Security.AccessControl;
//using Modulo.Collect.Probe.Windows.Helpers;
//using System.Security.Principal;
//using System.Security.Cryptography;
//using Modulo.Collect.Service.Contract.Security;
//using System.Security.Cryptography.X509Certificates;
//using Modulo.Collect.Service.Security;
//using System.Data.OleDb;
//using DevExpress.Xpo;
//using Variables = Modulo.Collect.OVAL.Variables;
//using Modulo.Collect.OVAL.Common;
//using Modulo.Collect.OVAL.Definitions;
//using Newtonsoft.Json;
//using Tamir.SharpSsh;
//using Modulo.Collect.Service.Controllers;
//using Modulo.Collect.Service.Data;
//using Quartz;
//using FrameworkNG.DependencyInjection;




//namespace ModuloOvalInterpreter
//{
//    public partial class frmMain : Form
//    {



//        public frmMain()
//        {
//            InitializeComponent();
//            //DevExpress.Xpo.XpoDefault.ConnectionString = appContext.DataConnectionString;
//        }

//        private string idRequestCollect;
//        private string CONFIGURATION_FILEPATH = @"c:\temp\moviconfiguration.txt";

//        private void button1_Click(object sender, EventArgs e)
//        {
//            SelectOvalDefinitionsFile(@"c:\");
//        }

//        private void SelectOvalDefinitionsFile(string initialDirectory)
//        {
//            var dialog = new OpenFileDialog() { InitialDirectory = initialDirectory, Title = "Select an oval definitions file" };
//            dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
//            if (dialog.ShowDialog() == DialogResult.OK)
//            {
//                Cursor = Cursors.WaitCursor;
//                try
//                {
//                    txtOvalDefinitions.Text = dialog.FileName;

//                    var ovalDefinitions = GetOvalDefinitions();
//                    externalVariablesPanel.Visible = ExistExternalVariables(ovalDefinitions);
//                    ConfigureExternalVariablesEditor(ovalDefinitions);
//                }
//                finally
//                {
//                    Cursor = Cursors.Default;
//                }
//            }
//        }

//        private Package GetCollectPackage(bool isLocal)
//        {
//            Request request = new Request();
//            request.DefinitionId = Guid.NewGuid().ToString();
//            request.Address = txtAddress.Text;
//            request.Credential = this.GetEncryptedCredentials(isLocal);


//            // request.Credential = ;
//            //request.ExternalVariables = GetExternalVariables();

//            return new Package()
//            {
//                Definitions = new DefinitionInfo[] { null },
//                CollectRequests = new Request[] { request },
//                ScheduleInformation = new ScheduleInformation() { ScheduleDate = this.sheduleDate.Value }
//            };
//        }



//        private string GetEncryptedCredentials(bool isLocal)
//        {
//            var credential = new Credential();

//            credential.Domain = txtDomain.Text;
//            credential.UserName = txtUserName.Text;
//            credential.Password = txtPassword.Text;

//            var credentialInBytes =
//                new CollectServiceCryptoProvider().EncryptCredentialBasedOnCertificateOfServer(
//                    credential, this.GetCertificate(isLocal));

//            return Encoding.Default.GetString(credentialInBytes);
//        }

//        private oval_definitions GetOvalDefinitionsFromStream(string ovalDefinitions)
//        {
//            IEnumerable<string> errors;
//            var docMS = new MemoryStream(Encoding.UTF8.GetBytes(ovalDefinitions));
//            var definitions = oval_definitions.GetOvalDefinitionsFromStream(docMS, out errors);
//            docMS.Position = 0;

//            return definitions;
//        }

//        private string GetVariableValueFromControl(string variableID)
//        {
//            foreach (var control in externalVariablesPanel.Controls)
//                if (control is TextBox)
//                    if (((TextBox)control).Tag.ToString().Equals(variableID))
//                        return ((TextBox)control).Text;

//            return string.Empty;
//        }

//        private Dictionary<string, string> GetAllExternalVariables()
//        {
//            var externalVariables = new Dictionary<string, string>();
//            foreach (var control in externalVariablesPanel.Controls)
//            {
//                if (control is TextBox)
//                {
//                    var variableID = ((TextBox)control).Tag.ToString();
//                    var variableValue = ((TextBox)control).Text;
//                    externalVariables.Add(variableID, variableValue);
//                }
//            }
//            return externalVariables;
//        }




//        private string GetExternalVariables()
//        {
//            if (externalVariablesPanel.Controls.Count <= 0)
//                return null;

//            var ovalVariables = CreateOvalVariables();
//            var definitions = GetOvalDefinitionsFromStream(GetOvalDefinitions());
//            if (definitions.variables != null)
//            {

//                foreach (var variable in definitions.variables.OfType<External_variable>())
//                {
//                    var varID = variable.id;
//                    var varValue = GetVariableValueFromControl(varID);
//                    var newVariableType
//                        = new Variables.VariableType(variable.datatype, varID, varValue);

//                    ovalVariables.variables.Add(newVariableType);
//                }
//            }

//            return ovalVariables.GetXmlDocument();
//        }

//        private Variables.oval_variables CreateOvalVariables()
//        {
//            return
//                new Variables.oval_variables()
//                    {
//                        generator = DocumentHelpers.GetDefaultGenerator(),
//                        variables = new List<Variables.VariableType>()
//                    };
//        }



//        private void ConfigureExternalVariablesEditor(string ovalDefinitions)
//        {
//            var definitions = GetOvalDefinitionsFromStream(ovalDefinitions);
//            var externalVariables = definitions.variables.OfType<External_variable>();
//            if (externalVariables.Count() <= 0)
//                return;

//            externalVariablesPanel.Controls.Clear();
//            int currentRow = 1;
//            foreach (var externalVariable in externalVariables)
//            {
//                var newTitleControl =
//                    new Label() { Text = externalVariable.id, AutoSize = true };

//                var newDatatypeControl =
//                    new Label() { Text = externalVariable.datatype.ToString(), AutoSize = true };

//                var newVariableTextBox =
//                    new TextBox { Tag = externalVariable.id, Text = externalVariable.comment, Multiline = true };

//                newVariableTextBox.SelectAll();

//                externalVariablesPanel.Controls.Add(newTitleControl, 1, currentRow);
//                currentRow++;
//                externalVariablesPanel.Controls.Add(newDatatypeControl, 1, currentRow);
//                currentRow++;
//                externalVariablesPanel.Controls.Add(newVariableTextBox, 1, currentRow);
//                currentRow++;
//            }

//            //var externalVariables = 
//            //    new Variables.oval_variables() 
//            //        { 
//            //            generator = DocumentHelpers.GetDefaultGenerator(), 
//            //            variables = new List<Variables.VariableType>() 
//            //        };


//            //if (definitions.variables != null)
//            //{
//            //    foreach (var variable in definitions.variables.OfType<External_variable>())
//            //    {
//            //        var varType = new Variables.VariableType(variable.datatype, variable.id, string.Empty);
//            //        externalVariables.variables.Add(varType);
//            //    }
//            //}

//            //return externalVariables.GetXmlDocument();
//        }

//        private CollectController CreateCollectController()
//        {
//            var newRequestCollectRepository = new RequestCollectRepository(XpoDefault.Session);
//            var appPath = Environment.CurrentDirectory;
//            var scheduler = InjectFactory.Resolve<ISchedulerFactory>().GetScheduler();

//            return new CollectController(newRequestCollectRepository, appPath, scheduler);
//        }

//        private X509Certificate2 GetCertificate(bool isLocal)
//        {
//            try
//            {
//                byte[] certificateInBytes = null;
//                if (isLocal)
//                    certificateInBytes = CreateCollectController().GetCertificate();
//                else
//                    certificateInBytes = this.GetCollectService().GetCertificate();



//                //X509Certificate2 certificate = new CertificateFactory(null).GetCertificate();
//                return new X509Certificate2(certificateInBytes);
//                //return certificate;                
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(string.Format("Error: {0}", ex.Message));
//            }
//            return null;
//        }

//        private DefinitionInfo GetOvalDefinitions()
//        {
//            XmlDocument xmlDoc = new XmlDocument();
//            try
//            {
//                xmlDoc.Load(this.txtOvalDefinitions.Text);
//            }
//            catch (XmlException e)
//            {
//                Console.WriteLine(e.Message);
//            }
//            // Now create StringWriter object to get data from xml document.
//            StringWriter sw = new StringWriter();
//            XmlTextWriter xw = new XmlTextWriter(sw);
//            xmlDoc.WriteTo(xw);
//            XmlTextReader rx = new XmlTextReader(this.txtOvalDefinitions.Text);


//            DefinitionInfo def = new DefinitionInfo();
//            def.Text = sw.ToString();
//            //def.Id = null;
//            return def;

//        }

//        private void btnExecute_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                lblInfo.Text = "Executing";
//                lblInfo.Visible = true;

//                var collectPackage = this.GetCollectPackage(true);
//                collectPackage.Date = DateTime.Now;

//                idRequestCollect = new InterpreterController().ExecuteCollect(collectPackage);

//                lblExecutionId.Text = "Execution Id: " + idRequestCollect;
//                lblInfo.Visible = true;

//                Application.DoEvents();
//            }
//            finally
//            {
//                lblInfo.Text = "Complete";
//            }
//        }


//        private void btnResults_Click(object sender, EventArgs e)
//        {
//            this.Cursor = Cursors.WaitCursor;
//            try
//            {
//                var frmResults = new FrmShowResults(GetAllExternalVariables());
//                frmResults.Show();
//                this.Cursor = Cursors.Default;
//            }
//            finally
//            {
//                this.Cursor = Cursors.Default;
//            }
//        }

//        private void frmMain_Shown(object sender, EventArgs e)
//        {
//            var databaseLocation = GetDatabaseLocationFromCurrentConnectionString();
//            this.Text = string.Format("Modulo OVAL Interpreter - {0}", databaseLocation);

//            try
//            {
//                var serializedConfiguration = System.IO.File.ReadAllText(CONFIGURATION_FILEPATH);
//                var moviConfiguration = JsonConvert.DeserializeObject<MOVIConfiguration>(serializedConfiguration);
//                this.SetConfigurationControllers(moviConfiguration);
//            }
//            catch (Exception)
//            {
//                this.SetConfigurationControllers(GetDefaultMOVIConfiguration());
//            }
//        }

//        private bool ExistExternalVariables(string ovalDefinitions)
//        {
//            var variables = GetOvalDefinitionsFromStream(ovalDefinitions).variables;
//            if (variables == null)
//                return false;

//            var externalVariables = variables.OfType<External_variable>();
//            return ((externalVariables != null) && (externalVariables.Count() > 0));
//        }

//        private void SetConfigurationControllers(MOVIConfiguration moviConfiguration)
//        {
//            txtAddress.Text = moviConfiguration.TargetAddress;
//            txtDomain.Text = moviConfiguration.Domain;
//            txtUserName.Text = moviConfiguration.Username;
//            txtOvalDefinitions.Text = moviConfiguration.OvalDefinitionsFilepath;

//            txtPassword.Focus();
//        }

//        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
//        {
//            System.Diagnostics.Process.GetCurrentProcess().Kill();
//        }

//        private void frmMain_DoubleClick(object sender, EventArgs e)
//        {

//            return;
//            //RegistryKey remoteKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "MSS-RJ-001.MSS.MODULO.COM.BR");
//            //string[] filter = txtExecutionId.Text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
//            //string subKey = filter.ElementAt(0);
//            //string SID = filter.ElementAt(1);

//            //WinNetUtils.connectToRemote("\\\\172.16.3.173", "Lab", "lab2002");
//            //RegistryKey remoteKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, txtAddress.Text).OpenSubKey(subKey);
//            //AuthorizationRuleCollection DACLs = remoteKey.GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

//            //foreach (RegistryAccessRule dacl in DACLs)
//            //    if (dacl.IdentityReference.Value.Equals(SID))
//            //    {
//            //        MessageBox.Show(((uint)dacl.RegistryRights).ToString());
//            //        return;
//            //    }

//            //MessageBox.Show(":-(");

//            return;


//            IntPtr pZero, pSid, psd = IntPtr.Zero;
//            AdvancedApi32Wrapper.SECURITY_INFORMATION sFlags = AdvancedApi32Wrapper.SECURITY_INFORMATION.Dacl;

//            char[] regKeyArray = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Modulo\\RiskManager".ToCharArray();
//            byte[] regKeyBytes = System.Text.Encoding.Unicode.GetBytes(regKeyArray);



//            uint errorReturn = AdvancedApi32Wrapper.GetNamedSecurityInfo(
//                System.Text.Encoding.Default.GetString(regKeyBytes, 0, regKeyArray.Length), AdvancedApi32Wrapper.SE_OBJECT_TYPE.SE_REGISTRY_KEY, sFlags, out pSid, out pZero, out pZero, out pZero, out psd);

//            if (errorReturn != 0)
//            {
//                throw (new Exception("An error of code: " + errorReturn + " has occured"));
//            }

//            MessageBox.Show(errorReturn.ToString());

//            return;

//            //TargetInfo targetInfo = new TargetInfo();
//            //targetInfo.Add("IPAddr", txtAddress.Text.Trim());

//            //if (!string.IsNullOrEmpty(txtUserName.Text.Trim()))
//            //{
//            //    targetInfo.credentials = new Credentials();
//            //    targetInfo.credentials.Add("UserName", txtUserName.Text);
//            //    targetInfo.credentials.Add("Password", txtPassword.Text);
//            //}

//            //string[] parameters = txtExecutionId.Text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
//            //string registryKey = parameters.ElementAt(0).Trim();
//            //string userName = parameters.ElementAt(1);
//            //string userSID = parameters.ElementAt(2);

//            //try
//            //{
//            //    AccessControlListProvider aclProvider = new AccessControlListProvider();
//            //    int accessMask = aclProvider.GetRegistryKeyEffectiveRights(targetInfo, registryKey, userName, userSID);

//            //    MessageBox.Show(accessMask.ToString());
//            //}
//            //catch (Exception ex)
//            //{
//            //    MessageBox.Show("Error: '" + ex.Message + "'");
//            //}
//        }

//        private void label5_Click(object sender, EventArgs e)
//        {

//        }

//        private void button3_Click(object sender, EventArgs e)
//        {
//            //try
//            //{
//            int numberOfCollects = int.Parse(this.txtNumberOfCollects.Text);
//            for (int i = 0; i < numberOfCollects; i++)
//            {
//                ICollectService collectService = this.GetCollectService();
//                Package collectPackageDTO = this.GetCollectPackage(false);

//                try
//                {
//                    //collectService.SendRequest( RequestCollect(collectPackageDTO));
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(string.Format("Error[Calling request Collect]: {0}", ex.Message));
//                }


//                try { System.IO.File.WriteAllText("DefinitionsFile.txt", txtOvalDefinitions.Text); }
//                catch (Exception) { }
//                //MessageBox.Show("Ok.");
//            }
//            //}
//            //catch (Exception ex)
//            //{
//            //    MessageBox.Show(string.Format("Error: {0}", ex.Message));
//            //}

//        }

//        public ICollectService GetCollectService()
//        {
//            try
//            {
//                WSHttpBinding httpBinding = new WSHttpBinding();
//                httpBinding.ReaderQuotas.MaxStringContentLength = 5242880;
//                httpBinding.ReaderQuotas.MaxArrayLength = 2097152;
//                httpBinding.MaxReceivedMessageSize = 5242880;
//                httpBinding.Security.Mode = SecurityMode.None;

//                Uri address = new Uri(this.txtServerUrl.Text);
//                EndpointAddress endPointAddress = new EndpointAddress(address.ToString());
//                ChannelFactory<Modulo.Collect.Service.Contract.ICollectService> channel = null;
//                channel = new ChannelFactory<ICollectService>(httpBinding, endPointAddress);
//                return channel.CreateChannel();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(string.Format("Error [GetCollectServiceChannel]: {0}", ex.Message));
//                throw ex;
//            }
//        }

//        private static void ConfigureWsDefaultBinding(WSHttpBinding wsDefaultBinding)
//        {
//            wsDefaultBinding.OpenTimeout = TimeSpan.FromMinutes(1f);
//            wsDefaultBinding.ReceiveTimeout = TimeSpan.FromMinutes(10f);
//            wsDefaultBinding.SendTimeout = TimeSpan.FromMinutes(10f);
//            wsDefaultBinding.CloseTimeout = TimeSpan.FromMinutes(1f);
//            wsDefaultBinding.BypassProxyOnLocal = false;
//            wsDefaultBinding.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
//            wsDefaultBinding.MaxBufferPoolSize = long.MaxValue;
//            wsDefaultBinding.MaxReceivedMessageSize = int.MaxValue;
//            wsDefaultBinding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;
//            wsDefaultBinding.TextEncoding = Encoding.UTF8;
//            wsDefaultBinding.UseDefaultWebProxy = true;
//            wsDefaultBinding.AllowCookies = false;
//            wsDefaultBinding.ReaderQuotas = new XmlDictionaryReaderQuotas();
//            wsDefaultBinding.ReaderQuotas.MaxDepth = int.MaxValue;
//            wsDefaultBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
//            wsDefaultBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
//            wsDefaultBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
//            wsDefaultBinding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
//            wsDefaultBinding.Security.Mode = SecurityMode.None;
//        }

//        private void frmMain_Load(object sender, EventArgs e)
//        {

//        }

//        private void button4_Click(object sender, EventArgs e)
//        {
//            var frmShowRequestCollects = new ShowRequestCollects();
//            frmShowRequestCollects.Show();
//        }

//        private void button5_Click(object sender, EventArgs e)
//        {
//            var frmShowExecutingCollect = new ShowExecutingCollect();
//            frmShowExecutingCollect.Show();
//        }

//        private void button6_Click(object sender, EventArgs e)
//        {

//        }

//        private void textBox1_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void button6_Click_1(object sender, EventArgs e)
//        {
//            var frmShowCollectDefinitions = new ShowCollectDefinitions();
//            frmShowCollectDefinitions.Show();

//            return;

//            try
//            {
//                SshExec ssh = new SshExec(txtAddress.Text, txtUserName.Text, txtPassword.Text);
//                ssh.Connect(22);

//                if (ssh.Connected)
//                    MessageBox.Show(";-)");
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
//            }

//            return;



//            var con = new OleDbConnection("Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=RM7_EN_BUILD9;Data Source=.\\SQLEXPRESS");
//            con.Open();

//            var cmd = new OleDbCommand("select ovaldefinitions from RequestCollect where oid='043553A3-644C-11DF-AA10-22BB91125840'", con);
//            var result = cmd.ExecuteScalar();

//            var ovaldef = result.ToString();
//            System.IO.File.WriteAllText(@"c:\temp\oval.xml", ovaldef);
//            textBox1.Text = ovaldef;
//        }


//        private static string GetDatabaseLocationFromCurrentConnectionString()
//        {
//            try
//            {
//                var currentConnectionString = XpoDefault.Session.ConnectionString;
//                var dbServerName = GetSectionValue(currentConnectionString, "Data Source", ";");
//                var dbCatalogName = GetSectionValue(currentConnectionString, "Initial Catalog", ";");

//                return string.Format("{0}@{1}", dbCatalogName, dbServerName);
//            }
//            catch (Exception)
//            {
//                return "[]";
//            }
//        }

//        private static string GetSectionValue(string sourceString, string sectionName, string sectionSeparator)
//        {
//            var endPositionOfSection = sourceString.IndexOf(sectionName) + sectionName.Length + 1;
//            var positionOfNextSeparatorOccurrenceAfterSection = sourceString.IndexOf(sectionSeparator, endPositionOfSection);
//            var sectionValueLength = positionOfNextSeparatorOccurrenceAfterSection - endPositionOfSection;

//            return sourceString.Substring(endPositionOfSection, sectionValueLength);
//        }

//        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            try
//            {
//                var configuration = new MOVIConfiguration() { TargetAddress = txtAddress.Text, Domain = txtDomain.Text, Username = txtUserName.Text, OvalDefinitionsFilepath = txtOvalDefinitions.Text };
//                var serializedConfiguration = JsonConvert.SerializeObject(configuration);
//                System.IO.File.WriteAllText(CONFIGURATION_FILEPATH, serializedConfiguration);
//            }
//            catch (Exception)
//            {

//            }
//        }

//        private void label1_DoubleClick(object sender, EventArgs e)
//        {
//            this.SetConfigurationControllers(GetDefaultMOVIConfiguration());
//        }

//        private MOVIConfiguration GetDefaultMOVIConfiguration()
//        {
//            return new MOVIConfiguration()
//            {
//                TargetAddress = "127.0.0.1",
//                Domain = Environment.UserDomainName,
//                Username = Environment.UserName,
//                OvalDefinitionsFilepath = txtOvalDefinitions.Text
//            };
//        }
//    }

//    public class MOVIConfiguration
//    {
//        public string TargetAddress { get; set; }
//        public string Domain { get; set; }
//        public string Username { get; set; }
//        public string OvalDefinitionsFilepath { get; set; }
//    }



//    public class AdvancedApi32Wrapper
//    {
//        public enum SE_OBJECT_TYPE
//        {
//            SE_UNKNOWN_OBJECT_TYPE = 0,
//            SE_FILE_OBJECT,
//            SE_SERVICE,
//            SE_PRINTER,
//            SE_REGISTRY_KEY,
//            SE_LMSHARE,
//            SE_KERNEL_OBJECT,
//            SE_WINDOW_OBJECT,
//            SE_DS_OBJECT,
//            SE_DS_OBJECT_ALL,
//            SE_PROVIDER_DEFINED_OBJECT,
//            SE_WMIGUID_OBJECT, SE_REGISTRY_WOW64_32KEY
//        }

//        [Flags]
//        public enum SECURITY_INFORMATION : uint
//        {
//            Owner = 0x00000001,
//            Group = 0x00000002,
//            Dacl = 0x00000004,
//            Sacl = 0x00000008,
//            ProtectedDacl = 0x80000000,
//            ProtectedSacl = 0x40000000,
//            UnprotectedDacl = 0x20000000,
//            UnprotectedSacl = 0x10000000
//        }


//        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
//        public static extern uint GetNamedSecurityInfo(
//            string pObjectName,
//            SE_OBJECT_TYPE ObjectType,
//            SECURITY_INFORMATION SecurityInfo,
//            out IntPtr pSidOwner,
//            out IntPtr pSidGroup,
//            out IntPtr pDacl,
//            out IntPtr pSacl,
//            out IntPtr pSecurityDescriptor);
//    }

//}
//// ==========================================================
//// ***** PLEASE, DON'T REMOVE THIS COMMENTED CODE BELOW *****
//// ==========================================================
//// HKEY_LOCAL_MACHINE\SOFTWARE\Modulo\RiskManager;mss\lfernandes;S-1-5-21-501351562-481299158-1019697294-10279
//// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\ADs;Lab;S-1-5-21-606747145-1563985344-842925246-1005
//// 
//// string key = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Modulo\\RiskManager";
//// IntPtr pZero, pDACL = IntPtr.Zero;
////try
////{
////    try
////    {
////        WinNetUtils.connectToRemote("\\\\172.16.3.173", "Lab", "lab2002");
////        ADsSecurityClass SD = new ADsSecurityClass();


////        //object secDescriptor = SD.GetSecurityDescriptor(@"RGY://HKEY_LOCAL_MACHINE\SOFTWARE\Modulo\RiskManager");
////        object secDescriptor = SD.GetSecurityDescriptor(@"RGY://172.16.3.173/HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\ADs");
////        //ADsSIDClass SID = (ADsSIDClass)SD.GetSID(@"WinNT://172.16.3.173/LAB-TESTE/Lab,user");


////        SecurityDescriptor x = (SecurityDescriptor)secDescriptor;
////        AccessControlList DACL = (AccessControlList)((SecurityDescriptor)secDescriptor).DiscretionaryAcl;
////        IEnumerator daclEnum = DACL.GetEnumerator();

////        string trustees = "";
////        while (daclEnum.MoveNext())
////        {
////            IADsAccessControlEntry ace = (IADsAccessControlEntry)daclEnum.Current;
////            trustees += ace.Trustee + "\r\n";
////        }



////        MessageBox.Show(trustees);
////        MessageBox.Show("OK");
////    }
////    catch (Exception ex)
////    {
////        MessageBox.Show(ex.Message);
////    }
////}
////finally
////{
////    WinNetUtils.disconnectRemote("172.16.3.173");
////}
