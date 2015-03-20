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


using System.Xml.Serialization;
using Modulo.Collect.OVAL.Common.XmlSignatures;
using Modulo.Collect.OVAL.Common;
using System.Collections.Generic;
using System.Xml.Linq;
using System;
using System.Text;
using System.IO;
using Modulo.Collect.OVAL.Schema;
using System.Xml;
using System.Xml.Schema;

namespace Modulo.Collect.OVAL.Variables
{
    public partial class oval_variables
    {
        public const int schemaMajorVersion = 5;
        public const int schemaMinorVersion = 9;

        public static oval_variables GetOvalVariablesFromText(string variablesDocumentText, out IEnumerable<string> schemaErrors)
        {
            var _schemaErrors = new List<string>();
            PrepareSerializer();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.XmlResolver = xmlResolver;
            settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.AllowXmlAttributes;
            settings.ValidationEventHandler += (o, args) => { if (args.Severity == XmlSeverityType.Error) _schemaErrors.Add(args.Message); };
            var varMS = new MemoryStream(Encoding.UTF8.GetBytes(variablesDocumentText));
            XmlReader reader = XmlReader.Create(varMS, settings);
            oval_variables result = xmlSerializer.Deserialize(reader) as oval_variables;
            reader.Close();

            if (_schemaErrors.Count > 0)
                result = null;

            schemaErrors = _schemaErrors;
            return result;
            
        }
        static XmlSerializer xmlSerializer;
        static XmlResolver xmlResolver;
        static object xmlSerializerSync = new object();
		
        private static void PrepareSerializer()
        {
            lock (xmlSerializerSync)
            {
                if (xmlSerializer == null)
                {
                    xmlSerializer = new XmlSerializer(typeof(oval_variables));
                    xmlResolver = new ExtensibleXmlResourceResolver();

                }
            }
        }

        public string GetXmlDocument()
        {
            PrepareSerializer();
            string variablesXML;

            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xmlSerializer.Serialize(xmlTextWriter, this);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            memoryStream.Position = 0;
            variablesXML = new UTF8Encoding().GetString(memoryStream.ToArray());
            return variablesXML;           
        }
    }
}
