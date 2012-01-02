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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53
{
    public class RegKeyEffectiveRightsItemTypeGenerator: IItemTypeGenerator
    {
        private string HiveEntityValue = regkeyeffectiverights53_object_ItemsChoices.hive.ToString();
        private string KeyEntityName = regkeyeffectiverights53_object_ItemsChoices.key.ToString();
        private string TrusteeSIDEntityName = regkeyeffectiverights53_object_ItemsChoices.trustee_sid.ToString();
        
        public RegKeyEffectiveRightsObjectCollector ObjectCollector { get; set; }
        public RegKeyEffectiveRightsOperationEvaluator OperationEvaluator { get; set; }

        /// <summary>
        /// Generates concrete items type to collect from object type.
        /// </summary>
        public virtual IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var itemsToCollect = new List<ItemType>();
            var variableEvaluator = new RegKeyEffectiveRightsVariableEvaluator(objectType, variables);

            var hiveValue = variableEvaluator.AllObjectEntities[HiveEntityValue].Value;

            var processedKeys = variableEvaluator.TryToProcessObjectEntity(KeyEntityName);
            if (processedKeys != null)
                processedKeys = this.OperationEvaluator.ProcessOperationForKeyEntity(objectType, processedKeys.ToArray());
            if (processedKeys == null)
                return new ItemType[] { };

            var processedTrustees = variableEvaluator.TryToProcessObjectEntity(TrusteeSIDEntityName);
            var trusteeSidEntity = variableEvaluator.AllObjectEntities[TrusteeSIDEntityName];
            if (processedTrustees == null)
                return new ItemType[] { };

            foreach (var regKey in processedKeys)
            {
                foreach (var sid in processedTrustees)
                {
                    if (trusteeSidEntity.operation.Equals(OperationEnumeration.equals))
                    {
                        var newItemToCollect = this.createRegKeyEffectiveRights(hiveValue, regKey, sid);
                        newItemToCollect.status = StatusEnumeration.notcollected;
                        itemsToCollect.Add(newItemToCollect);
                    }
                    else
                    {
                        var alreadyCollectedItems = ObjectCollector.CollectItemsApplyingOperation(hiveValue, regKey, sid, trusteeSidEntity.operation);
                        itemsToCollect.AddRange(alreadyCollectedItems);

                    }
                }
            }

            return itemsToCollect;
        }


        private regkeyeffectiverights_item createRegKeyEffectiveRights(string hiveValue, string keyValue, string trusteeValue)
        {
            return new regkeyeffectiverights_item()
            {
                hive = new EntityItemRegistryHiveType() { Value = hiveValue },
                key = OvalHelper.CreateItemEntityWithStringValue(keyValue),
                trustee_sid = OvalHelper.CreateItemEntityWithStringValue(trusteeValue)
            };
        }


    }
}
