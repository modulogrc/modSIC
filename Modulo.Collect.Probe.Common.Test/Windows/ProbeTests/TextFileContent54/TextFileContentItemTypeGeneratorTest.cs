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
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Rhino.Mocks;
using Definitions = Modulo.Collect.OVAL.Definitions;
using System.IO;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Windows.Test.TextFileContent54
{
    [TestClass]
    public class TextFileContentItemTypeGeneratorTest
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_possible_to_generate_textfilecontentItems_from_simple_textfilecontentObject()
        {
            #region
            //<textfilecontent54_object	id="oval:modulo:obj:900" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#independent">
            //    <path>C:\Windows\System32\drivers\etc</path>
            //    <filename>hosts</filename>
            //    <pattern>64.4.20.252 v4.windowsupdate.microsoft.com</pattern>
            //    <instance datatype="int">1</instance>
            //</textfilecontent54_object> 
            #endregion
            
            var fakeFileLinesContent = new string[] { "64.4.20.252 v4.windowsupdate.microsoft.com" };
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "900");
            var itemTypeGenerator = this.CreateItemTypeGeneratorWithMockedBehavior(fakeFileLinesContent, false);

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectType, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(textfilecontent_item));
            AssertGeneratedTextFileContentItem(
                generatedItems.Single(), 
                @"C:\Windows\System32\drivers\etc\hosts", 
                "64.4.20.252 v4.windowsupdate.microsoft.com", 
                "1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_textfilecontentItems_with_referenced_variables()
        {
            #region
            //<textfilecontent54_object	id="oval:modulo:obj:910" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#independent">
            //    <path var_ref="oval:modulo:var:911"/> "c:\windows"
            //    <filename>win.ini</filename>
            //    <pattern var_ref="oval:modulo:var:912"/> "MAPIXVER"
            //    <instance datatype="int" var_ref="oval:modulo:var:913"/> 1
            //</textfilecontent54_object>
            #endregion

            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "910");
            var variableIDs = new string[] { "911", "912", "913" };
            var variableValues = new string[] { "c:\\windows", "MAPIXVER", "1" };
            var fakeVariables = this.CreateFakeVariables(objectType.id, variableIDs, variableValues);
            var fakeFileLinesContent = new string[] { "MAPIXVER=1.0" };
            var itemTypeGenerator = CreateItemTypeGeneratorWithMockedBehavior(fakeFileLinesContent, false);

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectType, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(textfilecontent_item));
            AssertGeneratedTextFileContentItem(generatedItems.Single(), @"c:\windows\win.ini", "MAPIXVER", "1");
            ItemTypeEntityChecker.AssertItemTypeEntity(((textfilecontent_item)generatedItems.Single()).text, "MAPIXVER=1.0");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_textfilecontentItems_from_textfilecontentObject_with_referenced_variables_with_multiple_values_in_all_entities()
        {
            var objectType = (textfilecontent54_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "oval:modulo:obj:960");
            var fakeVariables = this.CreateFakeEvaluateVariablesForAllEntities(objectType.id);
            var itemTypeGenerator = CreateItemTypeGeneratorWithMockedBehavior(new string[] { "does not care" }, true);
            
            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectType, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 8, typeof(textfilecontent_item));
            AssertGeneratedTextFileContentItem(generatedItems.ElementAt(0), @"c:\windows\boot.ini", "^Services.*", "1");
            AssertGeneratedTextFileContentItem(generatedItems.ElementAt(1), @"c:\windows\boot.ini", "^Application.*", "1");
            AssertGeneratedTextFileContentItem(generatedItems.ElementAt(2), @"c:\windows\win.ini", "^Services.*", "1");
            AssertGeneratedTextFileContentItem(generatedItems.ElementAt(3), @"c:\windows\win.ini", "^Application.*", "1");
            AssertGeneratedTextFileContentItem(generatedItems.ElementAt(4), @"c:\windows NT\boot.ini", "^Services.*", "1");
            AssertGeneratedTextFileContentItem(generatedItems.ElementAt(5), @"c:\windows NT\boot.ini", "^Application.*", "1");
            AssertGeneratedTextFileContentItem(generatedItems.ElementAt(6), @"c:\windows NT\win.ini", "^Services.*", "1");
            AssertGeneratedTextFileContentItem(generatedItems.ElementAt(7), @"c:\windows NT\win.ini", "^Application.*", "1");
        }

        [TestMethod, Owner("lfernandes")]
        public void The_result_of_item_generation_should_be_a_list_with_items_already_collected()
        {
            #region
            //<textfilecontent54_object id="oval:modulo:obj:920">
            //    <filepath>c:\windows\win.ini</filepath>
            //    <pattern>VERSION.*</pattern>
            //    <instance datatype="int">1</instance>
            //</textfilecontent54_object>
            #endregion
            var fakeFileLines = new string[] { "VERSION=10.1" };
            var sourceObjectType = (textfilecontent54_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "920");
            var itemTypeGenerator = CreateItemTypeGeneratorWithMockedBehavior(fakeFileLines, false);

            var result = itemTypeGenerator.GetItemsToCollect(sourceObjectType, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(result, 1, typeof(textfilecontent_item));
            AssertGeneratedTextFileContentItem(result.Single(), @"c:\windows\win.ini", "VERSION.*", "1");
            ItemTypeEntityChecker.AssertItemTypeEntity(((textfilecontent_item)result.Single()).text, "VERSION=10.1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_an_object_with_referenced_variable_and_pattern_match_operation_at_same_time()
        {
            #region
            //<textfilecontent54_object id="oval:modulo:obj:991">
            //    <path var_ref="oval:modulo:var:911" />
            //    <filename operation="pattern match">.*.ini</filename>
            //    <pattern>3gp</pattern>
            //    <instance datatype="int">2</instance>
            //</textfilecontent54_object>
            #endregion
            var fakeFileLines = new string[] { "3gp=MPEGVideo" };
            var fakeFilepaths = new string[] { @"c:\windows\boot.ini", @"c:\windows\file.txt", @"c:\windows\win.ini" };
            var sourceObjectType = (textfilecontent54_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "991");
            var fakeEvaluatedVariables = CreateFakeEvaluateVariablesForAllEntities(sourceObjectType.id);
            var itemTypeGenerator = CreateItemTypeGeneratorWithMockedBehavior(fakeFileLines, true, fakeFilepaths);
            var expectedPaths = new String[] 
            { 
                @"c:\windows\boot.ini",
                @"c:\windows\win.ini",
                @"c:\windows NT\boot.ini",
                @"c:\windows NT\win.ini"
            };

            
            var result = itemTypeGenerator.GetItemsToCollect(sourceObjectType, fakeEvaluatedVariables).ToArray();


            ItemTypeChecker.DoBasicAssertForItems(result, 4, typeof(textfilecontent_item));
            AssertGeneratedTextFileContentItem(result.ElementAt(0), expectedPaths.ElementAt(0), "3gp", "2");
            AssertGeneratedTextFileContentItem(result.ElementAt(1), expectedPaths.ElementAt(1), "3gp", "2");
            AssertGeneratedTextFileContentItem(result.ElementAt(2), expectedPaths.ElementAt(2), "3gp", "2");
            AssertGeneratedTextFileContentItem(result.ElementAt(3), expectedPaths.ElementAt(3), "3gp", "2");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_complete_filepath_from_textfilecontent_item()
        {
            var expectedFilepath = @"c:\windows\win.ini";
            
            var textFileContentItem1 = 
                new textfilecontent_item() 
                { 
                    filepath = OvalHelper.CreateItemEntityWithStringValue(expectedFilepath) 
                };

            var textFileContentItem2 =
                new textfilecontent_item()
                {
                    path = OvalHelper.CreateItemEntityWithStringValue(Path.GetDirectoryName(expectedFilepath)),
                    filename = OvalHelper.CreateItemEntityWithStringValue(Path.GetFileName(expectedFilepath))
                };


            Assert.AreEqual(expectedFilepath, textFileContentItem1.GetCompleteFilepath());
            Assert.AreEqual(expectedFilepath, textFileContentItem2.GetCompleteFilepath());
            Assert.AreEqual(String.Empty, new textfilecontent_item().GetCompleteFilepath());
            
            Assert.AreEqual(String.Empty, 
                new textfilecontent_item() { path = OvalHelper.CreateItemEntityWithStringValue("a") }
                    .GetCompleteFilepath());

            Assert.AreEqual(String.Empty,
                new textfilecontent_item() { filename = OvalHelper.CreateItemEntityWithStringValue("a") }
                    .GetCompleteFilepath());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_an_object_with_pattern_match_operation_in_pattern_entity()
        {
            var fakeObjectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "993");
            var itemTypeGenerator = 
                CreateItemTypeGeneratorWithMockedBehavior(new string[] { "3gp=MPEGVideo" }, false, null);

            var generatedItems = itemTypeGenerator.GetItemsToCollect(fakeObjectType, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(textfilecontent_item));
            AssertGeneratedTextFileContentItem(generatedItems.Single(), @"c:\windows\win.ini", "^3gp.*", "1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_an_object_with_GreaterThanOrEqual_operation_in_instance_entity()
        {
            var sourceObjectType = (textfilecontent54_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "995");
            var fakeFileLines = new String[] { "Linha1\r\nLinha2\r\nLinha3\r\nLinha4\r\nLinha5\r\n" };
            var itemTypeGenerator = CreateItemTypeGeneratorWithMockedBehavior(fakeFileLines, false);

            var result = itemTypeGenerator.GetItemsToCollect(sourceObjectType, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(result, 1, typeof(textfilecontent_item));

        }

        private void AssertGeneratedTextFileContentItem(
            ItemType itemToAssert, string expectedPath, string expectedPattern, string expectedInstance)
        {
            var textFileContentItem = (textfilecontent_item)itemToAssert;
            ItemTypeEntityChecker.AssertItemTypeEntity(textFileContentItem.filepath, expectedPath);
            ItemTypeEntityChecker.AssertItemTypeEntity(textFileContentItem.path, Path.GetDirectoryName(expectedPath));
            ItemTypeEntityChecker.AssertItemTypeEntity(textFileContentItem.filename, Path.GetFileName(expectedPath));
        }


        private TextFileContentItemTypeGenerator CreateItemTypeGeneratorWithMockedBehavior(
            string[] fakeFileLinesContent, bool repeatAny, string[] fakeFilepathsToReturn = null)
        {
            var mocks = new MockRepository();
            var fakeObjectCollector = mocks.DynamicMock<BaseObjectCollector>();

            if (repeatAny)
                Expect.Call(fakeObjectCollector.GetValues(null))
                    .IgnoreArguments().Repeat.Any()
                        .Return(fakeFileLinesContent);
            else
                Expect.Call(fakeObjectCollector.GetValues(null))
                    .IgnoreArguments()
                        .Return(fakeFileLinesContent);

            IFileProvider fileProvider = null;
            if (fakeFilepathsToReturn != null)
            {
                fileProvider = mocks.DynamicMock<IFileProvider>();
                Expect.Call(fileProvider.GetFileChildren(null))
                    .IgnoreArguments()
                        .Return(fakeFilepathsToReturn);
            }
                
            
            mocks.ReplayAll();

            return new TextFileContentItemTypeGenerator()
            {
                OperationEvaluator =
                    new TextFileContentEntityOperationEvaluator(fakeObjectCollector, fileProvider, FamilyEnumeration.windows)
            };
        }

        private VariablesEvaluated CreateFakeVariables(
            string objectID, string[] variableIDs, string[] values)
        {
            if (!objectID.Contains(":"))
                objectID = string.Format("oval:modulo:obj:{0}", objectID);
            
            IEnumerable<VariableValue> newVariablesValues = new List<VariableValue>();
            for (int i = 0; i < variableIDs.Count(); i++)
            {
                string variableID = variableIDs[i].Contains(":") ? variableIDs[i] : string.Format("oval:modulo:var:{0}", variableIDs[i]);
                ((List<VariableValue>)newVariablesValues).Add(new VariableValue(objectID, variableID, new string[] { values[i] }));
            }

            return new VariablesEvaluated(newVariablesValues);
        }

        private VariablesEvaluated CreateFakeEvaluateVariablesForAllEntities(string objectID)
        {
            var fakeVariableValues = new Dictionary<String, IEnumerable<String>>();
            fakeVariableValues.Add("oval:modulo:var:911", new string[] { "c:\\windows", "c:\\windows NT" });
            fakeVariableValues.Add("oval:modulo:var:914", new string[] { "boot.ini", "win.ini" });
            fakeVariableValues.Add("oval:modulo:var:912", new string[] { "^Services.*", "^Application.*" });
            fakeVariableValues.Add("oval:modulo:var:916", new string[] { "1" });
            
            return VariableHelper.CreateEvaluatedVariables(objectID, fakeVariableValues);
        }
    }
}
