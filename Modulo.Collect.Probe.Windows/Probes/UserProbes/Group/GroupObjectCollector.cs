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
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Windows.Group
{
    public class GroupObjectCollector : BaseObjectCollector
    {
        public WmiDataProvider WmiDataProvider { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            var computerName = this.WmiDataProvider.SearchWmiObjects("Win32_ComputerSystem", null).FirstOrDefault().GetValueOf("Name");
            var groups = this.WmiDataProvider.SearchWmiObjects("Win32_Group", null);

            var values = new List<String>();
            foreach (var group in groups)
            {
                var domainName = group.GetValueOf("Domain").ToString();
                var groupFieldName = computerName.Equals(domainName) ? "Name" : "Caption";
                var groupName = group.GetValueOf(groupFieldName).ToString();
                
                values.Add(groupName);
            }

            return values;
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var groupItem = (group_item)systemItem;
            var groupName = groupItem.group.Value;
            
            try
            {
                base.ExecutionLogBuilder.CollectingDataFrom(groupName);
                this.CollectGroupItemSystemData(groupItem);
            }
            catch (KeyNotFoundException)
            {
                base.SetDoesNotExistStatusForItemType(groupItem, groupName);
                this.SetGroupEntityStatus(groupItem.group, StatusEnumeration.doesnotexist);
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(groupItem, BuildExecutionLog());
        }

        public void SetGroupEntityStatus(EntityItemSimpleBaseType groupNameEntity, StatusEnumeration status)
        {
            groupNameEntity.status = status;
            if ((status == StatusEnumeration.doesnotexist) || (status == StatusEnumeration.error))
                groupNameEntity.Value = string.Empty;
        }

        private void CollectGroupItemSystemData(group_item groupItem)
        {
            string groupName = groupItem.group.Value;
            IEnumerable<string> memberList = this.SearchGroupObjectsOnTarget(groupName);

            List<EntityItemStringType> cookedMemberList = new List<EntityItemStringType>();
            if ((memberList == null) || (memberList.Count() == 0))
            {
                cookedMemberList.Add(OvalHelper.CreateItemEntityWithStringValue("No Member"));
                cookedMemberList[0].status = StatusEnumeration.doesnotexist;
            }
            else
            {
                foreach (string memberName in memberList)
                {
                    cookedMemberList.Add(OvalHelper.CreateItemEntityWithStringValue(memberName));
                }
            }
            groupItem.user = cookedMemberList.ToArray();
        }

        private List<string> SearchGroupObjectsOnTarget(string groupName)
        {
            IEnumerable<WmiObject> wmiObjs;
            var memberList = new List<string>();

            var domGroup = groupName.Split(new char[] { '\\' }, 2);
            var theDomain = string.Empty;
            var theName = string.Empty;

            if (domGroup.GetUpperBound(0) >= 1)
            {
                theDomain = domGroup[0];
                theName = domGroup[1];
            }
            else
            {
                wmiObjs = this.WmiDataProvider.ExecuteWQL("SELECT Name FROM Win32_ComputerSystem");
                var theHost = wmiObjs.First().GetFieldValueAsString("Name");
                theDomain = theHost;
                theName = domGroup[0];
            }

            wmiObjs = this.WmiDataProvider.ExecuteWQL("SELECT PartComponent FROM Win32_GroupUser WHERE GroupComponent = \"Win32_Group.Domain='" + theDomain + "',Name='" + theName + "'\"");
            foreach (WmiObject wmiObj in wmiObjs)
            {
                string thisMember = wmiObj.GetFieldValueAsString("PartComponent");
                memberList.Add(GetWMIPropertyFromString(thisMember, "Domain") + "\\" + GetWMIPropertyFromString(thisMember, "Name"));
            }

            return memberList;
        }

        public string GetWMIPropertyFromString(string path, string propName)
        {
            int crudLeft = propName.Length + 3;
            int whereProp = path.IndexOf("." + propName + "=\"");
            if (whereProp < 0)
                whereProp = path.IndexOf("," + propName + "=\"");
            if (whereProp < 0)
                return "";
            int endProp = path.IndexOf('"', whereProp + crudLeft);
            if (endProp < 0)
                return path.Substring(whereProp + crudLeft);
            else
                return path.Substring(whereProp + crudLeft, endProp - whereProp - crudLeft);
        }
    }
}
