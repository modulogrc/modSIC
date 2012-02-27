namespace Modulo.Collect.GraphicalConsole
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.chooserOval = new System.Windows.Forms.OpenFileDialog();
            this.chooserFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btGo = new System.Windows.Forms.Button();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.grTarget = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbAdminPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTargetPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTargetUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTarget = new System.Windows.Forms.TextBox();
            this.grResults = new System.Windows.Forms.GroupBox();
            this.btSaveDir = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbSaveFolder = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbDefName = new System.Windows.Forms.Label();
            this.btChooseDefs = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbOvalDefs = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.grTarget.SuspendLayout();
            this.grResults.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.logoPictureBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(514, 128);
            this.panel1.TabIndex = 22;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.BackColor = System.Drawing.SystemColors.Window;
            this.logoPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(0, 0);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(514, 34);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 21;
            this.logoPictureBox.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.grTarget);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 152);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(514, 257);
            this.panel2.TabIndex = 24;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.grResults);
            this.panel3.Controls.Add(this.btGo);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 277);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(514, 132);
            this.panel3.TabIndex = 25;
            // 
            // btGo
            // 
            this.btGo.Location = new System.Drawing.Point(404, 85);
            this.btGo.Name = "btGo";
            this.btGo.Size = new System.Drawing.Size(100, 30);
            this.btGo.TabIndex = 23;
            this.btGo.Text = "Submit";
            this.btGo.UseVisualStyleBackColor = true;
            this.btGo.Click += new System.EventHandler(this.btGo_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.optionsToolStripMenuItem.Text = "&Options...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(514, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // grTarget
            // 
            this.grTarget.AutoSize = true;
            this.grTarget.Controls.Add(this.label4);
            this.grTarget.Controls.Add(this.tbAdminPassword);
            this.grTarget.Controls.Add(this.label3);
            this.grTarget.Controls.Add(this.tbTargetPassword);
            this.grTarget.Controls.Add(this.label2);
            this.grTarget.Controls.Add(this.tbTargetUsername);
            this.grTarget.Controls.Add(this.label1);
            this.grTarget.Controls.Add(this.tbTarget);
            this.grTarget.Location = new System.Drawing.Point(6, 0);
            this.grTarget.Name = "grTarget";
            this.grTarget.Size = new System.Drawing.Size(498, 119);
            this.grTarget.TabIndex = 25;
            this.grTarget.TabStop = false;
            this.grTarget.Text = " Step 2: Select Target ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(215, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Adm. Password (Optional):";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbAdminPassword
            // 
            this.tbAdminPassword.Location = new System.Drawing.Point(352, 80);
            this.tbAdminPassword.Name = "tbAdminPassword";
            this.tbAdminPassword.PasswordChar = '●';
            this.tbAdminPassword.Size = new System.Drawing.Size(134, 20);
            this.tbAdminPassword.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Password:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbTargetPassword
            // 
            this.tbTargetPassword.Location = new System.Drawing.Point(79, 80);
            this.tbTargetPassword.Name = "tbTargetPassword";
            this.tbTargetPassword.PasswordChar = '●';
            this.tbTargetPassword.Size = new System.Drawing.Size(128, 20);
            this.tbTargetPassword.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Username:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbTargetUsername
            // 
            this.tbTargetUsername.Location = new System.Drawing.Point(79, 54);
            this.tbTargetUsername.Name = "tbTargetUsername";
            this.tbTargetUsername.Size = new System.Drawing.Size(128, 20);
            this.tbTargetUsername.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Address:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbTarget
            // 
            this.tbTarget.Location = new System.Drawing.Point(79, 28);
            this.tbTarget.Name = "tbTarget";
            this.tbTarget.Size = new System.Drawing.Size(407, 20);
            this.tbTarget.TabIndex = 9;
            // 
            // grResults
            // 
            this.grResults.AutoSize = true;
            this.grResults.Controls.Add(this.btSaveDir);
            this.grResults.Controls.Add(this.label6);
            this.grResults.Controls.Add(this.tbSaveFolder);
            this.grResults.Location = new System.Drawing.Point(6, 3);
            this.grResults.Name = "grResults";
            this.grResults.Size = new System.Drawing.Size(498, 67);
            this.grResults.TabIndex = 24;
            this.grResults.TabStop = false;
            this.grResults.Text = " Step 3: Choose Where to Save OVAL Results ";
            // 
            // btSaveDir
            // 
            this.btSaveDir.Location = new System.Drawing.Point(411, 25);
            this.btSaveDir.Name = "btSaveDir";
            this.btSaveDir.Size = new System.Drawing.Size(75, 23);
            this.btSaveDir.TabIndex = 20;
            this.btSaveDir.Text = "Select...";
            this.btSaveDir.UseVisualStyleBackColor = true;
            this.btSaveDir.Click += new System.EventHandler(this.btSaveDir_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Destination Folder:";
            // 
            // tbSaveFolder
            // 
            this.tbSaveFolder.Enabled = false;
            this.tbSaveFolder.Location = new System.Drawing.Point(101, 28);
            this.tbSaveFolder.Name = "tbSaveFolder";
            this.tbSaveFolder.Size = new System.Drawing.Size(304, 20);
            this.tbSaveFolder.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbDefName);
            this.groupBox1.Controls.Add(this.btChooseDefs);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbOvalDefs);
            this.groupBox1.Location = new System.Drawing.Point(6, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 82);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Step 1: Select an OVAL Definitions File ";
            // 
            // lbDefName
            // 
            this.lbDefName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDefName.ForeColor = System.Drawing.Color.Red;
            this.lbDefName.Location = new System.Drawing.Point(25, 58);
            this.lbDefName.Name = "lbDefName";
            this.lbDefName.Size = new System.Drawing.Size(446, 20);
            this.lbDefName.TabIndex = 21;
            this.lbDefName.Text = "No OVAL Definitions selected";
            this.lbDefName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btChooseDefs
            // 
            this.btChooseDefs.Location = new System.Drawing.Point(411, 28);
            this.btChooseDefs.Name = "btChooseDefs";
            this.btChooseDefs.Size = new System.Drawing.Size(75, 23);
            this.btChooseDefs.TabIndex = 2;
            this.btChooseDefs.Text = "Browse...";
            this.btChooseDefs.UseVisualStyleBackColor = true;
            this.btChooseDefs.Click += new System.EventHandler(this.btChooseDefs_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "OVAL Defs:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbOvalDefs
            // 
            this.tbOvalDefs.Enabled = false;
            this.tbOvalDefs.Location = new System.Drawing.Point(79, 30);
            this.tbOvalDefs.Name = "tbOvalDefs";
            this.tbOvalDefs.Size = new System.Drawing.Size(326, 20);
            this.tbOvalDefs.TabIndex = 1;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(514, 409);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "modSIC";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grTarget.ResumeLayout(false);
            this.grTarget.PerformLayout();
            this.grResults.ResumeLayout(false);
            this.grResults.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog chooserOval;
        private System.Windows.Forms.FolderBrowserDialog chooserFolder;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btGo;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.GroupBox grTarget;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbAdminPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTargetPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTargetUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTarget;
        private System.Windows.Forms.GroupBox grResults;
        private System.Windows.Forms.Button btSaveDir;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbSaveFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbDefName;
        private System.Windows.Forms.Button btChooseDefs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbOvalDefs;
    }
}

