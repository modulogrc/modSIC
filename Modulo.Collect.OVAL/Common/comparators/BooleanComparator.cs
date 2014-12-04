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
using System.Linq;
using System.Collections.Generic;

namespace Modulo.Collect.OVAL.Common.comparators
{
    public class BooleanComparator : IOvalComparator
    {
        private const string INVALID_ENTITY_OPERATION_FOR_BOOLEAN_TYPE = "Invalid Operation {0} for Boolean DataType: The Boolean DataType supports only 'equals' and 'not equals'";
        private const string CONVERT_ERROR_MESSAGE = "Convert Error: The argument {0} informed is not a boolean type. Valid values are: 'true','false', '0' and '1'.";
        
        private OperatorHelper OperatorHelper = new OperatorHelper();
        private Dictionary<String, Boolean> ValidValuesForBooleanType;

        public BooleanComparator()
        {
            this.LoadValidValuesForBooleanType();
        }
    
        public bool Compare(string firstElement, string secondElement, OperationEnumeration operation)
        {
            var firstBooleanElement = this.TryConvertToBoolean(firstElement);
            var secondBooleanElement = this.TryConvertToBoolean(secondElement);

            return ProcessOperation(operation, firstBooleanElement, secondBooleanElement);
        }

        private bool ProcessOperation(OperationEnumeration operation, bool firstBooleanElement, bool secondBooleanElement)
        {
            if (OperatorHelper.IsEqualsOperation(operation))
            {
                return firstBooleanElement == secondBooleanElement;
            }
            else if (OperatorHelper.IsNotEqualsOperation(operation))
            {
                return firstBooleanElement != secondBooleanElement;
            }
            else
            {
                throw new ArgumentException(string.Format(INVALID_ENTITY_OPERATION_FOR_BOOLEAN_TYPE, operation.ToString()));
            }
        }

        private bool TryConvertToBoolean(string stringElement)
        {
            if (stringElement != null)
                stringElement = stringElement.ToLower();

            if (ValidValuesForBooleanType.ContainsKey(stringElement))
            {
                return this.ValidValuesForBooleanType[stringElement];
            }
            else
                throw new ArgumentException(string.Format(CONVERT_ERROR_MESSAGE, stringElement));
        }

        private void LoadValidValuesForBooleanType()
        {
            ValidValuesForBooleanType = new Dictionary<string,bool>();
            ValidValuesForBooleanType.Add("true", true);
            ValidValuesForBooleanType.Add("false", false);
            ValidValuesForBooleanType.Add("0", false);
            ValidValuesForBooleanType.Add("1", true);
            ValidValuesForBooleanType.Add(string.Empty, false);
        }

    }
}
