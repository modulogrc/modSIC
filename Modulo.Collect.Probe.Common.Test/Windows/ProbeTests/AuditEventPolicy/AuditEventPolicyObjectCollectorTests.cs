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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.AuditEventPolicy;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Windows.Test.AuditEventPolicy
{
    
    [TestClass]
    public class AuditEventPolicyObjectCollectorTests
    {
        private const string UNEXPECTED_NULL_ENTITY_ITEM_WAS_FOUND = "The '{0}' entity cannot be null.";
        private const string TEST_EXCEPTION_MESSAGE = "Test Exception was thrown";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_auditeventpolicy_object()
        {
            var fakeAuditEventPolicies = this.GetFakeAuditEventPolicies();
            var systemDataSource = this.CreateMockedSystemDataSource(fakeAuditEventPolicies);
            var newAuditEventPolicyItem = this.createAuditEventPolicyWithAllEntities();
            

            var collectedItems = systemDataSource.CollectDataForSystemItem(newAuditEventPolicyItem);


            this.DoBasicAssert(collectedItems, 1);
            var collectedAuditEventPolicyItemType = (auditeventpolicy_item)collectedItems.Single().ItemType;
            this.AssertAllAuditEventPolicyEntitiesAreNotNull(collectedAuditEventPolicyItemType);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.account_logon, AuditEventStatus.AUDIT_FAILURE);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.account_management, AuditEventStatus.AUDIT_NONE);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.detailed_tracking, AuditEventStatus.AUDIT_SUCCESS);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.directory_service_access, AuditEventStatus.AUDIT_SUCCESS_FAILURE);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.logon, AuditEventStatus.AUDIT_SUCCESS_FAILURE);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.object_access, AuditEventStatus.AUDIT_SUCCESS);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.policy_change, AuditEventStatus.AUDIT_NONE);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.privilege_use, AuditEventStatus.AUDIT_FAILURE);
            this.AssertAuditEventResultForItem(collectedAuditEventPolicyItemType.system, AuditEventStatus.EMPTY);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_return_collected_items_with_error_status()
        {
            var auditEventItemWithError = this.createAuditEventPolicyWithAllEntities();
            auditEventItemWithError.status = StatusEnumeration.error;
            var systemDataSource = this.CreateMockedSystemDataSource(this.GetFakeAuditEventPolicies());

            var collectedItems = systemDataSource.CollectDataForSystemItem(auditEventItemWithError);

            this.DoBasicAssert(collectedItems, 1);
            Assert.AreEqual(StatusEnumeration.error, collectedItems.Single().ItemType.status);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_return_item_with_error_status_if_some_error_occurs_when_trying_to_collect_audit_event_policies()
        {
            var fakeAuditEventPolicyItem = this.createAuditEventPolicyWithAllEntities();
            var systemDataSource = this.CreateSystemDataSourceWithExceptionThrowingExpectation();

            var collectedItems = systemDataSource.CollectDataForSystemItem(fakeAuditEventPolicyItem);

            this.DoBasicAssert(collectedItems, 1);
            Assert.AreEqual(StatusEnumeration.error, collectedItems.Single().ItemType.status);
            var messageType = collectedItems.Single().ItemType.message; 
            Assert.IsNotNull(messageType);
            Assert.IsTrue(messageType.First().Value.Contains("AuditEventPolicyObjectCollector"));
        }
       
    

        private Dictionary<AuditEventPolicies, AuditEventStatus> GetFakeAuditEventPolicies()
        {
            var fakeAuditEventPolicies = new Dictionary<AuditEventPolicies, AuditEventStatus>();
            fakeAuditEventPolicies.Add(AuditEventPolicies.account_logon, AuditEventStatus.AUDIT_FAILURE);
            fakeAuditEventPolicies.Add(AuditEventPolicies.account_management, AuditEventStatus.AUDIT_NONE);
            fakeAuditEventPolicies.Add(AuditEventPolicies.detailed_tracking, AuditEventStatus.AUDIT_SUCCESS);
            fakeAuditEventPolicies.Add(AuditEventPolicies.directory_service_access, AuditEventStatus.AUDIT_SUCCESS_FAILURE);
            fakeAuditEventPolicies.Add(AuditEventPolicies.logon, AuditEventStatus.AUDIT_SUCCESS_FAILURE);
            fakeAuditEventPolicies.Add(AuditEventPolicies.object_access, AuditEventStatus.AUDIT_SUCCESS);
            fakeAuditEventPolicies.Add(AuditEventPolicies.policy_change, AuditEventStatus.AUDIT_NONE);
            fakeAuditEventPolicies.Add(AuditEventPolicies.privilege_use, AuditEventStatus.AUDIT_FAILURE);
            fakeAuditEventPolicies.Add(AuditEventPolicies.system, AuditEventStatus.EMPTY);
            
            return fakeAuditEventPolicies;
        }

        private AuditEventPolicyObjectCollector CreateMockedSystemDataSource(
            Dictionary<AuditEventPolicies, AuditEventStatus> fakeAuditEventPoliciesToReturn)
        {
            MockRepository mocks = new MockRepository();
            var fakeAuditEventPolicyHelper = mocks.DynamicMock<AuditEventPolicyHelper>(ProbeHelper.CreateFakeTarget());
            Expect.Call(fakeAuditEventPolicyHelper.GetAuditEventPolicies(null)).IgnoreArguments().Return(fakeAuditEventPoliciesToReturn);
            mocks.ReplayAll();

            return new AuditEventPolicyObjectCollector() { AuditEventPolicyHelper = fakeAuditEventPolicyHelper };
        }

        private AuditEventPolicyObjectCollector CreateSystemDataSourceWithExceptionThrowingExpectation()
        {
            MockRepository mocks = new MockRepository();
            var fakeAuditEventPolicyHelper = mocks.DynamicMock<AuditEventPolicyHelper>(ProbeHelper.CreateFakeTarget());
            Expect.Call(fakeAuditEventPolicyHelper.GetAuditEventPolicies(null)).IgnoreArguments().Throw(new Exception(TEST_EXCEPTION_MESSAGE));
            mocks.ReplayAll();

            return new AuditEventPolicyObjectCollector() { AuditEventPolicyHelper = fakeAuditEventPolicyHelper };
        }

        private void AssertAuditEventResultForItem(EntityItemAuditType auditEventEntity, AuditEventStatus expectedAuditEventStatus)
        {
            Assert.AreEqual(expectedAuditEventStatus.ToString(), auditEventEntity.Value, "Unexpected audit event item was found");
        }

        private void AssertAllAuditEventPolicyEntitiesAreNotNull(auditeventpolicy_item collectedItem)
        {
            Assert.IsNotNull(collectedItem.account_logon, GetAssertFailedMessageForNullEntity("account_logon"));
            Assert.IsNotNull(collectedItem.account_management, GetAssertFailedMessageForNullEntity("account_management"));
            Assert.IsNotNull(collectedItem.detailed_tracking, GetAssertFailedMessageForNullEntity("detailed_tracking"));
            Assert.IsNotNull(collectedItem.directory_service_access, GetAssertFailedMessageForNullEntity("directory_service_access"));
            Assert.IsNotNull(collectedItem.logon, GetAssertFailedMessageForNullEntity("logon"));
            Assert.IsNotNull(collectedItem.object_access, GetAssertFailedMessageForNullEntity("object_access"));
            Assert.IsNotNull(collectedItem.policy_change, GetAssertFailedMessageForNullEntity("policy_change"));
            Assert.IsNotNull(collectedItem.privilege_use, GetAssertFailedMessageForNullEntity("privilege_use"));
            Assert.IsNotNull(collectedItem.system, GetAssertFailedMessageForNullEntity("system"));
        }

        private string GetAssertFailedMessageForNullEntity(string entityName)
        {
            return string.Format(UNEXPECTED_NULL_ENTITY_ITEM_WAS_FOUND, entityName);
        }

        private void DoBasicAssert(IEnumerable<CollectedItem> collectedItems, int expectedItemCount)
        {
            Assert.IsNotNull(collectedItems, "The collected items cannot be nuul.");
            Assert.AreEqual(expectedItemCount, collectedItems.Count(), "Unexpected item count was found.");
        }

        private auditeventpolicy_item createAuditEventPolicyWithAllEntities()
        {
            var newAuditEventPolicyItem = new auditeventpolicy_item();
            newAuditEventPolicyItem.CreateAllEntityItems();
            return newAuditEventPolicyItem;
        }
    }
}
