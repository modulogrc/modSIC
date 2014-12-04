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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Solaris;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Solaris.Probes.smf;

namespace Modulo.Collect.Probe.Solaris.Tests
{
    [TestClass]
    public class SMFProberTest : ProberTestBase
    {
        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_collect_a_smf_object()
        {
            var prober = new SMFProber();
            new ProberBehaviorCreator()
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    new ItemType[] {new smf_item() },
                    new CollectedItem[] {ProbeHelper.CreateFakeCollectedItem(new smf_item()) });

            var result = prober.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:2500"));

            DoAssertForSingleCollectedObject(result, typeof(smf_item));
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_get_set_element_for_smf_objects()
        {
            var prober = new SMFProber();
            new ProberBehaviorCreator().
                CreateBehaviorForNormalFlowExecution(
                    prober,
                    new ItemType[] { new smf_item() },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new smf_item()) });

            var result = prober.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:2600"));

            Assert.IsNotNull(result);
        }
    }
}
