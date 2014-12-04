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


using System.Xml.Serialization;
using Modulo.Collect.OVAL.Common;
using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using System;

namespace Modulo.Collect.OVAL.Results
{
  
    public partial class TestType
    {
        public TestType(Definitions.TestType test) : this()
        {
            this.test_id = test.id;
            this.check = test.check;
            this.check_existence = test.check_existence;
            this.state_operator = test.state_operator;
            this.version = test.version;
        }

        bool bAnalyzed = false;
        public ResultEnumeration Analyze(oval_results results)
        {
            if (!bAnalyzed)
            {
                var refTest = results.oval_definitions.tests.Single(x => x.id == test_id);
                if (!refTest.HasObjectReference)
                {
                    return ResultEnumeration.unknown;
                }
                var fi = refTest.GetType().GetProperty("object");
                ObjectRefType objRef = fi.GetValue(refTest,null) as ObjectRefType;
                var sc = results.results[0].oval_system_characteristics;
                var colObject = sc.collected_objects.SingleOrDefault(x => x.id == objRef.object_ref);
                if (colObject == null)
                {
                    result = ResultEnumeration.notevaluated;
                }
                else
                {
                    // Copy all Referenced Items to result
                    if (colObject.reference != null)
                    {
                        foreach (var itemKey in colObject.reference)
                        {
                            var item = sc.system_data.Single(x => x.id == itemKey.item_ref);
                            
                            var newMessageType = item.message == null ? new MessageType[] { } :  item.message;
                            var newTestedItemType = new TestedItemType()
                            {
                                item_id = item.id,
                                message = newMessageType.ToList(),
                                status = item.status,
                                result = ResultEnumeration.notevaluated
                            };

                            tested_item.Add(newTestedItemType);
                        }
                    }
                    // Copy all Referenced Items to result
                    if (colObject.variable_value != null)
                    {
                        foreach (var varKey in colObject.variable_value)
                        {
                            tested_variable.Add(new TestedVariableType() { variable_id = varKey.variable_id, Value = varKey.Value });
                        }
                    }

                    switch (colObject.flag)
                    {
                        case SystemCharacteristics.FlagEnumeration.error:
                            result = ResultEnumeration.error;
                            break;
                        case SystemCharacteristics.FlagEnumeration.notapplicable:
                            result = ResultEnumeration.notapplicable;
                            break;
                        case SystemCharacteristics.FlagEnumeration.notcollected:
                            result = ResultEnumeration.unknown;
                            break;
                        case SystemCharacteristics.FlagEnumeration.doesnotexist:
                            if ((check_existence == ExistenceEnumeration.none_exist) || (check_existence == ExistenceEnumeration.any_exist))
                                result = ResultEnumeration.@true;
                            else
                                result = ResultEnumeration.@false;
                            break;
                        case FlagEnumeration.complete:
                            ResultEnumeration completeResult = GetCheckExistenceResult();
                            
                            if (completeResult == ResultEnumeration.@true)
                                if (IsThereReferenceStateForTest(refTest))
                                    completeResult = GetCheckStateResult(results, refTest);
                            
                            result = completeResult;
                            
                            break;
                    }
                }
                bAnalyzed = true;
            }
            return result;

        }

        private bool IsThereReferenceStateForTest(Definitions.TestType test)
        {
            var testStateField = test.GetType().GetProperty("state");
            var testReference = testStateField.GetValue(test,null) as StateRefType;
            if (testReference != null)
                return true;
            
            var testReferenceArray = testStateField.GetValue(test,null) as StateRefType[];
            return (testReferenceArray != null);
        }

        public ResultEnumeration GetCheckExistenceResult()
        {
            ResultEnumeration existenceResult = ResultEnumeration.error;

            // get the count of each status value
            int errorCount = tested_item.Where(x => x.status == StatusEnumeration.error).Count();
            int existsCount = tested_item.Where(x => x.status == StatusEnumeration.exists).Count();
            int doesNotExistCount = tested_item.Where(x => x.status == StatusEnumeration.doesnotexist).Count();
            int notCollectedCount = tested_item.Where(x => x.status == StatusEnumeration.notcollected).Count();

            var resultCounts = tested_item.GroupBy(x => x.status).Select(x => new { status = x.Key,  count = x.Count()});
            switch (check_existence)
            {
                case ExistenceEnumeration.all_exist:
                    if(existsCount >= 1 && doesNotExistCount == 0 && errorCount == 0 && notCollectedCount == 0) {
            			existenceResult =ResultEnumeration.@true;			
            		} else if(existsCount >= 0 && doesNotExistCount >= 1 && errorCount >= 0 && notCollectedCount >= 0) {
            			existenceResult =ResultEnumeration.@false;
            		} else if(existsCount == 0 && doesNotExistCount == 0 && errorCount == 0 && notCollectedCount == 0) {
            			existenceResult =ResultEnumeration.@false;
            		} else if(existsCount >= 0 && doesNotExistCount == 0 && errorCount >= 1 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.error;
            		} else if(existsCount >= 0 && doesNotExistCount == 0 && errorCount == 0 && notCollectedCount >= 1) {
            			existenceResult = ResultEnumeration.unknown;
            		}
                    break;
                case ExistenceEnumeration.any_exist:
                    if(existsCount >= 0 && doesNotExistCount >= 0 && errorCount == 0 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.@true;	
            		} else if(existsCount >= 1 && doesNotExistCount >= 0 && errorCount >= 1 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.@true;	
            		} else if(existsCount == 0 && doesNotExistCount >= 0 && errorCount >= 1 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.error;
            		} 
                    break;
                case ExistenceEnumeration.at_least_one_exists:
                    if(existsCount >= 1 && doesNotExistCount >= 0 && errorCount >= 0 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.@true;			
            		} else if(existsCount == 0 && doesNotExistCount >= 1 && errorCount == 0 && notCollectedCount == 0) {
            			existenceResult = ResultEnumeration.@false;
            		} else if(existsCount == 0 && doesNotExistCount >= 0 && errorCount >= 1 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.error;
            		} else if(existsCount == 0 && doesNotExistCount >= 0 && errorCount == 0 && notCollectedCount >= 1) {
            			existenceResult = ResultEnumeration.unknown;
            		} 
                    break;
                case ExistenceEnumeration.none_exist:
                    if(existsCount == 0 && doesNotExistCount >= 0 && errorCount == 0 && notCollectedCount == 0) {
            			existenceResult = ResultEnumeration.@true;			
            		} else if(existsCount >= 1 && doesNotExistCount >= 0 && errorCount >= 0 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.@false;
            		} else if(existsCount == 0 && doesNotExistCount >= 0 && errorCount >= 1 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.error;
            		} else if(existsCount == 0 && doesNotExistCount >= 0 && errorCount == 0 && notCollectedCount >= 1) {
            			existenceResult = ResultEnumeration.unknown;
            		}
                    break;
                case ExistenceEnumeration.only_one_exists:
                    if(existsCount == 1 && doesNotExistCount >= 0 && errorCount == 0 && notCollectedCount == 0) {
            			existenceResult = ResultEnumeration.@true;			
            		} else if(existsCount >= 2 && doesNotExistCount >= 0 && errorCount >= 0 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.@false;
            		} else if(existsCount == 0 && doesNotExistCount >= 0 && errorCount == 0 && notCollectedCount == 0) {
            			existenceResult = ResultEnumeration.@false;
            		} else if(existsCount == 0 && doesNotExistCount >= 0 && errorCount >= 1 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.error;
            		} else if(existsCount == 1 && doesNotExistCount >= 0 && errorCount >= 1 && notCollectedCount >= 0) {
            			existenceResult = ResultEnumeration.error;
            		} else if(existsCount == 0 && doesNotExistCount >= 0 && errorCount == 0 && notCollectedCount >= 1) {
            			existenceResult = ResultEnumeration.unknown;
            		} else if(existsCount == 1 && doesNotExistCount >= 0 && errorCount == 0 && notCollectedCount >= 1) {
            			existenceResult = ResultEnumeration.unknown;
            		} 
                    break;

            }
            return existenceResult;
        }

        public ResultEnumeration GetCheckStateResult(oval_results results, Definitions.TestType originalTest)
        {
            
            ResultEnumeration stateResult = ResultEnumeration.error;
            var itemsResults = new List<ResultEnumeration>();

            var testStates = ((originalTest.GetType().GetProperty("state")
                .GetValue(originalTest,null) as StateRefType[]) ?? new StateRefType[]{}).Select(x => results.oval_definitions.states.SingleOrDefault(y => y.id == x.state_ref));
            foreach(var testedItem in tested_item)
            {
                var stateResults = new List<ResultEnumeration>();
                var collectedItem = results.results[0].oval_system_characteristics.system_data.SingleOrDefault(x => x.id == testedItem.item_id);
                ResultEnumeration itemResult = ResultEnumeration.notevaluated;

                if (collectedItem == null)
                    itemResult = ResultEnumeration.notevaluated;
                else
                {
                    foreach (var state in testStates)
                    {
                        var evaluatedVariables = CreateEvalutedVariablesFromStateType(state, results);
                        var tempResult = ExecuteStateComparator(collectedItem, state, evaluatedVariables);
                        stateResults.Add(tempResult);
                    }

                    itemResult = oval_results.CombineResultsByOperator(stateResults, state_operator);
                }
                
                testedItem.result = itemResult;
                itemsResults.Add(itemResult);
            }
            stateResult = oval_results.CombineResultsByCheck(itemsResults, check);

        	return stateResult;
        }

        private ResultEnumeration ExecuteStateComparator(ItemType collectedItem, StateType state, VariablesEvaluated evaluatedVariables)
        {
            try
            {
                var stateComparator = new StateTypeComparator(state, collectedItem, evaluatedVariables);
                if (stateComparator.IsEquals())
                    return ResultEnumeration.@true;

                return ResultEnumeration.@false;
            }
            catch (NotSupportedException)
            {
                return ResultEnumeration.unknown;
            }
            
        }



        private OVAL.Definitions.variableEvaluator.VariablesEvaluated CreateEvalutedVariablesFromStateType(
            StateType state, oval_results ovalResults)
        {
            var allStateEntities = GetAllStateEntities(state);
            var variableValues = new List<OVAL.Definitions.variableEvaluator.VariableValue>();

            foreach (var stateEntity in allStateEntities)
            {
                var newVariableValue = CreateVariableValuesFromStateEntity(state.id, stateEntity, ovalResults);
                if (newVariableValue != null)
                    variableValues.Add(newVariableValue);
            }

            return
                new OVAL.Definitions.variableEvaluator.VariablesEvaluated(
                    variableValues);

        }

        private IEnumerable<string> GetAllStateEntities(StateType state)
        {
            var referencedVariables = new List<string>();
            foreach (var field in state.GetType().GetProperties())
            {
                var stateField = field.GetValue(state, null);
                
                if (stateField is EntityStateSimpleBaseType)
                    referencedVariables.Add(((EntityStateSimpleBaseType)stateField).var_ref);
                else if (stateField is EntityStateRecordType)
                {
                    var recordFields = ((EntityStateRecordType)stateField).field;
                    referencedVariables.AddRange(recordFields.Select(f => f.var_ref));
                }
            }

            return referencedVariables.ToArray();
        }

        private OVAL.Definitions.variableEvaluator.VariableValue CreateVariableValuesFromStateEntity(
            string stateID,
            string stateEntityVariable,
            oval_results ovalResults)
        {
            if (string.IsNullOrEmpty(stateEntityVariable))
                return null;

            var variableEvaluator =
                new VariableEvaluator(
                    ovalResults.oval_definitions.variables.ToArray(),
                    ovalResults.results[0].oval_system_characteristics,
                    ovalResults.EvaluatedExternalVariables);
            
            var variableValues = variableEvaluator.EvaluateVariable(stateEntityVariable);

            return new VariableValue(stateID, stateEntityVariable, variableValues);
        }
    }
}
