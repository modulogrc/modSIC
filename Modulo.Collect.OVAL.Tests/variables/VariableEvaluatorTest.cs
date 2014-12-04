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
using Modulo.Collect.OVAL.SystemCharacteristics;

using Modulo.Collect.OVAL.Definitions.VariableEvaluators;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.OVAL.Tests.Variables
{
    /// <summary>
    /// Summary description for VariableEvaluatorTest
    /// </summary>
    [TestClass]
    public class VariableEvaluatorTest
    {
        public VariableEvaluatorTest()
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
        public void Should_be_possible_to_evaluate_a_constant_variables_of_an_entitytype()
        {
            oval_definitions definitions = new OvalDocumentLoader().GetFakeOvalDefinitions("oval_definitions.oval.org.mitre.oval.def.5921.xml");
            Assert.IsNotNull(definitions, "the definitios is not created");

            EntityObjectStringType entityType = new EntityObjectStringType();
            entityType.var_ref = "oval:org.mitre.oval:var:932";

            VariableEvaluator variableEvaluator = new VariableEvaluator(definitions.variables, null, null);
            IEnumerable<string> values = variableEvaluator.Evaluate(entityType);
            Assert.IsNotNull(values, "the valueToMatch of variable is null");
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == "1", "the valueToMatch is not expected");           
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_evaluate_a_variable_given_variableID()
        {
            oval_definitions definitions = new OvalDocumentLoader().GetFakeOvalDefinitions("oval_definitions.oval.org.mitre.oval.def.5921.xml");
            Assert.IsNotNull(definitions, "the definitios is not created");

            VariableEvaluator variableEvaluator = new VariableEvaluator(definitions.variables, null, null);
            IEnumerable<string> values = variableEvaluator.EvaluateVariable("oval:org.mitre.oval:var:932");
            Assert.IsNotNull(values, "the valueToMatch of variable is null");
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == "1", "the valueToMatch is not expected");                       
        }

        [TestMethod, Owner("lcosta")]
        public void Should_not_evaluate_a_variable_if_the_entityType_not_have_a_variable_reference()
        {
            oval_definitions definitions = new OvalDocumentLoader().GetFakeOvalDefinitions("definitionsSimple.xml");
            Assert.IsNotNull(definitions, "the definitios is not created");

            registry_object registryObject = (registry_object)definitions.objects[1];

            Dictionary<String, EntityObjectStringType> registryEntities = this.GetRegistryEntitiesFromName(registryObject);
            EntitySimpleBaseType keyName = registryEntities[registry_object_ItemsChoices.key.ToString()];

            VariableEvaluator variableEvaluator = new VariableEvaluator(definitions.variables,null, null);
            IEnumerable<string> values = variableEvaluator.Evaluate(keyName);
            Assert.IsNotNull(values, "the valueToMatch of variable is null");
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"Software\Microsoft\Windows NT\CurrentVersion", "the valueToMatch is not expected");    
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_evaluate_a_local_Variables_of_an_entityType()
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            Assert.IsNotNull(definitions, "the definitios is not created");

            oval_system_characteristics systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            Assert.IsNotNull(definitions, "the system Characteristics is not was created");

            EntityObjectStringType entityType = new EntityObjectStringType();
            entityType.var_ref = "oval:org.mitre.oval:var:4000";

            VariableEvaluator variableEvaluator = new VariableEvaluator(definitions.variables, systemCharacteristics, null);
            IEnumerable<string> values = variableEvaluator.Evaluate(entityType);
            Assert.IsNotNull(values, "the valueToMatch of variable is null");
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"Software\Microsoft\Windows NT\CurrentVersion", "the value is not expected");      
        }

        private Dictionary<string, EntityObjectStringType> GetRegistryEntitiesFromName(registry_object registryObject)
        {
            string hiveEntityName = registry_object_ItemsChoices.hive.ToString();
            string keyEntityName = registry_object_ItemsChoices.key.ToString();
            string nameEntityName = registry_object_ItemsChoices.name.ToString();

            object[] allEntities = registryObject.Items.ToArray();
            string[] allEntityNames = registryObject.RegistryObjectItemsElementName.Select(i => i.ToString()).ToArray<String>();

            Dictionary<String, EntityObjectStringType> registryEntities = new Dictionary<String, EntityObjectStringType>();
            registryEntities.Add(hiveEntityName, this.GetEntityObjectByName(hiveEntityName, allEntities, allEntityNames));
            registryEntities.Add(keyEntityName, this.GetEntityObjectByName(keyEntityName, allEntities, allEntityNames));
            registryEntities.Add(nameEntityName, this.GetEntityObjectByName(nameEntityName, allEntities, allEntityNames));

            return registryEntities;
        }

        private EntityObjectStringType GetEntityObjectByName(string entityName, object[] allEntities, string[] allEntityNames)
        {
            for (int i = 0; i < allEntityNames.Length; i++)
                if (entityName.Equals(allEntityNames[i].ToString()))
                    return (allEntities[i] as EntityObjectStringType);

            return null;

        }
    }
}
