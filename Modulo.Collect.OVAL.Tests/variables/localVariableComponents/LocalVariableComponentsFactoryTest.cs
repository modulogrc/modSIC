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
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents
{
    /// <summary>
    /// Summary description for LocalVariableComponentsFactory
    /// </summary>
    [TestClass]
    public class LocalVariableComponentsFactoryTest
    {
        public LocalVariableComponentsFactoryTest()
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
        public void Should_be_possible_to_instantiate_a_ConstantVariableComponent()
        {
            VariablesTypeVariableLocal_variable localVariable = new VariablesTypeVariableLocal_variable() { Item = new ObjectComponentType() };

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(null, null);
            LocalVariableComponent variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(LocalVariableObjectComponent),"the variable component is not expected");
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_instantiate_a_VariableComponent()
        {
            VariablesTypeVariableLocal_variable localVariable = new VariablesTypeVariableLocal_variable() { Item = new VariableComponentType() };

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(null,null);
            LocalVariableComponent variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(LocalVariableVariablesComponent), "the variable component is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_instantiate_a_LiteralComponent()
        {
            VariablesTypeVariableLocal_variable localVariable = new VariablesTypeVariableLocal_variable() { Item = new LiteralComponentType() };

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(null,null);
            LocalVariableComponent variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(LocalVariableLiteralComponent), "the variable component is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_instantiate_a_FunctionComponent()
        {
            ConcatFunctionType concatFunction = new ConcatFunctionType() { Items = new object[] { new ObjectComponentType(), new LiteralComponentType() } };
            VariablesTypeVariableLocal_variable localVariable = new VariablesTypeVariableLocal_variable() { Item = concatFunction };

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(null,null);
            LocalVariableComponent variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(LocalVariableFunctionComponent));
            Assert.IsTrue(((LocalVariableFunctionComponent)variableComponent).QuantityOfComponents() == 2, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_an_ArithmeticFunctionComponent()
        {
            var function = new ArithmeticFunctionType() { Items = new object[] { new ObjectComponentType(), new LiteralComponentType() } };
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(ArithmeticFunctionComponent));
            Assert.IsTrue(((ArithmeticFunctionComponent)variableComponent).QuantityOfComponents() == 2, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_a_BeginFunctionComponent()
        {
            var function = new BeginFunctionType() { Item = new LiteralComponentType() };
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(BeginFunctionComponent));
            Assert.IsTrue(((BeginFunctionComponent)variableComponent).QuantityOfComponents() == 1, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_an_EndFunctionComponent()
        {
            var function = new EndFunctionType() {Item = new LiteralComponentType() };
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(EndFunctionComponent));
            Assert.IsTrue(((EndFunctionComponent)variableComponent).QuantityOfComponents() == 1, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_an_EscapeRegexFunctionComponent()
        {
            var function = new EscapeRegexFunctionType() { Item = new LiteralComponentType() } ;
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            var factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(EscapeRegexFunctionComponent));
            Assert.IsTrue(((EscapeRegexFunctionComponent)variableComponent).QuantityOfComponents() == 1, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_an_RegexCaptureFunctionComponent()
        {
            var function = new RegexCaptureFunctionType() { Item = new LiteralComponentType() } ;
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            var factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(RegexCaptureFunctionComponent));
            Assert.IsTrue(((RegexCaptureFunctionComponent)variableComponent).QuantityOfComponents() == 1, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_an_SplitFunctionComponent()
        {
            var function = new SplitFunctionType() { Item = new LiteralComponentType() } ;
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            var factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(SplitFunctionComponent));
            Assert.IsTrue(((SplitFunctionComponent)variableComponent).QuantityOfComponents() == 1, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_an_SubstringFunctionComponent()
        {
            var function = new SubstringFunctionType() { Item = new LiteralComponentType() } ;
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            var factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(SubStringFunctionComponent));
            Assert.IsTrue(((SubStringFunctionComponent)variableComponent).QuantityOfComponents() == 1, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_an_TimeDifferenceFunctionComponent()
        {
            var function = new TimeDifferenceFunctionType() { Items = new object[] { new ObjectComponentType(), new ObjectComponentType() } };
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            var factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(TimeDifferenceFunctionComponent));
            Assert.IsTrue(((TimeDifferenceFunctionComponent)variableComponent).QuantityOfComponents() == 2, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_an_UniqueFunctionComponent()
        {
            var function = new UniqueFunctionType() { Items = new object[] { new ObjectComponentType(), new ObjectComponentType() } };
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            var factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(UniqueFunctionComponent));
            Assert.IsTrue(((UniqueFunctionComponent)variableComponent).QuantityOfComponents() == 2, "the quantity of component is not expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_instantiate_a_CountFunctionComponent()
        {
            var function = new CountFunctionType() { Items = new object[] { new ObjectComponentType(), new ObjectComponentType() } };
            var localVariable = new VariablesTypeVariableLocal_variable() { Item = function };

            var factory = new LocalVariableComponentsFactory(null, null);
            var variableComponent = factory.GetLocalVariableComponent(localVariable);

            Assert.IsInstanceOfType(variableComponent, typeof(CountFunctionComponent));
            Assert.IsTrue(((CountFunctionComponent)variableComponent).QuantityOfComponents() == 2, "the quantity of component is not expected");
        }
    }
}
