namespace ModuloOvalInterpreter
{
    partial class ServerCollectionsList
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoadServerCollectionsList = new System.Windows.Forms.Button();
            this.spinStartID = new System.Windows.Forms.NumericUpDown();
            this.txtCollections = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.spinStartID)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Start ID";
            // 
            // btnLoadServerCollectionsList
            // 
            this.btnLoadServerCollectionsList.Location = new System.Drawing.Point(195, 22);
            this.btnLoadServerCollectionsList.Name = "btnLoadServerCollectionsList";
            this.btnLoadServerCollectionsList.Size = new System.Drawing.Size(75, 23);
            this.btnLoadServerCollectionsList.TabIndex = 2;
            this.btnLoadServerCollectionsList.Text = "Load";
            this.btnLoadServerCollectionsList.UseVisualStyleBackColor = true;
            this.btnLoadServerCollectionsList.Click += new System.EventHandler(this.btnLoadServerCollectionsList_Click);
            // 
            // spinStartID
            // 
            this.spinStartID.Increment = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.spinStartID.Location = new System.Drawing.Point(16, 25);
            this.spinStartID.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this.spinStartID.Name = "spinStartID";
            this.spinStartID.Size = new System.Drawing.Size(171, 20);
            this.spinStartID.TabIndex = 3;
            this.spinStartID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinStartID.DoubleClick += new System.EventHandler(this.spinStartID_DoubleClick);
            // 
            // txtCollections
            // 
            this.txtCollections.FormattingEnabled = true;
            this.txtCollections.Location = new System.Drawing.Point(16, 51);
            this.txtCollections.Name = "txtCollections";
            this.txtCollections.Size = new System.Drawing.Size(254, 303);
            this.txtCollections.TabIndex = 4;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 393);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(638, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // ServerCollectionsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 415);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtCollections);
            this.Controls.Add(this.spinStartID);
            this.Controls.Add(this.btnLoadServerCollectionsList);
            this.Controls.Add(this.label1);
            this.Name = "ServerCollectionsList";
            this.Text = "ServerCollectionsList";
            ((System.ComponentModel.ISupportInitialize)(this.spinStartID)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoadServerCollectionsList;
        private System.Windows.Forms.NumericUpDown spinStartID;
        private System.Windows.Forms.ListBox txtCollections;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}