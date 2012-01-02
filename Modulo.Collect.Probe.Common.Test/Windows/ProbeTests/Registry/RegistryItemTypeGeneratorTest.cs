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

using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Windows.Registry;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.Probe.Windows.Test.Helpers;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Windows.Test.Registry
{
    [TestClass]
    public class RegistryItemTypeGeneratorTest
    {
        private const string DEFINITIONS_REGEX_ON_VALUE = "fdcc_xpfirewall_oval_regex_on_value.xml";
        private const string DEFINITIONS_SIMPLE = "definitionsSimple.xml";
        private const string DEFINITIONS_WITH_LOCAL_VARIABLE = "definitionsWithLocalVariable.xml";

        private const string OBJ_50003_ID = "oval:modulo:obj:50003";
        private const string OBJ_50004_ID = "oval:modulo:obj:50004";
        private const string OBJ_50005_ID = "oval:modulo:obj:50005";
        private const string OBJ_50006_ID = "oval:modulo:obj:50006";

        private const string OBJ_MITRE_3000_ID = "oval:org.mitre.oval:obj:3000";
        private const string VAR_MITRE_3000_ID = "oval:org.mitre.oval:var:3000";


        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_generate_itemTypes_from_objectTypes()
        {
            var ovalObject = WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_WITH_LOCAL_VARIABLE, OBJ_MITRE_3000_ID);
            var fakeDataSource = WindowsTestHelper.GetDataSourceFakewithoutRegex();
            var wmiDataProvider = new WmiDataProviderExpectFactory().GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess();
            var fakeRegistryKeyPath = new List<string>() { @"Software\Microsoft\Windows NT\CurrentVersion" };
            var variable = new VariableValue(ovalObject.id, VAR_MITRE_3000_ID, fakeRegistryKeyPath);
            var variables = new VariablesEvaluated(new List<VariableValue>() { variable });


            RegistryItemTypeGenerator itemGenerator = new RegistryItemTypeGenerator() { SystemDataSource = fakeDataSource, WmiDataProvider = wmiDataProvider };
            IEnumerable<ItemType> itemsToCollect = itemGenerator.GetItemsToCollect(ovalObject, variables);


            Assert.AreEqual(1, itemsToCollect.Count(), "the quantity of items is not expected");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(0), "HKEY_LOCAL_MACHINE", @"Software\Microsoft\Windows NT\CurrentVersion", "CurrentVersion");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_itemTypes_from_objectTypes_with_regex_operation()
        {
            string hiveHKLM = eHiveNames.HKEY_LOCAL_MACHINE.ToString();
            string startKey = "SOFTWARE\\Microsoft\\Windows";
            var obj50003 = WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_REGEX_ON_VALUE, OBJ_50003_ID);


            BaseObjectCollector fakeDataSource = WindowsTestHelper.GetDataSourceFakeWithRegex(startKey, 2);
            WmiDataProvider wmiDataProvider = new WmiDataProviderExpectFactory().GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess();

            RegistryItemTypeGenerator itemGenerator = new RegistryItemTypeGenerator() { SystemDataSource = fakeDataSource, WmiDataProvider = wmiDataProvider };
            var itemsToCollect = itemGenerator.GetItemsToCollect(obj50003, VariableHelper.CreateEmptyEvaluatedVariables()).Cast<registry_item>();

            Assert.AreEqual(4, itemsToCollect.Count());
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(0), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentBuild", "CurrentBuild");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(1), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentBuild", "LastBuild");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(2), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion", "CurrentBuild");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(3), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion", "LastBuild");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_generate_itemTypes_from_objectTypes_with_not_equal_operation()
        {
            string hiveHKLM = eHiveNames.HKEY_LOCAL_MACHINE.ToString();
            string startKey = "SOFTWARE\\Microsoft\\Windows";
            var obj50006 = WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_REGEX_ON_VALUE, OBJ_50006_ID);

            var fakeDataSource = new SystemDataSourceFactory().GetDataSourceFakeWithSpecificNames(startKey, new string[] { "CommonFileDir", 
                                                                                                                                         "DevicePath",
                                                                                                                                         "CSDVersion",
                                                                                                                                         "MediaPath"});
            WmiDataProvider wmiDataProvider = new WmiDataProviderExpectFactory().GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess();
            RegistryItemTypeGenerator itemGenerator = new RegistryItemTypeGenerator() { SystemDataSource = fakeDataSource, WmiDataProvider = wmiDataProvider };
            var itemsToCollect = itemGenerator.GetItemsToCollect(obj50006, VariableHelper.CreateEmptyEvaluatedVariables()).Cast<registry_item>();
            Assert.AreEqual(3, itemsToCollect.Count());
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(0), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion", "CommonFileDir");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(1), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion", "DevicePath");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(2), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion", "MediaPath");


        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_generate_itemTypes_from_objectTypes_with_variables()
        {
            string hiveHKLM = eHiveNames.HKEY_LOCAL_MACHINE.ToString();
            string key = @"Software\Microsoft\Windows NT\CurrentVersion";
            string name = "CurrentType";
            var ovalObject = WindowsTestHelper.GetObjectFromDefinitions("definitionsWithLocalVariable.xml", "oval:org.mitre.oval:obj:4000");
            BaseObjectCollector fakeDataSource = WindowsTestHelper.GetDataSourceFakewithoutRegex();
            WmiDataProvider wmiDataProvider = new WmiDataProviderExpectFactory().GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess();

            VariableValue variable = new VariableValue(ovalObject.id, "oval:org.mitre.oval:var:4000", new List<string>() { key });
            VariablesEvaluated variables = new VariablesEvaluated(new List<VariableValue>() { variable });
            
            var itemGenerator = new RegistryItemTypeGenerator() { SystemDataSource = fakeDataSource, WmiDataProvider = wmiDataProvider };
            var itemsToCollect = itemGenerator.GetItemsToCollect(ovalObject, variables);

            Assert.IsTrue(itemsToCollect.Count() == 1, "the quantity of items is not expected");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(0), hiveHKLM, key, name);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_generate_itemTypes_from_objectTypes_with_variables_and_regex()
        {
            string hiveHKLM = eHiveNames.HKEY_LOCAL_MACHINE.ToString();
            string startKey = "SOFTWARE\\Microsoft\\Windows";
            var ovalObject = WindowsTestHelper.GetObjectFromDefinitions("definitionsWithLocalVariable.xml", "oval:modulo:obj:5000");

            var fakeDataSource = WindowsTestHelper.GetDataSourceFakeWithRegex(startKey, 1);
            WmiDataProvider wmiDataProvider = new WmiDataProvider();// new WmiDataProviderFactory().GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess();

            VariableValue variable = new VariableValue(ovalObject.id, "oval:org.mitre.oval:var:3000", new List<string>() { "CurrentType" });
            VariablesEvaluated variables = new VariablesEvaluated(new List<VariableValue>() { variable });

            var itemGenerator = new RegistryItemTypeGenerator() { SystemDataSource = fakeDataSource, WmiDataProvider = wmiDataProvider };
            var itemsToCollect = itemGenerator.GetItemsToCollect(ovalObject, variables).Cast<registry_item>();

            Assert.AreEqual(2, itemsToCollect.Count());
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(0), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentBuild", "CurrentType");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(1), hiveHKLM, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion", "CurrentType");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_itemTypes_from_objectType_with_PatternMatchOperation_on_key_and_name_entities_at_same_time()
        {
            var objectType = WindowsTestHelper.GetObjectFromDefinitions("definitionsWithOnlyObjects.xml", "oval:modulo:obj:12345");
            var fakeDataSource = WindowsTestHelper.GetDataSourceFakeWithRegex("", 2);
            var fakeWmiDataProvider = new WmiDataProvider();// new WmiDataProviderFactory().GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess();
            var itemTypeGenerator = new RegistryItemTypeGenerator() { SystemDataSource = fakeDataSource, WmiDataProvider = fakeWmiDataProvider };
            
            
            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectType, VariableHelper.CreateEmptyEvaluatedVariables());


            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect method cannot be null.");
            Assert.AreEqual(4, generatedItems.Count(), "Unexpected generated items type count");
        }

        [Ignore, Owner("lcosta")]
        public void Should_be_possible_to_define_a_not_equals_operation_on_the_keyEntity()
        {
            string hiveHKLM = eHiveNames.HKEY_LOCAL_MACHINE.ToString();
            string startKey = "SOFTWARE\\Adobe";
            var ovalObject = WindowsTestHelper.GetObjectFromDefinitions("definitionsWithLocalVariable.xml", "oval:modulo:obj:6000");

            var fakeDataSource = 
                new SystemDataSourceFactory()
                    .GetDataSourceFakeWithSpecificNames(startKey, new string[] { "Acrobat Reader\\9.0\\Installer",
                                                                                 "Acrobat Reader\\9.0\\InstallPath",
                                                                                 "Acrobat Reader\\Language\\current",
                                                                                 "Adobe Air\\FileTypeRegistration",
                                                                                 "Adobe Air\\Repair\\9.0\\IOD"});

            var wmiDataProvider = new WmiDataProviderExpectFactory().GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess();
            var itemGenerator = new RegistryItemTypeGenerator() { SystemDataSource = fakeDataSource, WmiDataProvider = wmiDataProvider };
            var itemsToCollect = itemGenerator.GetItemsToCollect(ovalObject, VariableHelper.CreateEmptyEvaluatedVariables()).Cast<registry_item>();

            Assert.AreEqual(2, itemsToCollect.Count());
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(0), hiveHKLM, "SOFTWARE\\Adobe\\Adobe Air\\FileTypeRegistration", "Path");
            this.AssertGeneratedRegistryItem(itemsToCollect.ElementAt(1), hiveHKLM, "SOFTWARE\\Adobe\\Repair\\9.0\\IOD", "Path");
        }




        private void AssertGeneratedRegistryItem(ItemType generatedItem, string expectedHive, string expectedKey, string expectedName)
        {
            Assert.IsInstanceOfType(generatedItem, typeof(registry_item), "The type of generated Registry Item must be 'registry_item'");

            registry_item registryItem = (registry_item)generatedItem;
            Assert.AreEqual(expectedHive, registryItem.hive.Value, "A generated Registry Item with an unexpected 'hive' was found.");
            Assert.AreEqual(expectedKey, registryItem.key.Value, "A generated Registry Item with an unexpected 'key' was found.");
            Assert.AreEqual(expectedName, registryItem.name.Value, "A generated Registry Item with an unexpected 'Name' was found.");
        }
    }
}
