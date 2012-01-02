/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Family;
using Rhino.Mocks;
using Modulo.Collect.Probe.Unix.Family;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.Family
{
    [TestClass]
    public class FamilyObjectCollectorTest
    {
        private const string FAKE_FAMILY = "Fedora Linux";

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_a_family_item()
        {
            family_item fakeFamilyItem = new family_item();
            FamilyObjectCollector objectCollector = CreateMockedObjectCollector(FAKE_FAMILY);

            var collectedItems = objectCollector.CollectDataForSystemItem(fakeFamilyItem);

            //  Asserts
            var collectedItem = (family_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status);

            Assert.IsNotNull(collectedItem.family);
            Assert.IsFalse(string.IsNullOrEmpty(collectedItem.family.Value));
            Assert.AreEqual(collectedItem.family.Value, FAKE_FAMILY);
        }

        private FamilyObjectCollector CreateMockedObjectCollector(string FAKE_FAMILY)
        {
            MockRepository mocks = new MockRepository();
            var fakeFamilyCollectorUnix = mocks.DynamicMock<FamilyCollectorUnix>();
            Expect.Call(fakeFamilyCollectorUnix.GetOperatingSystemFamily()).Return(FAKE_FAMILY);
            mocks.ReplayAll();

            return new FamilyObjectCollector() { FamilyCollector = fakeFamilyCollectorUnix };
        }
    }
}
