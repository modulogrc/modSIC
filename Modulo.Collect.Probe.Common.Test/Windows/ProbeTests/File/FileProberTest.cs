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
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.Test.Helpers;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Definitions = Modulo.Collect.OVAL.Definitions;
using SystemCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Windows.Test.File
{
    [TestClass]
    public class FileProberTest
    {

        private string FILE_SIZE = GeneratedFileItemAttributes.FileSize.ToString();
        private string C_DATE = GeneratedFileItemAttributes.CreationDate.ToString();
        private string A_DATE = GeneratedFileItemAttributes.LastAccessed.ToString();
        private string M_DATE = GeneratedFileItemAttributes.LastModified.ToString();
        private string VERSION = GeneratedFileItemAttributes.Version.ToString();
        private string FILE_TYPE = GeneratedFileItemAttributes.FileType.ToString();
        private string COMPANY = GeneratedFileItemAttributes.Manufacturer.ToString();

        private string INVALID_FILEITEM_FORMAT_ERROR_MSG = "[Unexpected Item Type Entity]: Once the <file_object> was defined using the {0}, {1} should not exist.";
        private string FILEITEM_ENTITY_NOT_FOUND = "{0} file item entity was not found";
        private List<IConnectionProvider> FakeContext;
        private TargetInfo FakeTarget;


        public FileProberTest()
        {
            FakeContext = ProbeHelper.CreateFakeContext();
            FakeTarget = ProbeHelper.CreateFakeTarget();
        }


        /// <summary>
        /// It tests a same file defined two different ways as follows:
        ///
        /// 1. Using only "filepath" entity.
        /// <file_object>
        ///     <filepath>c:\temp\file.ext</path>
        /// </file_object>
        /// 2. Using "path" and "filename" entity
        /// <file_object>
        ///     <path>c:\temp</path>
        ///     <filename>file.ext</filename>
        /// </file_object>
        /// 
        /// </summary>
        /// </summary>
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_execute_a_simple_file_collect_defined_with_one_entity()
        {
            // Arrange
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1");
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new Definitions.ObjectType[] { objectType }, null, null);
            var fakeWmiObjects = new WmiObject[] { this.createFakeWmiObject() };
            var fakeFileItem = this.CreateFakeFileItem("c:\\temp\\file.txt", null, null);
            var fileProber = this.CreateMockedFileProber(fakeWmiObjects, new ItemType[] { fakeFileItem }, null);
            
            // Act
            var collectedObjects = (List<CollectedObject>)fileProber.Execute(FakeContext, FakeTarget, fakeCollectInfo).CollectedObjects;

            // Assert
            Assert.IsNotNull(collectedObjects, "The result of file collect cannot be null");
            Assert.IsNotNull(collectedObjects.Single(), string.Format("The object '{0}' was not found.", "oval:modulo:obj:1"));
            this.AssertCollectedFileItems(collectedObjects.Single(), fakeWmiObjects);
            this.AssertFilePathEntityExistence((file_item)collectedObjects.Single().SystemData[0]);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_FileObject_that_contains_a_referenced_variable()
        {
            // Arrange
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:3");
            var variables = VariableHelper.CreateVariableWithOneValue("oval:modulo:obj:3", "oval:modulo:var:3", "c:\\temp");
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new Definitions.ObjectType[] { objectType }, variables, null);
            var fakeWmiObjects = new WmiObject[] { this.createFakeWmiObject() };
            var fakeFileItems = new ItemType[] { this.CreateFakeFileItem(null, "c:\\temp", "file1.txt") };
            var fileProber = this.CreateMockedFileProber(fakeWmiObjects, fakeFileItems, null);

            // Act
            var collectedObjects = (List<CollectedObject>)fileProber.Execute(FakeContext, FakeTarget, fakeCollectInfo).CollectedObjects;
            
            // Assert
            Assert.IsNotNull(collectedObjects, "The result of file collect cannot be null");
            Assert.IsNotNull(collectedObjects.Single(), string.Format("There is no collected object."));
            this.AssertCollectedFileItems(collectedObjects.Single(), fakeWmiObjects);
            
            var collectedItem = (file_item)collectedObjects.Single().SystemData[0];
            this.AssertPathAndFileNameEntitiesExistence(collectedItem);
            Assert.AreEqual("c:\\temp", collectedItem.path.Value, "The variable value was not set to file item entity.");
        }

        [TestMethod, Owner("lfernandes")]
        public void When_some_error_occurred_during_item_type_generation_a_item_with_status_equals_to_error_must_be_returned()
        {
            var fakeObjectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:3");
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new Definitions.ObjectType[] { fakeObjectType }, null, null);
            var fileProber = this.CreateMockedFileProber(null, null, new Exception("Test Exception"));

            var collectedObjects = fileProber.Execute(FakeContext, FakeTarget, fakeCollectInfo);

            WindowsTestHelper.AssertItemWithErrorStatus(collectedObjects.CollectedObjects.First().SystemData, typeof(file_item), "Test Exception");
        }


        private WmiObject createFakeWmiObject()
        {
            WmiObject fakeWmiObject = new WmiObject();
            fakeWmiObject.Add(FILE_SIZE, 50);
            fakeWmiObject.Add(C_DATE, "20081217165939.013961 - 120");
            fakeWmiObject.Add(M_DATE, "20090116171121.673899 - 120");
            fakeWmiObject.Add(A_DATE, "20081217171525.895263 - 120");
            fakeWmiObject.Add(VERSION, "1");
            fakeWmiObject.Add(FILE_TYPE, "Text Document");
            fakeWmiObject.Add(COMPANY, "Modulo");
            return fakeWmiObject;
        }

        private file_item CreateFakeFileItem(string filepath, string path, string filename)
        {
            if (!string.IsNullOrEmpty(filepath))
            {
                return new file_item()
                {
                    filepath = new EntityItemStringType() { Value = filepath }
                };
            }
            else
            {
                return new file_item()
                {
                    path = OvalHelper.CreateItemEntityWithStringValue(path),
                    filename = OvalHelper.CreateItemEntityWithStringValue(filename)
                };
            }
        }

        private FileProber CreateMockedFileProber(IEnumerable<WmiObject> fakeWmiObjects, ItemType[] fakeItems, Exception exceptionToThrow)
        {
            MockRepository mocks = new MockRepository();
            var fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();
            var fakeSysInfoService = mocks.DynamicMock<ISystemInformationService>();
            var fakeFileConnectionProvider = mocks.DynamicMock<FileConnectionProvider>();
            var fakeWmiProvider = mocks.DynamicMock<WmiDataProvider>();
            var fakeItemTypeGenerator = mocks.DynamicMock<IItemTypeGenerator>();
            

            if (exceptionToThrow == null)
                Expect.Call(fakeWmiProvider.SearchWmiObjects(null, null)).IgnoreArguments().Repeat.Any().Return(fakeWmiObjects);
            else
                Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null)).IgnoreArguments().Throw(exceptionToThrow);

            Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null)).IgnoreArguments().Return(fakeItems);
            
            mocks.ReplayAll();

            FileObjectCollector fileSysDataSource = new FileObjectCollector() { WmiDataProvider = fakeWmiProvider };
            return new FileProber() { ConnectionManager = fakeConnectionManager, ObjectCollector = fileSysDataSource, ItemTypeGenerator = fakeItemTypeGenerator };
        }

        private void AssertCollectedFileItems(CollectedObject collectedObject, WmiObject[] expectedFiles)
        {
            string UNEXPECTED_ITEM_ATTRIBUTE_VALUE = "Unexpected value for a file_item attribute was found: {0}";
            SystemCharacteristics::ReferenceType[] objectReferences = collectedObject.ObjectType.reference.ToArray();
            IList<ItemType> fileItems = (IList<ItemType>)collectedObject.SystemData;

            Assert.AreEqual(expectedFiles.Count(), objectReferences.Count(), "Unexpected number of item references was found.");
            Assert.AreEqual(objectReferences.Count(), fileItems.Count, "Unexpected number of generated items type was found.");

            int i = 0;
            foreach (var expectedFile in expectedFiles)
            {
                Dictionary<string, object> fileItemFields = expectedFile.GetValues();
                Assert.IsInstanceOfType(fileItems[i], typeof(file_item), "The generated ItemType must be a instance of file_item class.");

                file_item fileItem = (file_item)fileItems[i];
                Assert.AreEqual(objectReferences[i].item_ref, fileItem.id, "The generated ItemType ID must be equal to collected object ID.");
                Assert.AreEqual(StatusEnumeration.exists, fileItem.status, "A generated ItemType with unexpected OVAL Status was found.");
                Assert.AreEqual(fileItemFields[FILE_SIZE].ToString(), fileItem.size.Value, string.Format(UNEXPECTED_ITEM_ATTRIBUTE_VALUE, FILE_SIZE));
                Assert.AreEqual(ConvertWmiTimeToFileTime(fileItemFields[C_DATE].ToString()), fileItem.c_time.Value, string.Format(UNEXPECTED_ITEM_ATTRIBUTE_VALUE, C_DATE));
                Assert.AreEqual(ConvertWmiTimeToFileTime(fileItemFields[M_DATE].ToString()), fileItem.m_time.Value, string.Format(UNEXPECTED_ITEM_ATTRIBUTE_VALUE, M_DATE));
                Assert.AreEqual(ConvertWmiTimeToFileTime(fileItemFields[A_DATE].ToString()), fileItem.a_time.Value, string.Format(UNEXPECTED_ITEM_ATTRIBUTE_VALUE, A_DATE));
                Assert.AreEqual(fileItemFields[VERSION], fileItem.version.Value, string.Format(UNEXPECTED_ITEM_ATTRIBUTE_VALUE, VERSION));
                Assert.AreEqual(fileItemFields[FILE_TYPE], fileItem.type.Value, string.Format(UNEXPECTED_ITEM_ATTRIBUTE_VALUE, FILE_SIZE));
                Assert.AreEqual(fileItemFields[COMPANY], fileItem.company.Value, string.Format(UNEXPECTED_ITEM_ATTRIBUTE_VALUE, COMPANY));

                i++;
            }
        }

        private void AssertFilePathEntityExistence(file_item fileItem)
        {
            Assert.IsTrue(IsItemEntityDefined(fileItem.filepath), string.Format(FILEITEM_ENTITY_NOT_FOUND, "<filepath>"));
            Assert.IsFalse(IsItemEntityDefined(fileItem.path), string.Format(INVALID_FILEITEM_FORMAT_ERROR_MSG, "<filepath>", "<path>"));
            Assert.IsFalse(IsItemEntityDefined(fileItem.filename), string.Format(INVALID_FILEITEM_FORMAT_ERROR_MSG, "<filepath>", "<filename>"));
        }

        private void AssertPathAndFileNameEntitiesExistence(file_item fileItem)
        {
            Assert.IsFalse(IsItemEntityDefined(fileItem.filepath), string.Format(INVALID_FILEITEM_FORMAT_ERROR_MSG, "<path> and <filename>", "<filepath>"));
            Assert.IsTrue(IsItemEntityDefined(fileItem.path), string.Format(FILEITEM_ENTITY_NOT_FOUND, "<filepath>"));
            Assert.IsTrue(IsItemEntityDefined(fileItem.filename), string.Format(FILEITEM_ENTITY_NOT_FOUND, "<filename>"));
        }

        private bool IsItemEntityDefined(EntityItemSimpleBaseType itemEntity)
        {
            return ((itemEntity != null) && (!string.IsNullOrEmpty(itemEntity.Value)));
        }

        private string ConvertWmiTimeToFileTime(string wmiFormattedDateTime)
        {
            if (wmiFormattedDateTime == null)
                return "-1";

            DateTime parsedDateTime = DateTime.ParseExact(wmiFormattedDateTime.ToString().Substring(0, 21), "yyyyMMddHHmmss.ffffff", CultureInfo.InvariantCulture);
            return parsedDateTime.ToFileTime().ToString();
        }
    }
}
