/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.Registry;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass()]
    public class RegistryObjectCollectorTests
    {
        private const string FAKE_REGISTRY_KEY = "SOFTWARE\\Modulo\\RiskManagerNG";
        private const string FAKE_REGISTRY_NAME = "ProductID";
        private const int VALUE_TYPE_ID_FOR_REGISTRY_NAME_THAT_NOT_EXISTS = 0;
        private const string ITEM_STATUS_MUST_BE_DOES_NOT_EXIST_MESSAGE = "The item status must be equal to 'does not exist'";

        [TestMethod, Owner("lfernandes")]
        public void Should_Be_Possible_To_Search_SubKeys_From_KeyName()
        {
            object fakeFoundSubKeys = new string[] { "Graphics", "GData", "XPTO" };
            Dictionary<string, object> searchKeysParameters = new Dictionary<string, object>();
            searchKeysParameters.Add(registry_object_ItemsChoices.hive.ToString(), eHiveNames.HKEY_LOCAL_MACHINE.ToString());
            searchKeysParameters.Add(registry_object_ItemsChoices.key.ToString(), "Microsoft");
            
            AbstractConstraint[] invokeMethodConstraints = new AbstractConstraint[] 
                { new Equal("EnumKey"), new Anything(), new Anything() };
            
            
            MockRepository mocks = new MockRepository();
            WmiDataProvider fakeWmiPrv = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeWmiPrv.InvokeMethod("", null, "")).Constraints(invokeMethodConstraints).Return(fakeFoundSubKeys);
            mocks.ReplayAll();
            
            RegistryObjectCollector systemDataSource = new RegistryObjectCollector() { WmiDataProvider = fakeWmiPrv };
            IList<string> registrySubKeys = systemDataSource.GetValues(searchKeysParameters);
            
            mocks.VerifyAll();

            Assert.IsNotNull(registrySubKeys);
            Assert.AreEqual(3, registrySubKeys.Count);
            Assert.AreEqual("Graphics", registrySubKeys[0]);
            Assert.AreEqual("GData", registrySubKeys[1]);
            Assert.AreEqual("XPTO", registrySubKeys[2]);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_Be_Possible_To_Search_KeyValues_From_KeyName()
        {
            object fakeFoundKeyValues = new List<string>(new string[] { "Risk Manager", "PCN", "Workflow Manager", "Dashboard" } );
            Dictionary<string, object> searchValuesParameters = new Dictionary<string, object>();
            searchValuesParameters.Add(registry_object_ItemsChoices.hive.ToString(), eHiveNames.HKEY_LOCAL_MACHINE.ToString());
            searchValuesParameters.Add(registry_object_ItemsChoices.key.ToString(), "SOFTWARE\\Modulo");
            searchValuesParameters.Add(registry_object_ItemsChoices.name.ToString(), "^.*");

            AbstractConstraint[] invokeMethodConstraints = 
                new AbstractConstraint[] { new Equal("EnumValues"), new Anything(), new Anything() };

            MockRepository mocks = new MockRepository();
            WmiDataProvider fakeWmiPrv = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeWmiPrv.InvokeMethod("", null, "")).Constraints(invokeMethodConstraints).Return(fakeFoundKeyValues);
            mocks.ReplayAll();

            RegistryObjectCollector systemDataSource = new RegistryObjectCollector() { WmiDataProvider = fakeWmiPrv };
            IList<string> registryKeyValues = systemDataSource.GetValues(searchValuesParameters);

            mocks.VerifyAll();

            Assert.IsNotNull(registryKeyValues);
            Assert.AreEqual(4, registryKeyValues.Count);
            Assert.IsTrue(registryKeyValues[0].Equals("Risk Manager"));
            Assert.IsTrue(registryKeyValues[1].Equals("PCN"));
            Assert.IsTrue(registryKeyValues[2].Equals("Workflow Manager"));
            Assert.IsTrue(registryKeyValues[3].Equals("Dashboard"));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_simple_registry_item()
        {
            var fakeItemToCollect = this.CreateFakeRegistryItem();
            var fakeWmiMethodReturn = this.CreateDictionaryWithOneElement("sValue", "1");
            var fakeWmiEnumValuesReturn = this.CreateFakeEnumValuesReturn(1);
            var objectCollector = this.CreateMockedRegistryObjectCollector(fakeWmiMethodReturn, fakeWmiEnumValuesReturn);
            
            var collectedItems = objectCollector.CollectDataForSystemItem(fakeItemToCollect);

            Assert.IsNotNull(collectedItems);
            Assert.AreEqual(1, collectedItems.Count());
            
            var collectedItem = collectedItems.Single().ItemType;
            Assert.IsNotNull(collectedItem);
            Assert.IsInstanceOfType(collectedItem, typeof(registry_item));
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status);

            var collectedRegistryItem = (registry_item)collectedItem;
            AssertRegistryEntityItem(collectedRegistryItem.hive, eHiveNames.HKEY_LOCAL_MACHINE.ToString());
            AssertRegistryEntityItem(collectedRegistryItem.key, FAKE_REGISTRY_KEY);
            AssertRegistryEntityItem(collectedRegistryItem.name, FAKE_REGISTRY_NAME);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_binary_registry_key_with_false_value()
        {
            var fakeItemToCollect = this.CreateFakeRegistryItem();
            var fakeWmiMethodReturn = this.CreateDictionaryWithOneElement("sValue", new byte[] { 0 });
            var fakeEnumValuesMethodReturn = this.CreateFakeEnumValuesReturn(3);
            var objectCollector = CreateMockedRegistryObjectCollector(fakeWmiMethodReturn, fakeEnumValuesMethodReturn);

            var collectedItem = objectCollector.CollectDataForSystemItem(fakeItemToCollect).Single().ItemType;

            var typeEntity = ((registry_item)collectedItem).type;
            Assert.IsNotNull(typeEntity);
            Assert.AreEqual("reg_binary", typeEntity.Value);

            var valueEntity = ((registry_item)collectedItem).value;
            Assert.IsNotNull(valueEntity);
            Assert.AreEqual(1, valueEntity.Count());
            Assert.AreEqual("00", valueEntity.Single().Value);
            Assert.AreEqual(SimpleDatatypeEnumeration.binary, valueEntity.Single().datatype);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_binary_registry_key_with_true_value()
        {
            var fakeItemToCollect = this.CreateFakeRegistryItem();
            var fakeWmiMethodReturn = this.CreateDictionaryWithOneElement("sValue", new byte[] { 1 });
            var fakeEnumValuesMethodReturn = this.CreateFakeEnumValuesReturn((int)eValueTypes.BINARY);
            var objectCollector = CreateMockedRegistryObjectCollector(fakeWmiMethodReturn, fakeEnumValuesMethodReturn);

            var collectedItem = objectCollector.CollectDataForSystemItem(fakeItemToCollect).Single().ItemType;

            var typeEntity = ((registry_item)collectedItem).type;
            Assert.IsNotNull(typeEntity);
            Assert.AreEqual("reg_binary", typeEntity.Value);

            var valueEntity = ((registry_item)collectedItem).value;
            Assert.IsNotNull(valueEntity);
            Assert.AreEqual(1, valueEntity.Count());
            Assert.AreEqual("01", valueEntity.Single().Value);
            Assert.AreEqual(SimpleDatatypeEnumeration.binary, valueEntity.Single().datatype);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_collect_a_DWORD_registry_key()
        {
            object fakeValue = 50;
            var fakeItemToCollect = this.CreateFakeRegistryItem();
            var fakeWmiMethodReturn = this.CreateDictionaryWithOneElement("sValue", fakeValue);
            var fakeEnumValuesMethodReturn = this.CreateFakeEnumValuesReturn((int)eValueTypes.DWORD);

            var objectCollector = CreateMockedRegistryObjectCollector(fakeWmiMethodReturn, fakeEnumValuesMethodReturn);

            var collectedItem = objectCollector.CollectDataForSystemItem(fakeItemToCollect).Single().ItemType;

            var typeEntity = ((registry_item)collectedItem).type;
            Assert.IsNotNull(typeEntity);
            Assert.AreEqual("reg_dword", typeEntity.Value);

            var valueEntity = ((registry_item)collectedItem).value;
            Assert.IsNotNull(valueEntity);
            Assert.AreEqual(1, valueEntity.Count());
            Assert.AreEqual("50", valueEntity.Single().Value);
            Assert.AreEqual(SimpleDatatypeEnumeration.@int, valueEntity.Single().datatype);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_registry_key_does_not_exists_the_registry_item_status_must_be_does_not_exist()
        {
            var fakeItemToCollect = this.CreateFakeRegistryItem();
            var fakeEnumValuesMethodReturn = this.CreateDictionaryWithOneElement("ReturnValue", (uint)2);
            var objectCollector = CreateMockedRegistryObjectCollector(null, fakeEnumValuesMethodReturn);

            var collectedItem = (registry_item)objectCollector.CollectDataForSystemItem(fakeItemToCollect).Single().ItemType;

            Assert.AreEqual(fakeItemToCollect.hive.Value, collectedItem.hive.Value);
            // Asserting Key Status and Value
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.key.status);
            Assert.AreEqual(fakeItemToCollect.key.Value, collectedItem.key.Value);
            Assert.IsNull(collectedItem.name);
            // Asserting the final registry item status
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.status, ITEM_STATUS_MUST_BE_DOES_NOT_EXIST_MESSAGE);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_registry_name_does_not_exists_the_registry_item_status_must_be_does_not_exist()
        {
            var fakeItemToCollect = this.CreateFakeRegistryItem();
            var fakeEnumValuesMethodReturn = CreateFakeEnumValuesReturn(VALUE_TYPE_ID_FOR_REGISTRY_NAME_THAT_NOT_EXISTS);
            var objectCollector = CreateMockedRegistryObjectCollector(null, fakeEnumValuesMethodReturn);

            var collectedItem = (registry_item)objectCollector.CollectDataForSystemItem(fakeItemToCollect).Single().ItemType;

            Assert.AreEqual(fakeItemToCollect.hive.Value, collectedItem.hive.Value);
            // Asserting Key Status and Value
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.key.status);
            Assert.AreEqual(fakeItemToCollect.key.Value, collectedItem.key.Value);
            // Asserting Name status and the final status of collected registry item.
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.name.status);
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.status, ITEM_STATUS_MUST_BE_DOES_NOT_EXIST_MESSAGE);
        }

        [TestMethod, Owner("lfernandes")]
        public void Learning_how_to_format_a_string_from_hexadecimal_arguments()
        {
            var stringWithHexadecimalArgs = string.Format("{0:x2}{1:x2}{2:x2}", (byte)186, (byte)186, (byte)202).ToUpper();
            var expectedResult = "BABACA";
            Assert.AreEqual(expectedResult, stringWithHexadecimalArgs);
        }

        [TestMethod, Owner("lfernandes")]
        public void Checking_PathCombineMethod_behavior_with_more_than_two_path()
        {
            var registryHive = "HKEY_LOCAL_MACHINE";
            var registryKey = "Software\\Modulo";
            var registryName = "RiskManagerNG";
            
            var combinedPath = System.IO.Path.Combine(registryHive, registryKey, registryName);

            Assert.AreEqual("HKEY_LOCAL_MACHINE\\Software\\Modulo\\RiskManagerNG", combinedPath);




        }

        private void AssertRegistryEntityItem(EntityItemSimpleBaseType entityToAssert, string expectedEntityValue)
        {
            Assert.IsNotNull(entityToAssert);
            Assert.AreEqual(StatusEnumeration.exists, entityToAssert.status);
            Assert.AreEqual(expectedEntityValue, entityToAssert.Value);
        }

        private registry_item CreateFakeRegistryItem()
        {
            return new registry_item()
            {
                hive = new EntityItemRegistryHiveType() { Value = eHiveNames.HKEY_LOCAL_MACHINE.ToString() },
                key = OvalHelper.CreateItemEntityWithStringValue(FAKE_REGISTRY_KEY),
                name = OvalHelper.CreateItemEntityWithStringValue(FAKE_REGISTRY_NAME)
            };
        }

        private Dictionary<string, object> CreateFakeEnumValuesReturn(int valueTypeID)
        {
            var wmiFieldNames = new string[] { "ReturnValue", "sNames", "Types" };
            var wmiFieldValues = new object[] { (uint)0, new string[] { FAKE_REGISTRY_NAME }, new int[] { valueTypeID } };
            
            return this.CreateDictionary(wmiFieldNames, wmiFieldValues);
        }

        private RegistryObjectCollector CreateMockedRegistryObjectCollector(
            Dictionary<string, object> fakeWmiMethodReturn, 
            Dictionary<string, object> fakeEnumValuesMethodReturn)
        {
            MockRepository mockRepository = new MockRepository();
            
            var fakeWmiDataProvider = mockRepository.StrictMock<WmiDataProvider>();

            Expect.Call(fakeWmiDataProvider.InvokeMethod("EnumValues", null)).IgnoreArguments()
                .Return(fakeEnumValuesMethodReturn);
            Expect.Call(fakeWmiDataProvider.InvokeMethod("GetStringValue", null)).IgnoreArguments()
                .Return(fakeWmiMethodReturn);
            
            mockRepository.ReplayAll();

            return new RegistryObjectCollector() { WmiDataProvider = fakeWmiDataProvider };
        }

        private Dictionary<string, object> CreateDictionaryWithOneElement(string elementKey, object elementValue)
        {
            var newDictionary = new Dictionary<string, object>();
            newDictionary.Add(elementKey, elementValue);
            
            return newDictionary;
        }

        private Dictionary<string, object> CreateDictionary(string[] keys, object[] values)
        {
            var newDictionary = new Dictionary<string, object>();
            for(int i = 0; i < keys.Length; i++)
                newDictionary.Add(keys.ElementAt(i), values.ElementAt(i));

            return newDictionary;
        }
    }



}
