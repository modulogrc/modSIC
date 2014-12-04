using System;
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Independent.Common.Operators;


namespace Modulo.Collect.Probe.Independent.Common.File
{
    public class FileEntityOperationEvaluator
    {

        public IFileProvider FileProvider;
        private PathOperatorEvaluator pathOperatorEvaluator;
        private OperatorHelper operatorHelper = new OperatorHelper();

        public virtual IEnumerable<ItemType> ProcessOperation(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            List<ItemType> itemTypes = new List<ItemType>();
            foreach (var objectType in objectTypes)
            {
                var possibleFileItems = this.ProcessOperation(objectType);
                foreach (var possibleItem in possibleFileItems)
                {
                    var filepathValue = GetCompleteFilepath(possibleItem);
                    if (!FileProvider.FileExists(filepathValue))
                        possibleItem.status = StatusEnumeration.doesnotexist;
                    
                    itemTypes.Add(possibleItem);
                }
            }

            if (itemTypes.Count > 1)
                return itemTypes.Where(item => item.status != StatusEnumeration.doesnotexist);

            return itemTypes;
        }

        private bool IsFilePathSet(OVAL.Definitions.ObjectType objectType)
        {
            if (objectType is OVAL.Definitions.Windows.file_object)
                return ((OVAL.Definitions.Windows.file_object)objectType).IsFilePathSet();
            else if (objectType is OVAL.Definitions.Unix.file_object)
                return ((OVAL.Definitions.Unix.file_object)objectType).IsFilePathSet();
            else
                throw new ArgumentException(String.Format("The object type '{0}' is not supported.", objectType.GetType().ToString()));
        }


        private String GetCompleteFilepath(ItemType itemType)
        {
            if (itemType is OVAL.SystemCharacteristics.file_item)
                return ((OVAL.SystemCharacteristics.file_item)itemType).GetFullFilepath();
            if (itemType is OVAL.SystemCharacteristics.Unix.file_item)
                return ((OVAL.SystemCharacteristics.Unix.file_item)itemType).GetFullFilepath();
            else
                throw new ArgumentException(String.Format("The object type '{0}' is not supported.", itemType.GetType().ToString()));
        }

        public FileEntityOperationEvaluator(IFileProvider fileProvider, FamilyEnumeration platform)
        {
            this.FileProvider = fileProvider;
            this.pathOperatorEvaluator = new PathOperatorEvaluator(FileProvider, platform);
        }

        public IEnumerable<ItemType> ProcessOperation(OVAL.Definitions.ObjectType objectType)
        {
            var itemTypes = new List<ItemType>();
            var fileEntities = OvalHelper.GetFileEntitiesFromObjectType(objectType);

            if (IsFilePathSet(objectType))
            {
                itemTypes.AddRange(this.ProcessOperationFilePath(objectType));
                return itemTypes;
            }
            else
            {
                var paths = this.ProcessOperationsPaths(fileEntities);
                var fileNames = this.ProcessOperationFileName(fileEntities, paths, false);
                var family = GetFamilyEnumerationForObjectType(objectType);
                return new FileItemTypeFactory(family)
                    .CreateFileItemTypesByCombinationOfEntitiesFrom(new List<string>(), paths, fileNames);
            }
        }

        private FamilyEnumeration GetFamilyEnumerationForObjectType(OVAL.Definitions.ObjectType objectType)
        {
            return objectType is OVAL.Definitions.Windows.file_object ? FamilyEnumeration.windows : FamilyEnumeration.unix;
        }

        private IEnumerable<string> ProcessOperationsPaths(Dictionary<string, EntityObjectStringType> fileEntities)
        {
            return pathOperatorEvaluator.ProcessOperationsPaths(fileEntities);
        }

        private IEnumerable<string> ProcessOperationFileName(Dictionary<string, EntityObjectStringType> fileEntities, IEnumerable<string> paths, bool concatenatedPath)
        {
            return pathOperatorEvaluator.ProcessOperationFileName(fileEntities, paths, concatenatedPath);
        }

        private string GetUnixPath(string completeFilepath)
        {
            var pathParts = completeFilepath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                var path = pathParts[i];
                if (!path.StartsWith("/"))
                    path = path.Insert(0, "/");
                
                stringBuilder.Append(path);
            }

            return stringBuilder.ToString();
        }

        private string GetUnixFilename(string completeFilepath)
        {
            var pathParts = completeFilepath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return pathParts.Last();
        }

        private IEnumerable<ItemType> ProcessOperationFilePath(OVAL.Definitions.ObjectType objectType)
        {
            var itemTypes = new List<ItemType>();

            EntityObjectStringType filePath = null;
            String path = null;
            String fileName = null;

            if (objectType is OVAL.Definitions.Windows.file_object)
            {
                filePath = (EntityObjectStringType)((OVAL.Definitions.Windows.file_object)objectType).GetItemValue(file_object_ItemsChoices.filepath);
                path = Path.GetDirectoryName(filePath.Value);
                fileName = Path.GetFileName(filePath.Value);
            }
            else if (objectType is OVAL.Definitions.Unix.file_object)
            {
                filePath = (EntityObjectStringType)((OVAL.Definitions.Unix.file_object)objectType).GetItemValue(OVAL.Definitions.Unix.ItemsChoiceType3.filepath);
                path = GetUnixPath(filePath.Value);
                fileName = GetUnixFilename(filePath.Value);
            }

            if (filePath == null || filePath.Value == null)
                return itemTypes;
            
            var filePathInformation = new FilePathRegexInformation(path);
            var operationForPath = filePath.operation;
            if (this.operatorHelper.IsRegularExpression(operationForPath))
            {
                if (filePathInformation.GetPathWithFirstRegex() == null)
                    operationForPath = OperationEnumeration.equals;
            }
            else if (this.operatorHelper.IsNotEqualsOperation(operationForPath))
                operationForPath = OperationEnumeration.equals;
            
            var pathEntity = new EntityObjectStringType() { Value = path, operation = operationForPath };
            var fileNameEntity = new EntityObjectStringType() { Value = fileName, operation = filePath.operation };
            var fileEntities = new Dictionary<string, EntityObjectStringType>();
            fileEntities.Add("path", pathEntity);
            fileEntities.Add("filename", fileNameEntity);
            var paths = this.ProcessOperationsPaths(fileEntities);
            var fileNames = this.ProcessOperationFileName(fileEntities, paths, true);

            var family = GetFamilyEnumerationForObjectType(objectType);
            return new FileItemTypeFactory(family).CreateFileItemTypesWithFilePathsByCombinationFrom(paths, fileNames);
        }

        private IEnumerable<string> EvaluateOperation(string value, IEnumerable<string> valuesToMatch, OperationEnumeration operation)
        {
            IOvalComparator comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            List<string> values = new List<string>();
            foreach (string valueToMatch in valuesToMatch)
            {
                if (comparator.Compare(value, valueToMatch, operation))
                    values.Add(value);
            }

            return values;
        }

        private IEnumerable<string> SubstitutionValueInPattern(List<string> values, PathLevelWithRegex basePath, FilePathRegexInformation regexInformation)
        {
            List<string> paths = new List<string>();
            foreach (string path in values)
            {
                string concatenatedString = regexInformation.ConcatPathWithNextLeveRegex(path, basePath.Level);
                if (!string.IsNullOrEmpty(concatenatedString))
                    paths.Add(concatenatedString);
            }
            return paths;
        }
    }
}
