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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Linux;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Linux;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.Probe.Unix;

namespace Modulo.Collect.Probe.Linux.RPMInfo
{
    public class RPMInfoItemTypeGenerator : IItemTypeGenerator
    {
        public SshCommandLineRunner CommandRunner { get; set; }

        public RPMInfoCollector RpmInfoCollector { get; set; }

        public virtual IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var packageNameEntity = (Modulo.Collect.OVAL.Definitions.EntityObjectStringType)((rpminfo_object)objectType).Items.OfType<EntityObjectStringType>().FirstOrDefault();
            var packageNameValues = this.EvaluateVariable(packageNameEntity, variables);
            packageNameValues = ProcessOperationForEntity(packageNameEntity, packageNameValues);

            var itemsToCollect = new List<ItemType>();
            foreach (var packageName in packageNameValues)
            {
                var newItemToCollect = new rpminfo_item() { name = OvalHelper.CreateItemEntityWithStringValue(packageName) };
                itemsToCollect.Add(newItemToCollect);
            }

            return itemsToCollect;
        }

        private IEnumerable<string> ProcessOperationForEntity(
            EntityObjectStringType nameEntity,
            IEnumerable<string> alreadyProcessedNames)
        {
            if (nameEntity.operation.Equals(OperationEnumeration.equals))
                return alreadyProcessedNames;

            var newNames = new List<string>();
            var allRpmInfos = GetAllRpmInfoOnTarget();
            
            if (nameEntity.operation.Equals(OperationEnumeration.patternmatch))
            {
                foreach (var processedName in alreadyProcessedNames)
                    foreach (var rpmInfo in allRpmInfos)
                        if (Regex.IsMatch(rpmInfo, processedName))
                            newNames.Add(rpmInfo);
            }

            if (nameEntity.operation.Equals(OperationEnumeration.notequal))
            {
                foreach (var processedName in alreadyProcessedNames)
                    foreach (var rpmInfo in allRpmInfos)
                        if (!rpmInfo.Equals(processedName))
                            newNames.Add(rpmInfo);
            }
            
            return newNames;
        }

        private IEnumerable<string> GetAllRpmInfoOnTarget()
        {
            if (this.RpmInfoCollector == null)
                this.RpmInfoCollector = new RPMInfoCollector() { CommandRunner = this.CommandRunner };
            
            return this.RpmInfoCollector.GetAllTargetRpmNames();
        }

        private IEnumerable<string> EvaluateVariable(Modulo.Collect.OVAL.Definitions.EntityObjectStringType variableNameEntity, VariablesEvaluated variables)
        {
            var variableEvaluator = new VariableEntityEvaluator(variables);
            return variableEvaluator.EvaluateVariableForEntity(variableNameEntity);
        }
    }
}
