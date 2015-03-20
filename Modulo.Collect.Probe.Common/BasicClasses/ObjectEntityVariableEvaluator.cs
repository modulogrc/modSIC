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
using System.Collections.Generic;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;

namespace Modulo.Collect.Probe.Common
{
    public abstract class ObjectEntityVariableEvaluator
    {
        private Dictionary<String, EntityObjectStringType> allObjectEntities;

        public ObjectType ObjectType { get; private set; }
        public VariablesEvaluated Variables { get; private set; }
        public Dictionary<String, EntityObjectStringType> AllObjectEntities { get { return this.allObjectEntities; } set { this.allObjectEntities = value; } }

        public ObjectEntityVariableEvaluator(ObjectType objectType, VariablesEvaluated variablesEvaluated)
        {
            this.ObjectType = objectType;
            this.Variables = variablesEvaluated;
        }

        private string getVariableId(EntityObjectStringType entity)
        {
            string varReference = entity.var_ref;
            return string.IsNullOrEmpty(varReference) ? string.Empty : varReference;
        }

        private IEnumerable<String> processObjectEntity(EntityObjectStringType entity)
        {
            if (this.Variables == null)
                return null;
            
            string variableID = this.getVariableId(entity);
            VariableValue variableValue = this.Variables.GetVariableValueForVariableId(variableID).FirstOrDefault();

            bool hasVariableValues = ((variableValue != null) && (variableValue.values != null) && (variableValue.values.Count() > 0));
            
            if (hasVariableValues)
                return variableValue.values;
            else
                return string.IsNullOrEmpty(entity.Value) ? null : new string[] { entity.Value };
        }


        public IEnumerable<String> TryToProcessObjectEntity(string entityName)
        {
            EntityObjectStringType objectEntity;
            this.allObjectEntities.TryGetValue(entityName, out objectEntity);

            return (objectEntity == null) ? new List<String>() : this.processObjectEntity(objectEntity);
        }


        public abstract IEnumerable<string> ProcessVariableForAllObjectEntities();

    }      
}
