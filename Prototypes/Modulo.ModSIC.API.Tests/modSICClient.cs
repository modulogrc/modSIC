using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Service.Contract;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.Reflection;
using System.IO;
using Modulo.Collect.Service.Contract.Security;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace ModSicApiTests
{
    public class modSICClient
    {
        byte[] apiCertHash = new byte[] { 0xB7, 0x55, 0xF8, 0x9A, 0x2E, 0xEB, 0x37, 0xB5, 0x96, 0xF6, 0x9B, 0xE1, 0x15, 0x0C, 0xA9, 0x1C, 0xC0, 0x47, 0xC2, 0x6E };

        public ICollectService Service { get; private set; }

        public modSICClient()
        {
            this.Connect();
        }

        private void Connect()
        {
            var httpBinding = new WSHttpBinding();
            httpBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
            httpBinding.ReaderQuotas.MaxArrayLength = 2147483647;
            httpBinding.MaxReceivedMessageSize = 2147483647;
            //httpBinding.Security.Mode = SecurityMode.Transport;
            httpBinding.Security.Mode = SecurityMode.None;
            //ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;
            
            var serviceEndPoint = new EndpointAddress("http://localhost:1000/CollectService");

            this.Service = new ChannelFactory<ICollectService>(httpBinding, serviceEndPoint).CreateChannel();
        }

        public String Login()
        {
            Connect();
            return this.Service.Login("admin", "Pa$$w@rd");
        }


        public Package CreatePackage(
            string definitionsResourcePath, 
            string targetAddress, 
            string username = "Oval", 
            string password = "M0dul0-0v4l")
        {
            // Creating definition info array
            var definitionID = Guid.NewGuid().ToString();
            var definitonContent = new ResourceLoader().GetDocumentContents(definitionsResourcePath);
            var definitions = new DefinitionInfo[] { new DefinitionInfo() { Id = definitionID, Text = definitonContent } };
            
            var credential = new Credential() { Domain = String.Empty, UserName = username, Password = password };
            
            var certificate = this.GetCertificate();
            var credentialInBytes = new CollectServiceCryptoProvider().EncryptCredentialBasedOnCertificateOfServer(credential, GetCertificate());
            var credentials = Encoding.Default.GetString(credentialInBytes);

            // Creating request array
            var requestID = Guid.NewGuid().ToString();
            var requests = new Request[] 
            { 
                new Request() 
                { 
                    DefinitionId = definitionID,
                    RequestId = requestID,
                    Address = targetAddress,
                    Credential = credentials,
                    ExternalVariables = null
                } 
            };

            var package = new Package()
            {
                Definitions = definitions,
                CollectRequests = requests,
                ScheduleInformation = new ScheduleInformation() { ScheduleDate = DateTime.Now }
            };

            return package;
        }

        public SendRequestResult ScheduleCollection(string ovalDefinitions, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            string SOME_MODSIC_USERNAME = "admin";
            string MODSIC_USER_PASSWORD = "Pa$$w@rd";

            // Building DefinitionInfo Array...
            var newDefinitionID = System.Guid.NewGuid().ToString();
            var ovalDefinitionsAsXML = ovalDefinitions;
            var definitionInfo = new DefinitionInfo[] { new DefinitionInfo() { Id = newDefinitionID, Text = ovalDefinitionsAsXML } };

            // Building Request Array...
            var newRequestID = System.Guid.NewGuid().ToString();
            var targetIP = "10.1.0.187";
            var encryptedCredentials = GetEncryptedCredentials(certificate);
            var requestInfo = new Request[] { new Request() { DefinitionId = newDefinitionID, RequestId = newRequestID, Address = targetIP, Credential = encryptedCredentials } };

            // Creating ScheduleInformation
            var scheduleInfo = new ScheduleInformation() { ScheduleDate = DateTime.UtcNow };

            // Building a new package structure needed to invoke the SendRequest operation
            var package = new Package() { Definitions = definitionInfo, CollectRequests = requestInfo, ScheduleInformation = scheduleInfo };

            // Scheduling a new collection
            //ICollectService collectServiceChannel = new ChannelFactory<ICollectService>().CreateChannel();
            var token = this.Service.Login(SOME_MODSIC_USERNAME, MODSIC_USER_PASSWORD);

            return this.Service.SendRequest(package, token);
        }


        private string GetEncryptedCredentials(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            var credential = new Credential() { UserName = "root", Password = "M0dul0-0v4l" };
            var publicKey = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            var info = JsonConvert.SerializeObject(credential);
            var serializedAndEncryptedCredentialsInBytes = publicKey.Encrypt(Encoding.Default.GetBytes(info), false);

            return Encoding.Default.GetString(serializedAndEncryptedCredentialsInBytes);
        }

        private X509Certificate2 GetCertificate()
        {
            String token = null;
            try
            {
                token = this.Service.Login("admin", "Pa$$w@rd");
                var collectorCertificateInBytes = this.Service.GetCertificate(token);
                
                return new X509Certificate2(collectorCertificateInBytes);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (!string.IsNullOrEmpty(token))
                    this.Service.Logout(token);
            }
        }

        private bool ValidateServerCertficate(
            object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                // Good certificate.
                return true;
            }

            bool certMatch = false; // Assume failure
            byte[] certHash = cert.GetCertHash();
            if (certHash.Length == apiCertHash.Length)
            {
                certMatch = true; // Now assume success.
                for (int idx = 0; idx < certHash.Length; idx++)
                {
                    if (certHash[idx] != apiCertHash[idx])
                    {
                        certMatch = false; // No match
                        break;
                    }
                }
            }

            // Return true => allow unauthenticated server,
            //        false => disallow unauthenticated server.
            return certMatch;
        }
    }
}


//System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);