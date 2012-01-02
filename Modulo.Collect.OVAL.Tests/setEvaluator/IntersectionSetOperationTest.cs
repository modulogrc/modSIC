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
using Modulo.Collect.OVAL.Definitions.setEvaluator.operations;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.OVAL.Tests.setEvaluator
{
    /// <summary>
    /// Summary description for IntersectionSetOperationTest
    /// </summary>
    [TestClass]
    public class IntersectionSetOperationTest
    {
        public IntersectionSetOperationTest()
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
        public void Should_be_possible_to_get_an_intersection_between_two_sets()
        {
            IEnumerable<string> firstReferences = new List<string>() { "10", "20", "40","60"};
            IEnumerable<string> secondReferences = new List<string>() { "20", "60" };

            SetOperation operation = new IntersectionSetOperation();
            IEnumerable<string> results = operation.Execute(firstReferences, secondReferences);
            Assert.AreEqual(2, results.Count());

            string element = results.Where<string>(item => item == "20").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
            element = results.Where<string>(item => item == "60").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_an_empty_set_if_not_exists_intersection_between_two_sets()
        {
            IEnumerable<string> firstReferences = new List<string>() { "10", "20", "40", "60" };
            IEnumerable<string> secondReferences = new List<string>() { "180", "80" };

            SetOperation operation = new IntersectionSetOperation();
            IEnumerable<string> results = operation.Execute(firstReferences, secondReferences);
            Assert.AreEqual(0, results.Count());
        }


        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_an_empty_set_if_an_empty_list_was_informed_for_intersection_operation()
        {
            IEnumerable<string> firstReferences = new List<string>() { "10", "20", "40", "60" };
            IEnumerable<string> secondReferences = new List<string>();

            SetOperation operation = new IntersectionSetOperation();
            IEnumerable<string> results = operation.Execute(firstReferences, secondReferences);
            Assert.AreEqual(0, results.Count());
        }
        
        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_define_the_object_flag_based_on_the_objects_of_set_for_the_intersection_operator()
        {
            FlagEnumeration firstObjectFlag = FlagEnumeration.error;
            FlagEnumeration secondObjectFlag = FlagEnumeration.complete;

            SetOperation operation = new IntersectionSetOperation();
            FlagEnumeration result = operation.GetObjectFlag(firstObjectFlag, secondObjectFlag);
            Assert.AreEqual(FlagEnumeration.error, result, "the flag enumaration is not expected");


            firstObjectFlag = FlagEnumeration.doesnotexist;
            secondObjectFlag = FlagEnumeration.notcollected;            
            result = operation.GetObjectFlag(firstObjectFlag, secondObjectFlag);
            Assert.AreEqual(FlagEnumeration.doesnotexist, result, "the flag enumaration is not expected");

        }
    }
}
