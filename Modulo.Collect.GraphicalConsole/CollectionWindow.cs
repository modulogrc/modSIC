#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
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
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Modulo.Collect.GraphicalConsole
{
    public partial class CollectionWindow : Form, ICollectionView
    {
        #region Public Members
        #endregion

        #region Private Members
        bool disableFormClose;
        private ServerConfiguration server;
        private TargetConfiguration target;
        private String definitioFilename;
        private String saveFolder;
        private String externalVariablesXml;
        private bool emptyOvalResults;
        #endregion

        #region Constructor
        public CollectionWindow(ServerConfiguration  _server, TargetConfiguration _target, String _definitionFilename, String _saveFolder, String _externalVariablesXml)
        {
            new CollectionController(this);

            server = _server;
            target = _target;
            definitioFilename = _definitionFilename;
            saveFolder = _saveFolder;
            externalVariablesXml = _externalVariablesXml;

            InitializeComponent();
            lbRequest.Text = Resource.RequestingCollection;
        }
        #endregion

        #region ICollectView Members
        public event EventHandler<RequestCollectionEvenArgs> OnRequestCollection;

        public void ShowRequestCollectMessage()
        {
            foreach (Control item in this.Controls)
            {
                if (item is Label || item is PictureBox)
                {
                    item.Hide();
                }
            }

            btnOK.Enabled = false;
            btnViewResults.Enabled = false;
            emptyOvalResults = false;
            this.Cursor = Cursors.WaitCursor;

            imgRequest.Show();
            lbRequest.Show();
        }

        public void ShowCollectionIdMessage(string collectrequest)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        imgRequest.Hide();

                        lbCollectId.Text = String.Format(Resource.ServerGeneratedID, collectrequest);
                        lbCollectId.Show();
                    }
                });
            }
        }

        public void ShowCollectingDataMessage(string address)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        lbCollecting.Text = String.Format(Resource.CollectingData, address);
                        imgCollecting.Show();
                        lbCollecting.Show();
                    }
                });
            }
        }

        public void ShowCollectionFinishedMessage()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {                       
                        imgCollecting.Hide();
                        lbCollectFinished.Text = Resource.CollectionFinished;
                        lbCollectFinished.Show();
                    }
                });
            }
        }

        public void ShowGetDocumentsMessage()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        imgGetDocuments.Show();
                        lbGetDocuments.Text = Resource.TryingFetchOVALDocuments;
                        lbGetDocuments.Show();
                    }
                });
            }
        }

        public void ShowSaveOvalResultsMessage()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        imgGetDocuments.Hide();
                        lbSaveResults.ForeColor = Color.Black;
                        lbSaveResults.Text = Resource.OVALResultsDocumentWasSaved;
                        lbSaveResults.Show();
                    }
                });
            }
        }

        public void ShowSaveSystemCharacteristicsMessage()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        lbSaveSystemCharacteristics.ForeColor = Color.Black;
                        lbSaveSystemCharacteristics.Text = Resource.OVALSystemCharacteristicsDocumentWasSaved;
                        lbSaveSystemCharacteristics.Show();
                    }
                });
            }
        }

        public void ShowSaveOvalResultsErrorMessage(string text)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        imgGetDocuments.Hide();
                        lbSaveResults.ForeColor = Color.Red;
                        lbSaveResults.Text = text;
                        lbSaveResults.Show();
                        emptyOvalResults = true;
                    }
                });
            }
        }

        public void ShowSaveSystemCharacteristicsErrorMessage(string text)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        lbSaveSystemCharacteristics.ForeColor = Color.Red;
                        lbSaveSystemCharacteristics.Show();
                    }
                });
            }
        }

        public void Finish()
        {
            disableFormClose = false;

            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        btnOK.Enabled = true;
                        if (!emptyOvalResults)
                        {
                            btnViewResults.Enabled = true;                            
                        }
                        this.Cursor = Cursors.Default;
                    }
                });
            }
        }

        public void ShowErrorMessage(string text)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        disableFormClose = false;
                        Dialog.Error(text);
                        this.Close();
                    }
                });
            }
        }
        #endregion

        #region Form Events
        private void CollectWindow_Load(object sender, EventArgs e)
        {
            disableFormClose = true;
            ShowRequestCollectMessage();
        }

        private void CollectWindow_Shown(object sender, EventArgs e)
        {
            var args = new RequestCollectionEvenArgs() { Server = this.server, Target = this.target, DefinitionFilename = this.definitioFilename, SaveFolder = this.saveFolder, ExternalVariablesXml = this.externalVariablesXml };
            OnRequestCollection(this, args);
        }

        private void CollectWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = disableFormClose;
        }

        private void btnViewResults_Click(object sender, EventArgs e)
        {
            OpenURL(String.Format("file:///{0}", Path.Combine(saveFolder, "results.html")));
        }

        private void OpenURL(string sUrl)
        {
            try
            {
                Process.Start(sUrl);
            }
            catch (Exception ex)
            {
                if (ex.GetType().ToString() != "System.ComponentModel.Win32Exception")
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("IExplore.exe", sUrl);
                    Process.Start(startInfo);
                    startInfo = null;
                }
            }
        }
        #endregion
    }
}
