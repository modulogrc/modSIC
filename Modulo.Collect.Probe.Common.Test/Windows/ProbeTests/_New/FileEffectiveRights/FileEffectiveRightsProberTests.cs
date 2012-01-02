/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.Windows.Probes._New.FileEffectiveRights53;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests._New.FileEffectiveRights
{
    [TestClass]
    public class FileEffectiveRightsProberTests: ProberTestBase
    {
        private ItemType[] FakeItemsToBeGenerateByItemTypeGenerator;
        private CollectedItem[] FakeCollectedItemsToBeReturnedByObjectCollector;
        private CollectInfo FakeCollectInfo;
        private OVAL.Definitions.oval_definitions OvalDefinitionsSample;
        

        public FileEffectiveRightsProberTests()
        {
            var fakeItemType = new fileeffectiverights_item();
            this.FakeItemsToBeGenerateByItemTypeGenerator = new ItemType[] { fakeItemType };
            this.FakeCollectedItemsToBeReturnedByObjectCollector = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(fakeItemType) };
            this.OvalDefinitionsSample = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple");
            var objectSample = OvalDefinitionsSample.objects.OfType<fileeffectiverights53_object>().Where(obj => obj.Items.Count() > 1).First();
            this.FakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new OVAL.Definitions.ObjectType[] { objectSample }, null, null);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_call_FileEffectiveRights53Prober_execution_without_errors()
        {
            var prober = new FileEffectiveRights53Prober();
            new ProberBehaviorCreator()
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    FakeItemsToBeGenerateByItemTypeGenerator,
                    FakeCollectedItemsToBeReturnedByObjectCollector);

            var proberResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(proberResult, typeof(fileeffectiverights_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_error_during_prober_execution()
        {
            var prober = new FileEffectiveRights53Prober();
            new ProberBehaviorCreator().CreateBehaviorWithExceptionThrowing(prober);

            var proberResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(proberResult, typeof(fileeffectiverights_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_use_SET_to_collect_fileeffectiverights53_object()
        {
            var objectWithSet = OvalDefinitionsSample.objects.Single(obj => obj.id.Equals("oval:modulo:obj:1001"));
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new OVAL.Definitions.ObjectType[] { objectWithSet }, null, null);
            var prober = new FileEffectiveRights53Prober();
            new ProberBehaviorCreator()
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    FakeItemsToBeGenerateByItemTypeGenerator,
                    FakeCollectedItemsToBeReturnedByObjectCollector);


            var proberResult = prober.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            Assert.AreEqual(0, proberResult.CollectedObjects.Count());
        }


    }
}
