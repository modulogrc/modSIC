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
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Solaris.Probes.smf;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics.Solaris;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Rhino.Mocks;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Solaris.Tests.ProberTests.SMF
{
    [TestClass]
    public class SMFItemTypeGeneratorTests
    {
        [TestMethod, Owner("dgomes, lfernandes")]
        public void Should_be_possible_to_generate_smf_items_to_collect()
        {
            // Arrange 
            var smfObjectSample = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2500");
            var itemsGenerator = new SMFItemTypeGenerator();

            // Act
            var generatedItems = itemsGenerator.GetItemsToCollect(smfObjectSample, null).ToArray();

            // Assert
            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(smf_item));
            AssertSmfItem((smf_item)generatedItems.Single(), "disable svc:/network/rpc/bind:default");
        }

        [TestMethod, Owner("dgomes, lfernandes")]
        public void Should_be_possible_to_generate_smf_items_from_an_object_with_referenced_variables()
        {
            // Arrange 
            var smfObjectSample = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2503");
            var fakeVariables = 
                VariableHelper.CreateVariableWithOneValue(
                    "oval:modulo:obj:2503", "oval:modulo:var:2503", "enable svc:/network/rpc/bind:default");
            
            var itemsGenerator = new SMFItemTypeGenerator();
            
            // Act
            var generatedItems = itemsGenerator.GetItemsToCollect(smfObjectSample, fakeVariables).ToArray();

            // Assert
            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(smf_item));
            AssertSmfItem((smf_item)generatedItems.Single(), "enable svc:/network/rpc/bind:default");
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_generate_smf_items_from_an_object_with_referenced_variables_that_multi_values()
        {
            var smfObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2700");
            string[] arrayVariables = new string[] {"disable svc:/network/rpc/bind:default", "enable svc:/network/rpc/bind:default"};
            var fakeVariables = VariableHelper.CreateVariableWithMultiplesValue
                ("oval:modulo:obj:2700", "oval:modulo:var:2700", arrayVariables);

            var itemsGenerator = new SMFItemTypeGenerator().GetItemsToCollect(smfObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsGenerator, 2, typeof(smf_item));
            AssertSmfItem((smf_item)itemsGenerator[0], "disable svc:/network/rpc/bind:default");
            AssertSmfItem((smf_item)itemsGenerator[1], "enable svc:/network/rpc/bind:default");
        }

        [TestMethod, Owner("dgomes, lfernandes")]
        public void Should_not_be_possible_generate_smf_items_from_an_object_with_not_equal_operation_on_service_name_entity()
        {
            var smfObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2800");            
            var fakeVariables = CreateFakeEvaluatedVariablesWithMultiValues("oval:modulo:obj:2800");
            
            var generatedItems = new SMFItemTypeGenerator().GetItemsToCollect(smfObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(smf_item), StatusEnumeration.error);
            var generatedSmfItem = (smf_item)generatedItems.Single();
            Assert.AreEqual(StatusEnumeration.error, generatedSmfItem.status, "The expected status is 'error'.");
            var fmriEntity = generatedSmfItem.fmri;
            Assert.IsNotNull(fmriEntity);
            Assert.AreEqual(StatusEnumeration.error, fmriEntity.status);
            Assert.IsNotNull(generatedSmfItem.message);
            Assert.AreEqual(MessageLevelEnumeration.error, generatedSmfItem.message.First().level);
            Assert.AreEqual("The 'notequal' operation is not supported for this entity.", generatedSmfItem.message.First().Value);
        }

        [TestMethod, Owner("dgomes")]
        public void Should_not_be_possible_generate_smf_items_from_an_object_with_pattern_match_operation_on_service_name_entity()
        {
            var smfObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2900");
            var fakeVariables = CreateFakeEvaluatedVariablesWithMultiValues("oval:modulo:obj:2900");

            var generatedItems = new SMFItemTypeGenerator().GetItemsToCollect(smfObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(smf_item), StatusEnumeration.error);
            var generatedSmfItem = (smf_item)generatedItems.Single();
            Assert.AreEqual(StatusEnumeration.error, generatedSmfItem.status, "The expected status is 'error'.");
            var fmriEntity = generatedSmfItem.fmri;
            Assert.IsNotNull(fmriEntity);
            Assert.AreEqual(StatusEnumeration.error, fmriEntity.status);
            Assert.IsNotNull(generatedSmfItem.message);
            Assert.AreEqual(MessageLevelEnumeration.error, generatedSmfItem.message.First().level);
            Assert.AreEqual("The 'patternmatch' operation is not supported for this entity.", generatedSmfItem.message.First().Value);
        }

        private VariablesEvaluated CreateFakeEvaluatedVariablesWithMultiValues(string objectID)
        {
            var fakeVariablesValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariablesValues.Add("oval:modulo:var:2800", new string[] { "enable svc:/network/rpc/bind:default" });

            return 
                VariableHelper
                    .CreateEvaluatedVariables(objectID, fakeVariablesValues);
        }

        private void AssertSmfItem(smf_item smfItem, String expectedFmriValue)
        {
            ItemTypeEntityChecker.AssertItemTypeEntity(smfItem.fmri, expectedFmriValue);            
        }        

    }
}
