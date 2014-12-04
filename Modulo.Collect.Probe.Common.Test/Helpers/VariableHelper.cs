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
using System.Linq;
using System.Collections.Generic;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.Probe.Common.Test.Helpers
{
    public class VariableHelper
    {
        public static VariablesEvaluated CreateVariableWithOneValue(string objID, string varID, string varValue)
        {
            var varValues = new List<string>() { varValue };
            var objectID = objID.Contains(":") ? objID : string.Format("oval:modulo:obj:{0}", objID);
            var variableID = varID.Contains(":") ? varID : string.Format("oval:modulo:var:{0}", varID);
            
            var variables = new List<VariableValue>() { new VariableValue(objectID, variableID, varValues) };
            return new VariablesEvaluated(variables);
        }

        public static VariablesEvaluated CreateVariableWithMultiplesValue(string objID, string varID, string[] varValues)
        {
            var objectID = objID.Contains(":") ? objID : string.Format("oval:modulo:obj:{0}", objID);
            var variableID = varID.Contains(":") ? varID : string.Format("oval:modulo:var:{0}", varID);
            
            var variableValues = new VariableValue(objectID, variableID, new List<string>(varValues));
            return new VariablesEvaluated(new List<VariableValue>() { variableValues });
        }

        /// <summary>
        /// It creates a set of evaluated variables and values for a collected object.
        /// </summary>
        /// <param name="objectID">Collected Object identificator.</param>
        /// <param name="variables">A list of key value pair where the key is the variable identificator and the value a list of variable values.</param>
        /// <returns></returns>
        public static VariablesEvaluated CreateEvaluatedVariables(string objectID, Dictionary<string, IEnumerable<string>> variables)
        {
            IEnumerable<VariableValue> newVariablesValues = new List<VariableValue>();
            foreach (var variable in variables) 
                ((List<VariableValue>)newVariablesValues).Add(new VariableValue(objectID, variable.Key, variable.Value));

            return new VariablesEvaluated(newVariablesValues);
        }

        public static VariablesEvaluated CreateEvaluatedVariables(string[] objectIDs, Dictionary<string, IEnumerable<string>> variables)
        {
            IEnumerable<VariableValue> newVariablesValues = new List<VariableValue>();
            for (int i = 0; i < objectIDs.Count(); i++)
            {
                var objectID = objectIDs.ElementAt(i);
                var variable = variables.ElementAt(i);

                ((List<VariableValue>)newVariablesValues).Add(new VariableValue(objectID, variable.Key, variable.Value));
            }

            return new VariablesEvaluated(newVariablesValues);
        }

        public static VariablesEvaluated CreateEvaluatedVariables(Dictionary<string, Dictionary<string, IList<string>>> evaluatedVariables)
        {
            var newVariablesValues = new List<VariableValue>();
            foreach (var variable in evaluatedVariables)
            {
                var objectID = variable.Key;
                var variableValues = variable.Value;
                foreach (var varValue in variableValues)
                    newVariablesValues.Add(new VariableValue(objectID, varValue.Key, varValue.Value));
            }
            
            return new VariablesEvaluated(newVariablesValues);
        }

        public static VariablesEvaluated CreateEmptyEvaluatedVariables()
        {
            return new VariablesEvaluated(new List<VariableValue>());
        }

        public static string ExtractVariableValueFromConstantVariable(oval_definitions definitions, string variableID)
        {
            var variable = (VariablesTypeVariableConstant_variable)definitions.variables.Where(v => v.id.Equals(variableID)).First();
            return variable.value.First().Value;
        }
    }
}
