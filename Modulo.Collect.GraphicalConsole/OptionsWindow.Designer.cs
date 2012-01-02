namespace Modulo.Collect.GraphicalConsole
{
    partial class OptionsWindow
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
            this.label3 = new System.Windows.Forms.Label();
            this.tbModSicPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbModSicUser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbModSicAddr = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbModSicPort = new System.Windows.Forms.TextBox();
            this.btSave = new System.Windows.Forms.Button();
            this.btReset = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Password:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbModSicPass
            // 
            this.tbModSicPass.Location = new System.Drawing.Point(84, 91);
            this.tbModSicPass.Name = "tbModSicPass";
            this.tbModSicPass.PasswordChar = '●';
            this.tbModSicPass.Size = new System.Drawing.Size(152, 20);
            this.tbModSicPass.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Username:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbModSicUser
            // 
            this.tbModSicUser.Location = new System.Drawing.Point(84, 65);
            this.tbModSicUser.Name = "tbModSicUser";
            this.tbModSicUser.Size = new System.Drawing.Size(152, 20);
            this.tbModSicUser.TabIndex = 9;
            this.tbModSicUser.TextChanged += new System.EventHandler(this.tbModSicUser_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Address:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbModSicAddr
            // 
            this.tbModSicAddr.Location = new System.Drawing.Point(84, 12);
            this.tbModSicAddr.Name = "tbModSicAddr";
            this.tbModSicAddr.Size = new System.Drawing.Size(233, 20);
            this.tbModSicAddr.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Port:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // tbModSicPort
            // 
            this.tbModSicPort.Location = new System.Drawing.Point(84, 38);
            this.tbModSicPort.Name = "tbModSicPort";
            this.tbModSicPort.Size = new System.Drawing.Size(60, 20);
            this.tbModSicPort.TabIndex = 8;
            this.tbModSicPort.TextChanged += new System.EventHandler(this.tbModSicPort_TextChanged);
            // 
            // btSave
            // 
            this.btSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btSave.Location = new System.Drawing.Point(161, 138);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 15;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            // 
            // btReset
            // 
            this.btReset.Location = new System.Drawing.Point(242, 89);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(75, 23);
            this.btReset.TabIndex = 16;
            this.btReset.Text = "Reset";
            this.btReset.UseVisualStyleBackColor = true;
            this.btReset.Click += new System.EventHandler(this.btReset_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(242, 138);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 17;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // OptionsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 170);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btReset);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbModSicPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbModSicPass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbModSicUser);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbModSicAddr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OptionsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "modSIC Server Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbModSicPass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbModSicUser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbModSicAddr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbModSicPort;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.Button btCancel;
    }
}