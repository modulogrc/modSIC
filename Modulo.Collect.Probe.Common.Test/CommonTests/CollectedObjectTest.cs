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
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Common.Test
{
    [TestClass]
    public class CollectedObjectTest
    {

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
        public void Should_be_possible_to_add_reference_for_variables_in_the_ObjectType()
        {
            List<string> variableValues = new List<string>() { "Multiprocessor Free" };
            VariableValue variable = new VariableValue("oval:org.mitre.oval:obj:6000", "oval:com.hp:var:1", variableValues);
            IEnumerable<VariableValue> variables = new List<VariableValue>() { variable };

            CollectedObject collectedObject = new CollectedObject("oval:org.mitre.oval:obj:6000");
            collectedObject.AddVariableReference(variables);
            Assert.IsNotNull(collectedObject.ObjectType.variable_value, "the variables was not set");
            Assert.IsTrue(collectedObject.ObjectType.variable_value.Count() == 1, "the quantity of the variable_value is not expected");
            Assert.AreEqual(collectedObject.ObjectType.variable_value[0].Value, "Multiprocessor Free","the value is not expected");            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_an_objectType_with_status_not_collected()
        {
            CollectedObject collectObject = new CollectedObject("oval:org.mitre.oval:obj:6000");
            Assert.AreEqual(Modulo.Collect.OVAL.SystemCharacteristics.FlagEnumeration.notcollected,collectObject.ObjectType.flag, "the objectType not has the expected flag");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_relation_between_objectType_and_systemData_through_of_referenceData()
        {
            CollectedObject collectObject = new CollectedObject("oval:org.mitre.oval:obj:6000");
            ItemType registryItem1 = new registry_item() { status = StatusEnumeration.exists, id = "1" };
            ItemType registryItem2 = new registry_item() { status = StatusEnumeration.doesnotexist, id = "2" };
            collectObject.AddItemToSystemData(registryItem1);
            collectObject.AddItemToSystemData(registryItem2);

            Assert.AreEqual(2, collectObject.ObjectType.reference.Count(), "the quantity of reference is not expected");
            Assert.AreEqual("1", collectObject.ObjectType.reference[0].item_ref, "the first element of reference not has the id expected");
            Assert.AreEqual("2", collectObject.ObjectType.reference[1].item_ref, "the second element of reference not has the id expected");

            CollectedObject otherCollectedObject = new CollectedObject("oval:org.mitre.oval:obj:6001");
            ItemType registryItem3 = new registry_item() { status = StatusEnumeration.exists, id = "3" };
            otherCollectedObject.AddItemToSystemData(registryItem3);

            Assert.AreEqual(1, otherCollectedObject.ObjectType.reference.Count(), "the quantity of reference is not expected for the second collectedObject");
            Assert.AreEqual("3", otherCollectedObject.ObjectType.reference[0].item_ref, "the referece id of element is not expected");

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_update_status_of_an_objectType_to_doesnotexists_based_on_in_systemData()
        {
            CollectedObject collectObject = new CollectedObject("oval:org.mitre.oval:obj:6000");
            ItemType registryItem1 = new registry_item() { status = StatusEnumeration.exists, id = "1" };
            ItemType registryItem2 = new registry_item() { status = StatusEnumeration.doesnotexist, id = "2" };
            collectObject.AddItemToSystemData(registryItem1);
            collectObject.AddItemToSystemData(registryItem2);

            collectObject.UpdateCollectedObjectStatus();

            Assert.AreEqual(FlagEnumeration.doesnotexist, collectObject.ObjectType.flag);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_update_status_of_an_objectType_to_error_based_on_in_systemData()
        {
            CollectedObject collectObject = new CollectedObject("oval:org.mitre.oval:obj:6000");
            ItemType registryItem1 = new registry_item() { status = StatusEnumeration.exists, id = "1" };
            ItemType registryItem2 = new registry_item() { status = StatusEnumeration.doesnotexist, id = "2" };
            ItemType registryItem3 = new registry_item() { status = StatusEnumeration.error, id = "3" };
            collectObject.AddItemToSystemData(registryItem1);
            collectObject.AddItemToSystemData(registryItem2);
            collectObject.AddItemToSystemData(registryItem3);

            collectObject.UpdateCollectedObjectStatus();

            Assert.AreEqual(FlagEnumeration.error, collectObject.ObjectType.flag);

        }

        public void Should_not_possible_to_add_a_item_type_if_it_already_exists_in_the_collected_object()
        {
            oval_system_characteristics systemCharacteristics = new LoadOvalDocument().GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            CollectedObject collectedObject = new CollectedObject("oval:org.mitre.oval:obj:1000");
            ItemType registryItem1 = new registry_item() { status = StatusEnumeration.exists, id = "1" };
            ItemType registryItem2 = systemCharacteristics.GetSystemDataByReferenceId("2");


        }

       
    }
}
