/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Independent.XmlFileContent;
using System.IO;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Windows.Probes.Metabase
{
    [TestClass]
    public class MetabaseObjectCollectorTests
    {
        private IEnumerable<string> FakeMetabaseContent;
        private IEnumerable<string> FakeMetabaseWithIncompleteSessionContent;
        private IEnumerable<string> FakeMBSchemaContent;

        public MetabaseObjectCollectorTests()
        {
            var documentLoader = new FileContentLoader();
            this.FakeMetabaseContent = documentLoader.ReadAllFileLinesFromResource("metabaseSample.xml");
            this.FakeMetabaseWithIncompleteSessionContent = documentLoader.ReadAllFileLinesFromResource("metabaseSampleWithIncompleteSessions.xml");
            this.FakeMBSchemaContent = documentLoader.ReadAllFileLinesFromResource("mbSchemaSample.xml");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_metabase_item()
        {
            var fakeMetabaseItemToCollect = CreateMetabaseItemSample("/LM/IISADMIN/EXTENSIONS/DCOMCLSIDS", "1028");
            
            var collectedItems =
                CreateMetabaseObjectCollectorWithBehavior(this.FakeMetabaseContent)
                    .CollectDataForSystemItem(fakeMetabaseItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(metabase_item), true);
            var collectedMetabaseItem = (metabase_item)collectedItems.First().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedMetabaseItem.id1, "1028");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedMetabaseItem.key, "/LM/IISADMIN/EXTENSIONS/DCOMCLSIDS");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedMetabaseItem.name, "MD_IISADMIN_EXTENSIONS");
            Assert.AreEqual(1, collectedMetabaseItem.data.Count());
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedMetabaseItem.data.First(), "{61738644-F196-11D0-9953-00C04FD919C1}");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedMetabaseItem.data_type , "MULTISZ");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedMetabaseItem.user_type, "IIS_MD_UT_SERVER");
        }

        [TestMethod, Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_metabase_item_even_if_some_attributes_werent_present()
        {
            var fakeMetabaseItemToCollect = CreateMetabaseItemSample("/LM/W3SVC", "9202");

            var collectedItems =
                CreateMetabaseObjectCollectorWithBehavior(this.FakeMetabaseWithIncompleteSessionContent)
                    .CollectDataForSystemItem(fakeMetabaseItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(metabase_item), true);
            var collectedMetabaseItem = (metabase_item)collectedItems.First().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedMetabaseItem.data.Single(), "4294967295");
            // Name Entity
            var nameEntity = collectedMetabaseItem.name;
            Assert.IsNotNull(nameEntity, "The name entity cannot be null.");
            Assert.AreEqual(StatusEnumeration.exists, nameEntity.status, "The name entity status must be 'exists'.");
            Assert.AreEqual(string.Empty, nameEntity.Value, "The name entity value must be an empty string.");
           // Data Type Entity
            var dataTypeEntity = collectedMetabaseItem.data_type;
            Assert.IsNotNull(dataTypeEntity, "The data type entity cannot be null.");
            Assert.AreEqual(StatusEnumeration.doesnotexist, dataTypeEntity.status, "The data type entity status must be 'does not exist'.");
            Assert.IsNull(dataTypeEntity.Value, "The data type entity value must be null.");
            // User Type Entity
            var userTypeEntity = collectedMetabaseItem.user_type;
            Assert.IsNotNull(userTypeEntity, "The user type entity cannot be null.");
            Assert.AreEqual(StatusEnumeration.doesnotexist, userTypeEntity.status, "The user type entity status must be 'does not exist'.");
            Assert.IsNull(userTypeEntity.Value, "The user type entity value must be null.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_metabase_item_with_multiple_string_type()
        {
            var fakeMetabaseItemToCollect = CreateMetabaseItemSample("/LM/IISADMIN/PROPERTYREGISTRATION", "1030");

            var collectedItems =
                CreateMetabaseObjectCollectorWithBehavior(this.FakeMetabaseWithIncompleteSessionContent)
                    .CollectDataForSystemItem(fakeMetabaseItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(metabase_item), true);
            var collectedDataEntity = ((metabase_item)collectedItems.First().ItemType).data;
            Assert.AreEqual(3, collectedDataEntity.Count(), "Unexpected amount of data entity was found.");
            Assert.AreEqual("0-65535", collectedDataEntity.ElementAt(0).Value);
            Assert.AreEqual("Microsoft Reserved\r\n\t\t\t65536-524288", collectedDataEntity.ElementAt(1).Value);
            Assert.AreEqual("Microsoft IIS Admin Objects Reserved", collectedDataEntity.ElementAt(2).Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_there_is_no_key__the_status_of_collected_item_must_be_doesNotExist()
        {
            var fakeMetabaseItemToCollect = CreateMetabaseItemSample("INVALID_LOCATION", "1030");

            var collectedItems =
                CreateMetabaseObjectCollectorWithBehavior(this.FakeMetabaseWithIncompleteSessionContent)
                    .CollectDataForSystemItem(fakeMetabaseItemToCollect);

            var collectedMetabaseItem = (metabase_item)collectedItems.First().ItemType;
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedMetabaseItem.status);
            Assert.AreEqual("1030", collectedMetabaseItem.id1.Value);
            Assert.AreEqual("INVALID_LOCATION", collectedMetabaseItem.key.Value);
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedMetabaseItem.key.status);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_a_metabase_property_hierarchically_for_websites()
        {
            var fakeMetabaseItemToCollect = CreateMetabaseItemSample("/LM/W3SVC/1", "1013");

            var collectedItems =
                CreateMetabaseObjectCollectorWithBehavior(this.FakeMetabaseContent)
                    .CollectDataForSystemItem(fakeMetabaseItemToCollect);

            var collectedMetabaseItem = (metabase_item)collectedItems.First().ItemType;
            Assert.AreEqual(StatusEnumeration.exists, collectedMetabaseItem.status);
            Assert.AreEqual("/LM/W3SVC/1", collectedMetabaseItem.key.Value);
            Assert.AreEqual("1013", collectedMetabaseItem.id1.Value);
            Assert.AreEqual("120", collectedMetabaseItem.data.First().Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_a_metabase_property_hierarchically_for_filters()
        {
            var GZIP_FILTER_LOCATION = "/LM/W3SVC/Filters/Compression/gzip";
            var FILTER_LOAD_ORDER_ID = "2040";

            var collectedItems =
                CreateMetabaseObjectCollectorWithBehavior(FakeMetabaseContent)
                    .CollectDataForSystemItem(
                        CreateMetabaseItemSample(GZIP_FILTER_LOCATION, FILTER_LOAD_ORDER_ID));

            var collectedMetabaseItem = (metabase_item)collectedItems.First().ItemType;
            Assert.AreEqual(StatusEnumeration.exists, collectedMetabaseItem.status);
            Assert.AreEqual("URLSCAN", collectedMetabaseItem.data.First().Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_a_metabase_property_hierarchically_for_applicationPools()
        {
            var DEFAULT_APPPOOLS_LOCATION = "/LM/W3SVC/AppPools/DefaultAppPool";
            var PERIODIC_RESTART_MEMORY_ID = "9024";
            var PING_INTERVAL_ID = "9013";
            var metabaseObjectCollector = CreateMetabaseObjectCollectorWithBehavior(FakeMetabaseContent);

            {
                var collectedItems =
                    metabaseObjectCollector
                        .CollectDataForSystemItem(
                            CreateMetabaseItemSample(DEFAULT_APPPOOLS_LOCATION, PERIODIC_RESTART_MEMORY_ID));
                Assert.AreEqual("512000", ((metabase_item)collectedItems.First().ItemType).data.First().Value);
            }
            {
                var collectedItems =
                    metabaseObjectCollector
                        .CollectDataForSystemItem(
                            CreateMetabaseItemSample(DEFAULT_APPPOOLS_LOCATION, PING_INTERVAL_ID));
                Assert.AreEqual("30", ((metabase_item)collectedItems.First().ItemType).data.First().Value);
            }
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_all_locations_from_metabase_file()
        {
            var objectCollector = CreateMetabaseObjectCollectorWithBehavior(this.FakeMetabaseContent);

            var allMetabaseKeys = objectCollector.GetAllMetabaseKeys();

            Assert.IsNotNull(allMetabaseKeys);
            Assert.AreEqual(57, allMetabaseKeys.Count());
            Assert.AreEqual(".", allMetabaseKeys.First());
            Assert.AreEqual("/LM/W3SVC/Info/Templates/Secure Web Site/Root", allMetabaseKeys.Last());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_all_IDs_from_metabase_file()
        {
            var objectCollector = CreateMetabaseObjectCollectorWithBehavior(this.FakeMetabaseContent);

            var allMetabaseIDs = objectCollector.GetAllMetabaseIDs();

            Assert.IsNotNull(allMetabaseIDs);
            Assert.AreEqual(591, allMetabaseIDs.Count());
        }



        private MetabaseObjectCollector CreateMetabaseObjectCollectorWithBehavior(IEnumerable<String> fakeMetabaseLines)
        {
            var mocks = new MockRepository();
            var fakeWindowsFileProvider = mocks.DynamicMock<WindowsFileProvider>(new object[] { null });
            Expect.Call(
                fakeWindowsFileProvider
                    .GetFileLinesContentFromHost(@"c:\windows\system32\inetsrv\metabase.xml"))
                    .Return(fakeMetabaseLines.ToArray());
            Expect.Call(
                fakeWindowsFileProvider
                    .GetFileLinesContentFromHost(@"c:\windows\system32\inetsrv\mbschema.xml"))
                    .Return(FakeMBSchemaContent.ToArray());

            mocks.ReplayAll();

            return new MetabaseObjectCollector(fakeWindowsFileProvider);
        }



        private metabase_item CreateMetabaseItemSample(string keyEntityValue, string idEntityValue)
        {
            return new metabase_item()
            {
                key = new EntityItemStringType() { Value = keyEntityValue },
                id1 = new EntityItemIntType() { Value = idEntityValue }
            };
        }
    }
}
