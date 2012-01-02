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
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.OVAL.Common.comparators
{
    public class IntegerComparator : IOvalComparator
    {

        private const string INVALID_ENTITY_OPERATION_FOR_INT_TYPE = "Invalid Operation {0} for int DataType: The int DataType supports only 'equals', 'not equals', " +
                                                                        " 'greater than', 'greater than or equals', 'less than', 'less than or equals', 'betwise and' and 'betwise or' ";

        private OperatorHelper operatorHelper = new OperatorHelper();

        public bool Compare(string firstElement, string secondElement, OperationEnumeration operation)
        {
            long firstIntElement = this.TryConvertToInt(firstElement);
            long secondIntElement = this.TryConvertToInt(secondElement);

            return processOperation(operation, firstIntElement, secondIntElement);
                
        }

        private long TryConvertToInt(string stringElement)
        {
            try
            {
                var isEmptyString = stringElement == string.Empty;
                return isEmptyString ? 0 : long.Parse(stringElement);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Convert Error: The argument {0} informed is not a int type. {1}", stringElement, ex.Message));
            }
        }

        private bool processOperation(OperationEnumeration operation, long firstIntElement, long secondIntElement)
        {
            if (this.operatorHelper.IsEqualsOperation(operation))
            {
                return (firstIntElement == secondIntElement);
            }
            else if (this.operatorHelper.IsNotEqualsOperation(operation))
            {
                return (firstIntElement != secondIntElement);
            }
            else if (this.operatorHelper.IsGreaterThanOperation(operation))
            {
                return (firstIntElement > secondIntElement);
            }
            else if (this.operatorHelper.IsGreaterThanOrEqualOperation(operation))
            {
                return (firstIntElement >= secondIntElement);
            }
            else if (this.operatorHelper.IsLessThanOperation(operation))
            {
                return (firstIntElement < secondIntElement);
            }
            else if (this.operatorHelper.IsLessThanOrEqualOperation(operation))
            {
                return (firstIntElement <= secondIntElement);
            }
            else if (this.operatorHelper.IsBitwiseAndOperation(operation))
            {
                return this.processBitwiseAndOperation(firstIntElement,secondIntElement);
            }
            else if (this.operatorHelper.IsBitwiseOrOperation(operation))
            {
                return this.processBitwiseOrOperation(firstIntElement,secondIntElement);
            }
            else                        
            {
                throw new ArgumentException(string.Format(INVALID_ENTITY_OPERATION_FOR_INT_TYPE,operation.ToString()));
            }                        
        }

        private bool processBitwiseOrOperation(long firstIntElement, long secondIntElement)
        {
            long result = firstIntElement | secondIntElement;
            return (result == firstIntElement);
        }

        private bool processBitwiseAndOperation(long firstIntElement, long secondIntElement)
        {
            long result = firstIntElement & secondIntElement;
            return (result == firstIntElement);
        }        
        
    }
}
