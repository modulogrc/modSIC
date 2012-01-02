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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.Family;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Rhino.Mocks;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass]
    public class FamilyProberTest
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_Be_Possible_To_Execute_A_Family_Probe()
        {
            var fakeDefinitions = ProbeHelper.GetFakeOvalDefinitions("fdcc_xpfirewall_oval_regex_on_value.xml");
            var fakeCollectInfo = new CollectInfo() { ObjectTypes = fakeDefinitions.objects.OfType<family_object>() };
            
            ProbeResult result = 
                GetFamilyProber()
                    .Execute(null, ProbeHelper.CreateFakeTarget(), fakeCollectInfo);            

            var collectedFamily = result.CollectedObjects.ElementAt(0);
            Assert.IsNotNull(result.CollectedObjects);
            Assert.AreEqual(1, result.CollectedObjects.Count());
            Assert.AreEqual(1, collectedFamily.ObjectType.reference.Count());
            Assert.AreEqual(collectedFamily.ObjectType.reference.Count(), collectedFamily.SystemData.Count);
            Assert.AreEqual("oval:modulo:obj:99", collectedFamily.ObjectType.id);
            Assert.AreEqual(collectedFamily.ObjectType.reference[0].item_ref, collectedFamily.SystemData[0].id);
            
            var familyItem = (family_item)collectedFamily.SystemData[0];
            Assert.AreEqual(StatusEnumeration.exists, familyItem.status);
            Assert.AreEqual("Windows", familyItem.family.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void For_Windows_Operation_Systems_the_value_of_family_item_should_be_equals_to_windows()
        {
            var nmapService = new NMAPService(null, null);

            var simpleOSNameForWindowsXP = nmapService.GetSimpleOSName("Microsoft Windows XP");
            var simpleOSNameForWindows7 = nmapService.GetSimpleOSName("WiNdOwS 7");
            
            Assert.AreEqual("windows", simpleOSNameForWindowsXP, "The simple OS name is incorrect.");
            Assert.AreEqual("windows", simpleOSNameForWindows7, "The simple OS name is incorrect.");
        }

        [TestMethod, Owner("lfernandes")]
        public void For_OperationSystems_different_of_Windows_the_value_of_family_item_should_be_equals_to_real_operation_system_name()
        {
            var nmapService = new NMAPService(string.Empty, string.Empty);

            var simpleOSNameForLindows = nmapService.GetSimpleOSName("Lindows");

            Assert.AreEqual("Lindows", simpleOSNameForLindows, "The simple OS name is incorrect.");
        }

        private FamilyProberWindows GetFamilyProber()
        {
            MockRepository mocks = new MockRepository();
            var fakeFamilyDataSource = mocks.DynamicMock<BaseObjectCollector>();
            var fakeInformationService = mocks.DynamicMock<ISystemInformationService>();
            var fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();

            Expect.Call(fakeFamilyDataSource.CollectDataForSystemItem(null)).IgnoreArguments().Return(this.createFakeFamilyItem());
            Expect.Call(fakeInformationService.GetSystemInformationFrom(null)).IgnoreArguments().Return(SystemInformationFactory.GetExpectedSystemInformation());
            mocks.ReplayAll();

            return new FamilyProberWindows() { ObjectCollector = fakeFamilyDataSource, ConnectionManager = fakeConnectionManager };
            
        }

        private IEnumerable<CollectedItem> createFakeFamilyItem()
        {
            var familyItemType = new EntityItemFamilyType() { Value = "Windows" };
            var family = new family_item() { id = "1", family = familyItemType };
            var collectedItem = new CollectedItem() { ItemType = family, ExecutionLog = new List<ProbeLogItem>() };
            return new List<CollectedItem>() { collectedItem } ;
        }
    }
}
