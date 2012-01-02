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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Windows.WMI;

namespace Modulo.Collect.Probe.Windows.SID_SID
{
    public class SID_SIDObjectCollector: BaseObjectCollector
    {
        public WmiDataProvider WmiDataProvider { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            var users = this.WmiDataProvider.SearchWmiObjects("Win32_Account", null);

            var values = new List<String>();
            foreach (var user in users)
                values.Add(user.GetValueOf("SID").ToString());

            return values;
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            try
            {
                string trusteeSID = ((sid_sid_item)systemItem).trustee_sid.Value;
                base.ExecutionLogBuilder.CollectingDataFrom(trusteeSID);
                this.collectSidItemSystemData((sid_sid_item)systemItem);
            }
            catch (KeyNotFoundException)
            {
                SetTrusteeSIDEntityStatus((sid_sid_item)systemItem, StatusEnumeration.doesnotexist);
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private void SetTrusteeSIDEntityStatus(sid_sid_item sidsidItem, StatusEnumeration status)
        {
            sidsidItem.trustee_sid.status = status;
            if (status.Equals(StatusEnumeration.doesnotexist) || status.Equals(StatusEnumeration.error))
                sidsidItem.trustee_sid.Value = string.Empty;
        }

        private void collectSidItemSystemData(sid_sid_item sidItem)
        {
            string trusteeSID = sidItem.trustee_sid.Value;

            IEnumerable<WmiObject> wmiObjects = this.searchSIDObjectsOnTarget(trusteeSID);
            if ((wmiObjects == null) || (wmiObjects.Count() == 0))
                throw new KeyNotFoundException();

            WmiObject userAccount = wmiObjects.First();
            sidItem.trustee_domain = OvalHelper.CreateItemEntityWithStringValue(userAccount.GetFieldValueAsString("Domain"));
            sidItem.trustee_name = OvalHelper.CreateItemEntityWithStringValue(userAccount.GetValueOf("Name").ToString());
        }

        private IEnumerable<WmiObject> searchSIDObjectsOnTarget(string trusteeSID)
        {
            try
            {
                Dictionary<String, String> parameters = new Dictionary<string, string>();
                parameters.Add("SID", trusteeSID);

                return this.WmiDataProvider.SearchWmiObjects("Win32_Account", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
