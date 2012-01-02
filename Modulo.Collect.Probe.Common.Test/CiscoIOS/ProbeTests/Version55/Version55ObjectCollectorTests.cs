/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.CiscoIOS.Probes.Version;
using Modulo.Collect.Probe.Common;
using Rhino.Mocks;
using Modulo.Collect.OVAL.SystemCharacteristics.Ios;
using Modulo.Collect.Probe.Common.Test.Checkers;

namespace Modulo.Collect.Probe.CiscoIOS.Tests.ProbeTests.Version55
{
    [TestClass]
    public class Version55ObjectCollectorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_version_item()
        {
            var fakeTelnetConnection = this.CreateTelnetConnectionMock();
            var objectCollector = new VersionObjectCollector() { TelnetConnection = fakeTelnetConnection };

            var collectedItems = objectCollector.CollectDataForSystemItem(new version_item());

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(version_item));
            var versionItem = (version_item)collectedItems.Single().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(versionItem.major_version, "12", "major_item");
            // #ToDo: must be check other entities.
        }

        private TelnetConnection CreateTelnetConnectionMock()
        {
            var mocks = new MockRepository();
            var fakeTelnetConnection = mocks.DynamicMock<TelnetConnection>();
            Expect.Call(fakeTelnetConnection.CiscoCommand("show version")).Return("IOS Version 12.2");
            mocks.ReplayAll();

            return fakeTelnetConnection;
        }

    }
}
