using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace modSIC_Client
{
    public partial class MainView : Form, IMainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnFormClose(this, EventArgs.Empty);
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            OnFormLoad(this, EventArgs.Empty);
        }

        #region IMainView Members

        public event EventHandler OnFormClose;

        public event EventHandler OnFormLoad;

        public event EventHandler<LoginEventArgs> OnLogin;

        public void ShowErrorMessage(string message)
        {
            Dialog.Error(message);
        }

        #endregion
    }
}
