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

namespace Modulo.Collect.Probe.Independent.TextFileContent54
{
    public class TextFileContentObjectFactory
    {
        public IEnumerable<ObjectType> CreateObjectTypeByCombinationOfEntities(
            textfilecontent54_object fileContentObject,
            IEnumerable<string> filePaths,
            IEnumerable<string> fileName,
            IEnumerable<string> paths,
            IEnumerable<string> patterns,
            IEnumerable<string> instances)
        {
            var fileContentObjects = new List<ObjectType>();
            fileContentObjects.AddRange(this.CreateFileObjects(fileContentObject, filePaths, fileName, paths, patterns, instances));
            return fileContentObjects;
        }

        public IEnumerable<ObjectType> CreateFileObjects(
            textfilecontent54_object fileContentObject,
            IEnumerable<string> filePaths,
            IEnumerable<string> fileNames,
            IEnumerable<string> paths,
            IEnumerable<string> patterns,
            IEnumerable<string> instances)
        {
            var fileContentObjects = new List<ObjectType>();

            if (fileContentObject.IsFilePathDefined())
                this.CreateFileObjectsWithFilePath(fileContentObject, filePaths, patterns, instances, fileContentObjects);
            else
                this.CreateFileObjectsWithoutFilePath(fileContentObject, fileNames, paths, patterns, instances, fileContentObjects);

            return fileContentObjects;
        }

        private void CreateFileObjectsWithFilePath(textfilecontent54_object fileContentObject, IEnumerable<string> filePaths, IEnumerable<string> patterns, IEnumerable<string> instances, List<ObjectType> fileContentObjects)
        {
            foreach (string filepath in filePaths)
                foreach (string pattern in patterns)
                    foreach (string instance in instances)
                        fileContentObjects.Add(this.CreateObjectTypeFrom(fileContentObject, filepath, null, null, pattern, instance));
        }

        private void CreateFileObjectsWithoutFilePath(textfilecontent54_object fileContentObject, IEnumerable<string> fileNamesOrFilePaths, IEnumerable<string> paths, IEnumerable<string> patterns, IEnumerable<string> instances, List<ObjectType> fileContentObjects)
        {
            foreach (string path in paths)
                foreach (string fileName in fileNamesOrFilePaths)
                    foreach (string pattern in patterns)
                        foreach (string instance in instances)
                            fileContentObjects.Add(this.CreateObjectTypeFrom(fileContentObject, null, fileName, path, pattern, instance));
        }

        private ObjectType CreateObjectTypeFrom(textfilecontent54_object fileContentObject, string filePath, string fileName, string path, string pattern, string instance)
        {
            EntityObjectStringType filePathFrom = null;
            EntityObjectStringType pathFrom = null;
            EntityObjectStringType fileNameFrom = null;
            EntityObjectStringType newFilePath = null;
            EntityObjectStringType newPath = null;
            EntityObjectStringType newFileName = null;

            var patternFrom = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent54_ItemsChoices.pattern);
            var instanceFrom = (EntityObjectIntType)fileContentObject.GetItemValue(textfilecontent54_ItemsChoices.instance);

            var newPattern = this.CopyEntityObjectStringType(patternFrom);
            newPattern.Value = !string.IsNullOrEmpty(pattern) ? pattern : newPattern.Value;
            
            var newInstance = this.CopyEntityObjectIntType(instanceFrom);
            newInstance.Value = !string.IsNullOrEmpty(instance) ? instance : newInstance.Value;


            var behaviors = fileContentObject.Items.OfType<Textfilecontent54Behaviors>().ToArray();
            if (fileContentObject.IsFilePathDefined())
            {
                filePathFrom = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent54_ItemsChoices.filepath);
                newFilePath = this.CopyEntityObjectStringType(filePathFrom);
                newFilePath.Value = !string.IsNullOrEmpty(filePath) ? filePath : newFilePath.Value;

                return this.CreateTextFileContentObject(newFilePath, null, null, newPattern, newInstance, behaviors);
            }
            else
            {
                pathFrom = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent54_ItemsChoices.path);
                fileNameFrom = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent54_ItemsChoices.filename);
                
                newPath = this.CopyEntityObjectStringType(pathFrom);
                newPath.Value = !string.IsNullOrEmpty(path) ? path : newPath.Value;
                
                newFileName = this.CopyEntityObjectStringType(fileNameFrom);
                newFileName.Value = !string.IsNullOrEmpty(fileName) ? fileName : newFileName.Value;

                return this.CreateTextFileContentObject(null, newFileName, newPath, newPattern, newInstance, behaviors);
            }

        }

        private EntityObjectStringType CopyEntityObjectStringType(EntityObjectStringType sourceEntityObject)
        {
            return new EntityObjectStringType()
            {
                datatype = sourceEntityObject.datatype,
                mask = sourceEntityObject.mask,
                operation = sourceEntityObject.operation,
                var_ref = sourceEntityObject.var_ref,
                Value = sourceEntityObject.Value
            };
            
        }

        private EntityObjectIntType CopyEntityObjectIntType(EntityObjectIntType sourceEntityObject)
        {
            return new EntityObjectIntType()
            {
                datatype = sourceEntityObject.datatype,
                mask = sourceEntityObject.mask,
                operation = sourceEntityObject.operation,
                var_ref = sourceEntityObject.var_ref,
                Value = sourceEntityObject.Value
            };
        }

        private textfilecontent54_object CreateTextFileContentObject(
            EntityObjectStringType filePath,
            EntityObjectStringType fileName,
            EntityObjectStringType path,
            EntityObjectStringType pattern,
            EntityObjectIntType instance,
            Object[] behaviors)
        {
            var fileContentObject = new textfilecontent54_object();
            object[] items;
            textfilecontent54_ItemsChoices[] itemChoices;
            var hasBehaviors = (behaviors != null) && (behaviors.Count() > 0);
            var behaviorCount = behaviors.Count();

            if (filePath == null)
            {
                if (hasBehaviors)
                {
                    var entityCount = behaviorCount + 4;
                    items = new object[entityCount];
                    itemChoices = new textfilecontent54_ItemsChoices[entityCount];
                    

                    for (int i = 0; i < behaviorCount; i++)
                    {
                        itemChoices[i] = textfilecontent54_ItemsChoices.behaviors;
                        items[i] = behaviors.ElementAt(i);
                    }

                    itemChoices[behaviorCount] = textfilecontent54_ItemsChoices.path;
                    itemChoices[behaviorCount + 1] = textfilecontent54_ItemsChoices.filename;
                    itemChoices[behaviorCount + 2] = textfilecontent54_ItemsChoices.pattern;
                    itemChoices[behaviorCount + 3] = textfilecontent54_ItemsChoices.instance;

                    items[behaviorCount] = path;
                    items[behaviorCount + 1] = fileName;
                    items[behaviorCount + 2] = pattern;
                    items[behaviorCount + 3] = instance;
                }
                else
                {
                    items = new EntitySimpleBaseType[4];
                    itemChoices = new textfilecontent54_ItemsChoices[4];
                    itemChoices[0] = textfilecontent54_ItemsChoices.path;
                    itemChoices[1] = textfilecontent54_ItemsChoices.filename;
                    itemChoices[2] = textfilecontent54_ItemsChoices.pattern;
                    itemChoices[3] = textfilecontent54_ItemsChoices.instance;
                    items[0] = path;
                    items[1] = fileName;
                    items[2] = pattern;
                    items[3] = instance;
                }
            }
            else
            {
                if (hasBehaviors)
                {
                    var entityCount = behaviorCount + 3;
                    items = new object[entityCount];
                    itemChoices = new textfilecontent54_ItemsChoices[entityCount];


                    for (int i = 0; i < behaviorCount; i++)
                    {
                        itemChoices[i] = textfilecontent54_ItemsChoices.behaviors;
                        items[i] = behaviors.ElementAt(i);
                    }

                    itemChoices[behaviorCount] = textfilecontent54_ItemsChoices.filepath;
                    itemChoices[behaviorCount + 1] = textfilecontent54_ItemsChoices.pattern;
                    itemChoices[behaviorCount + 2] = textfilecontent54_ItemsChoices.instance;

                    items[behaviorCount] = filePath;
                    items[behaviorCount + 1] = pattern;
                    items[behaviorCount + 2] = instance;
                }
                else
                {
                    items = new EntitySimpleBaseType[3];
                    itemChoices = new textfilecontent54_ItemsChoices[3];
                    itemChoices[0] = textfilecontent54_ItemsChoices.filepath;
                    itemChoices[1] = textfilecontent54_ItemsChoices.pattern;
                    itemChoices[2] = textfilecontent54_ItemsChoices.instance;
                    items[0] = filePath;
                    items[1] = pattern;
                    items[2] = instance;
                }

            }

            fileContentObject.Items = items;
            fileContentObject.Textfilecontent54ItemsElementName = itemChoices;
            
            return fileContentObject;
        }

    }
}
