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
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Unix.TextFileContent54;
using Rhino.Mocks;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.Probe.Independent.TextFileContent54;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.TextFileContent54
{
    [TestClass]
    public class TextFileContentProberUnixTests: ProberTestBase
    {
        private ItemType[] FakeItemsToCollect;
        private CollectedItem[] FakeCollectedItems;
        private CollectInfo FakeCollectInfo;

        public TextFileContentProberUnixTests()
        {
            FakeItemsToCollect = new ItemType[] { new textfilecontent_item() };
            FakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new textfilecontent_item()) };
            FakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:900");
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_textfilecontent_object_for_unix_systems()
        {
            var textFileContentProberUnix = new TextFileContentProberUnix();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    textFileContentProberUnix, FakeItemsToCollect, FakeCollectedItems);

            var probeExecutionResult = 
                textFileContentProberUnix.Execute(
                    FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(probeExecutionResult, typeof(textfilecontent_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void When_an_error_occurs_while_creating_items_to_collect_the_created_items_must_have_error_status()
        {
            var textFileContentProberUnix = new TextFileContentProberUnix();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(textFileContentProberUnix);

            var probeExecutionResult =
                textFileContentProberUnix.Execute(
                    FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(probeExecutionResult, typeof(textfilecontent_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_textfilecontent_items_to_collect_on_unix_systems()
        {
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "oval:modulo:obj:20");
            var fakeItemTypeGenerator = CreateMockedItemTypeGenerator();

            var itemsToCollect = fakeItemTypeGenerator.GetItemsToCollect(fakeObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(textfilecontent_item));
            AssertTextFileContentItem((textfilecontent_item)itemsToCollect.Single());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_textfilecontent_items_to_collect_on_unix_systems_from_an_object_definied_with_path_and_filename_entities()
        {
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "oval:modulo:obj:21");
            var fakeItemTypeGenerator = CreateMockedItemTypeGenerator();

            var itemsToCollect = fakeItemTypeGenerator.GetItemsToCollect(fakeObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(textfilecontent_item));
            AssertTextFileContentItem((textfilecontent_item)itemsToCollect.Single());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_textfilecontent_items_to_collect_on_unix_systems_from_an_object_with_variables()
        {
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "oval:modulo:obj:22");
            var fakeVariables = CreateFakeVariables(fakeObject.id);
            var fakeItemTypeGenerator = CreateMockedItemTypeGenerator();

            var itemsToCollect = fakeItemTypeGenerator.GetItemsToCollect(fakeObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(textfilecontent_item));
            AssertTextFileContentItem((textfilecontent_item)itemsToCollect.Single());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_textfilecontent_items_to_collect_on_unix_systems_from_an_object_definied_with_path_and_filename_entities_and_referenced_variables()
        {
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "oval:modulo:obj:23");
            var fakeVariables = CreateFakeVariables(fakeObject.id);
            var fakeItemTypeGenerator = CreateMockedItemTypeGenerator();

            var itemsToCollect = fakeItemTypeGenerator.GetItemsToCollect(fakeObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(textfilecontent_item));
            AssertTextFileContentItem((textfilecontent_item)itemsToCollect.Single());
        }

        private TemporaryItemTypeGenerator CreateMockedItemTypeGenerator()
        {
            var fakeTextFileContent = new TextFileContent[] { new TextFileContent() { Text = "xxx if yyy" } }.ToList();
            var mocks = new MockRepository();
            
            var fakeObjectCollector = mocks.DynamicMock<TextFileContentObjectCollector>();
            //var fakeFileContent = mocks.DynamicMock<FileContentCollector>((SSHProvider)null);
            //Expect.Call(fakeFileContent.GetTextFileContent(null, null)).IgnoreArguments().Return(fakeTextFileContent);
            Expect.Call(fakeObjectCollector.GetValues(null)).IgnoreArguments().Return(new string[] { "xxx if yyy" });
            mocks.ReplayAll();

            return new TemporaryItemTypeGenerator() { ObjectCollector = fakeObjectCollector };
        }

        private VariablesEvaluated CreateFakeVariables(string objectID)
        {
            var fakeVariableValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariableValues.Add("oval:modulo:var:110", new string[] { "/usr/tmp/autorun.sh" });
            fakeVariableValues.Add("oval:modulo:var:111", new string[] { "/usr/tmp/" });
            fakeVariableValues.Add("oval:modulo:var:112", new string[] { "autorun.sh" });
            fakeVariableValues.Add("oval:modulo:var:113", new string[] { "if" });
            fakeVariableValues.Add("oval:modulo:var:114", new string[] { "1" });

            return VariableHelper.CreateEvaluatedVariables(objectID, fakeVariableValues);
        }


        private void AssertTextFileContentItem(textfilecontent_item itemToAssert, bool assertPathAndFilename = false)
        {
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.filepath, "/usr/tmp/autorun.sh");
            if (assertPathAndFilename)
            {
                ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.path, "/usr/tmp/");
                ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.filename, "autorun.sh");
            }
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.pattern, "if");
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.instance, "1");
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.text, "xxx if yyy");
        }
    }
}
