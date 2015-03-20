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
using System.Linq;
using System.Reflection;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.entityOperations;

namespace Modulo.Collect.OVAL.Helpers
{
    public class StateTypeComparator
    {

        private StateType stateType;
        private ItemType itemType;
        private VariablesEvaluated variables;
        private const string MULTI_VALUED_VARIABLE_NOT_SUPPORTED = "References for variables with multiples values is not supported in state evaluation. Variable id [ {0} ]";

        public StateTypeComparator(StateType stateType, ItemType itemType, VariablesEvaluated variables)
        {
            this.itemType = itemType;
            this.stateType = stateType;
            this.variables = variables;
        }

        /// <summary>
        /// This method travels all the fields of ItemType comparing with the fields of another itemType.
        /// </summary>
        /// <param name="typeOfItemType">Type of the type of item.</param>
        /// <param name="typeOfStateType">Type of the type of other item.</param>
        /// <returns></returns>
        public bool IsEquals()
        {
            var stateEntityNameResolver = new StateEntityNameResolver();
            Type typeOfItemType = itemType.GetType();
            Type typeOfStateType = stateType.GetType();

            var fiedsOfStateType = typeOfStateType.GetProperties();
            

            foreach (var field in fiedsOfStateType)
            {
                var valueOfState = field.GetValue(this.stateType, null);
                var stateEntityName = stateEntityNameResolver.Resolve(field); 
                
                if (valueOfState is EntitySimpleBaseType)
                {
                    if ((EntitySimpleBaseType)valueOfState != null)
                    {
                        var allItemEntities = typeOfItemType.GetProperties();
                        var fieldOfItemType = 
                            allItemEntities
                                .FirstOrDefault(x => x.Name.Equals(stateEntityName, StringComparison.InvariantCultureIgnoreCase));
                        if (fieldOfItemType == null)
                            return false;

                        if (!this.ProcessComparations(field, fieldOfItemType))
                            return false;
                    }
                }
                else if (valueOfState is EntityComplexBaseType)
                {
                    if ((EntityComplexBaseType)valueOfState != null)
                    {
                        var fieldOfItemType = 
                            typeOfItemType.GetProperties()
                                .FirstOrDefault(x => x.Name.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase));

                        if (fieldOfItemType == null)
                            return false;
                        else
                            return ProcessComparisionForComplexEntityType(
                                (EntityItemRecordType[])fieldOfItemType.GetValue(this.itemType, null),
                                (EntityStateRecordType)valueOfState);
                    }

                }
            }
            return true;
        }

        private bool ProcessComparisionForComplexEntityType(
            EntityItemRecordType[] valueOfFieldItemType,
            EntityStateRecordType valueOfFieldStateType)
        {
            if (valueOfFieldItemType == null)
                return false;

            var recordFieldsOfState = valueOfFieldStateType.field;
            var recordFieldsOfItem = valueOfFieldItemType.First().field;

            foreach (var stateField in recordFieldsOfState)
            {
                var equivalentItemField = 
                    recordFieldsOfItem.Where(
                        item => item.name.Equals(stateField.name, StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();

                if (equivalentItemField == null)
                    return false;


                var stateEntityFieldValue = stateField.Value;
                if (!String.IsNullOrWhiteSpace(stateField.var_ref))
                    stateEntityFieldValue = ProcessVariableById(stateField.var_ref);

                var ovalComparator = new OvalComparatorFactory().GetComparator(stateField.datatype);
                if (!ovalComparator.Compare(equivalentItemField.Value, stateEntityFieldValue, stateField.operation))
                    return false;
            }

            return true;
        }

        private bool ProcessComparations(PropertyInfo field, PropertyInfo fieldOfItemType)
        {
            object valueOfItemTypeField = fieldOfItemType.GetValue(this.itemType, null);
            if (valueOfItemTypeField == null)
            {
                object valueOfField = field.GetValue(this.stateType, null);
                if (valueOfField != null)
                    return false;
            }
            if ((valueOfItemTypeField is EntityItemSimpleBaseType) || (valueOfItemTypeField is EntityItemSimpleBaseType[]))
            {
                if (!valueOfItemTypeField.GetType().IsArray)
                {
                    if (!this.CompareTwoEntities(field, fieldOfItemType))
                        return false;
                }
                else
                {
                    if (!this.CompareMultiplesEntities(field, fieldOfItemType))
                        return false;
                }
            }
            return true;
        }

        private bool CompareMultiplesEntities(PropertyInfo field, PropertyInfo fieldOfItemType)
        {
            object valueOfState = field.GetValue(this.stateType, null);
            if (valueOfState == null)
                return false;

            EntityItemSimpleBaseType[] valuesOfItemType = (EntityItemSimpleBaseType[])fieldOfItemType.GetValue(this.itemType, null);

            foreach (EntityItemSimpleBaseType entityItemType in valuesOfItemType)
            {
                if (this.Compare(entityItemType, (EntitySimpleBaseType)valueOfState))
                    return true;
            }
            return false;
        }

        private bool CompareTwoEntities(PropertyInfo field, PropertyInfo fieldOfItemType)
        {
            var valueOfFieldStateType = field.GetValue(this.stateType, null);
            var valueOfFieldItemType = fieldOfItemType.GetValue(this.itemType, null);
            if ((valueOfFieldStateType != null) && (valueOfFieldItemType != null))
            {
                return this.Compare((EntityItemSimpleBaseType)valueOfFieldItemType, (EntitySimpleBaseType)valueOfFieldStateType);
            }
            else
            {
                return this.CheckNullForEntities((EntityItemSimpleBaseType)valueOfFieldItemType, (EntitySimpleBaseType)valueOfFieldStateType);
            }
        }

        private bool CheckNullForEntities(EntityItemSimpleBaseType valueOfFieldItemType, EntitySimpleBaseType valueOfFieldStateType)
        {
            return (valueOfFieldItemType == null) && (valueOfFieldStateType == null);

        }

        private bool Compare(EntityItemSimpleBaseType valueOfFieldItemType, EntitySimpleBaseType valueOfFieldStateType)
        {
            string itemValue = valueOfFieldItemType.Value;
            string stateValue = valueOfFieldStateType.Value;

            if ((itemValue == null) && (stateValue == null))
                return true;

            // If one of objects to compare is null and another one is not null the compare result must be false.
            if (this.IsOneOfTheseObjectsNullAndAnotherOneNotNull(itemValue, stateValue) && string.IsNullOrEmpty(valueOfFieldStateType.var_ref))
                return false;

            var value = valueOfFieldStateType.Value;
            if (IsThereDefinedVariable(valueOfFieldStateType))
                value = ProcessVariableById(valueOfFieldStateType.var_ref);

            IOvalComparator comparator = new OvalComparatorFactory().GetComparator(valueOfFieldStateType.datatype);
            string firstElement = itemValue;
            string secondElement = value;
            return comparator.Compare(firstElement, secondElement, valueOfFieldStateType.operation);
        }

        private string ProcessVariableById(string variableID)
        {
            var variableValues = this.variables.GetVariableValueForVariableId(variableID);
            
            var valueNotFound =
                ( (variableValues == null) ||
                  (variableValues.Count() == 0) ||
                  (this.variables.VariableValues == null) ||
                  (this.variables.VariableValues.Count() == 0));

            if (valueNotFound)
                return string.Empty;
            
            if (variableValues.Count() > 1 || variableValues.First().values.Count() > 1)
                throw new NotSupportedException(string.Format(MULTI_VALUED_VARIABLE_NOT_SUPPORTED, variableID));
            
            var values = variableValues.First().values.FirstOrDefault();
            return values ?? string.Empty;
        }

        private bool IsOneOfTheseObjectsNullAndAnotherOneNotNull(object firstObject, object secondObject)
        {
            return (((firstObject == null) && (secondObject != null)) || ((firstObject != null) && (secondObject == null)));
        }

        private static bool IsThereDefinedVariable(EntitySimpleBaseType valueOfFieldStateType)
        {
            return !String.IsNullOrEmpty(valueOfFieldStateType.var_ref);
        }
    }
}
