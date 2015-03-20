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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Unix.RunLevel;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.RunLevel
{
    [TestClass]
    public class RunLevelProberTests: ProberTestBase
    {
        private ItemType FakeItemToCollect1;
        private ItemType FakeItemToCollect2;
        private CollectedItem FakeCollectedItem1;
        private CollectedItem FakeCollectedItem2;

        public RunLevelProberTests()
        {
            FakeItemToCollect1 = CreateFakeRunlevelItem("ssh", "2", null, null);
            FakeItemToCollect2 = CreateFakeRunlevelItem("cups", "3", null, null);
            FakeCollectedItem1 = ProbeHelper.CreateFakeCollectedItem(CreateFakeRunlevelItem("ssh", "2", "0", "1"));
            FakeCollectedItem2 = ProbeHelper.CreateFakeCollectedItem(CreateFakeRunlevelItem("cups", "3", "1", "0"));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_runlevel_object()
        {
            var prober = new RunLevelProber();
            new ProberBehaviorCreator()
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    new ItemType[] { new runlevel_item() },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new runlevel_item()) });

            var result = prober.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:2380"));

            DoAssertForSingleCollectedObject(result, typeof(runlevel_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_many_runlevel_objects()
        {
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(GetFakeRunlevelObjects(), null, null);
            var prober = this.CreateRunlevelProberToCollectManyRunlevelObjectsAtOnce();

            var proberExecutionResult = prober.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            Assert.IsNotNull(proberExecutionResult);
            Assert.IsNotNull(proberExecutionResult.CollectedObjects);
            Assert.AreEqual(2, proberExecutionResult.CollectedObjects.Count());

            var collectedObjects = proberExecutionResult.CollectedObjects;
            var collectedObject1 = collectedObjects.ElementAt(0).ObjectType;
            var collectedObject2 = collectedObjects.ElementAt(1).ObjectType;
            AssertCollectedObject(collectedObject1, "oval:modulo:obj:2380");
            AssertCollectedObject(collectedObject2, "oval:modulo:obj:2381");

            var systemDataOfCollectedObject1 = collectedObjects.ElementAt(0).SystemData;
            var systemDataOfCollectedObject2 = collectedObjects.ElementAt(1).SystemData;
            Assert.IsNotNull(systemDataOfCollectedObject1);
            Assert.IsNotNull(systemDataOfCollectedObject2);
            Assert.AreEqual(1, systemDataOfCollectedObject1.Count);
            Assert.AreEqual(1, systemDataOfCollectedObject2.Count);
            
            var referencedId1 = collectedObject1.reference.Single().item_ref;
            var referencedId2 = collectedObject2.reference.Single().item_ref;
            Assert.AreEqual(referencedId1, systemDataOfCollectedObject1.Single().id);
            Assert.AreEqual(referencedId2, systemDataOfCollectedObject2.Single().id);
            Assert.AreNotEqual(referencedId1, referencedId2);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_set_element_for_runlevel_objects()
        {
            var prober = new RunLevelProber();
            new ProberBehaviorCreator()
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    new ItemType[] { new runlevel_item() },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new runlevel_item()) });

            var result = prober.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:120", "definitions_all_unix"));

            Assert.IsNotNull(result);
            // Assert.AreEqual(2, result.CollectedObjects.Count());
            // DoAssertForSingleCollectedObject(result, typeof(runlevel_item));

        }


        private void AssertCollectedObject(ObjectType collectedObject, string expectedObjectID)
        {
            Assert.IsNotNull(collectedObject);
            Assert.AreEqual(expectedObjectID, collectedObject.id);
            var referencedItem = collectedObject.reference;
            Assert.IsNotNull(referencedItem);
            Assert.AreEqual(1, referencedItem.Count());
        }

        private OVAL.Definitions.ObjectType[] GetFakeRunlevelObjects()
        {
            var ovalDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            var fakeRunlevelObject1 = ProbeHelper.GetOvalComponentByOvalID(ovalDefinitions, "2380");
            var fakeRunlevelObject2 = ProbeHelper.GetOvalComponentByOvalID(ovalDefinitions, "2381");

            return new OVAL.Definitions.ObjectType[] { fakeRunlevelObject1, fakeRunlevelObject2 };
        }

        private RunLevelProber CreateRunlevelProberToCollectManyRunlevelObjectsAtOnce()
        {
            var mocks = new MockRepository();
            var fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();
            var fakeItemTypeGenerator = mocks.StrictMock<IItemTypeGenerator>();
            var fakeObjectCollector = mocks.DynamicMock<BaseObjectCollector>();

            Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null)).IgnoreArguments()
                .Return(new ItemType[] { FakeItemToCollect1 });
            Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null)).IgnoreArguments()
                .Return(new ItemType[] { FakeItemToCollect2 });

            Expect.Call(fakeObjectCollector.CollectDataForSystemItem(FakeItemToCollect1))
                .Return(new CollectedItem[] { FakeCollectedItem1 });
            Expect.Call(fakeObjectCollector.CollectDataForSystemItem(FakeItemToCollect2))
                .Return(new CollectedItem[] { FakeCollectedItem2 });

            mocks.ReplayAll();

            return new RunLevelProber()
            {
                ConnectionManager = fakeConnectionManager,
                ItemTypeGenerator = fakeItemTypeGenerator,
                ObjectCollector = fakeObjectCollector
            };
        }

        private ItemType CreateFakeRunlevelItem(string serviceName, string runlevel, string start, string kill)
        {
            return
                new runlevel_item()
                {
                    service_name = OvalHelper.CreateItemEntityWithStringValue(serviceName),
                    runlevel = OvalHelper.CreateItemEntityWithStringValue(runlevel),
                    start = start == null ? null : OvalHelper.CreateItemEntityWithBooleanValue(start),
                    kill = kill == null ? null: OvalHelper.CreateItemEntityWithBooleanValue(kill)
                };
        }
    }
}
