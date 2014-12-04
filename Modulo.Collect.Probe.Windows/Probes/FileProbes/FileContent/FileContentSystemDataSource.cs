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
using System.IO;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Windows.FileContent.Helpers;
using Modulo.Collect.Probe.Windows.Helpers;

namespace Modulo.Collect.Probe.Windows.FileContent
{
    public class FileContentSystemDataSource : BaseObjectCollector
    {
        private string hostUNC { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var filePath = string.Empty;
            try
            {
                var fileContentItem = (textfilecontent_item)systemItem;
                filePath = Path.Combine(fileContentItem.path.Value, fileContentItem.filename.Value);
                var interimList = WinNetUtils.getWinTextFileContent(hostUNC, filePath, fileContentItem.line.Value);

                var collectedItems = new List<CollectedItem>();
                foreach (FileContentItemSystemData srcItem in interimList)
                {
                    var destItem = new textfilecontent_item();
                    this.BuilderFileContentItem(destItem, srcItem, fileContentItem);
                    var newCollectedItem = new CollectedItem() { ItemType = destItem, ExecutionLog = BuildExecutionLog() };
                    collectedItems.Add(newCollectedItem);
                }

                return collectedItems;
            }
            catch (FileNotFoundException)
            {
                base.SetDoesNotExistStatusForItemType(systemItem, filePath);
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private void BuilderFileContentItem(textfilecontent_item item, FileContentItemSystemData itemSystemData, textfilecontent_item fileContentItem)
        {
            item.line = new EntityItemStringType() { Value = itemSystemData.Line.ToString(), datatype = Modulo.Collect.OVAL.Common.SimpleDatatypeEnumeration.@string, status = StatusEnumeration.exists };
            List<EntityItemAnySimpleType> destSubs = new List<EntityItemAnySimpleType>();
            item.filename = new EntityItemStringType() { Value = itemSystemData.FileName.ToString(), datatype = Modulo.Collect.OVAL.Common.SimpleDatatypeEnumeration.@string, status = StatusEnumeration.exists };
            foreach (string thisSub in itemSystemData.SubExpressions)
                destSubs.Add(new EntityItemAnySimpleType() { Value = thisSub, datatype = Modulo.Collect.OVAL.Common.SimpleDatatypeEnumeration.@string, status = StatusEnumeration.exists });
            item.subexpression = destSubs.ToArray();

            item.filepath = fileContentItem.filepath;
            item.path = fileContentItem.path;
            item.pattern = fileContentItem.pattern;
            item.status = StatusEnumeration.exists;
        }

        public FileContentSystemDataSource(string hostaddr)
        {
            this.hostUNC = "\\\\" + hostaddr;
        }

        public string File { get; set; }
    }
}
