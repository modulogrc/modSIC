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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.Registry;
using Modulo.Collect.Probe.Windows.Registry.variables;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass]
    public class RegistryEntityVariableEvaluatorTest
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

        private const string DEFINITIONS_WITH_CONST_VARIABLES = "definitionsWithConstantVariable.xml";
        private const string OBJ_3000_ID = "oval:org.mitre.oval:obj:3000";
        private const string VAR_3000_ID = "oval:org.mitre.oval:var:3000";
        private const string VAR_3000_VALUE_1 = @"Software\Microsoft\Windows NT\CurrentVersion";
        private const string VAR_3000_VALUE_2 = @"Software\Microsoft\Windows NT\CurrentBuild";

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_registryObject_with_his_variables_processed()
        {
            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions(DEFINITIONS_WITH_CONST_VARIABLES);
            registry_object ovalRegistryObject = (registry_object)ProbeHelper.GetOvalComponentByOvalID(definitions, OBJ_3000_ID);
            Assert.IsNotNull(ovalRegistryObject, "the oval registry object is not exists in the fakeDefinitions");

            Dictionary<String, EntityObjectStringType> allRegistryEntities = OvalHelper.GetRegistryEntitiesFromObjectType(ovalRegistryObject);
            string expectedHiveValue = allRegistryEntities[registry_object_ItemsChoices.hive.ToString()].Value;
            string expectedNameValue = allRegistryEntities[registry_object_ItemsChoices.name.ToString()].Value;

            VariablesEvaluated variablesEvaluated = VariableHelper.CreateVariableWithOneValue(OBJ_3000_ID, VAR_3000_ID, VAR_3000_VALUE_1);
            RegistryEntityVariableEvaluator registryEntityVariableEvaluator = new RegistryEntityVariableEvaluator(variablesEvaluated);
            
            
            IEnumerable<RegistryObject> registries =  registryEntityVariableEvaluator.ProcessVariableForRegistryObject(ovalRegistryObject);


            Assert.AreEqual(1, registries.Count(), "the quantity of registries is not expected");
            Assert.AreEqual(expectedHiveValue, registries.ElementAt(0).Hive, "the hive value is not expected");
            Assert.AreEqual(VAR_3000_VALUE_1, registries.ElementAt(0).Key, "the key value is not expected");
            Assert.AreEqual(expectedNameValue, registries.ElementAt(0).Name, "the name value is not expected");            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_registryObject_with_his_variable_processed_with_multiple_values()
        {
            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions(DEFINITIONS_WITH_CONST_VARIABLES);
            registry_object ovalRegistryObject = (registry_object)ProbeHelper.GetOvalComponentByOvalID(definitions, OBJ_3000_ID);
            Assert.IsNotNull(ovalRegistryObject, "the oval registry object is not exists in the fakeDefinitions");

            Dictionary<String, EntityObjectStringType> allRegistryEntities = OvalHelper.GetRegistryEntitiesFromObjectType(ovalRegistryObject);
            string expectedHiveValue = allRegistryEntities[registry_object_ItemsChoices.hive.ToString()].Value;
            string expectedNameValue = allRegistryEntities[registry_object_ItemsChoices.name.ToString()].Value;

            string[] variableValues = new string[] { VAR_3000_VALUE_1, VAR_3000_VALUE_2 };
            VariablesEvaluated variablesEvaluated = VariableHelper.CreateVariableWithMultiplesValue(OBJ_3000_ID, VAR_3000_ID, variableValues);
            RegistryEntityVariableEvaluator registryEntityVariableEvaluator = new RegistryEntityVariableEvaluator(variablesEvaluated);


            IEnumerable<RegistryObject> registries = registryEntityVariableEvaluator.ProcessVariableForRegistryObject(ovalRegistryObject);


            Assert.AreEqual(2, registries.Count(), "the quantity of registries is not expected");
            Assert.AreEqual(expectedHiveValue, registries.ElementAt(0).Hive, "the hive value is not expected");
            Assert.AreEqual(VAR_3000_VALUE_1, registries.ElementAt(0).Key, "the key value is not expected");
            Assert.AreEqual(expectedNameValue, registries.ElementAt(0).Name, "the name value is not expected");

            Assert.AreEqual(expectedHiveValue, registries.ElementAt(1).Hive, "the hive value is not expected");
            Assert.AreEqual(VAR_3000_VALUE_2, registries.ElementAt(1).Key, "the key value is not expected");
            Assert.AreEqual(expectedNameValue, registries.ElementAt(1).Name, "the name value is not expected");            
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_not_be_possible_to_create_a_RegistryEntityVariableEvaluator_with_a_null_VariableEvaluator()
        {
            RegistryEntityVariableEvaluator registryEntityVariableEvaluator = new RegistryEntityVariableEvaluator(null);
        }

    }
}
