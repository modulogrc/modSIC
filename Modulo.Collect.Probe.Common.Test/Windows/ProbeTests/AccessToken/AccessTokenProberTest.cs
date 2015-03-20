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
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.AccessToken;
using Rhino.Mocks;


namespace Modulo.Collect.Probe.Windows.Test.AccessToken
{
    [TestClass]
    public class AccessTokenProberTest: ProberTestBase
    {
        private CollectInfo FakeCollectInfo;
        private ItemType[] FakeItemsToCollect;
        private CollectedItem[] FakeCollectedItems;

        public AccessTokenProberTest()
        {
            FakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:840");
            FakeItemsToCollect = new ItemType[] { new accesstoken_item() };
            FakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new accesstoken_item()) };
        }
        
 
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_user_privileges_through_AccessTokenProber()
        {
            var accessTokenProber = new AccessTokenProber() { WMIConnectionProvider = GetMockedWmiConnectionProvider() };
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    accessTokenProber,
                    FakeItemsToCollect,
                    FakeCollectedItems);

            ProbeResult result = accessTokenProber.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(result, typeof(accesstoken_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_AccessTokenObject_with_SET_element_defined()
        {
            var accessTokenProber = new AccessTokenProber() { WMIConnectionProvider = GetMockedWmiConnectionProvider() };
            var fakeCollectInfoForSetElement = GetFakeCollectInfo("oval:modulo:obj:880");
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("oval:modulo:obj:860", "oval:modulo:var:860", "ADMINISTRATOR");
            var fakeSystemCharacteristics = ProbeHelper.GetOvalSystemCharacteristicsFromFile("system_characteristics_with_sets.xml");
            fakeCollectInfoForSetElement.Variables = fakeVariables;
            fakeCollectInfoForSetElement.SystemCharacteristics = fakeSystemCharacteristics;
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    accessTokenProber,
                    FakeItemsToCollect,
                    FakeCollectedItems);


            var proberExecutionResult = accessTokenProber.Execute(FakeContext, FakeTargetInfo, fakeCollectInfoForSetElement);

            Assert.IsNotNull(proberExecutionResult);
            Assert.IsNotNull(proberExecutionResult.CollectedObjects);
            Assert.AreEqual(1, proberExecutionResult.CollectedObjects.Count());
            
            var collectedObject = proberExecutionResult.CollectedObjects.Single();
            Assert.AreEqual(2, collectedObject.ObjectType.reference.Count());
            Assert.AreEqual(collectedObject.ObjectType.reference.Count(), collectedObject.SystemData.Count);

            Assert.AreEqual(collectedObject.ObjectType.reference.ElementAt(0).item_ref, collectedObject.SystemData.ElementAt(0).id);
            Assert.AreEqual(collectedObject.ObjectType.reference.ElementAt(1).item_ref, collectedObject.SystemData.ElementAt(1).id);

            Assert.IsInstanceOfType(collectedObject.SystemData.ElementAt(0), typeof(accesstoken_item));
            Assert.IsInstanceOfType(collectedObject.SystemData.ElementAt(1), typeof(accesstoken_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void When_an_error_occurs_while_creating_items_to_collect_the_created_items_must_have_error_status()
        {
            var accessTokenProber = new AccessTokenProber() { WMIConnectionProvider = GetMockedWmiConnectionProvider() };
            var fakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:860");
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(accessTokenProber);

            var probeExecutionResult = accessTokenProber.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            DoAssertForExecutionWithErrors(probeExecutionResult, typeof(accesstoken_item));
        }

        private WMIConnectionProvider GetMockedWmiConnectionProvider()
        {
            MockRepository mocks = new MockRepository();
            var fakeWmiConnectionProvider = mocks.DynamicMock<WMIConnectionProvider>(string.Empty);
            mocks.ReplayAll();

            return fakeWmiConnectionProvider;
        }

        private void DoAssertForSetCase(ProbeResult executionResult, int expectedCollectedObjectsCount)
        {
            Assert.IsNotNull(executionResult, "The probe execution cannot be null.");
            Assert.IsNotNull(executionResult.ExecutionLog, "The probe execution log cannot be null");
            Assert.AreEqual(expectedCollectedObjectsCount, executionResult.CollectedObjects.Count(), "Only one collected object is expected for this test.");

            foreach (var collectedObject in executionResult.CollectedObjects)
            {
                var collectedItems = collectedObject.SystemData;
                Assert.AreEqual(collectedObject.ObjectType.reference.Count(), 1, "Unexpected number of item references was found.");
                Assert.AreEqual(collectedObject.ObjectType.reference.Count(), collectedItems.Count, "Unexpected number of generated items type was found.");
                Assert.AreEqual(1, collectedItems.Count, "Only one item is expected on system data.");
                Assert.IsInstanceOfType(collectedItems.Single(), typeof(accesstoken_item), "An unexpected instance of item type was found in system data.");
                Assert.AreEqual(StatusEnumeration.exists, collectedItems.Single().status, "An unexpected item status was found in system data.");
            }
        }
    }
}
