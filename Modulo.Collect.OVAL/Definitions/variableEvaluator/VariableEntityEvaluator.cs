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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modulo.Collect.OVAL.Definitions.variableEvaluator
{
    public class VariableEntityEvaluator
    {

        private VariablesEvaluated variables;

        public VariableEntityEvaluator(VariablesEvaluated variables)
        {
            this.variables = variables;
        }

        /// <summary>
        /// Set the variable value in the Entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public IEnumerable<string> EvaluateVariableForEntity(EntitySimpleBaseType entity)
        {
            List<string> variableValues = new List<string>();
            if (entity == null)
                return variableValues;

            if (this.isVariableSet(entity))
                variableValues.AddRange(this.processEntityVariable(entity));
            else
                variableValues.Add(entity.Value);

            return variableValues;
        }        

        private bool isVariableSet(EntitySimpleBaseType entity)
        {
            return ((entity.var_ref != null) && (entity.var_ref != ""));
        }

        private IEnumerable<string> processEntityVariable(EntitySimpleBaseType entity)
        {
            List<string> values = new List<string>();
            IEnumerable<VariableValue> variablesOfEntity = variables.GetVariableValueForVariableId(entity.var_ref);
            if (variablesOfEntity.Count() > 0)
                values.AddRange(variablesOfEntity.FirstOrDefault().values);            
            return values;
        }
    }
}
