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
using sc = Modulo.Collect.OVAL.SystemCharacteristics;
using def = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.setEvaluator.Filter;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.OVAL.Tests.setEvaluator
{
    /// <summary>
    /// Summary description for FilterEvaluatorTest
    /// </summary>
    [TestClass]
    public class FilterEvaluatorTest
    {
        public FilterEvaluatorTest()
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


        private oval_system_characteristics systemCharacteristics;
        private IEnumerable<StateType> states;

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
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            this.systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");
            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithSet.xml");
            this.states = definitions.states;
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_apply_a_filter_in_ObjectTypeList_based_on_state()
        {   
            string[] idsOfObjectTypes = new string[] { "oval:org.mitre.oval:obj:2000","oval:org.mitre.oval:obj:5000" };
            IEnumerable<sc.ObjectType> objectTypes = this.GetObjectTypesForTest(idsOfObjectTypes, systemCharacteristics);            
            FilterEvaluator filterEvaluator = new FilterEvaluator(systemCharacteristics,states,null);
            IEnumerable<sc.ObjectType> objectTypesAfterFilter = filterEvaluator.ApplyFilter(objectTypes, "oval:org.mitre.oval:ste:99"); 
            Assert.IsNotNull(objectTypesAfterFilter);
            Assert.AreEqual(1,objectTypesAfterFilter.Count(),"the quantity of objects is not expected");            
        }

        [TestMethod, Owner("lcosta")]
        public void Must_not_apply_filter_for_objectTypes_that_not_match_with_state_specification_of_filter()
        {
            string[] idsOfObjectTypes = new string[] { "oval:org.mitre.oval:obj:2000", "oval:org.mitre.oval:obj:3000" };
            IEnumerable<sc.ObjectType> objectTypes = this.GetObjectTypesForTest(idsOfObjectTypes, systemCharacteristics);
            FilterEvaluator filterEvaluator = new FilterEvaluator(systemCharacteristics, states,null);
            IEnumerable<sc.ObjectType> objectTypesAfterFilter = filterEvaluator.ApplyFilter(objectTypes, "oval:org.mitre.oval:ste:99");
            Assert.IsNotNull(objectTypesAfterFilter);
            Assert.AreEqual(2, objectTypesAfterFilter.Count(), "the quantity of objects is not expected");
        }

        private IEnumerable<sc.ObjectType> GetObjectTypesForTest(string[] idsOfObjectTypes, oval_system_characteristics systemCharacteristics)
        {
            List<sc.ObjectType> objectTypes = new List<sc.ObjectType>();
            foreach(var id in idsOfObjectTypes)
            {
                objectTypes.Add( systemCharacteristics.GetCollectedObjectByID(id) );
            }

            return objectTypes;
        }
    }
}
