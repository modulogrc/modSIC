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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Windows.WQL;
using Rhino.Mocks;

namespace Modulo.Collect.Probe.Windows.Test.WMI
{
    [TestClass]
    public class WmiSystemDataSourceTest
    {
        private const string UNEXPECTED_ENTITY_ITEM_VALUE_FOUND_ASSERT_FAIL_MSG = "An unexpected entity item value was found.";
        private const string FAKE_NAMESPACE = "root\\cimv2";
        private const string FAKE_WQL_OS = "select caption from Win32_OperatingSystem";
        private const string FAKE_WQL_USER_ACCOUNT = "select name from Win32_UserAccount where domain = 'mss'";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_simple_wmi_object()
        {
            var fakeOperatingSystemName = new string[] { "Windows 8" };
            var fakeWmiItem = this.CreateWmiItem(FAKE_NAMESPACE, FAKE_WQL_OS);
            var fakeWmiResult = this.CreateFakeWmiObjectList("caption", fakeOperatingSystemName);
            var wmiSystemDataSource = this.GetMockedWmiSystemDataSource(fakeWmiResult, null);

            var result = wmiSystemDataSource.CollectDataForSystemItem(fakeWmiItem).ToArray();

            ItemTypeChecker.DoBasicAssertForCollectedItems(result, 1, typeof(wmi_item), true);
            var collectedWmiItemToAssert = (wmi_item)result.Single().ItemType;
            AssertWmiResultOnWmiItem(collectedWmiItemToAssert, FAKE_NAMESPACE, FAKE_WQL_OS, fakeOperatingSystemName);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_wmi_object_that_returns_more_than_one_result()
        {
            var fakeUsernames = new string[] { "lfernandes", "cpaiva", "mgaspar" };
            var fakeWmiItem = this.CreateWmiItem(FAKE_NAMESPACE, FAKE_WQL_USER_ACCOUNT);
            var fakeWmiResult = this.CreateFakeWmiObjectList("name", fakeUsernames);
            var wmiSystemDataSource = this.GetMockedWmiSystemDataSource(fakeWmiResult, null);

            var result = wmiSystemDataSource.CollectDataForSystemItem(fakeWmiItem).ToArray();

            ItemTypeChecker.DoBasicAssertForCollectedItems(result, 1, typeof(wmi_item), true);
            var collectedWmiItemToAssert = (wmi_item)result.Single().ItemType;
            AssertWmiResultOnWmiItem(collectedWmiItemToAssert, FAKE_NAMESPACE, FAKE_WQL_USER_ACCOUNT, fakeUsernames);
        }

        [TestMethod, Owner("lfernandes")]
        public void A_wmi_item_with_error_status_must_be_returned_if_an_item_with_error_was_passed()
        {
            var fakeWmiItemWithError = new wmi_item() { status = StatusEnumeration.error };

            var result = new WMIObjectCollector(null).CollectDataForSystemItem(fakeWmiItemWithError).ToArray();

            ItemTypeChecker.DoBasicAssertForCollectedItems(result, 1, typeof(wmi_item), false);
            ItemTypeChecker.AssertItemTypeWithErrorStatus(result.Single().ItemType, null);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_an_error_occurs_an_item_if_error_status_and_error_message_must_be_returned()
        {
            var fakeWmiItem = this.CreateWmiItem(FAKE_NAMESPACE, FAKE_WQL_OS);
            var fakeWmiResult = this.CreateFakeWmiObjectList("caption", new string[] { "Windows 8" });
            var fakeException = new Exception("Test Exception was thrown");
            var wmiSystemDataSource = this.GetMockedWmiSystemDataSource(null, fakeException);

            var result = wmiSystemDataSource.CollectDataForSystemItem(fakeWmiItem).ToArray();

            ItemTypeChecker.DoBasicAssertForCollectedItems(result, 1, typeof(wmi_item), false);
            ItemTypeChecker.AssertItemTypeWithErrorStatus(result.Single().ItemType, "[WMIObjectCollector]");
        }




        private void DoBasicAssert(CollectedItem[] collectedItems, int expectedResultCount)
        {
            Assert.IsNotNull(collectedItems, "The result of item collection cannot be null");
            Assert.AreEqual(1, collectedItems.Count(), "Unexpected items count was found.");
        }

        private void AssertWmiResultOnWmiItem(wmi_item itemToAssert, string expectedNamespace, string expectedWQL, string[] expectedResults)
        {
            Assert.AreEqual(StatusEnumeration.exists, itemToAssert.status, "An unexpected collected item status was found.");
            Assert.AreEqual(expectedNamespace, itemToAssert.@namespace.Value, UNEXPECTED_ENTITY_ITEM_VALUE_FOUND_ASSERT_FAIL_MSG);
            Assert.AreEqual(expectedWQL, itemToAssert.wql.Value, UNEXPECTED_ENTITY_ITEM_VALUE_FOUND_ASSERT_FAIL_MSG);

            Assert.AreEqual(expectedResults.Count(), itemToAssert.result.Count(), "An unexpected wmi records count was returned.");
            for (int i = 0; i < expectedResults.Count(); i++)
            {
                var expectedWmiFieldValue = expectedResults.ElementAt(i);
                var foundWmiFieldValue = itemToAssert.result.ElementAt(i).Value.ToString();
                Assert.AreEqual(expectedWmiFieldValue, foundWmiFieldValue, "An unexpected item result value was found.");
            }
        }

        private wmi_item CreateWmiItem(string nameSpace, string wql)
        {
            return new wmi_item() 
            { 
                @namespace = OvalHelper.CreateItemEntityWithStringValue(nameSpace), 
                wql = OvalHelper.CreateItemEntityWithStringValue(wql) 
            };
        }

        private WMIObjectCollector GetMockedWmiSystemDataSource(IEnumerable<WmiObject> fakeWmiResult, Exception exceptionToThrow)
        {
            MockRepository mocks = new MockRepository();
            var fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();

            if (exceptionToThrow != null)
                Expect.Call(fakeWmiDataProvider.ExecuteWQL(null)).IgnoreArguments().Throw(exceptionToThrow);

            if (fakeWmiResult != null)
                Expect.Call(fakeWmiDataProvider.ExecuteWQL(null)).IgnoreArguments().Return(fakeWmiResult);

            mocks.ReplayAll();

            return new WMIObjectCollector(null) { WmiDataProvider = fakeWmiDataProvider };
        }

        private IEnumerable<WmiObject> CreateFakeWmiObjectList(string wmiFieldName, string[] wmiFieldValues)
        {
            var result = new List<WmiObject>();
            foreach (var wmiFieldValue in wmiFieldValues)
            {
                var fakeWmiObject = new WmiObject();
                fakeWmiObject.Add(wmiFieldName, wmiFieldValue);

                result.Add(fakeWmiObject);
            }

            return result;
        }
    }
    

}
