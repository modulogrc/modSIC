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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Tests.helpers;

namespace Modulo.Collect.OVAL.Tests
{
    /// <summary>
    /// Summary description for SetTest
    /// </summary>
    [TestClass]
    public class SetTest
    {
        public SetTest()
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
        public void Should_be_possible_to_verify_if_a_set_has_another_set()
        {
            set setElement = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:5000");
            Assert.AreEqual(true,setElement.ExistsAnotherSetElement(), "the set element not has another set");

            setElement = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:6000");
            Assert.AreEqual(false, setElement.ExistsAnotherSetElement(), "the set element has another set");
        }

        
        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_an_item_value()
        {
            set setElement = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:5000");
            object itemValue = setElement.GetItemValue(set_items.set);
            Assert.IsNotNull(itemValue,"item value not found in set element");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_an_object_references_from_a_set()
        {
            set setElement = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:7000");
            IEnumerable<string> objectReferences = setElement.GetObjectReferences();
            Assert.AreEqual(2, objectReferences.Count(), "the number of objectReferences is not expected");

            Object firstObject = objectReferences.Where(obj => obj == "oval:org.mitre.oval:obj:4000").SingleOrDefault();
            Object secondObject = objectReferences.Where(obj => obj == "oval:org.mitre.oval:obj:2000").SingleOrDefault();

            Assert.IsNotNull(firstObject, "the object expected is not found");
            Assert.IsNotNull(secondObject, "the object expected is not found");          

        }

        [TestMethod,Owner("lcosta")]        
        public void Should_be_possible_to_get_child_set_references()
        {
            set setElement = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:5000");
            IEnumerable<set> objectReferences = setElement.GetSets();
            Assert.AreEqual(2, objectReferences.Count(), "the number of sets is not expected");

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_verify_if_a_set_has_filter_element()
        {
            set setElement = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:10000");
            Assert.IsTrue(setElement.HasFilterElement(), "the set element is not has a filter element");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_value_of_filter_element_in_a_set()
        {
            set setElement = SetFactory.GetSetFromDefinitionsOfRegistryObject("definitionsWithSet.xml", "oval:org.mitre.oval:obj:10000");
            string filterValue = setElement.GetFilterValue();
            Assert.AreEqual("oval:org.mitre.oval:ste:99", filterValue);
        }


    }
}
