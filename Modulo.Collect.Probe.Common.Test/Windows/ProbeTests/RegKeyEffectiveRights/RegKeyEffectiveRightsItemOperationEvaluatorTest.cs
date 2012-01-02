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
using Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53;
using Modulo.Collect.OVAL.Definitions;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test.RegKeyEffectiveRights
{
    [TestClass]
    public class RegKeyEffectiveRightsItemOperationEvaluatorTest
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_PatternMatchOperation_on_key_entity_of_a_RegKeyEffectiveRightsObject()
        {
            List<List<String>> fakeSubKeys = new List<List<String>>();
            fakeSubKeys.Add(new string[] { "RiskManager", "RiskManagerNG", "wwwRiskyyyy" }.ToList());
            var sourceObjectType = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "780");
            var operationEvaluator = this.CreateMockedOperationEvaluator(fakeSubKeys, new List<List<String>>());
            string[] expectedItemsAfterOperation = new string[] 
                { @"SOFTWARE\Modulo\RiskManager\CurrentVersion", @"SOFTWARE\Modulo\RiskManagerNG\CurrentVersion" };

            IEnumerable<String> applyPatternMatchResult = operationEvaluator.ProcessOperationForKeyEntity(sourceObjectType, null);

            this.DoBasicAsserts(applyPatternMatchResult, 2);
            this.AssertOperationResult(applyPatternMatchResult, expectedItemsAfterOperation);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_there_are_no_subkeys_matching_KeyPattern_the_OperationProcessing_must_return_a_empty_list()
        {
            List<List<String>> fakeSubKeys = new List<List<String>>();
            fakeSubKeys.Add(new string[] { "Key1", "Key2", "ManagementRisk" }.ToList());
            var sourceObjectType = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "780");
            var operationEvaluator = this.CreateMockedOperationEvaluator(fakeSubKeys, new List<List<String>>());

            IEnumerable<String> applyPatternMatchResult = operationEvaluator.ProcessOperationForKeyEntity(sourceObjectType, null);

            Assert.IsNotNull(applyPatternMatchResult, "The result of operation processing");
            Assert.AreEqual(0, applyPatternMatchResult.Count(), "This test don´t expect any items.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_PatterMatchOperation_on_Key_entity_with_multi_level_regex()
        {
            // SOFTWARE\Modulo\^Risk.*\Info\^Current.*
            List<List<String>> fakeSubKeys = new List<List<String>>();
            fakeSubKeys.Add(new string[] { "RiskManager", "RiskManagerNG", "PCN", "WorkflowManager" }.ToList());
            fakeSubKeys.Add(new string[] { "CurrentVersion", "VersionInfo", "CurrentBuild" }.ToList());
            var sourceObjectType = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "790");
            var operationEvaluator = this.CreateMockedOperationEvaluator(fakeSubKeys, new List<List<String>>());
            string[] expectedItemsAfterOperation = new string[] 
                { @"SOFTWARE\Modulo\RiskManager\Info\CurrentVersion", @"SOFTWARE\Modulo\RiskManager\Info\CurrentBuild",
                  @"SOFTWARE\Modulo\RiskManagerNG\Info\CurrentVersion", @"SOFTWARE\Modulo\RiskManagerNG\Info\CurrentBuild" };


            IEnumerable<String> applyPatternMatchResult = operationEvaluator.ProcessOperationForKeyEntity(sourceObjectType, null);


            this.DoBasicAsserts(applyPatternMatchResult, 4);
            this.AssertOperationResult(applyPatternMatchResult, expectedItemsAfterOperation);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_PatternMatchOperation_on_trustee_sid_entity()
        {
            var sourceObject = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "800");
            List<List<String>> fakeSIDs = new List<List<String>>();
            fakeSIDs.Add(new List<String>(new string[] { @"S-1-5-1000", @"S-1-5-100-500", "", "S-1-5-18", "S-1-5-20" }));
            RegKeyEffectiveRightsOperationEvaluator operationEvaluator = 
                this.CreateMockedOperationEvaluator(new List<List<String>>(), fakeSIDs);

            var operationResult = operationEvaluator.ProcessOperationForTrusteeSidEntity(sourceObject, null);

            this.DoBasicAsserts(operationResult, 2);
            this.AssertOperationResult(operationResult, new string[] { "S-1-5-1000", "S-1-5-100-500" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_NotEqualsOperation_on_key_entity_of_regkeyeffectiverights_object()
        {
            var sourceObject = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "810");
            List<List<String>> fakeKeys = new List<List<String>>();
            fakeKeys.Add(new List<String>(new string[] { @"SOFTWARE\Modulo\NG", @"SOFTWARE\Modulo\RiskManager", @"SOFTWARE\Modulo\RMNG", @"SOFTWARE\Modulo\RiskManagerNG" }));
            RegKeyEffectiveRightsOperationEvaluator operationEvaluator = 
                this.CreateMockedOperationEvaluator(fakeKeys, new List<List<String>>());

            var operationResult = operationEvaluator.ProcessOperationForKeyEntity(sourceObject, null);

            this.DoBasicAsserts(operationResult, 3);
            this.AssertOperationResult(operationResult, new string[] { fakeKeys[0][0], fakeKeys[0][2], fakeKeys[0][3] });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_NotEqualsOperation_on_TrusteeSID_entity_of_regkeyeffectiverights_object()
        {
            var sourceObject = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "820");
            var fakeTrusteeSIDs = new List<List<String>>();
            fakeTrusteeSIDs.Add(new string[] { "S-1-5-20", "S-1-5-18", "S-1-5-25" }.ToList());
            var operationEvaluator = CreateMockedOperationEvaluator(new List<List<String>>(), fakeTrusteeSIDs);

            var operationResult = operationEvaluator.ProcessOperationForTrusteeSidEntity(sourceObject, null);

            this.DoBasicAsserts(operationResult, 2);
            this.AssertOperationResult(operationResult, new string[] { "S-1-5-20", "S-1-5-25" });
        }



        private void DoBasicAsserts(IEnumerable<String> operationResultToAssert, int expectedResultCount)
        {
            Assert.IsNotNull(operationResultToAssert, "The result of operation processing cannot be null.");
            Assert.AreEqual(expectedResultCount, operationResultToAssert.Count(), "Unexpected items count after operation processing.");
        }

        private void AssertOperationResult(IEnumerable<String> operationResult, IEnumerable<String> orderedExpectedItems)
        {
            for (int i = 0; i < operationResult.Count(); i++)
            {
                string resultItem = operationResult.ElementAt(i);
                string expectedItem = orderedExpectedItems.ElementAt(i);
                Assert.AreEqual(expectedItem, resultItem, "An unexpected item in operation result was found.");
            }
        }

        private RegKeyEffectiveRightsOperationEvaluator CreateMockedOperationEvaluator(
            List<List<String>> fakeSubkeysToReturn, List<List<String>> fakeTrusteeSIDsToReturn)
        {
            MockRepository mocks = new MockRepository();
            var fakeSystemDataSource = mocks.StrictMock<RegKeyEffectiveRightsObjectCollector>();
            fakeSystemDataSource.TargetInfo = ProbeHelper.CreateFakeTarget();
            
            foreach(var fakeSubkey in fakeSubkeysToReturn)
                Expect.Call(fakeSystemDataSource.GetValues(null)).IgnoreArguments().Return(fakeSubkey);
            
            if (fakeSubkeysToReturn.Count == 2)
                Expect.Call(fakeSystemDataSource.GetValues(null)).IgnoreArguments().Return(fakeSubkeysToReturn[1]);

            foreach (var fakeTrustee in fakeTrusteeSIDsToReturn)
                Expect.Call(fakeSystemDataSource.SearchUserTrusteeSIDs()).Return(fakeTrustee);

            Expect.Call(
                fakeSystemDataSource.IsThereDACLOnRegistryKeyForUser(null, null, null))
                    .IgnoreArguments().Repeat.Any()
                    .Return(true);

            mocks.ReplayAll();

            return new RegKeyEffectiveRightsOperationEvaluator() { SystemDataSource = fakeSystemDataSource };
        }
    }
}
