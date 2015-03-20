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
using Modulo.Collect.Probe.Common.BasicClasses;

namespace Modulo.Collect.Probe.Common.Test
{
    [TestClass]
    public class ExecutionLogBuilderTest
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

        [TestMethod, Owner("lcosta")]
        public void Shoud_be_possible_to_build_an_execution_log_in_steps()
        {
            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("Registry");
            executionLog.TryConnectToHost("176.16.3.166");
            executionLog.CollectingInformationFrom("oval:id:7589");
            executionLog.EndCollect();
            IEnumerable<ProbeLogItem> executionsLog = executionLog.BuildExecutionLogs();
            Assert.AreEqual(4, executionsLog.Count(), "the quantity of execution logs is not expected");        
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_build_an_execution_log_in_steps_with_custom_errors_logs()
        {
            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("Registry");
            executionLog.TryConnectToHost("176.16.3.166");
            
            executionLog.AnErrorOccurred("Erro trying connect to host 176.16.3.166");
            
            executionLog.CollectingInformationFrom("oval:id:7589");            
            executionLog.EndCollect();
            IEnumerable<ProbeLogItem> executionsLog = executionLog.BuildExecutionLogs();
            Assert.AreEqual(5, executionsLog.Count());
            Assert.AreEqual(TypeItemLog.Error, executionsLog.ElementAt(2).Type);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_build_an_execution_log_in_steps_with_custom_warning_logs()
        {
            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("Registry");
            executionLog.TryConnectToHost("176.16.3.166");
            executionLog.ConnectedWithSuccess();                       
            executionLog.CollectingInformationFrom("oval:id:7589");            
            executionLog.CollectingDataFrom("Key: HKEY_LOCAL_MACHINE\\software\\microsoft\\windows\\currentVersion\\BuildType");

            executionLog.Warning("The key of registry item is not exists");

            executionLog.EndCollect();
            IEnumerable<ProbeLogItem> executionsLog = executionLog.BuildExecutionLogs();
            Assert.AreEqual(7, executionsLog.Count());
            Assert.AreEqual(TypeItemLog.Warning, executionsLog.ElementAt(5).Type, "the type of log is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void should_be_possible_to_buid_an_execution_log_in_steps_informing_what_element_is_collected()
        {
            string element = "Key: HKEY_LOCAL_MACHINE\\software\\microsoft\\windows\\currentVersion\\BuildType";
            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("Registry");
            executionLog.TryConnectToHost("176.16.3.166");
            executionLog.ConnectedWithSuccess();
            executionLog.CollectingInformationFrom("oval:id:7858");
            
            executionLog.CollectingDataFrom(element);            
            
            executionLog.EndCollect();

            IEnumerable<ProbeLogItem> executionsLog = executionLog.BuildExecutionLogs();

            Assert.AreEqual(6, executionsLog.Count());           

        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_add_a_detail_information_in_the_log()
        {
            ExecutionLogBuilder executionLogDetailBuilder = new ExecutionLogBuilder();
            executionLogDetailBuilder.CollectingDataFrom("Key: HKEY_LOCAL_MACHINE\\software\\microsoft\\windows\\currentVersion\\BuildType");
            executionLogDetailBuilder.CollectingDataFrom("Key: HKEY_LOCAL_MACHINE\\software\\microsoft\\windows\\currentVersion\\PathName");
            IEnumerable<ProbeLogItem> executionLogDetail = executionLogDetailBuilder.BuildExecutionLogs();

            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("Registry");
            executionLog.TryConnectToHost("176.16.3.166");

            executionLog.AddDetailInformation(executionLogDetail); 

            executionLog.CollectingInformationFrom("oval:id:7589");
            executionLog.EndCollect();
            IEnumerable<ProbeLogItem> executionLogComplete = executionLog.BuildExecutionLogs();

            Assert.AreEqual(6, executionLogComplete.Count(), "the quantity of logs entries is not expected");
            Assert.AreEqual(executionLogComplete.ElementAt(2).Message, executionLogDetail.ElementAt(0).Message, "the detail log is no found in the correct position in the complete log");
            Assert.AreEqual(executionLogComplete.ElementAt(3).Message, executionLogDetail.ElementAt(1).Message, "the detail log is no found in the correct position in the complete log");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_clear_the_execution_log_after_of_the_build_executionLogs()
        {
            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("Registry");
            executionLog.TryConnectToHost("176.16.3.166");
            executionLog.CollectingInformationFrom("oval:id:7589");
            executionLog.EndCollect();
            IEnumerable<ProbeLogItem> executionsLog = executionLog.BuildExecutionLogs();
            
            Assert.AreEqual(4, executionsLog.Count());

            executionLog.StartCollectOf("Registry");            
            executionsLog = executionLog.BuildExecutionLogs();         
            Assert.AreEqual(1, executionsLog.Count());
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_inform_the_finalization_of_an_especific_collect()
        {
            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("Registry");
            executionLog.TryConnectToHost("176.16.3.166");
            executionLog.CollectingInformationFrom("oval:id:7589");
            executionLog.EndCollectOf("Registry");
            IEnumerable<ProbeLogItem> executionsLog = executionLog.BuildExecutionLogs();
            Assert.AreEqual(4, executionsLog.Count());                
        }

        [TestMethod, Owner("lcosta")]
        public void Shoud_be_possible_to_inform_the_systemInformation_collect()
        {
            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();
            executionLog.StartCollectOf("Registry");
            executionLog.TryConnectToHost("176.16.3.166");
            executionLog.CollectingInformationFrom("oval:id:7589");
            
            executionLog.CollectSystemInformation();
            
            executionLog.EndCollectOf("Registry");
            IEnumerable<ProbeLogItem> executionsLog = executionLog.BuildExecutionLogs();
            Assert.AreEqual(5, executionsLog.Count());                
        }
    }
}
