/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
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


using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.XmlSignatures;
using Modulo.Collect.OVAL.Definitions.helpers;
using Modulo.Collect.OVAL.Plugins;
using Modulo.Collect.OVAL.Schema;


namespace Modulo.Collect.OVAL.Definitions
{
	public partial class oval_definitions
	{
		public const int schemaMajorVersion = 5;
		public const int schemaMinorVersion = 10;

		
		static XmlSerializer xmlSerializer;
        static XmlResolver xmlResolver;
        static object xmlSerializerSync = new object();
		public static oval_definitions GetOvalDefinitionsFromStream(Stream definitionsDocument, out IEnumerable<string> schemaErrors)
		{
			var _schemaErrors = new List<string>();
            PrepareSerializer();

			
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
            settings.XmlResolver = xmlResolver;
			settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.AllowXmlAttributes;
			settings.ValidationEventHandler += (o, args) => { if (args.Severity == XmlSeverityType.Error) _schemaErrors.Add(args.Message); };
			XmlReader reader = XmlReader.Create(definitionsDocument, settings);
			oval_definitions result = xmlSerializer.Deserialize(reader) as oval_definitions;
			reader.Close();

			if (_schemaErrors.Count > 0)
				result = null;

			schemaErrors = _schemaErrors;
			return result;

		}

        private static void PrepareSerializer()
        {
            lock (xmlSerializerSync)
            {
                if (xmlSerializer == null)
                {
                    XmlAttributeOverrides DefinitionsOverrides = GetExportedDefinitionsOverrides();
                    xmlSerializer = new XmlSerializer(typeof(oval_definitions), DefinitionsOverrides);
                    xmlResolver = new ExtensibleXmlResourceResolver();

                }
            }
        }

		public static XmlAttributeOverrides GetExportedDefinitionsOverrides()
		{
			return GetExportedDefinitionsOverrides(new XmlAttributeOverrides());
		}

		public static XmlAttributeOverrides GetExportedDefinitionsOverrides(XmlAttributeOverrides DefinitionsOverrides)
		{

		    var _container = PluginContainer.GetOvalCompositionContainer();

			var testTypes = _container.GetExports<TestType>().Select(exp => exp.Value.GetType());
			var objectTypes = _container.GetExports<ObjectType>().Select(exp => exp.Value.GetType());
			var stateTypes = _container.GetExports<StateType>().Select(exp => exp.Value.GetType());

            XmlAttributes testAttributes = new XmlAttributes();
			foreach (var testType in testTypes)
			{
				var xmlAttrs = (testType.GetCustomAttributes(typeof(XmlRootAttribute), false) as XmlRootAttribute[]).SingleOrDefault();
				testAttributes.XmlArrayItems.Add(new XmlArrayItemAttribute(xmlAttrs.ElementName, testType) { Namespace = xmlAttrs.Namespace });
			}
			DefinitionsOverrides.Add(typeof(oval_definitions), "tests", testAttributes);
			XmlAttributes objectAttributes = new XmlAttributes();
			foreach (var objectType in objectTypes)
			{
				var xmlAttrs = (objectType.GetCustomAttributes(typeof(XmlRootAttribute), false) as XmlRootAttribute[]).SingleOrDefault();
				objectAttributes.XmlArrayItems.Add(new XmlArrayItemAttribute(xmlAttrs.ElementName, objectType) { Namespace = xmlAttrs.Namespace });
			}
			DefinitionsOverrides.Add(typeof(oval_definitions), "objects", objectAttributes);

			XmlAttributes stateAttributes = new XmlAttributes();
			foreach (var stateType in stateTypes)
			{
				var xmlAttrs = (stateType.GetCustomAttributes(typeof(XmlRootAttribute), false) as XmlRootAttribute[]).SingleOrDefault();
				stateAttributes.XmlArrayItems.Add(new XmlArrayItemAttribute(xmlAttrs.ElementName, stateType) { Namespace = xmlAttrs.Namespace });
			}
			DefinitionsOverrides.Add(typeof(oval_definitions), "states", stateAttributes);

			return DefinitionsOverrides;
		}

		public string GetDefinitionsXml()
		{
			MemoryStream memoryStream = new MemoryStream();
            PrepareSerializer();
			xmlSerializer.Serialize(memoryStream, this);
			memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
			using (StreamReader streamReader = new System.IO.StreamReader(memoryStream))
			{
				return streamReader.ReadToEnd();
			}
		}

		public bool VerifySignature(out X509Certificate2 certificate)
		{
			return XmlSignatureHelper.VerifySignature(GetDefinitionsXml(), out certificate);
		}

		public bool VerifySignature(IEnumerable<X509Certificate2> trustedCertificates)
		{
			return XmlSignatureHelper.VerifySignature(GetDefinitionsXml(), trustedCertificates);
		}

		public void Sign(X509Certificate2 certificate)
		{
			string xml = GetDefinitionsXml();
			var signatureElement = XmlSignatureHelper.Sign(xml, certificate);

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SignatureType));
			using (StringReader stringReader = new StringReader(signatureElement.OuterXml))
			{
				Signature = ((SignatureType)(xmlSerializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
			}
		}
	}
}
