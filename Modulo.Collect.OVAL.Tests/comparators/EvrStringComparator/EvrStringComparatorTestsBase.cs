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

using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;

namespace Modulo.Collect.OVAL.Tests.comparators.EvrStringComparatorTests
{
    public class EvrStringComparatorTestsBase
    {
        protected const string UNEXPECTED_COMPARISION_RESULT = "Unexpected comparision result was found.";
        
        protected EvrStringComparatorChecker MakeSureThatThe;

        public EvrStringComparatorTestsBase(OperationEnumeration ovalOperation)
        {
            switch (ovalOperation)
            {
                case OperationEnumeration.equals:
                    this.MakeSureThatThe = new EvrStringComparatorChecker().Using().EqualsOperation();
                    break;
                case OperationEnumeration.notequal:
                    this.MakeSureThatThe = new EvrStringComparatorChecker().Using().NotEqualOperation();
                    break;
                case OperationEnumeration.greaterthan:
                    this.MakeSureThatThe = new EvrStringComparatorChecker().Using().GreaterThanOperation();
                    break;
                case OperationEnumeration.lessthan:
                    this.MakeSureThatThe = new EvrStringComparatorChecker().Using().LessThanOperation();
                    break;
                case OperationEnumeration.greaterthanorequal:
                    this.MakeSureThatThe = new EvrStringComparatorChecker().Using().GreaterOrEqualThanOperation();
                    break;
                case OperationEnumeration.lessthanorequal:
                    this.MakeSureThatThe = new EvrStringComparatorChecker().Using().LessOrEqualThanOperation();
                    break;
                default:
                    throw new InvalidOvalOperationException();
            }
        }

    }
}
