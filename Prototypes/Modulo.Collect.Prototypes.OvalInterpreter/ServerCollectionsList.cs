using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Modulo.Collect.Service.Client;


namespace ModuloOvalInterpreter
{
    public partial class ServerCollectionsList : Form
    {
        private ModSicConnection ModSicApi;

        public ServerCollectionsList(string modSicURL, string modSicUsername, string modSicUserPassword)
        {
            this.ModSicApi = new ModSicConnection(modSicURL, modSicUsername, modSicUserPassword);
            InitializeComponent();
        }

        private void spinStartID_DoubleClick(object sender, EventArgs e)
        {
            spinStartID.ResetText();
        }

        private void btnLoadServerCollectionsList_Click(object sender, EventArgs e)
        {
            if (spinStartID.Value == 0)
                spinStartID.Value = 1;

            txtCollections.Items.Clear();
            for (int i = (int)spinStartID.Value; i < 10; i++)
            {
                var currentRequestID = String.Format("collectrequests/{0}", i);
                ThreadStart threadStart = delegate { TryToGetCollectRequest(currentRequestID); };
                var thread = new Thread(threadStart);
                try
                {
                    thread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(":-( - " + ex.Message);
                    thread.Abort();
                }
            }
            //txtCollections.Invoke(
            statusStrip1.Text = "Complete";


        }

        public void AddItemToList(string contents)
        {
            txtCollections.Items.Add(contents);
        }

        public void TryToGetCollectRequest(string collectRequestID)
        {
            lock (txtCollections)
            {
                var itemIndex = txtCollections.Items.Add(String.Format("Getting {0}...", collectRequestID));
                var collectionsInExecution = this.ModSicApi.GetCollectionsInExecution().AsQueryable();
                if (collectionsInExecution.Count(c => c.CollectRequestId.Equals(collectRequestID)) > 0)
                {
                    var address = collectionsInExecution.Select(c => c.Address).First();
                    txtCollections.Items[itemIndex] = new RequestCollectDTO(collectRequestID, "In Execution", address);
                    return;
                }

                var results = this.ModSicApi.GetOvalResults(collectRequestID);
                var status = String.IsNullOrWhiteSpace(results) ? "Partial" : "Complete";
                txtCollections.Items[itemIndex] = new RequestCollectDTO(collectRequestID, status);
            }
        }

        public class RequestCollectDTO
        {
            public RequestCollectDTO(string collectRequestID, string status, string targetAddres = null)
            {
                this.CollectRequestID = collectRequestID;
                this.CollectStatus = status;
                this.TargetAddress = targetAddres ?? string.Empty;
            }

            public string CollectRequestID { get; private set; }
            public string CollectStatus { get; private set; }
            public string TargetAddress { get; private set; }

            public String ToString()
            {
                return String.Format("{0}:{1}@{2}", CollectRequestID, CollectStatus, TargetAddress);
            }

            
        }
    }
}
