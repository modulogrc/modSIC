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
using Modulo.Collect.Probe.Windows.SID;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass]
    public class SIDObjectCollectorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_all_SIDObjects_from_target()
        {
            // Arrange
            IList<WmiObject> fakeWmiObjects = new List<WmiObject>();
            fakeWmiObjects.Add(this.createFakeWmiObject("MSS", "lfernandes", "FALSE", "Win32_UserAccount"));
            fakeWmiObjects.Add(this.createFakeWmiObject("MSS-RJ-001", "Administrators", "TRUE", "Win32_Group"));
            fakeWmiObjects.Add(this.createFakeWmiObject("MSS-RJ-001", "NETWORK SERVICE", "TRUE", "Win32_SystemAccount"));
            SIDObjectCollector systemDataSource = this.CreateMockedSIDSystemDatasource(fakeWmiObjects);

            // Act
            IList<string> searchSystemObjectResult = systemDataSource.GetValues(null);

            // Assert
            Assert.IsNotNull(searchSystemObjectResult, "The result of get system objects cannot be null.");
            Assert.AreEqual(3, searchSystemObjectResult.Count, "Unexpected system objects count.");
            Assert.AreEqual(@"MSS\lfernandes", searchSystemObjectResult[0], "Unexpected account caption found.");
            Assert.AreEqual(@"MSS-RJ-001\Administrators", searchSystemObjectResult[1], "Unexpected account caption found.");
            Assert.AreEqual(@"MSS-RJ-001\NETWORK SERVICE", searchSystemObjectResult[2], "Unexpected account caption found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_SIDObject_for_given_trusteeName()
        {
            IList<WmiObject> fakeWmiObjects = new List<WmiObject>();
            fakeWmiObjects.Add(this.createFakeWmiObject("MSS", "lfernandes", "FALSE", "Win32_UserAccount"));
            SIDObjectCollector systemDataSource = this.CreateMockedSIDSystemDatasource(fakeWmiObjects);
            string expectedSID = fakeWmiObjects[0].GetValueOf("SID").ToString();

            IEnumerable<CollectedItem> collectedItems = systemDataSource.CollectDataForSystemItem(this.createFakeSidItem());

            Assert.IsNotNull(collectedItems, "The collected items cannot be null.");
            Assert.AreEqual(1, collectedItems.Count(), "Unexpected collected items count.");
            CollectedItem collectedItem = collectedItems.First();

            Assert.IsNotNull(collectedItem.ExecutionLog, "The execution log cannot be null.");
            Assert.AreEqual(1, collectedItem.ExecutionLog.Count(), "Unexpected execution log count.");
            Assert.AreEqual(TypeItemLog.Info, collectedItem.ExecutionLog.First().Type, "The type of execution log should be Info.");

            
            Assert.IsInstanceOfType(collectedItem.ItemType, typeof(sid_item), "The type of collected item type should be 'sid_item'.");
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.ItemType.status, "The status of collected item should be 'exists'.");
            Assert.AreEqual("lfernandes", ((sid_item)collectedItem.ItemType).trustee_name.Value, "Unexpected trustee name was found on collected SID Item.");
            Assert.AreEqual("MSS", ((sid_item)collectedItem.ItemType).trustee_domain.Value, "Unexpected trustee domain was found on collected SID Item.");
            Assert.AreEqual(expectedSID, ((sid_item)collectedItem.ItemType).trustee_sid.Value, "Unexpected trustee SID was found on collected SID Item.");
        }

        [TestMethod, Owner("lfernandes")]
        public void When_system_object_was_not_found_the_collected_item_status_should_be_DoesNotExist()
        {
            // Arrange
            var SidObjectCollector = this.CreateMockedSIDSystemDatasource(new List<WmiObject>());

            // Act
            var collectedItems = SidObjectCollector.CollectDataForSystemItem(this.createFakeSidItem());

            // Assert
            
            Assert.IsNotNull(collectedItems, "The collected items cannot be null.");
            Assert.AreEqual(1, collectedItems.Count(), "Unexpected collected items count.");
            
            var collectedItem = collectedItems.First();
            Assert.IsNotNull(collectedItem.ExecutionLog, "The execution log cannot be null.");
            Assert.AreEqual(2, collectedItem.ExecutionLog.Count(), "Unexpected execution log count.");
            
            Assert.IsInstanceOfType(collectedItem.ItemType, typeof(sid_item), "The type of collected item type should be 'sid_item'.");
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.ItemType.status, "The status of collected item should be 'does not exists'.");
            
            EntityItemStringType trusteeNameEntity = ((sid_item)collectedItem.ItemType).trustee_name;
            Assert.AreEqual(StatusEnumeration.doesnotexist, trusteeNameEntity.status, "Unexpected status for trustee name item entity.");
            Assert.AreEqual(string.Empty, trusteeNameEntity.Value.ToString(), "When a sid object cannot be found, the trustee_name entity item should be empty.");
            Assert.IsNull(((sid_item)collectedItem.ItemType).trustee_domain, "When a sid object cannot be found, the trustee_domain entity item cannot be created.");
            Assert.IsNull(((sid_item)collectedItem.ItemType).trustee_sid, "When a sid object cannot be found, the trustee_sid entity item cannot be created.");
        }


        private void AssertExecutionLogs(IEnumerable<ProbeLogItem> executionLog, TypeItemLog expectedLogType)
        {
        }

        private ItemType createFakeSidItem()
        {
            return new sid_item() { trustee_name = OvalHelper.CreateItemEntityWithStringValue("lfernandes") };
        }

        private SIDObjectCollector CreateMockedSIDSystemDatasource(IEnumerable<WmiObject> fakeWmiObjects)
        {
            MockRepository mocks = new MockRepository();
            var fakeWmiProvider = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeWmiProvider.SearchWmiObjects("Win32_Account", new Dictionary<string, string>())).IgnoreArguments()
                .Return(fakeWmiObjects);
            mocks.ReplayAll();
            return new SIDObjectCollector() { WmiDataProvider = fakeWmiProvider };
        }

        private WmiObject createFakeWmiObject(string accountDomain, string accountName, string isLocalAccount, string wmiClassName)
        {
            WmiObject fakeWmiObject = new WmiObject();
            fakeWmiObject.Add("__CLASS", wmiClassName);
            fakeWmiObject.Add("Description", "Administrator have complete and unrestricted access to the computer/domain");
            fakeWmiObject.Add("Domain", accountDomain);
            fakeWmiObject.Add("Name", accountName);
            fakeWmiObject.Add("InstallDate", null);
            fakeWmiObject.Add("LocalAccount", Boolean.Parse(isLocalAccount));
            fakeWmiObject.Add(@"Caption", string.Format(@"{0}\{1}", accountDomain, accountName));
            fakeWmiObject.Add("SID", Guid.NewGuid().ToString());
            fakeWmiObject.Add("SIDType", (UInt16)4);
            fakeWmiObject.Add("Status", "OK");

            return fakeWmiObject;
        }
    }
}
