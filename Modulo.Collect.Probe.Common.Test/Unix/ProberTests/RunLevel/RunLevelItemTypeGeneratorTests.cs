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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Unix.RunLevel;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Rhino.Mocks;
using Modulo.Collect.Probe.Unix.SSHCollectors;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.RunLevel
{
    [TestClass]
    public class RunLevelItemTypeGeneratorTests
    {
        private string[] FAKE_SERVICES = { "sshd", "cups", "openvpn", "openscap", "killall" };

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_runlevel_items_to_collect()
        {
            var runlevelObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "1");

            var generatedItems = new RunLevelItemTypeGenerator().GetItemsToCollect(runlevelObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(runlevel_item));
            var runLevelItemToAssert = (runlevel_item)generatedItems.Single();
            AssertRunlevelItem(runLevelItemToAssert, "ssh", "1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_runlevel_items_from_an_object_with_referenced_variables()
        {
            var runlevelObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "100");
            var fakeEvaluatedVariables = CreateFakeEvaluatedVariablesWithMultiValues("oval:modulo:obj:100");

            var generatedItems = 
                new RunLevelItemTypeGenerator()
                    .GetItemsToCollect(runlevelObject, fakeEvaluatedVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(runlevel_item));
            var runlevelItem = (runlevel_item)generatedItems.Single();
            AssertRunlevelItem(runlevelItem, "cups", "5");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_runlevel_items_from_an_object_with_referenced_variables_that_multi_values()
        {
            var runlevelObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "110");
            var fakeVariables = CreateFakeEvaluatedVariablesWithMultiValues();

            var generatedItems =
                new RunLevelItemTypeGenerator()
                    .GetItemsToCollect(runlevelObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 10, typeof(runlevel_item));
            AssertRunlevelItem((runlevel_item)generatedItems[0], "ssh", "1");
            AssertRunlevelItem((runlevel_item)generatedItems[1], "ssh", "2");
            AssertRunlevelItem((runlevel_item)generatedItems[2], "ssh", "3");
            AssertRunlevelItem((runlevel_item)generatedItems[3], "ssh", "4");
            AssertRunlevelItem((runlevel_item)generatedItems[4], "ssh", "5");
            AssertRunlevelItem((runlevel_item)generatedItems[5], "cups", "1");
            AssertRunlevelItem((runlevel_item)generatedItems[6], "cups", "2");
            AssertRunlevelItem((runlevel_item)generatedItems[7], "cups", "3");
            AssertRunlevelItem((runlevel_item)generatedItems[8], "cups", "4");
            AssertRunlevelItem((runlevel_item)generatedItems[9], "cups", "5");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_runlevel_items_from_an_object_with_pattern_match_operation_on_service_name_entity()
        {
            var runlevelObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "130");
            var fakeVariables = CreateFakeEvaluatedVariablesWithMultiValues("oval:modulo:obj:130");
            var mockedItemTypeGenerator = this.CreateMockedItemTypeGenerator();

            var generatedItems = mockedItemTypeGenerator.GetItemsToCollect(runlevelObject, fakeVariables).ToArray();

            Assert.IsNotNull(generatedItems);
            Assert.AreEqual(10, generatedItems.Count());
            AssertRunlevelItem((runlevel_item)generatedItems[0], "openvpn", "1");
            AssertRunlevelItem((runlevel_item)generatedItems[1], "openvpn", "2");
            AssertRunlevelItem((runlevel_item)generatedItems[2], "openvpn", "3");
            AssertRunlevelItem((runlevel_item)generatedItems[3], "openvpn", "4");
            AssertRunlevelItem((runlevel_item)generatedItems[4], "openvpn", "5");
            AssertRunlevelItem((runlevel_item)generatedItems[5], "openscap", "1");
            AssertRunlevelItem((runlevel_item)generatedItems[6], "openscap", "2");
            AssertRunlevelItem((runlevel_item)generatedItems[7], "openscap", "3");
            AssertRunlevelItem((runlevel_item)generatedItems[8], "openscap", "4");
            AssertRunlevelItem((runlevel_item)generatedItems[9], "openscap", "5");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_runlevel_items_from_an_object_with_pattern_match_operation_on_service_name_entity_and_reference_to_variables_in_all_entites()
        {
            var runlevelObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "140");
            var fakeVariables = CreateFakeEvaluatedVariablesWithMultiValues("oval:modulo:obj:140");
            var mockedItemTypeGenerator = this.CreateMockedItemTypeGenerator();

            var generatedItems = mockedItemTypeGenerator.GetItemsToCollect(runlevelObject, fakeVariables).ToArray();

            Assert.IsNotNull(generatedItems);
            Assert.AreEqual(15, generatedItems.Count());
            AssertRunlevelItem((runlevel_item)generatedItems[0], "openvpn", "1");
            AssertRunlevelItem((runlevel_item)generatedItems[1], "openvpn", "2");
            AssertRunlevelItem((runlevel_item)generatedItems[2], "openvpn", "3");
            AssertRunlevelItem((runlevel_item)generatedItems[3], "openvpn", "4");
            AssertRunlevelItem((runlevel_item)generatedItems[4], "openvpn", "5");
            AssertRunlevelItem((runlevel_item)generatedItems[5], "openscap", "1");
            AssertRunlevelItem((runlevel_item)generatedItems[6], "openscap", "2");
            AssertRunlevelItem((runlevel_item)generatedItems[7], "openscap", "3");
            AssertRunlevelItem((runlevel_item)generatedItems[8], "openscap", "4");
            AssertRunlevelItem((runlevel_item)generatedItems[9], "openscap", "5");
            AssertRunlevelItem((runlevel_item)generatedItems[10], "killall", "1");
            AssertRunlevelItem((runlevel_item)generatedItems[11], "killall", "2");
            AssertRunlevelItem((runlevel_item)generatedItems[12], "killall", "3");
            AssertRunlevelItem((runlevel_item)generatedItems[13], "killall", "4");
            AssertRunlevelItem((runlevel_item)generatedItems[14], "killall", "5");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_runlevel_item_from_an_object_with_not_equal_operation_on_service_name_entity()
        {
            var runlevelObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "150");
            var fakeVariables = CreateFakeEvaluatedVariablesWithMultiValues("oval:modulo:obj:150");
            var mockedItemTypeGenerator = this.CreateMockedItemTypeGenerator();

            var generatedItems = mockedItemTypeGenerator.GetItemsToCollect(runlevelObject, fakeVariables).ToArray();

            Assert.IsNotNull(generatedItems);
            Assert.AreEqual(4, generatedItems.Count());
            AssertRunlevelItem((runlevel_item)generatedItems[0], "sshd", "5");
            AssertRunlevelItem((runlevel_item)generatedItems[1], "openvpn", "5");
            AssertRunlevelItem((runlevel_item)generatedItems[2], "openscap", "5");
            AssertRunlevelItem((runlevel_item)generatedItems[3], "killall", "5");
        }


        private RunLevelItemTypeGenerator CreateMockedItemTypeGenerator()
        {
            var mocks = new MockRepository();
            var fakeRunLevelCollector = mocks.DynamicMock<RunLevelCollector>();
//            fakeRunLevelCollector.CommandLineRunner = 

            Expect.Call(fakeRunLevelCollector.GetTargetServices())
                .IgnoreArguments()
                    .Return(FAKE_SERVICES.ToList());
            
            mocks.ReplayAll();

            return new RunLevelItemTypeGenerator() { RunLevelCollector = fakeRunLevelCollector };
        }

        private VariablesEvaluated CreateFakeEvaluatedVariablesWithMultiValues(string objectID = "oval:modulo:obj:110")
        {
            var fakeVariablesValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariablesValues.Add("oval:modulo:var:101", new string[] { "cups" });
            fakeVariablesValues.Add("oval:modulo:var:102", new string[] { "5" });
            fakeVariablesValues.Add("oval:modulo:var:103", new string[] { "ssh", "cups" });
            fakeVariablesValues.Add("oval:modulo:var:104", new string[] { "1", "2", "3", "4", "5" });
            fakeVariablesValues.Add("oval:modulo:var:105", new string[] { "^open.*", ".*all$" });

            return
                VariableHelper
                    .CreateEvaluatedVariables(objectID, fakeVariablesValues);
        }

        private void AssertRunlevelItem(
            runlevel_item runlevelItemToAssert,
            string expectedServiceName,
            string expectedRunlevelValue)
        {
            ItemTypeEntityChecker.AssertItemTypeEntity(runlevelItemToAssert.service_name, expectedServiceName);
            ItemTypeEntityChecker.AssertItemTypeEntity(runlevelItemToAssert.runlevel, expectedRunlevelValue);
            Assert.IsNull(runlevelItemToAssert.start);
            Assert.IsNull(runlevelItemToAssert.kill);
        }
    }
}
