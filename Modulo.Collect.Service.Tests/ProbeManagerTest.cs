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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Probe.Unix.Family;
using Modulo.Collect.Probe.Windows.Family;
using Modulo.Collect.Probe.Windows.Registry;
using Modulo.Collect.Probe.Windows.SystemInformationService;
using Modulo.Collect.Service.Controllers;
using Modulo.Collect.Service.Probes;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.CiscoIOS.Probes.Line;


namespace Modulo.Collect.Service.Tests
{
    [TestClass]
    public class ProbeManagerTest
    {

        private ProbeManager ProbeManager;
        
        [TestInitialize()]
        public void MyTestInitialize()
        {
            var mocks = new MockRepository();
            Func<IProbe> registryMethod = delegate() { return new RegistryProber(); };
            Func<IProbe> familyProberWindowsMethod = delegate { return new FamilyProberWindows(); };
            Func<IProbe> familyProberUnixMethod = delegate { return new FamilyProberUnix(); };
            Func<IProbe> lineProberCiscoIosMethod = delegate { return new LineProber(); };
            Func<ISystemInformationService> systemInformationMethod = delegate() { return new WindowsSystemInformationService(); };

            var registryProbe =
                mocks.StrictMock<Lazy<IProbe, IProbeCapabilities>>(
                    registryMethod, GetProbeCapability("registry", FamilyEnumeration.windows));
            var familyProberWindows =
                mocks.StrictMock<Lazy<IProbe, IProbeCapabilities>>(
                    familyProberWindowsMethod, GetProbeCapability("family", FamilyEnumeration.windows));
            var familyProberUnix =
                mocks.StrictMock<Lazy<IProbe, IProbeCapabilities>>(
                    familyProberUnixMethod, GetProbeCapability("family", FamilyEnumeration.unix));
            var lineProberCiscoIOS =
                mocks.StrictMock<Lazy<IProbe, IProbeCapabilities>>(
                    lineProberCiscoIosMethod, GetProbeCapability("line", FamilyEnumeration.ios));
                    
            var probes = GetProbeCapabilities(registryProbe, familyProberWindows, familyProberUnix, lineProberCiscoIOS);

            var windowsInformationService =
                mocks.StrictMock<Lazy<ISystemInformationService, ISystemInformationServicePlataform>>(
                    systemInformationMethod, GetSystemInformationSvcForWindows());
            
            var systemInformations =
                new Lazy<ISystemInformationService, ISystemInformationServicePlataform>[] { windowsInformationService };
            
            ProbeManager = new ProbeManager() { probes = probes, systemInformationServices = systemInformations };
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possibe_to_get_capabilities_of_probes()
        {
            var capabilities = ProbeManager.GetCapabilities();
            Assert.IsNotNull(capabilities);

            var capability = 
                capabilities.Single(
                    p => (p.PlataformName == FamilyEnumeration.windows) && (p.OvalObject == "registry"));

            Assert.IsNotNull(capability);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_probe_given_a_capabilities()
        {
            var probeCapabilities = GetProbeCapability("registry", FamilyEnumeration.windows);
            var probe = ProbeManager.GetProbe(probeCapabilities);
            
            Assert.IsNotNull(probe);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_have_return_null_if_not_exists_a_probe_with_the_capability_informed()
        {
            var probeCapabilities = GetProbeCapability("environmentvariable", FamilyEnumeration.windows);
            var probe = ProbeManager.GetProbe(probeCapabilities);

            Assert.IsNull(probe);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_the_correct_probe_given_a_list_of_objectTypes()
        {
            var ovalDefinitions = this.GetFakeOvalDefinitions();

            var selectedProbes = ProbeManager.GetProbesFor(ovalDefinitions.objects, FamilyEnumeration.windows);

            Assert.IsNotNull(selectedProbes);
            Assert.AreEqual(2, selectedProbes.Count());
            Assert.AreEqual("family", selectedProbes.ElementAt(0).Capability.OvalObject);
            Assert.AreEqual("registry", selectedProbes.ElementAt(1).Capability.OvalObject);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_ciscoIOS_probes()
        {
            var ovalDefinitions = ProbeHelper.GetFakeOvalDefinitions("modulo-ios122-oval");

            var selectedProbes = ProbeManager.GetProbesFor(ovalDefinitions.objects, FamilyEnumeration.ios);

            Assert.IsNotNull(selectedProbes);
            Assert.AreEqual(1, selectedProbes.Count());
            Assert.AreEqual("line", selectedProbes.ElementAt(0).Capability.OvalObject);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_the_SystemInformationService_by_plataform_name()
        {
            var systemInformationService = ProbeManager.GetSystemInformationService(FamilyEnumeration.windows);
            Assert.IsNotNull(systemInformationService);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_have_return_null_if_not_exists_a_systemInformationService_with_the_plataform_informed()
        {
            var systemInformationService = ProbeManager.GetSystemInformationService(FamilyEnumeration.unix);
            Assert.IsNull(systemInformationService);
        }



        private oval_definitions GetFakeOvalDefinitions()
        {
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name + ".definitions.fdcc_xpfirewall_oval.xml");

            IEnumerable<string> errors;
            return oval_definitions.GetOvalDefinitionsFromStream(sampleDoc, out errors);
        }

        private Lazy<IProbe, IProbeCapabilities>[] GetProbeCapabilities(
            Lazy<IProbe, IProbeCapabilities> capability1,
            Lazy<IProbe, IProbeCapabilities> capability2,
            Lazy<IProbe, IProbeCapabilities> capability3,
            Lazy<IProbe, IProbeCapabilities> capability4)
        {
            return new Lazy<IProbe, IProbeCapabilities>[]
            {
                capability1,
                capability2,
                capability3,
                capability4
            };
        }

        private IProbeCapabilities GetProbeCapability(string ovalObject, FamilyEnumeration plataform)
        {
            return new ProbeCapabilities() { OvalObject = ovalObject, PlataformName = plataform };
        }

        private object GetSystemInformationSvcForWindows()
        {
            return new SystemInformationServicePlataform() { PlataformName = FamilyEnumeration.windows };
        }


    }


}
