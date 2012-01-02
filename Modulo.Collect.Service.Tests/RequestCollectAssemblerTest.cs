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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Assemblers;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Tests.Helpers;
using System;
using System.Linq;
using Modulo.Collect.Service.Tests.Entities;
using Raven.Client;
using Modulo.Collect.Service.Data;

namespace Modulo.Collect.Service.Tests
{
    
    
    /// <summary>
    ///This is a test class for CollectRequestAssemblerTest and is intended
    ///to contain all CollectRequestAssemblerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CollectRequestAssemblerTest
    {


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

        [TestInitialize()]
        public void MyTestInitialize()
        {
            dataProvider = new LocalDataProvider();
        }
        
        IDataProvider dataProvider;
        protected IDocumentSession GetSession()
        {
            
            return dataProvider.GetSession();
        }

        /// <summary>
        ///A test for CreateCollectRequestFromDTO
        ///</summary>
        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_collectRequest_entity_from_collectRequest_DTO()
        {
            // Arrange
            //Session fakeSession = this.GetSession();
            //XpoDefault.DataLayer = XpoDefault.GetDataLayer(AutoCreateOption.DatabaseAndSchema);
            var fakeSession = GetSession();
            CollectRequestAssembler collectRequestAssembler = new CollectRequestAssembler(dataProvider);

            // Act
            var collectPackageDTO = CollectRequestDtoFactory.CreateCollectPackageDTO("10.0.0.1");
            var collectRequestDTO = collectPackageDTO.CollectRequests[0];
            var collectRequest = collectRequestAssembler.CreateCollectRequestFromDTO(collectRequestDTO, collectPackageDTO.Definitions.First().Text);
            TargetParameter targetParameter = collectRequest.Target.GetTargetParameterByName("instance");            
            //Assert
            Assert.AreEqual("10.0.0.1", collectRequest.Target.Address);            
            Assert.IsNotNull(targetParameter, "There is no instance parameter in Request Modulo.Collect.");            
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_create_a_collectRequest_entity_from_collectRequest_DTO_with_targetParameters_null()
        {
            // Arrange
            //Session fakeSession = this.GetSession();
            //XpoDefault.DataLayer = XpoDefault.GetDataLayer(AutoCreateOption.DatabaseAndSchema);
            var fakeSession = GetSession();
            CollectRequestAssembler collectRequestAssembler = new CollectRequestAssembler(dataProvider);
                       
            var collectPackageDTO = CollectRequestDtoFactory.CreateCollectPackageDTO("10.0.0.1");
            var collectRequestDTO = collectPackageDTO.CollectRequests[0];
            // Act
            var collectRequest = collectRequestAssembler.CreateCollectRequestFromDTO(collectRequestDTO, collectPackageDTO.Definitions.First().Text); 
            TargetParameter targetParameter = null;
            //Assert
            Assert.AreEqual("10.0.0.1", collectRequest.Target.Address);         
            Assert.IsNull(targetParameter);            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_CollectInfoDTO_from_RequestColect()
        {
            var session = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequest(session).Item2;
            CollectRequestAssembler assembler = new CollectRequestAssembler(dataProvider);
            CollectInfo collectInfo = assembler.CreateCollectInfoFromCollectRequest(collectRequest);
            Assert.IsNotNull(collectInfo);
            Assert.AreEqual(collectRequest.Oid.ToString(), collectInfo.CollectRequestId);
            Assert.AreEqual(collectRequest.Target.Address, collectInfo.Address);
            Assert.AreEqual(collectRequest.ReceivedOn, collectInfo.ReceivedOn);
        }
    }
}
