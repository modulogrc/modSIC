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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;

namespace Modulo.Collect.OVAL.Tests.comparators
{
    /// <summary>
    /// Summary description for BooleanComparatorTest
    /// </summary>
    [TestClass]
    public class BooleanComparatorTest
    {
        private BooleanComparator BooleanComparator;
        private OperationEnumeration EQUALS = OperationEnumeration.equals;
        private OperationEnumeration NOT_EQUAL = OperationEnumeration.notequal;
        private string TRUE = "true";
        private string FALSE = "false";

        private string COMPARISON_RESULT_NOT_EXPECTED = "The result is not expected for the equals operation.";

        public BooleanComparatorTest()
        {
            this.BooleanComparator = new BooleanComparator();
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_booleans_using_equal_operator()
        {
            Assert.IsTrue(BooleanComparator.Compare(TRUE, TRUE, EQUALS), COMPARISON_RESULT_NOT_EXPECTED);
            Assert.IsTrue(BooleanComparator.Compare(FALSE, FALSE, EQUALS), COMPARISON_RESULT_NOT_EXPECTED);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_booleans_using_equal_operator_with_other_literal_for_boolean_representation()
        {
            Assert.IsTrue(BooleanComparator.Compare("1", "1", EQUALS), COMPARISON_RESULT_NOT_EXPECTED);
            Assert.IsTrue(BooleanComparator.Compare("0", "0", EQUALS), COMPARISON_RESULT_NOT_EXPECTED);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_booleans_using_not_equal_operator()
        {
            Assert.IsTrue(BooleanComparator.Compare(TRUE, FALSE, NOT_EQUAL), COMPARISON_RESULT_NOT_EXPECTED);
            Assert.IsFalse(BooleanComparator.Compare(FALSE, FALSE, NOT_EQUAL), COMPARISON_RESULT_NOT_EXPECTED);
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_compare_two_booleans_using_not_equals_operator_with_other_literal_for_boolean_respresentation()
        {
            Assert.IsTrue(BooleanComparator.Compare("1", "0", NOT_EQUAL), COMPARISON_RESULT_NOT_EXPECTED);
            Assert.IsFalse(BooleanComparator.Compare("0", "0", NOT_EQUAL), COMPARISON_RESULT_NOT_EXPECTED);
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_not_be_possible_to_compare_two_elements_with_not_valid_literal_representation_for_boolean()
        {
            BooleanComparator.Compare("v", "f", OperationEnumeration.equals);
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_be_not_possible_to_compare_two_elements_with_null_values()
        {
            BooleanComparator.Compare("1", null, OperationEnumeration.equals);
        }

        [TestMethod, Owner("lfernandes")]
        public void The_boolean_comparison_must_be_case_insensitive()
        {
            Assert.IsTrue(BooleanComparator.Compare("true", "TRUE", OperationEnumeration.equals));
            Assert.IsTrue(BooleanComparator.Compare("FALSE", "fAlSe", OperationEnumeration.equals));
        }
    }
}
