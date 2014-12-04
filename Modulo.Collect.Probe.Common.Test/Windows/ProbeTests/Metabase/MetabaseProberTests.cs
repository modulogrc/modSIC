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
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Windows.Probes.Metabase;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Windows.Tests.Probes.Metabase
{
    [TestClass]
    public class MetabaseProberTests: ProberTestBase
    {
        [TestMethod]
        public void Should_be_possible_to_collect_an_metabase_object()
        {
            var metabaseProber = new MetabaseProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    metabaseProber,
                    new ItemType[] { new metabase_item() },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new metabase_item()) }
            );

            var probeResult = metabaseProber.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("6000"));

            DoAssertForSingleCollectedObject(probeResult, typeof(metabase_item));
        }

        [TestMethod]
        public void Should_be_possible_to_handle_an_error_during_itemtypes_generation()
        {
            var metabaseProber = new MetabaseProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(metabaseProber);

            var probeResult = metabaseProber.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("6001"));

            DoAssertForExecutionWithErrors(probeResult, typeof(metabase_item));
        }

        [TestMethod]
        public void Should_be_possible_to_collect_a_metabase_object_with_set()
        {
            var metabaseProber = new MetabaseProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    metabaseProber,
                    new ItemType[] { new metabase_item() },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new metabase_item()) }
            );

            var probeResult = metabaseProber.Execute(
                FakeContext, 
                FakeTargetInfo, 
                GetFakeCollectInfo("6002", "definitionsSimple.xml", "system_characteristics_with_sets.xml"));

            Assert.IsNotNull(probeResult);
            Assert.IsFalse(probeResult.HasErrors());
            Assert.IsNotNull(probeResult.CollectedObjects);
            Assert.AreEqual(1, probeResult.CollectedObjects.Count());
            var collectedObject = probeResult.CollectedObjects.First();
            Assert.AreEqual("oval:modulo:obj:6002", collectedObject.ObjectType.id);
            Assert.AreEqual(2, collectedObject.SystemData.Count);
            Assert.AreEqual("2000", collectedObject.SystemData.ElementAt(0).id);
            Assert.AreEqual("2001", collectedObject.SystemData.ElementAt(1).id);
        }
    }
}
