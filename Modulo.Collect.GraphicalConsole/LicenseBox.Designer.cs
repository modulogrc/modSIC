namespace Modulo.Collect.GraphicalConsole
{
    partial class LicenseBox
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
            this.rtfLicense = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtfLicense
            // 
            this.rtfLicense.Location = new System.Drawing.Point(13, 13);
            this.rtfLicense.Name = "rtfLicense";
            this.rtfLicense.ReadOnly = true;
            this.rtfLicense.Size = new System.Drawing.Size(100, 96);
            this.rtfLicense.TabIndex = 0;
            this.rtfLicense.Text = "";
            // 
            // LicenseBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.rtfLicense);
            this.KeyPreview = true;
            this.Name = "LicenseBox";
            this.Text = "License Agreement (Press ESC to dismiss)";
            this.ClientSizeChanged += new System.EventHandler(this.LicenseBox_ClientSizeChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LicenseBox_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtfLicense;
    }
}