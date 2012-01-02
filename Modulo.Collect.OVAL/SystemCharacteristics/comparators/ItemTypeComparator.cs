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
using System.Text;
using System.Reflection;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.OVAL.SystemCharacteristics.comparators
{
    public class ItemTypeComparator
    {

        private object itemType;
        private object otherItemType;

        public ItemTypeComparator(object itemType, object otherItemType)
        {
            this.itemType = itemType;
            this.otherItemType = otherItemType;
        }

        /// <summary>
        /// This method travels all the fields of ItemType comparing with the fields of another itemType.
        /// </summary>
        /// <param name="typeOfItemType">Type of the type of item.</param>
        /// <param name="typeOfOtherItemType">Type of the type of other item.</param>
        /// <returns></returns>
        public bool IsEquals()
        {
            Type typeOfItemType = itemType.GetType();
            Type typeOfOtherItemType = otherItemType.GetType();

            var fieldsOfItemType = typeOfItemType.GetProperties();
            foreach (var field in fieldsOfItemType)
            {
                var fieldOfOtherItemType = typeOfOtherItemType.GetProperty(field.Name);
                if (fieldOfOtherItemType == null)
                    return false;

                if (!this.ProcessComparations(field, fieldOfOtherItemType))
                    return false;
            }
            return true;
        }

        private bool ProcessComparations(PropertyInfo field, PropertyInfo fieldOfOtherItemType)
        {
            object valueOfOtherField = fieldOfOtherItemType.GetValue(this.otherItemType,null);            
            if (valueOfOtherField == null)
            {
                object valueOfField = field.GetValue(this.itemType, null);
                if (valueOfOtherField != null)
                    return false;
            }
            if ((valueOfOtherField is EntityItemSimpleBaseType) || (valueOfOtherField is EntityItemSimpleBaseType[]))
            {
                if (!valueOfOtherField.GetType().IsArray)
                {
                    if (!this.CompareTwoEntities(field, fieldOfOtherItemType))
                        return false;
                }
                else
                {
                    if (!this.CompareMultiplesEntities(field, fieldOfOtherItemType))
                        return false;
                }
            }
            return true;
        }

        private bool CompareMultiplesEntities(PropertyInfo field, PropertyInfo fieldOfOtherItemType)
        {
            EntityItemSimpleBaseType[] valuesOfItemType = (EntityItemSimpleBaseType[])field.GetValue(this.itemType,null);
            EntityItemSimpleBaseType[] valuesOfOtherItemType = (EntityItemSimpleBaseType[])fieldOfOtherItemType.GetValue(this.otherItemType, null);

            if (valuesOfItemType.Count() != valuesOfOtherItemType.Count())
                return false;

            foreach (EntityItemSimpleBaseType entityItemType in valuesOfItemType)
            {
                IEnumerable<EntityItemSimpleBaseType> otherEntityItemType = valuesOfOtherItemType.Where(entity => this.Compare(entityItemType, entity));
                if ((otherEntityItemType == null) || (otherEntityItemType.Count() == 0))
                    return false;
            }
            return true;
        }

        private bool CompareTwoEntities(PropertyInfo field, PropertyInfo fieldOfOtherItemType)
        {
            EntityItemSimpleBaseType valueOfFieldItemType = (EntityItemSimpleBaseType)field.GetValue(this.itemType,null);
            EntityItemSimpleBaseType valueOfFieldOtherItemType = (EntityItemSimpleBaseType)fieldOfOtherItemType.GetValue(this.otherItemType, null);
            if ((valueOfFieldItemType != null) && (valueOfFieldOtherItemType != null))
            {
                return this.Compare(valueOfFieldItemType, valueOfFieldOtherItemType);            
            }
            else
            {
                return this.CheckNullForEntities(valueOfFieldItemType, valueOfFieldOtherItemType); 
            }
        }

        private bool CheckNullForEntities(EntityItemSimpleBaseType valueOfFieldItemType, EntityItemSimpleBaseType valueOfFieldOtherItemType)
        {
            return (valueOfFieldItemType == null) && (valueOfFieldOtherItemType == null);
                
        }

        private  bool Compare(EntityItemSimpleBaseType valueOfFieldItemType, EntityItemSimpleBaseType valueOfFieldOtherItemType)
        {           
            if ((valueOfFieldItemType.Value == null) && (valueOfFieldOtherItemType.Value == null))
                return true;


            if (valueOfFieldItemType.status != valueOfFieldOtherItemType.status)
                return false;

            IOvalComparator comparator = new OvalComparatorFactory().GetComparator(valueOfFieldItemType.datatype);
            return comparator.Compare(valueOfFieldItemType.Value, valueOfFieldOtherItemType.Value, OperationEnumeration.equals);
        }

    }
}
