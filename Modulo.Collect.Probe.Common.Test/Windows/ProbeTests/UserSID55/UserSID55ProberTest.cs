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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.UserSID55;
using Modulo.Collect.Probe.Windows.Probes.UserProbes.UserSID;

namespace Modulo.Collect.Probe.Windows.Test
{

    [TestClass]
    public class UserSID55ProberTest: ProberTestBase
    {
        private CollectInfo FakeCollectInfo;
        private ItemType[] FakeItemsToBeReturnedByItemTypeGenerator;
        private CollectedItem[] FakeCollectedItems;


        public UserSID55ProberTest()
        {
            FakeCollectInfo = this.GetFakeCollectInfo("oval:modulo:obj:1080");
            FakeItemsToBeReturnedByItemTypeGenerator = new ItemType[] { CreateFakeUserSIDItem() };
            FakeCollectedItems = CreateFakeCollectedItems();
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_UserSID55_objects()
        {
            var userSIDProber = new UserSID55Prober();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    userSIDProber,
                    FakeItemsToBeReturnedByItemTypeGenerator,
                    FakeCollectedItems);

            var probeExecutionResult = userSIDProber.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(probeExecutionResult, typeof(user_sid_item));
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_UserSID_objects()
        {
            var userSIDProber = new UserSIDProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    userSIDProber,
                    FakeItemsToBeReturnedByItemTypeGenerator,
                    FakeCollectedItems);

            var probeExecutionResult = userSIDProber.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:1102"));

            DoAssertForSingleCollectedObject(probeExecutionResult, typeof(user_sid_item));

        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned_for_UserSID55()
        {
            var userSIDProber = new UserSID55Prober();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(userSIDProber);

            var probeExecutionResult = userSIDProber.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(probeExecutionResult, typeof(user_sid_item));
        }


        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned_for_UserSID()
        {
            var userSIDProber = new UserSIDProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(userSIDProber);

            var probeExecutionResult = userSIDProber.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:1102"));

            DoAssertForExecutionWithErrors(probeExecutionResult, typeof(user_sid_item));
        }

        private user_sid_item CreateFakeUserSIDItem()
        {
            return new user_sid_item() { user_sid = OvalHelper.CreateItemEntityWithStringValue("S-1-15-18") };
        }

        private CollectedItem[] CreateFakeCollectedItems()
        {
            var fakeUserSIDItem = CreateFakeUserSIDItem();
            return new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(fakeUserSIDItem) };
        }


    }
}
