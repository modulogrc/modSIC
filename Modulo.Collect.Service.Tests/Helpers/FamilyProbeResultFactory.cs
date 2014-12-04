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
using definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Service.Tests.Helpers
{
    public class FamilyProbeResultFactory
    {
        public static ProbeResult CreateProbeResultForFamilyollect()
        {
            ProbeResult probeResult = new ProbeResult();
            probeResult.CollectedObjects = CreateCollectedResultsForFamilyCollect();
            probeResult.ExecutionLog = CreateAnExecutionLogForFamily();
            return probeResult;
        }

        public static ProbeResult CreateProbeResultForFamilyCollectWithError()
        {
            ProbeResult probeResult = new ProbeResult();
            probeResult.CollectedObjects = CreateCollectedResultsForFamilyCollect();
            probeResult.ExecutionLog = CreateAnExecutionLogForFamilyWithError();
            return probeResult;
        }

        private static IEnumerable<ProbeLogItem> CreateAnExecutionLogForFamily()
        {
            List<ProbeLogItem> executionLog = new List<ProbeLogItem>();
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Start Collect", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Collecting Information On Family", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Connecting to host", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Connected", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "End Collect", Type = TypeItemLog.Info });
            return executionLog;
        }

        private static IEnumerable<CollectedObject> CreateCollectedResultsForFamilyCollect()
        {
            List<CollectedObject> collectedObjects = new List<CollectedObject>();
            CollectedObject registry = CreateFamilyCollectedObject("oval:org.mitre.oval:obj:99", 1);
            collectedObjects.Add(registry);
            return collectedObjects;

        }

        private static CollectedObject CreateFamilyCollectedObject(string ovalId, int quantidadeDeItems)
        {
            CollectedObject collectedObject = new CollectedObject(ovalId);
            ItemType familyItem = CreateFamilyItem("windows");

            collectedObject.AddItemToSystemData(familyItem);
            return collectedObject;
        }

        private static ItemType CreateFamilyItem(string familyName)
        {
            family_item familyItem = CreateFamilySpecificItem("1");


            return familyItem;
        }

        private static family_item CreateFamilySpecificItem(string id)
        {
            family_item familyItem = new family_item()
            {
                family = new EntityItemFamilyType() { datatype = SimpleDatatypeEnumeration.@string },
                id = id,
                message = MessageType.FromString(""),
                status = StatusEnumeration.exists
            };
            return familyItem;
        }


        private static IEnumerable<ProbeLogItem> CreateAnExecutionLogForFamilyWithError()
        {
            List<ProbeLogItem> executionLog = new List<ProbeLogItem>();
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Start Collect", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Collecting Information On Registry", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Connecting to host", Type = TypeItemLog.Info });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "Error connecting to host [ MSS-RJ-215]: connection Timeout.", Type = TypeItemLog.Error });
            executionLog.Add(new ProbeLogItem() { Date = DateTime.Now, Message = "End Collect", Type = TypeItemLog.Info });
            return executionLog;
        }


        public static ProbeResult CreateProbeResultWithSpecificObject(IEnumerable<OVAL.Definitions.ObjectType> objectTypes, List<string> resultsForObjects)
        {
            ProbeResult probeResult = new ProbeResult();
            probeResult.CollectedObjects = CreateCollectedObjectsForSpecificObjectTypes(objectTypes,resultsForObjects);
            probeResult.ExecutionLog = CreateAnExecutionLogForFamily();
            return probeResult;
        }

        private static IEnumerable<CollectedObject> CreateCollectedObjectsForSpecificObjectTypes(IEnumerable<OVAL.Definitions.ObjectType> objectTypes, List<string> resultsForObjects)
        {
            List<CollectedObject> collectedObjects = new List<CollectedObject>();
            IEnumerable<family_object> familyObjects = objectTypes.OfType<family_object>();
            int id = 1;
            foreach (var familyObject in familyObjects)
            {
                string existId = resultsForObjects.Where(x => x.Equals(familyObject.id)).SingleOrDefault();
                if (string.IsNullOrEmpty(existId))
                {
                    CollectedObject collectedObject = new CollectedObject(familyObject.id);
                    ItemType familyItem = CreateFamilySpecificItem(id.ToString());

                    collectedObject.AddItemToSystemData(familyItem);
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
