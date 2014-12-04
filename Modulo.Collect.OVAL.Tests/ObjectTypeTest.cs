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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Tests.helpers;
using System.Collections;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.OVAL.Tests
{
    /// <summary>
    /// Summary description for ObjectTypeTest
    /// </summary>
    [TestClass]
    public class ObjectTypeTest
    {
        public ObjectTypeTest()
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

        [TestMethod,Owner("lcosta")]
        public void Shoud_be_possible_to_get_EntityBaseTypes_from_an_ObjectType()
        {

            oval_definitions definitions = new OvalDocumentLoader().GetFakeOvalDefinitions("definitionsSimple.xml");
            Assert.AreEqual(definitions.objects.Count(), 2, "the quantity of objectypes in definitions is not expected");

            IEnumerable<EntitySimpleBaseType> entitiesFromRegistryObject = definitions.objects[1].GetEntityBaseTypes();
            Assert.AreEqual(entitiesFromRegistryObject.Count(), 3, "the quantity of entityTypes for registry_Object is not expected");
            Assert.AreEqual(entitiesFromRegistryObject.ElementAt(0).Value, "HKEY_LOCAL_MACHINE");
        }

        [TestMethod,Owner("lcosta")]
        public void Shoud_be_possible_to_get_EntityBaseTypes_from_an_objectType_that_have_only_one_Entity()
        {
            group_object group = new group_object()
            {
                Items = new [] { new EntityObjectStringType() { Value = "Administrators" }}
            };

            IEnumerable<EntitySimpleBaseType> entities = group.GetEntityBaseTypes();
            Assert.AreEqual(entities.Count(), 1, "the quantity of entities is not expected");
            Assert.AreEqual(entities.ElementAt(0).Value, "Administrators");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_EntityBaseType_from_an_object_that_not_have_entities()
        {
            auditeventpolicy_object auditEvent = new auditeventpolicy_object();

            IEnumerable<EntitySimpleBaseType> entities = auditEvent.GetEntityBaseTypes();
            Assert.AreEqual(entities.Count(), 0, "the quantity is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_EntityBaseType_from_an_object_that_have_multiples_items_that_is_not_EntityBaseType()
        {         
            registry_object registry = new registry_object();

            IEnumerable<EntitySimpleBaseType> entities = registry.GetEntityBaseTypes();
            Assert.AreEqual(entities.Count(), 0, "the quantity is no expected");

            registry = new registry_object()
            {
                Items = new[] { new RegistryBehaviors() }
            };

            entities = registry.GetEntityBaseTypes();
            Assert.AreEqual(entities.Count(), 0, "the quantity is no expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_verify_if_an_object_references_a_variable()
        {
            oval_definitions definitions = new OvalDocumentLoader().GetFakeOvalDefinitions("definitionsWithConstantVariable.xml");
            Assert.AreEqual(definitions.objects.Count(), 2, "the quantity of objectypes in definitions is not expected");

            Assert.IsTrue(definitions.objects[1].HasReferenceForVariable("oval:org.mitre.oval:var:3000"), "the object is not have referece fo variable");
            Assert.IsFalse(definitions.objects[0].HasReferenceForVariable("oval:org.mitre.oval:var:3000"), "the object has referece fo variable");

        }
    }
}
