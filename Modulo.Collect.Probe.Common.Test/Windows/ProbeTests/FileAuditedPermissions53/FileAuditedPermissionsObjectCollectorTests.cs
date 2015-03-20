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
using Modulo.Collect.Probe.Windows.FileAuditedPermissions53;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.File.Helpers;
using Modulo.Collect.Probe.Windows.AuditEventPolicy;
using Modulo.Collect.OVAL.Definitions;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.FileAuditedPermissions53
{
    [TestClass]
    public class FileAuditedPermissionsObjectCollectorTests
    {
        private const string MINIMUM_LOG_ITEMS_EXPECTED_MESSAGE = "There is no log items in execution log.";
        private const string FAKE_FILEPATH = "c:\\temp\\file1.txt";
        private const string FAKE_SID = "S-1-0";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_map_AceFlags_to_AuditEventStatusEnum()
        {
            var noneAuditPolicy = new WMIWinACE { AceFlags = 0 };
            var successAuditPolicy = new WMIWinACE() { AceFlags = 64 };
            var failureAuditPolicy = new WMIWinACE() { AceFlags = 128 };
            var allAuditPolicy = new WMIWinACE() { AceFlags = 192 };

            Assert.AreEqual(AuditEventStatus.AUDIT_NONE, noneAuditPolicy.AuditEventPolicy);
            Assert.AreEqual(AuditEventStatus.AUDIT_SUCCESS, successAuditPolicy.AuditEventPolicy);
            Assert.AreEqual(AuditEventStatus.AUDIT_FAILURE, failureAuditPolicy.AuditEventPolicy);
            Assert.AreEqual(AuditEventStatus.AUDIT_SUCCESS_FAILURE, allAuditPolicy.AuditEventPolicy);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_AceFlags_is_not_set_the_audit_event_status_must_be_equals_to_EMPTY()
        {
            var WinACE = new WMIWinACE();

            Assert.AreEqual(AuditEventStatus.EMPTY, WinACE.AuditEventPolicy);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_fileauditedpermissions_item()
        {
            var itemToCollect = this.CreateFakeFileAuditedPermissionsItem();
            var fakeWinACEsToReturn = this.CreateFakeWMIWinACEs();
            var systemDataSource = this.CreateMockedFileAuditedPermissionsSystemDataSource(fakeWinACEsToReturn, null);

            var collectedItems = systemDataSource.CollectDataForSystemItem(itemToCollect);

            DoBasicAssert(collectedItems.ToArray(), 1);
            AssertExectionLogs(collectedItems.Single().ExecutionLog);
            AssertItemType(collectedItems.Single().ItemType, StatusEnumeration.exists);
            
            var fileAuditedPermissionsItem = (fileauditedpermissions_item)collectedItems.Single().ItemType;
            AssertItemTypeEntity(fileAuditedPermissionsItem.filepath, FAKE_FILEPATH);
            AssertItemTypeEntity(fileAuditedPermissionsItem.trustee_sid, FAKE_SID);
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_read_attributes, AuditEventStatus.AUDIT_SUCCESS.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.access_system_security, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_append_data, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_delete_child, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_execute, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_read_data, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_read_ea, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_write_attributes, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_write_data, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.file_write_ea, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.generic_all, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.generic_execute, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.generic_read, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.generic_write, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.standard_delete, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.standard_read_control, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.standard_synchronize, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.standard_write_dac, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileAuditedPermissionsItem.standard_write_owner, AuditEventStatus.AUDIT_NONE.ToString());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_deal_with_not_found_files()
        {
            var exceptionToThrow = new InvalidInvokeMethodException("", "GetSecurityDescriptor");
            var fakeItemToCollect = CreateFakeFileAuditedPermissionsItem();
            var systemDataSource = CreateMockedFileAuditedPermissionsSystemDataSource(null, exceptionToThrow);

            var collectedItems = systemDataSource.CollectDataForSystemItem(fakeItemToCollect);

            DoBasicAssert(collectedItems.ToArray(), 1);
            AssertExectionLogs(collectedItems.Single().ExecutionLog);
            AssertItemType(collectedItems.Single().ItemType, StatusEnumeration.doesnotexist);
            var fileAuditedPermissionsItem = (fileauditedpermissions_item)collectedItems.Single().ItemType;
            Assert.IsNull(fileAuditedPermissionsItem.access_system_security);

        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_deal_with_not_found_ACL()
        {
            var fakeItemToCollect = CreateFakeFileAuditedPermissionsItem();
            var systemDataSource = CreateMockedSystemDataSourceWithACLNotFoundBehavior(new ACLNotFoundException(SecurityDescriptorType.SACL));
            var collectedItems = systemDataSource.CollectDataForSystemItem(fakeItemToCollect);

            DoBasicAssert(collectedItems.ToArray(), 1);
            AssertExectionLogs(collectedItems.Single().ExecutionLog);
            AssertItemType(collectedItems.Single().ItemType, StatusEnumeration.doesnotexist);
            var fileauditedpermissionsItem = (fileauditedpermissions_item)collectedItems.Single().ItemType;
            AssertItemTypeEntity(fileauditedpermissionsItem.access_system_security, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_append_data, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_delete_child, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_execute, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_read_attributes, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_read_data, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_read_ea, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_write_attributes, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_write_data, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_write_ea, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.generic_all, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.generic_execute, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.generic_read, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.generic_write, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_delete, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_read_control, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_synchronize, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_write_dac, AuditEventStatus.AUDIT_NONE.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_write_owner, AuditEventStatus.AUDIT_NONE.ToString());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_deal_with_unexpected_exception()
        {
            var exceptionToThrow = new Exception("Test Exception");
            var fakeItemToCollect = CreateFakeFileAuditedPermissionsItem();
            var systemDataSource = CreateMockedFileAuditedPermissionsSystemDataSource(null, exceptionToThrow);

            var collectedItems = systemDataSource.CollectDataForSystemItem(fakeItemToCollect);

            DoBasicAssert(collectedItems.ToArray(), 1);
            AssertExectionLogs(collectedItems.Single().ExecutionLog);
            AssertItemType(collectedItems.Single().ItemType, StatusEnumeration.error);
            AssertMessageType(collectedItems.Single().ItemType.message.First(), exceptionToThrow.Message);
            
            var fileauditedpermissionsItem = (fileauditedpermissions_item)collectedItems.Single().ItemType;
            #region Audit Entity Items Asserts
            AssertItemTypeEntity(fileauditedpermissionsItem.access_system_security, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_append_data, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_delete_child, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_execute, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_read_attributes, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_read_data, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_read_ea, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_write_attributes, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_write_data, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.file_write_ea, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.generic_all, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.generic_execute, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.generic_read, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.generic_write, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_delete, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_read_control, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_synchronize, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_write_dac, AuditEventStatus.EMPTY.ToString());
            AssertItemTypeEntity(fileauditedpermissionsItem.standard_write_owner, AuditEventStatus.EMPTY.ToString());
            #endregion

        }


        private void DoBasicAssert(CollectedItem[] collectedItems, int expectedCollectedItemsCount)
        {
            Assert.IsNotNull(collectedItems, "There is no collected items.");
            Assert.AreEqual(expectedCollectedItemsCount, collectedItems.Count(), "Unexpected collected items count was found.");
        }

        private void AssertExectionLogs(IEnumerable<ProbeLogItem> logItems)
        {
            Assert.IsNotNull(logItems, "There is no ProbeLogItem instance in collected item");
            Assert.IsTrue(logItems.Count() > 0, "The log items count should be greater than zero.");
        }

        private void AssertItemType(ItemType collectedItem, StatusEnumeration expectedItemStatus)
        {
            Assert.IsNotNull(collectedItem, "The collected item type cannot be null.");
            Assert.IsInstanceOfType(collectedItem, typeof(fileauditedpermissions_item), "Unexpected item type instance type was found.");
            Assert.AreEqual(expectedItemStatus, collectedItem.status, "Unexpected item type status was found.");
        }

        private void AssertMessageType(MessageType messageTypeToAssert, string expectedChunkOfMessage)
        {
            Assert.IsNotNull(messageTypeToAssert, "There is no message type instance.");
            Assert.IsFalse(string.IsNullOrEmpty(messageTypeToAssert.Value), "There is no message type value.");
            Assert.IsTrue(messageTypeToAssert.Value.Contains("FileAuditedPermissionsObjectCollector"), "The expected chunk of text was not found in message type value.");
        }

        private IEnumerable<WMIWinACE> CreateFakeWMIWinACEs()
        {
            return new WMIWinACE[] { new WMIWinACE() { AccessMask = 128, AceFlags = 64 } };
        }

        private BaseObjectCollector CreateMockedFileAuditedPermissionsSystemDataSource(
            IEnumerable<WMIWinACE> fakeWMIWinACEs, Exception exceptionToThrow)
        {
            MockRepository mocks = new MockRepository();
            var fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            var fakeACLDisassembler = mocks.DynamicMock<WindowsSecurityDescriptorDisassembler>();

            if (exceptionToThrow != null)
                Expect.Call(fakeWmiDataProvider.InvokeMethodByWmiPath(null)).IgnoreArguments().Throw(exceptionToThrow);

            if (fakeWMIWinACEs != null)
                Expect.Call(fakeACLDisassembler.GetSecurityDescriptorsFromManagementObject(null, null, null)).IgnoreArguments()
                    .Return(fakeWMIWinACEs);

            mocks.ReplayAll();

            return new FileAuditedPermissionsObjectCollector
            { 
                WmiDataProvider = fakeWmiDataProvider,
                WindowsSecurityDescriptorDisassembler = fakeACLDisassembler
            };
        }

        private FileAuditedPermissionsObjectCollector CreateMockedSystemDataSourceWithACLNotFoundBehavior(ACLNotFoundException aclExcp)
        {
            MockRepository mocks = new MockRepository();
            var fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            var fakeACLDisassembler = mocks.DynamicMock<WindowsSecurityDescriptorDisassembler>();
            Expect.Call(fakeACLDisassembler.GetSecurityDescriptorsFromManagementObject(null, null, null)).IgnoreArguments().Throw(aclExcp);
            mocks.ReplayAll();

            return new FileAuditedPermissionsObjectCollector()
            {
                WmiDataProvider = fakeWmiDataProvider,
                WindowsSecurityDescriptorDisassembler = fakeACLDisassembler
            };
        }

        private void AssertItemTypeEntity(EntityItemSimpleBaseType entityToAssert, string expectedEntityVale)
        {
            Assert.IsNotNull(entityToAssert);
            Assert.IsFalse(string.IsNullOrEmpty(entityToAssert.Value));
            Assert.AreEqual(expectedEntityVale, entityToAssert.Value);
        }

        private fileauditedpermissions_item CreateFakeFileAuditedPermissionsItem()
        {
            return new fileauditedpermissions_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(FAKE_FILEPATH),
                trustee_sid = OvalHelper.CreateItemEntityWithStringValue(FAKE_SID)
            };
        }

    }
}
