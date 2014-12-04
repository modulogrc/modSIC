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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Unix;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Unix.SSHCollectors;

namespace Modulo.Collect.Probe.Unix.RunLevel
{
    public class RunLevelItemTypeGenerator : IItemTypeGenerator
    {
        public SshCommandLineRunner CommandLineRunner { get; set; }

        public RunLevelCollector RunLevelCollector { get; set; }

        public virtual IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var serviceNameEntity = ((runlevel_object)objectType).GetItemValue("service_name");
            var serviceNameValues = this.EvaluateVariable((EntityObjectStringType)serviceNameEntity, variables);

            IEnumerable<string> serviceUniverse = null;

            var operation = ((EntitySimpleBaseType)serviceNameEntity).operation;
            if (operation == OperationEnumeration.patternmatch)
            {
                serviceUniverse = GetServiceUniverse();
                serviceNameValues = this.DoPatternMatch(serviceNameValues, serviceUniverse);
            }
            else if (operation == OperationEnumeration.notequal)
            {
                serviceUniverse = GetServiceUniverse();
                serviceNameValues = this.NotEqualOperation(serviceNameValues, serviceUniverse);
            }

            var runLevelEntity = ((runlevel_object)objectType).GetItemValue("runlevel");
            var runLevelValues = this.EvaluateVariable((EntityObjectStringType)runLevelEntity, variables);

            var itemsToCollect = new List<ItemType>();
            foreach (var serviceName in serviceNameValues)
            {
                foreach (var runLevelName in runLevelValues)
                {
                    var newItemToCollect = new runlevel_item() { 
                        service_name = OvalHelper.CreateItemEntityWithStringValue(serviceName),
                        runlevel = OvalHelper.CreateItemEntityWithStringValue(runLevelName) };
                    itemsToCollect.Add(newItemToCollect);
                }
            }

            return itemsToCollect;
        }

        private IEnumerable<string> GetServiceUniverse()
        {
            if (this.RunLevelCollector == null)
                this.RunLevelCollector = new RunLevelCollector() { CommandLineRunner = CommandLineRunner };

            return this.RunLevelCollector.GetTargetServices();
        }

        private IEnumerable<string> NotEqualOperation(IEnumerable<string> serviceNameValues, IEnumerable<string> universe)
        {
            List<string> retList = new List<string>();

            foreach (string myService in universe)
            {
                bool matchesSomebody = false;
                foreach (string myPattern in serviceNameValues)
                {
                    if (myService == myPattern)
                        matchesSomebody = true;
                }

                if (!matchesSomebody)
                    retList.Add(myService);
            }

            return retList;
        }

        private IEnumerable<string> DoPatternMatch(IEnumerable<string> serviceNameValues, IEnumerable<string> universe)
        {
            List<string> retList = new List<string>();

            foreach (string myPattern in serviceNameValues)
            {
                foreach (string myService in universe)
                {
                    if (Regex.Match(myService, myPattern).Success)
                        retList.Add(myService);
                }
            }

            return retList;
        }

        private IEnumerable<string> EvaluateVariable(EntityObjectStringType variableNameEntity, VariablesEvaluated variables)
        {
            var variableEvaluator = new VariableEntityEvaluator(variables);
            return variableEvaluator.EvaluateVariableForEntity(variableNameEntity);
        }
    }
}
