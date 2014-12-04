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
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel.Composition.Hosting;
using Modulo.Collect.OVAL.Plugins;
using Modulo.Collect.OVAL.Schema;
using System.Xml.Schema;

namespace Modulo.Collect.OVAL.SystemCharacteristics
{
    public partial class oval_system_characteristics
    {

        public const int schemaMajorVersion = 5;
        public const int schemaMinorVersion = 10;
        
        /// <summary>
        /// Gets the object type by ID.
        /// </summary>
        /// <param name="ovalId">The oval id.</param>
        /// <returns></returns>
        public ObjectType GetCollectedObjectByID(string ovalId)
        {           
            ObjectType objectType = this.collected_objects.Where<ObjectType>(obj => obj.id == ovalId).SingleOrDefault();
            return objectType;
        }

        /// <summary>
        /// Gets the system data by reference id.
        /// </summary>
        /// <param name="referenceId">The reference id.</param>
        /// <returns></returns>
        public ItemType GetSystemDataByReferenceId(string referenceId)
        {
            ItemType itemType = this.system_data.Where<ItemType>(item => item.id == referenceId).SingleOrDefault();
            return itemType;
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
                    XmlAttributeOverrides DefinitionsOverrides = GetExportedScOverrides();
                    xmlSerializer = new XmlSerializer(typeof(oval_system_characteristics), DefinitionsOverrides);
                    xmlResolver = new ExtensibleXmlResourceResolver();
                }
            }
        }
        public static oval_system_characteristics GetOvalSystemCharacteristicsFromStream(Stream scDocument, out IEnumerable<string> schemaErrors)
        {
            var _schemaErrors = new List<string>();

            PrepareSerializer();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.XmlResolver = xmlResolver;
            settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.AllowXmlAttributes;
            settings.ValidationEventHandler += (o, args) => { if (args.Severity == XmlSeverityType.Error) _schemaErrors.Add(args.Message); };
            XmlReader reader = XmlReader.Create(scDocument, settings);
            oval_system_characteristics result = xmlSerializer.Deserialize(reader) as oval_system_characteristics;
            reader.Close();

            if (_schemaErrors.Count > 0)
                result = null;

            schemaErrors = _schemaErrors;
            return result;

        }


        /// <summary>
        /// Gets the system characteristics XML.
        /// </summary>
        /// <returns>the xml string in the oval system characteristics format.</returns>
        public string GetSystemCharacteristicsXML()
        {
            string systemCharacteristicsXML;


            MemoryStream memoryStream = new MemoryStream();
            PrepareSerializer();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xmlSerializer.Serialize(xmlTextWriter, this);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            systemCharacteristicsXML = this.UTF8ByteArrayToString(memoryStream.ToArray());
            return systemCharacteristicsXML;

        }

        public static XmlAttributeOverrides GetExportedScOverrides()
        {
            var _container = PluginContainer.GetOvalCompositionContainer();

            var itemTypes = _container.GetExports<ItemType>().Select(exp => exp.Value.GetType());

            XmlAttributeOverrides scOverrides = new XmlAttributeOverrides();

            XmlAttributes itemAttributes = new XmlAttributes();
            foreach (var itemType in itemTypes)
            {
                var xmlAttrs = (itemType.GetCustomAttributes(typeof(XmlRootAttribute), false) as XmlRootAttribute[]).SingleOrDefault();
                itemAttributes.XmlArrayItems.Add(new XmlArrayItemAttribute(xmlAttrs.ElementName, itemType) { Namespace = xmlAttrs.Namespace });
            }
            scOverrides.Add(typeof(oval_system_characteristics), "system_data", itemAttributes);

            return scOverrides;
        }


        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// http://www.dotnetjohn.com/articles.aspx?articleid=173 
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);

        }
    }
}
