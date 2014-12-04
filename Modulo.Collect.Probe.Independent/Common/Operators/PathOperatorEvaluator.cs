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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Extensions;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common.Operators;

namespace Modulo.Collect.Probe.Independent.Common
{
    public class PathOperatorEvaluator
    {
        public IFileProvider FileProvider { get; set; }
        private OperatorHelper operatorHelper = new OperatorHelper();
        public FamilyEnumeration Platform { get; private set; }

        public PathOperatorEvaluator(IFileProvider fileProvider, FamilyEnumeration platform)
        {
            this.FileProvider = fileProvider;
            this.Platform = platform;
        }

        public virtual IEnumerable<string> ProcessOperationsPaths(Dictionary<string, EntityObjectStringType> entities)
        {
            var values = new List<string>();
            
            if (this.IsThereFilePathEntity(entities))
                return values;
            
            EntityObjectStringType path;
            if (!entities.TryGetValue("path", out path))
                return values;
            
            var operation = path.operation;
            if (operatorHelper.IsEqualsOperation(operation))
                values.Add(path.Value);
            else if (operatorHelper.IsRegularExpression(operation))
                values.AddRange(EvaluateOperationPatternMatchOfPath(new List<string>() { path.Value }));
            else if (operatorHelper.IsNotEqualsOperation(operation))
                throw new ArgumentException("[PathOperatorEvaluator] - Operation 'not equal' is not supported in path.");
            
            return values;
        }

        public virtual IEnumerable<string> ProcessOperationFileName(Dictionary<string, EntityObjectStringType> fileEntities, IEnumerable<string> paths, bool concatenatedPath)
        {
            List<string> values = new List<string>();
            if (this.IsThereFilePathEntity(fileEntities))
                return new List<String>();

            EntityObjectStringType fileName;
            var isThereFilenameInDictionary = fileEntities.TryGetValue("filename", out fileName);
            if ((!isThereFilenameInDictionary) || (fileName == null))
                return new String[] { "" }.ToList();

            OperationEnumeration operation = fileName.operation;
            //string completePath = "";
            if (operatorHelper.IsEqualsOperation(operation))
            {
                foreach (string path in paths)
                {
                    if (concatenatedPath)
                    {
                        var completePath = Path.Combine(path, fileName.Value);
                        if (this.Platform.Equals(FamilyEnumeration.unix))
                            completePath = completePath.ToUnixPath();
                        
                        values.Add(completePath);
                    }
                    else
                    {
                        if (values.IndexOf(fileName.Value) < 0)
                            values.Add(fileName.Value);
                    }
                }
            }
            else if (operatorHelper.IsRegularExpression(operation))
            {
                foreach (string path in paths)
                {
                    var completePath = path.Trim().EndsWith("\\") ? path.Trim() + fileName.Value : path.Trim() + "\\" + fileName.Value;
                    if (this.Platform.Equals(FamilyEnumeration.unix))
                        completePath = completePath.ToUnixPath();

                    List<string> valuesMatch = this.EvaluateOperationPatternMatchOfFileName(completePath).ToList();
                    foreach (string valueMatch in valuesMatch)
                    {
                        string value = valueMatch;
                        if (concatenatedPath)
                            value = Path.Combine(path, value);
                        
                        if (! (values.IndexOf(value) >= 0))
                        {
                            if (Platform.Equals(FamilyEnumeration.unix))
                                value = value.ToUnixPath();
                            
                            values.Add(value);
                        }
                    }
                }
            }
            else if (operatorHelper.IsNotEqualsOperation(operation))
            {
                foreach (string path in paths)
                {
                    values.AddRange(this.EvaluateOperationNotEqualsOfFileName(path, fileName.Value,concatenatedPath));
                }
            }

            return values;
        }

        public virtual FilePathOperationResult ProcessOperationFilePath(Dictionary<string, EntityObjectStringType> fileEntities)
        {
            FilePathOperationResult filePathOperationResult = new FilePathOperationResult();
            if (!this.IsThereFilePathEntity(fileEntities))
                return filePathOperationResult;

            EntityObjectStringType filePath;
            if (!fileEntities.TryGetValue("filepath", out filePath))
                return filePathOperationResult;

            string path = Path.GetDirectoryName(filePath.Value);
            string fileName = Path.GetFileName(filePath.Value);
            FilePathRegexInformation filePathInformation = new FilePathRegexInformation(path);
            OperationEnumeration operationForPath = filePath.operation;
            OperationEnumeration operationForFileName = operationForPath;
            if (this.operatorHelper.IsRegularExpression(operationForPath))
            {
                PathLevelWithRegex pathWithRegex = filePathInformation.GetPathWithFirstRegex();
                if (pathWithRegex == null)
                    operationForPath = OperationEnumeration.equals;
                
                PathLevelWithRegex filenameWithRegex = new FilePathRegexInformation(fileName.Replace(".", "")).GetPathWithFirstRegex();
                if (filenameWithRegex == null)
                    operationForFileName = OperationEnumeration.equals;

            }
            else if (this.operatorHelper.IsNotEqualsOperation(operationForPath))
            {
                operationForPath = OperationEnumeration.equals;
                operationForFileName = operationForPath;
            }

                
                //RegexHelper.IsPathLevelARegexPattern(fileName) ? OperationEnumeration.patternmatch : OperationEnumeration.equals;
            Dictionary<string, EntityObjectStringType> pathAndFileNameEntities = new Dictionary<string, EntityObjectStringType>();
            EntityObjectStringType pathEntity = new EntityObjectStringType() { Value = path, operation = operationForPath };
            EntityObjectStringType fileNameEntity = new EntityObjectStringType() { Value = fileName, operation = operationForFileName };
            pathAndFileNameEntities.Add("path", pathEntity);
            pathAndFileNameEntities.Add("filename", fileNameEntity);
            IEnumerable<string> paths = this.ProcessOperationsPaths(pathAndFileNameEntities);
            IEnumerable<string> fileNames = this.ProcessOperationFileName(pathAndFileNameEntities, paths, true);

            filePathOperationResult.FileNames.AddRange(fileNames);
            filePathOperationResult.Paths.AddRange(paths);

            return filePathOperationResult;
        }

        private bool IsThereFilePathEntity(Dictionary<string, EntityObjectStringType> entities)
        {
            EntityObjectStringType filePath;
            if ((!entities.TryGetValue("filepath", out filePath)) || filePath == null)
                return false;
            return true;

        }

        private List<string> EvaluateOperationNotEqualsOfFileName(string path, string fileName,bool concatenatedPath)
        {
            List<string> values = new List<string>();
            string[] result;
            result = this.searchFileChildren(path, true);
            result = this.removeDirectoriesFromList(result);

            IOvalComparator comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            foreach (string valueToMatch in result)
            {
                string fileNameToMatch = Path.GetFileName(valueToMatch);
                if (comparator.Compare(fileName, fileNameToMatch, OperationEnumeration.notequal))
                {
                    if (concatenatedPath)
                        values.Add(valueToMatch);
                    else
                        values.Add(fileNameToMatch);
                }
            }

            return values;
        }

        private string[] removeDirectoriesFromList(string[] paths)
        {
            List<string> pathsWithoutDirectories = new List<string>();
            foreach (string path in paths)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (!string.IsNullOrEmpty(fileName))
                    pathsWithoutDirectories.Add(path);
            }
            return pathsWithoutDirectories.ToArray();
        }

        private List<string> EvaluateOperationPatternMatchOfFileName(string path)
        {
            List<string> values = new List<string>();
            string[] result;
            FilePathRegexInformation regexInformation = new FilePathRegexInformation(path);
            PathLevelWithRegex pathWithRegex = regexInformation.GetPathWithFirstRegex();
            result = this.searchFileChildren(path, true);
            result = this.removeDirectoriesFromList(result);
            foreach (string pathForMatch in result)
            {
                string fileName = Path.GetFileName(pathForMatch);
                values.AddRange(this.EvaluateOperation(fileName, new List<string>() { pathWithRegex.Pattern }, OperationEnumeration.patternmatch));
            }
            return values;
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

        private List<string> EvaluateOperationPatternMatchOfPath(IEnumerable<string> paths)
        {
            var values = new List<string>();
            foreach (string path in paths)
            {
                var regexInformation = new FilePathRegexInformation(path);
                var basePath = regexInformation.GetPathWithFirstRegex();
                if (basePath != null)
                {
                    var result = this.searchFileChildren(path, true);
                    if (regexInformation.IsWindowsFilepath())
                        result = this.removeFilesFromList(result);

                    var multiLevelPatternMatchOperator = new MultiLevelPatternMatchOperation(this.Platform);
                    var valuesWithMatch = multiLevelPatternMatchOperator.applyPatternMatch(path, result).ToList();
                    //var newPaths = this.SubstitutionValueInPattern(valuesWithMatch, basePath, regexInformation);
                    //if (newPaths.Count() > 0)
                    //    values.AddRange(this.EvaluateOperationPatternMatchOfPath(newPaths));
                    //else
                        values.AddRange(valuesWithMatch);
                }
            }
            return values;
        }

        private string[] searchFileChildren(string path, bool withRegex)
        {
            var isForUnix = this.Platform.Equals(FamilyEnumeration.unix);
            var filepath = withRegex ? RegexHelper.GetFixedPartFromPathWithPattern(path, isForUnix) : path;
            return this.FileProvider.GetFileChildren(filepath).ToArray();
        }

        private string[] removeFilesFromList(string[] paths)
        {
            List<string> pathsWithoutFileNames = new List<string>();
            foreach (string path in paths)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (string.IsNullOrEmpty(fileName))
                    pathsWithoutFileNames.Add(path);
            }
            return pathsWithoutFileNames.ToArray();
        }

        public IEnumerable<string> SubstitutionValueInPattern(List<string> values, PathLevelWithRegex basePath, FilePathRegexInformation regexInformation)
        {
            var paths = new List<string>();
            foreach (string path in values)
            {
                var level  = basePath.Level;
                if (!regexInformation.IsWindowsFilepath())
                    level -= 1;

                var concatenatedString = regexInformation.ConcatPathWithNextLeveRegex(path, level);
                if (!string.IsNullOrEmpty(concatenatedString))
                    paths.Add(concatenatedString);
            }

            return paths;
        }        
    }
}
