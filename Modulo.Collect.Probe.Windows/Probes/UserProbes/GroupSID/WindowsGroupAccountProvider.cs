/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.Helpers;
using Modulo.Collect.Probe.Windows.WMI;

namespace Modulo.Collect.Probe.Windows.GroupSID
{
    public class WindowsGroupAccountProvider
    {
        private WmiDataProvider WmiDataProvider;
        private TargetInfo TargetInfo;

        public WindowsGroupAccountProvider(WmiDataProvider wmiDataProvider, TargetInfo targetInfo) 
        {
            this.WmiDataProvider = wmiDataProvider;
            this.TargetInfo = targetInfo;
        }

        public virtual WindowsGroupAccount CollectWindowsGroupAccountInfoBySID(string groupSID)
        {
            var userSIDs = this.GetUsersBelongToGroupByGroupID(groupSID);
            return new WindowsGroupAccount() { GroupSID = groupSID, UserSIDs = userSIDs };
        }

        private bool IsThereGroupWithThisSID(string groupSID)
        {
            var wqlSearchWindowsGroupBySID = new WQLBuilder().WithWmiClass("Win32_Group").AddParameter("SID", groupSID).Build();
            var wmiResult = this.WmiDataProvider.ExecuteWQL(wqlSearchWindowsGroupBySID);

            return ((wmiResult == null) || (wmiResult.Count() == 0));
        }

        private List<String> GetUsersBelongToGroupByGroupID(string groupSID)
        {
            PrincipalContext principalContext = this.GetPrincipalContext();
            var allUsersSIDOfGroup = AccManUtils.getMembersOfGroupSidSafely(principalContext, groupSID);

            if ((allUsersSIDOfGroup == null) && (allUsersSIDOfGroup.Count == 0))
                throw new GroupSIDNotFoundException(groupSID);

            var result = new List<String>();
            foreach (var userSIDOfGroup in allUsersSIDOfGroup)
                result.Add(userSIDOfGroup);

            return result;
        }

        private PrincipalContext GetPrincipalContext()
        {
            var targetAddress = this.TargetInfo.GetAddress();
            var username = this.TargetInfo.credentials.GetFullyQualifiedUsername();
            var password = this.TargetInfo.credentials.GetPassword();
            
            return AccManUtils.accManConnect(targetAddress, username, password);
        }
    }
}
