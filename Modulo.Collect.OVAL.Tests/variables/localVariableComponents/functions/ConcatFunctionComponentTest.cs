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
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.variables.localVariableComponents.Builders;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.functions
{
    /// <summary>
    /// Summary description for ConcatFunctionComponentTest
    /// </summary>
    [TestClass]
    public class ConcatFunctionComponentTest
    {
        public ConcatFunctionComponentTest()
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

        private OvalDocumentLoader ovalDocument;   
         [TestInitialize()]
         public void MyTestInitialize() {
             this.ovalDocument = new OvalDocumentLoader();
         }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_add_other_components_in_the_function()
        {
            ConcatFunctionComponent functionComponent = new ConcatFunctionComponent(null,null,null);            
            LocalVariableObjectComponent objectComponent = new LocalVariableObjectComponent(null, null);
            functionComponent.AddComponent(objectComponent);
            Assert.IsTrue(functionComponent.QuantityOfComponents() == 1, "the quantity of component is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_value_of_concatFunction_with_ObjectComponent_and_LiteralComponent()
        {                      

            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                CreateTheLocalVariable().
                    WithConcat().
                        AddLiteralComponent(@"c:\").
                        AddObjectComponent("oval:org.mitre.oval:obj:1000", "family").
                    SetInLocalVariable().
                Build();

            oval_definitions definitions = this.ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            Assert.IsNotNull(definitions, "the definitions was loaded");

            oval_system_characteristics systemCharacteristics = this.ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            Assert.IsNotNull(systemCharacteristics,"the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics,definitions.variables);

            LocalVariableComponent concatFunctionComponent = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(concatFunctionComponent, typeof(ConcatFunctionComponent));
            IEnumerable<string> values = concatFunctionComponent.GetValue();
            Assert.IsTrue(values.Count() > 0, "the quantity is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"c:\windows", "the value is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_value_of_concatFunction_with_two_values_of_return()
        {

            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                CreateTheLocalVariable().
                    WithConcat().
                        AddObjectComponent("oval:org.mitre.oval:obj:5000", "name").
                        AddLiteralComponent(@"\").                        
                    SetInLocalVariable().
                Build();

            oval_definitions definitions = this.ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            Assert.IsNotNull(definitions, "the definitions was loaded");

            oval_system_characteristics systemCharacteristics = this.ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);

            LocalVariableComponent concatFunctionComponent = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(concatFunctionComponent, typeof(ConcatFunctionComponent));
            IEnumerable<string> values = concatFunctionComponent.GetValue();
            Assert.IsTrue(values.Count() == 2, "the quantity is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"BuildLabEx\", "the value is not expected");
            Assert.IsTrue(values.ElementAt(1) == @"CSDBuildNumber\", "the value is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_value_of_concatFunction_with_three_values_of_return()
        {

            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                CreateTheLocalVariable().
                    WithConcat().
                        AddObjectComponent("oval:org.mitre.oval:obj:5000", "name").
                        AddLiteralComponent(@"\").
                        AddObjectComponent("oval:org.mitre.oval:obj:6000", "name").
                     SetInLocalVariable().
                 Build();

            oval_definitions definitions = this.ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            Assert.IsNotNull(definitions, "the definitions was loaded");

            oval_system_characteristics systemCharacteristics = this.ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);

            LocalVariableComponent concatFunctionComponent = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(concatFunctionComponent, typeof(ConcatFunctionComponent));
            IEnumerable<string> values = concatFunctionComponent.GetValue();
            Assert.IsTrue(values.Count() == 4, "the quantity is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"BuildLabEx\SoftwareType", "the value is not expected");
            Assert.IsTrue(values.ElementAt(1) == @"BuildLabEx\SystemRoot", "the value is not expected");            
            Assert.IsTrue(values.ElementAt(2) == @"CSDBuildNumber\SoftwareType", "the value is not expected");
            Assert.IsTrue(values.ElementAt(3) == @"CSDBuildNumber\SystemRoot", "the value is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_value_of_concatFunction_with_multiples_values_of_return()
        {
            
            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                CreateTheLocalVariable().
                    WithConcat().
                        AddObjectComponent("oval:org.mitre.oval:obj:5000", "name").
                        AddLiteralComponent(@"\").
                        AddObjectComponent("oval:org.mitre.oval:obj:6000", "name").
                        AddLiteralComponent(@"\").
                        AddObjectComponent("oval:org.mitre.oval:obj:7000", "name").
                    SetInLocalVariable().
                 Build();

            oval_definitions definitions = this.ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            Assert.IsNotNull(definitions, "the definitions was loaded");

            oval_system_characteristics systemCharacteristics = this.ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);

            LocalVariableComponent concatFunctionComponent = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(concatFunctionComponent, typeof(ConcatFunctionComponent));
            IEnumerable<string> values = concatFunctionComponent.GetValue();
            Assert.IsTrue(values.Count() == 4, "the quantity is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"BuildLabEx\SoftwareType\InstallDate", "the value is not expected");
            Assert.IsTrue(values.ElementAt(1) == @"BuildLabEx\SystemRoot\InstallDate", "the value is not expected");
            Assert.IsTrue(values.ElementAt(2) == @"CSDBuildNumber\SoftwareType\InstallDate", "the value is not expected");
            Assert.IsTrue(values.ElementAt(3) == @"CSDBuildNumber\SystemRoot\InstallDate", "the value is not expected");
        }



        
    }
}
