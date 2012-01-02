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
using System.Management;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Windows.WMI;

namespace Modulo.Collect.Probe.Windows.GroupSID
{
    public class GroupSIDObjectCollector: BaseObjectCollector
    {
        public TargetInfo TargetInfo { get; set; }
        
        public WmiDataProvider WmiDataProvider { get; set; }
        
        public WindowsGroupAccountProvider WindowsGroupProvider { get; set; }

        
        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            this.CheckWmiDataProviderInstance();
            var wqlAllGroupSIDs = new WQLBuilder().WithWmiClass("Win32_Group").Build();
            var wmiResult = this.WmiDataProvider.ExecuteWQL(wqlAllGroupSIDs);

            IList<String> allGroupSIDs = new List<String>();
            foreach (var userSID in wmiResult)
                allGroupSIDs.Add(userSID.GetValueOf("SID").ToString());

            return allGroupSIDs;
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            this.CheckGroupProviderInstance();
            var groupSIDToCollect = string.Empty;
            try
            {
                groupSIDToCollect = ((group_sid_item)systemItem).group_sid.Value;
                var windowsGroupAccount = this.WindowsGroupProvider.CollectWindowsGroupAccountInfoBySID(groupSIDToCollect);

                this.FillItemTypeWithData((group_sid_item)systemItem, windowsGroupAccount);
            }
            catch (GroupSIDNotFoundException)
            {
                SetDoesNotExistStatusForItemType(systemItem, groupSIDToCollect);
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private void CheckWmiDataProviderInstance()
        {
            if (this.WmiDataProvider == null)
            {
                var managementPath = string.Format(@"\\{0}\root\cimv2", this.TargetInfo.GetAddress());
                this.WmiDataProvider = new WmiDataProvider(new ManagementScope(managementPath));
            }
        }

        private void CheckGroupProviderInstance()
        {
            this.CheckWmiDataProviderInstance();
            if (this.WindowsGroupProvider == null)
                this.WindowsGroupProvider = new WindowsGroupAccountProvider(this.WmiDataProvider, this.TargetInfo);
        }

        private void FillItemTypeWithData(group_sid_item groupSIDItemToFill, object collectedData)
        {
            var allUserSIDs = ((WindowsGroupAccount)collectedData).UserSIDs;

            groupSIDItemToFill.user_sid = new EntityItemStringType[allUserSIDs.Count];
            for (int i = 0; i < allUserSIDs.Count; i++)
                groupSIDItemToFill.user_sid[i] = OvalHelper.CreateItemEntityWithStringValue(allUserSIDs[i]);
        }
    }
}
