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
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.variables.localVariableComponents.Builders;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.functions
{
    /// <summary>
    /// Summary description for SplitFunctionComponentTest
    /// </summary>
    [TestClass]
    public class SplitFunctionComponentTest
    {
        public SplitFunctionComponentTest()
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
         public void MyTestInitialize() {
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
        
         [TestMethod, Owner("imenescal")]
         public void Should_be_possible_to_get_value_of_splitFunction_with_LiteralComponent()
         {
             VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                CreateTheLocalVariable().
                    WithSplit().
                        WithDelimeter(",").
                        AddLiteralComponent("1,2,3").
                        SetInLocalVariable().
                Build();

             Assert.IsNotNull(definitions, "the definitions was loaded");
             Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");
             LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
             LocalVariableComponent localVariableComponent = factory.GetLocalVariableComponent(localVariable);
             IEnumerable<string> values = localVariableComponent.GetValue();
             Assert.IsTrue((values.Count() == 3), "the quantity is not expected");
             Assert.IsTrue(values.ElementAt(0).ToString() == "1", "the value of first element is not expected");
             Assert.IsTrue(values.ElementAt(1).ToString() == "2", "the value of second element is not expected");
             Assert.IsTrue(values.ElementAt(2).ToString() == "3", "the value of third element is not expected");
        }
        
         [TestMethod, Owner("imenescal")]
         public void Should_be_possible_to_get_value_of_splitFunction_with_ObjectComponent()
         {
             VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                 CreateTheLocalVariable().
                    WithSplit().
                        WithDelimeter(".").
                        AddObjectComponent("oval:org.mitre.oval:obj:8000", "value").
                        SetInLocalVariable().
                 Build();

             Assert.IsNotNull(definitions, "the definitions was loaded");
             Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");
             LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
             LocalVariableComponent localVariableComponent = factory.GetLocalVariableComponent(localVariable);
             IEnumerable<string> values = localVariableComponent.GetValue();
             Assert.IsTrue((values.Count() == 2), "the quantity is not expected");
             Assert.IsTrue(values.ElementAt(0).ToString() == "4", "the value of first element is not expected");
             Assert.IsTrue(values.ElementAt(1).ToString() == "0", "the value of second element is not expected");
         }

         [TestMethod, Owner("imenescal")]
         public void Shoud_be_possible_to_get_value_of_splitFunction_with_ObjectComponent_with_two_values()
         {

             VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                 CreateTheLocalVariable().
                     WithSplit().
                         WithDelimeter(".").
                         AddObjectComponent("oval:org.mitre.oval:obj:5000", "value").
                         SetInLocalVariable().
                 Build();  

            Assert.IsNotNull(definitions, "the definitions was loaded");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");
            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent localVariableComponent = factory.GetLocalVariableComponent(localVariable);
            IEnumerable<string> values = localVariableComponent.GetValue();
            Assert.IsTrue((values.Count() == 6), "the quantity is not expected");
            Assert.IsTrue(values.ElementAt(0).ToString() == "6001", "the value of first element is not expected");
            Assert.IsTrue(values.ElementAt(1).ToString() == "18000", "the value of second element is not expected");
            Assert.IsTrue(values.ElementAt(2).ToString() == "x86fre", "the value of third element is not expected");
            Assert.IsTrue(values.ElementAt(3).ToString() == "longhorn_rtm", "the value of fourth element is not expected");
            Assert.IsTrue(values.ElementAt(4).ToString() == "080118-1840", "the value of fifth element is not expected");
            Assert.IsTrue(values.ElementAt(5).ToString() == "1616", "the value of sixth element is not expected");

         }
         
    }
}
