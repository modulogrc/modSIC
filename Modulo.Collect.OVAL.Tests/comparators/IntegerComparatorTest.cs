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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;

namespace Modulo.Collect.OVAL.Tests.comparators
{
    /// <summary>
    /// Summary description for IntegerComparatorTestcs
    /// </summary>
    [TestClass]
    public class IntegerComparatorTest
    {
        public IntegerComparatorTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_integers_using_equals_operator()
        {
            IntegerComparator comparator = new IntegerComparator();

            bool compareResult = comparator.Compare("10", "10", OperationEnumeration.equals);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("180", "1536", OperationEnumeration.equals);
            Assert.IsFalse(compareResult, "the value is not expected for equals operation");
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possibe_to_compare_two_integers_using_not_equals_operator()
        {
            IntegerComparator comparator = new IntegerComparator();

            bool compareResult = comparator.Compare("15899", "525", OperationEnumeration.notequal);
            Assert.IsTrue(compareResult, "the value is not expected for not equals operation");

            compareResult = comparator.Compare("18", "18", OperationEnumeration.notequal);
            Assert.IsFalse(compareResult, "the value is not expected for not equals operation");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_integers_using_greater_than_operator()
        {
            IntegerComparator comparator = new IntegerComparator();

            bool compareResult = comparator.Compare("181", "180", OperationEnumeration.greaterthan);
            Assert.IsTrue(compareResult, "the value is not expected for greater than operation");

            compareResult = comparator.Compare("8", "18", OperationEnumeration.greaterthan);
            Assert.IsFalse(compareResult, "the value is not expected for greater than operation");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_integers_using_greater_than_or_equals_operator()
        {
            IntegerComparator comparator = new IntegerComparator();

            bool compareResult = comparator.Compare("181", "180", OperationEnumeration.greaterthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for greater than or equal operation");

            compareResult = comparator.Compare("8", "8", OperationEnumeration.greaterthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for greater than or equal operation");
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_compare_two_integers_using_less_than_operator()
        {
            IntegerComparator comparator = new IntegerComparator();

            bool compareResult = comparator.Compare("9", "10", OperationEnumeration.lessthan);
            Assert.IsTrue(compareResult, "the value is not expected for less than operation");

            compareResult = comparator.Compare("1589", "18", OperationEnumeration.lessthan);
            Assert.IsFalse(compareResult, "the value is not expected for less than operation");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_integers_using_less_than_or_equals_operator()
        {
            IntegerComparator comparator = new IntegerComparator();

            bool compareResult = comparator.Compare("9", "10", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for less than equal operation");

            compareResult = comparator.Compare("96", "96", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for less than or equal operation");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_integers_using_bitwise_and_operator()
        {
            IntegerComparator comparator = new IntegerComparator();

            bool compareResult = comparator.Compare("6", "4", OperationEnumeration.bitwiseand);
            Assert.IsTrue(compareResult, "the value is not expected for less than equal operation");            
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_compare_two_integers_using_bitwise_and_operator_2()
        {
            var comparisionResult = new IntegerComparator().Compare("519", "2", OperationEnumeration.bitwiseand);
            
            Assert.IsTrue(comparisionResult);
        }


        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_integers_using_bitwise_or_operator()
        {
            IntegerComparator comparator = new IntegerComparator();

            bool compareResult = comparator.Compare("14", "6", OperationEnumeration.bitwiseor);
            Assert.IsTrue(compareResult, "the value is not expected for less than equal operation");

            compareResult = comparator.Compare("14", "1", OperationEnumeration.bitwiseor);
            Assert.IsFalse(compareResult, "the value is not expected for less than equal operation");            
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_not_possible_to_compare_elements_that_is_not_integer_with_any_operator()
        {
            IntegerComparator comparator = new IntegerComparator();
            bool compareResult = comparator.Compare("Is not Integer", "10", OperationEnumeration.lessthan);                       
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_not_possible_to_compare_elements_with_null_value()
        {
            IntegerComparator comparator = new IntegerComparator();
            bool compareResult = comparator.Compare("180", null, OperationEnumeration.greaterthan);                       
        }
    }
}
