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
using Modulo.Collect.OVAL.Definitions.variableEvaluator.exceptions;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;

namespace Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions
{
    public class TimeDifferenceFunctionComponent : LocalVariableFunctionComponent
    {
        private TimeDifferenceFunctionType timeDifferenceFunctionType;

        public TimeDifferenceFunctionComponent(TimeDifferenceFunctionType timeDifferenceFunctionType)
        {
            this.timeDifferenceFunctionType = timeDifferenceFunctionType;
        }

        public override IEnumerable<string> GetValue()
        {
            if (this.QuantityOfComponents() > 2)
                throw new FunctionWithMoreComponentsException("The number of components for the TimeDifference must be equals to 2");

            return CalculateDifferenceOfValuesOfFirstComponentWithOtherCompoenents();                
        }

        /// <summary>
        /// Calculates the difference of values of first component with other compoenents.
        /// For each value of first component results, the difference is calculate with results of other components.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> CalculateDifferenceOfValuesOfFirstComponentWithOtherCompoenents()
        {
            List<string> values = new List<string>();
            List<LocalVariableComponent> componentsClone = this.CloneComponentList();
            LocalVariableComponent firstComponent = componentsClone.First();
            IEnumerable<string> valuesOfFirstComponent = firstComponent.GetValue();
            componentsClone.Remove(firstComponent);

            foreach (string value in valuesOfFirstComponent)
            {
                values.AddRange(this.CalculateDifferenceWithOtherResults(value, componentsClone));
            }
            return values;
        }

        private List<string> CalculateDifferenceWithOtherResults(string valueOfFirstComponent, List<LocalVariableComponent> componentsClone)
        {
            List<string> result = new List<string>();            
            int index = 0;
            while (index <= componentsClone.Count - 1)
            {
                IEnumerable<string> valuesOfComponent = componentsClone.ElementAt(index).GetValue();
                foreach (string valueOfOtherComponent in valuesOfComponent)
                {
                    result.Add(this.CalculateDifferenceWithOtherResults(valueOfFirstComponent, valueOfOtherComponent));                   
                }
                index++;
            }
            return result;
        }

        private string CalculateDifferenceWithOtherResults(string valueOfFirstComponent, string valueOfOtherComponent)
        {
            TimeDifferenceFormatter formatter = new TimeDifferenceFormatter();
            DateTime dateOfFirstComponent = formatter.GetDateInFormat(valueOfFirstComponent, this.timeDifferenceFunctionType.format_1);
            DateTime dateOfOtherComponent = formatter.GetDateInFormat(valueOfOtherComponent, this.timeDifferenceFunctionType.format_2);
            TimeSpan difference =  dateOfOtherComponent - dateOfFirstComponent;

            return difference.TotalSeconds.ToString();            
        }

        private List<LocalVariableComponent> CloneComponentList()
        {
            return new List<LocalVariableComponent>(this.components);
        }
    }
}
