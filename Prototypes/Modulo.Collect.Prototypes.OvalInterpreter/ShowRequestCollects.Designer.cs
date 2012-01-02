namespace ModuloOvalInterpreter
{
    partial class ShowRequestCollects
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSaveResults = new System.Windows.Forms.Button();
            this.lblOidValue = new System.Windows.Forms.Label();
            this.lblOidTitle = new System.Windows.Forms.Label();
            this.tabControlCollectResult = new System.Windows.Forms.TabControl();
            this.tabOvalResults = new System.Windows.Forms.TabPage();
            this.txtOvalResults = new System.Windows.Forms.TextBox();
            this.tabSystemCharacteristics = new System.Windows.Forms.TabPage();
            this.txtSystemCharacteristics = new System.Windows.Forms.TextBox();
            this.tabExecutionLog = new System.Windows.Forms.TabPage();
            this.txtExecutionLog = new System.Windows.Forms.TextBox();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.lblTargetAddressValue = new System.Windows.Forms.Label();
            this.lblTargetAddressTitle = new System.Windows.Forms.Label();
            this.lblCollectRequestIDValue = new System.Windows.Forms.Label();
            this.lblCollectRequestIDTitle = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.lstCollectionInExecution = new System.Windows.Forms.ListBox();
            this.lblRequestID = new System.Windows.Forms.Label();
            this.lblTargetAddress = new System.Windows.Forms.Label();
            this.lblClientID = new System.Windows.Forms.Label();
            this.lblReceveidOn = new System.Windows.Forms.Label();
            this.chkShowAllCollections = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.tabControlCollectResult.SuspendLayout();
            this.tabOvalResults.SuspendLayout();
            this.tabSystemCharacteristics.SuspendLayout();
            this.tabExecutionLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.MultiColumn = true;
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(225, 251);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 4;
            this.listBox1.ValueMemberChanged += new System.EventHandler(this.listBox1_ValueMemberChanged);
            this.listBox1.SelectedValueChanged += new System.EventHandler(this.listBox1_SelectedValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnSaveResults);
            this.groupBox1.Controls.Add(this.lblOidValue);
            this.groupBox1.Controls.Add(this.lblOidTitle);
            this.groupBox1.Controls.Add(this.tabControlCollectResult);
            this.groupBox1.Controls.Add(this.lblStatusValue);
            this.groupBox1.Controls.Add(this.lblStatusTitle);
            this.groupBox1.Controls.Add(this.lblTargetAddressValue);
            this.groupBox1.Controls.Add(this.lblTargetAddressTitle);
            this.groupBox1.Controls.Add(this.lblCollectRequestIDValue);
            this.groupBox1.Controls.Add(this.lblCollectRequestIDTitle);
            this.groupBox1.Location = new System.Drawing.Point(249, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 468);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // btnSaveResults
            // 
            this.btnSaveResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveResults.Location = new System.Drawing.Point(404, 70);
            this.btnSaveResults.Name = "btnSaveResults";
            this.btnSaveResults.Size = new System.Drawing.Size(96, 32);
            this.btnSaveResults.TabIndex = 9;
            this.btnSaveResults.Text = "Save (XML)...";
            this.btnSaveResults.UseVisualStyleBackColor = true;
            this.btnSaveResults.Click += new System.EventHandler(this.btnGravar_Click);
            // 
            // lblOidValue
            // 
            this.lblOidValue.AutoSize = true;
            this.lblOidValue.Location = new System.Drawing.Point(109, 14);
            this.lblOidValue.Name = "lblOidValue";
            this.lblOidValue.Size = new System.Drawing.Size(0, 13);
            this.lblOidValue.TabIndex = 8;
            // 
            // lblOidTitle
            // 
            this.lblOidTitle.AutoSize = true;
            this.lblOidTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOidTitle.Location = new System.Drawing.Point(6, 14);
            this.lblOidTitle.Name = "lblOidTitle";
            this.lblOidTitle.Size = new System.Drawing.Size(30, 13);
            this.lblOidTitle.TabIndex = 7;
            this.lblOidTitle.Text = "Oid:";
            // 
            // tabControlCollectResult
            // 
            this.tabControlCollectResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlCollectResult.Controls.Add(this.tabOvalResults);
            this.tabControlCollectResult.Controls.Add(this.tabSystemCharacteristics);
            this.tabControlCollectResult.Controls.Add(this.tabExecutionLog);
            this.tabControlCollectResult.Location = new System.Drawing.Point(9, 104);
            this.tabControlCollectResult.Name = "tabControlCollectResult";
            this.tabControlCollectResult.SelectedIndex = 0;
            this.tabControlCollectResult.Size = new System.Drawing.Size(498, 358);
            this.tabControlCollectResult.TabIndex = 6;
            this.tabControlCollectResult.SelectedIndexChanged += new System.EventHandler(this.tabControlCollectResult_SelectedIndexChanged);
            // 
            // tabOvalResults
            // 
            this.tabOvalResults.Controls.Add(this.txtOvalResults);
            this.tabOvalResults.Location = new System.Drawing.Point(4, 22);
            this.tabOvalResults.Name = "tabOvalResults";
            this.tabOvalResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabOvalResults.Size = new System.Drawing.Size(490, 332);
            this.tabOvalResults.TabIndex = 0;
            this.tabOvalResults.Text = "Oval Results";
            this.tabOvalResults.UseVisualStyleBackColor = true;
            // 
            // txtOvalResults
            // 
            this.txtOvalResults.AcceptsReturn = true;
            this.txtOvalResults.AcceptsTab = true;
            this.txtOvalResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOvalResults.Location = new System.Drawing.Point(3, 3);
            this.txtOvalResults.MaxLength = 2147483647;
            this.txtOvalResults.Multiline = true;
            this.txtOvalResults.Name = "txtOvalResults";
            this.txtOvalResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOvalResults.Size = new System.Drawing.Size(484, 326);
            this.txtOvalResults.TabIndex = 0;
            // 
            // tabSystemCharacteristics
            // 
            this.tabSystemCharacteristics.Controls.Add(this.txtSystemCharacteristics);
            this.tabSystemCharacteristics.Location = new System.Drawing.Point(4, 22);
            this.tabSystemCharacteristics.Name = "tabSystemCharacteristics";
            this.tabSystemCharacteristics.Padding = new System.Windows.Forms.Padding(3);
            this.tabSystemCharacteristics.Size = new System.Drawing.Size(490, 332);
            this.tabSystemCharacteristics.TabIndex = 1;
            this.tabSystemCharacteristics.Text = "System Characteristics";
            this.tabSystemCharacteristics.UseVisualStyleBackColor = true;
            // 
            // txtSystemCharacteristics
            // 
            this.txtSystemCharacteristics.AcceptsReturn = true;
            this.txtSystemCharacteristics.AcceptsTab = true;
            this.txtSystemCharacteristics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSystemCharacteristics.Location = new System.Drawing.Point(3, 3);
            this.txtSystemCharacteristics.MaxLength = 2147483647;
            this.txtSystemCharacteristics.Multiline = true;
            this.txtSystemCharacteristics.Name = "txtSystemCharacteristics";
            this.txtSystemCharacteristics.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSystemCharacteristics.Size = new System.Drawing.Size(484, 326);
            this.txtSystemCharacteristics.TabIndex = 0;
            // 
            // tabExecutionLog
            // 
            this.tabExecutionLog.Controls.Add(this.txtExecutionLog);
            this.tabExecutionLog.Location = new System.Drawing.Point(4, 22);
            this.tabExecutionLog.Name = "tabExecutionLog";
            this.tabExecutionLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabExecutionLog.Size = new System.Drawing.Size(490, 332);
            this.tabExecutionLog.TabIndex = 2;
            this.tabExecutionLog.Text = "Log";
            this.tabExecutionLog.UseVisualStyleBackColor = true;
            // 
            // txtExecutionLog
            // 
            this.txtExecutionLog.AcceptsReturn = true;
            this.txtExecutionLog.AcceptsTab = true;
            this.txtExecutionLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExecutionLog.Location = new System.Drawing.Point(3, 3);
            this.txtExecutionLog.MaxLength = 2147483647;
            this.txtExecutionLog.Multiline = true;
            this.txtExecutionLog.Name = "txtExecutionLog";
            this.txtExecutionLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtExecutionLog.Size = new System.Drawing.Size(484, 326);
            this.txtExecutionLog.TabIndex = 0;
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Location = new System.Drawing.Point(109, 79);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(0, 13);
            this.lblStatusValue.TabIndex = 5;
            // 
            // lblStatusTitle
            // 
            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusTitle.Location = new System.Drawing.Point(6, 79);
            this.lblStatusTitle.Name = "lblStatusTitle";
            this.lblStatusTitle.Size = new System.Drawing.Size(47, 13);
            this.lblStatusTitle.TabIndex = 4;
            this.lblStatusTitle.Text = "Status:";
            // 
            // lblTargetAddressValue
            // 
            this.lblTargetAddressValue.AutoSize = true;
            this.lblTargetAddressValue.Location = new System.Drawing.Point(109, 57);
            this.lblTargetAddressValue.Name = "lblTargetAddressValue";
            this.lblTargetAddressValue.Size = new System.Drawing.Size(0, 13);
            this.lblTargetAddressValue.TabIndex = 3;
            // 
            // lblTargetAddressTitle
            // 
            this.lblTargetAddressTitle.AutoSize = true;
            this.lblTargetAddressTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetAddressTitle.Location = new System.Drawing.Point(6, 57);
            this.lblTargetAddressTitle.Name = "lblTargetAddressTitle";
            this.lblTargetAddressTitle.Size = new System.Drawing.Size(97, 13);
            this.lblTargetAddressTitle.TabIndex = 2;
            this.lblTargetAddressTitle.Text = "Target Address:";
            // 
            // lblCollectRequestIDValue
            // 
            this.lblCollectRequestIDValue.AutoSize = true;
            this.lblCollectRequestIDValue.Location = new System.Drawing.Point(109, 35);
            this.lblCollectRequestIDValue.Name = "lblCollectRequestIDValue";
            this.lblCollectRequestIDValue.Size = new System.Drawing.Size(0, 13);
            this.lblCollectRequestIDValue.TabIndex = 1;
            // 
            // lblCollectRequestIDTitle
            // 
            this.lblCollectRequestIDTitle.AutoSize = true;
            this.lblCollectRequestIDTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCollectRequestIDTitle.Location = new System.Drawing.Point(6, 35);
            this.lblCollectRequestIDTitle.Name = "lblCollectRequestIDTitle";
            this.lblCollectRequestIDTitle.Size = new System.Drawing.Size(75, 13);
            this.lblCollectRequestIDTitle.TabIndex = 0;
            this.lblCollectRequestIDTitle.Text = "Request ID:";
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.SelectedPath = "C:\\Temp";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 266);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "In Execution";
            // 
            // lstCollectionInExecution
            // 
            this.lstCollectionInExecution.FormattingEnabled = true;
            this.lstCollectionInExecution.Location = new System.Drawing.Point(12, 282);
            this.lstCollectionInExecution.Name = "lstCollectionInExecution";
            this.lstCollectionInExecution.Size = new System.Drawing.Size(225, 95);
            this.lstCollectionInExecution.TabIndex = 7;
            this.lstCollectionInExecution.SelectedValueChanged += new System.EventHandler(this.lstCollectionInExecution_SelectedValueChanged);
            // 
            // lblRequestID
            // 
            this.lblRequestID.AutoSize = true;
            this.lblRequestID.Location = new System.Drawing.Point(9, 380);
            this.lblRequestID.Name = "lblRequestID";
            this.lblRequestID.Size = new System.Drawing.Size(67, 13);
            this.lblRequestID.TabIndex = 8;
            this.lblRequestID.Text = "Request: {0}";
            // 
            // lblTargetAddress
            // 
            this.lblTargetAddress.AutoSize = true;
            this.lblTargetAddress.Location = new System.Drawing.Point(9, 404);
            this.lblTargetAddress.Name = "lblTargetAddress";
            this.lblTargetAddress.Size = new System.Drawing.Size(99, 13);
            this.lblTargetAddress.TabIndex = 9;
            this.lblTargetAddress.Text = "Target Address: {0}";
            // 
            // lblClientID
            // 
            this.lblClientID.AutoSize = true;
            this.lblClientID.Location = new System.Drawing.Point(9, 430);
            this.lblClientID.Name = "lblClientID";
            this.lblClientID.Size = new System.Drawing.Size(67, 13);
            this.lblClientID.TabIndex = 10;
            this.lblClientID.Text = "Client ID: {0}";
            // 
            // lblReceveidOn
            // 
            this.lblReceveidOn.AutoSize = true;
            this.lblReceveidOn.Location = new System.Drawing.Point(9, 455);
            this.lblReceveidOn.Name = "lblReceveidOn";
            this.lblReceveidOn.Size = new System.Drawing.Size(90, 13);
            this.lblReceveidOn.TabIndex = 11;
            this.lblReceveidOn.Text = "Received On: {0}";
            // 
            // chkShowAllCollections
            // 
            this.chkShowAllCollections.AutoSize = true;
            this.chkShowAllCollections.Location = new System.Drawing.Point(200, 383);
            this.chkShowAllCollections.Name = "chkShowAllCollections";
            this.chkShowAllCollections.Size = new System.Drawing.Size(37, 17);
            this.chkShowAllCollections.TabIndex = 12;
            this.chkShowAllCollections.Text = "All";
            this.chkShowAllCollections.UseVisualStyleBackColor = true;
            // 
            // ShowRequestCollects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 484);
            this.Controls.Add(this.chkShowAllCollections);
            this.Controls.Add(this.lblReceveidOn);
            this.Controls.Add(this.lblClientID);
            this.Controls.Add(this.lblTargetAddress);
            this.Controls.Add(this.lblRequestID);
            this.Controls.Add(this.lstCollectionInExecution);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.Name = "ShowRequestCollects";
            this.Text = "ShowRequestCollects";
            this.Shown += new System.EventHandler(this.ShowRequestCollects_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControlCollectResult.ResumeLayout(false);
            this.tabOvalResults.ResumeLayout(false);
            this.tabOvalResults.PerformLayout();
            this.tabSystemCharacteristics.ResumeLayout(false);
            this.tabSystemCharacteristics.PerformLayout();
            this.tabExecutionLog.ResumeLayout(false);
            this.tabExecutionLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblTargetAddressValue;
        private System.Windows.Forms.Label lblTargetAddressTitle;
        private System.Windows.Forms.Label lblCollectRequestIDValue;
        private System.Windows.Forms.Label lblCollectRequestIDTitle;
        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.TabControl tabControlCollectResult;
        private System.Windows.Forms.TabPage tabOvalResults;
        private System.Windows.Forms.TextBox txtOvalResults;
        private System.Windows.Forms.TabPage tabSystemCharacteristics;
        private System.Windows.Forms.TextBox txtSystemCharacteristics;
        private System.Windows.Forms.TabPage tabExecutionLog;
        private System.Windows.Forms.TextBox txtExecutionLog;
        private System.Windows.Forms.Label lblOidValue;
        private System.Windows.Forms.Label lblOidTitle;
        private System.Windows.Forms.Button btnSaveResults;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstCollectionInExecution;
        private System.Windows.Forms.Label lblRequestID;
        private System.Windows.Forms.Label lblTargetAddress;
        private System.Windows.Forms.Label lblClientID;
        private System.Windows.Forms.Label lblReceveidOn;
        private System.Windows.Forms.CheckBox chkShowAllCollections;
    }
}