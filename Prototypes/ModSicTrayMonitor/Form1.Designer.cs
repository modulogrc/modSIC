namespace WindowsFormsApplication2
{
    partial class optionsForm
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
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startServerMenu = new System.Windows.Forms.ToolStripTextBox();
            this.stopServerMenu = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsMenu = new System.Windows.Forms.ToolStripTextBox();
            this.closeMenu = new System.Windows.Forms.ToolStripTextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Text = "...";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startServerMenu,
            this.stopServerMenu,
            this.toolStripSeparator1,
            this.optionsMenu,
            this.closeMenu});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 82);
            // 
            // startServerMenu
            // 
            this.startServerMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.startServerMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.startServerMenu.Name = "startServerMenu";
            this.startServerMenu.ReadOnly = true;
            this.startServerMenu.Size = new System.Drawing.Size(80, 16);
            this.startServerMenu.Text = "Start Server";
            this.startServerMenu.Click += new System.EventHandler(this.startServerMenu_Click);
            this.startServerMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.startServerMenu_MouseMove);
            // 
            // stopServerMenu
            // 
            this.stopServerMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.stopServerMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.stopServerMenu.Name = "stopServerMenu";
            this.stopServerMenu.ReadOnly = true;
            this.stopServerMenu.Size = new System.Drawing.Size(80, 16);
            this.stopServerMenu.Text = "Stop Server";
            this.stopServerMenu.Click += new System.EventHandler(this.stopServerMenu_Click);
            this.stopServerMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.stopServerMenu_MouseMove);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.toolStripSeparator1.ForeColor = System.Drawing.SystemColors.Info;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // optionsMenu
            // 
            this.optionsMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.optionsMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.optionsMenu.Name = "optionsMenu";
            this.optionsMenu.ReadOnly = true;
            this.optionsMenu.Size = new System.Drawing.Size(100, 16);
            this.optionsMenu.Text = "Options";
            this.optionsMenu.Click += new System.EventHandler(this.optionsMenu_Click);
            this.optionsMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.optionsMenu_MouseMove);
            // 
            // closeMenu
            // 
            this.closeMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.closeMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.closeMenu.Name = "closeMenu";
            this.closeMenu.ReadOnly = true;
            this.closeMenu.Size = new System.Drawing.Size(100, 16);
            this.closeMenu.Text = "Close";
            this.closeMenu.Click += new System.EventHandler(this.closeMenu_Click);
            this.closeMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.closeMenu_MouseMove);
            // 
            // optionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(176, 105);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "optionsForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.Text = "Options";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripTextBox stopServerMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox closeMenu;
        private System.Windows.Forms.ToolStripTextBox startServerMenu;
        private System.Windows.Forms.ToolStripTextBox optionsMenu;
    }
}

