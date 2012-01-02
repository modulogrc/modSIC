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
using Modulo.Collect.Probe.Windows.Probes.WMI.Wmi57;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.WMI.Wmi57
{
    [TestClass]
    public class Wmi57ObjectCollectorTests
    {
        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_a_wmi57_item()
        {
            var fakeWmi57ItemToCollect = new wmi57_item() { @namespace = OvalHelper.CreateItemEntityWithStringValue("root\\cimv2"), wql = OvalHelper.CreateItemEntityWithStringValue("Select Name, Domain From Win32_Account") };
            var wmi57ObjectCollector = CreateWmi57ObjectCollectorWithAFakeWmiDataProvider();

            var collectedItems = wmi57ObjectCollector.CollectDataForSystemItem(fakeWmi57ItemToCollect);

            Assert.AreEqual(1, collectedItems.Count());
            var collectedItem = (wmi57_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status);
            
            Assert.AreEqual("root\\cimv2", collectedItem.@namespace.Value);
            Assert.AreEqual("Select Name, Domain From Win32_Account", collectedItem.wql.Value);
            Assert.AreEqual(2, collectedItem.result.Count());
            
            Assert.AreEqual(2, collectedItem.result[0].field.Count());
            Assert.AreEqual(ComplexDatatypeEnumeration.record, collectedItem.result[0].datatype);
            Assert.AreEqual("name", collectedItem.result[0].field[0].name);
            Assert.AreEqual("cpaiva", collectedItem.result[0].field[0].Value);
            Assert.AreEqual("domain", collectedItem.result[0].field[1].name);
            Assert.AreEqual("mss", collectedItem.result[0].field[1].Value);
            
            Assert.AreEqual(2, collectedItem.result[1].field.Count());
            Assert.AreEqual(ComplexDatatypeEnumeration.record, collectedItem.result[1].datatype);
            Assert.AreEqual("name", collectedItem.result[1].field[0].name);
            Assert.AreEqual("jcastro", collectedItem.result[1].field[0].Value);
            Assert.AreEqual("domain", collectedItem.result[1].field[1].name);
            Assert.AreEqual("local", collectedItem.result[1].field[1].Value);
        }

        [TestMethod, Owner("cpaiva")]
        public void When_the_wql_return_is_empty_an_item_with_a_result_entity_with_doesNotExist_status()
        {
            var fakeWmi57ItemToCollect = new wmi57_item() { @namespace = OvalHelper.CreateItemEntityWithStringValue("root\\cimv2"), wql = OvalHelper.CreateItemEntityWithStringValue("Select Name, Domain From Win32_Account") };
            var wmi57ObjectCollector = CreateWmi57ObjectCollectorWithAFakeWmiDataProvider(true);

            var collectedItems = wmi57ObjectCollector.CollectDataForSystemItem(fakeWmi57ItemToCollect);

            Assert.AreEqual(1, collectedItems.Count());
            var collectedItem = (wmi57_item)collectedItems.Single().ItemType;
            Assert.AreEqual("root\\cimv2", collectedItem.@namespace.Value);
            Assert.AreEqual("Select Name, Domain From Win32_Account", collectedItem.wql.Value);
            Assert.AreEqual(1, collectedItem.result.Count());
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.result[0].status);
            Assert.AreEqual(ComplexDatatypeEnumeration.record, collectedItem.result[0].datatype);

        }


        [TestMethod, Owner("modSIC Team")]
        public void When_the_wql_return_is_empty_an_item_with_doesNotExist_status()
        {
            var fakeWmi57ItemToCollect = new wmi57_item() { @namespace = OvalHelper.CreateItemEntityWithStringValue("root\\cimv2"), wql = OvalHelper.CreateItemEntityWithStringValue("Select Name, Domain From Win32_Account") };
            var wmi57ObjectCollector = CreateWmi57ObjectCollectorWithAFakeWmiDataProvider(true);

            var collectedItems = wmi57ObjectCollector.CollectDataForSystemItem(fakeWmi57ItemToCollect);

            var collectedItem = (wmi57_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.status, "Item with unexpected status was found.");
        }


        private Wmi57ObjectCollector CreateWmi57ObjectCollectorWithAFakeWmiDataProvider(bool emptyQuery = false)
        {
            MockRepository mocks = new MockRepository();
            var fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            var fakeReturn = emptyQuery ? new WmiObject[0] : CreateFakeWmiReturn();
            Expect.Call(fakeWmiDataProvider.ExecuteWQL(null)).IgnoreArguments().Return(fakeReturn);
            mocks.ReplayAll();

            return new Wmi57ObjectCollector(null) { WmiDataProvider = fakeWmiDataProvider };
        }

        private IEnumerable<WmiObject> CreateFakeWmiReturn()
        {
            var wmiObject = new WmiObject();
            wmiObject.Add("name", "cpaiva");
            wmiObject.Add("domain", "mss");
            var wmiObject2 = new WmiObject();
            wmiObject2.Add("name", "jcastro");
            wmiObject2.Add("domain", "local");

            return new WmiObject[] { wmiObject, wmiObject2 };

        }
    }
}
