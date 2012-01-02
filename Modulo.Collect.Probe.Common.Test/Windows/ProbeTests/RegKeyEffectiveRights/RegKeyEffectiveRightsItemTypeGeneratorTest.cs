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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Windows.WMI;


namespace Modulo.Collect.Probe.Windows.Test.RegKeyEffectiveRights
{
    [TestClass]
    public class RegKeyEffectiveRightsItemTypeGeneratorTest
    {
        private const string DEFAULT_REG_HIVE = "HKEY_LOCAL_MACHINE";
        private const string DEFAULT_REG_KEY = "SOFTWARE\\Modulo\\modSIC";
        private const uint KEY_CREATE_LINK_PERMISSION = 32;

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_to_collect_from_object_WITHOUT_referenced_variable()
        {
            #region regkeyeffectiverights53_object
            //<regkeyeffectiverights53_object id="oval:modulo:obj:750" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //    <hive>HKEY_LOCAL_MACHINE</hive>
            //    <key>SOFTWARE\Modulo\RiskManager</key>
            //    <trustee_sid>S-1-5-21-501351562-481299158-1019697294-10279</trustee_sid>
            //</regkeyeffectiverights53_object>
            #endregion

            var EXPECTED_HIVE = "HKEY_LOCAL_MACHINE";
            var EXPECTED_KEY = @"SOFTWARE\Modulo\RiskManager";
            var EXPECTED_TRUSTEE = "S-1-5-21-501351562-481299158-1019697294-10279";
            var regKeyEffectiveRightsObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "750");

            var generatedItems =
                CreateRegKeyEffectiveRightsItemTypeGeneratorWithoutBehavior()
                    .GetItemsToCollect(regKeyEffectiveRightsObject, VariableHelper.CreateEmptyEvaluatedVariables());

            ItemTypeChecker.DoBasicAssertForItems(generatedItems.ToArray(), 1, typeof(regkeyeffectiverights_item), StatusEnumeration.notcollected);

            AssertGeneratedRegKeyEffectiveRightsItem(generatedItems.ElementAt(0), EXPECTED_HIVE, EXPECTED_KEY, EXPECTED_TRUSTEE);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_to_collect_from_RegKeyEffectiveRightsObject_WITH_referenced_variable()
        {
            var fakeVariableValue = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            var regKeyEffectiveRightsObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "770");
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("770", "oval:modulo:var:770", fakeVariableValue);

            var itemsToCollect =
                CreateRegKeyEffectiveRightsItemTypeGeneratorWithoutBehavior()
                    .GetItemsToCollect(regKeyEffectiveRightsObject, fakeVariables);

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect.ToArray(), 1, typeof(regkeyeffectiverights_item), StatusEnumeration.notcollected);

            this.AssertGeneratedRegKeyEffectiveRightsItem(itemsToCollect.ElementAt(0), "HKEY_LOCAL_MACHINE", fakeVariableValue, "S-1-5-21-501351562-481299158-1019697294-10279");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_to_collected_from_RegKeyEffectiveRighstObject_with_referenced_variables_on_key_and_trusteeSID_entities()
        {
            var expectedKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            var sourceObjectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "771");
            var fakeVarValues = new Dictionary<String, IEnumerable<String>>();
            fakeVarValues.Add("oval:modulo:var:201", new string[] { @"S-1-5-18", @"S-1-5-20" });
            fakeVarValues.Add("oval:modulo:var:770", new string[] { expectedKey });
            var fakeVariables = VariableHelper.CreateEvaluatedVariables(sourceObjectType.id, fakeVarValues);

            var generatedItems =
                CreateRegKeyEffectiveRightsItemTypeGeneratorWithoutBehavior()
                    .GetItemsToCollect(sourceObjectType, fakeVariables);

            ItemTypeChecker.DoBasicAssertForItems(generatedItems.ToArray(), 2, typeof(regkeyeffectiverights_item), StatusEnumeration.notcollected);

            this.AssertGeneratedRegKeyEffectiveRightsItem(generatedItems.ElementAt(0), "HKEY_LOCAL_MACHINE", expectedKey, "S-1-5-18");
            this.AssertGeneratedRegKeyEffectiveRightsItem(generatedItems.ElementAt(1), "HKEY_LOCAL_MACHINE", expectedKey, "S-1-5-20");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_to_collect_from_ObjectType_with_pattern_match_operation()
        {
            var RegKeyStruct = @"SOFTWARE\Modulo\{0}\CurrentVersion";
            var fakeObject = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "780");
            var fakeKeys = new string[] { string.Format(RegKeyStruct, "RiskManager"), string.Format(RegKeyStruct, "RiskManagerNG") };
            var expectedTrusteeSID = ((regkeyeffectiverights53_object)fakeObject).GetAllObjectEntities()["trustee_sid"].Value;
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(fakeKeys, new string[] { "S-1-5-100-500" });

            var generatedItems = itemTypeGenerator.GetItemsToCollect(fakeObject, VariableHelper.CreateEmptyEvaluatedVariables());
            ItemTypeChecker.DoBasicAssertForItems(generatedItems.ToArray(), 2, typeof(regkeyeffectiverights_item), StatusEnumeration.notcollected);

            this.AssertGeneratedRegKeyEffectiveRightsItem(
                generatedItems.ElementAt(0), "HKEY_LOCAL_MACHINE", fakeKeys.ElementAt(0), expectedTrusteeSID);
            this.AssertGeneratedRegKeyEffectiveRightsItem(
                generatedItems.ElementAt(1), "HKEY_LOCAL_MACHINE", fakeKeys.ElementAt(1), expectedTrusteeSID);
        }

        [TestMethod, Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_to_collect_from_a_complex_object_type()
        {
            //# This test must be review. 
            //# In case of not equal operation with more than one given value,
            //# the operation that must be applied for each value given value or
            //# for given values like a set ?


            #region regkeyeffectiverights53_object id="oval:modulo:obj:830"
            //  <regkeyeffectiverights53_object id="oval:modulo:obj:830" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //    <hive>HKEY_LOCAL_MACHINE</hive>
            //    <key var_ref="oval:modulo:var:830" operation="pattern match"></key>
            //    <trustee_sid var_ref="oval:modulo:var:831" operation="not equal"></trustee_sid>
            //  </regkeyeffectiverights53_object>
            //  <constant_variable id="oval:modulo:var:830" datatype="string" version="1" comment="...">
            //    <value>SOFTWARE\Modulo\^RiskManager.*</value>
            //  </constant_variable>
            //  <constant_variable id="oval:modulo:var:831" datatype="string" version="1" comment="...">
            //    <value>S-1-5</value>
            //    <value>S-1-5-18</value>
            //  </constant_variable>
            #endregion
            var HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
            var fakeObject = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "830");
            var fakeSubKeys = new string[] { "ModuloRiskManager", "RiskManager", "RiskManagerNG" };
            var fakeTrusteeSIDs = new string[] { "S-1-5", "S-1-5-20", "S-1-5-25", "S-1-5-18" };
            var itemTypeGenerator = CreateMockedItemTypeGeneratorForAComplexObjectType(fakeSubKeys, fakeTrusteeSIDs);
            var fakeVars = new Dictionary<String, String[]>();
            fakeVars.Add("oval:modulo:var:830", new string[] { @"SOFTWARE\Modulo\^RiskManager.*" });
            fakeVars.Add("oval:modulo:var:831", new string[] { "S-1-5", "S-1-5-18" });
            var fakeEvaluatedVars = this.CreateFakeVariableEvaluated(fakeObject.id, fakeVars);

            var generatedItems = itemTypeGenerator.GetItemsToCollect(fakeObject, fakeEvaluatedVars);

            this.DoBasicAssert(generatedItems, 4);

            this.AssertGeneratedRegKeyEffectiveRightsItem(
                generatedItems.ElementAt(0), HKEY_LOCAL_MACHINE, @"SOFTWARE\Modulo\RiskManager", "S-1-5-20");
            this.AssertGeneratedRegKeyEffectiveRightsItem(
                generatedItems.ElementAt(1), HKEY_LOCAL_MACHINE, @"SOFTWARE\Modulo\RiskManager", "S-1-5-25");
            this.AssertGeneratedRegKeyEffectiveRightsItem(
                generatedItems.ElementAt(2), HKEY_LOCAL_MACHINE, @"SOFTWARE\Modulo\RiskManagerNG", "S-1-5-20");
            this.AssertGeneratedRegKeyEffectiveRightsItem(
                generatedItems.ElementAt(3), HKEY_LOCAL_MACHINE, @"SOFTWARE\Modulo\RiskManagerNG", "S-1-5-25");
        }

        /// <summary>
        /// It is for use case that has no information about registry key, ie, the registry hive effective rights will be collected.
        /// </summary>
        [TestMethod, Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_to_collect_from_a_object_that_contains_only_registry_hive()
        {
            var sourceObject = (regkeyeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "831");

            var generatedItems =
                CreateRegKeyEffectiveRightsItemTypeGeneratorWithoutBehavior()
                    .GetItemsToCollect(sourceObject, VariableHelper.CreateEmptyEvaluatedVariables());

            this.DoBasicAssert(generatedItems, 1);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_to_collect_only_for_users_that_have_DACL_on_registry_key()
        {
            //<regkeyeffectiverights53_object id="oval:modulo:obj:832">
            //    <hive>HKEY_LOCAL_MACHINE</hive>
            //    <key>SOFTWARE\Modulo\modSIC</key>
            //    <trustee_sid operation="pattern match">.*</trustee_sid>
            //</regkeyeffectiverights53_object>
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "832");

            var itemTypeGenerator = CreateMockedItemTypeGeneratorToReturnOnlySomeDACLs();

            var generatedItems = itemTypeGenerator.GetItemsToCollect(fakeObject, VariableHelper.CreateEmptyEvaluatedVariables());

            ItemTypeChecker.DoBasicAssertForItems(generatedItems.ToArray(), 3, typeof(regkeyeffectiverights_item));

            AssertGeneratedRegKeyEffectiveRightsItem(generatedItems.ElementAt(0), DEFAULT_REG_HIVE, DEFAULT_REG_KEY, "S-1-2");
            AssertGeneratedRegKeyEffectiveRightsItem(generatedItems.ElementAt(1), DEFAULT_REG_HIVE, DEFAULT_REG_KEY, "S-1-3");
            AssertGeneratedRegKeyEffectiveRightsItem(generatedItems.ElementAt(2), DEFAULT_REG_HIVE, DEFAULT_REG_KEY, "S-1-4");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_return_items_with_does_not_exist_status_when_operation_evaluator_throw_key_not_found_exception()
        {

        }

        //private RegKeyEffectiveRightsOperationEvaluator CreateOperationEvaluatorWithBehavior()
        //{
        //    var mocks = new MockRepository();
        //    var fakeOperationEvaluator= mocks.DynamicMock<RegKeyEffectiveRightsOperationEvaluator>();
        //    Expect.Call(
        //        fakeOperationEvaluator.ProcessOperationForKeyEntity(null, null)).
        //            IgnoreArguments().
        //            Throw(new 

        //    {
        //        SystemDataSource = new RegKeyEffectiveRightsObjectCollector()
        //        {
        //            AccessControlListProvider = Acce
        //}

        private VariablesEvaluated CreateFakeVariableEvaluated(string objectID, Dictionary<string, string[]> variables)
        {
            Dictionary<String, IEnumerable<String>> fakeVarValues = new Dictionary<String, IEnumerable<String>>();
            foreach (var variable in variables)
                fakeVarValues.Add(variable.Key, variable.Value);
            return VariableHelper.CreateEvaluatedVariables(objectID, fakeVarValues);
        }

        private RegKeyEffectiveRightsItemTypeGenerator CreateRegKeyEffectiveRightsItemTypeGeneratorWithoutBehavior()
        {
            var mocks = new MockRepository();
            var fakeSystemDataSource = mocks.DynamicMock<RegKeyEffectiveRightsObjectCollector>();
            Expect.Call(
                fakeSystemDataSource.IsThereDACLOnRegistryKeyForUser(null, null, null))
                    .IgnoreArguments().Repeat.Any().
                        Return(true);
            mocks.ReplayAll();

            return new RegKeyEffectiveRightsItemTypeGenerator()
            {
                OperationEvaluator = new RegKeyEffectiveRightsOperationEvaluator()
                {
                    SystemDataSource = fakeSystemDataSource
                }
            };
        }

        private RegKeyEffectiveRightsItemTypeGenerator CreateMockedItemTypeGeneratorForAComplexObjectType(
            string[] fakeSubKeys, string[] fakeTrusteeSIDs)
        {
            MockRepository mocks = new MockRepository();

            var fakeSystemDataSource = mocks.DynamicMock<RegKeyEffectiveRightsObjectCollector>();
            Expect.Call(
                fakeSystemDataSource.GetValues(null))
                    .IgnoreArguments()
                    .Return(fakeSubKeys);
            var fakeOperationEvaluator = mocks.DynamicMock<RegKeyEffectiveRightsOperationEvaluator>();
            fakeOperationEvaluator.SystemDataSource = fakeSystemDataSource;
            Expect.Call(
                fakeOperationEvaluator.ProcessOperationForKeyEntity(null, null))
                    .IgnoreArguments()
                    .CallOriginalMethod(OriginalCallOptions.NoExpectation);

            var fakeDACLsToReturn = fakeTrusteeSIDs.ToDictionary(sid => sid, sid => (uint)128);
            var fakeAclProvider = mocks.DynamicMock<AccessControlListProvider>();
            Expect.Call(
                fakeAclProvider
                    .GetRegKeyDACLs(null, null, null))
                    .IgnoreArguments()
                    .Return(fakeDACLsToReturn);

            mocks.ReplayAll();

            return new RegKeyEffectiveRightsItemTypeGenerator()
            {
                OperationEvaluator = fakeOperationEvaluator,
                ObjectCollector = new RegKeyEffectiveRightsObjectCollector() { AccessControlListProvider = fakeAclProvider }
            };
        }

        private RegKeyEffectiveRightsItemTypeGenerator CreateMockedItemTypeGeneratorToReturnOnlySomeDACLs()
        {


            //var fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            //Expect.Call(fakeACLProvider.GetRegistryKeyEffectiveRights(null, 0, null, null))
            //    .IgnoreArguments()
            //        .Return(KEY_CREATE_LINK_PERMISSION);
            //var fakeObjectCollector = new RegKeyEffectiveRightsObjectCollector()
            //{
            //    TargetInfo = ProbeHelper.CreateFakeTarget(),
            //    WmiDataProvider = fakeWmiDataProvider,
            //    AccessControlListProvider = fakeACLProvider
            //};

            var mocks = new MockRepository();
            var fakeACLProvider = mocks.DynamicMock<AccessControlListProvider>();

            var fakeReturn = new Dictionary<string, uint>();
            fakeReturn.Add("S-1-2", (uint)1055);
            fakeReturn.Add("S-1-3", (uint)95544);
            fakeReturn.Add("S-1-4", (uint)128);

            Expect.Call(
                fakeACLProvider.GetRegKeyDACLs(null, null, null))
                    .IgnoreArguments()
                        .Return(fakeReturn);

            mocks.ReplayAll();




            //Expect.Call(
            //    fakeObjectCollector.SearchUserTrusteeSIDs())
            //        .Return(new string[] { "S-1-1", "S-1-2", "S-1-3", "fakeReturnS-1-4", "S-1-5" });
            //Expect.Call(
            //    fakeObjectCollector.IsThereDACLOnRegistryKeyForUser(DEFAULT_REG_HIVE, DEFAULT_REG_KEY, "S-1-1"))
            //        .Return(false);
            //Expect.Call(
            //    fakeObjectCollector.IsThereDACLOnRegistryKeyForUser(DEFAULT_REG_HIVE, DEFAULT_REG_KEY, "S-1-2"))
            //        .Return(true);
            //Expect.Call(
            //    fakeObjectCollector.IsThereDACLOnRegistryKeyForUser(DEFAULT_REG_HIVE, DEFAULT_REG_KEY, "S-1-3"))
            //        .Return(true);
            //Expect.Call(
            //    fakeObjectCollector.IsThereDACLOnRegistryKeyForUser(DEFAULT_REG_HIVE, DEFAULT_REG_KEY, "S-1-4"))
            //        .Return(true);
            //Expect.Call(
            //    fakeObjectCollector.IsThereDACLOnRegistryKeyForUser(DEFAULT_REG_HIVE, DEFAULT_REG_KEY, "S-1-5"))
            //        .Return(false);


            var fakeObjectCollector = new RegKeyEffectiveRightsObjectCollector()
            {
                AccessControlListProvider = fakeACLProvider,
                TargetInfo = ProbeHelper.CreateFakeTarget()
            };

            return new RegKeyEffectiveRightsItemTypeGenerator()
            {
                OperationEvaluator = new RegKeyEffectiveRightsOperationEvaluator() { SystemDataSource = fakeObjectCollector },
                ObjectCollector = fakeObjectCollector
            };
        }

        private void DoBasicAssert(IEnumerable<ItemType> generatedItems, int expectedItemsCount)
        {
            ItemTypeChecker.DoBasicAssertForItems(generatedItems.ToArray(), expectedItemsCount, typeof(regkeyeffectiverights_item));
        }

        private void AssertGeneratedRegKeyEffectiveRightsItem(
            ItemType itemToAssert, string expectedHive, string expectedKey, string expectedTrusteeSID)
        {
            var generatedRegKeyEffectiveRightsItem = (regkeyeffectiverights_item)itemToAssert;
            ItemTypeEntityChecker.AssertItemTypeEntity(
                generatedRegKeyEffectiveRightsItem.hive,
                expectedHive);
            ItemTypeEntityChecker.AssertItemTypeEntity(
                generatedRegKeyEffectiveRightsItem.key,
                expectedKey);
            ItemTypeEntityChecker.AssertItemTypeEntity(
                generatedRegKeyEffectiveRightsItem.trustee_sid,
                expectedTrusteeSID);
        }

        private RegKeyEffectiveRightsItemTypeGenerator CreateMockedItemTypeGenerator(
            IEnumerable<String> fakeKeysToReturnAfterOperation, IEnumerable<String> fakeSIDsToReturnAfterOperation)
        {
            MockRepository mocks = new MockRepository();
            var fakeOperationEval = mocks.DynamicMock<RegKeyEffectiveRightsOperationEvaluator>();
            var fakeSystemDataSource = mocks.DynamicMock<RegKeyEffectiveRightsObjectCollector>();

            Expect.Call(fakeOperationEval.ProcessOperationForKeyEntity(null, null)).IgnoreArguments()
                .Return(new List<String>(fakeKeysToReturnAfterOperation));

            if (fakeSIDsToReturnAfterOperation != null)
                Expect.Call(
                    fakeOperationEval.ProcessOperationForTrusteeSidEntity(null, null))
                        .IgnoreArguments()
                        .Return(fakeSIDsToReturnAfterOperation);

            Expect.Call(
                fakeSystemDataSource.IsThereDACLOnRegistryKeyForUser(null, null, null))
                    .IgnoreArguments().Repeat.Any()
                    .Return(true);

            fakeOperationEval.SystemDataSource = fakeSystemDataSource;

            mocks.ReplayAll();

            return new RegKeyEffectiveRightsItemTypeGenerator() { OperationEvaluator = fakeOperationEval };
        }

    }
}
