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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Windows.XmlFileContent;
using Rhino.Mocks;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.XmlFileContent
{
    [TestClass]
    public class XmlFileContentProberTests : GenericXmlFileContentProberTestBase<StraightNetworkConnectionProvider>
    {
        private XmlFileContentProber XmlFileContentProberToTest;
        private ItemType[] FakeItemsToCollect;
        private CollectedItem[] FakeCollectedItems;
        private CollectInfo FakeCollectInfo;

        public XmlFileContentProberTests()
        {
            var mockedFileProvider = MockRepository.GenerateMock<IFileProvider>();
            XmlFileContentProberToTest = new XmlFileContentProber(mockedFileProvider);

            FakeItemsToCollect = CreateFakeItemTypes();
            FakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new xmlfilecontent_item()) };
            FakeCollectInfo = GetFakeCollectInfo("2300");
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_a_simple_xmlfilecontent_object()
        {
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    XmlFileContentProberToTest, 
                    FakeItemsToCollect, 
                    FakeCollectedItems);

            var probeResult = 
                XmlFileContentProberToTest.Execute(
                    FakeContext, 
                    FakeTargetInfo, 
                    FakeCollectInfo);

            DoAssertForSingleCollectedObject(probeResult, typeof(xmlfilecontent_item));
        }

        [TestMethod, Owner("cpaiva")]
        public void When_an_error_occurs_while_creating_items_to_collect_the_created_items_must_have_error_status()
        {
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(XmlFileContentProberToTest);

            var probeExecutionResult = 
                XmlFileContentProberToTest.Execute(
                    FakeContext, 
                    FakeTargetInfo, 
                    FakeCollectInfo);

            DoAssertForExecutionWithErrors(probeExecutionResult, typeof(xmlfilecontent_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_set_element_for_xmlfilecontent_objects()
        {
            var prober = new XmlFileContentProber();
            CreateFileProviderBehavior(prober);
            
            var result = prober.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:700", "definitionsWithSet", "system_characteristics_with_sets"));

            Assert.IsNotNull(result);
        }




        private void CreateFileProviderBehavior(XmlFileContentProber prober)
        {
            var mocks = new MockRepository();
            prober.CustomFileProvider = mocks.DynamicMock<IFileProvider>();
            mocks.ReplayAll();

            new ProberBehaviorCreator()
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    new ItemType[] { new xmlfilecontent_item() },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new xmlfilecontent_item()) });
        }

        private ItemType[] CreateFakeItemTypes()
        {
            var newItemType = new xmlfilecontent_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue("c:\\temp\\file1.txt"),
                xpath = OvalHelper.CreateItemEntityWithStringValue("xxx"),
            };

            return new ItemType[] { newItemType };
        }
    }
}
