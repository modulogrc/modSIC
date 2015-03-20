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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Windows.Providers;

namespace Modulo.Collect.Probe.Windows.UserSID55
{
    public class UserSID55ItemTypeGenerator: IItemTypeGenerator
    {
        private WindowsUsersProvider WindowsAccountProvider;
        //public TargetInfo TargetInfo { get; set; }
        
        //public UserSID55ObjectCollector SystemDataSource { get; set; }


        public UserSID55ItemTypeGenerator(WindowsUsersProvider windowsAccountProvider)
        {
            this.WindowsAccountProvider = windowsAccountProvider;
        }

        public virtual IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var userSIDEntity = ((user_sid55_object)objectType).GetUserSIDEntity();

            var userSIDs = this.processVariables(objectType, variables);
            if (userSIDs == null)
                return null;

            if (userSIDEntity.operation != OperationEnumeration.equals)
            {
                var collectedItems = ProcessOperationDifferentOfEquals(userSIDEntity.operation, userSIDs);
                if (collectedItems.Count > 0)
                    return collectedItems;

                var newUserSidItem = CreateUserSIDItemType(userSIDEntity.Value ?? "");
                newUserSidItem.status = StatusEnumeration.doesnotexist;
                newUserSidItem.user_sid.status = newUserSidItem.status;
                return new ItemType[] { newUserSidItem };
            }

            var itemsToCollect = new List<ItemType>();
            foreach (var userSID in userSIDs)
                itemsToCollect.Add(CreateUserSIDItemType(userSID));

            return itemsToCollect;
        }

        private IEnumerable<String> processVariables(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var userSIDEntity = ((user_sid55_object)objectType).UserSID;
            bool thereIsNoEvaluatedVariable = ((variables == null) || (variables.VariableValues.Count() == 0));

            if (thereIsNoEvaluatedVariable && (string.IsNullOrEmpty(userSIDEntity.var_ref)))
                return new List<String>(new string[] { userSIDEntity.Value });

            var variableEvaluator = new UserSID55EntityVariableEvaluator(objectType, variables);
            var processedVariables = variableEvaluator.ProcessVariableForAllObjectEntities();
            return (processedVariables == null) ? null : processedVariables;
        }

        private IList<ItemType> ProcessOperationDifferentOfEquals(OperationEnumeration operation, IEnumerable<String> soFarUserSIDs)
        {
            var collectedItems = new List<ItemType>();
            var allLocalUsers = this.WindowsAccountProvider.GetAllGroupByUsers();
            foreach(var soFarUser in soFarUserSIDs)
                foreach(var foundUser in allLocalUsers)
                    if (this.Compare(foundUser.AccountSID, soFarUser, operation))
                    {
                        var userGroups = this.WindowsAccountProvider.GetUserGroups(foundUser.Name, AccountSearchReturnType.SID);
                        var collectedUserItem = CreateCollectedUserItem(foundUser.AccountSID, foundUser.Enabled, userGroups);
                        collectedItems.Add(collectedUserItem);
                    }

            return collectedItems;
        }


        private bool Compare(string foundUserSID, string expectedUserSID, OperationEnumeration operation)
        {
            var ovalCompartor = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            return ovalCompartor.Compare(foundUserSID, expectedUserSID, operation);
        }
                    
        private user_sid_item CreateUserSIDItemType(string userSIDEntityValue)
        {
            return new user_sid_item() 
            { 
                status = StatusEnumeration.notcollected,
                user_sid = OvalHelper.CreateItemEntityWithStringValue(userSIDEntityValue) 
            };
        }

        private user_sid_item CreateCollectedUserItem(string userSID, bool? userEnabled, IEnumerable<string> groupSIDs)
        {
            var groupEntities = groupSIDs.Select(group => new EntityItemStringType() { Value = group });
            return new user_sid_item()
            {
                status = StatusEnumeration.exists,
                user_sid = OvalHelper.CreateItemEntityWithStringValue(userSID),
                enabled = OvalHelper.CreateBooleanEntityItemFromBoolValue((bool)userEnabled),
                group_sid = groupEntities.ToArray()
            };
        }
    }
}
