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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Linux.RPMInfo;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics.Linux;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Rhino.Mocks;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.Linux.RpmInfo
{
    [TestClass]
    public class RpmInfoItemTypeGeneratorTests
    {
        private const string DEFINITIONS_LINUX = "definitions_all_linux";
        private const string RPMINFO_OBJECT_ID_1 = "oval:modulo:obj:1";
        private const string RPMINFO_OBJECT_ID_10 = "oval:modulo:obj:10";
        private const string RPMINFO_OBJECT_ID_11 = "oval:modulo:obj:11";
        private const string RPMINFO_OBJECT_ID_12 = "oval:modulo:obj:12";
        private const string RPMINFO_OBJECT_ID_13 = "oval:modulo:obj:13";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_rpminfo_items_to_collect()
        {
            var rpmInfoObject = ProbeHelper.GetDefinitionObjectTypeByID(DEFINITIONS_LINUX, RPMINFO_OBJECT_ID_1);

            var generatedItems = new RPMInfoItemTypeGenerator().GetItemsToCollect(rpmInfoObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(rpminfo_item));
            AssertEntityItems(generatedItems, new string[] { "firefox" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_rpminfo_items_from_an_object_with_referenced_variable()
        {
            var rpmInfoObject = ProbeHelper.GetDefinitionObjectTypeByID(DEFINITIONS_LINUX, RPMINFO_OBJECT_ID_10);
            var fakeEvaluatedVariables = 
                VariableHelper.CreateVariableWithOneValue(
                    rpmInfoObject.id, "oval:modulo:var:2", "chrome");

            var generatedItems = 
                new RPMInfoItemTypeGenerator()
                    .GetItemsToCollect(rpmInfoObject, fakeEvaluatedVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(rpminfo_item));
            AssertEntityItems(generatedItems, new string[] { "chrome" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_pattern_match_operation_on_name_entity()
        {
            var rpmInfoObject = ProbeHelper.GetDefinitionObjectTypeByID(DEFINITIONS_LINUX, RPMINFO_OBJECT_ID_11);

            var generatedItems =
                CreateMockedItemTypeGenerator()
                    .GetItemsToCollect(rpmInfoObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 2, typeof(rpminfo_item));
            AssertEntityItems(generatedItems, new string[] { "rpm-libs", "libspectre" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_not_equal_operation_on_name_entity()
        {
            var rpmInfoObject = ProbeHelper.GetDefinitionObjectTypeByID(DEFINITIONS_LINUX, RPMINFO_OBJECT_ID_13);

            var generatedItems =
                CreateMockedItemTypeGenerator()
                    .GetItemsToCollect(rpmInfoObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 4, typeof(rpminfo_item));
            AssertEntityItems(generatedItems, new string[] { "ORBit2", "rpm-libs", "libspectre", "cdrdao" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_mix_pattern_match_operation_and_reference_to_variable_on_name_entity()
        {
            var rpmInfoObject = ProbeHelper.GetDefinitionObjectTypeByID(DEFINITIONS_LINUX, RPMINFO_OBJECT_ID_12);
            var fakeEvaluatedVariables =
                VariableHelper.CreateVariableWithOneValue(
                    rpmInfoObject.id, "oval:modulo:var:3", "libs*");

            var generatedItems =
                CreateMockedItemTypeGenerator()
                    .GetItemsToCollect(rpmInfoObject, fakeEvaluatedVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 2, typeof(rpminfo_item));
            AssertEntityItems(generatedItems, new string[] { "rpm-libs", "libspectre" });
        }


        private void AssertEntityItems(ItemType[] generatedItems, string[] expectedNameEntityValues)
        {
            for (int i = 0; i < expectedNameEntityValues.Count(); i++)
            {
                var nameEntity = ((rpminfo_item)generatedItems.ElementAt(i)).name;
                var expectedEntityValue = expectedNameEntityValues.ElementAt(i);
                ItemTypeEntityChecker.AssertItemTypeEntity(nameEntity, expectedEntityValue);
            }
        }

        private RPMInfoItemTypeGenerator CreateMockedItemTypeGenerator()
        {   
            var fakeRpmPackages = new string[] { "ORBit2", "rpm-libs", "plymouth-plugin-two-step", "libspectre", "cdrdao" };
            var mocks = new MockRepository();
            var fakeRpmInfoCollector = mocks.DynamicMock<RPMInfoCollector>();
            Expect.Call(fakeRpmInfoCollector.GetAllTargetRpmNames()).Return(fakeRpmPackages);
            mocks.ReplayAll();

            return new RPMInfoItemTypeGenerator() { RpmInfoCollector = fakeRpmInfoCollector };
        }
    }
}
