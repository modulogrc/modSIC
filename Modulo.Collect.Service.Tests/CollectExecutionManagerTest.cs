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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Controllers;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Tests.Entities;
using Modulo.Collect.Service.Tests.Helpers;
using Raven.Client;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Service.Tests
{
    /// <summary>
    /// Summary description for CollectExecutionManagerTest
    /// </summary>
    [TestClass]
    public class CollectExecutionManagerTest
    {   
        public CollectExecutionManagerTest()
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

        private LocalDataProvider provider;
        private IDocumentSession session;

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
            provider = new LocalDataProvider();
            session = provider.GetSession();            
        }

      
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_execute_a_collect()
        {
            IDocumentSession fakeSession = provider.GetSession();
            CollectRequest collectRequest = this.GetCollectRequest();
            CollectExecutionManagerFactory factory = new CollectExecutionManagerFactory(collectRequest, session);
            List<string> registryObjectsThatNotProcessInFirstTime = new List<string>(); //{"oval:org.mitre.oval:obj:4000"};
            List<string> familyObjectsThatNotProcessInFirstTime = new List<string>();  
            CollectExecutionManager executionManager = factory.CreateExecutionManagerForTheSuccessScenario(this.GetResultForRegistry(collectRequest, registryObjectsThatNotProcessInFirstTime), 
                                                                                                           this.GetResultForFamily(collectRequest,familyObjectsThatNotProcessInFirstTime));

            executionManager.ExecuteCollect(fakeSession, collectRequest, FamilyEnumeration.windows);
            session.SaveChanges();
            CollectRequest collectRequestAfterExecute = session.Load<CollectRequest>(collectRequest.Oid.ToString());  
            this.CheckTheDefaulStateOfRequestCollectAfterOneExecution(collectRequest);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_identify_when_an_unrecoverable_error_occurs()
        {
            IDocumentSession fakeSession = provider.GetSession();
            CollectRequest collectRequest = this.GetCollectRequest();
            session.SaveChanges();
            CollectExecutionManagerFactory factory = new CollectExecutionManagerFactory(collectRequest, session);
            session.SaveChanges();
            List<string> registryObjectsThatNotProcessInFirstTime = new List<string>() { "oval:org.mitre.oval:obj:4000" };
            List<string> familyObjectsThatNotProcessInFirstTime = new List<string>();
            CollectExecutionManager executionManager = factory.CreateExecutionManagerWWithInvalidCredentialsScenario(this.GetResultForRegistry(collectRequest, registryObjectsThatNotProcessInFirstTime),
                                                                                                           this.GetResultForFamily(collectRequest, familyObjectsThatNotProcessInFirstTime));
            session.SaveChanges(); 
            executionManager.ExecuteCollect(fakeSession, collectRequest, FamilyEnumeration.windows);
            session.SaveChanges();
            CollectRequest collectRequestAfterExecute = session.Load<CollectRequest>(collectRequest.Oid.ToString());
            this.CheckTheDefaulStateOfRequestCollectAfterOneExecution(collectRequestAfterExecute);
            Assert.AreEqual(true, collectRequestAfterExecute.GetCollectExecutions(session).First().ProbeExecutions.First().HasErrors());

            Assert.IsTrue(collectRequestAfterExecute.isClosed(), "the request Collect is not closed");
        }


        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_update_systemInformation_of_target_after_a_collect()
        {
            IDocumentSession fakeSession = provider.GetSession();
            CollectRequest collectRequest = this.GetCollectRequest();
            CollectExecutionManagerFactory factory = new CollectExecutionManagerFactory(collectRequest, session);
            List<string> registryObjectsThatNotProcessInFirstTime = new List<string>() { "oval:org.mitre.oval:obj:4000" };
            List<string> familyObjectsThatNotProcessInFirstTime = new List<string>();
            CollectExecutionManager executionManager = factory.CreateExecutionManagerForTheSuccessScenario(this.GetResultForRegistry(collectRequest, registryObjectsThatNotProcessInFirstTime),
                                                                                                           this.GetResultForFamily(collectRequest, familyObjectsThatNotProcessInFirstTime));
            session.SaveChanges();
            executionManager.ExecuteCollect(fakeSession, collectRequest, FamilyEnumeration.windows);
            session.SaveChanges();
            CollectRequest collectRequestAfterExecute = session.Load<CollectRequest>(collectRequest.Oid.ToString());
            Assert.IsNotNull(collectRequestAfterExecute);
            Assert.IsTrue(collectRequestAfterExecute.Target.IsSystemInformationDefined(), "the system information is not defined");
        }

      
        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_finalize_the_collectRequest_if_the_number_of_execution_exceeds_of_limits()
        {
            IDocumentSession fakeSession = provider.GetSession();
            CollectRequest collectRequest = this.GetCollectRequest();
            oval_definitions definitions = new OvalDocumentLoader().GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            var objList = new List<ObjectType>();
            objList.Add(new family_object() { id = "oval:org.mitre.oval:obj:1000" });
            objList.Add(new file_object() { id = "oval:org.mitre.oval:obj:5000" });
            definitions.objects = objList.ToArray();

            var newDefinitiondoc = new DefinitionDocument() { OriginalId = "01", Text = definitions.GetDefinitionsXml() };
            session.Store(newDefinitiondoc);
            collectRequest.OvalDefinitionsId = newDefinitiondoc.Oid;
            CollectExecutionManagerFactory factory = new CollectExecutionManagerFactory(collectRequest, session);
            List<string> registryObjectsThatNotProcessInFirstTime = new List<string>();
            List<string> familyObjectsThatNotProcessInFirstTime = new List<string>();
            CollectExecutionManager executionManager = factory.CreateExecutionManagerForTheSuccessScenario(this.GetResultForRegistry(collectRequest, registryObjectsThatNotProcessInFirstTime),
                                                                                                           this.GetResultForFamily(collectRequest, familyObjectsThatNotProcessInFirstTime));

            session.Store(collectRequest);
            var newExecution1 = new CollectExecution() { RequestId = collectRequest.Oid };
            session.Store(newExecution1);
            var newExecution2 = new CollectExecution() { RequestId = collectRequest.Oid };
            session.Store(newExecution2);
            var newExecution3 = new CollectExecution() { RequestId = collectRequest.Oid };
            session.Store(newExecution3);
            executionManager.ExecuteCollect(fakeSession, collectRequest, FamilyEnumeration.windows);
            session.SaveChanges();

            CollectRequest collectRequestAfterExecute = session.Load<CollectRequest>(collectRequest.Oid.ToString());  
            Assert.IsNotNull(collectRequest);
            Assert.AreEqual(4, collectRequestAfterExecute.GetNumberOfExecutions(session));
            Assert.IsTrue(collectRequestAfterExecute.isClosed());
        }

        [TestMethod, Owner("lfernandes")]
        public void When_it_was_not_possible_to_establish_a_connection_to_target_the_status_of_collect_result_must_be_error()
        {
            IDocumentSession fakeSession = provider.GetSession();
            var fakeRequestCollect = this.GetCollectRequest();
            var definitions = new OvalDocumentLoader().GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            var newDefinitiondoc = new DefinitionDocument() { OriginalId = "01", Text = definitions.GetDefinitionsXml() };
            session.Store(newDefinitiondoc);
            fakeRequestCollect.OvalDefinitionsId = newDefinitiondoc.Oid;
            
            var factory = new CollectExecutionManagerFactory(fakeRequestCollect, session);
            var executionManager =
                factory
                    .CreateExecutionManagerWithCannotConnectToHostExceptionScenario(
                        this.GetResultForRegistry(fakeRequestCollect, new List<String>()),
                        this.GetResultForFamily(fakeRequestCollect, new string[] { "oval:org.mitre.oval:obj:1000" }.ToList()),
                        true);

            //fakeRequestCollect.Collects.Add(new CollectExecution(session));
            //fakeRequestCollect.Collects.Add(new CollectExecution(session));
            //fakeRequestCollect.Collects.Add(new CollectExecution(session));
            session.SaveChanges();
            executionManager.ExecuteCollect(fakeSession, fakeRequestCollect, FamilyEnumeration.windows);
            

            var requestCollectAfterExecution = session.Load<CollectRequest>(fakeRequestCollect.Oid);
            Assert.AreEqual(CollectStatus.Error, requestCollectAfterExecution.Result.Status);
        }

        private void CheckTheDefaulStateOfRequestCollectAfterOneExecution(CollectRequest requestCollectAfterExecute)
        {
            Assert.IsNotNull(requestCollectAfterExecute);
            Assert.AreEqual(1, requestCollectAfterExecute.GetNumberOfExecutions(session));
            

        }


        private CollectRequest GetCollectRequest()
        {
            CollectRequestFactory factory = new CollectRequestFactory();
            CollectRequest collectRequest = factory.CreateCollectRequestWithSpecificDefinitions(session, "definitionsWithLocalVariable.xml");
           return collectRequest;
        }

        private ProbeResult GetResultForRegistry(CollectRequest collectRequest, List<string> resultsForObjects)
        {
            ProbeResultFactory factory = new ProbeResultFactory();
            IEnumerable<ObjectType> registryObjects = collectRequest.GetObjectTypes(session).OfType<registry_object>();
            return factory.CreateProbeResultForRegistryWithSpecificObjectTypes(registryObjects, resultsForObjects);
        }

        public ProbeResult GetResultForFamily(CollectRequest collectRequest, List<string> resultsForObjects)
        {
            ProbeResultFactory factory = new ProbeResultFactory();
            IEnumerable<ObjectType> familyObjects = collectRequest.GetObjectTypes(session).OfType<family_object>();
            return factory.CreateProbeResultForFamilyWithSpecificObjectTypes(familyObjects, resultsForObjects);
        }



       


        #region Old tests
        /*
        [Ignore,Owner("lcosta")]
        public void Should_be_possible_to_execute_a_collect()
        {
            this.CreateMocksForTheSuccessScenario();
            CollectExecutionManager executionManager = new CollectExecutionManager(connectionContext, target);
            executionManager.ProbeManager = probeManager;
            executionManager.DataProvider = dataProvider;
           
            executionManager.ExecuteCollect(collectRequest, FamilyEnumeration.windows);

            CollectRequest collectRequestAfterExecute = session.GetObjectByKey<CollectRequest>(collectRequest.Oid);
            this.CheckTheDefaulStateOfCollectRequestAfterOneExecution(collectRequestAfterExecute);
            Assert.IsTrue(collectRequestAfterExecute.Collects[0].ProbeExecutions[0].IsComplete(), "the execution is not complete");
            Assert.IsNotNull(collectRequestAfterExecute.Collects[0].ProbeExecutions[0].SystemCharacteristics, "the result is null");
            Assert.IsNotNull(collectRequestAfterExecute.Collects[0].ProbeExecutions[0].SystemCharacteristics, "the system characteristics is null");         
            Assert.IsNotNull(collectRequestAfterExecute.Result, "the result is not generated");         
        }

        [Ignore, Owner("lcosta")]
        public void Should_be_possible_to_execute_a_collect_for_the_two_probes()
        {
            this.CreateMocksForTheSuccessScenario();
            CollectExecutionManager executionManager = new CollectExecutionManager(connectionContext, target);
            executionManager.ProbeManager = probeManager;
            executionManager.DataProvider = dataProvider;

            executionManager.ExecuteCollect(collectRequest, FamilyEnumeration.windows);

            CollectRequest collectRequestAfterExecute = session.GetObjectByKey<CollectRequest>(collectRequest.Oid);
            this.CheckTheDefaulStateOfCollectRequestAfterOneExecution(collectRequestAfterExecute);
            Assert.IsTrue(collectRequestAfterExecute.Collects[0].ProbeExecutions[0].IsComplete(), "the execution is not complete");
            Assert.IsNotNull(collectRequestAfterExecute.Collects[0].ProbeExecutions[0].SystemCharacteristics, "the result is null");
            Assert.IsNotNull(collectRequestAfterExecute.Collects[0].ProbeExecutions[0].SystemCharacteristics, "the system characteristics is null");
            //Assert.IsTrue(collectRequestAfterExecute.isClosed(), "the collectRequest is not closed.");
            Assert.IsNotNull(collectRequestAfterExecute.Result, "the result is not generated");
            //Assert.IsTrue(collectRequestAfterExecute.Result.IsComplete(), "the result is not complete");
        }

        [Ignore, Owner("lcosta")]
        public void Should_be_possible_to_attempt_execute_a_collectRequest_if_a_error_recoverable_occurs()
        {
            
            this.CreateMocksWithCannotConnectToHostExceptionScenario();

            CollectExecutionManager executionManager = new CollectExecutionManager(connectionContext, target);
            executionManager.ProbeManager = probeManager;
            executionManager.DataProvider = dataProvider;

            executionManager.ExecuteCollect(collectRequest, FamilyEnumeration.windows);

            CollectRequest collectRequestAfterExecute = session.GetObjectByKey<CollectRequest>(collectRequest.Oid);
            this.CheckTheDefaulStateOfCollectRequestAfterOneExecution(collectRequestAfterExecute);
            Assert.IsTrue(collectRequestAfterExecute.Collects[0].ProbeExecutions[0].HasErrors(), "the probe execution is not have the errors as expected");
            Assert.IsTrue(collectRequestAfterExecute.isOpen(), "the collectRequest is not open.");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_identify_when_an_unrecoverable_error_occurs()
        {
            this.CreateMocksWithInvalidCredentialsScenario();

            CollectExecutionManager executionManager = new CollectExecutionManager(connectionContext, target);
            executionManager.ProbeManager = probeManager;
            executionManager.DataProvider = dataProvider;

            executionManager.ExecuteCollect(collectRequest, FamilyEnumeration.windows);

            CollectRequest collectRequestAfterExecute = session.GetObjectByKey<CollectRequest>(collectRequest.Oid);
            Assert.IsNotNull(collectRequestAfterExecute);
            Assert.IsTrue(collectRequestAfterExecute.Collects[0].ProbeExecutions[0].HasErrors(), "the probe execution is not have the errors as expected");
            Assert.IsTrue(collectRequestAfterExecute.isClosed(), "the collectRequest is not closed.");

        }

        [Ignore, Owner("lcosta")]
        public void Should_be_possible_to_update_systemInformation_of_target_after_a_collect()
        {
            this.CreateMocksForTheSuccessScenario();
            CollectExecutionManager executionManager = new CollectExecutionManager(connectionContext, target);
            executionManager.ProbeManager = probeManager;
            executionManager.DataProvider = dataProvider;           

            executionManager.ExecuteCollect(collectRequest, FamilyEnumeration.windows);

            CollectRequest collectRequestAfterExecute = session.GetObjectByKey<CollectRequest>(collectRequest.Oid);
            Assert.IsNotNull(collectRequest);
            Assert.IsTrue(collectRequest.Target.IsSystemInformationDefined(), "the system information is not defined");
        }


        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_attempt_execute_a_collectRequest_if_a_error_occurs_when_the_saving_CollectRequest()
        {
            this.CreateMocksWithErrorInTheCommitChanges();           

            CollectExecutionManager executionManager = new CollectExecutionManager(connectionContext, target);
            executionManager.ProbeManager = probeManager;
            executionManager.DataProvider = dataProvider;

            executionManager.ExecuteCollect(collectRequest, FamilyEnumeration.windows);

            CollectRequest collectRequestAfterExecute = session.GetObjectByKey<CollectRequest>(collectRequest.Oid);
            Assert.IsTrue(collectRequestAfterExecute.Collects.Count == 0, "was created one collect.");
            Assert.IsTrue(collectRequestAfterExecute.isOpen(), "the collectRequest is not open.");
        }   
    
        
              
        private void CheckTheDefaulStateOfCollectRequestAfterOneExecution(CollectRequest collectRequestAfterExecute)
        {
            Assert.IsNotNull(collectRequestAfterExecute);
            Assert.AreEqual(collectRequestAfterExecute.Collects.Count == 1, "the number of collect is not the expected");
            Assert.AreEqual(collectRequestAfterExecute.Collects[0].ProbeExecutions.Count == 1, "the number of probe executions is not the expected");
        }

        private void CreateMocksWithCannotConnectToHostExceptionScenario()
        {         
            MockRepository mocks = new MockRepository();
            probeManager = mocks.DynamicMock<IProbeManager>();
            probe = mocks.DynamicMock<IProbe>();
            familyProbe = mocks.DynamicMock<IProbe>();
            dataProvider = mocks.DynamicMock<IDataProvider>();
            
            CollectInfo collectInfo = new CollectInfo() { ObjectTypes = ovalObjects };

            Expect.Call(probeManager.GetProbesFor(null, FamilyEnumeration.windows)).Repeat.Any().IgnoreArguments().Return(GetSelectedProbes());           
            Expect.Call(probe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Throw(new CannotConnectToHostException(target, "Cannot Connection To Host..."));
            Expect.Call(familyProbe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Return(GetProbeResult());           
            Expect.Call(dataProvider.GetSession()).Repeat.Any().Return(session);
            Expect.Call(dataProvider.GetTransaction(session)).Repeat.Any().Return(new Transaction(session));
            

            mocks.ReplayAll();           
            
        }

        private void CreateMocksWithInvalidCredentialsScenario()
        {
            MockRepository mocks = new MockRepository();
            probeManager = mocks.DynamicMock<IProbeManager>();
            probe = mocks.DynamicMock<IProbe>();
            familyProbe = mocks.DynamicMock<IProbe>();
            dataProvider = mocks.DynamicMock<IDataProvider>();

            CollectInfo collectInfo = new CollectInfo() { ObjectTypes = ovalObjects };

            Expect.Call(probeManager.GetProbesFor(null, FamilyEnumeration.windows)).IgnoreArguments().Return(GetSelectedProbes());
            Expect.Call(probe.Execute(connectionContext, target, collectInfo)).Throw(new InvalidCredentialsException("The credentials is not valid",null));
            Expect.Call(familyProbe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Return(GetProbeResult());           
            Expect.Call(dataProvider.GetSession()).Repeat.Any().Return(session);
            Expect.Call(dataProvider.GetTransaction(session)).Repeat.Any().Return(new Transaction(session));

            mocks.ReplayAll();
        }

        private void CreateMocksWithErrorInTheCommitChanges()
        {
            MockRepository mocks = new MockRepository();
            probeManager = mocks.DynamicMock<IProbeManager>();
            probe = mocks.DynamicMock<IProbe>();
            familyProbe = mocks.DynamicMock<IProbe>();
            dataProvider = mocks.DynamicMock<IDataProvider>();
            ITransaction transactionFake = new TransactionFake(session);

            CollectInfo collectInfo = new CollectInfo() { ObjectTypes = ovalObjects };


            Expect.Call(probeManager.GetProbesFor(null, FamilyEnumeration.windows)).IgnoreArguments().Return(GetSelectedProbes());
            Expect.Call(probe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Return(GetProbeResult());
            Expect.Call(familyProbe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Return(GetProbeResult());           
            
            Expect.Call(dataProvider.GetSession()).Repeat.Any().Return(session);
            Expect.Call(dataProvider.GetTransaction(session)).Repeat.Any().Return(transactionFake);

            mocks.ReplayAll();
        }


        private void CreateMocksForTheSuccessScenario()
        {
            provider = new LocalDataProvider();
            MockRepository mocks = new MockRepository();
            probeManager = mocks.DynamicMock<IProbeManager>();
            probe = mocks.DynamicMock<IProbe>();
            familyProbe = mocks.DynamicMock<IProbe>();
            dataProvider = mocks.DynamicMock<IDataProvider>();
            systemInformationService = mocks.DynamicMock<ISystemInformationService>();

            CollectInfo collectInfo = new CollectInfo() { ObjectTypes = ovalObjects };

            Expect.Call(probeManager.GetProbesFor(null, FamilyEnumeration.windows)).IgnoreArguments().Return(GetSelectedProbes());
            Expect.Call(probeManager.GetSystemInformationService(FamilyEnumeration.windows)).IgnoreArguments().Return(new WindowsSystemInformationService());
            Expect.Call(probe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Return(GetProbeResult());
            Expect.Call(familyProbe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Return(GetProbeResult());
            Expect.Call(dataProvider.GetSession()).Repeat.Any().Return(session);
            Expect.Call(dataProvider.GetTransaction(session)).Repeat.Any().Return(new Transaction(session));
            Expect.Call(systemInformationService.GetSystemInformationFrom(target)).IgnoreArguments().Return(this.GetExpectedSystemInformation());

            mocks.ReplayAll();
        }

        private Modulo.Collect.Probe.Common.BasicClasses.ProbeResult GetProbeResult()
        {
            ProbeResultFactory factory = new ProbeResultFactory();
            return factory.CreateProbeResultForRegistryCollect();
        }

        private static List<IConnectionProvider> CreateConnectionContext()
        {
            List<IConnectionProvider> connectionContext = new List<IConnectionProvider>();
            return connectionContext;
        }

        private TargetInfo CreateTargetInfo()
        {
            TargetInfo target = new TargetInfo();
            target.Add("HostName", "mss-rj-215");
            return target;
        }

        private IEnumerable<Modulo.Collect.Service.Probes.SelectedProbe> GetSelectedProbes()
        {

            SelectedProbe registryProbe = new SelectedProbe(probe,
                                                            ovalObjects,
                                                            new ProbeCapabilities()
                                                            {
                                                                OvalObject = "registry",
                                                                PlataformName = FamilyEnumeration.windows
                                                            });


            SelectedProbe familyProber = new SelectedProbe( familyProbe,
                                                            ovalObjects,
                                                            new ProbeCapabilities()
                                                            {
                                                                OvalObject = "family",
                                                                PlataformName = FamilyEnumeration.windows
                                                            });

            List<SelectedProbe> selectedProbes = new List<SelectedProbe>();            
            selectedProbes.Add(registryProbe);
            selectedProbes.Add(familyProber);
            return selectedProbes;

        }


       

        public SystemInformation GetExpectedSystemInformation()
        {
            SystemInformation sysInfo = new SystemInformation();
            sysInfo.SystemName = "Microsoft Windows Server 2008 Enterprise SP2";
            sysInfo.SystemVersion = "6.0.6002";
            sysInfo.Architecture = "INTEL32";
            sysInfo.PrimaryHostName = "mss-rj-220.mss.modulo.com.br";
            sysInfo.Interfaces = new List<NetworkInterface>();
            sysInfo.Interfaces.Add(new NetworkInterface() { IpAddress = "172.16.3.166", MacAddress = "00 - 23 - AE - B6 - 6F - BF", Name = "Intel(R) 82567LM-3 Gigabit Network Connection" });
            return sysInfo;
        }
         */
        #endregion
    }
}
