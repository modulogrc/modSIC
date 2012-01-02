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
using Modulo.Collect.Probe.Windows.Registry;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test
{
    /// <summary>
    /// Summary description for RegistryObjectFactoryTest
    /// </summary>
    [TestClass]
    public class RegistryObjectFactoryTest
    {
        public RegistryObjectFactoryTest()
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
        private oval_definitions definitions;

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
         //Use TestInitialize to run code before running each test 
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             definitions = ProbeHelper.GetFakeOvalDefinitions("definitionsWithConstantVariable.xml");
         }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_create_a_registryObject_by_the_combination_of_entities()
        {
            List<string> hives = new List<string>() { "HKEY_LOCAL_MACHINE" };
            List<string> keys = new List<string>() { @"Software\Microsoft\Windows" };
            List<string> names = new List<string>() { "CurrentVersion" };

            IEnumerable<RegistryObject> registryObjects = RegistryObjectFactory.CreateRegistryObjectsByCombinationOfEntitiesFrom(hives, keys, names,this.GetRegistryObject());
            Assert.IsTrue(registryObjects.Count() == 1, "the quantity of registryObjects is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Hive, hives.ElementAt(0), "the hive is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Key, keys.ElementAt(0), "the key is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).GetVariableId(registry_object_ItemsChoices.key.ToString()), "oval:org.mitre.oval:var:3000", "the variable Id is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Name, names.ElementAt(0), "the name is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).GetOperationOfEntity(registry_object_ItemsChoices.name.ToString()), OperationEnumeration.equals, "the name operation is not expected");

            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_create_a_registryObject_by_the_combination_of_multiples_entities()
        {
            List<string> hives = new List<string>() { "HKEY_LOCAL_MACHINE" };
            List<string> keys = new List<string>() { @"Software\Microsoft\Windows", @"Software\Microsoft\Windows NT" };
            List<string> names = new List<string>() { "CurrentVersion" };

            IEnumerable<RegistryObject> registryObjects = RegistryObjectFactory.CreateRegistryObjectsByCombinationOfEntitiesFrom(hives, keys, names, this.GetRegistryObject());
            Assert.IsTrue(registryObjects.Count() == 2, "the quantity of registryObjects is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Hive, hives.ElementAt(0), "the hive is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Key, keys.ElementAt(0), "the key is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Name, names.ElementAt(0), "the name is not expected");
            Assert.AreEqual(registryObjects.ElementAt(1).Hive, hives.ElementAt(0), "the hive is not expected");
            Assert.AreEqual(registryObjects.ElementAt(1).Key, keys.ElementAt(1), "the key is not expected");
            Assert.AreEqual(registryObjects.ElementAt(1).Name, names.ElementAt(0), "the name is not expected");

            hives = new List<string>() { "HKEY_LOCAL_MACHINE" };
            keys = new List<string>() { @"Software\Microsoft\Windows", @"Software\Microsoft\Windows NT" };
            names = new List<string>() { "CurrentVersion", "CurrentBuild" };

            registryObjects = RegistryObjectFactory.CreateRegistryObjectsByCombinationOfEntitiesFrom(hives, keys, names, this.GetRegistryObject());
            Assert.IsTrue(registryObjects.Count() == 4, "the quantity of registryObjects is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Hive, hives.ElementAt(0), "the hive is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Key, keys.ElementAt(0), "the key is not expected");
            Assert.AreEqual(registryObjects.ElementAt(0).Name, names.ElementAt(0), "the name is not expected");
            Assert.AreEqual(registryObjects.ElementAt(1).Hive, hives.ElementAt(0), "the hive is not expected");
            Assert.AreEqual(registryObjects.ElementAt(1).Key, keys.ElementAt(0), "the key is not expected");
            Assert.AreEqual(registryObjects.ElementAt(1).Name, names.ElementAt(1), "the name is not expected");
            Assert.AreEqual(registryObjects.ElementAt(2).Hive, hives.ElementAt(0), "the hive is not expected");
            Assert.AreEqual(registryObjects.ElementAt(2).Key, keys.ElementAt(1), "the key is not expected");
            Assert.AreEqual(registryObjects.ElementAt(2).Name, names.ElementAt(0), "the name is not expected");
            Assert.AreEqual(registryObjects.ElementAt(3).Hive, hives.ElementAt(0), "the hive is not expected");
            Assert.AreEqual(registryObjects.ElementAt(3).Key, keys.ElementAt(1), "the key is not expected");
            Assert.AreEqual(registryObjects.ElementAt(3).Name, names.ElementAt(1), "the name is not expected");            

        }

        private RegistryObject GetRegistryObject()
        {            
            registry_object ovalRegistryObject = (registry_object)ProbeHelper.GetOvalComponentByOvalID(definitions, "oval:org.mitre.oval:obj:3000");
            RegistryObject registry = RegistryObjectFactory.CreateRegistryObject(ovalRegistryObject);
            return registry;
        }
    }
}
