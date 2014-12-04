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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;

namespace Modulo.Collect.OVAL.Tests.comparators
{
    /// <summary>
    /// Summary description for FloatComparatorTest
    /// </summary>
    [TestClass]
    public class FloatComparatorTest
    {
        public FloatComparatorTest()
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
        public void Should_be_possible_to_compare_two_floats_using_equals_operator()
        {
            FloatComparator comparator = new FloatComparator();

            bool compareResult = comparator.Compare("55.89", "55.89", OperationEnumeration.equals);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("10", "10", OperationEnumeration.equals);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("55.89", "55.88", OperationEnumeration.equals);
            Assert.IsFalse(compareResult, "the value is not expected for equals operation");           
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_floats_using_not_equals_operator()
        {
            FloatComparator comparator = new FloatComparator();

            bool compareResult = comparator.Compare("0.5", "0.55", OperationEnumeration.notequal);
            Assert.IsTrue(compareResult, "the value is not expected for not equals operation");

            compareResult = comparator.Compare("10", "11", OperationEnumeration.notequal);
            Assert.IsTrue(compareResult, "the value is not expected for not equals operation");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_floats_using_greater_than_operator()
        {
            FloatComparator comparator = new FloatComparator();

            bool compareResult = comparator.Compare("0.33", "0.10", OperationEnumeration.greaterthan);
            Assert.IsTrue(compareResult, "the value is not expected for greater than operation");

            compareResult = comparator.Compare("55", "12", OperationEnumeration.greaterthan);
            Assert.IsTrue(compareResult, "the value is not expected for greater than operation");

            compareResult = comparator.Compare("0.111", "0.9", OperationEnumeration.greaterthan);
            Assert.IsFalse(compareResult, "the value is not expected for greater than operation");
        }

        [TestMethod, Owner("lcosta")]        
        public void Should_be_possible_to_compare_two_floats_using_greather_than_or_equals_operator()
        {
            FloatComparator comparator = new FloatComparator();

            bool compareResult = comparator.Compare("0.33", "0.10", OperationEnumeration.greaterthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for greater than or equals operation");

            compareResult = comparator.Compare("0.33", "0.33", OperationEnumeration.greaterthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for greater than or equals operation");

            compareResult = comparator.Compare("0.1113", "0.1113", OperationEnumeration.greaterthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for greater than or equals operation");           
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_floats_using_less_than_operator()
        {
            FloatComparator comparator = new FloatComparator();

            bool compareResult = comparator.Compare("0.33", "0.99", OperationEnumeration.lessthan);
            Assert.IsTrue(compareResult, "the value is not expected for less than operation");

            compareResult = comparator.Compare("50", "180", OperationEnumeration.lessthan);
            Assert.IsTrue(compareResult, "the value is not expected for less than operation");

            // error in convertion case of 0.850. It convert to 850.0
            compareResult = comparator.Compare("0.85", "180", OperationEnumeration.lessthan);
            Assert.IsTrue(compareResult, "the value is not expected for less than operation");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_floats_using_less_than_or_equals_operator()
        {
            FloatComparator comparator = new FloatComparator();

            bool compareResult = comparator.Compare("0.33", "0.33", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for less than operation");

            compareResult = comparator.Compare("50", "180", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for less than operation");

            compareResult = comparator.Compare("0.550", "0.550", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for less than operation");
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_not_be_possible_two_compare_elements_that_is_not_float_with_any_operator()
        {
            FloatComparator comparator = new FloatComparator();

            bool compareResult = comparator.Compare("Is not a Float", "15", OperationEnumeration.greaterthanorequal);
            Assert.IsTrue(compareResult, "the value is not a float number");            
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_not_possible_to_compare_elements_with_null_value()
        {
            FloatComparator comparator = new FloatComparator();
            bool compareResult = comparator.Compare("29", null, OperationEnumeration.equals);
            Assert.IsFalse(compareResult, "the value is not expected for this operation");

        }

    }
}
