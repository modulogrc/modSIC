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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.EntityOperations;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Extensions;
using Modulo.Collect.Probe.Independent.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.Operators;
using System.Text.RegularExpressions;
using Modulo.Collect.Probe.Windows.Providers;

namespace Modulo.Collect.Probe.Windows.User
{
    public class UserItemTypeGenerator: IItemTypeGenerator
    {
        private WindowsUsersProvider WindowsAccountProvider;

        public UserItemTypeGenerator(WindowsUsersProvider accountProvider)
        {
            this.WindowsAccountProvider = accountProvider;
        }

        public virtual IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var userNames = this.processVariables(objectType, variables);
            if (userNames == null)
                return new List<ItemType>();

            return this.ProcessOperation(((user_object)objectType).User, userNames);
        }

        private IEnumerable<String> processVariables(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var userEntityValue = ((user_object)objectType).User.Value;
            if (variables == null || variables.VariableValues.IsEmpty())
                return new string[] { userEntityValue };

            return 
                new UserEntityVariableEvaluator(objectType, variables)
                    .ProcessVariableForAllObjectEntities();
        }

        private IEnumerable<ItemType> ProcessOperation(EntitySimpleBaseType userEntity, IEnumerable<String> evalutedVariables)
        {
            var entityOperation = userEntity.operation;
            var entityValue = userEntity.Value;

            var userNames = string.IsNullOrEmpty(userEntity.var_ref) ? new string[] { entityValue } : evalutedVariables;

            var result = new List<ItemType>();
            foreach (var userName in userNames)
                if (IsEqualsOperation(userEntity))
                    result.Add(CreateUserItem(userName));
                else
                    foreach (var collectedUser in WindowsAccountProvider.GetAllGroupByUsers())//WindowsAccountProvider.GetAllUsersByGroup())
                        if (this.Compare(userName, collectedUser.Name, entityOperation))
                            result.Add(CreateUserItemFromWindowsAccount(collectedUser));

            if (result.IsEmpty())
            {
                var newNotExistsItem = CreateUserItem(userNames.FirstOrDefault());
                newNotExistsItem.status = StatusEnumeration.doesnotexist;
                result.Add(newNotExistsItem);
            }

            return result;
        }

        private bool IsEqualsOperation(EntitySimpleBaseType entity)
        {
            return
                entity.operation.Equals(OperationEnumeration.equals) ||
                entity.operation.Equals(OperationEnumeration.caseinsensitiveequals);
        }

        private ItemType CreateUserItem(string userEntityValue)
        {
            return new user_item()
            {
                status = StatusEnumeration.notcollected,
                user = OvalHelper.CreateItemEntityWithStringValue(userEntityValue)
            };
        }

        private ItemType CreateUserItemFromWindowsAccount(WindowsAccount windowsAccount)
        {

            var groups = new EntityItemStringType[] { new EntityItemStringType() { status = StatusEnumeration.doesnotexist } };
            if (windowsAccount.Members.HasItems())
                groups = windowsAccount.Members.Select(g => new EntityItemStringType() { Value = g.Name }).ToArray();
            
            return new user_item()
            {
                status = StatusEnumeration.exists,
                user = OvalHelper.CreateItemEntityWithStringValue(windowsAccount.Name),
                enabled = OvalHelper.CreateBooleanEntityItemFromBoolValue(windowsAccount.Enabled),
                group = groups
            };
        }

        private bool Compare(string userEntityValue, string foundUsername, OperationEnumeration operation)
        {
            return
                new OvalComparatorFactory()
                    .GetComparator(SimpleDatatypeEnumeration.@string)
                    .Compare(foundUsername, userEntityValue, operation);
        }
    }
}
