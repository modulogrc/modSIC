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
using Modulo.Collect.OVAL.Definitions.setEvaluator.Filter;
using Modulo.Collect.OVAL.Definitions.setEvaluator.operations;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using sc = Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.OVAL.Definitions.setEvaluator
{
    public class SetEvaluator
    {
        private const int MAX_NUMBER_OF_OBJECTS_IN_SET = 2;
        private oval_system_characteristics systemCharacteristics;
        private IEnumerable<StateType> ovalDefinitionStates;
        private VariablesEvaluated variables;
        private FilterEvaluator filterEvaluator;
        

        public SetEvaluator(oval_system_characteristics systemCharacteristics, IEnumerable<StateType> states, VariablesEvaluated variables)
        {
            this.systemCharacteristics = systemCharacteristics;
            this.ovalDefinitionStates = states;
            this.variables = variables;
            this.filterEvaluator = new FilterEvaluator(this.systemCharacteristics, this.ovalDefinitionStates, this.variables);
        }


        /// <summary>
        /// Evaluates the specified setElement
        /// </summary>
        /// <param name="set">object that represents a Set</param>
        /// <returns></returns>
        public SetResult Evaluate(set setElement)
        {
            if (this.systemCharacteristics == null)
                return new SetResult( new List<String>(),FlagEnumeration.notcollected);

            if (setElement.ExistsAnotherSetElement())
            {
                return EvaluateOtherSets(setElement);
            }
            else
            {
                return EvaluateSetElement(setElement);
            }
        }

        private SetResult EvaluateSetElement(set setElement)
        {
            List<string> results = new List<string>();
            IEnumerable<string> objectReferences = setElement.GetObjectReferences();
            IEnumerable<sc.ObjectType> objectTypes = this.GetSystemCharacteristicsObjectType(objectReferences);
            if (objectTypes.Count() > 0)
            {
                if (setElement.HasFilterElement())
                {
                    string filterValue = setElement.GetFilterValue();
                    objectTypes = filterEvaluator.ApplyFilter(objectTypes,filterValue);
                }
                return this.ExecuteOperationForSetElement(setElement, objectTypes);
            }
            return new SetResult(new List<String>(), FlagEnumeration.notcollected);
        }

        /// <summary>
        /// Evaluates the setObjects in the tree. This method invokes the Evaluate method in the recursive form.
        /// The evaluation starts in the more deep level in the tree of objects. 
        /// Then the upper levels will evaluated based on result of the deeper levels.
        /// </summary>
        /// <param name="setElement">The set element.</param>
        /// <returns></returns>
        private SetResult EvaluateOtherSets(set setElement)
        {
            IEnumerable<set> otherSetsElements = setElement.GetSets();
            SetResult resultFirstSet = this.Evaluate(otherSetsElements.First());
            SetResult resultSecondSet = new SetResult(new List<String>(), FlagEnumeration.notcollected);
            //the max number of set is 2 (reference: oval_definitions schema)
            if (otherSetsElements.Count() == MAX_NUMBER_OF_OBJECTS_IN_SET)
            {
                resultSecondSet = this.Evaluate(otherSetsElements.ElementAt(1));
            }
            SetOperation operation = this.GetOperation(setElement.set_operator);
            IEnumerable<string> results = operation.Execute(resultFirstSet.Result, resultSecondSet.Result);
            FlagEnumeration objectFlag = operation.GetObjectFlag(resultFirstSet.ObjectFlag, resultSecondSet.ObjectFlag);
            return new SetResult(results, objectFlag);
        }

        
        private SetResult ExecuteOperationForSetElement(set setElement, IEnumerable<sc.ObjectType> objectTypes)
        {
            sc.ObjectType firstObjectType = objectTypes.First();
            sc.ObjectType secondObjectType = null;
            IEnumerable<string> firstReferenceTypes = firstObjectType.GetReferenceTypesInString();
            IEnumerable<string> secondReferenceTypes = new List<string>();
            FlagEnumeration firstObjectFlag = firstObjectType.flag;
            FlagEnumeration secondObjectFlag = FlagEnumeration.complete;
            if (objectTypes.Count() == MAX_NUMBER_OF_OBJECTS_IN_SET)
            {
                //the max number to the Object_Reference is 2 (reference: oval_definitions schema)                
                secondObjectType = objectTypes.ElementAt(1);
                secondReferenceTypes= secondObjectType.GetReferenceTypesInString();
                secondObjectFlag = secondObjectType.flag;
            }
            SetOperation operation = this.GetOperation(setElement.set_operator);
            IEnumerable<string> results = operation.Execute(firstReferenceTypes, secondReferenceTypes);
            FlagEnumeration objectFlag = operation.GetObjectFlag(firstObjectFlag, secondObjectFlag);
            SetResult result = new SetResult(results, objectFlag);
            return result;
        }

        private SetOperation GetOperation(SetOperatorEnumeration setOperator)
        {
            return new SetOperationFactory().CreateSetOperation(setOperator);
        }

        private IEnumerable<sc.ObjectType> GetSystemCharacteristicsObjectType(IEnumerable<string> objectReferences)
        {
            List<sc.ObjectType> objectTypes = new List<sc::ObjectType>();
            foreach (string objectReference in objectReferences)
            {
                sc.ObjectType objectType = this.systemCharacteristics.GetCollectedObjectByID(objectReference);
                if (objectType != null)
                    objectTypes.Add(objectType);
            }
            return objectTypes;
        }
    }
}
