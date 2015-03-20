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
using Modulo.Collect.OVAL.Common;
using System.Text.RegularExpressions;
using Modulo.Collect.OVAL.Common.comparators;

namespace Modulo.Collect.OVAL.Common.comparators
{
    public class StringComparator : IOvalComparator
    {
        private const string INVALID_ENTITY_OPERATION_FOR_STRING_TYPE = "Invalid Operation {0} for String DataType: The string DataType supports only 'equals', 'not equals', " +
                                                                        " 'case insensitive equals', 'case insensitive not equals' and pattern match";

        private OperatorHelper operatorHelper = new OperatorHelper();

        /// <summary>
        /// It compares a given value against a found value in target system using Oval operations.
        /// </summary>
        /// <param name="firstElement">Found Value.</param>
        /// <param name="secondElement">Expected value (entity value).</param>
        /// <param name="operation">Entity Operation.</param>
        /// <returns>The comparison result.</returns>
        public bool Compare(string firstElement, string secondElement, OperationEnumeration operation)
        {
            if (this.IsEqualsOperation(operation))
            {
                return OvalEntityComparer.IsEntityValuesEquals(operation,firstElement,secondElement);
            }
            else if (this.IsNotEqualsOperation(operation))
            {
                return OvalEntityComparer.IsEntityValuesNotEqual(operation, firstElement, secondElement);
            }
            else if (this.IsRegularExpression(operation))
            {
                return this.processRegex(firstElement, secondElement);
            }
            else
            {
                throw new ArgumentException(string.Format(INVALID_ENTITY_OPERATION_FOR_STRING_TYPE,operation.ToString()));
            }
        }

        private bool processRegex(string firstElement, string secondElement)
        {
            return Regex.IsMatch(firstElement, secondElement, RegexOptions.IgnoreCase);
        }
     
        private bool  IsEqualsOperation(OperationEnumeration operation)
        {
            return this.operatorHelper.IsEqualsOperation(operation);
        }
        
        private bool IsNotEqualsOperation(OperationEnumeration operation)
        {
            return this.operatorHelper.IsNotEqualsOperation(operation);
        }

        private bool IsRegularExpression(OperationEnumeration operation)
        {
            return this.operatorHelper.IsRegularExpression(operation);
        }
    }
}
