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
using Definitions = Modulo.Collect.OVAL.Definitions;
//using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators;
using Modulo.Collect.OVAL.Tests.helpers;

using Variables = Modulo.Collect.OVAL.Variables;

using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Variables;


namespace Modulo.Collect.OVAL.Tests.Variables
{
    /// <summary>
    /// Summary description for LocalVariableEvaluatorTest
    /// </summary>
    [TestClass]
    public class ExternalVariableEvaluatorTest
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

        

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_value_of_an_external_variable()
        {
            var externalVariable = this.CreateExternalVariable("oval:modulo:var:4000", SimpleDatatypeEnumeration.@string);
            var ovalVariables = this.CreateOvalVariables(externalVariable.id, externalVariable.datatype,  new string[] { "answer" });
            var externalVariableEvaluator = new ExternalVariableEvaluator(externalVariable, ovalVariables);
            
            IEnumerable<string> variableEvaluationResult = externalVariableEvaluator.GetValue();

            Assert.IsTrue(variableEvaluationResult.Count() > 0, "the quantity of values is not expected.");
            Assert.AreEqual("answer", variableEvaluationResult.ElementAt(0), "A variable with an unexpected value was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void When_the_external_variable_was_not_set_the_variable_evaluation_must_be_return_the_default_value_for_each_datatype()
        {
            var strExternalVar = this.CreateExternalVariable("oval:mss:var:1", SimpleDatatypeEnumeration.@string);
            var intExternalVar = this.CreateExternalVariable("oval:mss:var:2", SimpleDatatypeEnumeration.@int);
            var boolExternalVar = this.CreateExternalVariable("oval:mss:var:3", SimpleDatatypeEnumeration.boolean);
            var allExternalVariables = new Definitions.VariablesTypeVariableExternal_variable[] { strExternalVar, intExternalVar, boolExternalVar };
            var ovalVariables = this.CreateOvalVariablesFromExternalVariables(allExternalVariables);

            var stringExternalVarEvaluation = new ExternalVariableEvaluator(strExternalVar, ovalVariables).GetValue().Single();
            var integerExternalVarEvaluation = new ExternalVariableEvaluator(intExternalVar, ovalVariables).GetValue().Single();
            var booleanExternalVarEvaluation = new ExternalVariableEvaluator(boolExternalVar, ovalVariables).GetValue().Single();

            Assert.AreEqual(string.Empty, stringExternalVarEvaluation, "An unexpected default value for external string variable evaluation was found.");
            Assert.AreEqual(string.Empty, integerExternalVarEvaluation, "An unexpected default value for external integer variable evaluation was found.");
            Assert.AreEqual(string.Empty, booleanExternalVarEvaluation, "An unexpected default value for external boolean variable evaluation was found.");
        }

        private oval_variables CreateOvalVariablesFromExternalVariables(Definitions.VariablesTypeVariableExternal_variable[] externalVariables)
        {
            var newOvalVariables = new oval_variables();
            foreach (var externalVar in externalVariables)
            {
                var newVariableType = this.GetVariableType(externalVar.id, externalVar.datatype, new string[] { string.Empty });
                newOvalVariables.variables.Add(newVariableType);
            }

            return newOvalVariables;
        }


        private oval_variables CreateOvalVariables(string variableID, SimpleDatatypeEnumeration datatype, string[] variableValues)
        {
            var variableValue = this.GetVariableType(variableID, datatype, variableValues);
            return new oval_variables() { variables = new List<VariableType>() { variableValue } };
        }

        private VariableType GetVariableType(string variableID, SimpleDatatypeEnumeration datatype, string[] values)
        {
            return new VariableType() { id = variableID, datatype = datatype, value = values.ToList() };
        }

        private Definitions.VariablesTypeVariableExternal_variable CreateExternalVariable(string variableID, SimpleDatatypeEnumeration datatype)
        {
            return new Definitions.VariablesTypeVariableExternal_variable() { id = variableID, datatype = datatype };
        }
    }
}
