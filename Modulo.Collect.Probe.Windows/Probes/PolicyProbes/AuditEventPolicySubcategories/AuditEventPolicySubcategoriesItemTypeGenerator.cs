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
using System.Collections.Generic;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows.AuditEventPolicySubcategories
{
    public class AuditEventPolicySubcategoriesItemTypeGenerator: IItemTypeGenerator
    {
        public IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            return new auditeventpolicysubcategories_item[] { CreateAuditEventPolicySubcategoriesItemType() };
        }

        public static auditeventpolicysubcategories_item CreateAuditEventPolicySubcategoriesItemType()
        {
            return new auditeventpolicysubcategories_item()
            {
                #region SubCategories
                account_lockout = new EntityItemAuditType(),
                application_generated = new EntityItemAuditType(),
                application_group_management = new EntityItemAuditType(),
                audit_policy_change = new EntityItemAuditType(),
                authentication_policy_change = new EntityItemAuditType(),
                authorization_policy_change = new EntityItemAuditType(),
                certification_services = new EntityItemAuditType(),
                computer_account_management = new EntityItemAuditType(),
                credential_validation = new EntityItemAuditType(),
                detailed_directory_service_replication = new EntityItemAuditType(),
                directory_service_access = new EntityItemAuditType(),
                directory_service_changes = new EntityItemAuditType(),
                directory_service_replication = new EntityItemAuditType(),
                distribution_group_management = new EntityItemAuditType(),
                dpapi_activity = new EntityItemAuditType(),
                file_share = new EntityItemAuditType(),
                file_system = new EntityItemAuditType(),
                filtering_platform_connection = new EntityItemAuditType(),
                filtering_platform_packet_drop = new EntityItemAuditType(),
                filtering_platform_policy_change = new EntityItemAuditType(),
                handle_manipulation = new EntityItemAuditType(),
                ipsec_driver = new EntityItemAuditType(),
                ipsec_extended_mode = new EntityItemAuditType(),
                ipsec_main_mode = new EntityItemAuditType(),
                ipsec_quick_mode = new EntityItemAuditType(),
                kerberos_ticket_events = new EntityItemAuditType(),
                kernel_object = new EntityItemAuditType(),
                logoff = new EntityItemAuditType(),
                logon = new EntityItemAuditType(),
                mpssvc_rule_level_policy_change = new EntityItemAuditType(),
                non_sensitive_privilege_use = new EntityItemAuditType(),
                other_account_logon_events = new EntityItemAuditType(),
                other_account_management_events = new EntityItemAuditType(),
                other_logon_logoff_events = new EntityItemAuditType(),
                other_object_access_events = new EntityItemAuditType(),
                other_policy_change_events = new EntityItemAuditType(),
                other_privilege_use_events = new EntityItemAuditType(),
                other_system_events = new EntityItemAuditType(),
                process_creation = new EntityItemAuditType(),
                process_termination = new EntityItemAuditType(),
                registry = new EntityItemAuditType(),
                rpc_events = new EntityItemAuditType(),
                sam = new EntityItemAuditType(),
                security_group_management = new EntityItemAuditType(),
                security_state_change = new EntityItemAuditType(),
                security_system_extension = new EntityItemAuditType(),
                sensitive_privilege_use = new EntityItemAuditType(),
                special_logon = new EntityItemAuditType(),
                system_integrity = new EntityItemAuditType(),
                user_account_management = new EntityItemAuditType()
                #endregion
            };
        }
    }
}
