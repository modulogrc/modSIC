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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.FileEffectiveRights53;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Windows.File;

namespace Modulo.Collect.Probe.Windows.Test.FileEffectiveRights53
{
    [TestClass]
    public class FileEffectiveRights53EntityOperationEvaluatorTest
    {
        private const string UNEXPECTED_ENTITY_VALUE_FOUND = "An unexpected value was found for entity: '{0}'.";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_Equal_operation_on_file_entities_of_fileeffectiverights53()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", ConstantHelper.OBJECT_ID_1000);
            var operationEvaluator = this.CreateMockedOperationEvaluator(null, null);
            
            IEnumerable<ItemType> result = operationEvaluator.ProcessOperation((fileeffectiverights53_object)objectType);
            this.DoBasicAssertForOperationProcessing(result, 1);
            this.AssertCreatedItemTypeAgainstSourceObjectType((fileeffectiverights53_object)objectType, (fileeffectiverights_item)result.First());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_PatternMatchOperation_on_FilenameEntity()
        {
            var expectedTrusteeSID =  "S-1-15-500";
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", ConstantHelper.OBJECT_ID_1060);
            var fakeFilenames = new string[] { "file1.ext", "sample.doc", "file1.txt", "readme.txt", "win.ini", "disclaimer.txt" };
            var operationEvaluator = this.CreateMockedOperationEvaluator(fakeFilenames, null);
            
            IEnumerable<ItemType> result = operationEvaluator.ProcessOperation((fileeffectiverights53_object)objectType);
            this.DoBasicAssertForOperationProcessing(result, 3);
            this.AssertCreatedItemType((fileeffectiverights_item)result.ElementAt(0), @"c:\temp\file1.txt", expectedTrusteeSID);
            this.AssertCreatedItemType((fileeffectiverights_item)result.ElementAt(1), @"c:\temp\readme.txt", expectedTrusteeSID);
            this.AssertCreatedItemType((fileeffectiverights_item)result.ElementAt(2), @"c:\temp\disclaimer.txt", expectedTrusteeSID);
            //this.AssertCreatedItemType((fileeffectiverights_item)result.ElementAt(0), @"c:\temp\file1.txt", "S-1-15-500");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_PatternMatchOperation_on_FilepathAndTrusteeSID_entities()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", ConstantHelper.OBJECT_ID_1070);
            var fakeFiles = new string[] { @"c:\windows\win.ini", @"c:\windows\file1.txt", @"c:\windows\readme.txt", @"c:\windows\system.ini" };
            var fakeSIDs = new string[] { "S-0-0", "S-1-15-18", "S-1-15-500", "S-1-1-15" };
            var operationEvaluator = this.CreateMockedOperationEvaluator(fakeFiles, fakeSIDs);

            var result = operationEvaluator.ProcessOperation((fileeffectiverights53_object)objectType);

            this.DoBasicAssertForOperationProcessing(result, 4);
            this.AssertCreatedItemType((fileeffectiverights_item)result.ElementAt(0), @"c:\windows\win.ini", "S-1-15-18");
            this.AssertCreatedItemType((fileeffectiverights_item)result.ElementAt(1), @"c:\windows\win.ini", "S-1-15-500");
            this.AssertCreatedItemType((fileeffectiverights_item)result.ElementAt(2), @"c:\windows\system.ini", "S-1-15-18");
            this.AssertCreatedItemType((fileeffectiverights_item)result.ElementAt(3), @"c:\windows\system.ini", "S-1-15-500");
        }


        private void DoBasicAssertForOperationProcessing(IEnumerable<ItemType> resultOfOperationProcessing, int expectedResultCount)
        {
            Assert.IsNotNull(resultOfOperationProcessing, "The result of operation processing cannot be null");
            Assert.AreEqual(expectedResultCount, resultOfOperationProcessing.Count(), "Unexpected result count was found after operation processing");
        }

        private void AssertCreatedItemTypeAgainstSourceObjectType(
            fileeffectiverights53_object sourceObjectType, fileeffectiverights_item createdItemType)
        {
            var sourceFilePath = sourceObjectType.GetAllObjectEntities()[fileeffectiverights53_object_ItemsChoices.filepath.ToString()];
            var sourcePath = sourceObjectType.GetAllObjectEntities()[fileeffectiverights53_object_ItemsChoices.path.ToString()];
            var sourceFileName = sourceObjectType.GetAllObjectEntities()[fileeffectiverights53_object_ItemsChoices.filename.ToString()];
            var sourceTrusteeSID = sourceObjectType.GetAllObjectEntities()[fileeffectiverights53_object_ItemsChoices.trustee_sid.ToString()];

            this.assertGeneratedEntityItem(createdItemType.filepath, sourceFilePath, "filepath");
            this.assertGeneratedEntityItem(createdItemType.path, sourcePath, "path");
            this.assertGeneratedEntityItem(createdItemType.filename, sourceFileName, "filename");
            this.assertGeneratedEntityItem(createdItemType.trustee_sid, sourceTrusteeSID, "trusteeSID");
        }

        private void AssertCreatedItemType(fileeffectiverights_item itemToAssert, string filepath, string trusteeSID)
        {
            var path = System.IO.Path.GetDirectoryName(filepath);
            var filename = System.IO.Path.GetFileName(filepath);

            Assert.AreEqual(filepath, itemToAssert.filepath.Value, string.Format(UNEXPECTED_ENTITY_VALUE_FOUND, "filepath"));
            Assert.AreEqual(path, itemToAssert.path.Value, string.Format(UNEXPECTED_ENTITY_VALUE_FOUND, "path"));
            Assert.AreEqual(filename, itemToAssert.filename.Value, string.Format(UNEXPECTED_ENTITY_VALUE_FOUND, "filename"));
            Assert.AreEqual(trusteeSID, itemToAssert.trustee_sid.Value, string.Format(UNEXPECTED_ENTITY_VALUE_FOUND, "trustee_sid"));
        }

        private void assertGeneratedEntityItem(EntityItemStringType createdEntity, EntitySimpleBaseType sourceEntity, string entityName)
        {
            if (sourceEntity == null)
                return;

            Assert.AreEqual(sourceEntity.Value, createdEntity.Value, string.Format(UNEXPECTED_ENTITY_VALUE_FOUND, entityName));
        }

        private FileEffectiveRights53EntityOperationEvaluator CreateMockedOperationEvaluator(
            IList<String> fakeFilePathsToReturn, IList<String> fakeSIDsToReturn)
        {
            MockRepository mocks = new MockRepository();
            var fakeSystemDataSource = mocks.DynamicMock<FileEffectiveRightsObjectCollector>();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();

            Expect.Call(fakeFileProvider.GetFileChildren(null))
                .IgnoreArguments()
                    .Return(fakeFilePathsToReturn);
            Expect.Call(fakeSystemDataSource.GetValues(null))
                .IgnoreArguments()
                    .Return(fakeSIDsToReturn);
            Expect.Call(fakeSystemDataSource.IsThereUserACLInFileSecurityDescriptor(null, null, null))
                .IgnoreArguments().Repeat.Any()
                    .Return(true);
            
            mocks.ReplayAll();
            
            return 
                new FileEffectiveRights53EntityOperationEvaluator(
                    fakeSystemDataSource, fakeFileProvider);
        }
    }
}
