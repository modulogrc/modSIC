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
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Windows.AuditEventPolicy
{
    public class AuditEventPolicyItemTypeBuilder: ItemTypeBuilderBase
    {
        public AuditEventPolicyItemTypeBuilder(ItemType newItemType): base(newItemType) { }

        public override void FillItemTypeWithData(object collectedData)
        {
            auditeventpolicy_item buildingItem = (auditeventpolicy_item)base.BuildingItemType;
            var systemData = (Dictionary<AuditEventPolicies, AuditEventStatus>)collectedData;

            buildingItem.account_logon.Value = systemData[AuditEventPolicies.account_logon].ToString();
            buildingItem.account_management.Value = systemData[AuditEventPolicies.account_management].ToString();
            buildingItem.detailed_tracking.Value = systemData[AuditEventPolicies.detailed_tracking].ToString();
            buildingItem.directory_service_access.Value = systemData[AuditEventPolicies.directory_service_access].ToString();
            buildingItem.logon.Value = systemData[AuditEventPolicies.logon].ToString();
            buildingItem.object_access.Value = systemData[AuditEventPolicies.object_access].ToString();
            buildingItem.policy_change.Value = systemData[AuditEventPolicies.policy_change].ToString();
            buildingItem.privilege_use.Value = systemData[AuditEventPolicies.privilege_use].ToString();
            buildingItem.system.Value = systemData[AuditEventPolicies.system].ToString();
        }

        private void SetEntityWithValue(EntityItemAuditType auditEventEntity, AuditEventStatus auditEventPolicyValue)
        {
            auditEventEntity.Value = auditEventPolicyValue.ToString();
        }
    }
}
