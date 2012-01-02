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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.Probe.Independent.Common.File;


namespace Modulo.Collect.Probe.Independent.TextFileContent54
{
    public class TextFileContentEntityOperationEvaluator
    {
        public PathOperatorEvaluator PathOperatorEvaluator { get; set; }

        private BaseObjectCollector TextFileContentDataSource;

        private const string ERROR_MESSAGE = "[TextFileContentEntityOperationEvaluator]: An unexpected error occurred while trying to apply operation: {0}";


        public TextFileContentEntityOperationEvaluator(BaseObjectCollector systemDataSource, IFileProvider fileDataSource, FamilyEnumeration platform)
        {
            this.TextFileContentDataSource = systemDataSource;
            this.PathOperatorEvaluator = new PathOperatorEvaluator(fileDataSource, platform);
        }

        public IEnumerable<ItemType> ProcessOperation(IEnumerable<Definitions.ObjectType> fileContentObjects)
        {
            var result = new List<ItemType>();
            foreach (Definitions.ObjectType fileObject in fileContentObjects)
            {
                var derivedFileObjects = ProcessOperation((textfilecontent54_object)fileObject);
                result.AddRange(derivedFileObjects);
            }
            
            return result;
        }

        public IEnumerable<ItemType> ProcessOperation(textfilecontent54_object fileContentObject)
        {
            IEnumerable<string> fileNames = null;
            IEnumerable<string> paths = null;

            var fileEntities = FileContentOvalHelper.GetFileContentEntitiesFromObjectType(fileContentObject);            
            var pattern = ((EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent54_ItemsChoices.pattern)).Value;
            
            if (fileContentObject.IsFilePathDefined())
            {
                var filePath = this.GetDictionaryWithElement(textfilecontent54_ItemsChoices.filepath,fileEntities);
                var filePathOperationResult = this.PathOperatorEvaluator.ProcessOperationFilePath(filePath);
                fileNames = filePathOperationResult.FileNames;
                paths = filePathOperationResult.Paths;
            }
            else
            {
                paths = this.ProcessOperationsPaths(fileEntities);    
                fileNames = this.ProcessOperationsFileNames(fileEntities, paths);                                          
            }
            
            var itemTypes = this.ProcessOperationsPatterns(fileContentObject, fileNames);
            
            return this.ProcessOperationsInstance(itemTypes,fileContentObject);
        }

        private IEnumerable<ItemType> ProcessOperationsInstance(IEnumerable<ItemType> itemTypes, textfilecontent54_object textFileContent)
        {
            var result = new List<ItemType>();
            
            var instanceEntity = 
                (EntityObjectIntType)textFileContent.GetItemValue(textfilecontent54_ItemsChoices.instance);
            
            var comparator = new OvalComparatorFactory().GetComparator(instanceEntity.datatype);
            
            foreach (var itemType in itemTypes)
            {
                if (itemType.status == StatusEnumeration.exists)
                {
                    var textFileContentItem = (textfilecontent_item)itemType;
                    if (comparator.Compare(textFileContentItem.instance.Value, instanceEntity.Value, instanceEntity.operation))
                        result.Add(itemType);
                }
                else
                {
                    result.Add(itemType);
                }
            }

            return result;            
        }    

        private IList<ItemType> ProcessOperationsPatterns(
            textfilecontent54_object fileContentObject, IEnumerable<string> completePaths)
        {
            var result = new List<ItemType>();
            var pattern = ((EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent54_ItemsChoices.pattern)).Value;
            var instance = ((EntitySimpleBaseType)fileContentObject.GetItemValue(textfilecontent54_ItemsChoices.instance)).Value;
            var multilineBehavior = fileContentObject.IsMultiline();
            
            foreach (var filepath in completePaths)
            {
                var parameters =
                    TextFileContentObjectCollector.GetDictionaryWithParametersToSearchTextFileConten(
                        filepath, pattern, Int32.Parse(instance), multilineBehavior);

                result.Add(this.CollectItem(parameters));
                
                parameters.Clear();
            }

            return result;
        }

        private ItemType CollectItem(Dictionary<string, object> collectParameters)
        {
            var filepath = collectParameters[SearchTextFileContentParameters.filepath.ToString()].ToString();
            var instance = (int)collectParameters[SearchTextFileContentParameters.instance.ToString()];
            var pattern = collectParameters[SearchTextFileContentParameters.pattern.ToString()].ToString();

            try
            {
                var fileMatchLine = this.TextFileContentDataSource.GetValues(collectParameters).ToList();
                if (fileMatchLine.Count <= 0)
                    return CreateTextFileContentItemType(filepath, pattern, instance, null);
                
                return CreateTextFileContentItemType(filepath, pattern, instance, fileMatchLine.Single());
            }
            //catch (FileNotFoundException)
            //{
            //    var collectedItem = (textfilecontent_item)CreateTextFileContentItemType(filepath, string.Empty, 0, string.Empty);
            //    collectedItem.status = StatusEnumeration.doesnotexist;
            //    CleanTextFileContentItem(collectedItem);
            //    return collectedItem;
            //}
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    var collectedItem = (textfilecontent_item)CreateTextFileContentItemType(filepath, string.Empty, 0, string.Empty);
                    collectedItem.status = StatusEnumeration.doesnotexist;
                    CleanTextFileContentItem(collectedItem);
                    return collectedItem;
                }
                else
                {
                    var errorMessage = string.Format(ERROR_MESSAGE, ex.Message);
                    return new textfilecontent_item() { status = StatusEnumeration.error, message = MessageType.FromErrorString(errorMessage) };
                }
            }
        }

        private void CleanTextFileContentItem(textfilecontent_item itemToClean)
        {
            itemToClean.pattern = null;
            itemToClean.instance = null;
            itemToClean.filename.status = StatusEnumeration.doesnotexist;
            itemToClean.filepath.status = StatusEnumeration.doesnotexist;
            itemToClean.path.status = StatusEnumeration.doesnotexist;
        }
       
        private IEnumerable<string> ProcessOperationsPaths(Dictionary<string, EntitySimpleBaseType> fileEntities)
        {
            var paths = this.GetDictionaryWithElement(textfilecontent54_ItemsChoices.path, fileEntities);            
            return PathOperatorEvaluator.ProcessOperationsPaths(paths);
        }

        private IEnumerable<string> ProcessOperationsFileNames(Dictionary<string, EntitySimpleBaseType> fileEntities, IEnumerable<string> paths)
        {
            var fileNames = this.GetDictionaryWithElement(textfilecontent54_ItemsChoices.filename, fileEntities);            
            return PathOperatorEvaluator.ProcessOperationFileName(fileNames, paths, true);
        }

        private Dictionary<string, EntityObjectStringType> GetDictionaryWithElement(textfilecontent54_ItemsChoices item, Dictionary<string, EntitySimpleBaseType> entities)
        {
            var newEntities = new Dictionary<string, EntityObjectStringType>();
            
            var itemChoice = (EntityObjectStringType)entities[item.ToString()];
            newEntities.Add(item.ToString(), itemChoice);
            
            return newEntities;
        }

        private ItemType CreateTextFileContentItemType(string filepath, string pattern, int instance, string text)
        {
            var status = StatusEnumeration.exists;
            EntityItemAnySimpleType textEntity = null;

            if (string.IsNullOrEmpty(text))
                status = StatusEnumeration.doesnotexist;
            else
                textEntity = OvalHelper.CreateEntityItemAnyTypeWithValue(text);

            return new textfilecontent_item()
            {
                status = status,
                pattern = OvalHelper.CreateItemEntityWithStringValue(pattern),
                instance = OvalHelper.CreateItemEntityWithIntegerValue(instance.ToString()),
                filename = OvalHelper.CreateItemEntityWithStringValue(Path.GetFileName(filepath)),
                path = OvalHelper.CreateItemEntityWithStringValue(Path.GetDirectoryName(filepath)),
                filepath = OvalHelper.CreateItemEntityWithStringValue(filepath),
                text = textEntity
            };
        }
       
    }
}
