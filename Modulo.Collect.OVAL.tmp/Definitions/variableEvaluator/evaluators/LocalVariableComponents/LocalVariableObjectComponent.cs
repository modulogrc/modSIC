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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using systemCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;
using System.Reflection;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;

namespace Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents
{
    public class LocalVariableObjectComponent : LocalVariableComponent
    {

        private ObjectComponentType component;
        private oval_system_characteristics systemCharacteristics;

        public LocalVariableObjectComponent(ObjectComponentType objectComponent, oval_system_characteristics systemCharacteristics)
        {
            this.component = objectComponent;
            this.systemCharacteristics = systemCharacteristics;
        }

        /// <summary>
        /// Get value of the object defined in the object_ref property of ObjectComponentType.
        /// For this type of component, is necessary find the values in the systemCharacteristics.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetValue()
        {
            return this.GetAValueOfObjectInTheSystemCharacteristics(this.component.object_ref, this.component.item_field);
        }

        /// <summary>
        /// Gets the a value of object in the system characteristics.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private IEnumerable<string> GetAValueOfObjectInTheSystemCharacteristics(string objectId, string propertyName)
        {
            List<string> values = new List<string>();
            if (this.systemCharacteristics != null)
            {
                systemCharacteristics::ObjectType objectType = this.systemCharacteristics.collected_objects.Where(obj => obj.id == objectId).SingleOrDefault();
                if ((objectType != null) && (objectType.reference != null))
                {
                    foreach (systemCharacteristics::ReferenceType reference in objectType.reference)
                    {
                        ItemType itemtype = this.systemCharacteristics.system_data.Where(item => item.id == reference.item_ref).Single();

                        values.AddRange(this.GetValueFromProperty(itemtype, propertyName));
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// Gets the value from property. The property is defined by the item_ref property of ObjectComponentType.
        /// </summary>
        /// <param name="itemtype">The itemtype.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private List<string> GetValueFromProperty(ItemType itemtype, string propertyName)
        {
            List<string> values = new List<string>();
            Type type = itemtype.GetType();           
           
            object value = this.GetPropertyByName(itemtype, propertyName, type);
            if ((value is EntityItemSimpleBaseType))            
            {
                values.Add(this.GetPropertyByName(value,"Value",typeof(EntityItemSimpleBaseType)).ToString());
            }
            else if (value is EntityItemSimpleBaseType[])
            {
                values.AddRange(this.GetValuesOfArrayProperty(value));
            }
            return values;
        }

        private object GetPropertyByName(Object obj, string propertyName, Type type)
        {
            object value = null;
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                FieldInfo field = type.GetField(propertyName);
                value = field.GetValue(obj);
            }
            else
            {
                value = property.GetValue(obj, null);
            }            
            return value;
        }

        private List<string> GetValuesOfArrayProperty(object obj)
        {
            List<String> values = new List<string>();
            if (obj is object[])
            {
                foreach (object value in (object[])obj)
                {
                    values.Add(this.GetPropertyByName(value,"Value",value.GetType()).ToString());
                }
            }
            return values;
        }
        
    }
}
