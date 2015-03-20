/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.Probe.Common;
using Rhino.Mocks;
using Modulo.Collect.Probe.Solaris.Probes.smf;
using Modulo.Collect.OVAL.SystemCharacteristics.Solaris;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Solaris.Tests.ProberTests.SMF
{
    [TestClass]
    public class SMFObjectCollectorTests
    {
        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_collect_a_smf_item()
        {
            var fakeSmfInfoToReturn = CreateFakeSmfInfo(false);
            var smfObjectCollector = CreateSmfObjectCollector(fakeSmfInfoToReturn);
            var fakeItemToCollect = CreateFakeItemToCollect();

            var collectedItems = smfObjectCollector.CollectDataForSystemItem(fakeItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(smf_item), true);
            var collectedSmfItem = (smf_item)collectedItems.Single().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSmfItem.service_name, "a");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSmfItem.service_state, "s");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSmfItem.protocol, "d");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSmfItem.server_executable, "f");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSmfItem.server_arguements, "g");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSmfItem.exec_as_user, "h");
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_handle_fmri_entity_with_invalid_format()
        {
            var smfObjectCollector = CreateSmfObjectCollector(null);
            var fakeItemToCollect = CreateFakeItemToCollect();

            var collectedItems = smfObjectCollector.CollectDataForSystemItem(fakeItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(smf_item), false);
            var collectedItem = (smf_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.error, collectedItem.status);
            Assert.IsNotNull(collectedItem.message);
            Assert.AreEqual(MessageLevelEnumeration.error, collectedItem.message.First().level);
            Assert.AreEqual("The fmri format is invalid.", collectedItem.message.First().Value);
            Assert.AreEqual(StatusEnumeration.error, collectedItem.fmri.status);
        }

        private smf_item CreateFakeItemToCollect()
        {
            return new smf_item()
            {
                fmri = OvalHelper.CreateItemEntityWithStringValue("davi")
            };
        }

        private BaseObjectCollector CreateSmfObjectCollector(
            SolarisSmfInfo fakeSmfInfoToReturn)
        {
            var mocks = new MockRepository();
            var fakeSmfCollector = mocks.DynamicMock<SolarisSmfCollector>();
            Expect.Call(fakeSmfCollector.GetSmfInfo(null))
                .IgnoreArguments()
                    .Return(fakeSmfInfoToReturn);

            mocks.ReplayAll();

            return new SMFObjectCollector()
            {
                SMFCollector = fakeSmfCollector
            };
        }

        private SolarisSmfInfo CreateFakeSmfInfo(bool noSmfInfo)
        {
            var newSmfInfo = noSmfInfo ? null :
                new SolarisSmfInfo()
                {
                    frmi = "davi",
                    ServiceName = "a",
                    ServiceState = "s",
                    Protocol = "d",
                    ServerExecutable = "f",
                    ServerArgs = "g",
                    ExecAsUser = "h"
                };
            
            return newSmfInfo;
        }
    }
}
