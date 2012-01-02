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
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Windows.File;

namespace Modulo.Collect.Probe.Windows.FileEffectiveRights
{
    public enum SourceObjectTypes { FileEffectiveRights, FileEffectiveRights53 }

    public class FileEffectiveRightsItemTypeFactory
    {
        private SourceObjectTypes SourceObjectType;
        private FileEffectiveRightsObjectCollector ObjectCollector;

        public FileEffectiveRightsItemTypeFactory(
            SourceObjectTypes fileEffectiveRightsObjectType, 
            FileEffectiveRightsObjectCollector fileEffectiveRightsObjectCollector)
        {
            this.SourceObjectType = fileEffectiveRightsObjectType;
            this.ObjectCollector = fileEffectiveRightsObjectCollector;
        }
        
        public IEnumerable<ItemType> CreateFileItemTypesByCombinationOfEntitiesFrom(
            IEnumerable<string> paths, IEnumerable<string> fileNames, IEnumerable<string> trustees)
        {
            var itemTypes = new List<ItemType>();
            foreach (var path in paths)
                foreach(var filename in fileNames)
                    foreach (var trustee in trustees)
                        if (ObjectCollector.IsThereUserACLInFileSecurityDescriptor(path, filename, trustee))
                            itemTypes.Add(this.CreateItemTypeForCollect(path, filename, trustee));

            if ((itemTypes.Count == 0) && (paths.Count() == 1))
            {
                var newNotExistsType = this.CreateItemTypeForCollect(paths.First(), null, null);
                newNotExistsType.status = StatusEnumeration.doesnotexist;
                itemTypes.Add(newNotExistsType);
            }
            
            return itemTypes;
        }

        public ItemType CreateFileItemTypesWithError(string path, string fileName, string trustee, string message, StatusEnumeration status)
        {
            ItemType itemType = this.CreateItemTypeForCollect(path, fileName, trustee);
            itemType.message = MessageType.FromErrorString(message);
            itemType.status = status;
            return itemType;
        }

        private ItemType CreateItemTypeForCollect(string path, string fileName, string trustee)
        {
            var newFileEffectiveRightsItem = new fileeffectiverights_item();
            newFileEffectiveRightsItem.path = OvalHelper.CreateItemEntityWithStringValue(path);
            newFileEffectiveRightsItem.filename = OvalHelper.CreateItemEntityWithStringValue(fileName);
            
            var completeFilepath = string.Format(@"{0}\{1}", path, fileName);
            newFileEffectiveRightsItem.filepath = OvalHelper.CreateItemEntityWithStringValue(completeFilepath);

            if (this.SourceObjectType == SourceObjectTypes.FileEffectiveRights)
                newFileEffectiveRightsItem.trustee_name = OvalHelper.CreateItemEntityWithStringValue(trustee);
            else
            {
                newFileEffectiveRightsItem.trustee_sid = OvalHelper.CreateItemEntityWithStringValue(trustee);
                newFileEffectiveRightsItem.trustee_name = OvalHelper.CreateItemEntityWithStringValue(newFileEffectiveRightsItem.trustee_sid.Value);
            }

            return newFileEffectiveRightsItem;
        }
    }
}
