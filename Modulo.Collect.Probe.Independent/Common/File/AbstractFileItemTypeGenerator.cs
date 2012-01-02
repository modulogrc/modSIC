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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.EntityOperations;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.Operators;

namespace Modulo.Collect.Probe.Independent.Common.File
{
    public abstract class AbstractFileItemTypeGenerator
    {
        public FileEntityOperationEvaluator OperationEvaluator { get; set; }

        public FileEntityVariableEvaluator FileVariableEvaluator { get; set; }

        public BaseObjectCollector SystemDataSource { get; set; }

        public IFileProvider FileProvider { get; set; }

        /// <summary>
        /// Creates a items to collect from given object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>A list of ItemType</returns>
        public virtual IList<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            this.CreateEvaluators(variables);
            
            var objectTypes = new Definitions.ObjectType[] { objectType };
            if (this.HasVariableDefined(objectType))
            {
                if (objectType is OVAL.Definitions.Unix.file_object)
                    objectTypes = this.FileVariableEvaluator.ProcessVariables((OVAL.Definitions.Unix.file_object)objectType).ToArray();
                else
                    objectTypes = this.FileVariableEvaluator.ProcessVariables((OVAL.Definitions.Windows.file_object)objectType).ToArray();
            }

            var itemsToCollect = this.OperationEvaluator.ProcessOperation(objectTypes).ToList();
            this.OperationEvaluator = null;
            this.FileVariableEvaluator = null;
            return itemsToCollect;


        }

        protected abstract FamilyEnumeration GetPlatform();

        protected abstract bool HasVariableDefined(Definitions.ObjectType objectType);

        protected abstract ItemType CreateFileItem(string fullFilePath, bool containsFilePathEntity);

        protected EntityItemStringType CreateEntityItemWithValue(string entityValue)
        {
            return new EntityItemStringType() { Value = entityValue };
        }

        protected string GetObjectEntityValue(string entityName)
        {
            return this.VariableEvaluator.AllObjectEntities[entityName].Value.ToString();
        }


        private ObjectEntityVariableEvaluator VariableEvaluator = null;

        private OperatorHelper operatorHelper = new OperatorHelper();

        private IEnumerable<string> processVariables(Definitions::ObjectType objectType, VariablesEvaluated variables)
        {
            if (this.VariableEvaluator == null)
                throw new Exception("[AbstractFileItemTypeGenerator] - The Variable Evaluator must be set.");


            if ((variables == null) || (variables.VariableValues.Count() == 0))
                return new List<String>(new string[] { OvalHelper.GetFullFilePathFromObjectType(objectType) });

            return this.VariableEvaluator.ProcessVariableForAllObjectEntities();
        }

        private IEnumerable<string> processOperation(Definitions::ObjectType objectType, IEnumerable<string> fullFilePaths)
        {
            OperationEnumeration entityOperation = OvalHelper.GetFileEntityOperation(this.VariableEvaluator.AllObjectEntities);

            IEnumerable<string> allDerivedFilePaths = new List<String>();
            foreach (var fullFilePath in fullFilePaths)
            {
                bool isPatternMatchOperation = entityOperation == OperationEnumeration.patternmatch;
                string[] valuesToApply = isPatternMatchOperation ? this.searchFileChildren(fullFilePath) : new string[] { fullFilePath };
                ((List<String>)allDerivedFilePaths).AddRange(this.EvaluateOperations(entityOperation, fullFilePath, valuesToApply));
            }

            return allDerivedFilePaths;
        }

        private IEnumerable<string> EvaluateOperations(OperationEnumeration operation, string entityValue, IEnumerable<string> valuesToMatch)
        {
            if (operatorHelper.IsRegularExpression(operation))
            {
                var multiLevelPatternMatchOperator = new MultiLevelPatternMatchOperation(GetPlatform());
                return multiLevelPatternMatchOperator.applyPatternMatch(entityValue, valuesToMatch);
            }
            else
            {
                return this.EvaluateOperationsDifferentsOfPatternMatch(operation, entityValue, valuesToMatch);
            }
        }

        private IEnumerable<string> EvaluateOperationsDifferentsOfPatternMatch(OperationEnumeration operation, string entityValue, IEnumerable<string> valuesToMatch)
        {
            IOvalComparator comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            List<string> values = new List<string>();
            foreach (string valueToMatch in valuesToMatch)
            {
                if (comparator.Compare(entityValue, valueToMatch, operation))
                    values.Add(valueToMatch);
            }
            return values;
        }

        private IList<ItemType> createFileItemsToCollect(IEnumerable<string> resolvedFilePaths)
        {
            IList<ItemType> fileItemsToCollect = new List<ItemType>();
            foreach (var fullPath in resolvedFilePaths)
            {
                bool isFilePathDefined = OvalHelper.IsFilePathEntityDefined(this.VariableEvaluator.AllObjectEntities);
                fileItemsToCollect.Add(this.CreateFileItem(fullPath, isFilePathDefined));
            }

            return fileItemsToCollect;

        }

        private string[] searchFileChildren(string path)
        {
            var isUnix = GetPlatform().Equals(FamilyEnumeration.unix);

            var searchValuesParameters = new Dictionary<string, object>();
            searchValuesParameters.Add("filePath", RegexHelper.GetFixedPartFromPathWithPattern(path, isUnix));

            return this.SystemDataSource.GetValues(searchValuesParameters).ToArray();
        }

        private Dictionary<string, object> getFileEntityOperationParams(string[] valuesToApply, string entityValue)
        {
            Dictionary<string, object> entityOperationParameters = new Dictionary<string, object>();
            entityOperationParameters.Add(EntityOperationParameters.ValuesToApply.ToString(), valuesToApply);
            entityOperationParameters.Add(EntityOperationParameters.EntityValue.ToString(), entityValue);

            return entityOperationParameters;
        }

        private void CreateEvaluators(VariablesEvaluated variables)
        {
            if (this.OperationEvaluator == null)
                this.OperationEvaluator = new FileEntityOperationEvaluator(this.FileProvider, GetPlatform());

            if (this.FileVariableEvaluator == null)
                this.FileVariableEvaluator = new FileEntityVariableEvaluator(variables);
        }
    }
}
