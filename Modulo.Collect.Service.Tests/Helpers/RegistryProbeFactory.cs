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
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Service.Tests.Helpers
{
    public class RegistryProbeFactory
    {
        public const string ID_REGISTRY_OBJECT = "oval:org.mitre.oval:obj:717";
        public const string ID_OTHER_REGISTRY_OBJECT = "oval:org.mitre.oval:obj:718";


        public static ProbeResult CreateProbeResultForRegistryCollect()
        {
            ProbeResult probeResult = new ProbeResult();
            probeResult.CollectedObjects = CreateCollectedResultsForRegistryCollect();
            probeResult.ExecutionLog = CreateAnExecutionLogForRegistry();            
            return probeResult;
        }      

        public static ProbeResult CreateProbeResultForRegostryCollectWithError()
        {
            ProbeResult probeResult = new ProbeResult();
            probeResult.CollectedObjects = CreateCollectedResultsForRegistryCollect();
            probeResult.ExecutionLog = CreateAnExecutionLogForRegistryWithError();            
            return probeResult;
        }

        public static ProbeResult CreateProbeResultForRegistryCollectWithVariables()
        {
            ProbeResult probeResult = new ProbeResult();
            CollectedObject registryCollect = CreateRegistryCollectedObject(ID_REGISTRY_OBJECT, 2);
            registryCollect.AddVariableReference(CreateVariableReference());
            List<CollectedObject> collectedObjects = new List<CollectedObject>() { registryCollect };
            probeResult.CollectedObjects = collectedObjects;            
            probeResult.ExecutionLog = CreateAnExecutionLogForRegistry();
            return probeResult;
        }

        public static ProbeResult CreateProbeResultForRegistryCollectWithSytemDataDuplicated()
        {
            ProbeResult probeResult = new ProbeResult();
            CollectedObject registryCollect = CreateRegistryCollectedObject(ID_REGISTRY_OBJECT, 2);
            CollectedObject otherRegistryCollect = CreateRegistryCollectedObject(ID_OTHER_REGISTRY_OBJECT, 2);
            List<CollectedObject> collectedObjects = new List<CollectedObject>() { registryCollect, otherRegistryCollect };
            probeResult.CollectedObjects = collectedObjects;
            probeResult.ExecutionLog = CreateAnExecutionLogForRegistry();
            return probeResult;
        }

        private static IEnumerable<VariableValue> CreateVariableReference()
        {            
            List<string> values = new List<string>() { "Microsoft\\WindowsNT\\" };
            VariableValue variableValue = new VariableValue(ID_REGISTRY_OBJECT, "oval:com.hp:var:4", values);
            List<VariableValue> variables = new List<VariableValue>();
            variables.Add(variableValue);
            return variables;            
        }

        private static IEnumerable<ProbeLogItem> CreateAnExecutionLogForRegistry()
        {
            List<ProbeLogItem> executionLog = new List<ProbeLogItem>();
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Start Collect", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Collecting Information On Registry", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Connecting to host", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Connected", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Collecting information on HKEY_LOCAL_MACHINE", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Collecting information on CDSVersion", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Collecting information on SytemRoot", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "End Collect", Type = TypeItemLog.Info });
            return executionLog;
        }    

        private static IEnumerable<CollectedObject> CreateCollectedResultsForRegistryCollect()
        {
            List<CollectedObject> collectedObjects = new List<CollectedObject>();
            CollectedObject registry = CreateRegistryCollectedObject(ID_REGISTRY_OBJECT, 2);
            collectedObjects.Add(registry);
            return collectedObjects;

        }

        private static  CollectedObject CreateRegistryCollectedObject(string ovalId, int quantidadeDeItems)
        {
            CollectedObject collectedObject = new CollectedObject(ovalId);
            ItemType currentVersion = CreateRegistryItem("HKEY_LOCAL_MACHINE",
                                                    "1",
                                                    "",
                                                    "Microsoft\\WindowsNT\\",
                                                    "CDSVersion",
                                                    "Service Pack 1");

            collectedObject.AddItemToSystemData(currentVersion);

            ItemType systemRoot = CreateRegistryItem("HKEY_LOCAL_MACHINE",
                                                    "2",
                                                    "",
                                                    "Microsoft\\WindowsNT\\",
                                                    "SystemRoot",
                                                    @"c:\Windows");

            collectedObject.AddItemToSystemData(systemRoot);


            return collectedObject;
        }

        private static ItemType CreateRegistryItem(string hiveName, string ovalId, string message, string keyName, string name, string value)
        {
            registry_item registry_item = new registry_item()
            {
                hive = new EntityItemRegistryHiveType() { datatype = SimpleDatatypeEnumeration.@string, Value = hiveName },
                id = ovalId,
                message = MessageType.FromString(message),
                key = new EntityItemStringType() { datatype = SimpleDatatypeEnumeration.@string, Value = keyName },
                name = new EntityItemStringType() { datatype = SimpleDatatypeEnumeration.@string, Value = name },
                value = new EntityItemAnySimpleType[1] { new EntityItemAnySimpleType() { datatype = SimpleDatatypeEnumeration.@string, Value = name } }

            };
            return registry_item;
        }


        private static  IEnumerable<ProbeLogItem> CreateAnExecutionLogForRegistryWithError()
        {
            List<ProbeLogItem> executionLog = new List<ProbeLogItem>();
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Start Collect", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Collecting Information On Registry", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Connecting to host", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Error connecting to host [ MSS-RJ-215]: connection Timeout.", Type = TypeItemLog.Error });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "End Collect", Type = TypeItemLog.Info });
            return executionLog;
        }


        public static ProbeResult CreateProbeResultWithSpecificObject(IEnumerable<definitions.ObjectType> objectTypes, List<string> resultsForObjects)
        {
            ProbeResult probeResult = new ProbeResult();
            probeResult.CollectedObjects = CreateCollectedObjectsForSpecificObjectTypes(objectTypes, resultsForObjects);
            probeResult.ExecutionLog = CreateAnExecutionLogForRegistry();
            return probeResult;
        }

        private static IEnumerable<CollectedObject> CreateCollectedObjectsForSpecificObjectTypes(IEnumerable<definitions.ObjectType> objectTypes, List<string> resultsForObjects)
        {
            List<CollectedObject> collectedObjects = new List<CollectedObject>();
            IEnumerable<registry_object> registryObjects = objectTypes.OfType<registry_object>();
            int id = 1;
            foreach (var registryObject in registryObjects)
            {
                CollectedObject collectedObject = new CollectedObject(registryObject.id);
                string existId = resultsForObjects.Where(x => x.Equals(registryObject.id)).SingleOrDefault();
                if (string.IsNullOrEmpty(existId))
                {
                    ItemType registryItem = CreateRegistryItem(
                                                                registryObject.GetItemValue(registry_object_ItemsChoices.hive).ToString(),
                                                                id.ToString(),
                                                                "",
                                                                registryObject.GetItemValue(registry_object_ItemsChoices.key).ToString(),
                                                                registryObject.GetItemValue(registry_object_ItemsChoices.name).ToString(),
                                                                "default"
                                                               );

                    collectedObject.AddItemToSystemData(registryItem);
                    collectedObjects.Add(collectedObject);
                    id++;
                }
                else
                {
                    resultsForObjects.Remove(existId);
                }

            }
            return collectedObjects;
        }
    }
}
