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

namespace Modulo.Collect.OVAL.Definitions.EntityOperations
{
    public class OvalEntityOperationFactory
    {
        private static readonly string INVALID_ENTITY_OPERATION_ERROR_MESSAGE = "Unknown entity operation: '{0}'";
        private static readonly string NOT_IMPLEMENTED_ENTITY_OPERATION_ERROR_MESSAGE = "There is no implementation for this OVAL entity operation: '{0}'";


        private static OvalEntityOperationBase resolveOvalEntityOperation(OperationEnumeration operation)
        {
            switch (operation)
            {
                case OperationEnumeration.equals:
                    return new EqualsEntityOperation();
                case OperationEnumeration.notequal:
                    return new NotEqualsEntityOperation();
                case OperationEnumeration.caseinsensitiveequals:
                    return new CaseInsentiveEqualsOperation();
                case OperationEnumeration.caseinsensitivenotequal:
                    return new CaseInsensitiveNotEqualsOperation();
                case OperationEnumeration.greaterthan:
                    throw new NotImplementedException(string.Format(NOT_IMPLEMENTED_ENTITY_OPERATION_ERROR_MESSAGE, operation.ToString()));
                case OperationEnumeration.lessthan:
                    throw new NotImplementedException(string.Format(NOT_IMPLEMENTED_ENTITY_OPERATION_ERROR_MESSAGE, operation.ToString()));
                case OperationEnumeration.greaterthanorequal:
                    throw new NotImplementedException(string.Format(NOT_IMPLEMENTED_ENTITY_OPERATION_ERROR_MESSAGE, operation.ToString()));
                case OperationEnumeration.lessthanorequal:
                    throw new NotImplementedException(string.Format(NOT_IMPLEMENTED_ENTITY_OPERATION_ERROR_MESSAGE, operation.ToString()));
                case OperationEnumeration.bitwiseand:
                    throw new NotImplementedException(string.Format(NOT_IMPLEMENTED_ENTITY_OPERATION_ERROR_MESSAGE, operation.ToString()));
                case OperationEnumeration.bitwiseor:
                    throw new NotImplementedException(string.Format(NOT_IMPLEMENTED_ENTITY_OPERATION_ERROR_MESSAGE, operation.ToString()));
                case OperationEnumeration.patternmatch:
                    return new PatternMatchEntityOperation();
                default:
                    throw new NotImplementedException(string.Format(INVALID_ENTITY_OPERATION_ERROR_MESSAGE, operation.ToString()));
            }
        }
       
        public static OvalEntityOperationBase CreateOperationForOvalEntity(OperationEnumeration operation)
        {
            OvalEntityOperationBase newEntityOperation = resolveOvalEntityOperation(operation);
            return newEntityOperation;
        }

       
    }
}
