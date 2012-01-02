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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Registry.variables
{
    public class RegistryEntityVariableEvaluator
    {

        private VariablesEvaluated variables;

        public RegistryEntityVariableEvaluator(VariablesEvaluated variablesEvaluated)
        {
            if (variablesEvaluated == null)
                throw new ArgumentNullException("[ RegistryEntityVariableEvaluator ] - The VariableEvaluated cannot be null.");
            
            this.variables = variablesEvaluated;
        }

        /// <summary>
        /// Evaluates all the variables for registry object.
        /// One registry_object can create multiples RegistryObjects.
        /// This happen because the variable is defined in the RegistryEntity, and a variable can be have multiples values.
        /// </summary>
        /// <param name="registryObject">The registry object.</param>
        /// <returns></returns>
        public IEnumerable<RegistryObject> ProcessVariableForRegistryObject(registry_object registryObject)
        {
            var registry = RegistryObjectFactory.CreateRegistryObject(registryObject);
            return new List<RegistryObject>(this.ProcessVariables(registry));
        }

        private IEnumerable<RegistryObject> ProcessVariables(RegistryObject registry)
        {
            var hiveValues = this.ProcessEntityVariables(registry, registry_object_ItemsChoices.hive.ToString()).Distinct();
            var keyValues = this.ProcessEntityVariables(registry, registry_object_ItemsChoices.key.ToString()).Distinct();
            var nameValues = this.ProcessEntityVariables(registry, registry_object_ItemsChoices.name.ToString()).Distinct();            
            
            return RegistryObjectFactory
                .CreateRegistryObjectsByCombinationOfEntitiesFrom(
                    hiveValues, keyValues, nameValues,registry);
        }

        private IEnumerable<string> ProcessEntityVariables(RegistryObject registry,string entityName)
        {
            var values = new List<string>();
            var variableId = registry.GetVariableId(entityName);
            
            var variableValues =  variables.GetVariableValueForVariableId(variableId);
            if ((variableValues != null) && (variableValues.Count() > 0))
                foreach (VariableValue variableValue in variableValues)
                    values.AddRange(variableValue.values);
            else
                if ((registry.GetEntity(entityName) == null)  || (string.IsNullOrEmpty(registry.GetEntity(entityName).var_ref)))
                    values.Add(registry.GetValueOfEntity(entityName));
            
            return values;
        }
    }

}

