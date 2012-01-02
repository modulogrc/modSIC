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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Windows.AuditEventPolicy;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Windows.AuditEventPolicySubcategories
{
    

    public class AuditEventPolicySubcategoriesObjectCollector: BaseObjectCollector
    {
        public TargetInfo TargetInfo { get; set; }

        public AuditEventPolicyHelper AuditEventPolicyHelper { get; set; }

        public AuditEventPolicySubcategoriesObjectCollector()
        {

        }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            if (this.AuditEventPolicyHelper == null)
                this.AuditEventPolicyHelper = new AuditEventPolicyHelper(this.TargetInfo);

            var auditEventPolicySubcategories = this.AuditEventPolicyHelper.GetAuditEventSubcategoriesPolicy(this.TargetInfo);

            this.MapAuditEventSubcategoriesDictionaryToItemType(auditEventPolicySubcategories, (auditeventpolicysubcategories_item)systemItem);

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }


        private void MapAuditEventSubcategoriesDictionaryToItemType(
            Dictionary<AuditEventSubcategories, AuditEventStatus> auditEventPolicySubcategories, auditeventpolicysubcategories_item systemItem)
        {
            foreach (var entry in auditEventPolicySubcategories)
            {
                var field = systemItem.GetType().GetProperty(entry.Key.ToString());
                var fieldValue = field.GetValue(systemItem, null);
                var childField = fieldValue.GetType().GetProperty("Value");
                childField.SetValue(fieldValue, entry.Value.ToString(), null);
            }            
        }

        
    }
}
