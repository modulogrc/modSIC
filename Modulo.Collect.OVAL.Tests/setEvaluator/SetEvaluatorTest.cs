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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Tests.helpers;
using definitions = Modulo.Collect.OVAL.Definitions;
using sc =Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.setEvaluator;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.setEvaluator.operations;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.OVAL.Tests.setEvaluator
{
    /// <summary>
    /// Summary description for SetEvaluatorTest
    /// </summary>
    [TestClass]
    public class SetEvaluatorTest
    {
        public SetEvaluatorTest()
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
        public void Should_be_possible_to_evaluate_a_Set_in_the_ObjectType()
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            sc.oval_system_characteristics systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");
            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithSet.xml");
            IEnumerable<StateType> states = definitions.states;

            set registryObjectSet = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:6000");

            SetEvaluator setEvaluator = new SetEvaluator(systemCharacteristics,states, null);
            SetResult result = setEvaluator.Evaluate(registryObjectSet);

            Assert.IsNotNull(result, "the items expected is null");
            Assert.AreEqual(3, result.Result.Count(), "the quantity of items is not expected");
            Assert.AreEqual(FlagEnumeration.complete, result.ObjectFlag, "the object flag is not expected");

            string element = result.Result.Where<string>(item => item == "1").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
            element = result.Result.Where<string>(item => item == "2").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
            element = result.Result.Where<string>(item => item == "3").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_evaluate_a_set_in_the_ObjectType_with_Intersection_Operation()
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            sc.oval_system_characteristics systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");
            
            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithSet.xml");
            IEnumerable<StateType> states = definitions.states;

            set registryObjectSet = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:8000");

            SetEvaluator setEvaluator = new SetEvaluator(systemCharacteristics,states, null);
            SetResult result = setEvaluator.Evaluate(registryObjectSet);

            Assert.IsNotNull(result, "the items expected is null");
            Assert.AreEqual(1, result.Result.Count(), "the quantity of items is not expected");
            string element = result.Result.Where<string>(item => item == "2").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_evaluate_a_set_in_the_ObjectType_with_Complement_Operation()
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            sc.oval_system_characteristics systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");

            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithSet.xml");
            IEnumerable<StateType> states = definitions.states;

            set registryObjectSet = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:7000");
            
            SetEvaluator setEvaluator = new SetEvaluator(systemCharacteristics,states,null);
            SetResult result = setEvaluator.Evaluate(registryObjectSet);

            Assert.IsNotNull(result, "the items expected is null");
            Assert.AreEqual(2, result.Result.Count(), "the quantity of items is not expected");

            string element = result.Result.Where<string>(item => item == "1").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
            element = result.Result.Where<string>(item => item == "3").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_evaluate_a_set_with_another_set_defintion()
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            sc.oval_system_characteristics systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");
            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithSet.xml");
            IEnumerable<StateType> states = definitions.states;


            set registryObjectSet = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:5000");

            SetEvaluator setEvaluator = new SetEvaluator(systemCharacteristics,states, null);
            SetResult result = setEvaluator.Evaluate(registryObjectSet);

            Assert.IsNotNull(result, "the items expected is null");
            Assert.AreEqual(1, result.Result.Count(), "the quantity of items is not expected");
            
            string element = result.Result.Where<string>(item => item == "3").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_evaluate_a_set_with_another_set_definitios_with_multiples_levels()
        {

            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            sc.oval_system_characteristics systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");
            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithSet.xml");
            IEnumerable<StateType> states = definitions.states;

            set registryObjectSet = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:9000");


            SetEvaluator setEvaluator = new SetEvaluator(systemCharacteristics,states,null);
            SetResult result  = setEvaluator.Evaluate(registryObjectSet);

            Assert.IsNotNull(result, "the items expected is null");
            Assert.AreEqual(1, result.Result.Count(), "the quantity of items is not expected");

            string element = result.Result.Where<string>(item => item == "3").SingleOrDefault();
            Assert.IsNotNull(element, "the element expected is not exits");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_evaluate_a_set_with_Filter_defined()
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            sc.oval_system_characteristics systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");
            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithSet.xml");

            IEnumerable<StateType> states = definitions.states;
            set registryObjectSet = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:10000");

            SetEvaluator setEvaluator = new SetEvaluator(systemCharacteristics, states,null);
            SetResult result = setEvaluator.Evaluate(registryObjectSet);

            Assert.IsNotNull(result, "the items expected is null");
            Assert.AreEqual(1, result.Result.Count(), "the quantity of items is not expected");

            //string element = result.Result.Where<string>(item => item == "3").SingleOrDefault();
            //Assert.IsNotNull(element, "the element expected is not exits");
        }

    }
}
