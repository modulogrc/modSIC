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
using System.Text;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;

namespace Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions
{
    public class OvalConcatFunction
    {
        private List<LocalVariableComponent> components;

        public OvalConcatFunction(List<LocalVariableComponent> components)
        {
            this.components = components;
        }


        public List<string> ConcatValues()
        {
            return this.GetValuesConcatenated();
        }


        private List<string> GetValuesConcatenated()
        {
            List<string> values = new List<string>();
            IEnumerable<string> valuesOfFirstExpression = components.First().GetValue();
            this.components.Remove(components.First());
            values.AddRange(this.ConcatTheValuesOfComponentsWithTheValuesOfFirstExpression(valuesOfFirstExpression));
            return values;
        }

        private List<string> ConcatTheValuesOfComponentsWithTheValuesOfFirstExpression(IEnumerable<string> valuesOfFirstExpression)
        {
            List<string> resultOfExpression = new List<string>();
            foreach (string value in valuesOfFirstExpression)
            {
                List<string> values = new List<string>();
                values.Add(value);
                resultOfExpression.AddRange(this.ConcatTheValuesOfComponentsWithTheNextExpressions(values, this.GetCopyComponents()));

            }
            return resultOfExpression;
        }

        private List<string> ConcatTheValuesOfComponentsWithTheNextExpressions(List<string> valuesOfExpression, List<LocalVariableComponent> newComponentList)
        {
            if (newComponentList.Count() > 0)
            {

                List<string> values = new List<string>();
                IEnumerable<string> valuesOfComponent = newComponentList.First().GetValue();
                newComponentList.Remove(newComponentList.First());
                foreach (string valueInList in valuesOfExpression)
                {
                    foreach (string valueOfComponent in valuesOfComponent)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(valueInList);
                        builder.Append(valueOfComponent);
                        values.Add(builder.ToString());
                    }
                }
                return this.ConcatTheValuesOfComponentsWithTheNextExpressions(values, newComponentList);
            }
            else
            {
                return valuesOfExpression;
            }
        }

        private List<LocalVariableComponent> GetCopyComponents()
        {
            return new List<LocalVariableComponent>(this.components);
        }
    }
}
