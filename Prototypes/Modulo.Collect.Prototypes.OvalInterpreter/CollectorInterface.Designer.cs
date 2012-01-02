namespace ModuloOvalInterpreter
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnSendCollect = new System.Windows.Forms.Button();
            this.lblAdress = new System.Windows.Forms.Label();
            this.llblUser = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtIpOrHostname = new System.Windows.Forms.TextBox();
            this.txtAssetAdminUsername = new System.Windows.Forms.TextBox();
            this.txtAssetAdminPassword = new System.Windows.Forms.TextBox();
            this.OvalDefinitionsPath = new System.Windows.Forms.Label();
            this.txtOvalDefinitionsFilepath = new System.Windows.Forms.TextBox();
            this.DefinitionsPathSearch = new System.Windows.Forms.Button();
            this.lblExecutionDate = new System.Windows.Forms.Label();
            this.dtExecutionSchedule = new System.Windows.Forms.DateTimePicker();
            this.externalVariablesPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtModsicPassword = new System.Windows.Forms.TextBox();
            this.txtModsicUsername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCollectServiceURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.imgGreenBall = new System.Windows.Forms.PictureBox();
            this.imgRedBall = new System.Windows.Forms.PictureBox();
            this.imgYellowBall = new System.Windows.Forms.PictureBox();
            this.btnRequestCollects = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtAdminPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAssetAdminDomain = new System.Windows.Forms.TextBox();
            this.lblDomain = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSaveExternalVariablesXml = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgGreenBall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgRedBall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgYellowBall)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSendCollect
            // 
            this.btnSendCollect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendCollect.Location = new System.Drawing.Point(178, 241);
            this.btnSendCollect.Name = "btnSendCollect";
            this.btnSendCollect.Size = new System.Drawing.Size(95, 33);
            this.btnSendCollect.TabIndex = 7;
            this.btnSendCollect.Text = "Execute Collect";
            this.btnSendCollect.UseVisualStyleBackColor = true;
            this.btnSendCollect.Click += new System.EventHandler(this.ExecuteCollect_Click);
            // 
            // lblAdress
            // 
            this.lblAdress.AutoSize = true;
            this.lblAdress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAdress.Location = new System.Drawing.Point(7, 16);
            this.lblAdress.Name = "lblAdress";
            this.lblAdress.Size = new System.Drawing.Size(76, 13);
            this.lblAdress.TabIndex = 1;
            this.lblAdress.Text = "IP / Hostname";
            // 
            // llblUser
            // 
            this.llblUser.AutoSize = true;
            this.llblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llblUser.Location = new System.Drawing.Point(196, 16);
            this.llblUser.Name = "llblUser";
            this.llblUser.Size = new System.Drawing.Size(55, 13);
            this.llblUser.TabIndex = 2;
            this.llblUser.Text = "Username";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Location = new System.Drawing.Point(9, 55);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password";
            this.lblPassword.DoubleClick += new System.EventHandler(this.lblPassword_DoubleClick);
            // 
            // txtIpOrHostname
            // 
            this.txtIpOrHostname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIpOrHostname.Location = new System.Drawing.Point(10, 32);
            this.txtIpOrHostname.Name = "txtIpOrHostname";
            this.txtIpOrHostname.Size = new System.Drawing.Size(383, 20);
            this.txtIpOrHostname.TabIndex = 2;
            // 
            // txtAssetAdminUsername
            // 
            this.txtAssetAdminUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAssetAdminUsername.Location = new System.Drawing.Point(198, 32);
            this.txtAssetAdminUsername.Name = "txtAssetAdminUsername";
            this.txtAssetAdminUsername.Size = new System.Drawing.Size(181, 20);
            this.txtAssetAdminUsername.TabIndex = 3;
            // 
            // txtAssetAdminPassword
            // 
            this.txtAssetAdminPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAssetAdminPassword.Location = new System.Drawing.Point(12, 71);
            this.txtAssetAdminPassword.Name = "txtAssetAdminPassword";
            this.txtAssetAdminPassword.PasswordChar = '*';
            this.txtAssetAdminPassword.Size = new System.Drawing.Size(178, 20);
            this.txtAssetAdminPassword.TabIndex = 5;
            // 
            // OvalDefinitionsPath
            // 
            this.OvalDefinitionsPath.AutoSize = true;
            this.OvalDefinitionsPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OvalDefinitionsPath.Location = new System.Drawing.Point(7, 16);
            this.OvalDefinitionsPath.Name = "OvalDefinitionsPath";
            this.OvalDefinitionsPath.Size = new System.Drawing.Size(100, 13);
            this.OvalDefinitionsPath.TabIndex = 0;
            this.OvalDefinitionsPath.Text = "Oval Definitions File";
            // 
            // txtOvalDefinitionsFilepath
            // 
            this.txtOvalDefinitionsFilepath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOvalDefinitionsFilepath.Location = new System.Drawing.Point(10, 32);
            this.txtOvalDefinitionsFilepath.Name = "txtOvalDefinitionsFilepath";
            this.txtOvalDefinitionsFilepath.Size = new System.Drawing.Size(354, 20);
            this.txtOvalDefinitionsFilepath.TabIndex = 1;
            this.txtOvalDefinitionsFilepath.Text = "C:\\temp\\definitions\\definitions.xml";
            // 
            // DefinitionsPathSearch
            // 
            this.DefinitionsPathSearch.Location = new System.Drawing.Point(335, 32);
            this.DefinitionsPathSearch.Name = "DefinitionsPathSearch";
            this.DefinitionsPathSearch.Size = new System.Drawing.Size(29, 20);
            this.DefinitionsPathSearch.TabIndex = 2;
            this.DefinitionsPathSearch.Text = "...";
            this.DefinitionsPathSearch.UseVisualStyleBackColor = true;
            this.DefinitionsPathSearch.Click += new System.EventHandler(this.DefinitionsPathSearch_Click);
            // 
            // lblExecutionDate
            // 
            this.lblExecutionDate.AutoSize = true;
            this.lblExecutionDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExecutionDate.Location = new System.Drawing.Point(5, 238);
            this.lblExecutionDate.Name = "lblExecutionDate";
            this.lblExecutionDate.Size = new System.Drawing.Size(80, 13);
            this.lblExecutionDate.TabIndex = 5;
            this.lblExecutionDate.Text = "Execution Date";
            // 
            // dtExecutionSchedule
            // 
            this.dtExecutionSchedule.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtExecutionSchedule.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtExecutionSchedule.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtExecutionSchedule.Location = new System.Drawing.Point(8, 254);
            this.dtExecutionSchedule.Name = "dtExecutionSchedule";
            this.dtExecutionSchedule.Size = new System.Drawing.Size(154, 20);
            this.dtExecutionSchedule.TabIndex = 6;
            this.dtExecutionSchedule.Value = new System.DateTime(2010, 12, 17, 0, 0, 0, 0);
            // 
            // externalVariablesPanel
            // 
            this.externalVariablesPanel.AutoScroll = true;
            this.externalVariablesPanel.BackColor = System.Drawing.Color.White;
            this.externalVariablesPanel.ColumnCount = 1;
            this.externalVariablesPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 96.92308F));
            this.externalVariablesPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.076923F));
            this.externalVariablesPanel.Location = new System.Drawing.Point(10, 71);
            this.externalVariablesPanel.Name = "externalVariablesPanel";
            this.externalVariablesPanel.RowCount = 1;
            this.externalVariablesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 96.19048F));
            this.externalVariablesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.809524F));
            this.externalVariablesPanel.Size = new System.Drawing.Size(384, 115);
            this.externalVariablesPanel.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtModsicPassword);
            this.groupBox1.Controls.Add(this.txtModsicUsername);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtCollectServiceURL);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.imgGreenBall);
            this.groupBox1.Controls.Add(this.imgRedBall);
            this.groupBox1.Controls.Add(this.imgYellowBall);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(406, 119);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " modSIC Service ";
            // 
            // txtModsicPassword
            // 
            this.txtModsicPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtModsicPassword.Location = new System.Drawing.Point(223, 90);
            this.txtModsicPassword.Name = "txtModsicPassword";
            this.txtModsicPassword.Size = new System.Drawing.Size(170, 20);
            this.txtModsicPassword.TabIndex = 5;
            this.txtModsicPassword.Text = "Pa$$w@rd";
            // 
            // txtModsicUsername
            // 
            this.txtModsicUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtModsicUsername.Location = new System.Drawing.Point(9, 90);
            this.txtModsicUsername.Name = "txtModsicUsername";
            this.txtModsicUsername.Size = new System.Drawing.Size(199, 20);
            this.txtModsicUsername.TabIndex = 3;
            this.txtModsicUsername.Text = "admin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(220, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Service URL";
            // 
            // txtCollectServiceURL
            // 
            this.txtCollectServiceURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCollectServiceURL.Location = new System.Drawing.Point(9, 43);
            this.txtCollectServiceURL.Name = "txtCollectServiceURL";
            this.txtCollectServiceURL.Size = new System.Drawing.Size(301, 20);
            this.txtCollectServiceURL.TabIndex = 1;
            this.txtCollectServiceURL.Text = "http://localhost:1000/CollectService";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Username";
            // 
            // imgGreenBall
            // 
            this.imgGreenBall.Image = ((System.Drawing.Image)(resources.GetObject("imgGreenBall.Image")));
            this.imgGreenBall.Location = new System.Drawing.Point(329, 19);
            this.imgGreenBall.Name = "imgGreenBall";
            this.imgGreenBall.Size = new System.Drawing.Size(64, 64);
            this.imgGreenBall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgGreenBall.TabIndex = 25;
            this.imgGreenBall.TabStop = false;
            this.toolTip1.SetToolTip(this.imgGreenBall, "Online (Idle)");
            this.imgGreenBall.Click += new System.EventHandler(this.imgGreenBall_Click);
            // 
            // imgRedBall
            // 
            this.imgRedBall.Image = ((System.Drawing.Image)(resources.GetObject("imgRedBall.Image")));
            this.imgRedBall.Location = new System.Drawing.Point(329, 19);
            this.imgRedBall.Name = "imgRedBall";
            this.imgRedBall.Size = new System.Drawing.Size(64, 64);
            this.imgRedBall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgRedBall.TabIndex = 24;
            this.imgRedBall.TabStop = false;
            this.toolTip1.SetToolTip(this.imgRedBall, "Offline");
            this.imgRedBall.DoubleClick += new System.EventHandler(this.imgRedBall_DoubleClick);
            // 
            // imgYellowBall
            // 
            this.imgYellowBall.Image = ((System.Drawing.Image)(resources.GetObject("imgYellowBall.Image")));
            this.imgYellowBall.Location = new System.Drawing.Point(329, 19);
            this.imgYellowBall.Name = "imgYellowBall";
            this.imgYellowBall.Size = new System.Drawing.Size(64, 64);
            this.imgYellowBall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgYellowBall.TabIndex = 26;
            this.imgYellowBall.TabStop = false;
            this.toolTip1.SetToolTip(this.imgYellowBall, "Online (Collecting)");
            // 
            // btnRequestCollects
            // 
            this.btnRequestCollects.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRequestCollects.Location = new System.Drawing.Point(279, 241);
            this.btnRequestCollects.Name = "btnRequestCollects";
            this.btnRequestCollects.Size = new System.Drawing.Size(123, 33);
            this.btnRequestCollects.TabIndex = 8;
            this.btnRequestCollects.Text = "Collect Management...";
            this.btnRequestCollects.UseVisualStyleBackColor = true;
            this.btnRequestCollects.Click += new System.EventHandler(this.btnRequestCollects_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.lblAdress);
            this.groupBox2.Controls.Add(this.txtIpOrHostname);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(406, 167);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Asset ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtAdminPassword);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.txtAssetAdminDomain);
            this.groupBox4.Controls.Add(this.lblDomain);
            this.groupBox4.Controls.Add(this.txtAssetAdminUsername);
            this.groupBox4.Controls.Add(this.txtAssetAdminPassword);
            this.groupBox4.Controls.Add(this.lblPassword);
            this.groupBox4.Controls.Add(this.llblUser);
            this.groupBox4.Location = new System.Drawing.Point(8, 58);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(385, 103);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Credentials";
            // 
            // txtAdminPassword
            // 
            this.txtAdminPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdminPassword.Location = new System.Drawing.Point(196, 71);
            this.txtAdminPassword.Name = "txtAdminPassword";
            this.txtAdminPassword.PasswordChar = '*';
            this.txtAdminPassword.Size = new System.Drawing.Size(183, 20);
            this.txtAdminPassword.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(196, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Admin Password";
            this.toolTip1.SetToolTip(this.label5, "Super User or Root Password.");
            this.label5.DoubleClick += new System.EventHandler(this.label5_DoubleClick);
            // 
            // txtAssetAdminDomain
            // 
            this.txtAssetAdminDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAssetAdminDomain.Location = new System.Drawing.Point(12, 32);
            this.txtAssetAdminDomain.Name = "txtAssetAdminDomain";
            this.txtAssetAdminDomain.Size = new System.Drawing.Size(178, 20);
            this.txtAssetAdminDomain.TabIndex = 2;
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDomain.Location = new System.Drawing.Point(9, 16);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(43, 13);
            this.lblDomain.TabIndex = 0;
            this.lblDomain.Text = "Domain";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSaveExternalVariablesXml);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.DefinitionsPathSearch);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.OvalDefinitionsPath);
            this.groupBox3.Controls.Add(this.txtOvalDefinitionsFilepath);
            this.groupBox3.Controls.Add(this.btnRequestCollects);
            this.groupBox3.Controls.Add(this.externalVariablesPanel);
            this.groupBox3.Controls.Add(this.lblExecutionDate);
            this.groupBox3.Controls.Add(this.btnSendCollect);
            this.groupBox3.Controls.Add(this.dtExecutionSchedule);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 301);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(406, 282);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Send Collect";
            // 
            // btnSaveExternalVariablesXml
            // 
            this.btnSaveExternalVariablesXml.Location = new System.Drawing.Point(8, 193);
            this.btnSaveExternalVariablesXml.Name = "btnSaveExternalVariablesXml";
            this.btnSaveExternalVariablesXml.Size = new System.Drawing.Size(169, 23);
            this.btnSaveExternalVariablesXml.TabIndex = 10;
            this.btnSaveExternalVariablesXml.Text = "Save External Variables";
            this.btnSaveExternalVariablesXml.UseVisualStyleBackColor = true;
            this.btnSaveExternalVariablesXml.Click += new System.EventHandler(this.btnSaveExternalVariablesXml_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(370, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(22, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(7, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "External Variables";
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "Status";
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 587);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmMain";
            this.Text = "Modsic Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Shown += new System.EventHandler(this.CollectorInterface_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgGreenBall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgRedBall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgYellowBall)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSendCollect;
        private System.Windows.Forms.Label lblAdress;
        private System.Windows.Forms.Label llblUser;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtIpOrHostname;
        private System.Windows.Forms.TextBox txtAssetAdminUsername;
        private System.Windows.Forms.TextBox txtAssetAdminPassword;
        private System.Windows.Forms.Label OvalDefinitionsPath;
        private System.Windows.Forms.TextBox txtOvalDefinitionsFilepath;
        private System.Windows.Forms.Button DefinitionsPathSearch;
        private System.Windows.Forms.Label lblExecutionDate;
        private System.Windows.Forms.DateTimePicker dtExecutionSchedule;
        private System.Windows.Forms.TableLayoutPanel externalVariablesPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCollectServiceURL;
        private System.Windows.Forms.Button btnRequestCollects;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtModsicUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtModsicPassword;
        private System.Windows.Forms.PictureBox imgGreenBall;
        private System.Windows.Forms.PictureBox imgRedBall;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox imgYellowBall;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtAssetAdminDomain;
        private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSaveExternalVariablesXml;
        private System.Windows.Forms.TextBox txtAdminPassword;
        private System.Windows.Forms.Label label5;
    }
}