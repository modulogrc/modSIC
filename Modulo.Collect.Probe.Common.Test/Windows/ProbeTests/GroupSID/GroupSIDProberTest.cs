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
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.GroupSID;
using Modulo.Collect.Probe.Windows.Test.Helpers;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test;

namespace Modulo.Collect.Probe.Windows.Test.GroupSID
{
    [TestClass]
    public class GroupSIDProberTest: ProberTestBase
    {
        private CollectInfo FakeCollectInfo;
        private ItemType[] FakeItemsToReturnByItemTypeGenerator;
        private CollectedItem[] FakeCollectedItems;
        
        public GroupSIDProberTest()
        {
            FakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:1110");
            FakeItemsToReturnByItemTypeGenerator = new ItemType[] { new group_sid_item() };
            FakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(this.CreateFakeGroupSIDItemType()) };
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_GroupSIDObject()
        {
            var groupSIDProber = new GroupSIDProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    groupSIDProber, 
                    FakeItemsToReturnByItemTypeGenerator,
                    FakeCollectedItems);

            var probeExecutionResult = groupSIDProber.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);
            DoAssertForSingleCollectedObject(probeExecutionResult, typeof(group_sid_item));

            var collectedItem = (group_sid_item)probeExecutionResult.CollectedObjects.Single().SystemData.Single();
            Assert.AreEqual("S-1-32", collectedItem.group_sid.Value);
            Assert.AreEqual(2, collectedItem.user_sid.Count());
            Assert.AreEqual("S-1-0", collectedItem.user_sid.ElementAt(0).Value);
            Assert.AreEqual("S-1-1", collectedItem.user_sid.ElementAt(1).Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned()
        {
            var groupSIDProber = new GroupSIDProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(groupSIDProber);

            var probeExecutionResult = groupSIDProber.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(probeExecutionResult, typeof(group_sid_item));
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_GroupSIDObject_with_SET_element_defined()
        {
            var objectTypes = new Definitions.ObjectType[] { ProbeHelper.GetDefinitionObjectTypeByID("definitionsWithSet.xml", "oval:modulo:obj:1000") };
            var fakeSystemCharacteristics = ProbeHelper.GetOvalSystemCharacteristicsFromFile("system_characteristics_with_sets.xml");
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(objectTypes, null, fakeSystemCharacteristics);
            var fakeCollectedItem = ProbeHelper.CreateFakeCollectedItem(this.CreateFakeGroupSIDItemType());
            var groupSIDProber = new GroupSIDProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    groupSIDProber,
                    null,
                    new CollectedItem[] { fakeCollectedItem });

            
            var probeExecutionResult = groupSIDProber.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            Assert.IsNotNull(probeExecutionResult);
            Assert.AreEqual(1, probeExecutionResult.CollectedObjects.Count());

            var collectedObject = probeExecutionResult.CollectedObjects.ElementAt(0).ObjectType;
            Assert.AreEqual("oval:modulo:obj:1000", collectedObject.id);
            Assert.AreEqual(FlagEnumeration.complete, collectedObject.flag);
            Assert.AreEqual(2, collectedObject.reference.Count());
            Assert.AreEqual("1001", collectedObject.reference.ElementAt(0).item_ref);
            Assert.AreEqual("1002", collectedObject.reference.ElementAt(1).item_ref);
            
            var collectedItems = probeExecutionResult.CollectedObjects.ElementAt(0).SystemData;
            Assert.AreEqual(2, collectedItems.Count);
            var firstItem = (group_sid_item)collectedItems.ElementAt(0);
            this.AssertCollectedItem(firstItem, "1001", "S-1-5-32-500", new string[] { "S-1-5-32-500-1", "S-1-5-32-500-2" });
            var secondItem = (group_sid_item)collectedItems.ElementAt(1);
            this.AssertCollectedItem(secondItem, "1002", "S-1-5-32-501", new string[] { "S-1-5-32-501-1", "S-1-5-32-501-2" });
        }


        private void AssertCollectedItem(
            group_sid_item itemToAssert, string expectedObjectID, string expectedGroupSID, string[] expectedUserSIDs)
        {
            Assert.AreEqual(expectedObjectID, itemToAssert.id);
            Assert.AreEqual(StatusEnumeration.exists, itemToAssert.status);
            Assert.AreEqual(expectedGroupSID, itemToAssert.group_sid.Value);
            for(int i = 0; i < expectedUserSIDs.Count(); i++)
                Assert.AreEqual(expectedUserSIDs[i], itemToAssert.user_sid.ElementAt(i).Value);
        }

        private group_sid_item CreateFakeGroupSIDItemType()
        {
            return new group_sid_item()
            {
                group_sid = OvalHelper.CreateItemEntityWithStringValue("S-1-32"),
                user_sid = new EntityItemStringType[] { OvalHelper.CreateItemEntityWithStringValue("S-1-0"), OvalHelper.CreateItemEntityWithStringValue("S-1-1") }
            };

        }

    }
}
