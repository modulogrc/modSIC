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
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.exceptions;
using Modulo.Collect.OVAL.Tests.variables.localVariableComponents.Builders;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.functions
{
    /// <summary>
    /// Summary description for BeginFunctionComponentTest
    /// </summary>
    [TestClass]
    public class BeginFunctionComponentTest
    {
        public BeginFunctionComponentTest()
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

        private oval_definitions definitions;
        private oval_system_characteristics systemCharacteristics;
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_add_the_character_in_the_start_of_expression_if_the_expression_not_start_with_him()
        {

            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithBegin()
                        .WithCharacter("Software")
                        .AddLiteralComponent(@"\Software\Microsoft\Windows NT\CurrentVersion")
                    .SetInLocalVariable()
                .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);                                    
                        
            IEnumerable<string> values = component.GetValue();
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"Software\Software\Microsoft\Windows NT\CurrentVersion", "the value is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_not_add_the_character_in_the_start_of_expression_if_the_expression_start_with_him()
        {
          
            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithBegin()
                        .WithCharacter(@"\Software")
                        .AddLiteralComponent(@"\Software\Microsoft\Windows NT\CurrentVersion")
                    .SetInLocalVariable()
                .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);

            IEnumerable<string> values = component.GetValue();
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"\Software\Microsoft\Windows NT\CurrentVersion", "the value is not expected");
        }

        [TestMethod,Owner("lcosta")]
        [ExpectedException(typeof(FunctionWithMoreComponentsException))]
        public void Should_not_be_possible_to_apply_a_beginFunction_with_multiples_components_defined_in_evaluators()
        {
            
            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithBegin()
                        .WithCharacter(@"\Software")
                        .AddLiteralComponent(@"\Software\Microsoft\Windows NT\CurrentVersion")
                    .SetInLocalVariable()
                .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            //add new component evaluator in beginFunctonComponent
            ((LocalVariableFunctionComponent)component).AddComponent(new ArithmeticFunctionComponent(null));

            IEnumerable<string> values = component.GetValue();
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_apply_a_beginFunction_with_ObjectComponent_defined()
        {
            
            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithBegin()
                        .WithCharacter(@"Software\")
                        .AddObjectComponent("oval:org.mitre.oval:obj:3000", "name")
                    .SetInLocalVariable()
                .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);

            IEnumerable<string> values = component.GetValue();
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"Software\CurrentVersion", "the value is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_apply_a_beginFunction_with_ObjectComponent_with_multiple_values_defined()
        {
            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithBegin()
                        .WithCharacter(@"Software\")
                        .AddObjectComponent("oval:org.mitre.oval:obj:5000", "name")
                    .SetInLocalVariable()
                .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);

            IEnumerable<string> values = component.GetValue();
            Assert.IsTrue(values.Count() == 2, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == @"Software\BuildLabEx", "the value is not expected");
            Assert.IsTrue(values.ElementAt(1) == @"Software\CSDBuildNumber", "the value is not expected");
        }
    }
}
