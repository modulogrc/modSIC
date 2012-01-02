using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Modulo.Collect.Service.Client;
using Modulo.Collect.Service.Contract;
using Newtonsoft.Json;


namespace ModuloOvalInterpreter
{
    public partial class ShowRequestCollects : Form
    {
        private ModSicConnection ModSicApi;
        private List<RequestDTO> CurrentRequests;
        private bool Ready;
        private string ModsicUserpassword;
        private string ModsicUsername;
        private string LastClientRequestID;


        public ShowRequestCollects(
            ModSicConnection modsicApi, ModsicClientDB modsicDB, String modsicUsername, String modsicUserpassword, String lastClientRequestID)
        {
            this.Ready = false; 
            this.CurrentRequests = new List<RequestDTO>();
            this.ModSicApi = modsicApi;
            if (modsicDB != null)
                this.CurrentRequests = modsicDB.Requests;
            this.ModsicUsername = modsicUsername;
            this.ModsicUserpassword = modsicUserpassword;
            this.LastClientRequestID = lastClientRequestID;
            
            InitializeComponent();

            listBox1.DataSource = this.CurrentRequests;
            listBox1.DisplayMember = "RequestID";
            listBox1.ValueMember = "Oid";
            if (!String.IsNullOrWhiteSpace(this.LastClientRequestID))
                listBox1.SelectedValue = this.LastClientRequestID;

            timer1.Enabled = true;
        }

        private void listBox1_ValueMemberChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!Ready) return;


            Cursor = Cursors.WaitCursor;
            try
            {
                var currentRequest = ((RequestDTO)listBox1.SelectedItem);
                lblOidValue.Text = currentRequest.Oid;
                lblCollectRequestIDValue.Text = currentRequest.RequestID;
                lblTargetAddressValue.Text = currentRequest.TargetAddress;

                var collectionsInExecution = this.ModSicApi.GetCollectionsInExecution();
                var isCurrentCollectInExecution =
                    collectionsInExecution
                        .SingleOrDefault(x => x.CollectRequestId.Equals(currentRequest.RequestID)) != null;

                if (isCurrentCollectInExecution)
                {
                    tabControlCollectResult.Visible = false;
                    lblStatusValue.Text = "In Collection";
                }
                else
                {
                    txtExecutionLog.Clear();
                    try
                    {
                        var compressedLog = this.ModSicApi.GetFullCompressedLog(currentRequest.RequestID);
                        var uncompressedLog = JsonConvert.DeserializeObject<IEnumerable<ExecutionLog>>(compressedLog.ToUncompressedString());
                        var orderedExecutionLog = uncompressedLog.Select(x => new CollectExecutionLog(x)).OrderBy(x => x.Date).ToArray();
                        txtExecutionLog.Lines = orderedExecutionLog.Select(log => String.Format("[{0} - {1}] - {2}", log.LogType, log.Date.ToLocalTime(), log.Message)).ToArray();
                    }
                    catch (Exception ex)
                    {
                        txtExecutionLog.Text = String.Format("An error occurred while trying to get collect execution log: '{0}'", ex.Message);
                    }

                    tabControlCollectResult.Visible = true;
                    var collectResult = this.ModSicApi.GetResultDocument(currentRequest.RequestID);

                    if (collectResult == null)
                    {
                        MessageBox.Show("There is no results on server.");
                        return;
                    }


                    lblStatusValue.Text = collectResult.Status.ToString();
                    if (collectResult.Status == CollectStatus.Error)
                        return;

                    if (collectResult.Status == CollectStatus.Complete)
                    {
                        txtSystemCharacteristics.Text = collectResult.SystemCharacteristics;

                        try
                        {
                            txtOvalResults.Text = this.ModSicApi.GetOvalResults(currentRequest.RequestID);
                        }
                        catch (Exception ex)
                        {
                            txtOvalResults.Text = ex.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMsg = String.Format("Error while updating collect request '{0}' details: '{1}'", lblOidValue.Text, ex.Message);
                MessageBox.Show(errorMsg);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void tabControlCollectResult_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            var logFilepath = String.Format("{0}_execution_log.log", lblOidValue.Text);
            var ovalResultsFilepath = String.Format("{0}_oval_results.xml", lblOidValue.Text);
            var ovalSysCharacteristics = String.Format("{0}_oval_system_characteristics.xml", lblOidValue.Text);

            string selectedPath = @"C:\Temp";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                selectedPath = folderBrowserDialog.SelectedPath;
            }

            try
            {
                SaveFile(selectedPath, logFilepath, txtExecutionLog.Text);
                SaveFile(selectedPath, ovalResultsFilepath, txtOvalResults.Text);
                SaveFile(selectedPath, ovalSysCharacteristics, txtSystemCharacteristics.Text);               
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred while trying to save file: '{0}'", ex.Message));
            }
        }

        public void SaveFile(string selectedPath, string filename, string fileContents)
        {
            var filepath = Path.Combine(selectedPath, filename);
            System.IO.File.WriteAllText(filepath, fileContents);
        }

        private void ShowRequestCollects_Shown(object sender, EventArgs e)
        {
            Ready = true;
        }

        private CollectInfo[] GetCollectionInExecution()
        {
            if (chkShowAllCollections.Checked)
                return this.ModSicApi.GetAllCollectionsInExecution();

            return this.ModSicApi.GetCollectionsInExecution();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateScreenTitle();

            var selectedItemIndex = lstCollectionInExecution.SelectedIndex;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var runningCollections = this.GetCollectionInExecution();
                lstCollectionInExecution.Items.Clear();
                
                lstCollectionInExecution.DisplayMember = "Display";
                foreach (var collection in runningCollections)
                {
                    var item =
                        new 
                        { 
                            Display = collection.CollectRequestId + " on " + collection.Address,
                            CollectRequestID = collection.CollectRequestId,
                            ClientID = collection.ClientId,
                            Address = collection.Address,
                            ReceivedOn = collection.ReceivedOn.ToShortDateString() + " " + collection.ReceivedOn.ToShortTimeString()
                        };
                    
                    lstCollectionInExecution.Items.Add(item);
                }
            }
            finally
            {
                if (lstCollectionInExecution.Items.Count > 0)
                {
                    if (selectedItemIndex < lstCollectionInExecution.Items.Count)
                        lstCollectionInExecution.SelectedIndex = selectedItemIndex;
                }

                if (timer1.Interval != 5000)
                    timer1.Interval = 5000;
                this.Cursor = Cursors.Default;
            }
        }

        private void UpdateScreenTitle()
        {
            try
            {
                this.Text =
                    String.Format(
                        "API Version: {0} / modSIC Version: {1}",
                        ModSicApi.ServerAPIVersion.ToString(),
                        ModSicApi.ServerProgramVersion.ToString());
            }
            catch (Exception)
            {
            }
        }

        private void lstCollectionInExecution_SelectedValueChanged(object sender, EventArgs e)
        {
            dynamic currentCollection = (dynamic)lstCollectionInExecution.SelectedItem;
            if (currentCollection != null)
            {
                lblRequestID.Text = currentCollection.CollectRequestID;
                lblTargetAddress.Text = currentCollection.Address;
                lblClientID.Text = currentCollection.ClientID;
                lblReceveidOn.Text = currentCollection.ReceivedOn;
            }
        }

        
    }

    [JsonObject, Serializable]
    public class CollectExecutionLog
    {
        public CollectExecutionLog() { }
        public CollectExecutionLog(Modulo.Collect.Service.Contract.ExecutionLog x)
        {
            Date = x.Date;
            LogType = x.LogType;
            Message = x.Message;
        }

        public DateTime Date { get; set; }
        public string LogType { get; set; }
        public string Message { get; set; }
    }
}
