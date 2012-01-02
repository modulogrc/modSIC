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

namespace Modulo.Collect.OVAL.SystemCharacteristics
{
    public partial class auditeventpolicy_item : ItemType
    {

        private const string AUDIT_NONE_EVENT_STATUS = "AUDIT_NONE";

        public void CreateAllEntityItems()
        {
            this.account_logon = this.account_logon ?? new EntityItemAuditType();
            this.account_management = this.account_management ?? new EntityItemAuditType();
            this.detailed_tracking = this.detailed_tracking ?? new EntityItemAuditType();
            this.directory_service_access = this.directory_service_access ?? new EntityItemAuditType();
            this.logon = this.logon ?? new EntityItemAuditType();
            this.object_access = this.object_access ?? new EntityItemAuditType();
            this.policy_change = this.policy_change ?? new EntityItemAuditType();
            this.privilege_use = this.privilege_use ?? new EntityItemAuditType();
            this.system = this.system ?? new EntityItemAuditType();
        }

        public void CreateAllEntityItemsWithAuditNoneEventStatus()
        {
            CreateAllEntityItems();
            this.account_logon.Value = AUDIT_NONE_EVENT_STATUS;
            this.account_management.Value = AUDIT_NONE_EVENT_STATUS;
            this.detailed_tracking.Value = AUDIT_NONE_EVENT_STATUS;
            this.directory_service_access.Value = AUDIT_NONE_EVENT_STATUS;
            this.logon.Value = AUDIT_NONE_EVENT_STATUS;
            this.object_access.Value = AUDIT_NONE_EVENT_STATUS;
            this.policy_change.Value = AUDIT_NONE_EVENT_STATUS;
            this.privilege_use.Value = AUDIT_NONE_EVENT_STATUS;
            this.system.Value = AUDIT_NONE_EVENT_STATUS;
        }


    }
}
