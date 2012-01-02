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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Tests.variables.localVariableComponents.Builders;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.functions
{
    /// <summary>
    /// Summary description for ArithmeticFunctionComponentTest
    /// </summary>
    [TestClass]
    public class ArithmeticFunctionComponentTest
    {
        public ArithmeticFunctionComponentTest()
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
        public void Should_be_possible_to_get_value_of_arithmeticFunction_with_add_operation()
            
        {

            VariablesTypeVariableLocal_variable localVariableInt = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithArithmetic()
                        .WithArithmeticOperation(ArithmeticEnumeration.add)
                        .AddLiteralComponent(10.ToString(), SimpleDatatypeEnumeration.@int)
                        .AddLiteralComponent(10.ToString(), SimpleDatatypeEnumeration.@int)
                    .SetInLocalVariable()
                .Build();
                                   
            Assert.IsNotNull(definitions, "the definitions was loaded");           
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariableInt);
            Assert.IsInstanceOfType(component, typeof(ArithmeticFunctionComponent));
            IEnumerable<string> values = component.GetValue();
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == 20.ToString() , "the value is not expected");
                        
            VariablesTypeVariableLocal_variable localVariableFloat = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithArithmetic()
                        .WithArithmeticOperation(ArithmeticEnumeration.add)
                        .AddLiteralComponent(10.1.ToString(), SimpleDatatypeEnumeration.@float)
                        .AddLiteralComponent(10.1.ToString(), SimpleDatatypeEnumeration.@float)
                    .SetInLocalVariable()
                .Build();

            component = factory.GetLocalVariableComponent(localVariableFloat);
            Assert.IsInstanceOfType(component, typeof(ArithmeticFunctionComponent));
            values = component.GetValue();
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == 20.2f.ToString(), "the value is not expected");             
        
        }
        
        [TestMethod, Owner("lcosta,imenescal")]
        public void Should_be_possible_to_get_value_of_arithmeticFunction_with_multiply_literalComponent_operation()
        {
            
            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithArithmetic()
                        .WithArithmeticOperation(ArithmeticEnumeration.multiply)
                        .AddLiteralComponent(2.ToString(), SimpleDatatypeEnumeration.@int)
                        .AddLiteralComponent(2.ToString(), SimpleDatatypeEnumeration.@int)
                    .SetInLocalVariable()
                .Build();

            Assert.IsNotNull(definitions, "the definitions was loaded");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(component, typeof(ArithmeticFunctionComponent));
            IEnumerable<string> values = component.GetValue();
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == 4.ToString(), "the value is not expected");

        }

        [Ignore, Owner("lcosta,imenescal")]
        public void Should_be_possible_to_get_value_of_arithmeticFunction_with_multiply_objectComponent_operation()
        {
            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithArithmetic()
                        .WithArithmeticOperation(ArithmeticEnumeration.multiply)
                        .AddObjectComponent("oval:org.mitre.oval:obj:3000", "value")
                        .AddObjectComponent("oval:org.mitre.oval:obj:8000", "value")
                    .SetInLocalVariable()
                .Build();

            Assert.IsNotNull(definitions, "the definitions was loaded");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(component, typeof(ArithmeticFunctionComponent));
            IEnumerable<string> values = component.GetValue();
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt(0) == 24.ToString(), "the value is not expected");
        }

        [TestMethod, Owner("imenescal")]
        public void Should_throws_exception_when_the_value_type_is_not_correct_in_add_operation()
        {
            
            Assert.IsNotNull(definitions, "the definitions was loaded");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");
            
            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithArithmetic()
                        .WithArithmeticOperation(ArithmeticEnumeration.add)
                        .AddLiteralComponent("A", SimpleDatatypeEnumeration.@string)
                        .AddLiteralComponent("B", SimpleDatatypeEnumeration.@string)
                    .SetInLocalVariable()
                .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(component, typeof(ArithmeticFunctionComponent));
            try
            {
                IEnumerable<string> values = component.GetValue();
            }
            catch (Exception exc)
            {
                Assert.IsInstanceOfType(exc, typeof(FormatException), "the exception is not corret in add operator");
            }

        }

        [TestMethod, Owner("imenescal")]
        public void Should_throws_exception_when_the_value_type_is_not_correct_in_multiply_operation()
        {

            Assert.IsNotNull(definitions, "the definitions was loaded");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder
                .CreateTheLocalVariable()
                    .WithArithmetic()
                        .WithArithmeticOperation(ArithmeticEnumeration.multiply)
                        .AddLiteralComponent("A", SimpleDatatypeEnumeration.@string)
                        .AddLiteralComponent("B", SimpleDatatypeEnumeration.@string)
                    .SetInLocalVariable()
                .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(component, typeof(ArithmeticFunctionComponent));            

            try
            {
                IEnumerable<string> values = component.GetValue();
            }
            catch (Exception exc)
            {
                Assert.IsInstanceOfType(exc, typeof(FormatException), "the exception is not corret in multiply operator");
            }

        }

        private VariablesTypeVariableLocal_variable GetLocalVariableForAddOperationWithLiteralComponentTypeInt()
        {
            LiteralComponentType literalComponent1 = new LiteralComponentType() { Value = 10.ToString(),  datatype = SimpleDatatypeEnumeration.@int };
            LiteralComponentType literalComponent2 = new LiteralComponentType() { Value = 10.ToString(), datatype = SimpleDatatypeEnumeration.@int };
            ArithmeticFunctionType arithmeticFunctionType = new ArithmeticFunctionType() { arithmetic_operation = ArithmeticEnumeration.add, Items = new [] { literalComponent1, literalComponent2 } };            
            VariablesTypeVariableLocal_variable localVariable = new VariablesTypeVariableLocal_variable() { Item = arithmeticFunctionType };
            return localVariable;
        }

        private VariablesTypeVariableLocal_variable GetLocalVariableForAddOperationWithLiteralComponentTypeFloat()
        {
            LiteralComponentType literalComponent1 = new LiteralComponentType() { Value = 10.1f.ToString(), datatype = SimpleDatatypeEnumeration.@float };
            LiteralComponentType literalComponent2 = new LiteralComponentType() { Value = 10.1f.ToString(), datatype = SimpleDatatypeEnumeration.@float };
            ArithmeticFunctionType arithmeticFunctionType = new ArithmeticFunctionType() { arithmetic_operation = ArithmeticEnumeration.add, Items = new [] { literalComponent1, literalComponent2 } };

            VariablesTypeVariableLocal_variable localVariable = new VariablesTypeVariableLocal_variable() { Item = arithmeticFunctionType };
            return localVariable;
        }                    

    }
}


