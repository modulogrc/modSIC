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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;

namespace Modulo.Collect.Probe.Common.BasicClasses
{
    public class CollectedObject
    {       
        private ObjectType objectType = null;
        private List<ReferenceType> referenceTypeList;
        private List<ItemType> systemData = new List<ItemType>();


        public ObjectType ObjectType 
        { 
            get 
            {
                this.objectType.reference = referenceTypeList.ToArray();
                return this.objectType; 
            } 
        }

        public IList<ItemType> SystemData { get { return this.systemData; } }
    
        public CollectedObject(string componentOvalID)
        {
            this.objectType = new ObjectType() { id = componentOvalID, flag = FlagEnumeration.notcollected, version = "1" };
            referenceTypeList = new List<ReferenceType>();
        }

        public void AddItemToSystemData(ItemType itemType)
        {            
            referenceTypeList.Add(new ReferenceType() { item_ref = itemType.id });
            this.systemData.Add(itemType);
        }

        public void AddVariableReference(IEnumerable<VariableValue> variables)
        {
            List<VariableValueType> variableTypes = new List<VariableValueType>();
            foreach (VariableValue variable in variables)
            {
                variableTypes.AddRange(variable.ToOvalVariableType());                
            }
            var variableValueType = variableTypes.ToArray<VariableValueType>();
            this.objectType.variable_value = variableValueType;

        }

        public void UpdateCollectedObjectStatus()
        {
            IEnumerable<ItemType> itemsWithError = this.systemData.Where<ItemType>(item => item.status == StatusEnumeration.error);
            IEnumerable<ItemType> itemsDoesNotExists = this.systemData.Where<ItemType>(item => item.status == StatusEnumeration.doesnotexist);
            if (itemsWithError.Count() > 0)
            {
                this.objectType.flag = FlagEnumeration.error;
            }
            else if (itemsDoesNotExists.Count() > 0)
            {
                this.objectType.flag = FlagEnumeration.doesnotexist;
            }
            else
            {
                this.objectType.flag = FlagEnumeration.complete;
            }
        }

        public void SetEspecificObjectStatus(FlagEnumeration flag)
        {
            this.objectType.flag = flag;
        }
    }
}
