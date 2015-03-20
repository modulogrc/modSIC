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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Controllers;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Exceptions;
using Modulo.Collect.Service.Tests.Entities;
using Modulo.Collect.Service.Tests.Helpers;
using Quartz;
using Rhino.Mocks;
using Modulo.Collect.Service.Probes;
using Modulo.Collect.OVAL.Common;
using Raven.Client;
using Rhino.Mocks.Interfaces;
using Modulo.Collect.OVAL.Results;
using Modulo.Collect.Service.Assemblers;
using Modulo.Collect.Service.Entities.Factories;

namespace Modulo.Collect.Service.Tests
{
    /// <summary>
    /// This is a test class for CollectServiceTest and it's intended
    /// to contain all CollectServiceTest Unit Tests
    ///</summary>
    [TestClass]
    public class CollectServiceTest
    {
        private const string FAKE_IP = "192.168.1.10";
        private const string FAKE_MODSIC_CLIENTID = "Client2469171";

        private CollectRequestRepository Repository;
        private CollectController CollectController;
        private LocalDataProvider DataProvider;
        private IScheduler FakeScheduler;
        private IDocumentSession SessionDefault;
        private IProbeManager fakeProbeManager;
        private IScheduleController fakeScheduleController;
        ICollectRequestAssembler fakeCollectAssembler;
        ICollectResultAssembler fakeResultAssembler;
        ICollectPackageAssembler fakePackageAssembler;
        IDefinitionDocumentFactory definitionFactory;
        [TestCleanup]
        public void MyTestCleanup()
        {
           // XpoDefault.DataLayer = null;
        }

        [TestInitialize]
        public void Initialize()
        {
            DataProvider = new LocalDataProvider();
            SessionDefault = this.GetSession();
            Repository = new CollectRequestRepository(DataProvider);
            FakeScheduler = MockRepository.GenerateMock<IScheduler>();
            fakeProbeManager = MockRepository.GenerateMock<IProbeManager>();
            fakeScheduleController = MockRepository.GenerateMock<IScheduleController>();
            definitionFactory = new DefinitionDocumentFactory(DataProvider);
            fakeCollectAssembler = new CollectRequestAssembler(DataProvider);
            fakeResultAssembler = new CollectResultAssembler();
            fakePackageAssembler = new CollectPackageAssembler(DataProvider);
            CollectController = new CollectController(Repository, FakeScheduler, fakeScheduleController, fakeProbeManager, fakeCollectAssembler, fakeResultAssembler, fakePackageAssembler, definitionFactory);
        }

      

        protected IDocumentSession GetSession()
        {

            return DataProvider.GetSession();

        }

        /// <summary>
        /// A test for CollectRequestDTO
        ///</summary>
        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_request_a_collect_to_a_target()
        {
            var collectPackage = CollectRequestDtoFactory.CreateCollectPackageDTO(FAKE_IP);

            var requestIDs = CollectController.CollectRequest(collectPackage, FAKE_MODSIC_CLIENTID);

            Assert.IsNotNull(requestIDs, "The return of getCollectRequestion cannot be null.");
            Assert.IsNotNull(requestIDs[collectPackage.CollectRequests[0].RequestId], "the requestId cannot be null");
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_not_be_possible_to_request_a_collect_to_a_target_with_collectRequest_null()
        {
            Dictionary<string, string> requestIDs = CollectController.CollectRequest(null, FAKE_MODSIC_CLIENTID);
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(RequestItemNullException))]
        public void Should_not_be_possible_to_request_a_collect_to_a_target_with_collectRequestItem_null()
        {
            var collectionRequest1 = CollectRequestDtoFactory.CreateCollectRequestDTO(FAKE_IP);
            collectionRequest1.RequestId = "129";
            var collectionRequest2 = CollectRequestDtoFactory.CreateCollectRequestDTO("192.168.14.10");
            collectionRequest2.RequestId = "57";

            var collectPackage = CollectRequestDtoFactory.CreateCollectPackageDTO();
            collectPackage.CollectRequests = new Request[] { collectionRequest1, null, collectionRequest2 };
            CollectController.CollectRequest(collectPackage, FAKE_MODSIC_CLIENTID);
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(RequestIDNullException))]
        public void Should_not_be_possible_to_request_a_collect_to_a_target_with_a_requestID_null_or_empty()
        {
            var collectPackage = CollectRequestDtoFactory.CreateCollectPackageDTO(FAKE_IP);
            collectPackage.CollectRequests[0].RequestId = string.Empty;

            var requestIDs = CollectController.CollectRequest(collectPackage, FAKE_MODSIC_CLIENTID);
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(DuplicatedRequestIDsException))]
        public void Should_not_be_possible_request_a_collect_to_a_target_with_a_duplicated_requestID()
        {

            var collectionRequest1 = CollectRequestDtoFactory.CreateCollectRequestDTO(FAKE_IP);
            collectionRequest1.RequestId = "10";
            
            var collectionRequest2 = CollectRequestDtoFactory.CreateCollectRequestDTO(FAKE_IP);
            collectionRequest2.RequestId = "10";
            
            var collectionRequest3 = CollectRequestDtoFactory.CreateCollectRequestDTO("192.168.1.18");
            collectionRequest3.RequestId = "48899";

            var collectPackage = CollectRequestDtoFactory.CreateCollectPackageDTO();
            collectPackage.CollectRequests = 
                new Request[] { collectionRequest1, collectionRequest2, collectionRequest3 };

            var requestIDs = CollectController.CollectRequest(collectPackage, FAKE_MODSIC_CLIENTID);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_the_results_of_collect()
        {
            var collectRequest = new CollectRequestFactory().CreateCollectRequestCompleteAfterCollect(this.SessionDefault);
            var id = this.SaveCollectRequest(collectRequest, SessionDefault);

            var collectResult = this.CollectController.GetCollectedResultDocument(id.ToString());

            Assert.IsNotNull(collectResult);
            Assert.IsNotNull(collectResult.Date);
            Assert.IsNotNull(collectResult.SystemCharacteristics);
        }

        [TestMethod, Owner("lfernandes"), Ignore]
        public void Should_be_possible_to_get_the_same_capabability_for_many_plataforms()
        {
            Expect.Call(fakeProbeManager.GetCapabilities()).Return(CreateFakeProbeCapabilities());

            var capabilities = CollectController.GetCapabilities();

            Assert.IsNotNull(capabilities);
        }

        [TestMethod, Owner("lfalcao, cpaiva")]
        public void Should_be_possible_to_get_the_oval_results_document()
        {
            //arrange            
            var loadOvalDocument = new OvalDocumentLoader();
            var session = GetSession();
            var collectRequest = new CollectRequestFactory().CreateCollectRequestCompleteAfterCollect(this.SessionDefault);
            var newDefinitiondoc = new DefinitionDocument() { 
                OriginalId = "01", 
                Text = loadOvalDocument.GetFakeOvalDefinitions("OvalResultDocumentTest.xml").GetDefinitionsXml()};
            session.Store(newDefinitiondoc);
            collectRequest.OvalDefinitionsId = newDefinitiondoc.Oid;
            var id = this.SaveCollectRequest(collectRequest, session);                
          
            var collectResult = new Result();
            collectResult.SystemCharacteristics = loadOvalDocument.GetFakeOvalSystemCharacteristics("OvalResultDocumentTest.xml").GetSystemCharacteristicsXML();

            var ovalDocumentGenerator = new OvalDefinitionDocumentGenerator();
            //act
            oval_results document = ovalDocumentGenerator.GetDocument(collectRequest, newDefinitiondoc.Text, collectResult);

            //assert
            Assert.IsNotNull(document);
            Assert.AreEqual(ResultEnumeration.@true, document.results[0].definitions[0].result);
            
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_generate_hash_from_username_and_password()
        {
            var fakeUser = "admin";
            var fakePassword = "Pa$$w@rd";
            string fakeToken = "5B7A297A4F583FA23C5C71CE8104377B30C3DA57";

            var token = new Hash().GetAuthorizationHash(fakeUser, fakePassword);
            Assert.IsNotNull(token);
            Assert.AreEqual(fakeToken, token);
        }

        private IList<ProbeCapabilities> CreateFakeProbeCapabilities()
        {
            return new ProbeCapabilities[] 
            { 
                new ProbeCapabilities { OvalObject = "registry", PlataformName = FamilyEnumeration.windows },
                new ProbeCapabilities { OvalObject = "family", PlataformName = FamilyEnumeration.unix },
                new ProbeCapabilities { OvalObject = "family", PlataformName = FamilyEnumeration.windows }
            };
        }

        private string SaveCollectRequest(CollectRequest collectRequest, IDocumentSession session)
        {
           // using (UnitOfWork uow = new UnitOfWork())
            //{
                //try
                //{
               //     collectRequest.Save();
                 //   uow.CommitChanges();
                    session.SaveChanges();
                    return collectRequest.Oid;
                //}
                //catch (Exception ex)
                //{
                //   // uow.ReloadChangedObjects();
                //    throw new Exception(ex.Message, ex);
                //}
            //}
        }
    }
}
