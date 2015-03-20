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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.Family;
using Modulo.Collect.Probe.Unix.Family;
using Rhino.Mocks;
using Definitions = Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.Family
{
    [TestClass]
    public class FamilyProberTests
    {
        private const string FAKE_FAMILY = "Fedora Linux";

        private IList<IConnectionProvider> FakeContext;
        private TargetInfo FakeTargetInfo;

        public FamilyProberTests()
        {
            FakeContext = ProbeHelper.CreateFakeContext();
            FakeTargetInfo = ProbeHelper.CreateFakeTarget();
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_family_for_unix_based_systems()
        {
            var familyObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "oval:org.mitre.oval:obj:1000");
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new Definitions.ObjectType[] { familyObject }, null, null);
            var prober = this.GetMockedFamilyProberForUnix(null);

            var probeResult = prober.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            Assert.IsNotNull(probeResult);
            Assert.AreEqual(1, probeResult.CollectedObjects.Count());
            var collectedItems = probeResult.CollectedObjects.Single().SystemData;
            Assert.AreEqual(1, collectedItems.Count);

            var familyItem = (family_item)collectedItems.Single();
            Assert.AreEqual(familyItem.status, StatusEnumeration.exists);

            Assert.AreEqual(familyItem.family.Value, FAKE_FAMILY);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_an_error_occurs_while_creating_items_to_collect_the_created_items_must_have_error_status()
        {
            var fakeTargetInfo = ProbeHelper.CreateFakeTarget();
            var fakeContext = ProbeHelper.CreateFakeContext();
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:org.mitre.oval:obj:1000");
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new Definitions.ObjectType[] { objectType }, null, null);

            var prober = GetMockedFamilyProberForUnix(new Exception("Timeout"));
            var probeResult = prober.Execute(fakeContext, fakeTargetInfo, fakeCollectInfo);

            Assert.IsNotNull(probeResult);
            Assert.AreEqual(1, probeResult.CollectedObjects.Count());
            var collectedItems = probeResult.CollectedObjects.Single().SystemData;
            Assert.AreEqual(1, collectedItems.Count);

            var familyItem = (family_item)collectedItems.Single();
            Assert.AreEqual(familyItem.status, StatusEnumeration.error);
            Assert.IsNotNull(familyItem.message);
        }


        private FamilyProberUnix GetMockedFamilyProberForUnix(Exception exceptionToThrow)
        {
            MockRepository mocks = new MockRepository();
            var fakeConnMng = mocks.DynamicMock<IConnectionManager>();
            var fakeFamilyCollectorUnix = mocks.DynamicMock<FamilyCollectorUnix>();
            
            if (exceptionToThrow == null)
                Expect.Call(fakeFamilyCollectorUnix.GetOperatingSystemFamily()).Return(FAKE_FAMILY);
            else
                Expect.Call(fakeFamilyCollectorUnix.GetOperatingSystemFamily()).Throw(exceptionToThrow);
            
            mocks.ReplayAll();

            var FakeFamilyObjectCollector = new FamilyObjectCollector() { FamilyCollector = fakeFamilyCollectorUnix };
            return new FamilyProberUnix() { ConnectionManager = fakeConnMng, ObjectCollector = FakeFamilyObjectCollector };
        }

        private FamilyObjectCollector CreateMockedObjectCollector(string FAKE_FAMILY)
        {
            MockRepository mocks = new MockRepository();
            var fakeFamilyCollectorUnix = mocks.DynamicMock<FamilyCollectorUnix>();
            Expect.Call(fakeFamilyCollectorUnix.GetOperatingSystemFamily()).Return(FAKE_FAMILY);
            mocks.ReplayAll();

            return new FamilyObjectCollector() { FamilyCollector = fakeFamilyCollectorUnix };
        }

        private ItemType[] CreateFakeItemTypes()
        {
            var fakeItem = new family_item()
            {
                family = new EntityItemFamilyType() { Value = FAKE_FAMILY }
            };

            return new ItemType[] { fakeItem };
        }
    }
}
