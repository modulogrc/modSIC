#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Modulo.Collect.Service.Client;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.XPath;
using Modulo.Collect.Service.Contract;

namespace Modulo.Collect.GraphicalConsole
{
    public class CollectionController
    {
        #region Private members
        private ICollectionView view;
        private ModSicConnection modSicConnection;
        private String resultsFilename;
        private String systemCharacteristicsFilename;
        private String htmlFilename;
        #endregion

        #region Public Members
        public bool OnRequestCollectionCalled { get; set; }       
        #endregion

        #region Constructor
        public CollectionController(ICollectionView _view)
        {
            view = _view;
            view.OnRequestCollection += new EventHandler<RequestCollectionEvenArgs>(view_OnSendCollection);
        }
        #endregion

        #region View Events
        public void view_OnSendCollection(object sender, RequestCollectionEvenArgs e)
        {
            OnRequestCollectionCalled = true;

            try
            {
                if (e == null || e.Server == null || e.Target == null)
                {
                    view.ShowErrorMessage(Resource.InvalidConfiguration);
                    return;
                }

                var guid = (GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
                var clientId = System.Environment.MachineName + "\\" + guid.Value;

                string prefix = String.Empty;
                if (!String.IsNullOrEmpty(e.Server.Address))
                {
                    prefix = (e.Server.Address.StartsWith("http")) ? String.Empty : "http://";
                }

                var address = String.Format("{2}{0}:{1}/CollectService", e.Server.Address, e.Server.Port, prefix);
                modSicConnection = new ModSicConnection(address, e.Server.Username, e.Server.Password, clientId);

                systemCharacteristicsFilename = Path.Combine(e.SaveFolder, "system-characteristics.xml");
                resultsFilename = Path.Combine(e.SaveFolder, "results.xml");
                htmlFilename = Path.Combine(e.SaveFolder, "results.html");

                StartThread(e);
            }
            catch (Exception ex)
            {
                view.ShowErrorMessage(Util.FormatExceptionMessage(ex));
            }
        }
        #endregion

        #region Collect
        public void Run(Object obj)
        {
            try
            {
                var e = (RequestCollectionEvenArgs)obj;
                var target = e.Target;

                String ovalDefinitions = ReadFile(e.DefinitionFilename);
                if (ovalDefinitions != null)
                {
                    string collectRequestId;
                    var credentials = CreateCredentials(target);

                    Dictionary<string, string> targetParameters = null;
                    if (!String.IsNullOrEmpty(target.SSHPort))
                    {
                        targetParameters = new Dictionary<string, string>();
                        targetParameters.Add("sshPort", target.SSHPort);
                    }

                    var ovalResults = RequestCollectionSynchronous(target.Address, credentials, ovalDefinitions, out collectRequestId, e.ExternalVariablesXml, targetParameters);
                    SaveOvalDocuments(ovalResults, collectRequestId);
                }

                view.Finish();
            }
            catch (Exception ex)
            {
                view.ShowErrorMessage(Util.FormatExceptionMessage(ex));
            }
        }

        private Credential CreateCredentials(TargetConfiguration target)
        {
            return
                new Credential() 
                    { 
                        UserName = target.Username, 
                        Password = target.Password, 
                        AdministrativePassword = target.AdministrativePassword 
                    };
        }

        private String RequestCollectionSynchronous(string address, Credential credentials, string ovalDefinitions, out string collectRequestId, string externalVariables = null, Dictionary<string, string> targetParameters = null)
        {
            collectRequestId = null;
        
            var requestResult = modSicConnection.SendCollect(address, credentials, ovalDefinitions, externalVariables, targetParameters);
            if (requestResult.HasErrors)
            {
                view.ShowErrorMessage(requestResult.Message);
                return null;
            }

            collectRequestId = requestResult.Requests.First().ServiceRequestId;
            view.ShowCollectionIdMessage(collectRequestId);
            view.ShowCollectingDataMessage(address);

            while (true)
            {
                Sleep(10);

                var statusString = this.IsCollectionInExecution(collectRequestId);
                if (string.IsNullOrEmpty(statusString))
                {
                    view.ShowCollectionFinishedMessage();
                    view.ShowGetDocumentsMessage();
                    return modSicConnection.GetOvalResults(collectRequestId);
                }
            }
        }

        private string IsCollectionInExecution(string collectRequestId)
        {
            var collectionsInExecution = modSicConnection.GetCollectionsInExecution().ToDictionary(c => c.CollectRequestId, c => c.Status.ToString());
            var requestedCollect = collectionsInExecution
                .Where(collection => collection.Key.Equals(collectRequestId))
                .Select(collection => collection.Value);

            if (requestedCollect.Count() > 0)
            {
                return requestedCollect.First();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Save Files
        private void SaveOvalDocuments(string ovalResults, string collectRequestId)
        {
            if (!String.IsNullOrEmpty(ovalResults))
            {
                SaveFile(resultsFilename, ovalResults);
                SaveStream(htmlFilename, ConvertResultsToHtml(ovalResults));

                view.ShowSaveOvalResultsMessage();

                var systemCharacteristics = GetSystemCharacteristicsFromOvalResults(ovalResults);
                if (!String.IsNullOrEmpty(systemCharacteristics))
                {
                    SaveFile(systemCharacteristicsFilename, systemCharacteristics);
                    view.ShowSaveSystemCharacteristicsMessage();
                }
                else
                {
                    view.ShowSaveSystemCharacteristicsErrorMessage(Resource.ImpossibleToGetSystemCharacteristics);
                }
            }
            else
            {
                view.ShowSaveOvalResultsErrorMessage(Resource.EmptyOVALResults);

                var result = modSicConnection.GetResultDocument(collectRequestId);
                if (result != null)
                {
                    var systemCharacteristics = result.SystemCharacteristics;
                    if (!String.IsNullOrEmpty(systemCharacteristics))
                    {
                        File.WriteAllText(systemCharacteristicsFilename, systemCharacteristics);
                        view.ShowSaveSystemCharacteristicsMessage();
                    }
                }
            }
        }

        private void SaveStream(string filename, MemoryStream stream)
        {
            FileStream fileStream = File.Create(filename);
            stream.WriteTo(fileStream);
            fileStream.Flush();
            fileStream.Close();
            stream.Close();
        }

        private MemoryStream ConvertResultsToHtml(string ovalResults)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();

            MemoryStream input = GetStreamFromUTF8String(ovalResults);
            XmlReader reader = XmlReader.Create(input);
            reader.MoveToContent();

            var xmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Modulo.Collect.GraphicalConsole.results_to_html.xsl");
            XmlReader xmlReader = XmlReader.Create(xmlStream);
            xslt.Load(xmlReader);

            var xsltArgs = new XsltArgumentList();
            var outputStream = new MemoryStream();
            xslt.Transform(reader, xsltArgs, outputStream);
            outputStream.Flush();
            outputStream.Position = 0;

            xmlReader.Close();

            return outputStream;
        }

        private MemoryStream GetStreamFromUTF8String(string ovalResults)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(ovalResults);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;
            stream.Flush();

            return stream;
        }

        private String GetSystemCharacteristicsFromOvalResults(string ovalResults)
        {
            if (String.IsNullOrEmpty(ovalResults))
                return null;

            var posScStart = ovalResults.IndexOf("<oval_system_characteristics");
            var posScEnd = ovalResults.IndexOf("</oval_system_characteristics>");
            if ((posScStart <= 0) || (posScStart >= posScEnd))
                return null;

            return ovalResults.Substring(posScStart, posScEnd + "</oval_system_characteristics>".Length - posScStart);
        }
        #endregion

        public virtual void Sleep(int interval)
        {
            System.Threading.Thread.Sleep(interval * 1000);
        }

        public virtual void StartThread(RequestCollectionEvenArgs e)
        {
            Thread thread = new Thread(this.Run);
            thread.Start(e);
        }

        public virtual void SaveFile(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

        public virtual String ReadFile(string filename)
        {
            return File.ReadAllText(filename);
        }

        #region Credentials
        public virtual string EncryptCredentials(string username, string password, string administrativePassword)
        {
            var certificate = new X509Certificate2(modSicConnection.GetCertificate());
            var CryptoProvider = new ClientCryptoProvider();

            var resolvedUsername = this.ResolveUsername(username);
            return CryptoProvider.EncryptCredential(certificate, resolvedUsername.Key, resolvedUsername.Value, password, administrativePassword);
        }

        private KeyValuePair<String, String> ResolveUsername(string username)
        {
            var domain = string.Empty;
            var splittedUsername = username.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            if (splittedUsername.Count() == 2)
            {
                domain = splittedUsername.First();
                username = splittedUsername.Last();
            }

            return new KeyValuePair<String, String>(domain, username);
        }
        #endregion
    }
}
