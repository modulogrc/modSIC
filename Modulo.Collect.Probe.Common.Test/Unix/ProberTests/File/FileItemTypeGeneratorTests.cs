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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Unix.File;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Rhino.Mocks;
using Modulo.Collect.Probe.Unix.TextFileContent54;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.OVAL.Definitions.Unix;


namespace Modulo.Collect.Probe.Unix.Test.ProberTests.File
{
    [TestClass]
    public class FileItemTypeGeneratorTests
    {
        private Type TypeOfUnixFileItem = typeof(OVAL.SystemCharacteristics.Unix.file_item);

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_file_items_to_collect_unix_systems()
        {
            //<file_object id="oval:modulo:obj:2" version="1" comment="..." xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#unix">
            //  <path>home</path>
            //  <filename>file1</filename>
            //</file_object>
            var fileObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "2");

            var itemsToCollect = 
                CreateFileItemTypeGeneratorWithDefaultBehavior()
                    .GetItemsToCollect(fileObject, null).ToArray();

            AssertFileItem(itemsToCollect, "/home/", "file1", true);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_file_items_to_collect_unix_systems_from_an_file_object_with_only_filepath_entity_defined()
        {
            var fileObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "2000");

            var itemsToCollect =
                CreateFileItemTypeGeneratorWithDefaultBehavior()
                    .GetItemsToCollect(fileObject, null).ToArray();

            AssertFileItem(itemsToCollect, "/home/Desktop/", "file1", false);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_an_file_object_with_filename_entity_nil()
        {
            var fileObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "2001");
            
            var itemsToCollect =
                CreateFileItemTypeGeneratorWithDefaultBehavior()
                    .GetItemsToCollect(fileObject, null).ToArray();

            AssertFileItem(itemsToCollect, "/home/Desktop", null, true);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_an_file_object_with_pattern_match_operation_in_path_entity()
        {
            //<file_object id="oval:gov.irs.rhel5:obj:33">
            //    <path operation="pattern match">/lib/modules/.*/kernel/drivers/usb/storage</path>
            //    <filename>usb-storage.ko</filename>
            //</file_object>
            var expectedFilepath1 = "lib/modules/2.6.18-128.1.10.el5.xs5.5.0.51xen/kernel/drivers/usb/storage";
            var expectedFilepath2 = "lib/modules/2.6.18-128.el5/kernel/drivers/usb/storage";

            var fileObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "oval:gov.irs.rhel5:obj:33");
            var fileItemTypeGenerator = CreateFileItemTypeGeneratorWithMockedBehaviorToProcessPatternMatchOperation();
            
            var itemsToCollect = fileItemTypeGenerator.GetItemsToCollect(fileObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 2, TypeOfUnixFileItem);
            AssertFileItem(new ItemType[] { itemsToCollect[0] }, expectedFilepath1, string.Empty, true);
            AssertFileItem(new ItemType[] { itemsToCollect[1] }, expectedFilepath2, string.Empty, true);
        }

        [TestMethod, Owner("lfernandes")]
        public void The_result_of_item_type_generation_should_contains_only_file_that_exists_on_target()
        {
            // <path operation="pattern match">/lib/modules/.*/kernel/drivers/usb/storage</path>
            var fileObject = ProbeHelper.GetDefinitionObjectTypeByID("definitions_all_unix", "oval:gov.irs.rhel5:obj:33");
            var fileItemTypeGenerator = CreateFileItemTypeGeneratorWithMockedBehaviorToProcessPatternMatchOperation(false);

            var itemsToCollect = fileItemTypeGenerator.GetItemsToCollect(fileObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, TypeOfUnixFileItem);
        
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_complete_unix_filepath_from_a_fileitem()
        {
            var ovalDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitions_all_unix");
            var fileObjectWithFilepath = (file_object)ProbeHelper.GetOvalComponentByOvalID(ovalDefinitions, "oval:modulo:obj:2000");
            var fileObjectWithPathAndFilename = (file_object)ProbeHelper.GetOvalComponentByOvalID(ovalDefinitions, "oval:modulo:obj:2");
            var fileObjectWithNilFilename = (file_object)ProbeHelper.GetOvalComponentByOvalID(ovalDefinitions, "oval:modulo:obj:2001");

            Assert.AreEqual("/home/Desktop/file1", fileObjectWithFilepath.GetFullFilepath());
            Assert.AreEqual("/home/file1", fileObjectWithPathAndFilename.GetFullFilepath());
            Assert.AreEqual("/home/Desktop/", fileObjectWithNilFilename.GetFullFilepath());

        }


        private void AssertFileItem(ItemType[] itemsToAssert, string expectedPath, string expectedFilename, bool filepathMustBeNull)
        {
            ItemTypeChecker.DoBasicAssertForItems(itemsToAssert, 1, TypeOfUnixFileItem);
            
            var fileItemToAssert = (OVAL.SystemCharacteristics.Unix.file_item)itemsToAssert.Single();

            if (filepathMustBeNull)
            {
                Assert.IsNull(fileItemToAssert.filepath);
                ItemTypeEntityChecker.AssertItemTypeEntity(fileItemToAssert.path, expectedPath);
                if (!string.IsNullOrEmpty(expectedFilename))
                    ItemTypeEntityChecker.AssertItemTypeEntity(fileItemToAssert.filename, expectedFilename);
            }
            else
            {
                var expectedFilepath = string.Format("{0}{1}", expectedPath, expectedFilename);
                ItemTypeEntityChecker.AssertItemTypeEntity(fileItemToAssert.filepath, expectedFilepath);
                Assert.AreEqual(String.Empty, fileItemToAssert.path.Value);
                Assert.AreEqual(String.Empty, fileItemToAssert.filename.Value);
                
            }
        }

        private IItemTypeGenerator CreateFileItemTypeGeneratorWithMockedBehaviorToProcessPatternMatchOperation(
            bool doesTheSecondPathExists = true)
        {
            var mocks = new MockRepository();
            var fakeUnixFileProvider = mocks.StrictMock<IFileProvider>();
            Expect.Call(fakeUnixFileProvider.GetFileChildren("/lib/modules/"))
                .IgnoreArguments()
                .Return(new string[] { "/lib/modules/2.6.18-128.1.10.el5.xs5.5.0.51xen", "/lib/modules/2.6.18-128.el5" });

            Expect.Call(
                fakeUnixFileProvider
                    .FileExists(
                        "/lib/modules/2.6.18-128.1.10.el5.xs5.5.0.51xen/kernel/drivers/usb/storage/usb-storage.ko"))
                    .Return(true);

            Expect.Call(
                fakeUnixFileProvider
                    .FileExists(
                        "/lib/modules/2.6.18-128.el5/kernel/drivers/usb/storage/usb-storage.ko"))
                    .Return(doesTheSecondPathExists);
            
            mocks.ReplayAll();

            return new FileItemTypeGenerator()
            {
                OperationEvaluator = new FileEntityOperationEvaluator(fakeUnixFileProvider, FamilyEnumeration.unix)
            };
        }

        private IItemTypeGenerator CreateFileItemTypeGeneratorWithDefaultBehavior()
        {
            var mocks = new MockRepository();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(fakeFileProvider.FileExists(null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();

            return new FileItemTypeGenerator() { FileProvider = fakeFileProvider };
        }
    }
}
