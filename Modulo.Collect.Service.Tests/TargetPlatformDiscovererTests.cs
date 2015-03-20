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
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, ORa princ
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Service.Infra;
using Modulo.Collect.Service.Tests.Helpers;
using Modulo.Collect.OVAL.Helper;

namespace Modulo.Collect.Service.Tests
{
    [TestClass]
    public class TargetPlatformDiscovererTests
    {
        private OVAL.Definitions.oval_definitions WindowsOvalDefinitionsSample;
        private OVAL.Definitions.oval_definitions UnixOvalDefinitionsSample;
        private OVAL.Definitions.oval_definitions IndependentOvalDefinitionsSample;
        private OVAL.Definitions.oval_definitions CiscoIosOvalDefinitionsSample;

        public TargetPlatformDiscovererTests()
        {
            var ovalDocLoader = new OvalDocumentLoader();
            this.WindowsOvalDefinitionsSample = ovalDocLoader.GetFakeOvalDefinitions("fdcc_xpfirewall_oval.xml");
            this.UnixOvalDefinitionsSample = ovalDocLoader.GetFakeOvalDefinitions("RM7-scap-sol10-oval.xml");
            this.IndependentOvalDefinitionsSample = ovalDocLoader.GetFakeOvalDefinitions("definitions_all_independent.xml");
            this.CiscoIosOvalDefinitionsSample = ovalDocLoader.GetFakeOvalDefinitions("modulo-ios122-oval.xml");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_discover_windows_systems_by_windows_oval_objects()
        {
            var targetPlatformDiscoverer = 
                new TargetPlatformDiscoverer(WindowsOvalDefinitionsSample.objects);

            var discoveringResult = targetPlatformDiscoverer.Discover();

            Assert.AreEqual(FamilyEnumeration.windows, discoveringResult);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_discover_solaris_systems_by_solaris_oval_objects()
        {
            var targetPlatformDiscoverer = 
                new TargetPlatformDiscoverer(UnixOvalDefinitionsSample.objects);

            var discoveringResult = targetPlatformDiscoverer.Discover();

            Assert.AreEqual(FamilyEnumeration.unix, discoveringResult);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_there_are_no_windows_or_unix_objects_the_platform_must_be_undefinied()
        {
            var targetPlatformDiscoverer = new TargetPlatformDiscoverer(IndependentOvalDefinitionsSample.objects);

            var discoveringResult = targetPlatformDiscoverer.Discover();

            Assert.AreEqual(FamilyEnumeration.undefined, discoveringResult);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_there_are_no_objects_the_platform_must_be_undefined()
        {
            var targetPlatformDiscoverer = new TargetPlatformDiscoverer(null);

            var discoveringResult = targetPlatformDiscoverer.Discover();

            Assert.AreEqual(FamilyEnumeration.undefined, discoveringResult);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_discover_cisco_ios_systems_by_ios_oval_objects()
        {
            var targetPlatformDiscoverer = new TargetPlatformDiscoverer(this.CiscoIosOvalDefinitionsSample.objects);

            var discoveringResult = targetPlatformDiscoverer.Discover();

            Assert.AreEqual(FamilyEnumeration.ios, discoveringResult);
        }

    }
}
