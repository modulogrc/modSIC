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
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Windows.AccessToken
{
    public class AccessTokenItemTypeBuilder
    {
        public void FillItemTypeWithData(accesstoken_item itemTypeToFill, object collectedData)
        {
            var allCollectedAccessTokens = (List<string>)collectedData;

            this.SetExistentAccessTokens(itemTypeToFill, allCollectedAccessTokens);
            this.SetAbsentAccessTokens(itemTypeToFill);
        }

        private void SetAbsentAccessTokens(accesstoken_item accessTokenItemType)
        {
            if (accessTokenItemType.sesecurityprivilege == null)
                accessTokenItemType.sesecurityprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.sebackupprivilege == null)
                accessTokenItemType.sebackupprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.serestoreprivilege == null)
                accessTokenItemType.serestoreprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.sesystemtimeprivilege == null)
                accessTokenItemType.sesystemtimeprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seshutdownprivilege == null)
                accessTokenItemType.seshutdownprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seremoteshutdownprivilege == null)
                accessTokenItemType.seremoteshutdownprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.setakeownershipprivilege == null)
                accessTokenItemType.setakeownershipprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.sedebugprivilege == null)
                accessTokenItemType.sedebugprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.sesystemenvironmentprivilege == null)
                accessTokenItemType.sesystemenvironmentprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.sesystemprofileprivilege == null)
                accessTokenItemType.sesystemprofileprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seprofilesingleprocessprivilege == null)
                accessTokenItemType.seprofilesingleprocessprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seincreasebasepriorityprivilege == null)
                accessTokenItemType.seincreasebasepriorityprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seloaddriverprivilege == null)
                accessTokenItemType.seloaddriverprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.secreatepagefileprivilege == null)
                accessTokenItemType.secreatepagefileprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seincreasequotaprivilege == null)
                accessTokenItemType.seincreasequotaprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.sechangenotifyprivilege == null)
                accessTokenItemType.sechangenotifyprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seundockprivilege == null)
                accessTokenItemType.seundockprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.semanagevolumeprivilege == null)
                accessTokenItemType.semanagevolumeprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seimpersonateprivilege == null)
                accessTokenItemType.seimpersonateprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.secreateglobalprivilege == null)
                accessTokenItemType.secreateglobalprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.setimezoneprivilege == null)
                accessTokenItemType.setimezoneprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.secreatesymboliclinkprivilege == null)
                accessTokenItemType.secreatesymboliclinkprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seinteractivelogonright == null)
                accessTokenItemType.seinteractivelogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.senetworklogonright == null)
                accessTokenItemType.senetworklogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.sebatchlogonright == null)
                accessTokenItemType.sebatchlogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.seremoteinteractivelogonright == null)
                accessTokenItemType.seremoteinteractivelogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.seassignprimarytokenprivilege == null)
                accessTokenItemType.seassignprimarytokenprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seauditprivilege == null)
                accessTokenItemType.seauditprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.secreatepermanentprivilege == null)
                accessTokenItemType.secreatepermanentprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.secreatetokenprivilege == null)
                accessTokenItemType.secreatetokenprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seenabledelegationprivilege == null)
                accessTokenItemType.seenabledelegationprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seincreaseworkingsetprivilege == null)
                accessTokenItemType.seincreaseworkingsetprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.selockmemoryprivilege == null)
                accessTokenItemType.selockmemoryprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.semachineaccountprivilege == null)
                accessTokenItemType.semachineaccountprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.serelabelprivilege == null)
                accessTokenItemType.serelabelprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.sesyncagentprivilege == null)
                accessTokenItemType.sesyncagentprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.setcbprivilege == null)
                accessTokenItemType.setcbprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seunsolicitedinputprivilege == null)
                accessTokenItemType.seunsolicitedinputprivilege = this.CreateFalseEntityItem();

            if (accessTokenItemType.seservicelogonright == null)
                accessTokenItemType.seservicelogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.sedenybatchLogonright == null)
                accessTokenItemType.sedenybatchLogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.sedenyinteractivelogonright == null)
                accessTokenItemType.sedenyinteractivelogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.sedenynetworklogonright == null)
                accessTokenItemType.sedenynetworklogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.sedenyremoteInteractivelogonright == null)
                accessTokenItemType.sedenyremoteInteractivelogonright = this.CreateFalseEntityItem();

            if (accessTokenItemType.sedenyservicelogonright == null)
                accessTokenItemType.sedenyservicelogonright = this.CreateFalseEntityItem();
        }



        private void SetExistentAccessTokens(accesstoken_item accessTokenItemType, List<string> collectedAccessTokens)
        {
            foreach (string accessToken in collectedAccessTokens)
            {
                switch (accessToken)
                {
                    case "SeSecurityPrivilege": accessTokenItemType.sesecurityprivilege = CreateTrueEntityItem(); break;
                    case "SeBackupPrivilege": accessTokenItemType.sebackupprivilege = CreateTrueEntityItem(); break;
                    case "SeRestorePrivilege": accessTokenItemType.serestoreprivilege = CreateTrueEntityItem(); break;
                    case "SeSystemtimePrivilege": accessTokenItemType.sesystemtimeprivilege = CreateTrueEntityItem(); break;
                    case "SeShutdownPrivilege": accessTokenItemType.seshutdownprivilege = CreateTrueEntityItem(); break;
                    case "SeRemoteShutdownPrivilege": accessTokenItemType.seremoteshutdownprivilege = CreateTrueEntityItem(); break;
                    case "SeTakeOwnershipPrivilege": accessTokenItemType.setakeownershipprivilege = CreateTrueEntityItem(); break;
                    case "SeDebugPrivilege": accessTokenItemType.sedebugprivilege = CreateTrueEntityItem(); break;
                    case "SeSystemEnvironmentPrivilege": accessTokenItemType.sesystemenvironmentprivilege = CreateTrueEntityItem(); break;
                    case "SeSystemProfilePrivilege": accessTokenItemType.sesystemprofileprivilege = CreateTrueEntityItem(); break;
                    case "SeProfileSingleProcessPrivilege": accessTokenItemType.seprofilesingleprocessprivilege = CreateTrueEntityItem(); break;
                    case "SeIncreaseBasePriorityPrivilege": accessTokenItemType.seincreasebasepriorityprivilege = CreateTrueEntityItem(); break;
                    case "SeLoadDriverPrivilege": accessTokenItemType.seloaddriverprivilege = CreateTrueEntityItem(); break;
                    case "SeCreatePagefilePrivilege": accessTokenItemType.secreatepagefileprivilege = CreateTrueEntityItem(); break;
                    case "SeIncreaseQuotaPrivilege": accessTokenItemType.seincreasequotaprivilege = CreateTrueEntityItem(); break;
                    case "SeChangeNotifyPrivilege": accessTokenItemType.sechangenotifyprivilege = CreateTrueEntityItem(); break;
                    case "SeUndockPrivilege": accessTokenItemType.seundockprivilege = CreateTrueEntityItem(); break;
                    case "SeManageVolumePrivilege": accessTokenItemType.semanagevolumeprivilege = CreateTrueEntityItem(); break;
                    case "SeImpersonatePrivilege": accessTokenItemType.seimpersonateprivilege = CreateTrueEntityItem(); break;
                    case "SeCreateGlobalPrivilege": accessTokenItemType.secreateglobalprivilege = CreateTrueEntityItem(); break;
                    case "SeTimeZonePrivilege": accessTokenItemType.setimezoneprivilege = CreateTrueEntityItem(); break;
                    case "SeCreateSymbolicLinkPrivilege": accessTokenItemType.secreatesymboliclinkprivilege = CreateTrueEntityItem(); break;
                    case "SeInteractiveLogonRight": accessTokenItemType.seinteractivelogonright = CreateTrueEntityItem(); break;
                    case "SeNetworkLogonRight": accessTokenItemType.senetworklogonright = CreateTrueEntityItem(); break;
                    case "SeBatchLogonRight": accessTokenItemType.sebatchlogonright = CreateTrueEntityItem(); break;
                    case "SeRemoteInteractiveLogonRight": accessTokenItemType.seremoteinteractivelogonright = CreateTrueEntityItem(); break;
                    case "SeAssignPrimaryTokenPrivilege": accessTokenItemType.seassignprimarytokenprivilege = CreateTrueEntityItem(); break;
                    case "SeAuditPrivilege": accessTokenItemType.seauditprivilege = CreateTrueEntityItem(); break;
                    case "SeCreatePermanentPrivilege": accessTokenItemType.secreatepermanentprivilege = CreateTrueEntityItem(); break;
                    case "SeCreateTokenPrivilege": accessTokenItemType.secreatetokenprivilege = CreateTrueEntityItem(); break;
                    case "SeEnableDelegationPrivilege": accessTokenItemType.seenabledelegationprivilege = CreateTrueEntityItem(); break;
                    case "SeIncreaseWorkingSetPrivilege": accessTokenItemType.seincreaseworkingsetprivilege = CreateTrueEntityItem(); break;
                    case "SeLockMemoryPrivilege": accessTokenItemType.selockmemoryprivilege = CreateTrueEntityItem(); break;
                    case "SeMachineAccountPrivilege": accessTokenItemType.semachineaccountprivilege = CreateTrueEntityItem(); break;
                    case "SeRelabelPrivilege": accessTokenItemType.serelabelprivilege = CreateTrueEntityItem(); break;
                    case "SeSyncAgentPrivilege": accessTokenItemType.sesyncagentprivilege = CreateTrueEntityItem(); break;
                    case "SeTcbPrivilege": accessTokenItemType.setcbprivilege = CreateTrueEntityItem(); break;
                    case "SeUnsolicitedInputPrivilege": accessTokenItemType.seunsolicitedinputprivilege = CreateTrueEntityItem(); break;
                    case "SeServiceLogonRight": accessTokenItemType.seservicelogonright = CreateTrueEntityItem(); break;
                    case "SeDenyBatchLogonRight": accessTokenItemType.sedenybatchLogonright = CreateTrueEntityItem(); break;
                    case "SeDenyInteractiveLogonRight": accessTokenItemType.sedenyinteractivelogonright = CreateTrueEntityItem(); break;
                    case "SeDenyNetworkLogonRight": accessTokenItemType.sedenynetworklogonright = CreateTrueEntityItem(); break;
                    case "SeDenyRemoteInteractiveLogonRight": accessTokenItemType.sedenyremoteInteractivelogonright = CreateTrueEntityItem(); break;
                    case "SeDenyServiceLogonRight": accessTokenItemType.sedenyservicelogonright = CreateTrueEntityItem(); break;
                }
            }
        }


        private EntityItemBoolType CreateTrueEntityItem()
        {
            return OvalHelper.CreateBooleanEntityItemFromBoolValue(true);
        }

        private EntityItemBoolType CreateFalseEntityItem()
        {
            return OvalHelper.CreateBooleanEntityItemFromBoolValue(false);
        }
    }
}
