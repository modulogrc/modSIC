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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Service.Controllers;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Probes;
using Modulo.Collect.Probe.Windows.SystemInformationService;
using Modulo.Collect.Service.Tests.Entities;
using Modulo.Collect.Probe.Common.Exceptions;
using Raven.Client;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Service.Tests.Helpers
{
    public class CollectExecutionManagerFactory
    {
        private IProbeManager probeManager;
        private IProbe probe;
        private IProbe familyProbe;        
        private TargetInfo target;
        private List<IConnectionProvider> connectionContext;
        private IDataProvider dataProvider;
        private IDocumentSession session;
        private IEnumerable<ObjectType> ovalObjects;
        private ISystemInformationService systemInformationService;
        private CollectRequest collectRequest;        

        public CollectExecutionManagerFactory(CollectRequest collectRequest, IDocumentSession session)
        {
            this.session = session;
            target = this.CreateTargetInfo();
            this.collectRequest = collectRequest;
            this.ovalObjects = collectRequest.GetObjectTypes(session);
        }        

        public CollectExecutionManager CreateExecutionManagerForTheSuccessScenario(ProbeResult resultForRegistry, ProbeResult resultForFamily)
        {           
            MockRepository mocks = new MockRepository();
            probeManager = mocks.DynamicMock<IProbeManager>();
            probe = mocks.DynamicMock<IProbe>();
            familyProbe = mocks.DynamicMock<IProbe>();
            dataProvider = mocks.DynamicMock<IDataProvider>();
            systemInformationService = mocks.DynamicMock<ISystemInformationService>();
            connectionContext = this.CreateConnectionContext();

            CollectInfo collectInfo = new CollectInfo() { ObjectTypes = ovalObjects };

            Expect.Call(probeManager.GetProbesFor(null, FamilyEnumeration.windows)).IgnoreArguments().Repeat.Any().Return(this.GetSelectedProbes());
            Expect.Call(probeManager.GetSystemInformationService(FamilyEnumeration.windows)).IgnoreArguments().Repeat.Any().Return(systemInformationService);
            Expect.Call(probe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Repeat.Any().Return(resultForRegistry);
            Expect.Call(familyProbe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Repeat.Any().Return(resultForFamily);
            Expect.Call(dataProvider.GetSession()).Repeat.Any().Return(session).IgnoreArguments();
            Expect.Call(dataProvider.GetTransaction(session)).Repeat.Any().Return(new Transaction(session)).IgnoreArguments();
            Expect.Call(systemInformationService.GetSystemInformationFrom(target)).IgnoreArguments().Return(this.GetExpectedSystemInformation());

            mocks.ReplayAll();

            return new CollectExecutionManager(probeManager) { Target = target, connectionContext = connectionContext };

        }

        public CollectExecutionManager CreateExecutionManagerWithCannotConnectToHostExceptionScenario(
            ProbeResult resultForRegistry, ProbeResult resultForFamily, bool exceptionOnSystemInformationGathering = false)
        {
            MockRepository mocks = new MockRepository();
            probeManager = mocks.DynamicMock<IProbeManager>();
            probe = mocks.DynamicMock<IProbe>();
            familyProbe = mocks.DynamicMock<IProbe>();
            dataProvider = mocks.DynamicMock<IDataProvider>();
            

            var fakeCannotConnectException = new CannotConnectToHostException(target, "Cannot Connection To Host...");
            CollectInfo collectInfo = new CollectInfo() { ObjectTypes = ovalObjects };

            Expect.Call(probeManager.GetProbesFor(null, FamilyEnumeration.windows)).Repeat.Any().IgnoreArguments().Return(GetSelectedProbes());
            Expect.Call(probe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Throw(fakeCannotConnectException);
            Expect.Call(familyProbe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Return(resultForFamily);
            Expect.Call(dataProvider.GetSession()).Repeat.Any().Return(session);
            Expect.Call(dataProvider.GetTransaction(session)).Repeat.Any().Return(new Transaction(session));
            if (exceptionOnSystemInformationGathering)
            {
                systemInformationService = mocks.DynamicMock<ISystemInformationService>();
                Expect.Call(probeManager.GetSystemInformationService(FamilyEnumeration.windows)).IgnoreArguments().Return(systemInformationService);
                Expect.Call(systemInformationService.GetSystemInformationFrom(null)).IgnoreArguments().Throw(fakeCannotConnectException);
            }

            mocks.ReplayAll();

            return new CollectExecutionManager(probeManager) { Target = target, connectionContext = connectionContext };
        }

        public CollectExecutionManager CreateExecutionManagerWWithInvalidCredentialsScenario(ProbeResult resultForRegistry, ProbeResult resultForFamily)
        {
            MockRepository mocks = new MockRepository();
            probeManager = mocks.DynamicMock<IProbeManager>();
            probe = mocks.DynamicMock<IProbe>();
            familyProbe = mocks.DynamicMock<IProbe>();
            dataProvider = mocks.DynamicMock<IDataProvider>();

            CollectInfo collectInfo = new CollectInfo() { ObjectTypes = ovalObjects };

            Expect.Call(probeManager.GetProbesFor(null, FamilyEnumeration.windows)).IgnoreArguments().Return(GetSelectedProbes());
            Expect.Call(probe.Execute(connectionContext, target, collectInfo)).Throw(new InvalidCredentialsException("The credentials is not valid", null));
            Expect.Call(familyProbe.Execute(connectionContext, target, collectInfo)).IgnoreArguments().Return(resultForFamily);
            Expect.Call(dataProvider.GetSession()).Repeat.Any().Return(session).IgnoreArguments();
            Expect.Call(dataProvider.GetTransaction(session)).Repeat.Any().Return(new Transaction(session)).IgnoreArguments();

            mocks.ReplayAll();

            return new CollectExecutionManager(probeManager) { Target = target, connectionContext = connectionContext};
        }

        private SystemInformation GetExpectedSystemInformation()
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

        private TargetInfo CreateTargetInfo()
        {
            TargetInfo target = new TargetInfo();
            target.Add("HostName", "mss-rj-215");
            return target;
        }       
        private List<IConnectionProvider> CreateConnectionContext()
        {
            List<IConnectionProvider> connectionContext = new List<IConnectionProvider>();
            return connectionContext;
        }

        private IEnumerable<Probes.SelectedProbe> GetSelectedProbes()
        {
            SelectedProbe registry = this.CreateSelectedProbe<registry_object>(probe,"registry", FamilyEnumeration.windows, collectRequest);
            SelectedProbe family = this.CreateSelectedProbe<family_object>(familyProbe,"family", FamilyEnumeration.windows, collectRequest);

            List<SelectedProbe> selectedProbes = new List<SelectedProbe>();
            selectedProbes.Add(registry);
            selectedProbes.Add(family);
            return selectedProbes;
        }

        private SelectedProbe CreateSelectedProbe<T>(IProbe probe,string capability, FamilyEnumeration plataform, CollectRequest collectRequest) where T: ObjectType
        {
            SelectedProbe selectedProbe = new SelectedProbe(probe,
                                                         collectRequest.GetObjectTypes(session).OfType<T>(),
                                                         new ProbeCapabilities()
                                                         {
                                                             OvalObject = capability,
                                                             PlataformName = plataform
                                                         });
            return selectedProbe;
        }
       


        

    }
}
