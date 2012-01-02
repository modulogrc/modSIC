/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Independent.XmlFileContent
{
    public class XmlFileContentObjectCollector: BaseObjectCollector
    {
        public XPathOperator XPathOperator { get; set; }

        public override IList<String> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var xmlFilepath = string.Empty;
            var xmlFileContentItem = (xmlfilecontent_item)systemItem;
            
            try
            {
                xmlFilepath = this.GetCompleteFilepath(xmlFileContentItem);
                if (string.IsNullOrWhiteSpace(xmlFilepath))
                    throw new XPathNoResultException();
                var xPathResult = this.XPathOperator.Apply(xmlFilepath, xmlFileContentItem.xpath.Value);
                this.ConfigureXmlFileContentItem(xmlFileContentItem, xPathResult);
            }
            catch (FileNotFoundException)
            {
                var completeFilepath = this.GetCompleteFilepath((xmlfilecontent_item)xmlFileContentItem);
                base.SetDoesNotExistStatusForItemType(systemItem, completeFilepath);
            }
            catch (XPathNoResultException)
            {
                var xpath = string.Format("xpath: '{0}'", xmlFileContentItem.xpath.Value);
                base.SetDoesNotExistStatusForItemType(systemItem, xpath);
            }
            
            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private void ConfigureXmlFileContentItem(xmlfilecontent_item xmlFileContentItem, IEnumerable<String> xPathResult)
        {
            var completeFilepath = this.GetCompleteFilepath(xmlFileContentItem);
            xmlFileContentItem.filepath = OvalHelper.CreateItemEntityWithStringValue(completeFilepath);
            xmlFileContentItem.path = OvalHelper.CreateItemEntityWithStringValue(Path.GetDirectoryName(completeFilepath));
            xmlFileContentItem.filename = OvalHelper.CreateItemEntityWithStringValue(Path.GetFileName(completeFilepath));

            xmlFileContentItem.value_of = new EntityItemAnySimpleType[xPathResult.Count()];
            for (int i = 0; i < xPathResult.Count(); i++)
                xmlFileContentItem.value_of[i] = OvalHelper.CreateEntityItemAnyTypeWithValue(xPathResult.ElementAt(i));
        }
            
        private string GetCompleteFilepath(xmlfilecontent_item xmlFileContentItem)
        {
            if (IsFilePathDefined(xmlFileContentItem))
                return xmlFileContentItem.filepath.Value;
            else
                return Path.Combine(xmlFileContentItem.path.Value, xmlFileContentItem.filename.Value);
        }

        private bool IsFilePathDefined(xmlfilecontent_item xmlFileContentItem)
        {
            var filepathEntity = xmlFileContentItem.filepath;
            return ((filepathEntity != null) && (!string.IsNullOrEmpty(filepathEntity.Value)));
        }



        public OVAL.Common.FamilyEnumeration Platform { get; set; }
    }
}
