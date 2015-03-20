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
using System.Text;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Modulo.Collect.Probe.Common.Test
{
    public class GenericXmlFileContentProberTestBase<ConnectionProviderType> : ProberTestBase 
        where ConnectionProviderType : IConnectionProvider, new()
    {
        public GenericXmlFileContentProberTestBase() : base()
        {
            ProberBehaviorCreator = new GenericXmlFileContentProberBehaviorCreator<ConnectionProviderType>();
        }
    }

    public class ProberTestBase
    {
        protected TargetInfo FakeTargetInfo;
        protected List<IConnectionProvider> FakeContext;
        protected ProberBehaviorCreator ProberBehaviorCreator;
        
        public ProberTestBase()
        {
            FakeTargetInfo = ProbeHelper.CreateFakeTarget();
            FakeContext = ProbeHelper.CreateFakeContext();
            ProberBehaviorCreator = new ProberBehaviorCreator();
        }

        protected CollectInfo GetFakeCollectInfo(
            string objectTypeID, 
            string definitionsFilename = "definitionsSimple.xml",
            string systemCharacteristicsFilename = null)
        {
            var fakeObjectType = ProbeHelper.GetDefinitionObjectTypeByID(definitionsFilename, objectTypeID);
            var fakeObjectTypes = new OVAL.Definitions.ObjectType[] { fakeObjectType };
            var fakeSysCharacteristics = ProbeHelper.GetOvalSystemCharacteristicsFromFile(systemCharacteristicsFilename);

            return ProbeHelper.CreateFakeCollectInfo(fakeObjectTypes, null, fakeSysCharacteristics);
        }

        //protected CollectInfo GetFakeCollectInfoWithManyObjectTypes(string[] objectTypeIDs)
        //{
        //    var fakeObjectTypes = new List<OVAL.Definitions.ObjectType>();
        //    foreach (var objectID in objectTypeIDs)
        //    {
        //        var newObjectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", objectID);
        //        fakeObjectTypes.Add(newObjectType);
        //    }

        //    return ProbeHelper.CreateFakeCollectInfo(fakeObjectTypes.ToArray(), null, null);
        //}

        protected void DoAssertForSingleCollectedObject(
            ProbeResult executionResult, 
            Type expectedItemInstanceType)
        {
            DoBasicAssert(executionResult);

            var collectedItems = executionResult.CollectedObjects.Single().SystemData;
            Assert.AreEqual(1, collectedItems.Count, "Only one item is expected on system data.");
            Assert.IsInstanceOfType(collectedItems.Single(), expectedItemInstanceType, "An unexpected instance of item type was found in system data.");
            Assert.AreEqual(StatusEnumeration.exists, collectedItems.Single().status, "An unexpected item status was found in system data.");
        }

        protected void DoAssertForExecutionWithErrors(ProbeResult probeExecutionResult, Type expectedType)
        {
            DoBasicAssert(probeExecutionResult);

            var itemsType = probeExecutionResult.CollectedObjects.First().SystemData;
            Assert.IsNotNull(itemsType, "The return of GetItemsToCollect cannot be null.");
            Assert.AreEqual(1, itemsType.Count(), "Unexpected quantity of generated items.");

            var systemItem = itemsType.Single();
            Assert.IsInstanceOfType(systemItem, expectedType, "Unxpected generated item type was found.");
            Assert.AreEqual(StatusEnumeration.error, systemItem.status, "An unexpected item status was found.");
            Assert.IsNotNull(systemItem.message, "The Entity Item Message Type cannot be null");

            var itemTypeErrorMessage = string.Format("The exception message cannot be found in entity item message. Found message: '{0}'", systemItem.message.First().Value);
            Assert.IsTrue(systemItem.message.First().Value.Contains(ProberBehaviorCreator.FAKE_EXCEPTION_MESSAGE), itemTypeErrorMessage);
        }

        private void DoBasicAssert(ProbeResult executionResult)
        {
            Assert.IsNotNull(executionResult, "The probe execution cannot be null.");
            Assert.IsNotNull(executionResult.ExecutionLog, "The probe execution log cannot be null");
            Assert.AreEqual(1, executionResult.CollectedObjects.Count(), "Only one collected object is expected for this test.");
            
            var collectedObject = executionResult.CollectedObjects.Single();
            var collectedItems = collectedObject.SystemData;

            Assert.AreEqual(collectedObject.ObjectType.reference.Count(), 1, "Unexpected number of item references was found.");
            Assert.AreEqual(collectedObject.ObjectType.reference.Count(), collectedItems.Count, "Unexpected number of generated items type was found.");
        }

    }
}
