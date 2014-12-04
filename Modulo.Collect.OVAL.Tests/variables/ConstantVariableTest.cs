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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators;


namespace Modulo.Collect.OVAL.Tests.Variables
{
    /// <summary>
    /// Summary description for ConstantVariableTest
    /// </summary>
    [TestClass]
    public class ConstantVariableTest
    {
        private const string KEY_OF_REGISTRY = @"Software\Microsoft\Windows NT";
        private const string KEY_OF_REGISTRY_WINDOWS = @"Software\Microsoft\Windows";
        private const string KEY_OF_REGISTRY_WINDOWS_TEMP = @"Software\Microsoft\Windows\Temp";
        public ConstantVariableTest()
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
        public void Should_be_possible_to_get_value_of_a_constant_variable()
        {
            VariablesTypeVariableConstant_variable variable = this.CreateAConstantVariableWithASingleValue();

            ConstantVariableEvaluator constantVariable = new ConstantVariableEvaluator(variable);
            IEnumerable<string> values = constantVariable.GetValue();
            Assert.IsNotNull(values, "the valueToMatch of variable is null");
            Assert.IsTrue(values.Count() == 1, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt<string>(0).Equals(KEY_OF_REGISTRY), "the valueToMatch is not expected");    
            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_list_of_value_of_a_constant_variable()
        {
            VariablesTypeVariableConstant_variable variable = this.CreateAConstantVariableWithAListOfValues();

            ConstantVariableEvaluator constantVariable = new ConstantVariableEvaluator(variable);
            IEnumerable<string> values = constantVariable.GetValue();
            Assert.IsNotNull(values, "the valueToMatch of variable is null");
            Assert.IsTrue(values.Count() == 3, "the quantity of values is not expected");
            Assert.IsTrue(values.ElementAt<string>(0).Equals(KEY_OF_REGISTRY), "the valueToMatch is not expected");
            Assert.IsTrue(values.ElementAt<string>(1).Equals(KEY_OF_REGISTRY_WINDOWS), "the valueToMatch is not expected");
            Assert.IsTrue(values.ElementAt<string>(2).Equals(KEY_OF_REGISTRY_WINDOWS_TEMP), "the valueToMatch is not expected");    
        }

        private VariablesTypeVariableConstant_variable CreateAConstantVariableWithAListOfValues()
        {
            VariablesTypeVariableConstant_variable variable = new VariablesTypeVariableConstant_variable();
            variable.id = "oval:org.mitre.oval:var:932";
            variable.datatype = SimpleDatatypeEnumeration.@string;
            variable.version = "1";
            Modulo.Collect.OVAL.Definitions.ValueType[] values = new Modulo.Collect.OVAL.Definitions.ValueType[3];
            Modulo.Collect.OVAL.Definitions.ValueType value1 = new Modulo.Collect.OVAL.Definitions.ValueType();
            Modulo.Collect.OVAL.Definitions.ValueType value2 = new Modulo.Collect.OVAL.Definitions.ValueType();
            Modulo.Collect.OVAL.Definitions.ValueType value3 = new Modulo.Collect.OVAL.Definitions.ValueType();
            value1.Value = KEY_OF_REGISTRY;
            value2.Value = KEY_OF_REGISTRY_WINDOWS;
            value3.Value = KEY_OF_REGISTRY_WINDOWS_TEMP;
            values[0] = value1;
            values[1] = value2;
            values[2] = value3;
            variable.value = values; 
            return variable;
        }

        private VariablesTypeVariableConstant_variable CreateAConstantVariableWithASingleValue()
        {

            VariablesTypeVariableConstant_variable variable = new VariablesTypeVariableConstant_variable();
            variable.id = "oval:org.mitre.oval:var:932";
            variable.datatype = SimpleDatatypeEnumeration.@string;
            variable.version = "1";
            Modulo.Collect.OVAL.Definitions.ValueType[] values = new Modulo.Collect.OVAL.Definitions.ValueType[1];
            Modulo.Collect.OVAL.Definitions.ValueType value = new Modulo.Collect.OVAL.Definitions.ValueType();
            values[0] = value;
            variable.value = values;
            value.Value = KEY_OF_REGISTRY;
            return variable;
        }
    }
}
