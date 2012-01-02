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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Windows.AccessToken
{
    public class AccessTokenObjectCollector: BaseObjectCollector
    {
        private const string ACCESSTOKEN_COLLECT_WARNING_MESSAGE = "Access Tokens object for '{0}' at '{1}' cannot be collected: '{2}'";
        private const string ACCOUNT_DOES_NOT_EXIST_ERROR_MESSAGE = "The account '{0}' cannot be found";
        private const string AN_UNEXPECTED_ERROR_OCCURRED_MSG = "[AccessTokenSystemDataSource] - An unexpected error occured while trying to get access tokens for '{0}': '{1}'";

        public String TargetHostName { get; set; }
        public AccessTokenProvider AccessTokenProvider { get; set; }

        public AccessTokenObjectCollector()
        {
            if (this.AccessTokenProvider == null)
                this.AccessTokenProvider = new AccessTokenProvider();
        }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            base.ExecutionLogBuilder.CollectingDataFrom(TargetHostName);
            var securityPrincipleEntity = ((accesstoken_item)systemItem).security_principle;
            
            var collectedAccessToken = this.AccessTokenProvider.GetAccessTokens(this.TargetHostName, securityPrincipleEntity.Value);
            new AccessTokenItemTypeBuilder().FillItemTypeWithData((accesstoken_item)systemItem, collectedAccessToken);

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private string TryToGetAccountNameFromItem(accesstoken_item accesstokenItem)
        {
            var entityItem = accesstokenItem.security_principle;
            bool isSecurityPrincipalUndefined = ((entityItem == null) || (string.IsNullOrWhiteSpace(entityItem.Value)));
            return isSecurityPrincipalUndefined ? string.Empty : entityItem.Value;
        }
    }
}
