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
using System;
using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Unix.SSHCollectors;

namespace Modulo.Collect.Probe.Unix.File
{
    public class FileObjectCollector : BaseObjectCollector
    {
        public FileCollector FilesCollector { get; set; }

        public FileObjectCollector(FileCollector filesCollector)
        {
            this.FilesCollector = filesCollector;
        }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(Modulo.Collect.OVAL.SystemCharacteristics.ItemType systemItem)
        {
            var fileItem = (file_item)systemItem;
            try
            {
                var completeFilepath = GetCompleteFilepath(fileItem);
                var collectedVariableValue = this.TryToCollectFile(completeFilepath);

                var shortFileTime = this.ToShortFileTime(collectedVariableValue.LastModified);
                fileItem.m_time = OvalHelper.CreateItemEntityWithIntegerValue(shortFileTime);
                fileItem.filepath = OvalHelper.CreateItemEntityWithStringValue(completeFilepath);
                fileItem.size = OvalHelper.CreateItemEntityWithIntegerValue(collectedVariableValue.FileSize.ToString());
                fileItem.type = OvalHelper.CreateItemEntityWithStringValue(collectedVariableValue.FileType);

                fileItem.suid = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0800) != 0);
                fileItem.sgid = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0400) != 0);
                fileItem.sticky = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0200) != 0);
                fileItem.uread = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0100) != 0);
                fileItem.uwrite = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0080) != 0);
                fileItem.uexec = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0040) != 0);
                fileItem.gread = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0020) != 0);
                fileItem.gwrite = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0010) != 0);
                fileItem.gexec = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0008) != 0);
                fileItem.oread = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0004) != 0);
                fileItem.owrite = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0002) != 0);
                fileItem.oexec = OvalHelper.CreateBooleanEntityItemFromBoolValue((collectedVariableValue.Mode & 0x0001) != 0);

                fileItem.group_id = OvalHelper.CreateItemEntityWithIntegerValue(collectedVariableValue.Group);
                fileItem.user_id = OvalHelper.CreateItemEntityWithIntegerValue(collectedVariableValue.Owner);
            }
            catch (FileNotExistsException)
            {
                base.SetDoesNotExistStatusForItemType(systemItem, String.Format("{0}/{1}", fileItem.path.Value, fileItem.filename.Value));
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private string GetCompleteFilepath(file_item fileItem) 
        {
            if (fileItem.filepath != null && !string.IsNullOrWhiteSpace(fileItem.filepath.Value))
                return fileItem.filepath.Value.Trim();

            var path = fileItem.path.Value.TrimStart();
            var pathSeparator = path.EndsWith("/") ? string.Empty : "/";
            
            var filename = string.Empty;
            if ((fileItem.filename != null) && (!string.IsNullOrWhiteSpace(fileItem.filename.Value)))
                filename = fileItem.filename.Value;

            var completeFilepath = String.Format("{0}{1}{2}", path, pathSeparator, filename);
            if (!completeFilepath.StartsWith("/"))
                completeFilepath = "/" + completeFilepath;
            return completeFilepath;

        }

        private FileInfo TryToCollectFile(string fileName)
        {
            FileInfo variableValue = null;
            var allFiles = this.FilesCollector.GetTargetFileInfo(fileName);

            allFiles.TryGetValue(fileName, out variableValue);

            if (variableValue == null)
                throw new FileNotExistsException();

            return variableValue;
        }

        private string ToShortFileTime(DateTime dateTimeToConvert)
        {
            var fileTime = dateTimeToConvert.ToFileTimeUtc().ToString();
            return string.Join("", fileTime.Take(11));
        }
    }
}
