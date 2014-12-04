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
using Modulo.Collect.Service.Entities.Factories;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Service.Tests.Helpers;
using System.Xml.Serialization;
using System.IO;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Contract;
using Raven.Client;

namespace Modulo.Collect.Service.Tests.Entities
{
    /// <summary>
    /// Summary description for SystemCharacteristicsTest
    /// </summary>
    [TestClass]
    public class DefinitionDocumentFactoryTest
    {
        private LocalDataProvider localDataProvider;

       
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             localDataProvider = new LocalDataProvider();
         }
       
        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_create_a_definition_document_from_Dto()
        {

            // Arrange
            var sampleInfo = new DefinitionInfo();
            sampleInfo.Id = "def01";
            sampleInfo.Text = "def01_text";
            var factory = new DefinitionDocumentFactory(localDataProvider);

            // Act
            var definitionDocument = factory.CreateDefinitionDocumentFromInfo(localDataProvider.GetSession(), sampleInfo);
            
            // Assert
            Assert.IsNotNull(definitionDocument);
            Assert.AreEqual(sampleInfo.Id, definitionDocument.OriginalId);
            Assert.AreEqual(sampleInfo.Text, definitionDocument.Text);
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_load_a_created_definition()
        {
            IDocumentSession fakeSession = localDataProvider.GetSession();

            // Arrange
            var sampleInfo = new DefinitionInfo();
            sampleInfo.Id = "def01";
            sampleInfo.Text = "def01_text";
            var factory = new DefinitionDocumentFactory(localDataProvider);

            // Act           
            factory.CreateDefinitionDocumentFromInfo(fakeSession, sampleInfo);
            var definitionDocument = localDataProvider.GetSession().Query<DefinitionDocument>().Single(x=>x.OriginalId == sampleInfo.Id);

            // Assert
            Assert.IsNotNull(definitionDocument);
            Assert.AreEqual(sampleInfo.Id, definitionDocument.OriginalId);
            Assert.AreEqual(sampleInfo.Text, definitionDocument.Text);
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_stored_a_created_definition_hash()
        {

            // Arrange
            var sampleInfo = new DefinitionInfo() { Id = "def01", Text = "def01_text" };
            var factory = new DefinitionDocumentFactory(localDataProvider);

            // Act
            var result = factory.CreateDefinitionDocumentFromInfo(localDataProvider.GetSession(), sampleInfo);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result.Hash));
        }
    }
}
