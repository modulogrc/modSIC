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
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.SID;
using Modulo.Collect.Probe.Windows.SID_SID;
using Modulo.Collect.Probe.Windows.Test.Helpers;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Common.Test.Checkers;


namespace Modulo.Collect.Probe.Windows.Test
{

    [TestClass]
    public class ItemTypeGeneratorTest
    {
        private const string DEFINITIONS_REGEX_ON_VALUE = "fdcc_xpfirewall_oval_regex_on_value.xml";
        private const string DEFINITIONS_SIMPLE = "definitionsSimple.xml";

        private const string OBJ_50003_ID = "oval:modulo:obj:50003";
        private const string OBJ_50004_ID = "oval:modulo:obj:50004";
        private const string OBJ_50005_ID = "oval:modulo:obj:50005";
        private const string OBJ_50006_ID = "oval:modulo:obj:50006";
        private const string OBJ_50007_ID = "oval:modulo:obj:50007";
        private const string OBJ_50008_ID = "oval:modulo:obj:50008";
        private const string OBJ_50009_ID = "oval:modulo:obj:50009";
        private const string OBJ_50010_ID = "oval:modulo:obj:50010";

        private const string OBJ_SIMPLE_7 = "oval:modulo:obj:7";
        private const string OBJ_SIMPLE_8 = "oval:modulo:obj:8";

        private const string TEST_EXCEPTION_MESSAGE = "An error occurred while testing";

        [TestInitialize()]
        public void MyTestInitialize()
        {
            variableEvaluated = new VariablesEvaluated(new List<VariableValue>());
        }

        private VariablesEvaluated variableEvaluated;
        
        private Dictionary<string, string> getPathParts(string fullPath)
        {
            
            string drive = Path.GetPathRoot(fullPath);
            string path = Path.GetDirectoryName(fullPath).Replace(drive, string.Empty);
            string filename = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);

            Dictionary<string, string> pathParts = new Dictionary<string, string>();
            pathParts.Add("drive", drive);
            pathParts.Add("path", path);
            pathParts.Add("filename", filename);
            pathParts.Add("extension", extension);
            
            return pathParts;
        }


        [TestMethod, Owner("lfernandes")]
        public void Learning_how_to_use_PathClass_methods()
        {
            Dictionary<string, string> pathPartsToAssert;

            pathPartsToAssert = this.getPathParts(@"c:\windows\^system.*\drivers.exe");
            Assert.AreEqual("c:\\", pathPartsToAssert["drive"]);
            Assert.AreEqual("windows\\^system.*", pathPartsToAssert["path"]);
            Assert.AreEqual("drivers", pathPartsToAssert["filename"]);
            Assert.AreEqual(".exe", pathPartsToAssert["extension"]);

            pathPartsToAssert = this.getPathParts(@"c:\windows\");
            Assert.AreEqual("c:\\", pathPartsToAssert["drive"]);
            Assert.AreEqual("windows", pathPartsToAssert["path"]);
            Assert.IsTrue(string.IsNullOrEmpty(pathPartsToAssert["filename"]));
            Assert.IsTrue(string.IsNullOrEmpty(pathPartsToAssert["extension"]));
        }

        // ======================================================
        // ========== File Object Items Generator Test ==========
        // ======================================================
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_file_items_from_object_types()
        {
            // Arrange
            var fileObject = (file_object)WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_REGEX_ON_VALUE, OBJ_50004_ID);
            var fakeFileProvider = CreateFileProviderToAlwaysReturnThatTheFileExists();
            var fileItemTypeGenerator = new FileItemTypeGenerator() { FileProvider =  fakeFileProvider };

            // Act
            var itemsToCollect = fileItemTypeGenerator.GetItemsToCollect(fileObject, null).ToList();

            // Assert
            Assert.IsNotNull(itemsToCollect, "The return of GetItemsToCollect cannot be null.");
            Assert.AreEqual(1, itemsToCollect.Count, "The number of items to collect is unexpected.");
            this.AssertGeneratedFileItem(itemsToCollect[0], null, "c:\\windows", "foo.exe");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_possible_to_collect_directory_passing_nil_filename()
        {
            var fileObject = (file_object)WindowsTestHelper.GetObjectFromDefinitions("definitionsSimple.xml", "oval:com.modulo.IIS6:obj:6877401");
            var fakeFileProvider = CreateFileProviderToAlwaysReturnThatTheFileExists();
            var fakeEvaluatedVars = VariableHelper.CreateVariableWithOneValue(fileObject.id, "oval:com.modulo.IIS6:var:687741", @"c:\System32\Inetsrv\Iisadmpwd");
            var fileItemTypeGenerator = new FileItemTypeGenerator() { FileProvider = fakeFileProvider };

            var itemsToCollect = fileItemTypeGenerator.GetItemsToCollect(fileObject, fakeEvaluatedVars).ToList();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(file_item));
            var fileItem = (file_item)itemsToCollect.Single();
            ItemTypeEntityChecker.AssertItemTypeEntity(fileItem.path, @"c:\System32\Inetsrv\Iisadmpwd", "path");
            ItemTypeEntityChecker.AssertItemTypeEntity(fileItem.filename, string.Empty, "filename");
            
        }

        private IFileProvider CreateFileProviderToAlwaysReturnThatTheFileExists()
        {
            var mocks = new MockRepository();
            var fileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(fileProvider.FileExists(null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();
            
            return fileProvider;
        }


        private void AssertGeneratedItemStatusAndMessageType(ItemType itemToAssert, StatusEnumeration expectedItemStatus, string expectedMessage)
        {
            Assert.AreEqual(expectedItemStatus, itemToAssert.status, "A generated item with unexpected status was found.");
            
            var itemMessage = itemToAssert.message;
            if (expectedItemStatus == StatusEnumeration.error)
            {
                Assert.IsNotNull(itemMessage, "The message type item entity cannot be null for items with 'Error' status");
                Assert.IsFalse(string.IsNullOrEmpty(itemMessage.First().Value), "The message cannot be empty for items with 'Error' status");
            }

            if ((!string.IsNullOrEmpty(expectedMessage)) && (itemMessage != null))
                Assert.IsTrue(itemMessage.First().Value.Contains(expectedMessage), "The exception message was not found in item type message.");
        }

        private FileEntityOperationEvaluator CreateMockedFileEntityOperationEvaluationWithExpectedExceptionThrowing()
        {
            MockRepository mocks = new MockRepository();
            var fakeSystemDataSource = mocks.DynamicMock<BaseObjectCollector>();
            var fakeOperationEvaluator = mocks.DynamicMock<FileEntityOperationEvaluator>(fakeSystemDataSource);
            Expect.Call(fakeOperationEvaluator.ProcessOperation(new Definitions.ObjectType[] { })).IgnoreArguments()
                .Throw(new Exception(TEST_EXCEPTION_MESSAGE));
            mocks.ReplayAll();

            return fakeOperationEvaluator;
        }


        

        [Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_generate_file_items_from_object_types_with_regex_on_all_entities()
        {
            #region Oval File Object
            //  <file_object id="oval:modulo:obj:50005" version="1" xmlns="#windows">
            //    <path operation="pattern match">c:\windows\^system.*\drivers</path>
            //    <filename operation="pattern match">.*driver.exe</filename>
            //  </file_object>
            #endregion

            // Arrange
            string[] fakeFoundPaths = new string[] { 
                "c:\\windows\\xsystem\\drivers\\driver.exe", @"c:\windows\system\drivers\vgadriver.exe", 
                "c:\\windows\\system\\drivers\\driverx.exe", @"c:\windows\system 32\drivers\xptodriver.exe", 
                "c:\\windows\\system 32\\drivers\\drivery.exe", "c:\\windows\\system 32\\drivers\\x64" };

            file_object fileObj50005;
            FileItemTypeGenerator fileItemsGenerator = this.GetMockedFileItemTypeGenerator(OBJ_50005_ID, fakeFoundPaths, out fileObj50005);


            // Act
            IList<ItemType> itemsToCollect = fileItemsGenerator.GetItemsToCollect(fileObj50005, null).ToList();

            // Assert
            Assert.IsNotNull(itemsToCollect, "The return of GetItemsToCollect method cannot be null.");
            Assert.AreEqual(2, itemsToCollect.Count, "The number of generated items is not expected.");
            this.AssertGeneratedFileItem(itemsToCollect[0], null, @"c:\windows\system\drivers", "vgadriver.exe");
            this.AssertGeneratedFileItem(itemsToCollect[1], null, @"c:\windows\system 32\drivers", "xptodriver.exe");
        }

        [Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_generate_FileItems_from_FileObject_with_operation_in_path_and_variableReference_in_filename()
        {
            #region Oval File Object and Variable
            // <file_object id="oval:modulo:obj:50007" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //   <path operation="pattern match">c:\windows\^system.*\drivers</path>
            //   <filename var_ref="oval:modulo:var:50007"></filename>
            // </file_object>

            // <constant_variable id="oval:modulo:var:50007" datatype="string" version="1" comment="...">
            //   <value>driver.dll</value>
            // </constant_variable>
            #endregion

            // Arrange
            string[] fakeFoundPaths = new string[] 
                { "c:\\windows\\xsystem\\drivers\\driver.exe", "c:\\windows\\system\\drivers\\vgadriver.exe", 
                  "c:\\windows\\system\\drivers\\driverx.exe", @"c:\windows\system 32\drivers\driver.dll", 
                  "c:\\windows\\system 32\\drivers\\drivery.exe", "c:\\windows\\system 32\\drivers\\x64" };


            file_object fileObj50007;
            VariablesEvaluated vars = VariableHelper.CreateVariableWithOneValue(OBJ_50007_ID, "oval:modulo:var:50007", "driver.dll");
            FileItemTypeGenerator fileItemsGenerator = this.GetMockedFileItemTypeGenerator(OBJ_50007_ID, fakeFoundPaths, out fileObj50007);

            // Act
            IList<ItemType> itemsToCollect = fileItemsGenerator.GetItemsToCollect(fileObj50007, vars).ToList();


            // Assert
            Assert.IsNotNull(itemsToCollect, "The generated items cannot be null.");
            Assert.AreEqual(1, itemsToCollect.Count, "The number of generated items is not expected.");
            this.AssertGeneratedFileItem(itemsToCollect[0], null, @"c:\windows\system 32\drivers", "driver.dll");
        }

        [Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_generate_FileItems_from_FileObject_with_FilePathEntity_defined()
        {
            #region Oval File Object
            //  <file_object id="oval:modulo:obj:50008" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //      <filepath>c:\windows\foo.exe</filepath>
            //  </file_object>
            #endregion

            // Arrange
            file_object fileObj50008 = (file_object)WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_REGEX_ON_VALUE, OBJ_50008_ID);
            var fileItemsGenerator = new FileItemTypeGenerator() { SystemDataSource = WindowsTestHelper.GetDataSourceFakewithoutRegex() };


            // Act
            IList<ItemType> itemsToCollect = fileItemsGenerator.GetItemsToCollect(fileObj50008, null).ToList();
            Assert.IsNotNull(itemsToCollect, "The generated items cannot be null.");
            Assert.AreEqual(1, itemsToCollect.Count, "The number of generated items is not expected.");
            this.AssertGeneratedFileItem(itemsToCollect[0], @"c:\windows\foo.exe", null, null);
        }

        [Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_generate_FileItems_from_FileObject_with_FilePathEntity_defined_with_PatternMatchOperation()
        {
            #region Oval File Object
            //  <file_object id="oval:modulo:obj:5009" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //      <filepath operation="pattern match">c:\windows\^system.*\drivers\^driv.*</filepath>
            //  </file_object>
            #endregion

            // Arrange
            string[] fakeFoundPaths = new string[] 
                { "c:\\windows\\xsystem\\drivers\\driver.exe", "c:\\windows\\system\\drivers\\vgadriver.exe", 
                  @"c:\windows\system\drivers\driverx.exe", @"c:\windows\system 32\drivers\driver.dll", 
                  @"c:\windows\system 32\drivers\drivery.exe", "c:\\windows\\system 32\\drivers\\x64" };

            file_object fileObj50009;
            FileItemTypeGenerator fileItemsGenerator = this.GetMockedFileItemTypeGenerator(OBJ_50009_ID, fakeFoundPaths, out fileObj50009);


            // Act
            IList<ItemType> itemsToCollect = fileItemsGenerator.GetItemsToCollect(fileObj50009, null).ToList();
            Assert.IsNotNull(itemsToCollect, "The generated items cannot be null.");
            Assert.AreEqual(3, itemsToCollect.Count, "The number of generated items is not expected.");
            this.AssertGeneratedFileItem(itemsToCollect[0], @"c:\windows\system\drivers\driverx.exe", null, null);
            this.AssertGeneratedFileItem(itemsToCollect[1], @"c:\windows\system 32\drivers\driver.dll", null, null);
            this.AssertGeneratedFileItem(itemsToCollect[2], @"c:\windows\system 32\drivers\drivery.exe", null, null);
        }

        [Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_generate_FileItems_from_FileObject_with_FilePathEntity_defined_with_reference_to_variable()
        {
            #region Oval File Object
            //  <file_object id="oval:modulo:obj:50010" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //      <filepath var_ref="oval:modulo:var:50010"></filepath>
            //  </file_object>
            //  <constant_variable id="oval:modulo:var:50010" datatype="string" version="1" comment="...">
            //      <value>c:\windows\system32\vgadriver.dll</value>
            //  </constant_variable>
            #endregion

            // Arrange
            string fakeVariableValue = @"c:\windows\system32\vgadriver.dll";
            var fileObj50010 = (file_object)WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_REGEX_ON_VALUE, OBJ_50010_ID);
            var vars = VariableHelper.CreateVariableWithOneValue(OBJ_50010_ID, "oval:modulo:var:50010", fakeVariableValue);
            var fileItemsGenerator = new FileItemTypeGenerator() { SystemDataSource = WindowsTestHelper.GetDataSourceFakewithoutRegex() };

            // Act
            IList<ItemType> itemsToCollect = fileItemsGenerator.GetItemsToCollect(fileObj50010, vars).ToList();

            // Assert
            Assert.IsNotNull(itemsToCollect, "The generated items cannot be null.");
            Assert.AreEqual(1, itemsToCollect.Count, "The number of generated items is not expected.");
            this.AssertGeneratedFileItem(itemsToCollect[0], fakeVariableValue, null, null);
        }


        // =======================================================================
        // ========== File Effective Rights Object Items Generator Test ==========
        // =======================================================================
        [Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_generate_FileItems_from_FileEffectiveRightsObject()
        {
            // Arrange
            var obj = (fileeffectiverights_object)WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_SIMPLE, OBJ_SIMPLE_7);
            var itemTypeGenerator = new FileEffectiveRightsItemTypeGenerator();

            // Act
            IList<ItemType> generatedItems = itemTypeGenerator.GetItemsToCollect(obj, null).ToList();

            // Assert
            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(1, generatedItems.Count, "Unexpected items count was found.");
            this.AssertGeneratedFileEffectiveRightsItem(generatedItems[0], null, @"c:\temp", "file1.txt", @"mss\lfernandes");
        }


        // =====================================================
        // ========== SID Object Items Generator Test ==========
        // =====================================================
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_SIDItems_from_SIDObject()
        {
            #region SID OBJECT
            //<sid_object id="oval:modulo:obj:8" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //    <trustee_name>Administrator</trustee_name>
            //</sid_object>
            #endregion

            // Arrange
            var sidObject = (sid_object)WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_SIMPLE, OBJ_SIMPLE_8);
            SIDItemTypeGenerator itemTypeGenerator = new SIDItemTypeGenerator();

            // Act
            IList<ItemType> generatedItems = itemTypeGenerator.GetItemsToCollect(sidObject, null).ToList();

            // Assert
            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(1, generatedItems.Count, "Unexpected items count was found.");
            Assert.IsInstanceOfType(generatedItems[0], typeof(sid_item), "Unexpected type of generated item found. The correct type is 'sid_item'");
            Assert.AreEqual("Administrator", ((sid_item)generatedItems[0]).trustee_name.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_SIDItems_from_SIDObject_with_referenced_variable()
        {
            // Arrange
            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions(DEFINITIONS_SIMPLE);
            sid_object sidObject10 = (sid_object)definitions.objects.Where(obj => obj.id.Equals("oval:modulo:obj:11")).Single();
            VariableType variable10 = definitions.variables.Where(var => var.id.Equals("oval:modulo:var:10")).Single();
            string variable10Value = ((VariablesTypeVariableConstant_variable)variable10).value.First().Value;

            VariablesEvaluated fakeVariables = VariableHelper.CreateVariableWithOneValue(sidObject10.id, variable10.id, variable10Value);
            SIDItemTypeGenerator itemGenerator = new SIDItemTypeGenerator();

            // Act
            IList<ItemType> generatedItems = itemGenerator.GetItemsToCollect(sidObject10, fakeVariables).ToList();

            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(1, generatedItems.Count, "Unexpected items count was found.");
            Assert.IsInstanceOfType(generatedItems[0], typeof(sid_item), "Unexpected type of generated item found. The correct type is 'sid_item'");
            Assert.AreEqual(variable10Value, ((sid_item)generatedItems[0]).trustee_name.Value);
        }

        [TestMethod, Ignore, Owner("lfernandes")]
        public void Should_be_possible_generate_SIDItems_from_with_PatternMatch_operation_on_TrusteeNameEntity()
        {
            // Pattern .*\fernandes
            // Arrange
            var fakeTrusteeNames = new string[]  { "XPTO\\fernandess", @"MSS\lfernandes", @"LOCAL\lfernandes", @"lfernandes", "MSS\\admin" };
            var sidObject = (sid_object)WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_SIMPLE, "oval:modulo:obj:12");
            var itemGenerator = this.GetMockecSidItemTypeGenerator(sidObject.id, fakeTrusteeNames);

            // Act
            var generatedItems = itemGenerator.GetItemsToCollect(sidObject, null);

            // Assert
            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(3, generatedItems.Count(), "Unexpected items count was found.");
            this.AssertGeneratedSidItem(generatedItems.ElementAt(0), @"MSS\lfernandes");
            this.AssertGeneratedSidItem(generatedItems.ElementAt(1), @"LOCAL\lfernandes");
            this.AssertGeneratedSidItem(generatedItems.ElementAt(2), @"lfernandes");
        }

        [TestMethod, Ignore, Owner("lfernandes")]
        public void Should_be_possible_generate_SIDItems_from_SIDObject_with_PatternMatchOperation_and_VariableReference_on_TrusteeNameEntity()
        {
            // Arrange
            string[] fakeTrusteeNames = new string[] 
                { "XPTO\\fernandess", @"MSS\lfernandes", @"LOCAL\lfernandes", @"lfernandes", "MSS\\admin" };
            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions(DEFINITIONS_SIMPLE);
            sid_object sidObject = (sid_object)definitions.objects.Where(obj => obj.id.Equals("oval:modulo:obj:99")).Single();
            VariablesEvaluated fakeVariables = this.createFakeVariablesEvaluated(definitions, sidObject.id, "oval:modulo:var:99");
            SIDItemTypeGenerator itemGenerator = this.GetMockecSidItemTypeGenerator(sidObject.id, fakeTrusteeNames);

            // Act
           var generatedItems = itemGenerator.GetItemsToCollect(sidObject, fakeVariables);

            // Arrange
            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(3, generatedItems.Count(), "Unexpected items count was found.");
            this.AssertAllGeneratedSidItems(generatedItems.ToList(), new string[] { @"MSS\lfernandes", @"LOCAL\lfernandes", @"lfernandes" });
        }



        // =====================================================
        // ========== SID Object Items Generator Test ==========
        // =====================================================
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_SID_SID_Items_from_SID_SIDObject()
        {
            #region SID SID OBJECT
            //<sid_sid_object id="oval:modulo:obj:200">
            //  <trustee_sid>S-1-5-20</trustee_sid>
            //</sid_sid_object>
            #endregion

            // Arrange
            var sid_sidObject = (sid_sid_object)WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_SIMPLE, "oval:modulo:obj:200");
            SID_SIDItemTypeGenerator itemTypeGenerator = new SID_SIDItemTypeGenerator();

            // Act
            var generatedItems = itemTypeGenerator.GetItemsToCollect(sid_sidObject, null);

            // Assert
            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(1, generatedItems.Count(), "Unexpected items count was found.");
            Assert.IsInstanceOfType(generatedItems.ElementAt(0), typeof(sid_sid_item), "Unexpected type of generated item found. The correct type is 'sid_sid_item'");
            Assert.AreEqual("S-1-5-20", ((sid_sid_item)generatedItems.ElementAt(0)).trustee_sid.Value, "A generated sid_sid_item with unexpected sid was found.");
            Assert.IsNull(((sid_sid_item)generatedItems.ElementAt(0)).trustee_name);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_SID_SIDItems_from_SID_SIDObject_with_referenced_variable()
        {
            // Arrange
            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions(DEFINITIONS_SIMPLE);
            sid_sid_object sid_sidObject = (sid_sid_object)definitions.objects.Where(obj => obj.id.Equals("oval:modulo:obj:201")).Single();
            VariableType ovalVariable = definitions.variables.Where(var => var.id.Equals("oval:modulo:var:201")).Single();
            string variableValue = ((VariablesTypeVariableConstant_variable)ovalVariable).value.First().Value;

            VariablesEvaluated fakeVariables = VariableHelper.CreateVariableWithOneValue(sid_sidObject.id, ovalVariable.id, variableValue);
            SID_SIDItemTypeGenerator itemGenerator = new SID_SIDItemTypeGenerator();

            // Act
            var generatedItems = itemGenerator.GetItemsToCollect(sid_sidObject, fakeVariables);

            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(1, generatedItems.Count(), "Unexpected items count was found.");
            Assert.IsInstanceOfType(generatedItems.ElementAt(0), typeof(sid_sid_item), "Unexpected type of generated item found. The correct type is 'sid_sid_item'");
            Assert.AreEqual(variableValue, ((sid_sid_item)generatedItems.ElementAt(0)).trustee_sid.Value, "A generated sid_sid_item with unexpected sid was found.");
            Assert.IsNull(((sid_sid_item)generatedItems.ElementAt(0)).trustee_name);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_SID_SIDItems_from_with_PatternMatch_operation_on_TrusteeSIDEntity()
        {
            #region SID SID OBJECT
            //<sid_sid_object id="oval:modulo:obj:202">
            //    <trustee_sid operation="pattern match">^S.*-500$</trustee_sid>
            //</sid_sid_object>
            #endregion

            // Arrange
            string[] fakeTrusteeSIDs = new string[] { "S-1-5", @"S-1-18-500", @"S-1-55-500", "S-1-25-5000" };
            var sidObject = (sid_sid_object)WindowsTestHelper.GetObjectFromDefinitions(DEFINITIONS_SIMPLE, "oval:modulo:obj:202");
            SID_SIDItemTypeGenerator itemGenerator = this.GetMockecSid_SidItemTypeGenerator(sidObject.id, fakeTrusteeSIDs);

            // Act
            var generatedItems = itemGenerator.GetItemsToCollect(sidObject, null);

            // Assert
            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(2, generatedItems.Count(), "Unexpected items count was found.");
            this.AssertGeneratedSid_SidItem(generatedItems.ElementAt(0), @"S-1-18-500");
            this.AssertGeneratedSid_SidItem(generatedItems.ElementAt(1), @"S-1-55-500");
        }


        #region Private Methods
        
        private VariablesEvaluated createFakeVariablesEvaluated(oval_definitions definitions, string objectID, string variableID)
        {
            VariableType variable = definitions.variables.Where(var => var.id.Equals(variableID)).Single();
            string variableValue = ((VariablesTypeVariableConstant_variable)variable).value.First().Value;

            return VariableHelper.CreateVariableWithOneValue(objectID, variableID, variableValue);
        }
        
        private string getConstantVariableValueFromOvalDefinitions(oval_definitions definitions, string variableID)
        {
            VariableType variable = definitions.variables.Where(var => var.id.Equals(variableID)).Single();
            return ((VariablesTypeVariableConstant_variable)variable).value.First().Value;
        }

        private SIDItemTypeGenerator GetMockecSidItemTypeGenerator(string sidObjectID, string[] fakeSidNames)
        {
            MockRepository mocks = new MockRepository();
            var fakeSystemDataSource = mocks.DynamicMock<BaseObjectCollector>();
            Expect.Call(fakeSystemDataSource.GetValues(null)).IgnoreArguments().Return(fakeSidNames);
            mocks.ReplayAll();

            return new SIDItemTypeGenerator() { SystemDataSource = fakeSystemDataSource };
        }

        private SID_SIDItemTypeGenerator GetMockecSid_SidItemTypeGenerator(string sidObjectID, string[] fakeSIDs)
        {
            MockRepository mocks = new MockRepository();
            var fakeSystemDataSource = mocks.DynamicMock<BaseObjectCollector>();
            Expect.Call(fakeSystemDataSource.GetValues(null)).IgnoreArguments().Return(fakeSIDs);
            mocks.ReplayAll();

            return new SID_SIDItemTypeGenerator() { SystemDataSource = fakeSystemDataSource };
        }

        private FileItemTypeGenerator GetMockedFileItemTypeGenerator(string fileObjectID, string[] fakeFoundPaths, out file_object fileObject)
        {
            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions(DEFINITIONS_REGEX_ON_VALUE);
            fileObject = (file_object)ProbeHelper.GetOvalComponentByOvalID(definitions, fileObjectID);

            List<WmiObject> fakeWmiObjects = new List<WmiObject>();
            foreach(var fakePath in fakeFoundPaths)
                fakeWmiObjects.Add(this.createFakeWmiObject(fakePath, "Application", 100));

            MockRepository mocks = new MockRepository();
            WmiDataProvider fakeWmiProvider = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeWmiProvider.SearchWmiObjects(null, null)).IgnoreArguments().Return(fakeWmiObjects.ToArray());
            mocks.ReplayAll();

            return new FileItemTypeGenerator() { SystemDataSource = new FileObjectCollector() { WmiDataProvider = fakeWmiProvider } };
        }



        private void AssertGeneratedFileItem(ItemType generatedItem, string expectedFilePath, string expectedPath, string expectedFileName)
        {
            Assert.IsInstanceOfType(generatedItem, typeof(file_item), "The type of generated File Item must be 'file_item'");

            file_item fileItem = (file_item)generatedItem;
            if (string.IsNullOrEmpty(expectedFilePath))
            {
                Assert.AreEqual(expectedPath, fileItem.path.Value, "A generated File Item with an unexpected 'path' value was found.");
                Assert.AreEqual(expectedFileName, fileItem.filename.Value, "A generated File Item with an unexpected 'filename' value was found.");
                return;
            }
            Assert.AreEqual(expectedFilePath, fileItem.filepath.Value, "A generated File Item with an unexpected 'filepath' value was found.");
        }

        private void AssertGeneratedFileEffectiveRightsItem(ItemType generatedItem, string expectedFilePath, string expectedPath, string expectedFileName, string expectedTrusteeName)
        {
            Assert.IsInstanceOfType(generatedItem, typeof(fileeffectiverights_item), "The type of generated file effective rights item must be 'fileeffectiverights_item'");

            fileeffectiverights_item fileItem = (fileeffectiverights_item)generatedItem;
            Assert.AreEqual(expectedTrusteeName, fileItem.trustee_name.Value, "A generated file effective rights item with an unexpected 'trustee_name' value was found.");
            if (string.IsNullOrEmpty(expectedFilePath))
            {
                Assert.AreEqual(expectedPath, fileItem.path.Value, "A generated file effective rights item with an unexpected 'path' value was found.");
                Assert.AreEqual(expectedFileName, fileItem.filename.Value, "A generated file effective rights item with an unexpected 'filename' value was found.");
                return;
            }
            Assert.AreEqual(expectedFilePath, fileItem.filepath.Value, "A generated file effective rights item with an unexpected 'filepath' value was found.");
        }

        private void AssertGeneratedSidItem(ItemType generatedItem, string trusteeName)
        {
            Assert.IsInstanceOfType(generatedItem, typeof(sid_item), "The type of generated sid item must be 'sid_item'");
            Assert.AreEqual(trusteeName, ((sid_item)generatedItem).trustee_name.Value, "A generated sid item with an unexpected 'trustee_name' value was found.");
        }

        private void AssertGeneratedSid_SidItem(ItemType generatedItem, string trusteeSID)
        {
            Assert.IsInstanceOfType(generatedItem, typeof(sid_sid_item), "The type of generated sid item must be 'sid_sid_item'");
            Assert.AreEqual(trusteeSID, ((sid_sid_item)generatedItem).trustee_sid.Value, "A generated sid item with an unexpected 'trustee_sid' value was found.");
        }

        private void AssertAllGeneratedSidItems(IList<ItemType> generatedItems, string[] expectedTrusteeNames)
        {
            Assert.IsNotNull(generatedItems, "The result of GetItemsToCollect cannot be null.");
            Assert.AreEqual(expectedTrusteeNames.Count(), generatedItems.Count, "Unexpected items count was found.");
            for (int i = 0; i < generatedItems.Count; i++)
                this.AssertGeneratedSidItem(generatedItems[i], expectedTrusteeNames[i]);
        }

        





        private WmiObject createFakeWmiObject(string completeFileName, string fileType, int fileSize)
        {
            WmiObject fakeWmiObject = new WmiObject();
            fakeWmiObject.Add("Name", completeFileName);
            fakeWmiObject.Add(GeneratedFileItemAttributes.FileSize.ToString(), fileSize);
            fakeWmiObject.Add(GeneratedFileItemAttributes.CreationDate.ToString(), "20081217165939.013961 - 120");
            fakeWmiObject.Add(GeneratedFileItemAttributes.LastModified.ToString(), "20090116171121.673899 - 120");
            fakeWmiObject.Add(GeneratedFileItemAttributes.LastAccessed.ToString(), "20081217171525.895263 - 120");
            fakeWmiObject.Add(GeneratedFileItemAttributes.Version.ToString(), "1");
            fakeWmiObject.Add(GeneratedFileItemAttributes.FileType.ToString(), fileType);
            fakeWmiObject.Add(GeneratedFileItemAttributes.Manufacturer.ToString(), "Modulo");
            return fakeWmiObject;
        }
        
        #endregion
    }
}
