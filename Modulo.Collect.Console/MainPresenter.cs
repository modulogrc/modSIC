#region License
/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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
using CommandLine;
using System.Configuration;
using System.Text.RegularExpressions;
using Modulo.Collect.Service.Contract;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Modulo.Collect.ClientConsole
{
    public enum Operation { SendCollect, SendCollectSync, GetResults, CancelCollect, ListCollectsInExecution, ListAllCollectsInExecution, ValidateSchematronOnly, Error };

    public class MainPresenter
    {
        #region Constants
        private const string COULD_NOT_OPEN_FILE = "An error occurred while trying to open the {0} file '{1}': '{2}'";
        private const string COLLECT_WAS_SENT = "Collect was sent successfully. The request collect ID is: {0}";
        private const string WAS_NOT_POSSIBLE_TO_GET_OVAL_DEFINITIONS = "The collect was not requested because it was not possible to get Oval Definitions.";

        const string InvalidParametersMessage = "Invalid command line parameters.";
        const string InvalidRequestCollectMessage = "Invalid request collect parameter.";
        const string DefinitionsNotFoundMessage = "Oval definitions file not found.";
        const string NoCollectionIsBeingPerformedMessage = "No collection is being performed.";
        const string CollectCanceledMessage = "The collect was canceled successfully.";

        const string defaultDefinitionsFilename = "definitions.xml";
        const string defaultResultsFilename = "results.xml";
        const string defaultExternalVariableFilename = "external-variables.xml";
        const string defaultOvalSchemaPath = "xml";
        const string defaultSystemCharacteristicsFilename = "system-characteristics.xml";
        #endregion

        #region Private Members
        private IMainView view;
        private IMainModel model;
        private Options options = null;
        private CollectServerParameters server = null;
        private TargetParameters target = null;
        private string definitionsFilename;
        private string resultsFilename;
        private string externalVariableFilename;
        private string ovalSchemaPath;
        private string systemCharacteristicsFilename;
        private ServerSection serverSection = null;
        private ModSicService modSICService = null;
        #endregion

        public MainPresenter(IMainView _view, IMainModel _model)
        {
            view = _view;
            model = _model;
        }

        public bool PrepareOptions(string[] args)
        {
            options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(System.Console.Error));
            if (!parser.ParseArguments(args, options))
            {
                return false;
            }

            return true;
        }

        public void Start(string[] args)
        {
            if (!PrepareOptions(args))
                return;

            var operation = CheckParameters();
            if (operation == Operation.Error)
            {
                view.ShowMessage(System.Environment.NewLine);
                view.ShowMessage(options.GetUsage());
                return;
            }

            ProcessParameters();

            if (operation == Operation.SendCollect || operation == Operation.SendCollectSync)
            {
                if (!ValidateSchema())
                    return;
                if (options.ValidateSchematron)
                {
                    if (!ValidadeSchematron())
                        return;
                }

                var certificate = GetCertificate();
                target =
                    new TargetParameters(
                        options.TargetAddress,
                        options.TargetUsername,
                        options.TargetPassword,
                        options.TargetAdministrativePassword,
                        certificate);
            }

            Execute(operation);
        }

        private void Execute(Operation operation)
        {
            modSICService = new ModSicService(this.server);
            switch (operation)
            {
                case Operation.ValidateSchematronOnly:
                    if (!ValidateSchema())
                        return;
                    if (options.ValidateSchematron)
                    {
                        if (!ValidadeSchematron())
                            return;
                    }
                    break;

                case Operation.SendCollect:
                    {
                        var ovalDefinitions = GetFileContents("Oval Definitions", this.definitionsFilename);
                        var ovalVariables = GetOvalExternalVariablesFile();
                        Dictionary<string, string> targetParameters = null;
                        if (!String.IsNullOrEmpty(options.SSHPort))
                        {
                            targetParameters = new Dictionary<string, string>();
                            targetParameters.Add("sshPort", options.SSHPort);
                        }

                        if (ovalDefinitions != null)
                        {
                            view.ShowMessage("Requesting collect...");
                            var sendCollectResult = modSICService.SendCollect(target.Address, CreateCredentials(), ovalDefinitions, ovalVariables, targetParameters);

                            view.ShowMessage(String.Format(COLLECT_WAS_SENT, sendCollectResult.First().Value));
                        }
                        else
                        {
                            view.ShowMessage(WAS_NOT_POSSIBLE_TO_GET_OVAL_DEFINITIONS);
                        }

                        break;
                    }

                case Operation.SendCollectSync:
                    {
                        var ovalDefinitions = GetFileContents("Oval Definitions", this.definitionsFilename);
                        var ovalVariables = GetOvalExternalVariablesFile();
                        Dictionary<string, string> targetParameters = null;
                        if (!String.IsNullOrEmpty(options.SSHPort))
                        {
                            targetParameters = new Dictionary<string, string>();
                            targetParameters.Add("sshPort", options.SSHPort);
                        }

                        if (ovalDefinitions != null)
                        {
                            string collectRequestID;

                            view.ShowMessage("Requesting collect...");
                            var credentials = this.CreateCredentials();
                            var ovalResults = modSICService.SendCollectSynchronous(target.Address, credentials, ovalDefinitions, out collectRequestID, 10, ovalVariables, targetParameters);

                            SaveResults(ovalResults, collectRequestID);
                            view.ShowMessage("[Operation completed]");
                        }
                        else
                        {
                            view.ShowMessage(WAS_NOT_POSSIBLE_TO_GET_OVAL_DEFINITIONS);
                        }

                        break;
                    }

                case Operation.GetResults:
                    {
                        var collectRequestID = this.options.RequestId;
                        view.ShowMessage(String.Format("Trying to get Oval Results for '{0}'...", collectRequestID));
                        var ovalResults = modSICService.GetOvalResults(collectRequestID);
                        SaveResults(ovalResults, collectRequestID);
                        view.ShowMessage("[Operation completed]");

                        break;
                    }

                case Operation.CancelCollect:
                    {
                        var collectRequestID = this.options.CancelRequestId;
                        view.ShowMessage(String.Format("Trying to cancel '{0}'...", collectRequestID));
                        if (modSICService.CancelCollect(collectRequestID))
                        {
                            view.ShowMessage(CollectCanceledMessage);
                        }
                        else
                        {
                            view.ShowMessage("The collect request was not canceled by modSIC because weather it is no longer in execution or the collect request does not exist.");
                        }

                        break;
                    }

                case Operation.ListCollectsInExecution:
                case Operation.ListAllCollectsInExecution:
                    {
                        view.ShowMessage("Trying to get collections in execution...");
                        var collectsInExecutions = (operation == Operation.ListCollectsInExecution) ?
                            modSICService.GetCollectionsInExecution(options.VerboseOutput) :
                            modSICService.GetAllCollectionsInExecution(options.VerboseOutput);

                        view.ShowMessage(String.Format("modSIC server version is {0}, providing API {1}",
                                                        modSICService.ServerProgramVersion,
                                                        modSICService.ServerAPIVersion));

                        if ((collectsInExecutions != null) && (collectsInExecutions.Count() > 0))
                            view.ShowMessages(collectsInExecutions.Select(col => col.Key + " on " + col.Value).ToArray());
                        else
                            view.ShowMessage(NoCollectionIsBeingPerformedMessage);

                        break;
                    }
            }
        }

        private Credential CreateCredentials()
        {
            return new Credential
            {
                UserName = this.options.TargetUsername,
                Password = this.options.TargetPassword,
                AdministrativePassword = this.options.TargetAdministrativePassword
            };
        }

        private bool ValidadeSchematron()
        {
            if (!File.Exists(definitionsFilename))
            {
                view.ShowMessage(DefinitionsNotFoundMessage);
                return false;
            }

            view.ShowMessage("Validating schematron.");

            XslCompiledTransform xslt = new XslCompiledTransform();
            var xmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Modulo.Collect.ClientConsole.schematron-compiled.xsl");
            XmlReader xmlReader = XmlReader.Create(xmlStream);
            xslt.Load(xmlReader);

            var xsltArgs = new XsltArgumentList();
            var stream = new MemoryStream();
            xslt.Transform(definitionsFilename, xsltArgs, stream);
            stream.Flush();
            stream.Position = 0;

            XPathDocument document = new XPathDocument(stream);
            XPathNavigator navigator = document.CreateNavigator();
            XPathExpression query = navigator.Compile("//svrl:successful-report/svrl:text");
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("svrl", "http://purl.oclc.org/dsdl/svrl");
            query.SetContext(manager);
            XPathNodeIterator nodes = navigator.Select(query);

            if (nodes.Count == 0)
            {
                view.ShowMessage("Schematron validation succeeded.");
                return true;
            }
            else
            {
                List<string> messages = new List<string>();
                messages.Add("Schematron errors:");
                while (nodes.MoveNext())
                {
                    messages.Add(String.Format("\t{0}", nodes.Current.Value.Trim()));
                }
                view.ShowMessages(messages.ToArray());
                return false;
            }
        }

        private void SaveResults(string ovalResults, string collectRequestID)
        {
            if (!String.IsNullOrEmpty(ovalResults))
            {
                SaveOvalDocument(ovalResults, this.resultsFilename, "Oval Results");

                var systemCharacteristics = GetSystemCharacteristicsFromOvalResultsXML(ovalResults);
                if (!String.IsNullOrEmpty(systemCharacteristics))
                    SaveOvalDocument(systemCharacteristics, this.systemCharacteristicsFilename, "Oval System Characteristics");
                else
                    view.ShowMessage("It was not possible to get System Characteristics from Oval Results document.");
            }
            else
            {
                view.ShowMessage("modSIC returned a empty Oval Results.");

                var result = modSICService.GetResultDocument(collectRequestID);
                if (result != null)
                {
                    var systemCharacteristics = result.SystemCharacteristics;
                    if (!String.IsNullOrEmpty(systemCharacteristics))
                    {
                        SaveOvalDocument(systemCharacteristics, this.systemCharacteristicsFilename, "Oval System Characteristics");
                    }                  
                }
            }
        }

        public Operation CheckParameters()
        {
            ReadServerSection();
            ReadServerParameters();

            Operation operation = Operation.Error;

            var i = 0;
            if (options.SendCollect)
            {
                operation = Operation.SendCollect;
                i++;
            }
            if (options.SendCollectSync)
            {
                operation = Operation.SendCollectSync;
                i++;
            }
            if (options.ListCollectsInExecution)
            {
                operation = Operation.ListCollectsInExecution;
                i++;
            }
            if (options.ListAllCollectsInExecution)
            {
                operation = Operation.ListAllCollectsInExecution;
                i++;
            }
            if (!String.IsNullOrEmpty(options.CancelRequestId))
            {
                operation = Operation.CancelCollect;
                i++;
            }
            if (!String.IsNullOrEmpty(options.RequestId))
            {
                operation = Operation.GetResults;
                i++;
            }
            if (i > 1)
            {
                view.ShowMessage(InvalidParametersMessage);
                return Operation.Error;
            }
            else if (i == 0)
            {
                if (options.ValidateSchematron && String.IsNullOrEmpty(options.Preset))
                {
                    return Operation.ValidateSchematronOnly;
                }
                return Operation.Error;
            }

            //  Server
            if (String.IsNullOrEmpty(options.ServerAddress) || String.IsNullOrEmpty(options.ServerUsername) || String.IsNullOrEmpty(options.ServerPassword))
            {
                view.ShowMessage(InvalidParametersMessage);
                return Operation.Error;
            }

            switch (operation)
            {
                case Operation.SendCollect:
                case Operation.SendCollectSync:
                    if (!String.IsNullOrEmpty(options.Preset))
                    {
                        if (!ReadPresetParameters(options.Preset))
                        {
                            view.ShowMessage(InvalidParametersMessage);
                            return Operation.Error;
                        }
                    }

                    //  Target
                    if (String.IsNullOrEmpty(options.TargetAddress) || String.IsNullOrEmpty(options.TargetUsername) || String.IsNullOrEmpty(options.TargetPassword))
                    {
                        view.ShowMessage(InvalidParametersMessage);
                        return Operation.Error;
                    }
                    break;

                case Operation.ListCollectsInExecution:
                case Operation.ListAllCollectsInExecution:
                case Operation.GetResults:
                case Operation.CancelCollect:
                    if (!String.IsNullOrEmpty(options.TargetAddress) || !String.IsNullOrEmpty(options.TargetUsername) || !String.IsNullOrEmpty(options.TargetPassword)
                        || options.ValidateSchematron || !String.IsNullOrEmpty(options.Preset))
                    {
                        view.ShowMessage(InvalidParametersMessage);
                        return Operation.Error;
                    }

                    if ((operation != Operation.ListCollectsInExecution) && (operation != Operation.ListAllCollectsInExecution))
                    {
                        var requestCollect = String.IsNullOrEmpty(options.RequestId) ? options.CancelRequestId : options.RequestId;
                        string re = @"^collectrequests/\d*$";
                        Regex r = new Regex(re, RegexOptions.Singleline);
                        Match m = r.Match(requestCollect);
                        if (!m.Success)
                        {
                            view.ShowMessage(InvalidRequestCollectMessage);
                            return Operation.Error;
                        }
                    }
                    break;
            }

            return operation;
        }

        public virtual ServerSection ReadServerSection()
        {
            if (serverSection != null)
            {
                return serverSection;
            }
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.GetSection("ServerSection") as ServerSection;
        }

        #region Private Methods

        private bool ReadPresetParameters(string name)
        {
            var serverSection = ReadServerSection();
            if (serverSection != null && serverSection.Collects != null)
            {
                var preset = serverSection.Collects[name];
                if (preset != null)
                {
                    options.TargetAddress = String.IsNullOrEmpty(options.TargetAddress) ? preset.Target : options.TargetAddress;
                    options.TargetUsername = String.IsNullOrEmpty(options.TargetUsername) ? preset.Username : options.TargetUsername;
                    options.TargetPassword = String.IsNullOrEmpty(options.TargetPassword) ? preset.Password : options.TargetPassword;
                    options.OvalDefinitionFilename = String.IsNullOrEmpty(options.OvalDefinitionFilename) ? preset.Definitions : options.OvalDefinitionFilename;

                    return true;
                }
            }

            return false;
        }

        private void ReadServerParameters()
        {
            var serverSection = ReadServerSection();
            if (serverSection != null)
            {
                if (serverSection.Server != null)
                {
                    options.ServerAddress = !String.IsNullOrEmpty(options.ServerAddress) ? options.ServerAddress : serverSection.Server.address;
                    options.ServerUsername = !String.IsNullOrEmpty(options.ServerUsername) ? options.ServerUsername : serverSection.Server.username;
                    options.ServerPassword = !String.IsNullOrEmpty(options.ServerPassword) ? options.ServerPassword : serverSection.Server.password;
                }
            }
        }

        private void ProcessParameters()
        {
            definitionsFilename = String.IsNullOrEmpty(options.OvalDefinitionFilename) ? defaultDefinitionsFilename : options.OvalDefinitionFilename;
            externalVariableFilename = String.IsNullOrEmpty(options.ExternalVariableFilename) ? defaultExternalVariableFilename : options.ExternalVariableFilename;
            ovalSchemaPath = String.IsNullOrEmpty(options.OvalSchemaPath) ? defaultOvalSchemaPath : options.OvalSchemaPath;
            systemCharacteristicsFilename = String.IsNullOrEmpty(options.SystemCharacteristicsFilename) ? defaultSystemCharacteristicsFilename : options.SystemCharacteristicsFilename;
            resultsFilename = String.IsNullOrEmpty(options.ResultsFilename) ? defaultResultsFilename : options.ResultsFilename;

            server = new CollectServerParameters();
            server.Address = String.Format("http://{0}/CollectService", options.ServerAddress);
            server.Username = options.ServerUsername;
            server.Password = options.ServerPassword;

            var attrib = (GuidAttribute) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
            server.ClientId = System.Environment.MachineName + "\\" + attrib.Value;
        }

        private void SaveOvalDocument(string documentContents, string documentFilename, string documentDescription)
        {
            view.ShowMessage(String.Format("Saving {0}...", documentDescription));
            System.IO.File.WriteAllText(documentFilename, documentContents);
            view.ShowMessage(String.Format("The {0} Document was saved in {1}", documentDescription, documentFilename));
        }

        private String GetSystemCharacteristicsFromOvalResultsXML(string ovalResults)
        {
            if (String.IsNullOrEmpty(ovalResults))
                return null;

            var posScStart = ovalResults.IndexOf("<oval_system_characteristics");
            var posScEnd = ovalResults.IndexOf("</oval_system_characteristics>");
            if ((posScStart <= 0) || (posScStart >= posScEnd))
                return null;

            return ovalResults.Substring(posScStart, posScEnd + "</oval_system_characteristics>".Length - posScStart);
        }

        private void ShowErrorMessage(string operationDescription, Exception exception)
        {
            view.ShowMessage(
                String.Format("An error occurred while trying to {0}: {1}: '{2}'.",
                    operationDescription,
                    exception.GetType().Name,
                    exception.Message));
        }

        private string GetOvalExternalVariablesFile()
        {
            if (string.IsNullOrEmpty(this.externalVariableFilename))
                return null;

            if (!System.IO.File.Exists(this.externalVariableFilename))
                return null;

            return this.GetFileContents("Oval External Variables", this.externalVariableFilename);
        }

        private string GetFileContents(string fileDescription, string filepath)
        {
            try
            {
                return System.IO.File.ReadAllText(filepath);
            }
            catch (FileNotFoundException)
            {
                this.view.ShowMessage(String.Format("{0} file not found ('{1}')", fileDescription, filepath));
                return null;
            }
            catch (Exception ex)
            {
                this.view.ShowMessage(String.Format(COULD_NOT_OPEN_FILE, fileDescription, filepath, ex.Message));
                return null;
            }
        }

        private X509Certificate2 GetCertificate()
        {
            var certificate = new ModSicService(server).GetCertificate();
            return new X509Certificate2(certificate);
        }

        private bool ValidateSchema()
        {
            if (!File.Exists(definitionsFilename))
            {
                view.ShowMessage(DefinitionsNotFoundMessage);
                return false;
            }

            view.ShowMessage("Validating xml schema.");

            var schemas = CreateXmlSchemaSet();
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
            settings.ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationEventHandler;

            using (var validationReader = XmlReader.Create(definitionsFilename, settings))
            {
                while (validationReader.Read()) { }
            }

            return true;
        }

        private XmlSchemaSet CreateXmlSchemaSet()
        {
            var schemas = new XmlSchemaSet();

            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-common-5", "oval-common-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5", "oval-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#aix", "aix-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#apache", "apache-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#catos", "catos-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#esx", "esx-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#freebsd", "freebsd-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#hpux", "hpux-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#pixos", "pixos-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#sharepoint", "sharepoint-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#solaris", "solaris-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#unix", "unix-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#windows", "windows-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#independent", "independent-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#ios", "ios-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#linux", "linux-definitions-schema.xsd");
            AddSchema(schemas, "http://oval.mitre.org/XMLSchema/oval-definitions-5#macos", "macos-definitions-schema.xsd");
            AddSchema(schemas, "http://www.w3.org/2000/09/xmldsig#", "xmldsig-core-schema.xsd");

            return schemas;
        }

        private void AddSchema(XmlSchemaSet schemas, string targetNamespace, string schemaUri)
        {
            var file = Path.Combine(ovalSchemaPath, schemaUri);
            if (File.Exists(file))
            {
                schemas.Add(targetNamespace, file);
            }
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("\tSeverity: {0}{1}", args.Severity.ToString(), Environment.NewLine);
                sb.AppendFormat("\tMessage: {0}{1}", args.Message, Environment.NewLine);
                sb.AppendFormat("\tFile: {0}{1}", definitionsFilename, Environment.NewLine);
                sb.AppendFormat("\tLine {0}", args.Exception.LineNumber);

                throw new Exception(sb.ToString());
            }
        }
        #endregion
    }
}
