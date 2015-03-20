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
using System.Text;
using Modulo.Collect.Service.Entities;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using Modulo.Collect.OVAL.Schema;
using System.IO;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Service.Tests.Helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Raven.Client;

namespace Modulo.Collect.Service.Tests.Entities
{
    public class CollectRequestFactory
    {
        public Tuple<CollectPackage,CollectRequest> CreateCollectRequest(IDocumentSession session)
        {
            var request = GEtCollectRequest(session);
            var collectRequestPackage = request.Item1;
            var collectRequest = request.Item2;
            var newDefinitiondoc = new DefinitionDocument()
            {
                OriginalId = "01",
                Text = GetOvalObjectInXML(".definitions.fdcc_xpfirewall_oval.xml")
            };
            session.Store(newDefinitiondoc);
            collectRequest.OvalDefinitionsId = newDefinitiondoc.Oid; 
            collectRequest.ExternalVariables = new ExternalVariableFactory().GetExternalVariableWithDefaultValueFrom("fdcc_xpfirewall_oval.xml");
            this.CreateTarget(session, collectRequest);
            session.Store(collectRequest);
            return new Tuple<CollectPackage, CollectRequest>(collectRequestPackage, collectRequest);
        }

        public CollectRequest CreateCollectRequestWithInvalidDefinitions(IDocumentSession session)
        {
            CollectRequest collectRequest = GEtCollectRequest(session).Item2;
            string definitionsInXml = GetOvalObjectInXML(".definitions.oval.org.mitre.oval.def.5368.invalid.xml");
            StringBuilder builder = new StringBuilder(definitionsInXml);
            var newDefinitiondoc = new DefinitionDocument()
            {
                OriginalId = "01",
                Text = builder.ToString()
            };
            session.Store(newDefinitiondoc);
            collectRequest.OvalDefinitionsId = newDefinitiondoc.Oid; return collectRequest;
        }

        public CollectRequest CreateCollectRequestWithInvalidSchemaDefinitions(IDocumentSession session)
        {
            CollectRequest collectRequest = GEtCollectRequest(session).Item2;
            string definitionsInXml = GetOvalObjectInXML(".definitions.definitionsSimpleInvalidSchema.xml");
            StringBuilder builder = new StringBuilder(definitionsInXml);
            var newDefinitiondoc = new DefinitionDocument()
            {
                OriginalId = "01",
                Text = builder.ToString()
            };
            session.Store(newDefinitiondoc);
            collectRequest.OvalDefinitionsId = newDefinitiondoc.Oid; 
            return collectRequest;
        }


        public CollectRequest CreateCollectRequestWithDefinitionsWithVariables(IDocumentSession session)
        {
            CollectRequest collectRequest = GEtCollectRequest(session).Item2;
            string definitionsInXml = GetOvalObjectInXML(".definitions.oval.org.mitre.oval.def.5368.xml");
            StringBuilder builder = new StringBuilder(definitionsInXml);
            var newDefinitiondoc = new DefinitionDocument()
            {
                OriginalId = "01",
                Text = builder.ToString()
            };
            session.Store(newDefinitiondoc);
            collectRequest.OvalDefinitionsId = newDefinitiondoc.Oid;
          
            return collectRequest;
        }

        public CollectRequest CreateCollectRequestWithSpecificDefinitions(IDocumentSession session, string nameOfDefinitions)
        {
            string completeName = ".definitions." + nameOfDefinitions;
            CollectRequest collectRequest = GEtCollectRequest(session).Item2;
            var newDefinitiondoc = new DefinitionDocument()
            {
                OriginalId = "01",
                Text = GetOvalObjectInXML(completeName)
            };
            session.Store(newDefinitiondoc);
            collectRequest.OvalDefinitionsId = newDefinitiondoc.Oid;
            CreateTarget(session, collectRequest);
            session.Store(collectRequest);
            return collectRequest;
        }

        private void CreateTarget(IDocumentSession session, CollectRequest collectRequest)
        {
            Target target = new Target();
            target.Address = "172.16.3.166";
            //target.CollectRequest = collectRequest;
            collectRequest.Target = target;
        }

        public CollectExecution CreateCollect(IDocumentSession session)
        {
            CollectExecution collect = new CollectExecution();
            collect.SetDateStartCollect();
            collect.SetDateEndCollect();
            session.Store(collect);
            return collect;           
        }

        public ProbeExecution CreateProbeExecution(IDocumentSession session, string capability)
        {
            ProbeExecution registryProbe = new ProbeExecution()
            {
                Capability = capability,                               
            };
            return registryProbe;
        }

        public CollectRequest CreateCollectRequestCompleteAfterCollect(IDocumentSession session)
        {
            CollectRequest collectRequest = new CollectRequest();
            session.Store(collectRequest);
            CollectExecution collectExecution = new CollectExecution();
            session.Store(collectExecution);
            collectExecution.RequestId = collectRequest.Oid;
            collectExecution.SetDateStartCollect();
            collectExecution.SetDateEndCollect();
            ProbeResult probeResult = new ProbeResultFactory().CreateProbeResultForRegistryCollect();
            
            CollectFactory collectFactory = new CollectFactory(session);            
            ProbeExecution probeExecution = collectFactory.CreateAProbeExecution(probeResult, "registry");
            CollectResult collectResult = new CollectResultFactory().CreateCollectResultForTheProbeExecution(probeResult);
            collectRequest.Result = collectResult;
            string systemCharacteristicsXml = this.GetSystemCharacteristicsInXML(".system_characteristics.oval.org.mitre.oval.sc.5368.xml");
            collectResult.SystemCharacteristics = systemCharacteristicsXml;

            probeExecution.SystemCharacteristics = collectResult.SystemCharacteristics;
            collectExecution.ProbeExecutions.Add(probeExecution);
            this.CreateTarget(session, collectRequest);
            //session.Store(collectRequest);
            collectExecution.RequestId = collectRequest.Oid;
            return collectRequest;

        }

        public CollectRequest CreateCompleteCollectRequestAfterCollectWithSystemCharacteristics(
            IDocumentSession session, string definitionsFilename, string SystemCharacteristicsFilename, string externalVariables)
        {
            var probeResult = new ProbeResultFactory().CreateProbeResultForRegistryCollect();
            var definitionsInXML = GetOvalObjectInXML(definitionsFilename);
            var systemCharacteristicsXml = GetSystemCharacteristicsInXML(SystemCharacteristicsFilename);
            var collectResult = new CollectResultFactory().CreateCollectResultForTheProbeExecution(probeResult);
            collectResult.SystemCharacteristics = systemCharacteristicsXml;
            var probeExecution = new CollectFactory(session).CreateAProbeExecution(probeResult, "registry");
            probeExecution.SystemCharacteristics = collectResult.SystemCharacteristics;

            var collectRequest = GetOvalObjectInXML(session, definitionsInXML, externalVariables);
            var collectExecution = CreateCollectExecution(session, collectRequest);
            collectExecution.ProbeExecutions.Add(probeExecution);
           // collectRequest.Collects.Add(collectExecution);
            
            CreateTarget(session, collectRequest);
            
            return collectRequest;
        }

        private CollectRequest GetOvalObjectInXML(
            IDocumentSession session, string definitions, string externalVariables)
        {
            var newDefinitiondoc = new DefinitionDocument() { OriginalId = "01", Text = definitions };
            session.Store(newDefinitiondoc);
            return new CollectRequest() 
            {
                OvalDefinitionsId = newDefinitiondoc.Oid,
                ExternalVariables = externalVariables
            };
        }

        private static CollectExecution CreateCollectExecution(IDocumentSession session, CollectRequest collectRequest)
        {
            var collectExecution = 
                new CollectExecution() { RequestId = collectRequest.Oid };

            collectExecution.SetDateStartCollect();
            collectExecution.SetDateEndCollect();
            
            return collectExecution;
        }


        public CollectRequest CreateCollectRequestIncompleteAfterCollect(IDocumentSession session)
        {
            CollectRequest collectRequest = new CollectRequest();

            string definitionsInXml = GetOvalObjectInXML(".definitions.fdcc_xpfirewall_oval.xml");
            StringBuilder builder = new StringBuilder(definitionsInXml);
            var newDefinitiondoc = new DefinitionDocument()
            {
                OriginalId = "01",
                Text = builder.ToString()
            };
            session.Store(newDefinitiondoc);
            collectRequest.OvalDefinitionsId = newDefinitiondoc.Oid ;
            session.Store(collectRequest);
            CollectExecution collectExecution = new CollectExecution();
            collectExecution.RequestId = collectRequest.Oid;
            session.Store(collectExecution);
            collectExecution.SetDateStartCollect();
            collectExecution.SetDateEndCollect();
            //collectRequest.Collects.Add(collectExecution);
            ProbeResult probeResult = new ProbeResultFactory().CreateProbeResultForRegistryCollect();

            CollectFactory collectFactory = new CollectFactory(session);
            ProbeExecution probeExecution = collectFactory.CreateAProbeExecution(probeResult, "registry");
            CollectResult collectResult = new CollectResultFactory().CreateCollectResultForTheProbeExecution(probeResult);
            string systemCharacteristicsXml = this.GetSystemCharacteristicsInXML(".system_characteristics.fdcc_xpfirewall_oval.sc_incomplete.xml");            
            collectResult.SystemCharacteristics = systemCharacteristicsXml;

            probeExecution.SystemCharacteristics = collectResult.SystemCharacteristics;
            collectExecution.ProbeExecutions.Add(probeExecution);
           // collectRequest.Collects.Add(collectExecution);
            this.CreateTarget(session, collectRequest);
            session.SaveChanges();
            return collectRequest;
        }

        private static Tuple<CollectPackage, CollectRequest> GEtCollectRequest(IDocumentSession session)
        {
            CollectRequest collectRequest = new CollectRequest();
            collectRequest.ReceivedOn = DateTime.Now;
            CollectPackage collectPackage = new CollectPackage();
            collectPackage.ScheduleInformation = new CollectScheduleInformation() { ExecutionDate = DateTime.Now };
            session.Store(collectPackage);
            collectRequest.CollectPackageId = collectPackage.Oid;
            return new Tuple<CollectPackage,CollectRequest>(collectPackage, collectRequest);
        }

        
        private string GetOvalObjectInXML(string xmlFile)
        {            
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name +  xmlFile);

            byte[] bytes = new byte[sampleDoc.Length];
            sampleDoc.Read(bytes, 0, (int)sampleDoc.Length);
            String xml = Encoding.ASCII.GetString(bytes);
            return xml;            
        }

        private string GetSystemCharacteristicsInXML(string xml)
        {
            IEnumerable<string> errors;
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name + xml);
            oval_system_characteristics systemCharacteristics = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);
            return systemCharacteristics.GetSystemCharacteristicsXML();
        }

        

       
    }
}
