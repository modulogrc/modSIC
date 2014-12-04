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
using System.Linq;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;

using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Extensions;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.Common;


namespace Modulo.Collect.Probe.Independent.XmlFileContent
{
    public class XmlFileContentItemTypeGenerator : IItemTypeGenerator
    {
        public PathOperatorEvaluator PathOperatorEvaluator { get; set; }

        public XmlFileContentItemTypeGenerator(PathOperatorEvaluator poe)
        {
            this.PathOperatorEvaluator = poe;
        }

        public virtual IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var itemsToCollect = new List<ItemType>();
            var xmlFileObjects = new XmlFileContentVariableEvaluator(variables).ProcessVariables(objectType);

            foreach (var xmlFileContentObject in xmlFileObjects)
            {
                var processedItems = this.ProcessOperation((xmlfilecontent_object)xmlFileContentObject);
                itemsToCollect.AddRange(processedItems);
            }

            return itemsToCollect;
        }

        private bool MustCheckIfFileExists(xmlfilecontent_object xmlFileContentObject)
        {
            var allObjectEntities = xmlFileContentObject.GetAllObjectEntities();
            EntitySimpleBaseType filepathEntity = null;
            allObjectEntities.TryGetValue(xmlfilecontent_ItemsChoices.filepath.ToString(), out filepathEntity);
            if (filepathEntity != null)
                return filepathEntity.var_check.Equals(CheckEnumeration.all);

            EntitySimpleBaseType pathEntity = null;
            EntitySimpleBaseType filenameEntity = null;
            allObjectEntities.TryGetValue(xmlfilecontent_ItemsChoices.path.ToString(), out pathEntity);
            allObjectEntities.TryGetValue(xmlfilecontent_ItemsChoices.filename.ToString(), out filenameEntity);
            return (((pathEntity != null) && string.IsNullOrWhiteSpace(pathEntity.var_ref) && pathEntity.var_check.Equals(CheckEnumeration.all)) ||
                    ((filenameEntity != null) && string.IsNullOrWhiteSpace(filenameEntity.var_ref) && filenameEntity.var_check.Equals(CheckEnumeration.all)));
        }

        private IEnumerable<ItemType> ProcessOperation(xmlfilecontent_object objectToCollect)
        {
            IEnumerable<string> processedFilenames = null;
            IEnumerable<string> processedPaths = null;

            

            if (objectToCollect.IsFilePathDefined())
            {
                var filepath = this.GetDictionaryWithElement(xmlfilecontent_ItemsChoices.filepath, objectToCollect.GetAllObjectEntities());
                var filePathOperationResult = this.PathOperatorEvaluator.ProcessOperationFilePath(filepath);
                processedPaths = filePathOperationResult.Paths;

                processedFilenames = new List<String>();
                foreach (var completeFilepath in filePathOperationResult.FileNames)
                    ((List<String>)processedFilenames).Add(System.IO.Path.GetFileName(completeFilepath));
            }
            else
            {
                var paths = this.GetDictionaryWithElement(xmlfilecontent_ItemsChoices.path, objectToCollect.GetAllObjectEntities());
                processedPaths =this.PathOperatorEvaluator.ProcessOperationsPaths(paths);
                
                var fileNames = this.GetDictionaryWithElement(xmlfilecontent_ItemsChoices.filename, objectToCollect.GetAllObjectEntities());
                processedFilenames = this.PathOperatorEvaluator.ProcessOperationFileName(fileNames, processedPaths, false);
            }

            var xPathEntity = ((EntitySimpleBaseType)objectToCollect.GetItemValue(xmlfilecontent_ItemsChoices.xpath));
            var xpaths = new string[] { xPathEntity.Value };

            if ((processedPaths == null) || (processedPaths.Count() == 0) || 
                (processedFilenames == null) || (processedFilenames.Count() == 0))
            {
                var completeFilepath = objectToCollect.GetCompleteFilepath();
                if (PathOperatorEvaluator.Platform.Equals(FamilyEnumeration.unix))
                    completeFilepath = completeFilepath.ToUnixPath();
                
                var newXmlFileContentItem = CreateXmlFileItem(completeFilepath, xpaths.First());
                newXmlFileContentItem.status = StatusEnumeration.doesnotexist;
                newXmlFileContentItem.filepath.status = newXmlFileContentItem.status;
                
                return new ItemType[] { newXmlFileContentItem };
            }

            var mustCheckIfFileExists = true; // this.MustCheckIfFileExists(objectToCollect);
            return this.CreateFileItemTypesByCombinationOfEntitiesFrom(processedPaths, processedFilenames, xpaths, mustCheckIfFileExists);
        }

        private object getChildFilesFromFilepath(string filepath)
        {
            throw new NotImplementedException();
        }

        private xmlfilecontent_item CreateXmlFileItem(string filepath, string xpath)
        {
            return new xmlfilecontent_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(filepath),
                xpath = OvalHelper.CreateItemEntityWithStringValue(xpath)
            };
        }

        private Dictionary<string, EntityObjectStringType> GetDictionaryWithElement(
            xmlfilecontent_ItemsChoices entityName, Dictionary<string, EntitySimpleBaseType> allEntities)
        {
            var dictionary = new Dictionary<String, EntityObjectStringType>();
            dictionary.Add(entityName.ToString(), (EntityObjectStringType)allEntities[entityName.ToString()]);

            return dictionary;
        }

        private IEnumerable<ItemType> CreateFileItemTypesByCombinationOfEntitiesFrom(
            IEnumerable<string> paths, IEnumerable<string> fileNames, IEnumerable<string> xpaths, bool mustCheckIfFileExists)
        {
            var itemTypes = new List<ItemType>();
            foreach (string path in paths.Distinct())
                foreach (string fileName in fileNames.Distinct())
                    foreach (string xpath in xpaths)
                    {
                        var completeFilepath = Path.Combine(path, fileName);
                        if (PathOperatorEvaluator.Platform.Equals(FamilyEnumeration.unix))
                            completeFilepath = completeFilepath.ToUnixPath();

                        var varChecked = true;
                        if (mustCheckIfFileExists)
                            varChecked = FileExists(completeFilepath);
                                 
                        if (varChecked)
                        {
                            var newItemType = this.CreateXmlFileItem(completeFilepath, xpath);
                            itemTypes.Add(newItemType);
                        }
                    }

            return itemTypes;
        }

        private bool FileExists(string completeFilepath)
        {
            return 
                this.PathOperatorEvaluator
                    .FileProvider
                        .FileExists(completeFilepath);
        }
    }
}
