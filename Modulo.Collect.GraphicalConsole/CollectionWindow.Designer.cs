namespace Modulo.Collect.GraphicalConsole
{
    partial class CollectionWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionWindow));
            this.lbRequest = new System.Windows.Forms.Label();
            this.imgRequest = new System.Windows.Forms.PictureBox();
            this.lbCollectId = new System.Windows.Forms.Label();
            this.lbCollectFinished = new System.Windows.Forms.Label();
            this.imgGetDocuments = new System.Windows.Forms.PictureBox();
            this.lbSaveResults = new System.Windows.Forms.Label();
            this.lbSaveSystemCharacteristics = new System.Windows.Forms.Label();
            this.lbCollecting = new System.Windows.Forms.Label();
            this.imgCollecting = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.lbGetDocuments = new System.Windows.Forms.Label();
            this.btnViewResults = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgRequest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgGetDocuments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCollecting)).BeginInit();
            this.SuspendLayout();
            // 
            // lbRequest
            // 
            this.lbRequest.AutoSize = true;
            this.lbRequest.Location = new System.Drawing.Point(33, 16);
            this.lbRequest.Name = "lbRequest";
            this.lbRequest.Size = new System.Drawing.Size(28, 13);
            this.lbRequest.TabIndex = 3;
            this.lbRequest.Text = "Text";
            this.lbRequest.Visible = false;
            // 
            // imgRequest
            // 
            this.imgRequest.Image = global::Modulo.Collect.GraphicalConsole.Properties.Resources.ajax_loading;
            this.imgRequest.Location = new System.Drawing.Point(11, 14);
            this.imgRequest.Name = "imgRequest";
            this.imgRequest.Size = new System.Drawing.Size(16, 16);
            this.imgRequest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgRequest.TabIndex = 2;
            this.imgRequest.TabStop = false;
            this.imgRequest.Visible = false;
            // 
            // lbCollectId
            // 
            this.lbCollectId.AutoSize = true;
            this.lbCollectId.Location = new System.Drawing.Point(33, 40);
            this.lbCollectId.Name = "lbCollectId";
            this.lbCollectId.Size = new System.Drawing.Size(28, 13);
            this.lbCollectId.TabIndex = 5;
            this.lbCollectId.Text = "Text";
            this.lbCollectId.Visible = false;
            // 
            // lbCollectFinished
            // 
            this.lbCollectFinished.AutoSize = true;
            this.lbCollectFinished.Location = new System.Drawing.Point(33, 88);
            this.lbCollectFinished.Name = "lbCollectFinished";
            this.lbCollectFinished.Size = new System.Drawing.Size(28, 13);
            this.lbCollectFinished.TabIndex = 7;
            this.lbCollectFinished.Text = "Text";
            this.lbCollectFinished.Visible = false;
            // 
            // imgGetDocuments
            // 
            this.imgGetDocuments.Image = global::Modulo.Collect.GraphicalConsole.Properties.Resources.ajax_loading;
            this.imgGetDocuments.Location = new System.Drawing.Point(11, 110);
            this.imgGetDocuments.Name = "imgGetDocuments";
            this.imgGetDocuments.Size = new System.Drawing.Size(16, 16);
            this.imgGetDocuments.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgGetDocuments.TabIndex = 6;
            this.imgGetDocuments.TabStop = false;
            this.imgGetDocuments.Visible = false;
            // 
            // lbSaveResults
            // 
            this.lbSaveResults.AutoSize = true;
            this.lbSaveResults.Location = new System.Drawing.Point(33, 136);
            this.lbSaveResults.Name = "lbSaveResults";
            this.lbSaveResults.Size = new System.Drawing.Size(28, 13);
            this.lbSaveResults.TabIndex = 9;
            this.lbSaveResults.Text = "Text";
            this.lbSaveResults.Visible = false;
            // 
            // lbSaveSystemCharacteristics
            // 
            this.lbSaveSystemCharacteristics.AutoSize = true;
            this.lbSaveSystemCharacteristics.Location = new System.Drawing.Point(33, 160);
            this.lbSaveSystemCharacteristics.Name = "lbSaveSystemCharacteristics";
            this.lbSaveSystemCharacteristics.Size = new System.Drawing.Size(28, 13);
            this.lbSaveSystemCharacteristics.TabIndex = 11;
            this.lbSaveSystemCharacteristics.Text = "Text";
            this.lbSaveSystemCharacteristics.Visible = false;
            // 
            // lbCollecting
            // 
            this.lbCollecting.AutoSize = true;
            this.lbCollecting.Location = new System.Drawing.Point(33, 64);
            this.lbCollecting.Name = "lbCollecting";
            this.lbCollecting.Size = new System.Drawing.Size(28, 13);
            this.lbCollecting.TabIndex = 13;
            this.lbCollecting.Text = "Text";
            this.lbCollecting.Visible = false;
            // 
            // imgCollecting
            // 
            this.imgCollecting.Image = global::Modulo.Collect.GraphicalConsole.Properties.Resources.ajax_loading;
            this.imgCollecting.Location = new System.Drawing.Point(11, 62);
            this.imgCollecting.Name = "imgCollecting";
            this.imgCollecting.Size = new System.Drawing.Size(16, 16);
            this.imgCollecting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgCollecting.TabIndex = 12;
            this.imgCollecting.TabStop = false;
            this.imgCollecting.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(335, 191);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lbGetDocuments
            // 
            this.lbGetDocuments.AutoSize = true;
            this.lbGetDocuments.Location = new System.Drawing.Point(33, 112);
            this.lbGetDocuments.Name = "lbGetDocuments";
            this.lbGetDocuments.Size = new System.Drawing.Size(28, 13);
            this.lbGetDocuments.TabIndex = 16;
            this.lbGetDocuments.Text = "Text";
            this.lbGetDocuments.Visible = false;
            // 
            // btnViewResults
            // 
            this.btnViewResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewResults.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnViewResults.Enabled = false;
            this.btnViewResults.Location = new System.Drawing.Point(231, 191);
            this.btnViewResults.Name = "btnViewResults";
            this.btnViewResults.Size = new System.Drawing.Size(98, 23);
            this.btnViewResults.TabIndex = 17;
            this.btnViewResults.Text = "View Results";
            this.btnViewResults.UseVisualStyleBackColor = true;
            this.btnViewResults.Click += new System.EventHandler(this.btnViewResults_Click);
            // 
            // CollectWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 226);
            this.ControlBox = false;
            this.Controls.Add(this.btnViewResults);
            this.Controls.Add(this.lbGetDocuments);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lbCollecting);
            this.Controls.Add(this.imgCollecting);
            this.Controls.Add(this.lbSaveSystemCharacteristics);
            this.Controls.Add(this.lbSaveResults);
            this.Controls.Add(this.lbCollectFinished);
            this.Controls.Add(this.imgGetDocuments);
            this.Controls.Add(this.lbCollectId);
            this.Controls.Add(this.lbRequest);
            this.Controls.Add(this.imgRequest);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CollectWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Collection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CollectWindow_FormClosing);
            this.Load += new System.EventHandler(this.CollectWindow_Load);
            this.Shown += new System.EventHandler(this.CollectWindow_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.imgRequest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgGetDocuments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCollecting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbRequest;
        private System.Windows.Forms.PictureBox imgRequest;
        private System.Windows.Forms.Label lbCollectId;
        private System.Windows.Forms.Label lbCollectFinished;
        private System.Windows.Forms.PictureBox imgGetDocuments;
        private System.Windows.Forms.Label lbSaveResults;
        private System.Windows.Forms.Label lbSaveSystemCharacteristics;
        private System.Windows.Forms.Label lbCollecting;
        private System.Windows.Forms.PictureBox imgCollecting;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lbGetDocuments;
        private System.Windows.Forms.Button btnViewResults;
    }
}