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
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Common.VariableProbe
{
    public class VariableObjectCollector: BaseObjectCollector
    {
        public VariablesEvaluated VariableEvaluated { get; set; }
        
        private ExecutionLogBuilder executionLogBuilder = new ExecutionLogBuilder();

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            return new List<string>();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            if (systemItem.status == StatusEnumeration.doesnotexist)
                return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());

            var variableReference = string.Empty;
            try
            {
                
                variableReference = ((variable_item)systemItem).var_ref.Value;
                base.ExecutionLogBuilder.CollectingDataFrom(variableReference);
                
                var collectedVariableValues = this.CollectVariableItem(variableReference);
                
                SetVariableItemFromVariableValues((variable_item)systemItem, collectedVariableValues);
            }
            catch(VariableNotFoundException)
            {
                SetDoesNotExistStatusForItemType(systemItem, variableReference);
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        

        private IEnumerable<VariableValue> CollectVariableItem(string variableReference)
        {
            var variableValues = this.VariableEvaluated.GetVariableValueForVariableId(variableReference);
            
            var variableNotFound = ((variableValues == null) || (variableValues.Count() == 0));
            if (variableNotFound)
                throw new VariableNotFoundException();

            return variableValues;
        }        

        private void SetVariableItemFromVariableValues(variable_item variableItem, IEnumerable<VariableValue> variableValues)
        {
            var values = new List<string>();
            foreach(var variableValue in variableValues)
                foreach(string value in variableValue.values)
                    values.Add(value);

            if (values.Count == 0)
                throw new VariableNotFoundException();

            variableItem.value = new EntityItemAnySimpleType[values.Count()];
            for (int i = 0; i < values.Count(); i++)
                variableItem.value[i] = OvalHelper.CreateEntityItemAnyTypeWithValue(values.ElementAt(i));
        }       
    }

    public class VariableNotFoundException : Exception { } 
}
