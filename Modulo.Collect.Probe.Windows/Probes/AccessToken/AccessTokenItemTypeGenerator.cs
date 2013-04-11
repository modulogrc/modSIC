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
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.WMI;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Windows.Providers;

namespace Modulo.Collect.Probe.Windows.AccessToken
{
    public class AccessTokenItemTypeGenerator : IItemTypeGenerator
    {
        private const string ITEMS_CREATION_ERROR_MESSAGE = "[AccessTokenProber] - An error occrred while trying to create items to collect from object '{0}': '{1}'";
        private WindowsUsersProvider WindowsAccountProvider;

        public AccessTokenItemTypeGenerator(WindowsUsersProvider accountProvider)
        {
            this.WindowsAccountProvider = accountProvider;
        }

        public virtual IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var securityPrincipleEntity = ((accesstoken_object)objectType).GetSecurityPrincipleEntity();
            var securityPrinciples = this.EvaluateVariable(objectType, variables);
            securityPrinciples = this.ProcessOperation(securityPrinciples, securityPrincipleEntity);

            return this.CreateItemsToCollectFromSecurityPrincipleList(securityPrinciples);
        }

        private IList<string> EvaluateVariable(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var securityPrincipleEntity = ((accesstoken_object)objectType).GetSecurityPrincipleEntity();
            if (string.IsNullOrEmpty(securityPrincipleEntity.var_ref))
                return new List<String>(new string[] { securityPrincipleEntity.Value });

            var variableEvaluator = new AccessTokenVariableEvaluator(objectType, variables);
            var result = variableEvaluator.ProcessVariableForAllObjectEntities();
            return (result == null) ? new string[] { }.ToList() : result.ToList();
        }

        private IEnumerable<ItemType> CreateItemsToCollectFromSecurityPrincipleList(IList<string> securityPrinciples)
        {
            var itemsToCollect = new List<ItemType>();
            foreach (var securityPrinciple in securityPrinciples)
            {
                var newItem = new accesstoken_item() { security_principle = OvalHelper.CreateItemEntityWithStringValue(securityPrinciple) };
                itemsToCollect.Add(newItem);
            }

            return itemsToCollect;
        }

        private IList<String> ProcessOperation(IEnumerable<string> entityValuesAlreadyProcessed, EntityObjectStringType entity)
        {
            if (entity.operation == OperationEnumeration.equals)
                return entityValuesAlreadyProcessed.ToList();

            var newResult = new List<String>();
            foreach (var entityValueAlreadyProcessed in entityValuesAlreadyProcessed)
                ((List<String>)newResult).AddRange(ProcessOperationDifferentOfEquals(entity.operation, entityValueAlreadyProcessed));

            return newResult;
        }

        private IList<string> ProcessOperationDifferentOfEquals(OperationEnumeration operation, String entityValue)
        {
            var allSecurityPrinciples = this.getAllSecurityPrinciples();
            var comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);

            var processingResult = new List<String>();
            foreach (var securityPrinciple in allSecurityPrinciples)
                if (comparator.Compare(securityPrinciple, entityValue, operation))
                    processingResult.Add(securityPrinciple);

            return processingResult;
        }

        private IEnumerable<String> getAllSecurityPrinciples()
        {
            var allSecurityPrinciples = new List<String>();
            var allGroupsAndUsers = this.WindowsAccountProvider.GetAllGroupsAndUsers();

            return allGroupsAndUsers.Select(a => a.Name);
        }
    }
}
