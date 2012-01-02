using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Modulo.Collect.Monitor;

namespace WindowsFormsApplication2
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();
            try
            {
                var serializedConfig = System.IO.File.ReadAllText("c:\\temp\\modsicsrv_config.txt");
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ModsicConfiguration>(serializedConfig);

                txtURL.Text = config.Url;
                txtPort.Text = config.Port;
                txtUsername.Text = config.Username;
                txtPassword.Text = config.Password;
            }
            catch (Exception)
            {
            }
        }            

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var config =
                new ModsicConfiguration
                {
                    Url = txtURL.Text,
                    Port = txtPort.Text,
                    Username = txtUsername.Text,
                    Password = txtPassword.Text
                };

            var serializedConfig = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            System.IO.File.WriteAllText("c:\\temp\\modsicsrv_config.txt", serializedConfig);

            this.Close();
        }
    }    
}
