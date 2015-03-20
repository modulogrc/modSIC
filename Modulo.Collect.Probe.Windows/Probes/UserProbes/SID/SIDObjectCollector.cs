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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Windows.SID
{
    public class SIDObjectCollector: BaseObjectCollector
    {
        private const string OBJECT_NOT_EXISTS = "The SID object '{0}' was not found on target machine.";
        private const string OBJECT_CANNOT_BE_COLLECTED = "The SID object '{0}' cannot be collected.";

        public WmiDataProvider WmiDataProvider { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            var users = this.WmiDataProvider.SearchWmiObjects("Win32_Account", null);

            var values = new List<String>();
            foreach (var user in users)
                values.Add(user.GetValueOf("Caption").ToString());
            
            return values;
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            string trusteeName = string.Empty;
            try
            {
                trusteeName = ((sid_item)systemItem).trustee_name.Value;
                base.ExecutionLogBuilder.CollectingDataFrom(trusteeName);
                this.CollectSidItemSystemData((sid_item)systemItem);
            }
            catch (KeyNotFoundException)
            {
                base.SetDoesNotExistStatusForItemType(systemItem, trusteeName);
                this.SetTrusteeNameEntityStatus((sid_item)systemItem, StatusEnumeration.doesnotexist);
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private void SetTrusteeNameEntityStatus(sid_item sidItem, StatusEnumeration status)
        {
            if (sidItem.status.Equals(StatusEnumeration.doesnotexist) || sidItem.status.Equals(StatusEnumeration.error))
            {
                sidItem.trustee_name.status = status;
                sidItem.trustee_name.Value = string.Empty;
            }
        }

        private void CollectSidItemSystemData(sid_item sidItem)
        {
            var wmiObjects = this.SearchSIDObjectsOnTarget(sidItem.trustee_name.Value);
            if ((wmiObjects == null) || (wmiObjects.Count() == 0))
                throw new KeyNotFoundException();
            
            var userAccount = wmiObjects.First();
            sidItem.trustee_domain = OvalHelper.CreateItemEntityWithStringValue(userAccount.GetFieldValueAsString("Domain"));
            sidItem.trustee_sid = OvalHelper.CreateItemEntityWithStringValue(userAccount.GetValueOf("SID").ToString());
        }

        private IEnumerable<WmiObject> SearchSIDObjectsOnTarget(string trusteeName)
        {
            var isSystemAccount = (trusteeName.IndexOf("\\") < 0);
            var wmiAccountClass = isSystemAccount ? "Win32_SystemAccount" : "Win32_Account";
            var objPropertyName = isSystemAccount ? "Name" : "Caption";

            var parameters = new Dictionary<string, string>();
            parameters.Add(objPropertyName, trusteeName.Replace(@"\", @"\\"));

            return this.WmiDataProvider.SearchWmiObjects(wmiAccountClass, parameters);
        }


    }
}
