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
using Modulo.Collect.OVAL.Common.comparators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Modulo.Collect.OVAL.Tests.comparators.EvrStringComparatorTests
{
    public class EvrStringComparatorChecker
    {
        private const string UNEXPECTED_COMPARISION_RESULT = "Unexpected comparision result was found.";

        private OperationEnumeration Operation;
        private string FirstElement;
        private string SecondElement;

        public EvrStringComparatorChecker()
        {
            this.Operation = OperationEnumeration.equals;
        }

        public EvrStringComparatorChecker ComparingEvrString(String firstEvrString)
        {
            this.FirstElement = firstEvrString;
            return this;
        }

        public EvrStringComparatorChecker To(String secondEvrString)
        {
            this.SecondElement = secondEvrString;
            return this;
        }

        public EvrStringComparatorChecker Using()
        {
            return this;
        }

        public EvrStringComparatorChecker EqualsOperation()
        {
            this.Operation = OperationEnumeration.equals;
            return this;
        }

        public EvrStringComparatorChecker NotEqualOperation()
        {
            this.Operation = OperationEnumeration.notequal;
            return this;
        }

        public EvrStringComparatorChecker GreaterThanOperation()
        {
            this.Operation = OperationEnumeration.greaterthan;
            return this;
        }

        public EvrStringComparatorChecker GreaterOrEqualThanOperation()
        {
            this.Operation = OperationEnumeration.greaterthanorequal;
            return this;
        }

        public EvrStringComparatorChecker LessThanOperation()
        {
            this.Operation = OperationEnumeration.lessthan;
            return this;
        }

        public EvrStringComparatorChecker LessOrEqualThanOperation()
        {
            this.Operation = OperationEnumeration.lessthanorequal;
            return this;
        }

        public void IsTrue()
        {
            var comparisionResult = ExecuteEvrCompator();
            Assert.IsTrue(comparisionResult);
        }

        public void IsFalse()
        {
            var comparisionResult = ExecuteEvrCompator();
            Assert.IsFalse(comparisionResult);
        }

        private bool ExecuteEvrCompator()
        {
            return new EvrStringComparator().Compare(FirstElement, SecondElement, Operation);
        }
    }
}
