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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.Probes._New.FileEffectiveRights53;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;
using System.IO;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.File;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests._New.FileEffectiveRights
{
    [TestClass]
    public class FileEffectiveRightsItemTypeGeneratorTests
    {
        private  IEnumerable<fileeffectiverights53_object> FileEffectiveRightsObjectSamples;
        private  FileEffectiveRights53ItemTypeGenerator ItemTypeGenerator;
        private FileEffectiveRights53ObjectFactory ObjectFactory;
        private Dictionary<string, IEnumerable<string>> FakeVariables;
        private Dictionary<string, IEnumerable<string>> FakeMultiVariables;

        public FileEffectiveRightsItemTypeGeneratorTests()
        {
            var ovalDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple");
            this.FileEffectiveRightsObjectSamples = ovalDefinitions.objects.OfType<fileeffectiverights53_object>();
            this.ItemTypeGenerator = new FileEffectiveRights53ItemTypeGenerator(null, null);
            this.ObjectFactory = new FileEffectiveRights53ObjectFactory();
            
            this.FakeVariables = new Dictionary<string, IEnumerable<string>>();
            FakeVariables.Add("oval:modsic.test:var:1", new string[] { "c:\\windows\\odbc.ini" });
            FakeVariables.Add("oval:modsic.test:var:2", new string[] { "c:\\windows" });
            FakeVariables.Add("oval:modsic.test:var:3", new string[] { "odbc.ini" });
            FakeVariables.Add("oval:modsic.test:var:4", new string[] { "S-1-1-0" });

            this.FakeMultiVariables = new Dictionary<string, IEnumerable<string>>();
            FakeMultiVariables.Add("oval:modsic.test:var:1", new string[] { "c:\\windows", "c:\\windows NT" });
            FakeMultiVariables.Add("oval:modsic.test:var:2", new string[] { "odbc.ini" });
            FakeMultiVariables.Add("oval:modsic.test:var:3", new string[] { "S-1-0", "S-1-1" });

        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_fileeffetiverights_object_defined_with_filepath_entity()
        {
            var fileEffectiveRightsObject = this.ObjectFactory.CreateFileEffectiveRightsObject(@"c:\windows", @"win.ini", "S-1-2-15");

            var generatedItems = ItemTypeGenerator.GetItemsToCollect(fileEffectiveRightsObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(fileeffectiverights_item), StatusEnumeration.notcollected);
            var itemToAssert = (fileeffectiverights_item)generatedItems.Single();
            AssertGeneratedItem(itemToAssert, @"c:\windows\win.ini", @"c:\windows", @"win.ini", @"S-1-2-15");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_fileeffetiverights_object_defined_with_path_and_filename_entities()
        {
            var fileEffectiveRightsObject = this.ObjectFactory.CreateFileEffectiveRightsObject(@"c:\windows", @"win.ini", "S-1-1", false);

            var generatedItems = ItemTypeGenerator.GetItemsToCollect(fileEffectiveRightsObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(fileeffectiverights_item), StatusEnumeration.notcollected);
            var itemToAssert = (fileeffectiverights_item)generatedItems.Single();
            AssertGeneratedItem(itemToAssert, @"c:\windows\win.ini", @"c:\windows", @"win.ini", @"S-1-1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_fileeffetiverights_object_with_referenced_variables()
        {
            var fileEffectiveRightsObject = this.ObjectFactory.CreateFileEffectiveRightsObject("", "", "");
            ((EntityObjectStringType)fileEffectiveRightsObject.Items[0]).var_ref = "oval:modsic.test:var:1";
            ((EntityObjectStringType)fileEffectiveRightsObject.Items[1]).var_ref = "oval:modsic.test:var:4";
            var fakeVariables = VariableHelper.CreateEvaluatedVariables(fileEffectiveRightsObject.id, this.FakeVariables);

            var generatedItems = ItemTypeGenerator.GetItemsToCollect(fileEffectiveRightsObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(fileeffectiverights_item), StatusEnumeration.notcollected);
            var itemToAssert = (fileeffectiverights_item)generatedItems.Single();
            AssertGeneratedItem(itemToAssert, "c:\\windows\\odbc.ini", "c:\\windows", "odbc.ini", "S-1-1-0");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_fileeffetiverights_object_with_referenced_variables_with_multi_values()
        {
            var fileEffectiveRightsObject = this.ObjectFactory.CreateFileEffectiveRightsObject("", "", "", false);
            ((EntityObjectStringType)fileEffectiveRightsObject.Items[0]).var_ref = "oval:modsic.test:var:1";
            ((EntityObjectStringType)fileEffectiveRightsObject.Items[1]).var_ref = "oval:modsic.test:var:2";
            ((EntityObjectStringType)fileEffectiveRightsObject.Items[2]).var_ref = "oval:modsic.test:var:3";
            var fakeVariables = VariableHelper.CreateEvaluatedVariables(fileEffectiveRightsObject.id, FakeMultiVariables);

            var generatedItems = ItemTypeGenerator.GetItemsToCollect(fileEffectiveRightsObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 4, typeof(fileeffectiverights_item), StatusEnumeration.notcollected);
            var itemsToAssert = generatedItems.OfType<fileeffectiverights_item>();
            AssertGeneratedItem(itemsToAssert.ElementAt(0), "c:\\windows\\odbc.ini", "c:\\windows", "odbc.ini", "S-1-0");
            AssertGeneratedItem(itemsToAssert.ElementAt(1), "c:\\windows\\odbc.ini", "c:\\windows", "odbc.ini", "S-1-1");
            AssertGeneratedItem(itemsToAssert.ElementAt(2), "c:\\windows NT\\odbc.ini", "c:\\windows NT", "odbc.ini", "S-1-0");
            AssertGeneratedItem(itemsToAssert.ElementAt(3), "c:\\windows NT\\odbc.ini", "c:\\windows NT", "odbc.ini", "S-1-1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_fileeffetiverights_object_with_pattern_match_on_filepath_entity()
        {
            var objectSample = GetFileEffectiveRightsObjectWithPatternMatchOnFilepath("c:\\^windows.*\\odbc.ini");
            var mockFileProvider = this.CreateWindowsFileProviderMockingDirectorySearch();
            var itemTypeGenerator = new FileEffectiveRights53ItemTypeGenerator(mockFileProvider, null);

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectSample, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 2, typeof(fileeffectiverights_item), StatusEnumeration.notcollected);
            var itemsToAssert = generatedItems.OfType<fileeffectiverights_item>();
            AssertGeneratedItem(itemsToAssert.ElementAt(0), "c:\\windows\\odbc.ini", "c:\\windows", "odbc.ini", "S-1-1");
            AssertGeneratedItem(itemsToAssert.ElementAt(1), "c:\\windows NT\\odbc.ini", "c:\\windows NT", "odbc.ini", "S-1-1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_fileeffetiverights_object_with_pattern_match_on_path_and_filename_entities()
        {
            var objectSample = GetFileEffectiveRightsObjectWithPatternMatchOnPathAndFilename("c:\\windows\\^system.*", ".*.dll");
            var mockFileProvider = this.CreateWindowsFileProviderMockingDirectoryAndFileSearches();
            var itemTypeGenerator = new FileEffectiveRights53ItemTypeGenerator(mockFileProvider, null);
            
            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectSample, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 4, typeof(fileeffectiverights_item), StatusEnumeration.notcollected);
            var itemsToAssert = generatedItems.OfType<fileeffectiverights_item>();
            AssertGeneratedItem(itemsToAssert.ElementAt(0), "c:\\windows\\system\\b.dll", "c:\\windows\\system", "b.dll", "S-1-1");
            AssertGeneratedItem(itemsToAssert.ElementAt(1), "c:\\windows\\system\\c.dll", "c:\\windows\\system", "c.dll", "S-1-1");
            AssertGeneratedItem(itemsToAssert.ElementAt(2), "c:\\windows\\system32\\b.dll", "c:\\windows\\system32", "b.dll", "S-1-1");
            AssertGeneratedItem(itemsToAssert.ElementAt(3), "c:\\windows\\system32\\c.dll", "c:\\windows\\system32", "c.dll", "S-1-1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_fileeffetiverights_object_with_pattern_match_on_trusteeSid_entity()
        {
            var objectSample = GetFilEffectiveRightsObjectWithPatternMatchOnTrusteeSid(".*");
            var mockObjectCollector = CreateObjectCollectorToGetAllTargetUsers(".*", OperationEnumeration.patternmatch);
            var itemTypeGenerator = new FileEffectiveRights53ItemTypeGenerator(null, mockObjectCollector);

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectSample, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 2, typeof(fileeffectiverights_item));
            Assert.AreEqual("1", generatedItems.ElementAt(0).id);
            Assert.AreEqual("2", generatedItems.ElementAt(1).id);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_fileeffetiverights_object_with_path_but_no_filename()
        {
            var objectSample = 
                new FileEffectiveRights53ObjectFactory()
                    .CreateFileEffectiveRightsObject("c:\\windows", null, "S-1-1", false);
            var itemTypeGenerator = new FileEffectiveRights53ItemTypeGenerator(null, null);

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectSample, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(fileeffectiverights_item), StatusEnumeration.notcollected);
            var itemsToAssert = generatedItems.OfType<fileeffectiverights_item>();
            AssertGeneratedItem(itemsToAssert.ElementAt(0), "c:\\windows", "c:\\windows", null, "S-1-1");

        }

        private FileEffectiveRights53ObjectCollector CreateObjectCollectorToGetAllTargetUsers(
            string sidPattern, OperationEnumeration operation)
        {
            var mocks = new MockRepository();
            var objectCollector = mocks.DynamicMock<FileEffectiveRights53ObjectCollector>(new object[] { null });
            Expect.Call(
                objectCollector
                    .CollectItems(@"c:\windows\win.ini", sidPattern, operation))
                    .Return(
                        new ItemType[] 
                        { 
                            new fileeffectiverights_item() { id = "1" }, 
                            new fileeffectiverights_item() { id = "2" }
                        });
            
            mocks.ReplayAll();

            return objectCollector;
        }

        private WindowsFileProvider CreateWindowsFileProviderMockingDirectorySearch()
        {
            var mocks = new MockRepository();
            var fileProvider = mocks.DynamicMock<WindowsFileProvider>(new object[] { null } );
            Expect.Call(
                fileProvider
                    .GetChildrenDirectories("c:"))
                    .Return(new string[] { "program files", "windows", "windows NT", "system" });
            mocks.ReplayAll();

            return fileProvider;
        }

        private WindowsFileProvider CreateWindowsFileProviderMockingDirectoryAndFileSearches()
        {
            var fakeSubDirs = new string[] { "debug", "help", "system", "system32", "winsys" };
            var fakeFiles = new string[] { "a.exe", "b.dll", "c.dll", "d.chm" };

            var mocks = new MockRepository();
            var fileProvider = mocks.StrictMock<WindowsFileProvider>(new object[] { null });
            Expect.Call(fileProvider.GetChildrenDirectories("c:\\windows")).Return(fakeSubDirs);
            Expect.Call(fileProvider.GetChildrenFiles("c:\\windows\\system")).Return(fakeFiles);
            Expect.Call(fileProvider.GetChildrenFiles("c:\\windows\\system32")).Return(fakeFiles);
            mocks.ReplayAll();

            return fileProvider;
        }

        private void AssertGeneratedItem(
            fileeffectiverights_item itemToAssert, 
            string expectedFilepath, 
            string expectedPath, 
            string expectedFilename, 
            string expectedTrusteeSID)
        {
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.filepath, expectedFilepath, "filepath");
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.path, expectedPath, "path");
            if (expectedFilename != null)
                ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.filename, expectedFilename, "filename");
            ItemTypeEntityChecker.AssertItemTypeEntity(itemToAssert.trustee_sid, expectedTrusteeSID, "trustee_sid");
        }

        private OVAL.Definitions.ObjectType GetFileEffectiveRightsObjectWithPatternMatchOnFilepath(string pattern)
        {
            var fileEffectiveRightsObject = ObjectFactory.CreateFileEffectiveRightsObject(pattern, "", "S-1-1");
            ((EntityObjectStringType)fileEffectiveRightsObject.Items[0]).operation = OperationEnumeration.patternmatch;
            return fileEffectiveRightsObject;
        }

        private OVAL.Definitions.ObjectType GetFileEffectiveRightsObjectWithPatternMatchOnPathAndFilename(
            string patternForPath = null, string patternForFilename = null)
        {
            var fileEffectiveRightsObject = ObjectFactory.CreateFileEffectiveRightsObject(patternForPath ?? "", patternForFilename ?? "", "S-1-1", false);
            
            var pathEntity = (EntityObjectStringType)fileEffectiveRightsObject.Items[0];
            pathEntity.operation = OperationEnumeration.patternmatch;
            if (patternForPath == null)
                pathEntity.var_ref = "oval:modsic:var:1";

            var filenameEntity = (EntityObjectStringType)fileEffectiveRightsObject.Items[1];
            filenameEntity.operation = OperationEnumeration.patternmatch;
            if (patternForFilename == null)
                filenameEntity.var_ref = "oval:modsic:var:2";

            return fileEffectiveRightsObject;
        }

        private OVAL.Definitions.ObjectType GetFilEffectiveRightsObjectWithPatternMatchOnTrusteeSid(string trusteeSidPattern = null)
        {
            var fileEffectiveRightsObject = ObjectFactory.CreateFileEffectiveRightsObject("c:\\windows", "win.ini", ".*");
            var trusteeSidEntity = (EntityObjectStringType)fileEffectiveRightsObject.Items.ElementAt(1);
            trusteeSidEntity.operation = OperationEnumeration.patternmatch;
            if (trusteeSidPattern == null)
                trusteeSidEntity.var_ref = "oval:modsic:var:1";

            return fileEffectiveRightsObject;
        }

    }

    public class FileEffectiveRights53ObjectFactory
    {
        public fileeffectiverights53_object CreateFileEffectiveRightsObject(
            string path, string filename, string trusteeSID, bool useFilepathEntity = true)
        {
            var objectEntities = new List<EntityObjectStringType>();
            if (useFilepathEntity)
                objectEntities.Add(new EntityObjectStringType() { Value = Path.Combine(path, filename) });
            else
            {
                objectEntities.Add(new EntityObjectStringType() { Value = path });
                if (filename != null)
                    objectEntities.Add(new EntityObjectStringType() { Value = filename });
                else
                    objectEntities.Add(null);
            }
            objectEntities.Add(new EntityObjectStringType() { Value = trusteeSID });

            return this.CreateFileEffectiveRightsObject(objectEntities, useFilepathEntity);
        }

        private fileeffectiverights53_object CreateFileEffectiveRightsObject(
            IEnumerable<EntitySimpleBaseType> entities, bool useFilepathEntity = true)
        {
            var itemsElementName = new List<fileeffectiverights53_object_ItemsChoices>();
            if (useFilepathEntity)
                itemsElementName.Add(fileeffectiverights53_object_ItemsChoices.filepath);
            else
            {
                itemsElementName.Add(fileeffectiverights53_object_ItemsChoices.path);
                if (entities.ElementAt(1) == null)
                    entities = new EntitySimpleBaseType[] { entities.First(), entities.Last() };
                else
                    itemsElementName.Add(fileeffectiverights53_object_ItemsChoices.filename);
            }
            itemsElementName.Add(fileeffectiverights53_object_ItemsChoices.trustee_sid);

            return new fileeffectiverights53_object()
            {
                id = "oval:modsic.fakeobject:obj:1",
                version = "1",
                Fileeffectiverights53ObjectItemsElementName = itemsElementName.ToArray(),
                Items = entities.ToArray()
            };
        }
    }
}
