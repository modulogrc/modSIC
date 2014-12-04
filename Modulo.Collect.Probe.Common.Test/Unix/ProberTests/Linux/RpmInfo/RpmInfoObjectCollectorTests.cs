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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Linux.RPMInfo;
using Rhino.Mocks;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.OVAL.SystemCharacteristics.Linux;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Checkers;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.Linux.RpmInfo
{
    [TestClass]
    public class RpmInfoObjectCollectorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_rpminfo_item()
        {
            var fakeRPMInfoCollector = CreateMockedRpmInfoCollector();
            var objectCollector = new RPMInfoObjectCollector() { RPMInfosCollector = fakeRPMInfoCollector };

            var collectedItems = objectCollector.CollectDataForSystemItem(CreateFakeRpmInfoItem()).ToArray();

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(rpminfo_item), true);
            var collectedItem = (rpminfo_item)collectedItems.Single().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.name, "fakePackage");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.version, "0.6.8");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.release, "9.fc13");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.arch, "x86_64");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.epoch, "1");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.evr, "1:0.6.8-9.fc13");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_map_LinuxPackageInfo()
        {
            var fakeLinuxPackageInfo = CreateFakeLinuxPackageInfo(true);

            Assert.AreEqual("wpa_supplicant", fakeLinuxPackageInfo.Name);
            Assert.AreEqual("0.6.8", fakeLinuxPackageInfo.Version);
            Assert.AreEqual("9.fc13", fakeLinuxPackageInfo.Release);
            Assert.AreEqual("x86_64", fakeLinuxPackageInfo.Arch);
            Assert.AreEqual((uint)0, fakeLinuxPackageInfo.Epoch);

            Assert.AreEqual("9.fc13", fakeLinuxPackageInfo.Revision);
            Assert.AreEqual("0.6.8-9.fc13", fakeLinuxPackageInfo.Evr);
        }

        private static LinuxPackageInfo CreateFakeLinuxPackageInfo(bool withoutEpoch)
        {
            // {NAME}          {VERSION}   {RELEASE}   {ARCH}   {EPOCH}
            // wpa_supplicant   0.6.8	    9.fc13	    x86_64	 1
            return new LinuxPackageInfo()
            {
                Name = "wpa_supplicant",
                Arch = "x86_64",
                Epoch = withoutEpoch ? 0 : (uint)1,
                Release = "9.fc13",
                Version = "0.6.8"
            };
        }

        private RPMInfoCollector CreateMockedRpmInfoCollector()
        {
            var mocks = new MockRepository();
            var fakeRpmInfoCollector = mocks.DynamicMock<RPMInfoCollector>();
            Expect.Call(fakeRpmInfoCollector.GetTargetRPMByPackageName(null))
                .IgnoreArguments()
                    .Return(CreateFakeLinuxPackageInfo(false));
            mocks.ReplayAll();

            return fakeRpmInfoCollector;
        }

        private rpminfo_item CreateFakeRpmInfoItem()
        {
            return new rpminfo_item()
            {
                name = OvalHelper.CreateItemEntityWithStringValue("fakePackage")
            };
        }
    }
}
