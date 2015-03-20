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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common.VariableProbe;
using Rhino.Mocks;
using System;
using Rhino.Mocks.Interfaces;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Common.Test
{
    /// <summary>
    /// Summary description for VariableProberTest
    /// </summary>
    [TestClass]
    public class VariableProberTest
    {
        private TestContext testContextInstance;
        private List<IConnectionProvider> FakeContext;
        private TargetInfo FakeTargetInfo;

        public VariableProberTest()
        {
            FakeContext = ProbeHelper.CreateFakeContext();
            FakeTargetInfo = ProbeHelper.CreateFakeTarget();
        }



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
        public void Should_be_possible_execute_a_variable_collect()
        {
            var definitions = new LoadOvalDocument().GetFakeOvalDefinitions("definitionsWithConstantVariable.xml");
            var objects = definitions.objects.OfType<variable_object>().ToArray();
            var variables = this.CreateVariable("oval:modulo:obj:4000", "oval:modulo:var:1002", new string[] { "c:\\temp" });
            var collectInfo = ProbeHelper.CreateFakeCollectInfo(objects, variables, null);
            collectInfo.Variables = variables;

            var collectedObjects = new VariableProber().Execute(FakeContext, FakeTargetInfo, collectInfo).CollectedObjects;

            Assert.AreEqual(1,collectedObjects.Count());
            Assert.AreEqual(1, collectedObjects.Single().SystemData.Count);
            var collectedItem = (variable_item)collectedObjects.Single().SystemData.First();
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status);
            Assert.AreEqual("c:\\temp", collectedItem.value[0].Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_an_error_occurs_while_creating_items_to_collect_the_created_items_must_have_error_status()
        {
            var definitions = new LoadOvalDocument().GetFakeOvalDefinitions("definitionsWithConstantVariable.xml");
            var objects = definitions.objects.OfType<variable_object>().ToArray();
            var variables = this.CreateVariable("oval:modulo:obj:4000", "oval:modulo:var:1002", new string[] { "c:\\temp" });
            var fakeException = new Exception("Timeout");
            var collectInfo = ProbeHelper.CreateFakeCollectInfo(objects, variables, null);
            collectInfo.Variables = variables;
            var variableProber = this.CreateMockedVariableProber(fakeException);

            var result = variableProber.Execute(FakeContext, FakeTargetInfo, collectInfo);

            // Asserting Collected Objects
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CollectedObjects);
            Assert.AreEqual(1, result.CollectedObjects.Count());

            // Asserting System Data
            var systemData = result.CollectedObjects.Single().SystemData;
            Assert.IsNotNull(systemData);
            Assert.AreEqual(1, systemData.Count);
            
            // Assert Colelcted Item
            var collectedItem = systemData.Single();
            Assert.IsInstanceOfType(collectedItem, typeof(variable_item));
            Assert.AreEqual(StatusEnumeration.error, collectedItem.status);
            Assert.IsNotNull(collectedItem.message);
            Assert.IsTrue(collectedItem.message.First().Value.Contains(fakeException.Message));
        }


        private ProbeBase CreateMockedVariableProber(Exception exceptionToThrow)
        {
            var mocks = new MockRepository();
            
            var fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();
            var fakeItemTypeGenerator = mocks.DynamicMock<IItemTypeGenerator>();
            var fakeObjectCollector = mocks.DynamicMock<VariableObjectCollector>();

            if (exceptionToThrow != null)
            {
                Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null)).IgnoreArguments().Throw(exceptionToThrow);
                Expect.Call(fakeObjectCollector.CollectDataForSystemItem(null)).IgnoreArguments().CallOriginalMethod(OriginalCallOptions.NoExpectation);
            }

            mocks.ReplayAll();



            return new VariableProber()
            {
                ConnectionManager = fakeConnectionManager,
                ObjectCollector = fakeObjectCollector,
                ItemTypeGenerator = fakeItemTypeGenerator
            };
        }

        private VariablesEvaluated CreateVariable(string objectId, string variableId, IEnumerable<string> values)
        {             
            IEnumerable<VariableValue> variables = new List<VariableValue>() { new VariableValue(objectId, variableId, values) };
            
            return new VariablesEvaluated(variables);
        }
    }
}
