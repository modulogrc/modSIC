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
using System.Text;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Independent.Common.File
{
    public class FileItemTypeFactory
    {

        private FamilyEnumeration Platform;

        public FileItemTypeFactory(FamilyEnumeration platform)
        {
            this.Platform = platform;
        }

        public IEnumerable<ItemType> CreateFileItemTypesByCombinationOfEntitiesFrom(IEnumerable<string> filePaths, IEnumerable<string> paths, IEnumerable<string> fileNames)
        {
            if (filePaths.Count() > 0)
            {
                return this.CreateFileItemTypesByCombinationOfEntitiesWithFilePaths(filePaths);
            }
            else
            {
                return this.CreateFileItemTypesByCombinationOfEntitiesWithoutFilePaths(paths,fileNames);
            }
        }

        public IEnumerable<ItemType> CreateFileItemTypesWithFilePathsByCombinationFrom(IEnumerable<string> paths, IEnumerable<string> filenames)
        {            
            return this.CreateFileItemTypesByCombinationOfEntitiesWithFilePaths(filenames);
        }

        private IEnumerable<string> CombinationFilePath(IEnumerable<string> paths, IEnumerable<string> filenames)
        {
            List<string> filePaths = new List<string>();
            string filepath = "";
            foreach (string path in paths)
            {
                foreach (string filename in filenames)
                {
                    if (path.EndsWith("\\"))
                        filepath = path + filename;
                    else
                        filepath = string.Format("{0}\\{1}", path, filename);

                    filePaths.Add(filepath);
                }
            }
            return filePaths;
        }

        private IEnumerable<ItemType> CreateFileItemTypesByCombinationOfEntitiesWithoutFilePaths(IEnumerable<string> paths, IEnumerable<string> fileNames)
        {
            List<ItemType> itemTypes = new List<ItemType>();
            foreach (string path in paths)
                foreach (string filename in fileNames)
                    itemTypes.Add(this.CreateItemType("", path, filename));
            return itemTypes;
        }

        private IEnumerable<ItemType> CreateFileItemTypesByCombinationOfEntitiesWithFilePaths(IEnumerable<string> filePaths)
        {
            List<ItemType> itemTypes = new List<ItemType>();
            foreach (string filePath in filePaths)
                itemTypes.Add(this.CreateItemType(filePath, "", ""));
            return itemTypes;
        }

        private ItemType CreateItemType(string filePath, string path, string fileName)
        {
            if (this.Platform.Equals(FamilyEnumeration.windows))
            {
                return new OVAL.SystemCharacteristics.file_item()
                {
                    filepath = string.IsNullOrEmpty(filePath) ? null : new EntityItemStringType() { Value = filePath },
                    filename = new EntityItemStringType() { Value = fileName },
                    path = new EntityItemStringType() { Value = path }
                };
            }
            else if (this.Platform.Equals(FamilyEnumeration.unix))
            {
                return new OVAL.SystemCharacteristics.Unix.file_item()
                {
                        filepath = string.IsNullOrEmpty(filePath) ? null : new EntityItemStringType() { Value = filePath },
                        filename = new EntityItemStringType() { Value = fileName },
                        path = new EntityItemStringType() { Value = path }
                 };
            }
            else
                throw new ArgumentException(String.Format("The object type '{0}' is not supported.", this.Platform.ToString()));
        }
    }
}
