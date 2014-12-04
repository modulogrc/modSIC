/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.User;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common.BasicClasses;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Windows.Test.Factories.Objects;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.User
{
    [TestClass]
    public class UserProberTests : ProberTestBase
    {
        private CollectInfo FakeCollectInfo;

        public UserProberTests()
        {
            var fakeObjects = new Definitions.ObjectType[] { new UserObjectFactory().NewObject("guest") };
            this.FakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(fakeObjects, null, null);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_call_UserProber_execution_without_erros()
        {
            var prober = new UserProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    new ItemType[] { new user_item() },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new user_item()) });

            var probeResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(probeResult, typeof(user_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_execution_erros()
        {
            var prober = new UserProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(prober);

            var probeResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(probeResult, typeof(user_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_user_object_with_set()
        {
            var fakeUserObjectGuest = new UserObjectFactory().NewObject("Guest");
            var fakeUserObjectAdmin = new UserObjectFactory().NewObject("admin");
            var fakeUserObjectWithSet = new UserObjectFactory().NewObjectWithSet(fakeUserObjectGuest, fakeUserObjectAdmin);
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new Definitions.ObjectType[] { fakeUserObjectWithSet });
            var prober = new UserProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    new ItemType[] { new user_item() },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new user_item()) });

            var probeResult = prober.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            Assert.IsNotNull(probeResult, "The result of probe execution cannot be null.");
            Assert.AreEqual(0, probeResult.CollectedObjects.Count(), "No items are expected");
        }



    }
}
