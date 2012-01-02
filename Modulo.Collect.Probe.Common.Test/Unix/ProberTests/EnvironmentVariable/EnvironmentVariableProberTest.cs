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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Unix.EnvironmentVariable;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test;


namespace Modulo.Collect.Probe.Unix.Test.ProberTests.EnvironmentVariable
{
    [TestClass]
    public class EnvironmentVariableProberTest: ProberTestBase
    {
        private const string FAKE_ENVIRONMENT_VARIABLE_NAME = "PATH";
        private const string FAKE_ENVIRONMENT_VARIABLE_VALUE = "/home/cpaiva:/bin/usr/bin";
        
        private CollectInfo FakeCollectInfo;
        private ItemType[] FakeItemsToCollect;
        private CollectedItem[] FakeCollectedItems;

        public EnvironmentVariableProberTest()
        {
            FakeItemsToCollect = new ItemType[] { GetFakeEnvironmentVariableItem() };
            FakeCollectedItems =  new CollectedItem[] { GetFakeCollectedItem() };
            FakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:2360");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_simple_environmentvariable_object()
        {
            var prober = new EnvironmentVariableProber();
            ProberBehaviorCreator.
                CreateBehaviorForNormalFlowExecution(
                    prober,
                    FakeItemsToCollect,
                    FakeCollectedItems);

            var probeResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(probeResult, typeof(environmentvariable_item));
            AssertEnvironmentItemEntities(probeResult.CollectedObjects.Single().SystemData.Single());
        }

        [TestMethod, Owner("cpaiva")]
        public void When_an_error_occurs_while_creating_items_to_collect_the_created_items_must_have_error_status()
        {
            var prober = new EnvironmentVariableProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(prober);

            var probeResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(probeResult, typeof(environmentvariable_item));
        }


        private ItemType GetFakeEnvironmentVariableItem()
        {
            return new environmentvariable_item()
            {
                name = OvalHelper.CreateItemEntityWithStringValue(FAKE_ENVIRONMENT_VARIABLE_NAME)
            };
        }

        private CollectedItem GetFakeCollectedItem()
        {
            var newCollectedItem = new environmentvariable_item()
            {
                name = OvalHelper.CreateItemEntityWithStringValue(FAKE_ENVIRONMENT_VARIABLE_NAME),
                value = OvalHelper.CreateEntityItemAnyTypeWithValue(FAKE_ENVIRONMENT_VARIABLE_VALUE)
            };

            return ProbeHelper.CreateFakeCollectedItem(newCollectedItem);
        }

        private void AssertEnvironmentItemEntities(ItemType itemToAssert)
        {
            var environmentVariableItem = (environmentvariable_item)itemToAssert;
            Assert.AreEqual(environmentVariableItem.name.Value, FAKE_ENVIRONMENT_VARIABLE_NAME);
            Assert.AreEqual(environmentVariableItem.value.Value, FAKE_ENVIRONMENT_VARIABLE_VALUE);
        }
    }
}
