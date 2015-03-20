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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.EntityOperations;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.Operators;


namespace Modulo.Collect.Probe.Windows.SID
{
    public class SIDItemTypeGenerator : IItemTypeGenerator
    {
        public BaseObjectCollector SystemDataSource { get; set; }

        private OperatorHelper operatorHelper = new OperatorHelper();



        public IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var trusteeNames = this.processVariables(objectType, variables);
            if (trusteeNames == null)
                return new List<ItemType>();

            var derivedTrusteeNames = this.processOperation(objectType, trusteeNames.ToArray());

            return this.createSidItemsToCollect(derivedTrusteeNames.ToList());
        }


        private IEnumerable<String> processVariables(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            string trusteeNameEntityValue = ((sid_object)objectType).TrusteeName.Value;

            if ((variables == null) || (variables.VariableValues.Count() == 0))
                return new List<String>(new string[] { trusteeNameEntityValue });
            else
            {
                var variableEvaluator = new SIDEntityVariableEvaluator(objectType, variables);
                var processedVariables = variableEvaluator.ProcessVariableForAllObjectEntities();
                return (processedVariables == null) ? null : processedVariables;
            }
        }

        private IEnumerable<string> processOperation(OVAL.Definitions.ObjectType objectType, IEnumerable<String> evalutedVariables)
        {
            var trusteeEntity = ((sid_object)objectType).TrusteeName;
            var entityOperation = trusteeEntity.operation;
            string[] trusteeNames = string.IsNullOrEmpty(trusteeEntity.var_ref) ? new string[] { trusteeEntity.Value } : evalutedVariables.ToArray();

            var operationResult = new List<String>();
            foreach (var trusteeName in trusteeNames)
            {
                var valuesToApply = (this.getValuesToApplyOperation(entityOperation, trusteeName)).ToArray();
                if (entityOperation == OperationEnumeration.patternmatch)
                {
                    var multiLevelOperation = new MultiLevelPatternMatchOperation(FamilyEnumeration.windows);
                    operationResult.AddRange(multiLevelOperation.applyPatternMatch(trusteeName, valuesToApply));
                }
                else
                    operationResult.AddRange(this.EvaluateOperationsDifferentsOfPatternMatch(entityOperation, trusteeName, valuesToApply));
            }

            return operationResult;
        }

        private IList<ItemType> createSidItemsToCollect(List<string> derivedUsers)
        {
            var sidItemsToCollect = new List<ItemType>();
            foreach (var user in derivedUsers)
                sidItemsToCollect.Add(this.createSIDItem(user));

            return sidItemsToCollect;
        }

        private ItemType createSIDItem(string user)
        {
            return new sid_item() { trustee_name = new EntityItemStringType() { Value = user } };
        }

        private IEnumerable<string> getValuesToApplyOperation(OperationEnumeration entityOperation, string entityValue)
        {
            bool isEqualOperation =
                ((entityOperation == OperationEnumeration.equals) ||
                 (entityOperation == OperationEnumeration.caseinsensitiveequals));

            return isEqualOperation ? new string[] { entityValue } : this.searchUsers(entityValue);
        }

        private IEnumerable<string> EvaluateOperationsDifferentsOfPatternMatch(OperationEnumeration operation, string entityValue, IEnumerable<string> valuesToMatch)
        {
            var comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            var values = new List<string>();
            foreach (string valueToMatch in valuesToMatch)
                if (comparator.Compare(entityValue, valueToMatch, operation))
                    values.Add(valueToMatch);
            
            return values;
        }

        private string[] searchUsers(string trusteeName)
        {
            var searchValuesParameters = new Dictionary<string, object>();
            searchValuesParameters.Add("trusteeName", trusteeName);

            return this.SystemDataSource.GetValues(searchValuesParameters).ToArray();
        }

        private Dictionary<string, object> getSIDEntityOperationParams(string[] valuesToApply, string entityValue)
        {
            var entityOperationParameters = new Dictionary<string, object>();
            entityOperationParameters.Add(EntityOperationParameters.ValuesToApply.ToString(), valuesToApply);
            entityOperationParameters.Add(EntityOperationParameters.EntityValue.ToString(), entityValue);

            return entityOperationParameters;
        }
    }
}
