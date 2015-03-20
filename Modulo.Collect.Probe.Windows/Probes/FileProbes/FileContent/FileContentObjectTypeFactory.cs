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
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Windows.FileContent
{
    public class FileContentObjectTypeFactory
    {

        public IEnumerable<ObjectType> CreateObjectTypeByCombinationOfEntities(textfilecontent_object fileContentObject, IEnumerable<string> fileNames, IEnumerable<string> lines, IEnumerable<string> paths)
        {
            List<ObjectType> fileContentObjects = new List<ObjectType>();
            fileContentObjects.AddRange(this.CreateFileObjects(fileContentObject, fileNames, lines, paths));
            return fileContentObjects;
        }

        public IEnumerable<ObjectType> CreateFileObjects(textfilecontent_object fileContentObject, IEnumerable<string> fileNames, IEnumerable<string> lines, IEnumerable<string> paths)
        {
            List<ObjectType> fileContentObjects = new List<ObjectType>();
            foreach (string fileName in fileNames)            
                foreach (string line in lines)
                    foreach (string path in paths)
                        fileContentObjects.Add(this.CreateObjectTypeFrom(fileContentObject,fileName,line,path));                                    
            
            return fileContentObjects;
        }

        private ObjectType CreateObjectTypeFrom(textfilecontent_object fileContentObject, string fileName, string line, string path)
        {
            EntityObjectStringType pathFrom = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.path);
            EntityObjectStringType fileNameFrom = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.filename);
            EntityObjectStringType lineFrom = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.line);        
    
            EntityObjectStringType newPath = this.CreateObjectStringTypeFrom(pathFrom);
            newPath.Value = !string.IsNullOrEmpty(path) ? path : newPath.Value;
            EntityObjectStringType newFileName = this.CreateObjectStringTypeFrom(fileNameFrom);
            newFileName.Value = !string.IsNullOrEmpty(fileName) ? fileName : newFileName.Value;
            EntityObjectStringType newLine = this.CreateObjectStringTypeFrom(lineFrom);
            newLine.Value = !string.IsNullOrEmpty(line) ? line : newLine.Value;
            
            return this.CreateFileObject(newFileName, newLine, newPath);
        }

        private EntityObjectStringType CreateObjectStringTypeFrom(EntityObjectStringType objectStringType)
        {
            EntityObjectStringType newObjectStringType = new EntityObjectStringType();
            newObjectStringType.datatype = objectStringType.datatype;
            newObjectStringType.mask = objectStringType.mask;
            newObjectStringType.operation = objectStringType.operation;
            newObjectStringType.var_ref = objectStringType.var_ref;
            newObjectStringType.Value = objectStringType.Value;
            return newObjectStringType;
        }

        private textfilecontent_object CreateFileObject(EntityObjectStringType fileName, EntityObjectStringType line, EntityObjectStringType path)
        {
            textfilecontent_object fileContentObject = new textfilecontent_object();

            var items = new List<object>();
            var itemChoices = new List<textfilecontent_ItemsChoices>();
            items.Add(fileName);
            items.Add(line);
            items.Add(path);
            itemChoices.Add(textfilecontent_ItemsChoices.filename);
            itemChoices.Add(textfilecontent_ItemsChoices.line);
            itemChoices.Add(textfilecontent_ItemsChoices.path);
            
            fileContentObject.Items = items.ToArray();
            fileContentObject.TextfilecontentItemsElementName = itemChoices.ToArray();
            return fileContentObject;
        }
    }
}

