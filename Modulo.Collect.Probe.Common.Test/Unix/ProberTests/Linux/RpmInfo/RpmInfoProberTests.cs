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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Linux;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Linux.RPMInfo;
using Rhino.Mocks;
using System;
using System.Text.RegularExpressions;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.RpmInfo
{
    [TestClass]
    public class RpmInfoProberTests : ProberTestBase
    {
        private ItemType FakeItemToCollect1;
        private ItemType FakeItemToCollect2;
        private CollectedItem FakeCollectedItem1;
        private CollectedItem FakeCollectedItem2;

        public RpmInfoProberTests()
        {
            FakeItemToCollect1 = CreateFakeRpmInfoItem("vbox", null, null, null, null, null);
            FakeItemToCollect2 = CreateFakeRpmInfoItem("firefox", null, null, null, null, null);

            var fakeCollectedItem1 = CreateFakeRpmInfoItem("vbox", "x86", "2010", "1020", "12345678", "1");
            FakeCollectedItem1 = ProbeHelper.CreateFakeCollectedItem(fakeCollectedItem1);
            var fakeCollectedItem2 = CreateFakeRpmInfoItem("firefox", "x86", "2010", "1020", "12345678", "1");
            FakeCollectedItem2 = ProbeHelper.CreateFakeCollectedItem(fakeCollectedItem2);
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_many_rpminfo_objects()
        {
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(GetFakeRpmInfoObjects(), null, null);
            var prober = this.CreateRunlevelProberToCollectManyRunlevelObjectsAtOnce();

            var proberExecutionResult = prober.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            Assert.IsNotNull(proberExecutionResult);
            Assert.IsNotNull(proberExecutionResult.CollectedObjects);
            Assert.AreEqual(2, proberExecutionResult.CollectedObjects.Count());

            var collectedObjects = proberExecutionResult.CollectedObjects;
            var collectedObject1 = collectedObjects.ElementAt(0).ObjectType;
            var collectedObject2 = collectedObjects.ElementAt(1).ObjectType;
            AssertCollectedObject(collectedObject1, "oval:modulo:obj:2390");
            AssertCollectedObject(collectedObject2, "oval:modulo:obj:2391");

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
        public void Should_be_possible_to_use_set_element_in_rpminfo_object()
        {
            var prober = new RpmInfoProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    new ItemType[] { FakeItemToCollect1, FakeItemToCollect2 },
                    new CollectedItem[] { FakeCollectedItem1, FakeCollectedItem2 });

            var result =
                prober.Execute(
                    FakeContext,
                    FakeTargetInfo,
                    GetFakeCollectInfo("oval:modulo:obj:100", "definitions_all_linux"));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [Owner("lfernandes")]
        [TestCategory("Learning")]
        public void Learning_how_to_write_a_regular_expression_eliminate_digits_from_string_matching()
        {
            try
            {
                string NO_DIGITS_PATTERN = @"^5[^\d]+"; // Another valid expression: @"^5[^0-9]+";
                Assert.IsTrue(Regex.IsMatch("5Server", NO_DIGITS_PATTERN));
                Assert.IsFalse(Regex.IsMatch("51Server", NO_DIGITS_PATTERN));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        private ItemType CreateFakeRpmInfoItem(
            string name, string arch, string epoch, string release, string signKeyID, string version)
        {
            return
                new rpminfo_item()
                {
                    arch = OvalHelper.CreateItemEntityWithStringValue(arch),
                    epoch = new rpminfo_itemEpoch() { Value = epoch },
                    name = OvalHelper.CreateItemEntityWithStringValue(name),
                    release = new rpminfo_itemRelease() { Value = release },
                    signature_keyid = OvalHelper.CreateItemEntityWithStringValue(signKeyID),
                    version = new rpminfo_itemVersion() { Value = version }
                };
        }

        private RpmInfoProber CreateRunlevelProberToCollectManyRunlevelObjectsAtOnce()
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

            return new Modulo.Collect.Probe.Linux.RPMInfo.RpmInfoProber()
            {
                ConnectionManager = fakeConnectionManager,
                ItemTypeGenerator = fakeItemTypeGenerator,
                ObjectCollector = fakeObjectCollector
            };
        }

        private OVAL.Definitions.ObjectType[] GetFakeRpmInfoObjects()
        {
            var ovalDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            var fakeRunlevelObject1 = ProbeHelper.GetOvalComponentByOvalID(ovalDefinitions, "2390");
            var fakeRunlevelObject2 = ProbeHelper.GetOvalComponentByOvalID(ovalDefinitions, "2391");

            return new OVAL.Definitions.ObjectType[] { fakeRunlevelObject1, fakeRunlevelObject2 };
        }

        private void AssertCollectedObject(ObjectType collectedObject, string expectedObjectID)
        {
            Assert.IsNotNull(collectedObject);
            Assert.AreEqual(expectedObjectID, collectedObject.id);
            var referencedItem = collectedObject.reference;
            Assert.IsNotNull(referencedItem);
            Assert.AreEqual(1, referencedItem.Count());
        }
    }
}
