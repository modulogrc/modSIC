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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Common.Test
{
    [TestClass]
    public class VariablesEvaluatedTest
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private const string DEFINITIONS_SIMPLE = "definitionsSimple.xml";
        
        private const string VARIABLE_ID_11 = "oval:modulo:var:11";
        private const string OBJECT_ID_1010 = "oval:modulo:obj:1010";

        private const string MSG_FOR_INVALID_EVALUATION_RESULT = "Unexpected variable value was found after evaluation";

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_variable_by_ovalComponentId()
        {
            List<string> variableValues = new List<string>() { "Multiprocessor Free" };
            VariableValue variable = new VariableValue("oval:org.mitre.oval:obj:6000", "oval:com.hp:var:1", variableValues);
            IEnumerable<VariableValue> variables = new List<VariableValue>() { variable };
            VariablesEvaluated variablesEvaluated = new VariablesEvaluated(variables);

            IEnumerable<VariableValue> variablesExpected = variablesEvaluated.GetVariableValueForOvalComponent("oval:org.mitre.oval:obj:6000");
            Assert.IsNotNull(variablesExpected, "the variable was not found");
            variablesExpected = variablesEvaluated.GetVariableValueForOvalComponent("oval:org.mitre.oval:obj:6005");
            Assert.IsTrue(variablesExpected.Count() == 0, "the variable is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_variable_by_variableId()
        {
            List<string> variableValues = new List<string>() { "Multiprocessor Free" };
            VariableValue variable = new VariableValue("oval:org.mitre.oval:obj:6000", "oval:com.hp:var:1", variableValues);
            IEnumerable<VariableValue> variables = new List<VariableValue>() { variable };
            VariablesEvaluated variablesEvaluated = new VariablesEvaluated(variables);

            IEnumerable<VariableValue> variablesExpected = variablesEvaluated.GetVariableValueForVariableId("oval:com.hp:var:1");
            Assert.IsNotNull(variablesExpected, "the variable was not found");
            variablesExpected = variablesEvaluated.GetVariableValueForVariableId("oval:com.hp:var:4");
            Assert.AreEqual(0, variablesExpected.Count(), "the variable is not expected");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_evaluate_variable_for_a_entity()
        {
            string variableValue;
            var objectType = (fileeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID(DEFINITIONS_SIMPLE, OBJECT_ID_1010);
            var evaluator = this.CreateVariableEntityEvaluator(DEFINITIONS_SIMPLE, OBJECT_ID_1010, VARIABLE_ID_11, out variableValue);

            
            var evaluationResult = evaluator.EvaluateVariableForEntity(objectType.GetAllObjectEntities()["filepath"]);


            this.DoBasicAssertForEntityVariableEvaluation(evaluationResult.ToArray(), 1);
            Assert.AreEqual(variableValue, evaluationResult.First(), MSG_FOR_INVALID_EVALUATION_RESULT);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_there_is_no_variable_reference_the_entity_variable_evaluation_must_return_the_own_entity_value()
        {
            var objectType = (fileeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID(DEFINITIONS_SIMPLE, OBJECT_ID_1010);
            var variableEntityEvaluator = new VariableEntityEvaluator(VariableHelper.CreateEmptyEvaluatedVariables());
            var trusteeSIDEntity = objectType.GetAllObjectEntities()[fileeffectiverights53_object_ItemsChoices.trustee_sid.ToString()];
            var evaluationResult = variableEntityEvaluator.EvaluateVariableForEntity(trusteeSIDEntity);

            this.DoBasicAssertForEntityVariableEvaluation(evaluationResult.ToArray(), 1);
            Assert.AreEqual(trusteeSIDEntity.Value, evaluationResult.First(), MSG_FOR_INVALID_EVALUATION_RESULT);
        }

        [TestMethod, Owner("lfernandes")]
        public void For_a_null_entity_the_result_of_variable_evaluation_must_be_a_empty_list()
        {
            var pathEntityName = fileeffectiverights53_object_ItemsChoices.path.ToString();
            var objectType = (fileeffectiverights53_object)ProbeHelper.GetDefinitionObjectTypeByID(DEFINITIONS_SIMPLE, OBJECT_ID_1010);
            var evaluator = this.CreateVariableEntityEvaluatorForNonReferencedVariable();

            var evaluationResult = evaluator.EvaluateVariableForEntity(objectType.GetAllObjectEntities()[pathEntityName]);

            this.DoBasicAssertForEntityVariableEvaluation(evaluationResult.ToArray(), 0);
        }

        private VariableEntityEvaluator CreateVariableEntityEvaluator(string definitionsFileName, string objectID, string variableID, out string variableValue)
        {
            var definitions = ProbeHelper.GetFakeOvalDefinitions(definitionsFileName);
            var objectType = (fileeffectiverights53_object)ProbeHelper.GetOvalComponentByOvalID(definitions, objectID);
            variableValue = VariableHelper.ExtractVariableValueFromConstantVariable(definitions, variableID);
            var fakeEvaluatedVariables = VariableHelper.CreateVariableWithOneValue(objectType.id, variableID, variableValue);
            return new VariableEntityEvaluator(fakeEvaluatedVariables);
        }

        private VariableEntityEvaluator CreateVariableEntityEvaluatorForNonReferencedVariable()
        {
            var fakeEvaluatedVariables = VariableHelper.CreateEmptyEvaluatedVariables();
            return new VariableEntityEvaluator(fakeEvaluatedVariables);
        }

        private void DoBasicAssertForEntityVariableEvaluation(string[] evaluationResultToAssert, int expectedResultcount)
        {
            Assert.IsNotNull(evaluationResultToAssert, "The result of variable evaluation cannot be null");
            Assert.AreEqual(expectedResultcount, evaluationResultToAssert.Count(), "Unexpected result count of variable evaluation was found.");
        }

    }
}
