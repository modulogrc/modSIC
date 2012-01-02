//namespace ModuloOvalInterpreter
//{
//    partial class frmMain
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.txtAddress = new System.Windows.Forms.TextBox();
//            this.label1 = new System.Windows.Forms.Label();
//            this.label2 = new System.Windows.Forms.Label();
//            this.txtUserName = new System.Windows.Forms.TextBox();
//            this.label3 = new System.Windows.Forms.Label();
//            this.txtPassword = new System.Windows.Forms.TextBox();
//            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
//            this.label4 = new System.Windows.Forms.Label();
//            this.txtOvalDefinitions = new System.Windows.Forms.TextBox();
//            this.btnExecute = new System.Windows.Forms.Button();
//            this.button1 = new System.Windows.Forms.Button();
//            this.lblInfo = new System.Windows.Forms.Label();
//            this.btnResults = new System.Windows.Forms.Button();
//            this.lblExecutionId = new System.Windows.Forms.Label();
//            this.label7 = new System.Windows.Forms.Label();
//            this.sheduleDate = new System.Windows.Forms.DateTimePicker();
//            this.button3 = new System.Windows.Forms.Button();
//            this.button4 = new System.Windows.Forms.Button();
//            this.groupBox2 = new System.Windows.Forms.GroupBox();
//            this.grpCollectResults = new System.Windows.Forms.GroupBox();
//            this.button5 = new System.Windows.Forms.Button();
//            this.label8 = new System.Windows.Forms.Label();
//            this.txtNumberOfCollects = new System.Windows.Forms.TextBox();
//            this.txtServerUrl = new System.Windows.Forms.TextBox();
//            this.label5 = new System.Windows.Forms.Label();
//            this.groupBox1 = new System.Windows.Forms.GroupBox();
//            this.txtDomain = new System.Windows.Forms.TextBox();
//            this.label9 = new System.Windows.Forms.Label();
//            this.textBox1 = new System.Windows.Forms.TextBox();
//            this.button6 = new System.Windows.Forms.Button();
//            this.groupBox3 = new System.Windows.Forms.GroupBox();
//            this.externalVariablesPanel = new System.Windows.Forms.TableLayoutPanel();
//            this.groupBox2.SuspendLayout();
//            this.grpCollectResults.SuspendLayout();
//            this.groupBox1.SuspendLayout();
//            this.groupBox3.SuspendLayout();
//            this.SuspendLayout();
//            // 
//            // txtAddress
//            // 
//            this.txtAddress.Location = new System.Drawing.Point(12, 25);
//            this.txtAddress.Name = "txtAddress";
//            this.txtAddress.Size = new System.Drawing.Size(243, 20);
//            this.txtAddress.TabIndex = 1;
//            // 
//            // label1
//            // 
//            this.label1.AutoSize = true;
//            this.label1.Location = new System.Drawing.Point(9, 9);
//            this.label1.Name = "label1";
//            this.label1.Size = new System.Drawing.Size(45, 13);
//            this.label1.TabIndex = 0;
//            this.label1.Text = "Address";
//            this.label1.DoubleClick += new System.EventHandler(this.label1_DoubleClick);
//            // 
//            // label2
//            // 
//            this.label2.AutoSize = true;
//            this.label2.Location = new System.Drawing.Point(11, 87);
//            this.label2.Name = "label2";
//            this.label2.Size = new System.Drawing.Size(55, 13);
//            this.label2.TabIndex = 4;
//            this.label2.Text = "Username";
//            // 
//            // txtUserName
//            // 
//            this.txtUserName.Location = new System.Drawing.Point(12, 103);
//            this.txtUserName.Name = "txtUserName";
//            this.txtUserName.Size = new System.Drawing.Size(243, 20);
//            this.txtUserName.TabIndex = 5;
//            // 
//            // label3
//            // 
//            this.label3.AutoSize = true;
//            this.label3.Location = new System.Drawing.Point(13, 129);
//            this.label3.Name = "label3";
//            this.label3.Size = new System.Drawing.Size(53, 13);
//            this.label3.TabIndex = 6;
//            this.label3.Text = "Password";
//            // 
//            // txtPassword
//            // 
//            this.txtPassword.Location = new System.Drawing.Point(12, 145);
//            this.txtPassword.Name = "txtPassword";
//            this.txtPassword.Size = new System.Drawing.Size(243, 20);
//            this.txtPassword.TabIndex = 7;
//            this.txtPassword.UseSystemPasswordChar = true;
//            // 
//            // openFileDialog1
//            // 
//            this.openFileDialog1.FileName = "openFileDialog1";
//            // 
//            // label4
//            // 
//            this.label4.AutoSize = true;
//            this.label4.Location = new System.Drawing.Point(13, 168);
//            this.label4.Name = "label4";
//            this.label4.Size = new System.Drawing.Size(106, 13);
//            this.label4.TabIndex = 12;
//            this.label4.Text = "Oval Definitions Path";
//            // 
//            // txtOvalDefinitions
//            // 
//            this.txtOvalDefinitions.Location = new System.Drawing.Point(12, 184);
//            this.txtOvalDefinitions.Name = "txtOvalDefinitions";
//            this.txtOvalDefinitions.Size = new System.Drawing.Size(243, 20);
//            this.txtOvalDefinitions.TabIndex = 13;
//            this.txtOvalDefinitions.Text = "C:\\temp\\definitions\\definitions.xml";
//            // 
//            // btnExecute
//            // 
//            this.btnExecute.Location = new System.Drawing.Point(6, 19);
//            this.btnExecute.Name = "btnExecute";
//            this.btnExecute.Size = new System.Drawing.Size(104, 26);
//            this.btnExecute.TabIndex = 18;
//            this.btnExecute.Text = "Execute Collect";
//            this.btnExecute.UseVisualStyleBackColor = true;
//            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
//            // 
//            // button1
//            // 
//            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.button1.Location = new System.Drawing.Point(231, 183);
//            this.button1.Name = "button1";
//            this.button1.Size = new System.Drawing.Size(24, 20);
//            this.button1.TabIndex = 14;
//            this.button1.Text = "...";
//            this.button1.UseVisualStyleBackColor = true;
//            this.button1.Click += new System.EventHandler(this.button1_Click);
//            // 
//            // lblInfo
//            // 
//            this.lblInfo.AutoSize = true;
//            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.lblInfo.ForeColor = System.Drawing.Color.Navy;
//            this.lblInfo.Location = new System.Drawing.Point(116, 24);
//            this.lblInfo.Name = "lblInfo";
//            this.lblInfo.Size = new System.Drawing.Size(86, 15);
//            this.lblInfo.TabIndex = 17;
//            this.lblInfo.Text = "Executing....";
//            this.lblInfo.Visible = false;
//            // 
//            // btnResults
//            // 
//            this.btnResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.btnResults.Location = new System.Drawing.Point(7, 60);
//            this.btnResults.Name = "btnResults";
//            this.btnResults.Size = new System.Drawing.Size(129, 30);
//            this.btnResults.TabIndex = 11;
//            this.btnResults.Text = "Show Results...";
//            this.btnResults.UseVisualStyleBackColor = true;
//            this.btnResults.Click += new System.EventHandler(this.btnResults_Click);
//            // 
//            // lblExecutionId
//            // 
//            this.lblExecutionId.AutoSize = true;
//            this.lblExecutionId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.lblExecutionId.ForeColor = System.Drawing.Color.Navy;
//            this.lblExecutionId.Location = new System.Drawing.Point(6, 52);
//            this.lblExecutionId.Name = "lblExecutionId";
//            this.lblExecutionId.Size = new System.Drawing.Size(82, 13);
//            this.lblExecutionId.TabIndex = 19;
//            this.lblExecutionId.Text = "Execution Id:";
//            // 
//            // label7
//            // 
//            this.label7.AutoSize = true;
//            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label7.Location = new System.Drawing.Point(11, 209);
//            this.label7.Name = "label7";
//            this.label7.Size = new System.Drawing.Size(80, 13);
//            this.label7.TabIndex = 0;
//            this.label7.Text = "Execution Date";
//            // 
//            // sheduleDate
//            // 
//            this.sheduleDate.CustomFormat = "dd/MM/yyyy HH:mm:ss";
//            this.sheduleDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.sheduleDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
//            this.sheduleDate.Location = new System.Drawing.Point(14, 225);
//            this.sheduleDate.Name = "sheduleDate";
//            this.sheduleDate.Size = new System.Drawing.Size(241, 20);
//            this.sheduleDate.TabIndex = 1;
//            // 
//            // button3
//            // 
//            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
//            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.button3.Location = new System.Drawing.Point(9, 122);
//            this.button3.Name = "button3";
//            this.button3.Size = new System.Drawing.Size(149, 30);
//            this.button3.TabIndex = 4;
//            this.button3.Text = "Execute Collect on Service";
//            this.button3.UseVisualStyleBackColor = true;
//            this.button3.Click += new System.EventHandler(this.button3_Click);
//            // 
//            // button4
//            // 
//            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.button4.Location = new System.Drawing.Point(7, 24);
//            this.button4.Name = "button4";
//            this.button4.Size = new System.Drawing.Size(129, 30);
//            this.button4.TabIndex = 5;
//            this.button4.Text = "Request Collects...";
//            this.button4.UseVisualStyleBackColor = true;
//            this.button4.Click += new System.EventHandler(this.button4_Click);
//            // 
//            // groupBox2
//            // 
//            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
//                        | System.Windows.Forms.AnchorStyles.Right)));
//            this.groupBox2.Controls.Add(this.grpCollectResults);
//            this.groupBox2.Controls.Add(this.label8);
//            this.groupBox2.Controls.Add(this.txtNumberOfCollects);
//            this.groupBox2.Controls.Add(this.txtServerUrl);
//            this.groupBox2.Controls.Add(this.label5);
//            this.groupBox2.Controls.Add(this.button3);
//            this.groupBox2.Controls.Add(this.groupBox1);
//            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.groupBox2.Location = new System.Drawing.Point(12, 259);
//            this.groupBox2.Name = "groupBox2";
//            this.groupBox2.Size = new System.Drawing.Size(541, 169);
//            this.groupBox2.TabIndex = 16;
//            this.groupBox2.TabStop = false;
//            this.groupBox2.Text = "Collect Server Information";
//            // 
//            // grpCollectResults
//            // 
//            this.grpCollectResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.grpCollectResults.Controls.Add(this.button5);
//            this.grpCollectResults.Controls.Add(this.button4);
//            this.grpCollectResults.Controls.Add(this.btnResults);
//            this.grpCollectResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.grpCollectResults.Location = new System.Drawing.Point(389, 25);
//            this.grpCollectResults.Name = "grpCollectResults";
//            this.grpCollectResults.Size = new System.Drawing.Size(146, 138);
//            this.grpCollectResults.TabIndex = 23;
//            this.grpCollectResults.TabStop = false;
//            this.grpCollectResults.Text = " Collect Management ";
//            // 
//            // button5
//            // 
//            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.button5.Location = new System.Drawing.Point(7, 96);
//            this.button5.Name = "button5";
//            this.button5.Size = new System.Drawing.Size(129, 30);
//            this.button5.TabIndex = 6;
//            this.button5.Text = "In Execution...";
//            this.button5.UseVisualStyleBackColor = true;
//            this.button5.Click += new System.EventHandler(this.button5_Click);
//            // 
//            // label8
//            // 
//            this.label8.AutoSize = true;
//            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label8.Location = new System.Drawing.Point(6, 74);
//            this.label8.Name = "label8";
//            this.label8.Size = new System.Drawing.Size(96, 13);
//            this.label8.TabIndex = 2;
//            this.label8.Text = "Number of Collects";
//            // 
//            // txtNumberOfCollects
//            // 
//            this.txtNumberOfCollects.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.txtNumberOfCollects.Location = new System.Drawing.Point(9, 89);
//            this.txtNumberOfCollects.Name = "txtNumberOfCollects";
//            this.txtNumberOfCollects.Size = new System.Drawing.Size(93, 20);
//            this.txtNumberOfCollects.TabIndex = 3;
//            this.txtNumberOfCollects.Text = "1";
//            // 
//            // txtServerUrl
//            // 
//            this.txtServerUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
//                        | System.Windows.Forms.AnchorStyles.Right)));
//            this.txtServerUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.txtServerUrl.Location = new System.Drawing.Point(9, 42);
//            this.txtServerUrl.Name = "txtServerUrl";
//            this.txtServerUrl.Size = new System.Drawing.Size(374, 20);
//            this.txtServerUrl.TabIndex = 1;
//            this.txtServerUrl.Text = "http://localhost:8000/collectservice";
//            // 
//            // label5
//            // 
//            this.label5.AutoSize = true;
//            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
//            this.label5.Location = new System.Drawing.Point(6, 26);
//            this.label5.Name = "label5";
//            this.label5.Size = new System.Drawing.Size(63, 13);
//            this.label5.TabIndex = 0;
//            this.label5.Text = "Server URL";
//            // 
//            // groupBox1
//            // 
//            this.groupBox1.Controls.Add(this.lblInfo);
//            this.groupBox1.Controls.Add(this.btnExecute);
//            this.groupBox1.Controls.Add(this.lblExecutionId);
//            this.groupBox1.Location = new System.Drawing.Point(164, 78);
//            this.groupBox1.Name = "groupBox1";
//            this.groupBox1.Size = new System.Drawing.Size(217, 74);
//            this.groupBox1.TabIndex = 23;
//            this.groupBox1.TabStop = false;
//            this.groupBox1.Text = "Deprecated Area";
//            // 
//            // txtDomain
//            // 
//            this.txtDomain.Location = new System.Drawing.Point(12, 64);
//            this.txtDomain.Name = "txtDomain";
//            this.txtDomain.Size = new System.Drawing.Size(243, 20);
//            this.txtDomain.TabIndex = 3;
//            this.txtDomain.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
//            // 
//            // label9
//            // 
//            this.label9.AutoSize = true;
//            this.label9.Location = new System.Drawing.Point(11, 48);
//            this.label9.Name = "label9";
//            this.label9.Size = new System.Drawing.Size(43, 13);
//            this.label9.TabIndex = 2;
//            this.label9.Text = "Domain";
//            // 
//            // textBox1
//            // 
//            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
//                        | System.Windows.Forms.AnchorStyles.Left)
//                        | System.Windows.Forms.AnchorStyles.Right)));
//            this.textBox1.Location = new System.Drawing.Point(6, 26);
//            this.textBox1.MaxLength = 2111111111;
//            this.textBox1.Multiline = true;
//            this.textBox1.Name = "textBox1";
//            this.textBox1.Size = new System.Drawing.Size(206, 25);
//            this.textBox1.TabIndex = 20;
//            // 
//            // button6
//            // 
//            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.button6.Location = new System.Drawing.Point(218, 29);
//            this.button6.Name = "button6";
//            this.button6.Size = new System.Drawing.Size(52, 23);
//            this.button6.TabIndex = 21;
//            this.button6.Text = "Test";
//            this.button6.UseVisualStyleBackColor = true;
//            this.button6.Click += new System.EventHandler(this.button6_Click_1);
//            // 
//            // groupBox3
//            // 
//            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
//                        | System.Windows.Forms.AnchorStyles.Right)));
//            this.groupBox3.Controls.Add(this.textBox1);
//            this.groupBox3.Controls.Add(this.button6);
//            this.groupBox3.Location = new System.Drawing.Point(271, 202);
//            this.groupBox3.Name = "groupBox3";
//            this.groupBox3.Size = new System.Drawing.Size(276, 58);
//            this.groupBox3.TabIndex = 22;
//            this.groupBox3.TabStop = false;
//            this.groupBox3.Text = " Test Area ";
//            // 
//            // externalVariablesPanel
//            // 
//            this.externalVariablesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
//                        | System.Windows.Forms.AnchorStyles.Left)
//                        | System.Windows.Forms.AnchorStyles.Right)));
//            this.externalVariablesPanel.AutoScroll = true;
//            this.externalVariablesPanel.ColumnCount = 1;
//            this.externalVariablesPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
//            this.externalVariablesPanel.Location = new System.Drawing.Point(264, 25);
//            this.externalVariablesPanel.Name = "externalVariablesPanel";
//            this.externalVariablesPanel.RowCount = 1;
//            this.externalVariablesPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
//            this.externalVariablesPanel.Size = new System.Drawing.Size(289, 98);
//            this.externalVariablesPanel.TabIndex = 24;
//            this.externalVariablesPanel.Visible = false;
//            // 
//            // frmMain
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(562, 443);
//            this.Controls.Add(this.groupBox3);
//            this.Controls.Add(this.externalVariablesPanel);
//            this.Controls.Add(this.sheduleDate);
//            this.Controls.Add(this.label7);
//            this.Controls.Add(this.label9);
//            this.Controls.Add(this.txtDomain);
//            this.Controls.Add(this.groupBox2);
//            this.Controls.Add(this.button1);
//            this.Controls.Add(this.txtOvalDefinitions);
//            this.Controls.Add(this.label4);
//            this.Controls.Add(this.txtPassword);
//            this.Controls.Add(this.label3);
//            this.Controls.Add(this.txtUserName);
//            this.Controls.Add(this.label2);
//            this.Controls.Add(this.label1);
//            this.Controls.Add(this.txtAddress);
//            this.Name = "frmMain";
//            this.Text = "Interpretador de Oval";
//            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
//            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
//            this.Load += new System.EventHandler(this.frmMain_Load);
//            this.Shown += new System.EventHandler(this.frmMain_Shown);
//            this.DoubleClick += new System.EventHandler(this.frmMain_DoubleClick);
//            this.groupBox2.ResumeLayout(false);
//            this.groupBox2.PerformLayout();
//            this.grpCollectResults.ResumeLayout(false);
//            this.groupBox1.ResumeLayout(false);
//            this.groupBox1.PerformLayout();
//            this.groupBox3.ResumeLayout(false);
//            this.groupBox3.PerformLayout();
//            this.ResumeLayout(false);
//            this.PerformLayout();

//        }

//        #endregion

//        private System.Windows.Forms.TextBox txtAddress;
//        private System.Windows.Forms.Label label1;
//        private System.Windows.Forms.Label label2;
//        private System.Windows.Forms.TextBox txtUserName;
//        private System.Windows.Forms.Label label3;
//        private System.Windows.Forms.TextBox txtPassword;
//        private System.Windows.Forms.OpenFileDialog openFileDialog1;
//        private System.Windows.Forms.Label label4;
//        private System.Windows.Forms.TextBox txtOvalDefinitions;
//        private System.Windows.Forms.Button btnExecute;
//        private System.Windows.Forms.Button button1;
//        private System.Windows.Forms.Label lblInfo;
//        private System.Windows.Forms.Button btnResults;
//        private System.Windows.Forms.Label lblExecutionId;
//        private System.Windows.Forms.Label label7;
//        private System.Windows.Forms.DateTimePicker sheduleDate;
//        private System.Windows.Forms.Button button3;
//        private System.Windows.Forms.Button button4;
//        private System.Windows.Forms.GroupBox groupBox2;
//        private System.Windows.Forms.Label label5;
//        private System.Windows.Forms.TextBox txtServerUrl;
//        private System.Windows.Forms.Button button5;
//        private System.Windows.Forms.Label label8;
//        private System.Windows.Forms.TextBox txtNumberOfCollects;
//        private System.Windows.Forms.TextBox txtDomain;
//        private System.Windows.Forms.Label label9;
//        private System.Windows.Forms.TextBox textBox1;
//        private System.Windows.Forms.Button button6;
//        private System.Windows.Forms.GroupBox groupBox3;
//        private System.Windows.Forms.GroupBox grpCollectResults;
//        private System.Windows.Forms.GroupBox groupBox1;
//        private System.Windows.Forms.TableLayoutPanel externalVariablesPanel;
//    }
//}

