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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.File;

using Rhino.Mocks;

using Definitions = Modulo.Collect.OVAL.Definitions;
using SystemCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.Windows;


namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass]
    public class FileEffectiveRightsProberTest
    {

        private const string MSG_UNEXPECTED_COLLECTED_ITEM_VALUE = "Unexpected collected item value: '{0}'";
        private const string MSG_UNEXPECTED_COLLECTED_ENTITY_VALUE = "Unexpected entity value for collected file item: 'fileeffectiverights_item.{0}'";

        [Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_collect_file_effective_rights()
        {
            #region File Effective Rights Object
            //<fileeffectiverights_object id="oval:modulo:obj:7" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //    <path>c:\temp</path>
            //    <filename>file1.txt</filename>
            //    <trustee_name>mss\lfernandes</trustee_name>
            //</fileeffectiverights_object>
            #endregion

            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            CollectInfo fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(definitions.objects, null, null);
            Definitions::ObjectType fileEffectiveRightsObject = ProbeHelper.GetOvalComponentByOvalID(definitions, "oval:modulo:obj:7");
            IEnumerable<CollectedItem> fakeCollectedItems = new CollectedItem[] { this.createFakeCollectedItem(fileEffectiveRightsObject) };

            FileEffectiveRightsProber prober = this.createMockedFileEffectiveRightsProber(fakeCollectedItems);
            ProbeResult result = prober.Execute(ProbeHelper.CreateFakeContext(), ProbeHelper.CreateFakeTarget(), fakeCollectInfo);

            Assert.IsNotNull(result, "The result of FileEffectiveRightsProber execution cannot be null.");
            Assert.IsNotNull(result.ExecutionLog, "The ExecutionLog of FileEffectiveRightsProber was not created.");
            Assert.IsNotNull(result.CollectedObjects, "There are no collected objects.");
            Assert.AreEqual(1, result.CollectedObjects.Count(), "Unexpected collected objects count found.");

            this.AssertCollectedFileItems(fileEffectiveRightsObject, result.CollectedObjects.ElementAt(0), fakeCollectedItems);
        }

        private void AssertCollectedFileItems(Definitions::ObjectType sourceObject, CollectedObject collectedObject, IEnumerable<CollectedItem> expectedItems)
        {
            SystemCharacteristics.ReferenceType[] objectReferences = collectedObject.ObjectType.reference.ToArray();
            IList<ItemType> collectedItems = (IList<ItemType>)collectedObject.SystemData;
            
            this.assertCollectedItemsReferences(collectedObject, collectedItems);

            for (int i = 0; i < expectedItems.Count(); i++)
            {
                CollectedItem expectedFileItem = expectedItems.ElementAt(i);
                ItemType collectedItem = collectedItems[i];

                this.assertCollectedItemStatus(objectReferences[i], collectedItem);
                this.assertFileEffectiveRightsInitialEntities(sourceObject, collectedItem);
                this.assertFileEffectiveRightsCollectedItems(expectedFileItem, collectedItem);
            }
        }

        private void assertFileEffectiveRightsCollectedItems(CollectedItem expectedItem, ItemType collectedItem)
        {
            fileeffectiverights_item expectedFileItem = (fileeffectiverights_item)expectedItem.ItemType;
            fileeffectiverights_item collectedFileItem = (fileeffectiverights_item)collectedItem;

            Assert.AreEqual(expectedFileItem.trustee_sid.Value, collectedFileItem.trustee_sid.Value, string.Format(MSG_UNEXPECTED_COLLECTED_ENTITY_VALUE, "trustee_sid"));
        }

        private void assertCollectedItemsReferences(CollectedObject collectedObject, IList<ItemType> collectedItems)
        {
            int collectedObjectCount = collectedObject.ObjectType.reference.Count();

            Assert.AreEqual(collectedItems.Count(), collectedObjectCount, "Unexpected number of item references was found.");
            Assert.AreEqual(collectedObjectCount, collectedItems.Count, "Unexpected number of generated items type was found.");
        }

        private void assertCollectedItemStatus(SystemCharacteristics.ReferenceType objectReference, SystemCharacteristics.ItemType collectedItem)
        {
            Assert.IsInstanceOfType(collectedItem, typeof(fileeffectiverights_item), "The generated ItemType must be a instance of fileeffectiverights_item class.");
            Assert.AreEqual(objectReference.item_ref, collectedItem.id, "The generated ItemType ID must be equal to collected object ID.");
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status, "A generated ItemType with unexpected OVAL Status was found.");
        }

        private void assertFileEffectiveRightsInitialEntities(Definitions.ObjectType sourceObject, SystemCharacteristics.ItemType collectedItem)
        {
            fileeffectiverights_item collectedFileEffectiveRightsItem = (fileeffectiverights_item)collectedItem;   
            
            var allExpectedEntities = OvalHelper.GetFileEffectiveRightsFromObjectType((fileeffectiverights_object)sourceObject);
            
            string expectedPath = allExpectedEntities[fileeffectiverights_object_ItemsChoices.path.ToString()].Value;
            string expectedFileName = allExpectedEntities[fileeffectiverights_object_ItemsChoices.filename.ToString()].Value;
            string expectedTrusteeName = allExpectedEntities[fileeffectiverights_object_ItemsChoices.trustee_name.ToString()].Value;

            Assert.AreEqual(expectedPath, collectedFileEffectiveRightsItem.path.Value, string.Format(MSG_UNEXPECTED_COLLECTED_ITEM_VALUE, "path"));
            Assert.AreEqual(expectedFileName, collectedFileEffectiveRightsItem.filename.Value, string.Format(MSG_UNEXPECTED_COLLECTED_ITEM_VALUE, "filename"));
            Assert.AreEqual(expectedTrusteeName, collectedFileEffectiveRightsItem.trustee_name.Value, string.Format(MSG_UNEXPECTED_COLLECTED_ITEM_VALUE, "trustee_name"));
        }


        private CollectedItem createFakeCollectedItem(Definitions::ObjectType fileEffectiveRightsObject)
        {
            var allEntities = OvalHelper.GetFileEffectiveRightsFromObjectType((fileeffectiverights_object)fileEffectiveRightsObject);
            
            fileeffectiverights_item newItem = new fileeffectiverights_item()
            {
                id = fileEffectiveRightsObject.id,
                path = this.createInitialItemEntity(allEntities, fileeffectiverights_object_ItemsChoices.path.ToString()),
                filename = this.createInitialItemEntity(allEntities, fileeffectiverights_object_ItemsChoices.filename.ToString()),
                trustee_name = this.createInitialItemEntity(allEntities, fileeffectiverights_object_ItemsChoices.trustee_name.ToString()),
                trustee_sid = this.createItemEntityWithStringValue("12345"),
                standard_delete = this.createItemEntityWithBooleanValue("1"),
                generic_read = this.createItemEntityWithBooleanValue("0")
            };

            return ProbeHelper.CreateFakeCollectedItem(newItem);
        }

        private EntityItemStringType createInitialItemEntity(Dictionary<string, EntityObjectStringType> allEntities, string entityName)
        {
            return new EntityItemStringType { Value = allEntities[entityName].Value };
        }

        private EntityItemStringType createItemEntityWithStringValue(string entityValue)
        {
            return new EntityItemStringType { Value = entityValue };
        }

        private EntityItemBoolType createItemEntityWithBooleanValue(string entityValue)
        {
            return new EntityItemBoolType { Value = entityValue };
        }

        private FileEffectiveRightsProber createMockedFileEffectiveRightsProber(IEnumerable<CollectedItem> fakeCollectedItems)
        {
            MockRepository mocks = new MockRepository();
            IConnectionManager fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();
            ISystemInformationService fakeSysInfoService = mocks.DynamicMock<ISystemInformationService>();
            FileConnectionProvider fakeProvider = mocks.DynamicMock<FileConnectionProvider>();
            FileEffectiveRightsObjectCollector fakeSystemDataSource = mocks.DynamicMock<FileEffectiveRightsObjectCollector>();
            Expect.Call(fakeSystemDataSource.CollectDataForSystemItem(null)).IgnoreArguments().Return(fakeCollectedItems);
            mocks.ReplayAll();
            
            return new FileEffectiveRightsProber() { ConnectionManager = fakeConnectionManager, ObjectCollector = fakeSystemDataSource };
        }
    }
}
