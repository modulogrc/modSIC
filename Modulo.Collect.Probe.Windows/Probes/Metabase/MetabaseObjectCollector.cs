/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Independent.XmlFileContent;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Windows.Probes.Metabase
{
    public class MetabaseObjectCollector : BaseObjectCollector
    {
        private const string METABASE_CONTENT_NOT_FOUND_MSG = "The metabase content was not found. Check administrative sharing folder in target system.";
        private const string METABASE_FILENAME = @"c:\windows\system32\inetsrv\metabase.xml";
        private const string MB_SCHEMA_FILENAME = @"c:\windows\system32\inetsrv\mbschema.xml";
        private string MetabaseFileContent = null;
        private string MetabaseSchemaContent = null;

        
        public MetabaseObjectCollector(WindowsFileProvider fileProvider)
        {
            try
            {
                var metabaseFileLines = fileProvider.GetFileLinesContentFromHost(METABASE_FILENAME);
                if (metabaseFileLines != null)
                {
                    this.MetabaseFileContent = String.Join(Environment.NewLine, metabaseFileLines);

                    var mbSchemaFileLines = fileProvider.GetFileLinesContentFromHost(MB_SCHEMA_FILENAME);
                    if (mbSchemaFileLines != null)
                        this.MetabaseSchemaContent = String.Join(Environment.NewLine, mbSchemaFileLines);
                }
            }
            catch (Exception)
            {
                this.MetabaseFileContent = string.Empty;
                this.MetabaseSchemaContent = string.Empty;
            }
        }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var metabaseItem = (metabase_item)systemItem;

            try
            {
                if (string.IsNullOrWhiteSpace(MetabaseFileContent) || string.IsNullOrWhiteSpace(MetabaseSchemaContent))
                    throw new Exception(METABASE_CONTENT_NOT_FOUND_MSG);
                
                var metabaseSession = GetMetabaseProperty(metabaseItem.key.Value, metabaseItem.id1.Value);
                metabaseItem.data = CreateMetabaseEntityItemFromMetabaseDataProperty(metabaseSession.Value);
                
                if ((metabaseItem.data.Length == 1) && (metabaseItem.data[0].status == StatusEnumeration.doesnotexist))
                {
                    metabaseItem.status = StatusEnumeration.doesnotexist;
                    metabaseItem.id1.status = StatusEnumeration.doesnotexist;
                    metabaseItem.name = null;
                    metabaseItem.data_type = null;
                    metabaseItem.user_type = null;
                }
                else
                {
                    metabaseItem.name = CreateMetabaseEntityFromMetabaseProperty(metabaseSession.Name);
                    metabaseItem.data_type = CreateMetabaseEntityFromMetabaseProperty(metabaseSession.Type);
                    metabaseItem.user_type = CreateMetabaseEntityFromMetabaseProperty(metabaseSession.UserType);
                }
            }
            catch (MetabaseItemNotFoundException)
            {
                metabaseItem.status = StatusEnumeration.doesnotexist;
                metabaseItem.key.status = metabaseItem.status;
            }   
         
            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private MetabaseSessionConfiguration GetMetabaseProperty(string location, string propertyID)
        {
            var rootLocation = "/LM/W3SVC";
            if (location.Contains("/LM/W3SVC/AppPools"))
                rootLocation = "/LM/W3SVC/AppPools";
            else if (location.Contains("/LM/W3SVC/Filters"))
                rootLocation = "/LM/W3SVC/Filters";
            
            while (true)
            {
                try
                {
                    var xmlFragmentFromMetabase = this.GetXmlFragmentForLocation(location);
                    var metabaseFragment = new MetabaseFragment(xmlFragmentFromMetabase, MetabaseSchemaContent);
                    return metabaseFragment.GetMetabaseSessionByID(propertyID);
                }
                catch (XPathNoResultException)
                {
                    if (location.Equals(rootLocation))
                        throw new MetabaseItemNotFoundException();

                    var locationParts = location.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (locationParts.Count() < 2)
                        throw new MetabaseItemNotFoundException();

                    location = location.Replace("/" + locationParts.Last(), string.Empty);
                }
            }
        }

        public virtual IEnumerable<String> GetAllMetabaseKeys()
        {
            return new XPathOperator().applyXpath(this.MetabaseFileContent, "//*/@Location");
        }

        public virtual IEnumerable<String> GetAllMetabaseIDs()
        {
            return new XPathOperator().applyXpath(this.MetabaseSchemaContent, "//*/@ID");
        }

        private string GetXmlFragmentForLocation(string location)
        {
            var xpathToGetAllIIisConfigurationByLocation = string.Format("//*[@Location = \"{0}\"]", location);
            var allIisProperties = new XPathOperator().applyXpath(MetabaseFileContent, xpathToGetAllIIisConfigurationByLocation, true);
            return string.Join(Environment.NewLine, allIisProperties);
        }

        private EntityItemAnySimpleType[] CreateMetabaseEntityItemFromMetabaseDataProperty(string metabaseDataProperty)
        {
            if (metabaseDataProperty == null)
                return new EntityItemAnySimpleType[] { new EntityItemAnySimpleType() { status = StatusEnumeration.doesnotexist } };

            var metabaseDataValues = metabaseDataProperty.Split(new char[] { ';' });

            var itemEntities = new List<EntityItemAnySimpleType>();
            foreach(var metabaseDataValue in metabaseDataValues)
                itemEntities.Add(new EntityItemAnySimpleType() { Value = metabaseDataValue });
            return itemEntities.ToArray();
        }

        private EntityItemStringType CreateMetabaseEntityFromMetabaseProperty(string metabaseProperty)
        {
            return new EntityItemStringType()
            {
                Value = metabaseProperty,
                status = metabaseProperty == null ? StatusEnumeration.doesnotexist : StatusEnumeration.exists
            };
        }

        
    }

    public class MetabaseItemNotFoundException : Exception { }
}
