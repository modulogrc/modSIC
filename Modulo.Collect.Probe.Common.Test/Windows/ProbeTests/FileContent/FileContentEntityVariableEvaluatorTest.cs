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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.FileContent;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Windows.Test.FileContent
{
    /// <summary>
    /// Summary description for FileContentEntityVariableEvaluatorTest
    /// </summary>
    [TestClass]
    public class FileContentEntityVariableEvaluatorTest
    {

        private const string DEFINITIONS_WITH_CONST_VARIABLES = "definitionsWithConstantVariable.xml";        
        private const string OBJ_ID_PATTERN = "oval:modulo:obj:{0}";
        private const string VAR_2001_ID_PATH = "oval:modulo:var:2001";
        private const string VAR_2002_ID_FILENAME = "oval:modulo:var:2002";
        private const string VAR_2003_ID_LINE = "oval:modulo:var:2003";
        private const string INVALID_VARIABLE_VALUE_FOUND = "An invalid variable value was found.";
        private const string UNEXPECTED_AMOUNT_OF_ENTITIES = "Unexpected amount of entities.";
        private const string FILE_OBJECT_NOT_FOUND = "There is no file_object in the oval_definitions file ('{0}').";
        

        [TestMethod, Owner("imenescal")]
        public void Should_be_possible_to_process_entity_that_contains_a_reference_to_a_variable()
        {
            var fileContentObject2000 = this.GetFileContentObjectToTest(DEFINITIONS_WITH_CONST_VARIABLES, "2000");
            var fakeVariablesValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariablesValues.Add(VAR_2001_ID_PATH,  new string[] { "c:\\temp" } );
            fakeVariablesValues.Add(VAR_2002_ID_FILENAME, new string[] { "test.txt" });
            fakeVariablesValues.Add(VAR_2003_ID_LINE,  new string[] { "test" } );
            var fakeEvaluatedVariables = VariableHelper.CreateEvaluatedVariables(fileContentObject2000.id, fakeVariablesValues);

            var fileContentVariableEvaluator = new FileContentEntityVariableEvaluator(fakeEvaluatedVariables);
            IEnumerable<ObjectType> variables = fileContentVariableEvaluator.ProcessVariables(fileContentObject2000);
            
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count(), UNEXPECTED_AMOUNT_OF_ENTITIES);
            var fileContentObject = (textfilecontent_object)variables.First();
            
            var  path = (EntityObjectStringType) fileContentObject.GetItemValue(textfilecontent_ItemsChoices.path);
            Assert.AreEqual(@"c:\temp", path.Value, INVALID_VARIABLE_VALUE_FOUND);
            
            var line = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.line);
            Assert.AreEqual("test", line.Value, INVALID_VARIABLE_VALUE_FOUND);
            
            var fileName = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.filename);
            Assert.AreEqual("test.txt", fileName.Value, INVALID_VARIABLE_VALUE_FOUND);
        }

        [TestMethod, Owner("imenescal")]
        public void Should_be_possible_to_process_path_entity_that_contains_a_reference_to_a_variable_with_multi_values()
        {
            var fileContentObject2000 = this.GetFileContentObjectToTest(DEFINITIONS_WITH_CONST_VARIABLES, "2000");

            var fakeVariablesValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariablesValues.Add(VAR_2001_ID_PATH, new string[] { "c:\\temp", "c:\\Windows" });
            
            var fakeEvaluatedVariables = VariableHelper.CreateEvaluatedVariables(fileContentObject2000.id, fakeVariablesValues);
            var fileContentVariableEvaluator = new FileContentEntityVariableEvaluator(fakeEvaluatedVariables);
            var variables = fileContentVariableEvaluator.ProcessVariables(fileContentObject2000);

            Assert.IsNotNull(variables);
            Assert.AreEqual(2, variables.Count(), UNEXPECTED_AMOUNT_OF_ENTITIES);

            var fileContentObject = (textfilecontent_object)variables.ElementAt(0);
            var path = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.path);
            Assert.AreEqual(@"c:\temp", path.Value, INVALID_VARIABLE_VALUE_FOUND);

            fileContentObject = (textfilecontent_object)variables.ElementAt(1);
            path = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.path);
            Assert.AreEqual(@"c:\Windows", path.Value, INVALID_VARIABLE_VALUE_FOUND);
        }

        [TestMethod, Owner("imenescal")]
        public void Should_be_possible_to_process_fileName_entity_that_contains_a_reference_to_a_variable_with_multi_values()
        {
            var fileContentObject2000 = this.GetFileContentObjectToTest(DEFINITIONS_WITH_CONST_VARIABLES, "2000");
            var fakeVariablesValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariablesValues.Add(VAR_2002_ID_FILENAME, new string[] { "file1.txt", "file2.txt", "file3.txt" });

            var fakeEvaluatedVariables = VariableHelper.CreateEvaluatedVariables(fileContentObject2000.id, fakeVariablesValues);
            var fileContentVariableEvaluator = new FileContentEntityVariableEvaluator(fakeEvaluatedVariables);
            IEnumerable<ObjectType> variables = fileContentVariableEvaluator.ProcessVariables(fileContentObject2000);

            Assert.IsNotNull(variables);
            Assert.AreEqual(3, variables.Count(), UNEXPECTED_AMOUNT_OF_ENTITIES);

            var fileContentObject = (textfilecontent_object)variables.ElementAt(0);
            var fileName = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.filename);
            Assert.AreEqual(@"file1.txt", fileName.Value, INVALID_VARIABLE_VALUE_FOUND);

            fileContentObject = (textfilecontent_object)variables.ElementAt(1);
            fileName = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.filename);
            Assert.AreEqual(@"file2.txt", fileName.Value, INVALID_VARIABLE_VALUE_FOUND);

            fileContentObject = (textfilecontent_object)variables.ElementAt(2);
            fileName = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.filename);
            Assert.AreEqual(@"file3.txt", fileName.Value, INVALID_VARIABLE_VALUE_FOUND);
            
        }

        [TestMethod, Owner("imenescal")]
        public void Should_be_possible_to_process_line_entity_that_contains_a_reference_to_a_variable_with_multi_values()
        {
            var fileContentObject2000 = this.GetFileContentObjectToTest(DEFINITIONS_WITH_CONST_VARIABLES, "2000");
            var fakeVariablesValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariablesValues.Add(VAR_2003_ID_LINE, new string[] { "Line1", "Line 2" });

            var fakeEvaluatedVariables = VariableHelper.CreateEvaluatedVariables(fileContentObject2000.id, fakeVariablesValues);
            var fileContentVariableEvaluator = new FileContentEntityVariableEvaluator(fakeEvaluatedVariables);
            var variables = fileContentVariableEvaluator.ProcessVariables(fileContentObject2000);

            Assert.IsNotNull(variables);
            Assert.AreEqual(2, variables.Count(), UNEXPECTED_AMOUNT_OF_ENTITIES);

            var fileContentObject = (textfilecontent_object)variables.ElementAt(0);
            var line = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.line);
            Assert.AreEqual("Line1", line.Value, INVALID_VARIABLE_VALUE_FOUND);

            fileContentObject = (textfilecontent_object)variables.ElementAt(1);
            line = (EntityObjectStringType)fileContentObject.GetItemValue(textfilecontent_ItemsChoices.line);
            Assert.AreEqual("Line 2", line.Value, INVALID_VARIABLE_VALUE_FOUND);
            
        }

        private textfilecontent_object GetFileContentObjectToTest(string definitionsFileName, string objectNumber)
        {
            var objectID = string.Format(OBJ_ID_PATTERN, objectNumber);
            var definitions = ProbeHelper.GetFakeOvalDefinitions(definitionsFileName);
            var fileContentObject2001 = (textfilecontent_object)ProbeHelper.GetOvalComponentByOvalID(definitions, objectID);

            var assertFailMessage = String.Format(FILE_OBJECT_NOT_FOUND, definitionsFileName);
            Assert.IsNotNull(fileContentObject2001, assertFailMessage);
            
            return fileContentObject2001;
        }

        private string getFileNameEntityValueFromFileObject(file_object fileObject)
        {
            var allFileEntities = OvalHelper.GetFileEntitiesFromObjectType(fileObject);
            return allFileEntities[file_object_ItemsChoices.filename.ToString()].Value;
        }
    }
}
