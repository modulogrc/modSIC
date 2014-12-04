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
using System.Text.RegularExpressions;

namespace Modulo.Collect.OVAL.Common.comparators
{
    public class VersionComparator : IOvalComparator
    {
        private IntegerComparator integerComparator = new IntegerComparator();
        private OperatorHelper operatorHelper = new OperatorHelper();
    
        public bool Compare(string firstElement, string secondElement, OperationEnumeration operation)
        {
            string[] firstNumbers = Regex.Split(firstElement, "[^0-9]+");
            string[] secondNumbers = Regex.Split(secondElement, "[^0-9]+");
            firstNumbers = this.CompleteArrayWithZeros(firstNumbers, secondNumbers);
            secondNumbers = this.CompleteArrayWithZeros(secondNumbers, firstNumbers);

            return this.ProcessOperation(firstNumbers, secondNumbers, operation);
        }

        private string[] CompleteArrayWithZeros(string[] firstNumbers, string[] secondNumbers)
        {
            List<string> modifiedArray = new List<string>(firstNumbers);
            if (firstNumbers.Count() < secondNumbers.Count())
            {
                int difference = secondNumbers.Count() - firstNumbers.Count();
                for (int i = 0; i < difference; i++)
                    modifiedArray.Add("0");
            }
            return modifiedArray.ToArray();
        }

        private bool ProcessOperation(string[] firstNumbers, string[] secondNumbers, OperationEnumeration operation)
        {

            if (operatorHelper.IsEqualsOperation(operation))
            {
                return this.ProcessEqualsOperation(firstNumbers, secondNumbers, operation);
            }
            else if (operatorHelper.IsNotEqualsOperation(operation))
            {
                return !this.ProcessEqualsOperation(firstNumbers, secondNumbers, OperationEnumeration.equals);
            }
            else if (operatorHelper.IsLessThanOperation(operation))
            {
                return this.ProcessLessThanAndGreaterThanOperation(firstNumbers, secondNumbers, operation);
            }
            else if (operatorHelper.IsGreaterThanOperation(operation))
            {
                return this.ProcessLessThanAndGreaterThanOperation(firstNumbers, secondNumbers, operation);
            }
            else if (operatorHelper.IsLessThanOrEqualOperation(operation))
            {
                return this.ProcessLessThanAndGreaterThanOperation(firstNumbers, secondNumbers, operation);
            }
            else if (operatorHelper.IsGreaterThanOrEqualOperation(operation))
            {
                return this.ProcessLessThanAndGreaterThanOperation(firstNumbers, secondNumbers, operation);
            }
            
            return true;


        }

        private bool ProcessEqualsOperation(string[] firstNumbers, string[] secondNumbers, OperationEnumeration operation)
        {
            for (int i = 0; i < firstNumbers.Length; i++)
            {                
                if (!integerComparator.Compare(firstNumbers[i], secondNumbers[i], operation))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ProcessLessThanAndGreaterThanOperation(string[] firstNumbers, string[] secondNumbers, OperationEnumeration operation)
        {            
            for (int i = 0; i < firstNumbers.Length; i++)
            {
                if (integerComparator.Compare(firstNumbers[i], secondNumbers[i], operation))
                {
                    return true;
                }
                else
                {
                    if (!integerComparator.Compare(firstNumbers[i], secondNumbers[i], OperationEnumeration.equals))
                        return false;

                }                
            }
            if (operatorHelper.IsGreaterThanOrEqualOperation(operation) || operatorHelper.IsLessThanOrEqualOperation(operation))
            {
                return this.ProcessLessThanOrEqualAndGreatherThanOrEqualOperation(firstNumbers, secondNumbers, operation);
               
            }
            return false;            
        }


        private bool ProcessLessThanOrEqualAndGreatherThanOrEqualOperation(string[] firstNumbers, string[] secondNumbers, OperationEnumeration operation)
        {
            string firstLastElement = firstNumbers.Last();
            string secondLastElement = secondNumbers.Last();
            return integerComparator.Compare(firstLastElement, secondLastElement, operation);

        }

        

     
    }
}
