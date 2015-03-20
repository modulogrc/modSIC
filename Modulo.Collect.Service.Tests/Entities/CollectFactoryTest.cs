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
using Modulo.Collect.Service.Tests.Helpers;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Data;
using Rhino.Mocks;

namespace Modulo.Collect.Service.Tests.Entities
{
    /// <summary>
    /// Summary description for CollectFactoryTest
    /// </summary>
    [TestClass]
    public class CollectFactoryTest
    {
        public CollectFactoryTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

       

        private ProbeResultFactory probeResultFactory;
        private LocalDataProvider dataprovider;
        private IDataProvider provider;

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
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             probeResultFactory = new ProbeResultFactory();
             dataprovider = new LocalDataProvider();
             MockRepository mocks = new MockRepository();

             provider = mocks.StrictMock<IDataProvider>();
             Expect.Call(provider.GetSession()).Repeat.Any().Return(dataprovider.GetSession());                                       
             mocks.ReplayAll();

         }
        
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_build_a_probeExecution_from_collectedObject()
        {
            CollectFactory collectFactory = new CollectFactory(provider.GetSession());
            ProbeResult probeResult = probeResultFactory.CreateProbeResultForRegistryCollect();
            
            ProbeExecution probeExecution = collectFactory.CreateAProbeExecution(probeResult, "registry");
            Assert.IsNotNull(probeExecution);
            Assert.AreEqual("registry", probeExecution.Capability);
            Assert.IsTrue(probeExecution.ExecutionLogs.Count == 8, "the probe execution not have executionLogs expecteds");            
            Assert.IsTrue(probeExecution.IsComplete(), "the probe execution not have a status expected");
        
        }
        
        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_build_a_probeExecution_with_the_error_status_if_exists_problems_in_the_log()
        {
            CollectFactory collectFactory = new CollectFactory(provider.GetSession());
            ProbeResult probeResult = probeResultFactory.CreateProbeResultForRegostryCollectWithError();

            ProbeExecution probeExecution = collectFactory.CreateAProbeExecution(probeResult, "registry");
            Assert.IsNotNull(probeExecution);
            Assert.AreEqual("registry", probeExecution.Capability);
            Assert.IsTrue(probeExecution.ExecutionLogs.Count == probeResult.ExecutionLog.Count(), "the probe execution not have executionLogs expecteds");            
            Assert.IsTrue(probeExecution.HasErrors(), "the probe execution not have a status expected");
        }

        [TestMethod,Owner("lcosta")]
        public void Shoud_be_possible_to_build_a_probeExecution_with_the_error_status_if_not_collect_was_executed()
        {

            CollectFactory collectFactory = new CollectFactory(provider.GetSession());
            ProbeResult probeResult = probeResultFactory.CreateProbeResultForRegostryCollectWithError();

            ProbeExecution probeExecution = collectFactory.CreateAProbeExecutionWithError("registry", "Erro connecting to host");
            Assert.IsNotNull(probeExecution);
            Assert.AreEqual("registry", probeExecution.Capability);
            Assert.IsTrue(probeExecution.ExecutionLogs.Count == 1, "the probe execution not have executionLogs expecteds");            
            Assert.IsTrue(probeExecution.HasErrors(), "the probe execution not have a status expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_build_a_probeExecution_with_error_status_and_execution_logs_if_not_collect_was_executed()
        {
            CollectFactory collectFactory = new CollectFactory(provider.GetSession());
            ProbeResult probeResult = probeResultFactory.CreateProbeResultForRegostryCollectWithError();

            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("registry");
            executionLog.TryConnectToHost("176.16.3.22");
            executionLog.AnErrorOccurred("Error connecting to host");
            executionLog.EndCollect();

            ProbeExecution probeExecution = collectFactory.CreateAProbeExecutionWithError("registry", executionLog.BuildExecutionLogs());
            Assert.IsNotNull(probeExecution);
            Assert.AreEqual("registry", probeExecution.Capability);
            Assert.IsTrue(probeExecution.ExecutionLogs.Count == 4, "the probe execution not have executionLogs expecteds");
            Assert.IsTrue(probeExecution.HasErrors(), "the probe execution not have a status expected");
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_initialize_a_new_Collect_Execution()
        {
            var session = provider.GetSession();
            CollectFactory collectFactory = new CollectFactory(session);
            //ProbeResult probeResult = probeResultFactory.CreateProbeResultForRegostryCollectWithError();
            var request = new CollectRequestFactory().CreateCollectRequest(session).Item2;

            var execution = collectFactory.CreateCollectExecution(session, request);
            Assert.IsNotNull(execution);
            
        }
        
    }
}
