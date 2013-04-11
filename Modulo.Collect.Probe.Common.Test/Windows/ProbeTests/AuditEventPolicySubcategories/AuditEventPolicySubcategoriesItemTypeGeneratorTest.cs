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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Windows.AuditEventPolicySubcategories;

namespace Modulo.Collect.Probe.Windows.Test.AuditEventPolicySubcategories
{
    [TestClass]
    public class AuditEventPolicySubcategoriesItemTypeGeneratorTest
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_auditeventsubcategoriespolicy_object()
        {
            var itemTypeGenerator = new AuditEventPolicySubcategoriesItemTypeGenerator();

            var generatedItems = itemTypeGenerator.GetItemsToCollect(null, null);

            Assert.IsNotNull(generatedItems);
            Assert.AreEqual(1, generatedItems.Count());
            var itemToAssert = (auditeventpolicysubcategories_item)generatedItems.Single();
            // All entities must be created.
            Assert.IsNotNull(itemToAssert.account_lockout);
            Assert.IsNotNull(itemToAssert.application_generated);
            Assert.IsNotNull(itemToAssert.application_group_management);
            Assert.IsNotNull(itemToAssert.audit_policy_change);
            Assert.IsNotNull(itemToAssert.authentication_policy_change);
            Assert.IsNotNull(itemToAssert.authorization_policy_change);
            Assert.IsNotNull(itemToAssert.certification_services);
            Assert.IsNotNull(itemToAssert.computer_account_management);
            Assert.IsNotNull(itemToAssert.credential_validation);
            Assert.IsNotNull(itemToAssert.detailed_directory_service_replication);
            Assert.IsNotNull(itemToAssert.directory_service_access);
            Assert.IsNotNull(itemToAssert.directory_service_changes);
            Assert.IsNotNull(itemToAssert.directory_service_replication);
            Assert.IsNotNull(itemToAssert.distribution_group_management);
            Assert.IsNotNull(itemToAssert.dpapi_activity);
            Assert.IsNotNull(itemToAssert.file_share);
            Assert.IsNotNull(itemToAssert.file_system);
            Assert.IsNotNull(itemToAssert.filtering_platform_connection);
            Assert.IsNotNull(itemToAssert.filtering_platform_packet_drop);
            Assert.IsNotNull(itemToAssert.filtering_platform_policy_change);
            Assert.IsNotNull(itemToAssert.handle_manipulation);
            Assert.IsNotNull(itemToAssert.ipsec_driver);
            Assert.IsNotNull(itemToAssert.ipsec_extended_mode);
            Assert.IsNotNull(itemToAssert.ipsec_main_mode);
            Assert.IsNotNull(itemToAssert.ipsec_quick_mode);
            Assert.IsNotNull(itemToAssert.kernel_object);
            Assert.IsNotNull(itemToAssert.logoff);
            Assert.IsNotNull(itemToAssert.logon);
            Assert.IsNotNull(itemToAssert.mpssvc_rule_level_policy_change);
            Assert.IsNotNull(itemToAssert.non_sensitive_privilege_use);
            Assert.IsNotNull(itemToAssert.other_account_logon_events);
            Assert.IsNotNull(itemToAssert.other_account_management_events);
            Assert.IsNotNull(itemToAssert.other_logon_logoff_events);
            Assert.IsNotNull(itemToAssert.other_object_access_events);
            Assert.IsNotNull(itemToAssert.other_policy_change_events);
            Assert.IsNotNull(itemToAssert.other_privilege_use_events);
            Assert.IsNotNull(itemToAssert.other_system_events);
            Assert.IsNotNull(itemToAssert.process_creation);
            Assert.IsNotNull(itemToAssert.process_termination);
            Assert.IsNotNull(itemToAssert.registry);
            Assert.IsNotNull(itemToAssert.rpc_events);
            Assert.IsNotNull(itemToAssert.sam);
            Assert.IsNotNull(itemToAssert.security_group_management);
            Assert.IsNotNull(itemToAssert.security_state_change);
            Assert.IsNotNull(itemToAssert.security_system_extension);
            Assert.IsNotNull(itemToAssert.sensitive_privilege_use);
            Assert.IsNotNull(itemToAssert.special_logon);
            Assert.IsNotNull(itemToAssert.system_integrity);
            Assert.IsNotNull(itemToAssert.user_account_management);
            Assert.IsNotNull(itemToAssert.kerberos_authentication_service);
            Assert.IsNotNull(itemToAssert.kerberos_service_ticket_operations);
            Assert.IsNotNull(itemToAssert.detailed_file_share);
            Assert.IsNotNull(itemToAssert.network_policy_server);
        }
    }
}
