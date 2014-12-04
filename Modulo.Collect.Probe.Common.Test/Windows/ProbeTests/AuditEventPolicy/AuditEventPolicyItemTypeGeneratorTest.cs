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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Windows.AuditEventPolicy;

namespace Modulo.Collect.Probe.Windows.Test.AuditEventPolicy
{
    [TestClass]
    public class AuditEventPolicyItemTypeGeneratorTest
    {
        private const string NULL_AUDIT_ITEM_ENTITY_ASSERT_FAIILED_MSG = 
            "An audit event policy item with an null entity was found. Entity: '{0}'";
        private const string INVALID_AUDIT_ITEM_ENTITY_STATUS_ASSERT_FAIILED_MSG =
            "An audit event policy item with invalid status was found. Entity: {0}.";
        private const string INVALID_AUDIT_ITEM_ENTITY_DATATYPE_ASSERT_FAIILED_MSG =
            "An audit event policy item with invalid datatype was found. Entity: {0}.";
        private const string INVALID_AUDIT_ITEM_VALUE_ASSERT_FAIILED_MSG =
            "An audit event policy item with value different of null was found. Entity: {0}.";


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_auditeventpolicy_object()
        {
            var itemTypeGenerator = new AuditEventPolicyItemTypeGenerator();

            var generatedItems = itemTypeGenerator.GetItemsToCollect(null, null);

            Assert.IsNotNull(generatedItems);
            Assert.AreEqual(1, generatedItems.Count());
            Assert.IsNotNull(generatedItems.First()); 
            Assert.IsInstanceOfType(generatedItems.First(), typeof(auditeventpolicy_item));
            this.AssertAllAuditEventPolicyEntities((auditeventpolicy_item)generatedItems.First());
        }


        private void AssertAllAuditEventPolicyEntities(auditeventpolicy_item generatedItem)
        {
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.account_logon, "account_logon");
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.account_management, "account_management");
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.detailed_tracking, "detailed_tracking");
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.directory_service_access, "directory_service_access");
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.logon, "logon");
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.object_access, "object_access");
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.policy_change, "policy_change");
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.privilege_use, "privilege_use");
            this.AssertGeneratedAuditEventPolicyItem(generatedItem.system, "system");
        }

        private void AssertGeneratedAuditEventPolicyItem(EntityItemAuditType auditEventPolicyItemEntity, string entityName)
        {
            Assert.IsNotNull(auditEventPolicyItemEntity, string.Format(NULL_AUDIT_ITEM_ENTITY_ASSERT_FAIILED_MSG, entityName));
            Assert.AreEqual(auditEventPolicyItemEntity.status, StatusEnumeration.exists, string.Format(INVALID_AUDIT_ITEM_ENTITY_STATUS_ASSERT_FAIILED_MSG, entityName));
            Assert.AreEqual(auditEventPolicyItemEntity.datatype, SimpleDatatypeEnumeration.@string, string.Format(INVALID_AUDIT_ITEM_ENTITY_DATATYPE_ASSERT_FAIILED_MSG, entityName));
            Assert.IsNull(auditEventPolicyItemEntity.Value, string.Format(INVALID_AUDIT_ITEM_VALUE_ASSERT_FAIILED_MSG, entityName));
        }
    }
}
