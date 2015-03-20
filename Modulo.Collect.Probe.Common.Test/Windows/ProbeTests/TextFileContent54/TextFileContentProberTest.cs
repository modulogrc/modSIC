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
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Modulo.Collect.Probe.Windows.TextFileContent54;
using Rhino.Mocks;
using Definitions = Modulo.Collect.OVAL.Definitions;
using SysCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.Independent;


namespace Modulo.Collect.Probe.Windows.Test.TextFileContent54
{
    [TestClass]
    public class TextFileContentProberTest: ProberTestBase
    {
        private const string FAKE_PATH = @"C:\Windows\System32\drivers\etc";
        private const string FAKE_FILENAME = "hosts";
        private const string FAKE_PATTERN = "64.4.20.252 v4.windowsupdate.microsoft.com";
        private const string FAKE_INSTANCE = "1";

        private ItemType[] FakeItemsToReturnByItemTypeGenerator;
        private CollectedItem[] FakeColletedItems;
        private textfilecontent54_object TextFileContent54ObjectsWithoutBehaviors;
        private textfilecontent54_object TextFileContent54ObjectWithMultilineBehavior;
        private textfilecontent54_object TextFileContent54ObjectWithoutMultilineBehavior;

        public TextFileContentProberTest()
        {
            FakeItemsToReturnByItemTypeGenerator =
                new ItemType[] 
                { 
                    CreateFakeTextFileContentItem(null, FAKE_PATH, FAKE_FILENAME, FAKE_PATTERN, FAKE_INSTANCE)
                };

            FakeColletedItems = 
                new CollectedItem[] 
                { 
                    ProbeHelper.CreateFakeCollectedItem(FakeItemsToReturnByItemTypeGenerator.First())
                };

            var textFileContent54ObjectsSample = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple").objects.OfType<textfilecontent54_object>();
            this.TextFileContent54ObjectsWithoutBehaviors = textFileContent54ObjectsSample.First();
            this.TextFileContent54ObjectWithMultilineBehavior = textFileContent54ObjectsSample.Single(obj => obj.id.Equals("oval:modulo:obj:910"));
            this.TextFileContent54ObjectWithoutMultilineBehavior = textFileContent54ObjectsSample.Single(obj => obj.id.Equals("oval:modulo:obj:920"));
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_simple_TextFileContent54Object()
        {
            var textFileContentProber = new TextFileContentProberWindows();
            var fakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:900");
            ProberBehaviorCreator.
                CreateBehaviorForNormalFlowExecution(
                    textFileContentProber,
                    FakeItemsToReturnByItemTypeGenerator,
                    FakeColletedItems);

            var probeExecutionResult = textFileContentProber.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            var collectedObject = probeExecutionResult.CollectedObjects.ElementAt(0);
            this.AssertCollectedItemsReferences(collectedObject, collectedObject.SystemData);
            this.AssertCollectedItemStatus(collectedObject.ObjectType.reference.ElementAt(0), collectedObject.SystemData.ElementAt(0));
        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned()
        {
            var textFileContentProber = new TextFileContentProberWindows();
            var fakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:900");
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(textFileContentProber);

            var proberResult = textFileContentProber.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            DoAssertForExecutionWithErrors(proberResult, typeof(textfilecontent_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_complete_filepath_from_textfilecontent_item_defined_with_FilepathEntity()
        {
            string EXPECTED_COMPLETE_FILEPATH = @"c:\temp\file1.txt";

            var fakeTextFileContentItem = this.CreateFakeTextFileContentItem(EXPECTED_COMPLETE_FILEPATH, "c:\\tmp", "file666.txt", null, null);
            string foundCompleteFilePath = fakeTextFileContentItem.GetCompleteFilepath();
            Assert.AreEqual(EXPECTED_COMPLETE_FILEPATH, foundCompleteFilePath, "The complete file path is wrong.");

            fakeTextFileContentItem = this.CreateFakeTextFileContentItem(EXPECTED_COMPLETE_FILEPATH, null, null, null, null);
            foundCompleteFilePath = fakeTextFileContentItem.GetCompleteFilepath();
            Assert.AreEqual(EXPECTED_COMPLETE_FILEPATH, foundCompleteFilePath, "The complete file path is wrong.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_complete_filepath_from_textfilecontent_item_defined_with_path_and_filename_entity()
        {
            string EXPECTED_COMPLETE_FILEPATH = @"c:\tmp\file2.txt";
             
            var fakeTextFileContentItem = this.CreateFakeTextFileContentItem("", "c:\\tmp", "file2.txt", null, null);
            string foundCompleteFilePath = fakeTextFileContentItem.GetCompleteFilepath();
            Assert.AreEqual(EXPECTED_COMPLETE_FILEPATH, foundCompleteFilePath, "The complete file path is wrong.");

            fakeTextFileContentItem = this.CreateFakeTextFileContentItem(null, "c:\\tmp", "file2.txt", null, null);
            foundCompleteFilePath = fakeTextFileContentItem.GetCompleteFilepath();
            Assert.AreEqual(EXPECTED_COMPLETE_FILEPATH, foundCompleteFilePath, "The complete file path is wrong.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_textfilecontent_multiline_behavior()
        {
            // If an text file content object has no behaviors defined explicity, the default value for multiline behavior is true.
            Assert.IsNull(TextFileContent54ObjectsWithoutBehaviors.Items.OfType<Textfilecontent54Behaviors>().FirstOrDefault(), "No behaviors must be defined for next assert.");
            Assert.IsTrue(TextFileContent54ObjectsWithoutBehaviors.IsMultiline(), "Unexpected behavior ('multiline') value was found.");

            Assert.IsNotNull(TextFileContent54ObjectWithMultilineBehavior.Items.OfType<Textfilecontent54Behaviors>().FirstOrDefault(), "For next assert, a behavior must defined explicity.");
            Assert.IsTrue(TextFileContent54ObjectWithMultilineBehavior.IsMultiline(), "Unexpected behavior ('multiline') value was found.");

            Assert.IsNotNull(TextFileContent54ObjectWithoutMultilineBehavior.Items.OfType<Textfilecontent54Behaviors>().FirstOrDefault(), "For next assert, a behavior must defined explicity.");
            Assert.IsFalse(TextFileContent54ObjectWithoutMultilineBehavior.IsMultiline(), "Unexpected behavior ('multiline') value was found.");
        }

        [TestMethod, Ignore, Owner("lfernandes")]
        public void The_operation_evaluator_of_TextFileContentItemTypeGenerator_must_be_configured()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "900");
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new Definitions.ObjectType[] { objectType }, null, null);
            var fakeCollectedItems = new CollectedItem[] {  ProbeHelper.CreateFakeCollectedItem(new textfilecontent_item()) };
            var textFileContentProber = CreateMockedProberWithNoFakeItemTypeGenerator(fakeCollectedItems);

            textFileContentProber.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            Assert.IsInstanceOfType(textFileContentProber.ItemTypeGenerator, typeof(TextFileContentItemTypeGenerator));
            var itemTypeGenerator = (TextFileContentItemTypeGenerator)textFileContentProber.ItemTypeGenerator;
            Assert.IsNotNull(itemTypeGenerator.OperationEvaluator);
        }


        private textfilecontent_item CreateFakeTextFileContentItem(
            string filepath, string path, string filename, string pattern, string instance)
        {
            return new textfilecontent_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(filepath),
                path = (path == null) ? null : OvalHelper.CreateItemEntityWithStringValue(path),
                filename = (filename == null) ? null : OvalHelper.CreateItemEntityWithStringValue(filename),
                pattern = (pattern == null) ? null : OvalHelper.CreateItemEntityWithStringValue(pattern),
                instance = (instance == null) ? null : OvalHelper.CreateItemEntityWithIntegerValue(instance)
            };
        }

        private TextFileContentProberWindows CreateMockedProberWithNoFakeItemTypeGenerator(
            IEnumerable<CollectedItem> fakeCollectedItems)
        {
            MockRepository mocks = new MockRepository();
            var fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();
            var fakeSystemDataSource = mocks.DynamicMock<BaseObjectCollector>();

            Expect.Call(
                fakeSystemDataSource.CollectDataForSystemItem(null))
                    .IgnoreArguments()
                        .Return(fakeCollectedItems);
            
            mocks.ReplayAll();

            return new TextFileContentProberWindows()
            {
                ConnectionManager = fakeConnectionManager,
                ObjectCollector = fakeSystemDataSource
            };
        }

        private void DoBasicProbeResultAssert(ProbeResult resultToAssert, int expectedCollectedObjectsCount)
        {
            Assert.IsNotNull(resultToAssert, "The result of probe execution cannot be null.");
            Assert.IsNotNull(resultToAssert.ExecutionLog, "The ExecutionLog of TextFileContentProber was not created.");
            Assert.IsNotNull(resultToAssert.CollectedObjects, "There are no collected objects.");
            Assert.AreEqual(expectedCollectedObjectsCount, resultToAssert.CollectedObjects.Count(), "Unexpected collected objects count.");
        }

        private void AssertCollectedItemsReferences(CollectedObject collectedObject, IList<ItemType> collectedItems)
        {
            int collectedObjectCount = collectedObject.ObjectType.reference.Count();

            Assert.AreEqual(collectedItems.Count(), collectedObjectCount, "Unexpected number of item references was found.");
            Assert.AreEqual(collectedObjectCount, collectedItems.Count, "Unexpected number of generated items type was found.");
        }

        private void AssertCollectedItemStatus(SysCharacteristics.ReferenceType objectReference, ItemType collectedItem)
        {
            Assert.IsInstanceOfType(collectedItem, typeof(textfilecontent_item), "The generated ItemType must be a instance of textfilecontent_item class.");
            Assert.AreEqual(objectReference.item_ref, collectedItem.id, "The generated ItemType ID must be equal to collected object ID.");
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status, "A generated ItemType with unexpected OVAL Status was found.");
        }
    }
}
