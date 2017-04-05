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
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Windows.PasswordPolicy
{
    public class PasswordPolicyObjectCollector : BaseObjectCollector
    {
        public String TargetHostName { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            base.ExecutionLogBuilder.CollectingDataFrom(TargetHostName);
            var collectedPasswordPolicies = PasswordPolicyHelper.getUserModalsInfo0(TargetHostName);
            this.MapPasswrodPolicyToPasswordPolicyItemType((passwordpolicy_item)systemItem, collectedPasswordPolicies);

            PasswordPolicySamServer.DomainPasswordInformation? domainPasswordInfo;
            try
            {
                domainPasswordInfo = PasswordPolicySamServer.GetDomainPasswordInformation(TargetHostName);
            }
            catch
            {
                domainPasswordInfo = null;
            }

            MapDomainPasswrodInformationToPasswordPolicyItemType((passwordpolicy_item)systemItem, domainPasswordInfo);

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, base.BuildExecutionLog());
        }

        private void MapPasswrodPolicyToPasswordPolicyItemType(passwordpolicy_item passwordItemType, PasswordPolicyHelper.USER_MODALS_INFO_0 passwordPolicies)
        {
            var maxPasswordAge = passwordPolicies.usrmod0_max_passwd_age.ToString();
            var minPasswordAge = passwordPolicies.usrmod0_min_passwd_age.ToString();
            var minPasswordLength = passwordPolicies.usrmod0_min_passwd_len.ToString();
            var passwordHistoryLength = passwordPolicies.usrmod0_password_hist_len.ToString();

            passwordItemType.max_passwd_age = OvalHelper.CreateItemEntityWithIntegerValue(maxPasswordAge);
            passwordItemType.min_passwd_age = OvalHelper.CreateItemEntityWithIntegerValue(minPasswordAge);
            passwordItemType.min_passwd_len = OvalHelper.CreateItemEntityWithIntegerValue(minPasswordLength);
            passwordItemType.password_hist_len = OvalHelper.CreateItemEntityWithIntegerValue(passwordHistoryLength);
        }

        private void MapDomainPasswrodInformationToPasswordPolicyItemType(passwordpolicy_item passwordItemType, PasswordPolicySamServer.DomainPasswordInformation? passwordInfo)
        {
            passwordItemType.password_complexity = OvalHelper.CreateBooleanEntityItemFromBoolValue(passwordInfo.HasValue? (bool?) passwordInfo.Value.PasswordComplex: null);
            passwordItemType.reversible_encryption = OvalHelper.CreateBooleanEntityItemFromBoolValue(passwordInfo.HasValue ? (bool?) passwordInfo.Value.PasswordReversibleEncryption : null);
        }
    }
}
