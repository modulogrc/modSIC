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
using Modulo.Collect.Probe.Windows.UserSID55;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test.UserSID55
{
    [TestClass]
    public class UserSID55EntityVariableEvaluatorTest
    {

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variables_for_userSID55Object()
        {
            var fakeObjectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1090");
            var fakeVariablesEvaluated = VariableHelper.CreateVariableWithOneValue(fakeObjectType.id, ((user_sid55_object)fakeObjectType).UserSID.var_ref, "S-1-11");
            var variableEvaluator = new UserSID55EntityVariableEvaluator(fakeObjectType, fakeVariablesEvaluated);

            var variableEvaluationResult = variableEvaluator.ProcessVariableForAllObjectEntities().ToArray();

            this.AssertVariableEvaluation(variableEvaluationResult, new string[] { "S-1-11" } );
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_multi_variables_for_userSID55Object()
        {
            var fakeVariableValues = new string[] { "S-1-11", "S-1-22", "S-1-33" };
            var fakeObjectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1090");
            var fakeVariablesEvaluated = VariableHelper.CreateVariableWithMultiplesValue(fakeObjectType.id, ((user_sid55_object)fakeObjectType).UserSID.var_ref, fakeVariableValues);
            var variableEvaluator = new UserSID55EntityVariableEvaluator(fakeObjectType, fakeVariablesEvaluated);

            var variableEvaluationResult = variableEvaluator.ProcessVariableForAllObjectEntities().ToArray();

            this.AssertVariableEvaluation(variableEvaluationResult, fakeVariableValues);
        }

        [TestMethod, Owner("lfernandes")]
        public void VariableEvaluation_should_be_return_null_when_there_is_no_referenced_variable()
        {
            var fakeObjectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1090");
            var fakeVariablesEvaluated = VariableHelper.CreateEmptyEvaluatedVariables();
            var variableEvaluator = new UserSID55EntityVariableEvaluator(fakeObjectType, fakeVariablesEvaluated);

            var variableEvaluationResult = variableEvaluator.ProcessVariableForAllObjectEntities();

            Assert.IsNull(variableEvaluationResult, "The result of variable evaluation must be NULL.");
        }

        private void AssertVariableEvaluation(string[] evaluationResult, string[] expectedResult)        
        {
            Assert.IsNotNull(evaluationResult, "The result of variable evaluation cannot be null");
            Assert.AreEqual(expectedResult.Count(), evaluationResult.Count(), "Unexpected variable evaluation result count was found.");
            for(int i = 0; i < expectedResult.Count(); i++)
                Assert.AreEqual(expectedResult[i], evaluationResult[i], "Unexpected variable was found after evaluation.");    
        }

    }
}
