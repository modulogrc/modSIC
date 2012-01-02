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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.FileContent;

namespace Modulo.Collect.Probe.Windows.Test.FileContent
{
    /// <summary>
    /// Summary description for FileContentProbeTest
    /// </summary>
    [TestClass]
    public class FileContentProbeTest : ProberTestBase
    {
        private ItemType[] FakeItemsToReturn;
        private CollectedItem[] FakeCollectedItem;
        private CollectInfo FakeCollectInfo;


        public FileContentProbeTest()
        {
            FakeItemsToReturn = new ItemType[] { CreateFakeTextFileContent() };
            FakeCollectedItem = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(CreateFakeTextFileContent()) };
            FakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:9");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_execute_a_simple_textFileContent_collect_defined_with_one_entity()
        {
            var prober = new FileContentProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    FakeItemsToReturn,
                    FakeCollectedItem);

            var proberResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(proberResult, typeof(textfilecontent_item));

            var textFileContentItem = (textfilecontent_item)proberResult.CollectedObjects.Single().SystemData.Single();
            this.AssertTextFileContentItem(textFileContentItem, "teste", StatusEnumeration.exists);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_some_error_occurred_during_item_type_generation_a_item_with_status_equals_to_error_must_be_returned()
        {
            var prober = new FileContentProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(prober);

            var proberResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(proberResult, typeof(textfilecontent_item));
        }


        private textfilecontent_item CreateFakeTextFileContent()
        {
            return new textfilecontent_item()
            {
                path = OvalHelper.CreateItemEntityWithStringValue("c:\\temp"),
                filename = OvalHelper.CreateItemEntityWithStringValue("teste.txt"),
                line = OvalHelper.CreateItemEntityWithStringValue("teste")
            };
        }

        private void AssertTextFileContentItem(textfilecontent_item textFileContentItem, string expectedValueForLineEntity, StatusEnumeration expectedCollectedObjectStatus)
        {
            var assertStatusFailedMessage = string.Format("The status must be '{0}'", expectedCollectedObjectStatus.ToString());
            Assert.AreEqual(expectedCollectedObjectStatus, textFileContentItem.status, assertStatusFailedMessage);

            if (!string.IsNullOrEmpty(expectedValueForLineEntity))
            {
                Assert.IsNotNull(textFileContentItem.line, "The line entity cannot be null.");
                Assert.AreEqual(expectedValueForLineEntity, textFileContentItem.line.Value, "Unexpected value for line entity was found.");
            }
        }
    }
}
