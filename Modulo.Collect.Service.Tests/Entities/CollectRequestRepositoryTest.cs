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
using System.Xml;
using System.IO;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Exceptions;
using Modulo.Collect.Service.Tests.Helpers;
using Raven.Client;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Service.Contract;


namespace Modulo.Collect.Service.Tests.Entities
{
    [TestClass]
    public class CollectRequestRepositoryTest
    {
        private LocalDataProvider dataprovider;

        private string MORE_THAN_ONE_COLLECT_REQUEST_WAS_FOUND = "More than one collect request which Oid is '{0}' was found in loaded collect requests.";
        private string MORE_THAN_ONE_COLLECT_PACKAGE_WAS_FOUND = "More than one collect package which Oid is '{0}' was found in loaded collect packages.";

        [TestInitialize()]
        public void MyTestInitialize()
        {
            dataprovider = new LocalDataProvider();
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_get_a_collectRequest_By_Id()
        {
            IDocumentSession fakeSession = this.GetSession();
            var collectRequest = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            fakeSession.SaveChanges();

            var repository = new CollectRequestRepository(dataprovider);
            var loadedRequest = repository.GetCollectRequest(fakeSession, collectRequest.Oid);
            Assert.IsNotNull(loadedRequest);
            Assert.AreEqual(collectRequest.Oid, loadedRequest.Oid);
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_get_collectRequests_By_Ids()
        {
            IDocumentSession fakeSession = this.GetSession();
            var collectRequest1 = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            var collectRequest2 = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            fakeSession.SaveChanges();

            var repository = new CollectRequestRepository(dataprovider);
            var loadedRequests = repository.GetCollectRequests(fakeSession, collectRequest1.Oid, collectRequest2.Oid);
            Assert.IsNotNull(loadedRequests);
            Assert.AreEqual(2, loadedRequests.Count());
            Assert.AreEqual(1, loadedRequests.Count(x => x.Oid == collectRequest1.Oid));
            Assert.AreEqual(1, loadedRequests.Count(x => x.Oid == collectRequest2.Oid));
        }

        [TestMethod, Owner("lfernandes")]
        public void When_the_list_of_ids_contains_repeated_items_GetCollectRequests_should_not_return_repeated_collect_requests()
        {
            IDocumentSession fakeSession = this.GetSession();
            var collectRequest1 = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            var collectRequest2 = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            fakeSession.SaveChanges();

            var repository = new CollectRequestRepository(dataprovider);
            var loadedRequests = repository.GetCollectRequests(fakeSession, collectRequest1.Oid, collectRequest1.Oid, collectRequest2.Oid);
            Assert.IsNotNull(loadedRequests);
            Assert.AreEqual(1, loadedRequests.Count(x => x.Oid == collectRequest1.Oid), MORE_THAN_ONE_COLLECT_REQUEST_WAS_FOUND, collectRequest1.Oid);
            Assert.AreEqual(1, loadedRequests.Count(x => x.Oid == collectRequest2.Oid), MORE_THAN_ONE_COLLECT_REQUEST_WAS_FOUND, collectRequest2.Oid);
            Assert.AreEqual(2, loadedRequests.Count());
        }

        private CollectPackage CreateFakeCollectPackage(IDocumentSession session)
        {
            var newCollectPackage = new CollectPackage() 
            { 
                Date = DateTime.UtcNow, 
                ScheduleInformation = new CollectScheduleInformation() { ExecutionDate = DateTime.UtcNow.AddMinutes(2) }
            };
            session.Store(newCollectPackage);
            return newCollectPackage;
        }

        [TestMethod, Owner("lfernandes")]
        public void When_the_list_of_ids_contains_repeated_items_GetPackages_should_not_return_repeated_collect_requests()
        {
            IDocumentSession fakeSession = this.GetSession();
            var pkg1 = CreateFakeCollectPackage(fakeSession);
            var pkg2 = CreateFakeCollectPackage(fakeSession);
            var pkg3 = CreateFakeCollectPackage(fakeSession);
            fakeSession.SaveChanges();

            var repository = new CollectRequestRepository(dataprovider);
            var loadedPackages = repository.GetCollectPackages(fakeSession, pkg1.Oid, pkg2.Oid, pkg2.Oid, pkg3.Oid, pkg3.Oid, pkg3.Oid);
            Assert.IsNotNull(loadedPackages);
            Assert.AreEqual(1, loadedPackages.Count(x => x.Oid == pkg1.Oid), MORE_THAN_ONE_COLLECT_PACKAGE_WAS_FOUND, pkg1.Oid);
            Assert.AreEqual(1, loadedPackages.Count(x => x.Oid == pkg2.Oid), MORE_THAN_ONE_COLLECT_PACKAGE_WAS_FOUND, pkg2.Oid);
            Assert.AreEqual(1, loadedPackages.Count(x => x.Oid == pkg3.Oid), MORE_THAN_ONE_COLLECT_PACKAGE_WAS_FOUND, pkg3.Oid);
            Assert.AreEqual(3, loadedPackages.Count());
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_get_All_Open_CollectRequests()
        {
            IDocumentSession fakeSession = this.GetSession();
            var collectRequest1 = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            collectRequest1.Status = CollectRequestStatus.Open;
            var collectRequest2 = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            collectRequest2.Status = CollectRequestStatus.Open;
            var collectRequest3 = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            collectRequest3.Status = CollectRequestStatus.Close;
            fakeSession.SaveChanges();

            var repository = new CollectRequestRepository(dataprovider);
            var loadedRequests = repository.GetOpenCollectRequests(fakeSession).ToList();
            Assert.IsNotNull(loadedRequests);
            Assert.AreEqual(2, loadedRequests.Count());
            Assert.AreEqual(1, loadedRequests.Count(x => x.Oid == collectRequest1.Oid));
            Assert.AreEqual(1, loadedRequests.Count(x => x.Oid == collectRequest2.Oid));
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_get_a_definition_by_Ids()
        {
            IDocumentSession fakeSession = this.GetSession();

            string definitionId = "def1";
            var fakeDefinition = new DefinitionDocument() { OriginalId = definitionId };
            fakeSession.Store(fakeDefinition);
            fakeSession.SaveChanges();

            var repository = new CollectRequestRepository(dataprovider);
            var resultByOriginalId = repository.GetDefinitionByOriginalId(fakeSession, definitionId);
            var resultByNewId = repository.GetDefinitionByDocumentId(fakeSession, fakeDefinition.Oid);

            Assert.IsNotNull(resultByOriginalId);
            Assert.AreEqual(definitionId, resultByOriginalId.OriginalId);
            Assert.AreEqual(fakeDefinition.Oid, resultByOriginalId.Oid);
            Assert.AreEqual(resultByOriginalId, resultByNewId);
        }

        private IDocumentSession GetSession()
        {
            return dataprovider.GetSession();

        }



    }
}