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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53;
using Rhino.Mocks;
using Definitions = Modulo.Collect.OVAL.Definitions;
using SystemCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Windows.Test.RegKeyEffectiveRights
{
    [TestClass]
    public class RegKeyEffectiveRightsProberTest: ProberTestBase
    {

        private const string MSG_UNEXPECTED_COLLECTED_ITEM_VALUE = "Unexpected collected item value: '{0}'";
        private const string MSG_UNEXPECTED_COLLECTED_ENTITY_VALUE = "Unexpected entity value for collected file item: 'fileeffectiverights_item.{0}'";
        
        private ItemType[] FakeItemsToReturnByItemTypeGenerator;
        private CollectInfo FakeCollectInfo;
        private CollectedItem[] FakeCollectedItems;

        public RegKeyEffectiveRightsProberTest()
        {
            FakeCollectInfo = GetFakeCollectInfo("oval:modulo:obj:750");
            FakeItemsToReturnByItemTypeGenerator = new ItemType[] { new regkeyeffectiverights_item() };
            FakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new regkeyeffectiverights_item()) };
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_execute_a_simple_collect_for_RegKeyEffectiveRights_object()
        {
            var prober = new RegKeyEffectiveRightsProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    prober,
                    FakeItemsToReturnByItemTypeGenerator,
                    FakeCollectedItems);

            var probeExecutionResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForSingleCollectedObject(probeExecutionResult, typeof(regkeyeffectiverights_item));

            this.doBasicProbeResultAssert(probeExecutionResult);
            CollectedObject collectedObject = probeExecutionResult.CollectedObjects.ElementAt(0);
            this.assertCollectedItemsReferences(collectedObject, (IList<ItemType>)collectedObject.SystemData);
            this.assertCollectedItemStatus(collectedObject.ObjectType.reference.ElementAt(0), collectedObject.SystemData[0]);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_process_a_RegKeyEffectiveRightsObject_with_set_operation()
        {
            var definitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            var fakeObjects = this.GetFakeObjectsFromDefinitions(definitions, new string[] { "750", "751", "760" });
            var fakeSysCharac = ProbeHelper.GetOvalSystemCharacteristicsFromFile("system_characteristics_with_sets.xml");
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(fakeObjects, null, fakeSysCharac);
            var fakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new regkeyeffectiverights_item()) };
            var prober = this.GetMockedRegKeyEffectiveRightsProber(fakeCollectedItems, null);

            ProbeResult proberExecutionResult = prober.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);


            this.doBasicProbeResultAssert(proberExecutionResult);
            Assert.AreEqual(3, proberExecutionResult.CollectedObjects.Count(), "Unexpected collected objects count.");
            
            var collectedObjFromObjectWithSet = proberExecutionResult.CollectedObjects.SingleOrDefault(obj => obj.ObjectType.id.Equals("oval:modulo:obj:760"));
            Assert.IsNotNull(collectedObjFromObjectWithSet, "There are no collected objects generated from object type with set.");
            
            var referencedItems = collectedObjFromObjectWithSet.ObjectType.reference;
            Assert.AreEqual(2, referencedItems.Count(), "Unexpected collected object references count.");
            Assert.AreEqual("761", referencedItems.ElementAt(0).item_ref, "Unexpected referenced item was found on collected object.");
            Assert.AreEqual("762", referencedItems.ElementAt(1).item_ref, "Unexpected referenced item was found on collected object.");

            var generatedItemFromObjectTypeWithSet = collectedObjFromObjectWithSet.SystemData;
            Assert.IsNotNull(generatedItemFromObjectTypeWithSet, "No item type was found in collected object system data.");
            Assert.AreEqual(2, generatedItemFromObjectTypeWithSet.Count, "Unexpected system data count.");
            
            var firstRegKeyEffectiveRightsItem = (regkeyeffectiverights_item)generatedItemFromObjectTypeWithSet[0];
            Assert.AreEqual("761", firstRegKeyEffectiveRightsItem.id, "Unexpected id for a item was found.");
            Assert.AreEqual(@"SOFTWARE\Modulo\RiskManager", firstRegKeyEffectiveRightsItem.key.Value, "Unexpected entity item type value was found.");
            Assert.AreEqual("0", firstRegKeyEffectiveRightsItem.key_enumerate_sub_keys.Value, "Unexpected entity item type value was found.");
            
            var secondRegKeyEffectiveRightsItem = (regkeyeffectiverights_item)generatedItemFromObjectTypeWithSet[1];
            Assert.AreEqual("762", secondRegKeyEffectiveRightsItem.id, "Unexpected id for a item was found.");
            Assert.AreEqual(@"SOFTWARE\Modulo\RiskManagerNG", secondRegKeyEffectiveRightsItem.key.Value, "Unexpected entity item type value was found.");
            Assert.AreEqual("1", secondRegKeyEffectiveRightsItem.key_enumerate_sub_keys.Value, "Unexpected entity item type value was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_execute_a_collect_for_RegKeyEffectiveRights_with_referenced_variable_on_key_entity()
        {
            var fakeObjects = new Definitions.ObjectType[] { ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "770") };
            string fakeVariableValue = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            var fakeEvaluatedVariables = VariableHelper.CreateVariableWithOneValue("oval:modulo:obj:770", "oval:modulo:var:770", fakeVariableValue);
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(fakeObjects, fakeEvaluatedVariables, null);
            var fakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(new regkeyeffectiverights_item()) };
            var prober = this.GetMockedRegKeyEffectiveRightsProber(fakeCollectedItems, null);

            var proberExecutionResult = prober.Execute(FakeContext, FakeTargetInfo, fakeCollectInfo);

            this.doBasicProbeResultAssert(proberExecutionResult);
            CollectedObject collectedObject = proberExecutionResult.CollectedObjects.Single(obj => obj.ObjectType.id.Equals("oval:modulo:obj:770"));
            this.assertCollectedItemsReferences(collectedObject, (IList<ItemType>)collectedObject.SystemData);
            Assert.AreEqual(1, collectedObject.SystemData.Count, "Unexpected system data count.");
            this.assertCollectedItemStatus(collectedObject.ObjectType.reference.ElementAt(0), collectedObject.SystemData[0]);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned()
        {
            var prober = new RegKeyEffectiveRightsProber();
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(prober);

            var proberResult = prober.Execute(FakeContext, FakeTargetInfo, FakeCollectInfo);

            DoAssertForExecutionWithErrors(proberResult, typeof(regkeyeffectiverights_item));
        }




        private void doBasicProbeResultAssert(ProbeResult resultToAssert)
        {
            Assert.IsNotNull(resultToAssert, "The result of probe execution cannot be null.");
            Assert.IsNotNull(resultToAssert.ExecutionLog, "The ExecutionLog of RegKeyEffectiveRightsProber was not created.");
            Assert.IsNotNull(resultToAssert.CollectedObjects, "There are no collected objects.");
        }

        private void assertCollectedItemsReferences(CollectedObject collectedObject, IList<ItemType> collectedItems)
        {
            int collectedObjectCount = collectedObject.ObjectType.reference.Count();

            Assert.AreEqual(collectedItems.Count(), collectedObjectCount, "Unexpected number of item references was found.");
            Assert.AreEqual(collectedObjectCount, collectedItems.Count, "Unexpected number of generated items type was found.");
        }

        private void assertCollectedItemStatus(SystemCharacteristics.ReferenceType objectReference, ItemType collectedItem)
        {
            Assert.IsInstanceOfType(collectedItem, typeof(regkeyeffectiverights_item), "The generated ItemType must be a instance of regkeyeffectiverights_item class.");
            Assert.AreEqual(objectReference.item_ref, collectedItem.id, "The generated ItemType ID must be equal to collected object ID.");
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status, "A generated ItemType with unexpected OVAL Status was found.");
        }

        private RegKeyEffectiveRightsProber GetMockedRegKeyEffectiveRightsProber(IEnumerable<CollectedItem> fakeCollectedItems, IEnumerable<String> fakeProcessedKeys)
        {
            MockRepository mocks = new MockRepository();
            var fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();
            var fakeObjectCollector = mocks.DynamicMock<BaseObjectCollector>();
            var fakeItemTypeGenerator = new RegKeyEffectiveRightsItemTypeGenerator();

            if (fakeProcessedKeys != null)
            {
                var fakeOperationEvaluator = mocks.DynamicMock<RegKeyEffectiveRightsOperationEvaluator>();
                fakeOperationEvaluator.SystemDataSource = fakeObjectCollector;
                Expect.Call(fakeOperationEvaluator.ProcessOperationForKeyEntity(null, null)).IgnoreArguments().Return(fakeProcessedKeys);
                fakeItemTypeGenerator.OperationEvaluator = fakeOperationEvaluator;
            }
            Expect.Call(fakeObjectCollector.CollectDataForSystemItem(null)).IgnoreArguments().Repeat.Any().Return(fakeCollectedItems);
            
            mocks.ReplayAll();


            return new RegKeyEffectiveRightsProber() { ConnectionManager = fakeConnectionManager, ObjectCollector = fakeObjectCollector, ItemTypeGenerator = fakeItemTypeGenerator };
        }

        private Definitions.ObjectType[] GetFakeObjectsFromDefinitions(oval_definitions definitions, string[] objectIDs)
        {
            var fakeObjects = new List<Definitions.ObjectType>();
            foreach(var objectID in objectIDs)
                fakeObjects.Add(ProbeHelper.GetOvalComponentByOvalID(definitions, string.Format("oval:modulo:obj:{0}", objectID)));
            
            return fakeObjects.ToArray();
        }

    }
}
