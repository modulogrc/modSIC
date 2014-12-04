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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.GroupSID
{
    public class GroupSIDItemTypeGenerator : IItemTypeGenerator
    {
        public BaseObjectCollector SystemDataSource { get; set; }

        public TargetInfo TargetInfo { get; set; }


        public virtual IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var processedGroupSIDs = this.processVariables(objectType, variables);
            if (processedGroupSIDs == null)
                return null;

            processedGroupSIDs = this.ApplyOperations(objectType, processedGroupSIDs);

            var itemsToCollect = new List<ItemType>();
            foreach (var groupSID in processedGroupSIDs)
            {
                var newItemToCollect = new group_sid_item() { group_sid = OvalHelper.CreateItemEntityWithStringValue(groupSID) };
                itemsToCollect.Add(newItemToCollect);
            }

            return itemsToCollect;
        }

        private IEnumerable<string> ApplyOperations(Definitions.ObjectType sourceObject, IEnumerable<string> processedGroupSIDs)
        {
            var groupSIDEntity = ((group_sid_object)sourceObject).GetGroupSIDEntity();
            if (groupSIDEntity.operation == OperationEnumeration.equals)
                return processedGroupSIDs;

            return this.ProcessOperationDifferentOfEquals(groupSIDEntity.operation, processedGroupSIDs);
        }

        private IList<string> ProcessOperationDifferentOfEquals(OperationEnumeration operation, IEnumerable<String> allEnntityValues)
        {
            var allGroupSIDs = this.getAllGroupSIDsOnTargetMachine();
            var result = new List<String>();
            foreach (var entityValue in allEnntityValues)
                result.AddRange(this.processOperation(allGroupSIDs.ToArray(), entityValue, operation));

            return result;
        }

        private IList<String> processOperation(string[] allGroupSIDs, string entityValue, OperationEnumeration operation)
        {
            var comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);

            var processingResult = new List<String>();
            foreach (var groupSID in allGroupSIDs)
                if (comparator.Compare(groupSID, entityValue, operation))
                    processingResult.Add(groupSID);

            return processingResult;
        }

        private IEnumerable<String> getAllGroupSIDsOnTargetMachine()
        {
            if (this.SystemDataSource == null)
                this.SystemDataSource = new GroupSIDObjectCollector() { TargetInfo = this.TargetInfo };

            return this.SystemDataSource.GetValues(null);

        }

        private IEnumerable<String> processVariables(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var groupSIDEntity = ((group_sid_object)objectType).GetGroupSIDEntity();
            var thereIsNoEvaluatedVariable = ((variables == null) || (variables.VariableValues.Count() == 0));

            if (thereIsNoEvaluatedVariable && (string.IsNullOrEmpty(groupSIDEntity.var_ref)))
                return new List<String>(new string[] { groupSIDEntity.Value });

            var result = new GroupSIDEntityVariableEvaluator(objectType, variables).TryToProcessObjectEntity(objectType.ComponentString);
            return (result == null) ? null : result;
        }
    }
}
