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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.AuditEventPolicySubcategories;

namespace Modulo.Collect.Probe.Windows.Test.AuditEventPolicySubcategories
{
    [TestClass]
    public class AuditEventPolicySubcategoriesProberTest : ProberTestBase
    {
        private CollectInfo FakeCollectInfo;
        private CollectedItem[] FakeCollectedItems;
        private ItemType[] FakeItemsToCollect;
        
        public AuditEventPolicySubcategoriesProberTest()
        {
            var fakeItem = new auditeventpolicysubcategories_item();

            FakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:2000");
            FakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(fakeItem) };
            FakeItemsToCollect = new ItemType[] { fakeItem };
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_AuditEventPolicySubcategories()
        {
            var prober = new AuditEventPolicySubcategoriesProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    FakeItemsToCollect,
                    FakeCollectedItems);

            var probeResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(probeResult, typeof(auditeventpolicysubcategories_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void When_an_error_occurs_while_creating_items_to_collect_the_created_items_must_have_error_status()
        {
            var prober = new AuditEventPolicySubcategoriesProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(prober);

            var proberExecutionResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(proberExecutionResult, typeof(auditeventpolicysubcategories_item));
        }
    }
}
