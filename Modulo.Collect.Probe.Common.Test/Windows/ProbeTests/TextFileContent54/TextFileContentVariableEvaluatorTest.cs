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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Windows.Test.TextFileContent54
{
    [TestClass]
    public class TextFileContentVariableEvaluatorTest
    {
        private string OBJECT_TYPE_920 = "oval:modulo:obj:920";
        private string OBJECT_TYPE_930 = "oval:modulo:obj:930";
        private string OBJECT_TYPE_940 = "oval:modulo:obj:940";
        private string OBJECT_TYPE_950 = "oval:modulo:obj:950";
        private string OBJECT_TYPE_960 = "oval:modulo:obj:960";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variables_for_textfilecontent_which_was_definied_with_filepathEntity()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", OBJECT_TYPE_920);
            var fakeVariableValues = new Dictionary<String, IEnumerable<String>>();
            fakeVariableValues.Add("oval:modulo:var:911", new string[] { "c:\\windows" });
            fakeVariableValues.Add("oval:modulo:var:914", new string[] { "win.ini" });
            var fakeVariables = VariableHelper.CreateEvaluatedVariables(OBJECT_TYPE_920, fakeVariableValues);
            var variableEvaluator = new TextFileContentVariableEvaluator(fakeVariables);

            var evaluationVariableResult = variableEvaluator.ProcessVariablesForTypeFilePathEntities(objectType);

            this.DoBasicAssert(evaluationVariableResult, 1);
            Assert.AreEqual(@"c:\windows\win.ini", evaluationVariableResult.ElementAt(0), "Unexpected variable value was found after variables evaluation.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_multiple_variables_for_textfilecontent_which_was_definied_with_path_and_filename_entities()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", OBJECT_TYPE_930);
            var fakeVariableValues = new Dictionary<String, IEnumerable<String>>();
            fakeVariableValues.Add("oval:modulo:var:911", new string[] { "c:\\windows", "c:\\windows NT" });
            fakeVariableValues.Add("oval:modulo:var:914", new string[] { "boot.ini", "win.ini" });
            var fakeVariables = VariableHelper.CreateEvaluatedVariables(OBJECT_TYPE_930, fakeVariableValues);
            TextFileContentVariableEvaluator variableEvaluator = new TextFileContentVariableEvaluator(fakeVariables);

            var evaluationVariableResult = variableEvaluator.ProcessVariablesForTypeFilePathEntities(objectType);

            this.DoBasicAssert(evaluationVariableResult, 4);
            Assert.AreEqual(@"c:\windows\boot.ini", evaluationVariableResult.ElementAt(0), "Unexpected variable value was found after variables evaluation.");
            Assert.AreEqual(@"c:\windows\win.ini", evaluationVariableResult.ElementAt(1), "Unexpected variable value was found after variables evaluation.");
            Assert.AreEqual(@"c:\windows NT\boot.ini", evaluationVariableResult.ElementAt(2), "Unexpected variable value was found after variables evaluation.");
            Assert.AreEqual(@"c:\windows NT\win.ini", evaluationVariableResult.ElementAt(3), "Unexpected variable value was found after variables evaluation.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variables_for_textfilecontent_which_was_definied_with_path_and_filename_entities()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", OBJECT_TYPE_940);
            var fakeVariableValues = new Dictionary<String, IEnumerable<String>>();
            fakeVariableValues.Add("oval:modulo:var:911", new string[] { "c:\\windows" });
            fakeVariableValues.Add("oval:modulo:var:914", new string[] { "win.ini" });
            var fakeVariables = VariableHelper.CreateEvaluatedVariables(OBJECT_TYPE_940, fakeVariableValues);
            TextFileContentVariableEvaluator variableEvaluator = new TextFileContentVariableEvaluator(fakeVariables);

            var evaluationVariableResult = variableEvaluator.ProcessVariablesForTypeFilePathEntities(objectType);

            this.DoBasicAssert(evaluationVariableResult, 1);
            Assert.AreEqual(@"c:\windows\win.ini", evaluationVariableResult.ElementAt(0), "Unexpected variable value was found after variables evaluation.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_multiple_variables_for_textfilecontent_which_was_definied_with_filepath_entity()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", OBJECT_TYPE_950);
            var fakeVariableValues = new Dictionary<String, IEnumerable<String>>();
            fakeVariableValues.Add("oval:modulo:var:915", new string[] { "c:\\windows\\win.ini", "c:\\boot.ini" });
            var fakeVariables = VariableHelper.CreateEvaluatedVariables(OBJECT_TYPE_950, fakeVariableValues);
            TextFileContentVariableEvaluator variableEvaluator = new TextFileContentVariableEvaluator(fakeVariables);

            var evaluationVariableResult = variableEvaluator.ProcessVariablesForTypeFilePathEntities(objectType);

            this.DoBasicAssert(evaluationVariableResult, 2);
            Assert.AreEqual(@"c:\windows\win.ini", evaluationVariableResult.ElementAt(0), "Unexpected variable value was found after variables evaluation.");
            Assert.AreEqual(@"c:\boot.ini", evaluationVariableResult.ElementAt(1), "Unexpected variable value was found after variables evaluation.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variables_for_all_entities()
        {
            #region
            //<textfilecontent54_object	id="oval:modulo:obj:960" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#independent">
            //    <path var_ref="oval:modulo:var:911"/>
            //    <filename var_ref="oval:modulo:var:914"/>
            //    <pattern var_ref="oval:modulo:var:912"/>
            //    <instance datatype="int" var_ref="oval:modulo:var:915"/>
            //</textfilecontent54_object>
            #endregion
            
            var objectType = (textfilecontent54_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", OBJECT_TYPE_960);
            var fakeVariables = createFakeEvaluateVariablesForAllEntities();
            TextFileContentVariableEvaluator variableEvaluator = new TextFileContentVariableEvaluator(fakeVariables);

            var resultForFilePathEntities = variableEvaluator.ProcessVariablesForTypeFilePathEntities(objectType);
            this.DoBasicAssert(resultForFilePathEntities, 1);
            Assert.AreEqual(@"c:\windows\win.ini", resultForFilePathEntities.ElementAt(0), "Unexpected variable value was found after variables evaluation.");

            var resultForPatternEntity = variableEvaluator.EvaluateVariableForEntity(objectType, textfilecontent54_ItemsChoices.pattern);
            this.DoBasicAssert(resultForPatternEntity, 1);
            Assert.AreEqual(@"^Services.*", resultForPatternEntity.ElementAt(0), "Unexpected variable value was found after variables evaluation.");

            var resultForInstanceEntity = variableEvaluator.EvaluateVariableForEntity(objectType, textfilecontent54_ItemsChoices.instance);
            this.DoBasicAssert(resultForInstanceEntity, 1);
            Assert.AreEqual(@"2", resultForInstanceEntity.ElementAt(0), "Unexpected variable value was found after variables evaluation.");
        }


        private void DoBasicAssert(IEnumerable<string> evaluationVariableResult, int expectedResultCount)
        {
            Assert.IsNotNull(evaluationVariableResult, "The result of evaluation variable cannot be null.");
            Assert.AreEqual(expectedResultCount, evaluationVariableResult.Count(), "Unexpected evaluation variable result count were found.");
        }

        private VariablesEvaluated createFakeEvaluateVariablesForAllEntities()
        {
            var fakeVariableValues = new Dictionary<String, IEnumerable<String>>();
            fakeVariableValues.Add("oval:modulo:var:911", new string[] { "c:\\windows" });
            fakeVariableValues.Add("oval:modulo:var:914", new string[] { "win.ini" });
            fakeVariableValues.Add("oval:modulo:var:912", new string[] { "^Services.*" });
            fakeVariableValues.Add("oval:modulo:var:916", new string[] { "2" });
            var fakeVariables = VariableHelper.CreateEvaluatedVariables(OBJECT_TYPE_960, fakeVariableValues);
            return fakeVariables;
        }
    }
}
