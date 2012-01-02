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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Results;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.helpers;
using Rhino.Mocks;

namespace Modulo.Collect.OVAL.Tests
{
    
    [TestClass()]
    public class TestType_Test
    {
        #region Test Data Helpers

        protected oval_results GetSampleResults()
        {
            IEnumerable<string> errors;
            var definitionsDoc = GetType().Assembly.GetManifestResourceStream(
                            GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.def.5368.xml");
            var definitions = oval_definitions.GetOvalDefinitionsFromStream(definitionsDoc, out errors);
            var scDoc = GetType().Assembly.GetManifestResourceStream(
                            GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.sc.5368.xml");
            var systemcharacteristics = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(scDoc, out errors);
            return oval_results.CreateFromDocuments(definitions, systemcharacteristics, null);
        }

        #endregion


        [TestMethod, Owner("mgaspar")]
        public void Analyze_Of_A_Test_With_No_Object_Id_Should_Return_Unknown()
        {
            var results = new oval_results();
            results.oval_definitions = new oval_definitions();
            var fakeTest = MockRepository.GenerateMock<Modulo.Collect.OVAL.Definitions.TestType>();
            fakeTest.Expect(x => x.HasObjectReference).Return(false).Repeat.Any();
            results.oval_definitions.tests = new [] { fakeTest };

            var target = new Modulo.Collect.OVAL.Results.TestType();
            target.test_id = results.oval_definitions.tests[0].id;
            
            ResultEnumeration result = target.Analyze(results);
            Assert.AreEqual(ResultEnumeration.unknown, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Contain_All_Tested_Items()
        {
            var results = GetSampleResults();

            var targetTest = results.results[0].tests[0];
            targetTest.Analyze(results);
            Assert.AreEqual(1, targetTest.tested_item.Count());
            
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Contain_All_Variable_Results()
        {
            var results = GetSampleResults();
            var targetTest = results.results[0].tests[0];
            results.results[0].oval_system_characteristics.collected_objects[0].variable_value = new [] { new VariableValueType() { variable_id = results.oval_definitions.variables[0].id, Value = "xx" } }; 
            targetTest.Analyze(results);
            Assert.AreEqual(1, targetTest.tested_variable.Count());
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Handle_Collect_Error()
        {
            var results = GetSampleResults();
            var targetTest = results.results[0].tests[0];
            results.results[0].oval_system_characteristics.collected_objects[0].flag = FlagEnumeration.error;
            var result = targetTest.Analyze(results);
            Assert.AreEqual(ResultEnumeration.error, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Handle_Collect_NotApplicable_And_Return_NotApplicable()
        {
            var results = GetSampleResults();
            var targetTest = results.results[0].tests[0];
            results.results[0].oval_system_characteristics.collected_objects[0].flag = FlagEnumeration.notapplicable;
            var result = targetTest.Analyze(results);
            Assert.AreEqual(ResultEnumeration.notapplicable, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Handle_Collect_NotCollected_And_Return_Unknown()
        {
            var results = GetSampleResults();
            var targetTest = results.results[0].tests[0];
            results.results[0].oval_system_characteristics.collected_objects[0].flag = FlagEnumeration.notcollected;
            var result = targetTest.Analyze(results);
            Assert.AreEqual(ResultEnumeration.unknown, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Return_False_By_Default_If_Collect_DoesNotExist()
        {
            var results = GetSampleResults();
            var targetTest = results.results[0].tests[0];
            results.results[0].oval_system_characteristics.collected_objects[0].flag = FlagEnumeration.doesnotexist;
            var result = targetTest.Analyze(results);
            Assert.AreEqual(ResultEnumeration.@false, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Return_True_If_Collect_DoesNotExist_But_Check_Existence_Is_Set_To_None()
        {
            var results = GetSampleResults();
            var targetTest = results.results[0].tests[0];
            targetTest.check_existence = ExistenceEnumeration.none_exist;
            results.results[0].oval_system_characteristics.collected_objects[0].flag = FlagEnumeration.doesnotexist;
            var result = targetTest.Analyze(results);
            Assert.AreEqual(ResultEnumeration.@true, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Check_Existence_If_Object_Flag_Is_Completed()
        {
            var results = GetSampleResults();
            var targetTest = results.results[0].tests[0];
            // Set Check to none_exist  so Result shoukld be false since item actually exists
            targetTest.check_existence = ExistenceEnumeration.none_exist;
            results.results[0].oval_system_characteristics.collected_objects[0].flag = FlagEnumeration.complete;
            var result = targetTest.Analyze(results);
            Assert.AreEqual(ResultEnumeration.@false, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Result_Should_Check_State_If_Object_Flag_Is_Completed_And_Existence_Is_true()
        {
            var results = GetSampleResults();
            var targetTest = results.results[0].tests[1]; // This Test is marked as false in sample result
            var result = targetTest.Analyze(results);
            Assert.AreEqual(ResultEnumeration.@false, result);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_analyze_a_test_where_referenced_state_does_reference_to_a_variable()
        {
            var ovalResultsSample = GetOvalResultsSample();
            var targetTest = ovalResultsSample.results.First().tests.First();
            
            var testResult = targetTest.Analyze(ovalResultsSample);

            Assert.AreEqual(ResultEnumeration.@true, testResult);
            
        }

        private oval_results GetOvalResultsSample()
        {
            var ovalDocLoader = new OvalDocumentLoader();
            var fakeDefinitions = ovalDocLoader.GetFakeOvalDefinitions("definitionsWithExternalVariables.xml");
            var fakeSystemCharacteristics = ovalDocLoader.GetFakeOvalSystemCharacteristics("system_characteristics_with_variables.xml");
            
            return oval_results.CreateFromDocuments(fakeDefinitions, fakeSystemCharacteristics, null);
        }

       
        //[TestMethod, Owner("mgaspar")]
        //public void Test_Should_be_Able_To_Check_Existence_On_Collected_Items()
        //{
        //    var results = GetSampleResults();
        //    var targetTest = results.results[0].tests[0];
        //    targetTest.check_existence = ExistenceEnumeration.none_exist;
        //    //results.results[0].oval_system_characteristics.collected_objects[0].flag = FlagEnumeration.doesnotexist;
        //    targetTest.Analyze(results);
        //    ResultEnumeration result = targetTest.GetCheckExistenceResult();
        //    Assert.AreEqual(ResultEnumeration.@true, result);
        //}
    }
}
