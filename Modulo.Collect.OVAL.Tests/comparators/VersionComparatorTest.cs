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
    /// Summary description for VersionComparatorTest
    /// </summary>
    [TestClass]
    public class VersionComparatorTest
    {
        public VersionComparatorTest()
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
        public void Should_be_possible_to_compare_two_elements_with_version_dataType_with_equals_operator()
        {
            VersionComparator comparator = new VersionComparator();

            bool compareResult = comparator.Compare("5.7.23", "5.7.23", OperationEnumeration.equals);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5.7.21", "5.7.23", OperationEnumeration.equals);
            Assert.IsFalse(compareResult, "the value is not expected for equals operation");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_elements_with_version_dataType_not_equals_operator()
        {
            VersionComparator comparator = new VersionComparator();

            bool compareResult = comparator.Compare("5.7.21", "5.7.23", OperationEnumeration.notequal);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5.7.23", "5.7.23", OperationEnumeration.notequal);
            Assert.IsFalse(compareResult, "the value is not expected for equals operation");            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_elements_with_version_dataType_with_less_than_operator()
        {
            VersionComparator comparator = new VersionComparator();

            bool compareResult = comparator.Compare("5.7.21", "5.8.0", OperationEnumeration.lessthan);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5.7.21", "5.7.22", OperationEnumeration.lessthan);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("4.0.0", "5.8.0", OperationEnumeration.lessthan);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_compare_two_elements_with_version_dataType_with_greater_than_operator()
        {
            VersionComparator comparator = new VersionComparator();

            bool compareResult = comparator.Compare("5.8.0", "5.7.23", OperationEnumeration.greaterthan);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5.7.22", "5.7.21", OperationEnumeration.greaterthan);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5.0.0", "4.0.0", OperationEnumeration.greaterthan);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_elements_with_version_dataType_with_greater_than_or_equals_operator()
        {
            VersionComparator comparator = new VersionComparator();

            bool compareResult = comparator.Compare("5.7.22", "5.7.22", OperationEnumeration.greaterthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5.8.0", "5.7.23", OperationEnumeration.greaterthanorequal);
             Assert.IsTrue(compareResult, "the value is not expected for equals operation");


             compareResult = comparator.Compare("5.0.0", "5.0.0", OperationEnumeration.greaterthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_two_elements_with_version_dataType_with_less_than_or_equals_operator()
        {
            VersionComparator comparator = new VersionComparator();

            bool compareResult = comparator.Compare("5.7.22", "5.7.22", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5.6.1", "5.6.2", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");


            compareResult = comparator.Compare("5.0.0", "5.0.0", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");        
        }

        [TestMethod, Owner("lcosta")]
        public void Should_complete_with_zero_the_version_number_when_is_not_complete()
        {
            VersionComparator comparator = new VersionComparator();

            bool compareResult = comparator.Compare("5.7", "5.7.22", OperationEnumeration.lessthanorequal);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5.7.8", "5", OperationEnumeration.greaterthan);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_define_any_separator_for_version_number()
        {
            VersionComparator comparator = new VersionComparator();

            bool compareResult = comparator.Compare("5#7#8", "5#7#8", OperationEnumeration.equals);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

            compareResult = comparator.Compare("5#7#8", "5%7%8", OperationEnumeration.equals);
            Assert.IsTrue(compareResult, "the value is not expected for equals operation");

        }
    }
}
