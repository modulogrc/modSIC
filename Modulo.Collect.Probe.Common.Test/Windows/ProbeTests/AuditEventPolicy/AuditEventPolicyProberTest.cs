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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.AuditEventPolicy;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Definitions = Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.Probe.Windows.Test.AuditEventPolicy
{
    [TestClass]
    public class AuditEventPolicyProberTest
    {
        private const string UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND = "The '{0}' entity must be null after item type creation.";
        private const string UNEXPECTED_NULL_ENTITY_WAS_FOUND = "The '{0}' entity must be not null.";

        private List<IConnectionProvider> FakeContext;
        private TargetInfo FakeTargetInfo;

        private CollectedItem[] FakeCollectedItems;
        private ItemType[] FakeItemsToCollect;
        private CollectInfo FakeCollectInfo;


        public AuditEventPolicyProberTest()
        {
            this.FakeContext = ProbeHelper.CreateFakeContext();
            this.FakeTargetInfo = ProbeHelper.CreateFakeTarget();
            this.FakeItemsToCollect = new ItemType[] { new auditeventpolicy_item() };

            var fakeAuditEventPolicyItem = new auditeventpolicy_item();
            fakeAuditEventPolicyItem.CreateAllEntityItemsWithAuditNoneEventStatus();
            fakeAuditEventPolicyItem.detailed_tracking.Value = AuditEventStatus.AUDIT_SUCCESS_FAILURE.ToString();
            this.FakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(fakeAuditEventPolicyItem) };

            var objects = new Definitions.ObjectType[] { ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2100") };
            this.FakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(objects, null, null);
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_auditeventpolicy_object()
        {
            var prober = this.CreateMockedProbe(FakeCollectedItems, FakeItemsToCollect, null);

            var collectingResult = prober.Execute(this.FakeContext, this.FakeTargetInfo, this.FakeCollectInfo);

            Assert.IsNotNull(collectingResult);
            Assert.AreEqual(1, collectingResult.CollectedObjects.Count());
            var systemData = collectingResult.CollectedObjects.Single().SystemData;
            Assert.IsNotNull(systemData);
            Assert.AreEqual(1, systemData.Count);
            Assert.IsInstanceOfType(systemData.Single(), typeof(auditeventpolicy_item));
            Assert.AreEqual(StatusEnumeration.exists, systemData.Single().status);
            Assert.AreEqual(AuditEventStatus.AUDIT_NONE.ToString(), ((auditeventpolicy_item)systemData.Single()).account_logon.Value);
            Assert.AreEqual(AuditEventStatus.AUDIT_SUCCESS_FAILURE.ToString(), ((auditeventpolicy_item)systemData.Single()).detailed_tracking.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_an_error_occurs_while_creating_items_to_collect_the_created_items_must_have_error_status()
        {
            var prober = this.CreateMockedProbe(FakeCollectedItems, null, new Exception("Test Exception"));

            var collectingResult = prober.Execute(this.FakeContext, this.FakeTargetInfo, this.FakeCollectInfo);

            Assert.IsNotNull(collectingResult);
            Assert.AreEqual(1, collectingResult.CollectedObjects.Count());
            var systemData = collectingResult.CollectedObjects.Single().SystemData;
            Assert.IsNotNull(systemData);
            Assert.AreEqual(1, systemData.Count);
            Assert.IsInstanceOfType(systemData.Single(), typeof(auditeventpolicy_item));
            Assert.AreEqual(StatusEnumeration.error, systemData.Single().status);
            Assert.IsNotNull(systemData.Single().message);
            Assert.IsTrue(systemData.Single().message.First().Value.Contains("Test Exception"));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_all_audit_entities_for_an_auditeventpolicy_item()
        {
            var newAuditEventPolicyItem = new auditeventpolicy_item();
            AssertAllEntitiesIsNull(newAuditEventPolicyItem);

            newAuditEventPolicyItem.CreateAllEntityItems();
            AssertAllEntitiesWasCreated(newAuditEventPolicyItem, AuditEventStatus.EMPTY);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_all_audit_entities_with_default_audit_event_status_for_an_auditeventpolicy_item()
        {
            var newAuditEventPolicyItem = new auditeventpolicy_item();
            newAuditEventPolicyItem.CreateAllEntityItemsWithAuditNoneEventStatus();

            AssertAllEntitiesWasCreated(newAuditEventPolicyItem, AuditEventStatus.AUDIT_NONE);
        }


        private ProbeBase CreateMockedProbe(CollectedItem[] fakeCollectedItems, ItemType[] fakeGeneratedItems, Exception exceptionToThrow)
        {
            MockRepository mocks = new MockRepository();
            var fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();
            var fakeSystemDataSource = mocks.DynamicMock<BaseObjectCollector>();
            var fakeItemTypeGenerator = mocks.DynamicMock<IItemTypeGenerator>();

            if (exceptionToThrow == null)
            {
                Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null)).IgnoreArguments().Return(fakeGeneratedItems);
                Expect.Call(fakeSystemDataSource.CollectDataForSystemItem(null)).IgnoreArguments().Return(fakeCollectedItems);
            }
            else
            {
                Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null)).IgnoreArguments().Throw(exceptionToThrow);
                Expect.Call(fakeSystemDataSource.CollectDataForSystemItem(null)).IgnoreArguments().CallOriginalMethod(OriginalCallOptions.NoExpectation);
            }

            mocks.ReplayAll();

            return new AuditEventPolicyProber()
            {
                ConnectionManager = fakeConnectionManager,
                ObjectCollector = fakeSystemDataSource,
                ItemTypeGenerator = fakeItemTypeGenerator
            };
        }

        private void AssertAllEntitiesIsNull(auditeventpolicy_item auditEventItem)
        {
            Assert.IsNull(auditEventItem.account_logon, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "account_logon"));
            Assert.IsNull(auditEventItem.account_management, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "account_management"));
            Assert.IsNull(auditEventItem.detailed_tracking, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "detailed_tracking"));
            Assert.IsNull(auditEventItem.directory_service_access, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "directory_service_access"));
            Assert.IsNull(auditEventItem.logon, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "logon"));
            Assert.IsNull(auditEventItem.object_access, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "object_access"));
            Assert.IsNull(auditEventItem.policy_change, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "policy_change"));
            Assert.IsNull(auditEventItem.privilege_use, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "privilege_use"));
            Assert.IsNull(auditEventItem.system, string.Format(UNEXPECTED_NOTNULL_ENTITY_WAS_FOUND, "system"));
        }

        private void AssertAllEntitiesWasCreated(auditeventpolicy_item auditEventPolicyItem, AuditEventStatus withDefaultValue)
        {
            Assert.IsNotNull(auditEventPolicyItem.account_logon, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "account_logon"));
            Assert.IsNotNull(auditEventPolicyItem.account_management, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "account_management"));
            Assert.IsNotNull(auditEventPolicyItem.detailed_tracking, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "detailed_tracking"));
            Assert.IsNotNull(auditEventPolicyItem.directory_service_access, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "directory_service_access"));
            Assert.IsNotNull(auditEventPolicyItem.logon, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "logon"));
            Assert.IsNotNull(auditEventPolicyItem.object_access, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "object_access"));
            Assert.IsNotNull(auditEventPolicyItem.policy_change, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "policy_change"));
            Assert.IsNotNull(auditEventPolicyItem.privilege_use, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "privilege_use"));
            Assert.IsNotNull(auditEventPolicyItem.system, string.Format(UNEXPECTED_NULL_ENTITY_WAS_FOUND, "system"));

            if (withDefaultValue != AuditEventStatus.EMPTY)
            {
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.account_logon.Value);
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.account_management.Value);
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.detailed_tracking.Value);
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.directory_service_access.Value);
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.logon.Value);
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.object_access.Value);
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.policy_change.Value);
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.privilege_use.Value);
                Assert.AreEqual(withDefaultValue.ToString(), auditEventPolicyItem.system.Value);
            }
        }

    }
}
