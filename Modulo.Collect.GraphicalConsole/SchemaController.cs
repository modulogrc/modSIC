#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Forms;

namespace Modulo.Collect.GraphicalConsole
{
    public class SchemaController
    {
        #region Private members
        private ISchemaView view;
        private bool error;
        private StringBuilder errorDescription;
        #endregion

        #region Public Members
        private string SchemaPath { get; set; }
        public bool OnValidadeSchemaCalled { get; set; }
        #endregion

        #region Constructor
        public SchemaController(ISchemaView _view)
        {
            view = _view;
            view.OnValidateSchema += new EventHandler<SchemaEventArgs>(view_OnValidateSchema);

            SchemaPath = Path.Combine(Application.StartupPath, "xml");
        }
        #endregion

        #region View Events
        public void view_OnValidateSchema(object sender, SchemaEventArgs e)
        {
            OnValidadeSchemaCalled = true;

            try
            {
                if (String.IsNullOrEmpty(e.DefinitionFilename))
                {
                    e.ShortErrorMessage = Resource.EmptyDefinitionFilename;
                    e.LongErrorMessage = null;
                    e.Result = false;
                }
                else if (!DefinitionsFileExists(e.DefinitionFilename))
                {
                    e.ShortErrorMessage = Resource.OVALDefinitionsFileNotFound;
                    e.LongErrorMessage = null;
                    e.Result = false;
                }
                else
                {
                    var schemas = CreateXmlSchemaSet();
                    var settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.Schema;
                    settings.Schemas = schemas;
                    settings.ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ReportValidationWarnings;
                    settings.ValidationEventHandler += ValidationEventHandler;

                    this.error = false;
                    this.errorDescription = new StringBuilder();

                    using (var stream = GetStream(e.DefinitionFilename))
                    {
                        var validationReader = XmlReader.Create(stream, settings);

                        while (validationReader.Read()) { }

                        if (this.error)
                        {
                            e.ShortErrorMessage = Resource.SchemaValidationFailure;
                            e.LongErrorMessage = errorDescription.ToString();
                            e.Result = false;
                        }
                        else
                        {
                            validationReader.Close();
                            e.Result = true;
                        }

                        this.errorDescription = null;
                    }
                }
            }
            catch (Exception ex)
            {
                view.ShowErrorMessage(Util.FormatExceptionMessage(ex));
            }
        }

        #endregion

        #region Internal
        public virtual XmlSchemaSet CreateXmlSchemaSet()
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
            var file = Path.Combine(SchemaPath, schemaUri);
            if (File.Exists(file))
            {
                schemas.Add(targetNamespace, file);
            }
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                errorDescription.AppendFormat("Line: {0}{1}", args.Exception.LineNumber, Environment.NewLine);
                errorDescription.AppendFormat("Severity: {0}{1}", args.Severity.ToString(), Environment.NewLine);
                errorDescription.AppendFormat("Message: {0}{1}", args.Message, Environment.NewLine);
                errorDescription.AppendLine();

                this.error = true;               
            }
        }
        #endregion

        #region Useful Methods for Tests
        public virtual Stream GetStream(string filename)
        {
            string fileContent = File.ReadAllText(filename);
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            return stream;
        }

        public virtual bool DefinitionsFileExists(string filename)
        {
            return File.Exists(filename);
        }
        #endregion
    }
}
