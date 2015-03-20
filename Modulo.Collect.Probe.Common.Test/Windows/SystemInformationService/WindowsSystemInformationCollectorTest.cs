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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Windows.SystemInformationService;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Windows.Test.SystemInformationService
{
    [TestClass]
    public class WindowsSystemInformationCollectorTest
    {
        private WmiDataProvider WmiDataProvider;
        private TargetInfo FakeTargetInfo;

        public WindowsSystemInformationCollectorTest()
        {
            this.WmiDataProvider = new WmiDataProviderExpectFactory().GetFakeWmiDataProviderForTestExecutingQueriesForSystemInfo();            
            this.FakeTargetInfo = ProbeHelper.CreateFakeTarget("MSS-RJ-215", "MSS", "lcosta", "password");
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_get_system_informations_of_windows_plataform()
        {
            var systemInfo = new WindowsSystemInformationCollector(WmiDataProvider).GetSystemInformation();            

            Assert.AreEqual(GetExpectedSystemInformation(), systemInfo, "This system info is not expected.");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_get_system_informations_about_the_primaryHostName()
        {
            var expectedPrimaryHostname = GetExpectedSystemInformation().PrimaryHostName;
            var systemInfo = new WindowsSystemInformationCollector(WmiDataProvider).GetSystemInformation();

            Assert.AreEqual(expectedPrimaryHostname, systemInfo.PrimaryHostName, "This primary hostname property is not expected.");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_get_system_informations_about_the_operationalSystem()
        {
            var expectedSystemInfo = this.GetExpectedSystemInformation();
            
            var systemInfo = new WindowsSystemInformationCollector(WmiDataProvider).GetSystemInformation();

            Assert.AreEqual(expectedSystemInfo.SystemName, systemInfo.SystemName, "the system name is not expected");
            Assert.AreEqual(expectedSystemInfo.SystemVersion, systemInfo.SystemVersion, "the system version is not expected");
            Assert.AreEqual(expectedSystemInfo.Architecture, systemInfo.Architecture, "the architecture is not expected");            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_get_system_informations_about_the_networkInterfaces()
        {
            var expectedSystemInfo = this.GetExpectedSystemInformation();

            var systemInfo = new WindowsSystemInformationCollector(WmiDataProvider).GetSystemInformation();

            Assert.AreEqual(expectedSystemInfo.Interfaces.Count(), systemInfo.Interfaces.Count(), "the amount of network interfaces is not expected");
            Assert.AreEqual(expectedSystemInfo.Interfaces[0], systemInfo.Interfaces[0], "the network interfaces property is not expected");            
        }

        private SystemInformation GetExpectedSystemInformation()
        {
            return SystemInformationFactory.GetExpectedSystemInformation();
        }
    }
}
