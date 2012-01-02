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
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Common;
using System.Globalization;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.exceptions;

namespace Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions
{
    public class ArithmeticFunctionComponent : LocalVariableFunctionComponent
    {

        private ArithmeticFunctionType arithmeticFunctionType;        

        public ArithmeticFunctionComponent(ArithmeticFunctionType arithmeticFunctionType)
        {
            this.arithmeticFunctionType = arithmeticFunctionType;          
        }

        public override IEnumerable<string> GetValue()
        {
            List<string> valuesOfComponents = this.GetListValues();
            List<string> values = new List<string>();

            if (this.IsSumOperator())            
                values.Add(this.SumValuesOfComponents(valuesOfComponents).ToString());            
            else if (IsMultiplyOperator())            
                values.Add(this.MultiplyValuesOfComponents(valuesOfComponents).ToString());
            
            return values;
            
        }

        private bool IsMultiplyOperator()
        {
            return arithmeticFunctionType.arithmetic_operation == ArithmeticEnumeration.multiply;
        }

        private bool IsSumOperator()
        {
            return arithmeticFunctionType.arithmetic_operation == ArithmeticEnumeration.add;
        }

        private double MultiplyValuesOfComponents(List<string> valuesOfComponents)
        {
            double resultOfMultply = 1;

            foreach (string value in valuesOfComponents)
            {
                double doubleValue = ConverValueToDouble(value);
                resultOfMultply = resultOfMultply * doubleValue;
            }
            return resultOfMultply;
        }

        private double SumValuesOfComponents(List<string> valuesOfComponents)
        {
            double resultOfSum = 0;

            foreach (string value in valuesOfComponents)
            {
                double doubleValue = ConverValueToDouble(value);
                resultOfSum += doubleValue;
            }
            return resultOfSum;
        }

        private double ConverValueToDouble(string value)
        {
            double doubleValue = 0;

            try
            {
                doubleValue = double.Parse(value);
            }
            catch (FormatException)
            {
                throw new FormatException();
            }

            return doubleValue;
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
