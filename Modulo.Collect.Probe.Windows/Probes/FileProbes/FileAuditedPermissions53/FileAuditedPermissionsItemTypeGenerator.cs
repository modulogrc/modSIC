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
using System.IO;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Windows.FileAuditedPermissions53
{
    public class FileAuditedPermissionsItemTypeGenerator : IItemTypeGenerator
    {
        private const string CIMV2 = "root\\cimv2";

        public PathOperatorEvaluator PathOperatorEvaluator { get; set; }

        public IFileProvider WindowsFileProvider { get; set; }

        public virtual IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var itemsToCollect = new List<ItemType>();
            var fileObjects = new FileAuditedPermissionsVariableEvaluator(variables).ProcessVariables(objectType);

            foreach (var fileObject in fileObjects)
            {
                var processedItems = this.ProcessOperation((fileauditedpermissions53_object)fileObject);
                itemsToCollect.AddRange(processedItems);
            }

            return itemsToCollect;
        }


        private IEnumerable<ItemType> ProcessOperation(fileauditedpermissions53_object objectToCollect)
        {
            this.checkPathOperatorInstance();

            IEnumerable<string> processedFilenames = null;
            IEnumerable<string> processedPaths = null;

            if (objectToCollect.IsFilePathDefined())
            {
                var filepath = this.GetDictionaryWithElement(fileauditedpermissions53_objectItemsChoices.filepath, objectToCollect.GetAllObjectEntities());
                var filePathOperationResult = this.PathOperatorEvaluator.ProcessOperationFilePath(filepath);
                processedPaths = filePathOperationResult.Paths;

                processedFilenames = new List<String>();
                foreach (var completeFilepath in filePathOperationResult.FileNames)
                    ((List<String>)processedFilenames).Add(System.IO.Path.GetFileName(completeFilepath));
            }
            else
            {
                var paths = this.GetDictionaryWithElement(fileauditedpermissions53_objectItemsChoices.path, objectToCollect.GetAllObjectEntities());
                processedPaths = this.PathOperatorEvaluator.ProcessOperationsPaths(paths);

                var fileNames = this.GetDictionaryWithElement(fileauditedpermissions53_objectItemsChoices.filename, objectToCollect.GetAllObjectEntities());
                processedFilenames = this.PathOperatorEvaluator.ProcessOperationFileName(fileNames, processedPaths, false);
            }

            var trusteeSID = ((EntitySimpleBaseType)objectToCollect.GetItemValue(fileauditedpermissions53_objectItemsChoices.trustee_sid));
            var xpaths = new string[] { trusteeSID.Value };

            return this.CreateFileItemTypesByCombinationOfEntitiesFrom(processedPaths, processedFilenames, xpaths);
        }

        private void checkPathOperatorInstance()
        {
            if (this.PathOperatorEvaluator == null)
            {
                var newPathOperator = new PathOperatorEvaluator(WindowsFileProvider, FamilyEnumeration.windows);
                this.PathOperatorEvaluator = newPathOperator;
            }
        }

        private fileauditedpermissions_item CreateXmlFileItem(string filepath, string trusteeSID)
        {
            return new fileauditedpermissions_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(filepath),
                trustee_sid = OvalHelper.CreateItemEntityWithStringValue(trusteeSID)
            };
        }

        private Dictionary<string, EntityObjectStringType> GetDictionaryWithElement(
            fileauditedpermissions53_objectItemsChoices entityName,
            Dictionary<string, EntitySimpleBaseType> allEntities)
        {
            var dictionary = new Dictionary<String, EntityObjectStringType>();
            dictionary.Add(entityName.ToString(), (EntityObjectStringType)allEntities[entityName.ToString()]);

            return dictionary;
        }

        private IEnumerable<ItemType> CreateFileItemTypesByCombinationOfEntitiesFrom(
            IEnumerable<string> paths, IEnumerable<string> fileNames, IEnumerable<string> xpaths)
        {
            var itemTypes = new List<ItemType>();
            foreach (string path in paths)
                foreach (string fileName in fileNames)
                    foreach (string xpath in xpaths)
                    {
                        var newItemType = this.CreateXmlFileItem(Path.Combine(path, fileName), xpath);
                        itemTypes.Add(newItemType);
                    }

            return itemTypes;
        }
    }
}
