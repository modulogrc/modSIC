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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Windows.Test.File
{
    [TestClass]
    public class FileEntityVariableEvaluatorTest
    {
        
        private const string DEFINITIONS_WITH_CONST_VARIABLE = "definitionsWithConstantVariable.xml";
        private const string OBJ_ID_PATTERN = "oval:modulo:obj:{0}";
        private const string VAR_1001_ID = "oval:modulo:var:1001";
        private const string VAR_1002_ID = "oval:modulo:var:1002";


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_process_path_entity_that_contains_a_reference_to_a_variable()
        {
            file_object fileObject1001 = this.GetFileObjectToTest(DEFINITIONS_WITH_CONST_VARIABLE, "1001");
            VariablesEvaluated vars = VariableHelper.CreateVariableWithOneValue(fileObject1001.id, VAR_1001_ID, "c:\\windows\\system32");

            FileEntityVariableEvaluator fileVariableEvaluator = new FileEntityVariableEvaluator(vars);
            //IEnumerable<string> derivedEntitites = fileVariableEvaluator.ProcessVariableForAllObjectEntities();
            IEnumerable<ObjectType> derivedEntities = fileVariableEvaluator.ProcessVariables(fileObject1001);
            Assert.IsNotNull(derivedEntities);

            file_object fileObject = (file_object)derivedEntities.ElementAt(0);
            
            Assert.AreEqual(1, derivedEntities.Count());
            Assert.AreEqual("c:\\windows\\system32",((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value );
            Assert.AreEqual("inetcomm.dll", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_process_path_entity_that_contains_a_reference_to_a_variable_with_multi_values()
        {
            string[] var1001Values =
                new string[] { "c:\\windows\\system32", "c:\\windows\\system", "c:\\windows\\drivers" };

            file_object fileObject1001 = this.GetFileObjectToTest(DEFINITIONS_WITH_CONST_VARIABLE, "1001"); 
            VariablesEvaluated vars = VariableHelper.CreateVariableWithMultiplesValue(fileObject1001.id, VAR_1001_ID, var1001Values);
            string filename = this.getFileNameEntityValueFromFileObject(fileObject1001);


            FileEntityVariableEvaluator fileVariableEvaluator = new FileEntityVariableEvaluator(vars);
            IEnumerable<ObjectType> derivedEntitites = fileVariableEvaluator.ProcessVariables(fileObject1001);
            Assert.IsNotNull(derivedEntitites);
            Assert.AreEqual(3, derivedEntitites.Count());

            file_object fileObject = (file_object)derivedEntitites.ElementAt(0);
            Assert.AreEqual("c:\\windows\\system32", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("inetcomm.dll", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);
            
            fileObject = (file_object)derivedEntitites.ElementAt(1);
            Assert.AreEqual("c:\\windows\\system", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("inetcomm.dll", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);

            fileObject = (file_object)derivedEntitites.ElementAt(2);
            Assert.AreEqual("c:\\windows\\drivers", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("inetcomm.dll", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);        
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_process_filepath_entity_that_contains_a_reference_to_a_variable_with_multi_values()
        {
            string[] fakeVariableValues = new string[] { "c:\\temp\\file1.txt", "c:\\temp\\file2.txt" };
            file_object fileObject1002 = this.GetFileObjectToTest(DEFINITIONS_WITH_CONST_VARIABLE, "1002");
            VariablesEvaluated vars = VariableHelper.CreateVariableWithMultiplesValue(fileObject1002.id, VAR_1002_ID, fakeVariableValues);


            FileEntityVariableEvaluator fileVariableEvaluator = new FileEntityVariableEvaluator(vars);
            IEnumerable<ObjectType> derivedEntitites = fileVariableEvaluator.ProcessVariables(fileObject1002);
            
            Assert.IsNotNull(derivedEntitites);
            Assert.AreEqual(2, derivedEntitites.Count());
            
            file_object fileObject = (file_object)derivedEntitites.ElementAt(0);
            Assert.AreEqual("c:\\temp\\file1.txt", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filepath)).Value);
            
            fileObject = (file_object)derivedEntitites.ElementAt(1);
            Assert.AreEqual("c:\\temp\\file2.txt", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filepath)).Value);        
            
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_process_fileObject_where_all_entities_have_referenced_multi_values_variables()
        {
            Dictionary<string, IEnumerable<string>> fakeVariablesValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariablesValues.Add(VAR_1001_ID,  new string[] { "c:\\tmp", "c:\\temp" });
            fakeVariablesValues.Add(VAR_1002_ID,  new string[] { "file1.txt", "file2.ext", "file3.exe" });

            file_object fileObject1003 = this.GetFileObjectToTest(DEFINITIONS_WITH_CONST_VARIABLE, "1003");
            VariablesEvaluated fakeEvaluatedVariables = VariableHelper.CreateEvaluatedVariables(fileObject1003.id, fakeVariablesValues);


            FileEntityVariableEvaluator fileVariableEvaluator = new FileEntityVariableEvaluator(fakeEvaluatedVariables);
            IEnumerable<ObjectType> derivedEntitites = fileVariableEvaluator.ProcessVariables(fileObject1003);

            Assert.IsNotNull(derivedEntitites);
            Assert.AreEqual(6, derivedEntitites.Count(), "The number of created entities is not expected.");
            
            file_object fileObject = (file_object)derivedEntitites.ElementAt(0);
            Assert.AreEqual(@"c:\tmp", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file1.txt", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);            
            fileObject = (file_object)derivedEntitites.ElementAt(1);
            Assert.AreEqual(@"c:\tmp", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file2.ext", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);
            fileObject = (file_object)derivedEntitites.ElementAt(2);
            Assert.AreEqual(@"c:\tmp", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file3.exe", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);
            fileObject = (file_object)derivedEntitites.ElementAt(3);
            Assert.AreEqual(@"c:\temp", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file1.txt", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);
            fileObject = (file_object)derivedEntitites.ElementAt(4);
            Assert.AreEqual(@"c:\temp", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file2.ext", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);
            fileObject = (file_object)derivedEntitites.ElementAt(5);
            Assert.AreEqual(@"c:\temp", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file3.exe", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_process_variable_from_fileObject_and_keep_the_operation_info_in_object_type()
        {          

            file_object fileObject1004 = this.GetFileObjectToTest(DEFINITIONS_WITH_CONST_VARIABLE, "1004");
            VariablesEvaluated vars = VariableHelper.CreateVariableWithOneValue(fileObject1004.id, VAR_1001_ID, "c:\\windows\\system32");

            FileEntityVariableEvaluator fileVariableEvaluator = new FileEntityVariableEvaluator(vars);            
            IEnumerable<ObjectType> derivedEntities = fileVariableEvaluator.ProcessVariables(fileObject1004);
            Assert.IsNotNull(derivedEntities);

            file_object fileObject = (file_object)derivedEntities.ElementAt(0);
            
            Assert.AreEqual(1, derivedEntities.Count());
            Assert.AreEqual("c:\\windows\\system32",((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.path)).Value );
            Assert.AreEqual("file.txt", ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);
            Assert.AreEqual(OperationEnumeration.notequal, ((EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename)).operation);

        }
        
        private string getFileNameEntityValueFromFileObject(file_object fileObject)
        {
            Dictionary<String, EntityObjectStringType> allFileEntities = OvalHelper.GetFileEntitiesFromObjectType(fileObject);
            return allFileEntities[file_object_ItemsChoices.filename.ToString()].Value;
        }

        private file_object GetFileObjectToTest(string definitionsFileName, string objectNumber)
        {
            string objectID = string.Format(OBJ_ID_PATTERN, objectNumber);

            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions(definitionsFileName);
            file_object fileObject1001 = (file_object)ProbeHelper.GetOvalComponentByOvalID(definitions, objectID);

            Assert.IsNotNull(fileObject1001, "the oval file object not exists in the fakeDefinitions");
            return fileObject1001;
        }
    }
}
