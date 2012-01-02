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
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;

namespace Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions
{
    public class SubStringFunctionComponent : LocalVariableFunctionComponent
    {

        private SubstringFunctionType component;

        public SubStringFunctionComponent(SubstringFunctionType component)
        {
            this.component = component;
        }

        public override IEnumerable<string> GetValue()
        {         
            return ExecuteSubstringFunction();
        }

        private List<string> ExecuteSubstringFunction()
        {
            LocalVariableComponent variable = base.components.First();
            IEnumerable<string> variableValues = variable.GetValue();
            
            List<string> resultsOfExpression = new List<string>();
            foreach (string value in variableValues)
            {
                int start = this.component.substring_start;

                string resultOfExpression = value.Substring(start-1, this.calculateSubstringLength(value));
                
                resultsOfExpression.Add(resultOfExpression);            
            }

            return resultsOfExpression;
        }

        private int calculateSubstringLength(string value)
        {
            var substringLen = this.component.substring_length;
            var valueLengthFromStart = value.Length - this.component.substring_start + 1;
            bool returnAllCharactersFromStart = ((substringLen <= 0) || (substringLen > valueLengthFromStart));

            return returnAllCharactersFromStart ? valueLengthFromStart : this.component.substring_length;
        }

        private List<string> GetListValues()
        {
            List<string> valuesOfComponents = new List<string>();

            if (this.QuantityOfComponents() > 0)
            {
                foreach (LocalVariableComponent component in base.components)
                {
                    valuesOfComponents.AddRange(component.GetValue());
                }

            }

            return valuesOfComponents;
        }
    
    }
}
