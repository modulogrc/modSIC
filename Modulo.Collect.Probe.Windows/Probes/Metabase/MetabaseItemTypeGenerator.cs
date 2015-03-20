/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;

namespace Modulo.Collect.Probe.Windows.Probes.Metabase
{
    public class MetabaseItemTypeGenerator: IItemTypeGenerator
    {
        public MetabaseObjectCollector ObjectCollector { get; set; }

        public IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, OVAL.Definitions.variableEvaluator.VariablesEvaluated variables)
        {
            var metabaseObject = (metabase_object)objectType;

            var variableEvaluator = new VariableEntityEvaluator(variables);
            var idEntityValues = variableEvaluator.EvaluateVariableForEntity(metabaseObject.GetIdEntity());
            var keyEntityValues = variableEvaluator.EvaluateVariableForEntity(metabaseObject.GetKeyEntity());
            var allIdsAfterOperation = this.ProcessOperationForIdEntity(metabaseObject.GetIdEntity(), idEntityValues);
            var allKeysAfterOperation = this.ProcessOperationForKeyEntity(metabaseObject.GetKeyEntity(), keyEntityValues);

            return CreateMetabaseItems(allKeysAfterOperation, allIdsAfterOperation);

        }

        private IEnumerable<String> ProcessOperationForIdEntity(
            EntityObjectIntType idEntity, IEnumerable<string> currentIdValues)
        {
            if (idEntity.operation.Equals(OperationEnumeration.equals))
                return currentIdValues;

            var result = new List<String>();
            var allMetabaseIDs = this.ObjectCollector.GetAllMetabaseIDs();
            var ovalComparator = new OvalComparatorFactory().GetComparator(idEntity.datatype);
            foreach (var idValue in currentIdValues)
                foreach (var metabaseId in allMetabaseIDs)
                    if (ovalComparator.Compare(metabaseId, idValue, idEntity.operation))
                        result.Add(metabaseId);

            return result;
        }

        private IEnumerable<String> ProcessOperationForKeyEntity(
            EntityObjectStringType keyEntity, IEnumerable<string> currentKeyValues)
        {
            if (keyEntity.operation.Equals(OperationEnumeration.equals))
                return currentKeyValues;


            var result = new List<String>();
            var allMetabaseKeys = this.ObjectCollector.GetAllMetabaseKeys();
            var ovalComparator = new OvalComparatorFactory().GetComparator(keyEntity.datatype);
            foreach (var keyValue in currentKeyValues)
                foreach (var metabaseKey in allMetabaseKeys)
                    if (ovalComparator.Compare(metabaseKey, keyValue, keyEntity.operation))
                        result.Add(metabaseKey);

            return result;
        }

        private IEnumerable<ItemType> CreateMetabaseItems(IEnumerable<String> keys, IEnumerable<String> ids)
        {
            var itemsToCollect = new List<ItemType>();
            foreach (var idEntityValue in ids)
            {
                foreach (var keyEntityValue in keys)
                {
                    var newItemToCollect =
                        new metabase_item()
                        {
                            id1 = new EntityItemIntType() { Value = idEntityValue },
                            key = new EntityItemStringType() { Value = keyEntityValue }
                        };

                    itemsToCollect.Add(newItemToCollect);
                }
            }

            return itemsToCollect;
        }
    }
}
