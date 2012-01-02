using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Contract.Security;
using Newtonsoft.Json;
using Variables = Modulo.Collect.OVAL.Variables;
using Modulo.Collect.Service.Client;


namespace ModuloOvalInterpreter
{
    public enum CollectServiceStates { Offline, Busy, Idle };

    public partial class frmMain : Form
    {
        private const string CONFIGURATION_FILEPATH = @"c:\temp\moviconfiguration.txt";
        private const string MODSIC_DB_FILEPATH = @"c:\temp\modsicDB.txt";
        
        private ModsicClientDB ModsicClientDB;
        private string LastCollectRequestID;
        private string LastClientRequestID;

        public frmMain(string logfile)
        {
            InitializeComponent();
        }

        private void DefinitionsPathSearch_Click(object sender, EventArgs e)
        {
            SelectOvalDefinitionsFile(String.IsNullOrEmpty(txtOvalDefinitionsFilepath.Text) ? @"C:\" : Path.GetDirectoryName(txtOvalDefinitionsFilepath.Text));
        }

        private void ExecuteCollect_Click(object sender, EventArgs e)
        {
            try
            {
                var package = GetCollectPackage();

                var modsicApi = CreateCollectService();
                var sendRequestInfo = modsicApi.SendCollect(package);

                this.LastCollectRequestID = sendRequestInfo.Requests.First().ServiceRequestId;
                this.LastClientRequestID = sendRequestInfo.Requests.First().ClientRequestId;
                UpdateModsicClientDB(sendRequestInfo, package.CollectRequests.First().Address);

                if (sendRequestInfo.HasErrors)
                    throw new Exception(sendRequestInfo.Message);

                MessageBox.Show(String.Format("Collect was scheduled successfully. Collect Request ID: '{0}'", this.LastCollectRequestID));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error on collect sending: '{0}'", ex.Message));
            }
        }

        private void UpdateModsicClientDB(SendRequestResult sendRequestInfo, string targetAddress)
        {
            if (this.ModsicClientDB == null)
                this.ModsicClientDB = new ModsicClientDB() { Requests = new List<RequestDTO>() };

            foreach (var request in sendRequestInfo.Requests)
            {
                var newRequestDTO = new RequestDTO(request.ClientRequestId, request.ServiceRequestId, targetAddress);
                this.ModsicClientDB.Requests.Add(newRequestDTO);
            }

            this.SerializeAndPersistObject(this.ModsicClientDB, MODSIC_DB_FILEPATH);
        }

        private void btnRequestCollects_Click(object sender, EventArgs e)
        {
            var modsicApi = CreateCollectService();
            new ShowRequestCollects(modsicApi, ModsicClientDB, txtModsicUsername.Text, txtModsicPassword.Text, this.LastClientRequestID).Show();
        }

        private OpenFileDialog GetOpenDialog(string initialDirectory)
        {
            return new OpenFileDialog() 
            { 
                InitialDirectory = initialDirectory, 
                Title = "Select an oval definitions file" ,
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
            };
        }

        private void SelectOvalDefinitionsFile(string initialDirectory)
        {
            var openDialog = GetOpenDialog(initialDirectory);
            if (openDialog.ShowDialog().Equals(DialogResult.OK))
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    txtOvalDefinitionsFilepath.Text = openDialog.FileName;
                    var ovalDefinitions = GetOvalDefinitions();
                    
                    if (!IsThereExternalVariables(ovalDefinitions.Text))
                        return;

                    ConfigureExternalVariablesEditor(ovalDefinitions.Text);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private DefinitionInfo GetOvalDefinitions()
        {
            var xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(txtOvalDefinitionsFilepath.Text);
                
                var sw = new StringWriter();
                var xw = new XmlTextWriter(sw);
                xmlDocument.WriteTo(xw);

                return new DefinitionInfo()  { Id = Guid.NewGuid().ToString(), Text = sw.ToString() };
            }
            catch (XmlException e)
            {
                var errorMsg = String.Format("An error occurred while trying to load oval definitions file: '{0}'", e.Message);
                MessageBox.Show(errorMsg);
                throw;
            }
        }

        private bool IsThereExternalVariables(string ovaldefinitions)
        {
            var definitions = GetOvalDefinitionsFromStream(ovaldefinitions);
            if (definitions == null)
                throw new Exception("God damn it!");

            if (definitions.variables == null)
                return false;

            var externalVariables = definitions.variables.OfType<VariablesTypeVariableExternal_variable>();
            return ((externalVariables != null) && (externalVariables.Count() > 0));
        }

        private void ConfigureExternalVariablesEditor(string ovalDefinitions)
        {
            var definitions = GetOvalDefinitionsFromStream(ovalDefinitions);
            if (definitions == null) return;

            var externalVariables = definitions.variables.OfType<VariablesTypeVariableExternal_variable>();
            if (externalVariables.Count() <= 0)
                return;

            externalVariablesPanel.Controls.Clear();
            int currentRow = 1;
            foreach (var externalVariable in externalVariables)
            {
                var newTitleControl =
                    new Label() { Text = externalVariable.id, AutoSize = true };

                var newDatatypeControl =
                    new Label() { Text = externalVariable.comment, AutoSize = true };

                var newVariableTextBox =
                    new TextBox { Tag = externalVariable.id, MaxLength = 1000, Text = externalVariable.datatype.ToString(), Width = 300, Anchor = AnchorStyles.Left | AnchorStyles.Right };

                newVariableTextBox.SelectAll();

                externalVariablesPanel.Controls.Add(newTitleControl, 1, currentRow);
                currentRow++;
                externalVariablesPanel.Controls.Add(newDatatypeControl, 1, currentRow);
                currentRow++;
                externalVariablesPanel.Controls.Add(newVariableTextBox, 1, currentRow);
                currentRow++;
            }
        }

        private oval_definitions GetOvalDefinitionsFromStream(string ovalDefinitions)
        {
            IEnumerable<string> errors;
            var docMS = new MemoryStream(Encoding.UTF8.GetBytes(ovalDefinitions));
            var definitions = oval_definitions.GetOvalDefinitionsFromStream(docMS, out errors);
            docMS.Position = 0;

            if ((definitions == null) || (errors.Count() > 0))
            {
                var joinedErrors = string.Join(Environment.NewLine, errors);
                var errorMsg = String.Format("The oval_definitions file cannot be load\r\n{0}", joinedErrors);
                MessageBox.Show(errorMsg);
            }

            return definitions;
        }

        public ModSicConnection CreateCollectService()
        {
            var modsicClientId = string.Format("ModsicClientWin32-{0}", System.Environment.MachineName);

            return new ModSicConnection(
                txtCollectServiceURL.Text,
                txtModsicUsername.Text,
                txtModsicPassword.Text,
                modsicClientId);
        }

        private WSHttpBinding CreateChuckNorrisBinding(Boolean isSecureChannel)
        {
            var chuckNorrisBinding = new WSHttpBinding()
            {
                MaxBufferPoolSize = Int32.MaxValue,
                MaxReceivedMessageSize = Int32.MaxValue,
                ReaderQuotas = new XmlDictionaryReaderQuotas()
                {
                    MaxArrayLength = Int32.MaxValue,
                    MaxBytesPerRead = Int32.MaxValue,
                    MaxNameTableCharCount = Int32.MaxValue,
                    MaxStringContentLength = Int32.MaxValue
                }
            };
 
            if (isSecureChannel)
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                chuckNorrisBinding.Security.Mode = SecurityMode.Transport;
            }
            else
            {
                chuckNorrisBinding.Security.Mode = SecurityMode.None;
            }
            
            return chuckNorrisBinding;
        }

        private Package GetCollectPackage()
        {
            var newDefinitionInfo = GetOvalDefinitions();
            return new Package()
            {
                Definitions = new DefinitionInfo[] { newDefinitionInfo },
                CollectRequests = new Request[] { CreateRequest(newDefinitionInfo.Id) },
                ScheduleInformation = new ScheduleInformation() { ScheduleDate = GetUtcDateTimeFromDateControl() }
            };
        }

        private Request CreateRequest(string definitionID)
        {
            return new Request()
            {
                DefinitionId = definitionID,
                RequestId = Guid.NewGuid().ToString(),
                Address = txtIpOrHostname.Text,
                Credential = GetEncryptedCredentials(),
                ExternalVariables = GetExternalVariables()
            };
        }

        private string GetExternalVariables()
        {
            if (externalVariablesPanel.Controls.Count <= 0)
                return null;

            var ovalVariables = CreateOvalVariables();
            var definitions = GetOvalDefinitionsFromStream(GetOvalDefinitions().Text);
            if ((definitions != null) && (definitions.variables != null))
            {

                foreach (var variable in definitions.variables.OfType<VariablesTypeVariableExternal_variable>())
                {
                    var varID = variable.id;
                    var varValue = GetVariableValueFromControl(varID);
                    var newVariableType = new Variables.VariableType(variable.datatype, varID, varValue);

                    ovalVariables.variables.Add(newVariableType);
                }
            }

            return ovalVariables.GetXmlDocument();
        }

        private Variables.oval_variables CreateOvalVariables()
        {
            return
                new Variables.oval_variables()
                    {
                        generator = DocumentHelpers.GetDefaultGenerator(),
                        variables = new List<Variables.VariableType>()
                    };
        }

        private string GetVariableValueFromControl(string variableID)
        {
            foreach (var control in externalVariablesPanel.Controls)
                if (control is TextBox)
                    if (((TextBox)control).Tag.ToString().Equals(variableID))
                        return ((TextBox)control).Text;

            return string.Empty;
        }

        private Dictionary<string, string> GetAllExternalVariables()
        {
            var externalVariables = new Dictionary<string, string>();
            foreach (var control in externalVariablesPanel.Controls)
            {
                if (control is TextBox)
                {
                    var variableID = ((TextBox)control).Tag.ToString();
                    var variableValue = ((TextBox)control).Text;
                    externalVariables.Add(variableID, variableValue);
                }
            }
            return externalVariables;
        }

        private DateTime GetUtcDateTimeFromDateControl()
        {
            var currentScheduleDate = dtExecutionSchedule.Value;
            
            var year = currentScheduleDate.Year;
            var month = currentScheduleDate.Month;
            var day = currentScheduleDate.Day;
            var hour = currentScheduleDate.Hour;
            var minute = currentScheduleDate.Minute;

            return new DateTime(year, month, day, hour, minute, 0).ToUniversalTime();
        }

        private string ExtractDomainFromUsername(string username)
        {
            if (username.Contains("\\"))
                return username.Split(new char[] { '\\' }).First();

            if (username.Contains("@"))
                return username.Split(new char[] { '@' }).First();

            return String.Empty;
        }

        private string GetEncryptedCredentials()
        {
            var domain = txtAssetAdminDomain.Text;
            var username = txtAssetAdminUsername.Text;
            var password = txtAssetAdminPassword.Text;
            var adminPassword = txtAdminPassword.Text;
            var credential = new Credential() { Domain = domain, UserName = username, Password = password, AdministrativePassword  = adminPassword };
            
            var credentialInBytes =
                new CollectServiceCryptoProvider().EncryptCredentialBasedOnCertificateOfServer(
                    credential, GetCertificate());
            
            return Encoding.Default.GetString(credentialInBytes);
        }

        private X509Certificate2 GetCertificate()
        {
            try
            {
                var modsicAPI = CreateCollectService();
                var certificateInBytes = modsicAPI.GetCertificate();

                return new X509Certificate2(certificateInBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}", ex.Message));
                return null;
            }
        }

        private void CollectorInterface_Shown(object sender, EventArgs e)
        {
            SwitchBalls(CollectServiceStates.Offline);

            try
            {
                this.ModsicClientDB = LoadModsicClientDB();
                var serializedConfiguration = System.IO.File.ReadAllText(CONFIGURATION_FILEPATH);
                var moviConfiguration = JsonConvert.DeserializeObject<ModsicConfiguration>(serializedConfiguration);
                this.SetConfigurationControllers(moviConfiguration);
                dtExecutionSchedule.Value = DateTime.Now;
            }
            catch (Exception)
            {
                this.SetConfigurationControllers(null);
            }
        }

        private ModsicClientDB LoadModsicClientDB()
        {
            if (System.IO.File.Exists(MODSIC_DB_FILEPATH))
            {
                var serializedDB = System.IO.File.ReadAllText(MODSIC_DB_FILEPATH);
                return JsonConvert.DeserializeObject<ModsicClientDB>(serializedDB);
            }

            System.IO.File.CreateText(MODSIC_DB_FILEPATH);
            return null;
        }

        private void SetConfigurationControllers(ModsicConfiguration moviConfiguration)
        {
            var defaultConfig = moviConfiguration == null;
            
            txtCollectServiceURL.Text = defaultConfig ? "http://localhost:800/Collector" : moviConfiguration.ModsicURL;
            txtModsicUsername.Text = defaultConfig ? "admin" : moviConfiguration.ModsicUsername;
            txtModsicPassword.Text = defaultConfig ? "Pa$$w@rd" : moviConfiguration.ModsicPassword;

            if (!defaultConfig)
            {
                txtIpOrHostname.Text = moviConfiguration.AssetIpOrHostname;
                txtAssetAdminUsername.Text = moviConfiguration.AssetUsername;
                txtAssetAdminPassword.Text = moviConfiguration.AssetPassword;
                txtAdminPassword.Text = moviConfiguration.AssetAdministrativePassword;
                txtOvalDefinitionsFilepath.Text = moviConfiguration.OvalDefinitionsFilepath;
            }
        }

        private void SwitchBalls(CollectServiceStates state)
        {
            switch (state)
            {
                case CollectServiceStates.Offline:
                    toolTip1.SetToolTip(imgRedBall, "Offline");
                    imgRedBall.Visible = true;
                    imgYellowBall.Visible = !imgRedBall.Visible;
                    imgGreenBall.Visible = !imgRedBall.Visible;
                    break;
                
                case CollectServiceStates.Busy:
                    toolTip1.SetToolTip(imgYellowBall, "Online (Collecting)");
                    imgYellowBall.Visible = true;
                    imgRedBall.Visible = !imgYellowBall.Visible;
                    imgGreenBall.Visible = !imgYellowBall.Visible;
                    break;
                
                case CollectServiceStates.Idle:
                    toolTip1.SetToolTip(imgGreenBall, "Online (Idle)");
                    imgGreenBall.Visible = true;
                    imgRedBall.Visible = !imgGreenBall.Visible;
                    imgYellowBall.Visible = !imgGreenBall.Visible;

                    break;
                
                default:
                    break;
            }

        }

        private void imgRedBall_DoubleClick(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                try
                {
                    CheckServiceIdleState();
                }
                catch (Exception ex)
                {
                    var errorMessage = String.Format("An error occurred while trying to connect to modSIC: '{0}'", ex.Message);
                    MessageBox.Show(errorMessage);
                }
            }
            finally
            {
                timer1.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void CheckServiceIdleState()
        {
            var modsicApi = CreateCollectService();
            try
            {
                var modsicVersion = modsicApi.Heartbeat();
                this.Text = String.Format("Server Version: {0}", modsicVersion);
                Application.DoEvents();
                SwitchBalls(CollectServiceStates.Idle);
            }
            catch (Exception ex)
            {
                SwitchBalls(CollectServiceStates.Offline);
                toolTip1.SetToolTip(imgRedBall, string.Format("Error [Heartbeat Call]: {0}", ex.Message));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                CheckServiceIdleState();
                try
                {
                    var modsicApi = CreateCollectService();
                    var collectsInExecution = modsicApi.GetCollectionsInExecution();
                    if ((collectsInExecution != null) && (collectsInExecution.Count() > 0))
                        SwitchBalls(CollectServiceStates.Busy);
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
                SwitchBalls(CollectServiceStates.Offline);
            }

            finally
            {
                Cursor.Current = Cursors.Default;
                Application.DoEvents();
                timer1.Enabled = true;
            }
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                var configuration = CreateModsicConfigurationFromGUI();
                SerializeAndPersistObject(configuration, CONFIGURATION_FILEPATH);
            }
            catch (Exception)
            {

            }
        }

        private void SerializeAndPersistObject(object value, string filepath)
        {
            var serializedObject = JsonConvert.SerializeObject(value);
            System.IO.File.WriteAllText(filepath, serializedObject);
        }

        private ModsicConfiguration CreateModsicConfigurationFromGUI()
        {
            return new ModsicConfiguration
            {
                ModsicURL = txtCollectServiceURL.Text,
                ModsicUsername = txtModsicUsername.Text,
                ModsicPassword = txtModsicPassword.Text,
                AssetIpOrHostname = txtIpOrHostname.Text,
                AssetUsername = txtAssetAdminUsername.Text,
                AssetPassword = txtAssetAdminPassword.Text,
                AssetAdministrativePassword = txtAdminPassword.Text,
                OvalDefinitionsFilepath = txtOvalDefinitionsFilepath.Text
            };
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void imgGreenBall_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ServerCollectionsList(txtCollectServiceURL.Text, txtModsicUsername.Text, txtModsicPassword.Text).Show();
        }

        private void btnSaveExternalVariablesXml_Click(object sender, EventArgs e)
        {
            var xml = GetExternalVariables();

            if (String.IsNullOrEmpty(xml))
            {
                return;
            }
            
            string selectedPath = @"C:\Temp";
            try
            {
                var filepath = Path.Combine(selectedPath, "external-variables.xml");
                System.IO.File.WriteAllText(filepath, xml);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred while trying to save file: '{0}'", ex.Message));
            }
        }

        private void lblPassword_DoubleClick(object sender, EventArgs e)
        {
            if (txtAssetAdminPassword.PasswordChar == '\0')
                txtAssetAdminPassword.PasswordChar = '*';
            else
                txtAssetAdminPassword.PasswordChar = '\0';
        }

        private void label5_DoubleClick(object sender, EventArgs e)
        {
            if (txtAdminPassword.PasswordChar == '\0')
                txtAdminPassword.PasswordChar = '*';
            else
                txtAdminPassword.PasswordChar = '\0';
        }
    }

    public class ModsicConfiguration
    {
        public string ModsicURL { get; set; }
        public string ModsicUsername { get; set; }
        public string ModsicPassword { get; set; }

        public string AssetIpOrHostname { get; set; }
        public string AssetUsername { get; set; }
        public string AssetPassword { get; set; }
        public string AssetAdministrativePassword { get; set; }
        
        public string OvalDefinitionsFilepath { get; set; }
    }

    public class ModsicClientDB
    {
        public List<RequestDTO> Requests;
    }

    public class RequestDTO
    {
        public String Oid { get; set; }

        public String RequestID { get; set; }

        public String TargetAddress { get; set; }

        public RequestDTO(string oid, string requestID, string targetAddress)
        {
            this.Oid = oid;
            this.RequestID = requestID;
            this.TargetAddress = targetAddress;
        }

        public override string ToString()
        {
            return String.Format("{0};{1};{2}", Oid, RequestID, TargetAddress);
        }
    }
}
