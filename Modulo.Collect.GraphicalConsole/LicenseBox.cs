using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Modulo.Collect.GraphicalConsole
{
    public partial class LicenseBox : Form
    {
        public LicenseBox()
        {
            InitializeComponent();
        }

        public void Prepare(string rtfFile)
        {
            this.rtfLicense.LoadFile(rtfFile);
            LicenseBox_ClientSizeChanged(this, null);
        }

        private void LicenseBox_ClientSizeChanged(object sender, EventArgs e)
        {
            this.rtfLicense.Top = 0;
            this.rtfLicense.Left = 0;
            this.rtfLicense.Width = this.ClientSize.Width;
            this.rtfLicense.Height = this.ClientSize.Height;
        }

        private void LicenseBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\x1B')
            {
                e.Handled = true;
                this.Close();
            }
        }
    }
}
