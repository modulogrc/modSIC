namespace OvalDefinitionsGenerator
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
            this.btnSelectOvalDefinitionsFilename = new System.Windows.Forms.Button();
            this.txtSourceDefinitionsFilename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNewDefinitionsShortName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSelectOvalDefinitionsFilename
            // 
            this.btnSelectOvalDefinitionsFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectOvalDefinitionsFilename.Location = new System.Drawing.Point(654, 35);
            this.btnSelectOvalDefinitionsFilename.Name = "btnSelectOvalDefinitionsFilename";
            this.btnSelectOvalDefinitionsFilename.Size = new System.Drawing.Size(26, 20);
            this.btnSelectOvalDefinitionsFilename.TabIndex = 0;
            this.btnSelectOvalDefinitionsFilename.Text = "...";
            this.btnSelectOvalDefinitionsFilename.UseVisualStyleBackColor = true;
            // 
            // txtSourceDefinitionsFilename
            // 
            this.txtSourceDefinitionsFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceDefinitionsFilename.Location = new System.Drawing.Point(12, 36);
            this.txtSourceDefinitionsFilename.Name = "txtSourceDefinitionsFilename";
            this.txtSourceDefinitionsFilename.Size = new System.Drawing.Size(636, 20);
            this.txtSourceDefinitionsFilename.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Oval Definitions Source";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(594, 71);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(86, 28);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(12, 133);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(668, 316);
            this.textBox2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Oval Definitions";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "New Definition Short Name";
            // 
            // txtNewDefinitionsShortName
            // 
            this.txtNewDefinitionsShortName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewDefinitionsShortName.Location = new System.Drawing.Point(15, 79);
            this.txtNewDefinitionsShortName.Name = "txtNewDefinitionsShortName";
            this.txtNewDefinitionsShortName.Size = new System.Drawing.Size(224, 20);
            this.txtNewDefinitionsShortName.TabIndex = 6;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 461);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNewDefinitionsShortName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSourceDefinitionsFilename);
            this.Controls.Add(this.btnSelectOvalDefinitionsFilename);
            this.Name = "frmMain";
            this.Text = "Oval Definitions Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectOvalDefinitionsFilename;
        private System.Windows.Forms.TextBox txtSourceDefinitionsFilename;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNewDefinitionsShortName;
    }
}

