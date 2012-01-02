using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Xml;

namespace TestCertificate
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private X509Certificate2 Certificate = new X509Certificate2("c:\\temp\\CollectServer.pfx", "RiskManager@NG");
        private Byte[] EncryptedCredentials = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private byte[] EncryptCredentialBasedOnCertificateOfServer(CredentialDTO credentialDTO, X509Certificate2 certificate)
        {
            RSACryptoServiceProvider publicKey = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            
            string serializedCredentials = JsonConvert.SerializeObject(credentialDTO);
            byte[] credentialInfoByte = publicKey.Encrypt(Encoding.Default.GetBytes(serializedCredentials), false);
            
            return credentialInfoByte;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CredentialDTO credentials = new CredentialDTO("lfernands", "RiskManager");
            this.EncryptedCredentials = this.EncryptCredentialBasedOnCertificateOfServer(credentials, this.Certificate);
            MessageBox.Show("Encrypt OK");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            CredentialDTO decryptedCredentials = this.DecryptCredentials();
            MessageBox.Show(string.Format("Username: '{0}'\r\nPassword: '{1}'", decryptedCredentials.Username, decryptedCredentials.Password));
        }

        private CredentialDTO DecryptCredentials()
        {
            RSACryptoServiceProvider privateKeyProvider = (RSACryptoServiceProvider)this.Certificate.PrivateKey;
            var serializedCredentials = privateKeyProvider.Decrypt(this.EncryptedCredentials, false);
            var serializedCredentialsAsString = Encoding.Default.GetString(serializedCredentials);
            
            return JsonConvert.DeserializeObject<CredentialDTO>(serializedCredentialsAsString);
        }
        
        private String GetPrivateKeyFromCryptoServiceProvider()
        {
            RSACryptoServiceProvider privateKeyProvider = (RSACryptoServiceProvider)this.Certificate.PrivateKey;
            
            XmlDocument xmlPK = new XmlDocument();
            xmlPK.LoadXml(privateKeyProvider.ToXmlString(false));
            string pkAsString = xmlPK.GetElementById("modulus").InnerText;

            return pkAsString;
        }




    }

    public class CredentialDTO
    {
        public String Username { get; private set; }
        public String Password { get; private set; }

        public CredentialDTO(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

    }
}
