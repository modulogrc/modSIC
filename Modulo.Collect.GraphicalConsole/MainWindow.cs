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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Modulo.Collect.OVAL.Definitions;
using System.Text;
using Modulo.Collect.OVAL.Helper;

namespace Modulo.Collect.GraphicalConsole
{
    public partial class MainWindow : Form, IConfigurationView, ISchemaView, IExternalVariable
    {
        #region IConfigurationView Members
        public ServerConfiguration Server { get; set; }
        public TargetConfiguration Target { get; set; }
        public String DestinationFolder { get; set; }
        public String DefinitionFilename { get; set; }

        public event EventHandler OnReadConfiguration;
        public event EventHandler OnWriteConfiguration;

        public void ShowErrorMessage(string text)
        {
            Dialog.Error(text);
        }
        #endregion

        #region ISchemaView Members
        public event EventHandler<SchemaEventArgs> OnValidateSchema;
        #endregion

        #region IExternalVariable Members
        public string ExternalVariablesXml { get; set; }
        public IEnumerable<VariablesTypeVariableExternal_variable> ExternalVariables { get; set; }
        public Dictionary<string, string> ExternalVariablesValues { get; set; }
        #endregion

        #region Private Members
        Label lbSSHPort = null;
        TextBox txtSSHPort = null;
        #endregion

        #region Constructor
        public MainWindow()
        {
            new ConfigurationController(this);
            new SchemaController(this);

            InitializeComponent();
            this.Text = AboutBox.AssemblyTitle;
            lbDefName.ForeColor = Color.Red;

            ReadConfiguration();
            ShowSSHPortWhenNecessary();
        }
        #endregion

        #region SSH Port
        private void ShowSSHPortWhenNecessary()
        {
            if (IsUnixDefinition(DefinitionFilename))
            {
                lbSSHPort = new Label() { Text = Resource.SSHPort, Parent = grTarget, Location = new Point() { X = 19, Y = 112 }, AutoSize = true };
                txtSSHPort = new TextBox() { Parent = grTarget, Location = new Point() { X = 79, Y = 109 }, Size = new Size() { Width = 128, Height = 20 } };
                if (!String.IsNullOrEmpty(Target.SSHPort))
                {
                    txtSSHPort.Text = Target.SSHPort;
                }
                this.Size = new Size() { Width = 520, Height = 460};
            }
            else
            {
                if (lbSSHPort != null && txtSSHPort != null)
                {
                    lbSSHPort.Parent = null;
                    txtSSHPort.Parent = null;

                    lbSSHPort = null;
                    txtSSHPort = null;

                    this.Size = new Size() { Width = 520, Height = 437 };
                }
            }
        }

        public bool IsUnixDefinition(string filename)
        {
            if (!String.IsNullOrEmpty(filename))
            {
                var stream = GetStream(filename);
                IEnumerable<string> errors = null;
                var ovalDefinitions = oval_definitions.GetOvalDefinitionsFromStream(stream, out errors);
                var x = new TargetPlatformDiscoverer(ovalDefinitions.objects);
                var family = x.Discover();
                if (family.ToString() == "unix")
                {
                    return true;
                }
            }
            return false;
        }

        public virtual Stream GetStream(string filename)
        {
            string fileContent = File.ReadAllText(filename);
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            return stream;
        }

        #endregion

        #region Form Events
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string GetText(OptionsWindow window, string name)
        {
            var control = window.Controls.Find(name, true).First() as TextBox;
            return control.Text;
        }

        private void SetText(OptionsWindow window, string name, string text)
        {
            var control = window.Controls.Find(name, true).First() as TextBox;
            control.Text = text;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var window = new OptionsWindow();
            if (Server != null)
            {
                SetText(window, "tbModSicAddr", Server.Address);
                SetText(window, "tbModSicUser", Server.Username);
                SetText(window, "tbModSicPass", Server.Password);
                SetText(window, "tbModSicPort", Server.Port);
            }

            window.Icon = this.Icon;

            if (window.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Server.Address = GetText(window, "tbModSicAddr");
                Server.Username = GetText(window, "tbModSicUser");
                Server.Password = GetText(window, "tbModSicPass");
                Server.Port = GetText(window, "tbModSicPort");
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var window = new AboutBox();
            window.Icon = this.Icon;
            window.ShowDialog(this);
            window.Dispose();
        }

        private void btChooseDefs_Click(object sender, EventArgs e)
        {
            chooserOval.Filter = Resource.OVALDefinitionsFilter;
            chooserOval.CheckFileExists = true;
            chooserOval.Title = Resource.SelectOVALDefinitionsFile;
            chooserOval.RestoreDirectory = true;
            if (String.IsNullOrEmpty(tbOvalDefs.Text))
            {
                chooserOval.InitialDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Application.ExecutablePath)) + Path.DirectorySeparatorChar + "Samples";
            }
            DialogResult res = chooserOval.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                tbOvalDefs.Text = chooserOval.FileName;

                var args = new SchemaEventArgs();
                args.DefinitionFilename = tbOvalDefs.Text;
                OnValidateSchema(this, args);

                ExternalVariablesValues = new Dictionary<string, string>();
                ExternalVariables = null;
                DefinitionFilename = String.Empty;

                if (args.Result)
                {
                    if (CheckExternalVariables(tbOvalDefs.Text, true))
                    {
                        DefinitionFilename = tbOvalDefs.Text;
                    }
                }
                else
                {
                    lbDefName.Text = args.ShortErrorMessage;
                    lbDefName.ForeColor = Color.Red;
                    if (args.LongErrorMessage != null)
                    {
                        ShowSchemaWindow(args);
                    }
                }

                ShowSSHPortWhenNecessary();
            }
        }

        private bool CheckExternalVariables(string path, bool verbose = false)
        {
            string errors;

            ExternalVariables = new ExternalVariableHelper().GetExternalVariablesFromFile(path, out errors);

            if (!String.IsNullOrEmpty(errors))
            {
                if (verbose)
                {
                    ShowErrorMessage(errors);                   
                } 
                return false;
            }
            else
            {
                lbDefName.Text = Path.GetFileName(path);
                lbDefName.ForeColor = Color.Black;
                return true;
            }
        }

        private void btSaveDir_Click(object sender, EventArgs e)
        {
            chooserFolder.ShowNewFolderButton = true;
            chooserFolder.Description = Resource.SelectFolderToSaveResults;
            DialogResult res = chooserFolder.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                tbSaveFolder.Text = tbSaveFolder.Text = chooserFolder.SelectedPath;
            }
        }

        private void btGo_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            btGo.Enabled = false;

            try
            {
                WriteConfiguration();

                if (String.IsNullOrEmpty(Server.Address) || String.IsNullOrEmpty(Server.Password)
                    || String.IsNullOrEmpty(Server.Port) || String.IsNullOrEmpty(Server.Username))
                {
                    Dialog.Error(Resource.InvalidServerConfiguration);
                }
                else if (String.IsNullOrEmpty(Target.Address) || String.IsNullOrEmpty(Target.Username)
                    || String.IsNullOrEmpty(Target.Password) || (txtSSHPort != null && String.IsNullOrEmpty(Target.SSHPort)))
                {
                    Dialog.Error(Resource.InvalidTargetConfiguration);
                }
                if (String.IsNullOrEmpty(DefinitionFilename))
                {
                    Dialog.Error(Resource.EmptyDefinitionFilename);
                }
                else if (String.IsNullOrEmpty(DestinationFolder))
                {
                    Dialog.Error(Resource.EmptyDestinationFolderName);
                }
                else
                {
                    var hasExternalVariable = ((ExternalVariables != null) && (ExternalVariables.Count() > 0));
                    if (hasExternalVariable)
                    {
                        var externalVariableWindow = new ExternalVariableWindow(this);
                        if (externalVariableWindow.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }
                    }

                    SaveExternalVariableXml();

                    var collectionWindow = new CollectionWindow(this.Server, this.Target, this.DefinitionFilename, this.DestinationFolder, this.ExternalVariablesXml);
                    collectionWindow.ShowDialog();
                }
            }
            finally
            {
                btGo.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void SaveExternalVariableXml()
        {
            try
            {
                var externalVariablesFilename = Path.Combine(DestinationFolder, "external-variables.xml");
                File.WriteAllText(externalVariablesFilename, ExternalVariablesXml);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(Util.FormatExceptionMessage(ex));
            }
        }

        private void ShowSchemaWindow(SchemaEventArgs args)
        {
            var window = new SchemaWindow(args.LongErrorMessage);
            window.ShowDialog();
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            WriteConfiguration();
        }
        #endregion

        #region Configuration
        private void ReadConfiguration()
        {
            OnReadConfiguration(this, EventArgs.Empty);

            if (Target != null)
            {
                tbTarget.Text = Target.Address;
                tbTargetUsername.Text = Target.Username;
                tbTargetPassword.Text = Target.Password;
                tbAdminPassword.Text = Target.AdministrativePassword;
            }

            tbSaveFolder.Text = DestinationFolder ?? String.Empty;
            if (!ValidateSchema())
            {
                DefinitionFilename = String.Empty;               
            }
        }

        private void WriteConfiguration()
        {
            if (Target != null)
            {
                Target.Address = tbTarget.Text;
                Target.Username = tbTargetUsername.Text;
                Target.Password = tbTargetPassword.Text;
                Target.AdministrativePassword = tbAdminPassword.Text;
                Target.SSHPort = txtSSHPort != null ? txtSSHPort.Text : null;
            }
            DestinationFolder = tbSaveFolder.Text;

            OnWriteConfiguration(this, EventArgs.Empty);
        }

        private bool ValidateSchema()
        {
            if (File.Exists(DefinitionFilename))
            {
                var args = new SchemaEventArgs();
                args.DefinitionFilename = DefinitionFilename;
                OnValidateSchema(this, args);

                ExternalVariablesValues = new Dictionary<string, string>();
                ExternalVariables = null;

                if (args.Result)
                {
                    if (CheckExternalVariables(DefinitionFilename))
                    {
                        tbOvalDefs.Text = DefinitionFilename;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
