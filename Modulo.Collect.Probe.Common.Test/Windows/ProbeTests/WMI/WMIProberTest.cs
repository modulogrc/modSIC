/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.Windows.WQL;
using Rhino.Mocks;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Windows.Test.Helpers;
using Rhino.Mocks.Interfaces;
using Modulo.Collect.Probe.Common.Test;

namespace Modulo.Collect.Probe.Windows.Test.WMI
{
    [TestClass]
    public class WMIProberTest: ProberTestBase
    {
        private const string FAKE_WMI_NAMESPACE = "root\\cimv2";
        private const string FAKE_WQL = "select caption from Win32_OperatingSystem";
        private WMIProber WMIProber = new WMIProber();


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_wmi_object()
        {
            var fakeItemsToReturn = this.CreateWmiItems(FAKE_WMI_NAMESPACE, FAKE_WQL);
            var fakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(fakeItemsToReturn.First()) };
            var fakeCollectedInfo = GetFakeCollectInfo("oval:modulo:obj:2200");
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    WMIProber,
                    fakeItemsToReturn,
                    fakeCollectedItems);

            var proberExecutionResult = WMIProber.Execute(FakeContext, FakeTargetInfo, fakeCollectedInfo);

            DoAssertForSingleCollectedObject(proberExecutionResult, typeof(wmi_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned()
        {
            var fakeCollectedInfo = GetFakeCollectInfo("oval:modulo:obj:2200");
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(WMIProber);

            var proberExecutionResult = WMIProber.Execute(FakeContext, FakeTargetInfo, fakeCollectedInfo);

            DoAssertForExecutionWithErrors(proberExecutionResult, typeof(wmi_item));
        }

        private ItemType[] CreateWmiItems(string nameSpace, string wql)
        {
            var newWmiItem =
                new wmi_item()
                {
                    @namespace = OvalHelper.CreateItemEntityWithStringValue(nameSpace),
                    wql = OvalHelper.CreateItemEntityWithStringValue(wql)
                };

            return new ItemType[] { newWmiItem };
        }
    }
}
    
