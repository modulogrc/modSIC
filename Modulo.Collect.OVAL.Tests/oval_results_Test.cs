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
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Results;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.Variables;
using System.Xml.Serialization;
using System.Reflection;
using Modulo.Collect.OVAL.Definitions.entityOperations;

namespace Modulo.Collect.OVAL.Tests
{
    /// <summary>
    ///This is a test class for oval_system_characteristics and is intended
    ///to contain all oval_system_characteristics Unit Tests
    ///</summary>
    [TestClass()]
    public class ovalresults_Test
    {
        private const string MODULO_OVAL_DEFINITIONS_VISTA = "modulo-VISTAUL-oval.xml";
        private const string MODULO_OVAL_SYS_CHARACTERISTICS_VISTA = "modulo-VISTAUL-SystemCharacteristics.xml";
        private const string CREATE_OVAL_RESULTS_DOC_ERROR_MSG = "An error occurred while trying to create oval results document: '{0}'";
        private const string FAILED_ENTITY_NAME_RESOLVING = "It was not possible to resolve entity name (found name: '{0}')";

        [TestMethod, Owner("mgaspar")]
        public void Test_Load_Sample_Document()
        {
            IEnumerable<string> errors;
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.results.5368.xml");
            var target = oval_results.GetOvalResultsFromStream(sampleDoc, out errors);
            Assert.IsNotNull(target);
            Assert.AreEqual(0, errors.Count());
            Assert.AreEqual(16, target.oval_definitions.tests.Count());
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Negate_A_Result()
        {
            var resultTrue = ResultEnumeration.@true;
            var resultFalse = ResultEnumeration.@false;
            var resultOther = ResultEnumeration.notapplicable;

            Assert.AreEqual(ResultEnumeration.@false, oval_results.NegateResult(resultTrue, true));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.NegateResult(resultFalse, true));
            Assert.AreEqual(resultOther, oval_results.NegateResult(resultOther, true));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.NegateResult(resultTrue, false));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.NegateResult(resultFalse, false));
            Assert.AreEqual(resultOther, oval_results.NegateResult(resultOther, false));
            
        }
       
        [TestMethod, Owner("mgaspar")]
        public void Should_Return_Unknown_When_Combining_Empty_Result_Set()
        {
            var result = oval_results.CombineResultsByOperator(new ResultEnumeration[] { }, OperatorEnumeration.AND);
            Assert.AreEqual(ResultEnumeration.unknown, result);
        }

        /// <summary>
        ///               ||  num of individual results  ||
        ///  operator is  ||                             ||  final result is
        ///               || T  | F  | E  | U  | NE | NA ||
        /// --------------||-----------------------------||------------------
        ///               || 1+ | 0  | 0  | 0  | 0  | 0+ ||  True
        ///               || 0+ | 1+ | 0+ | 0+ | 0+ | 0+ ||  False
        ///      AND      || 0+ | 0  | 1+ | 0+ | 0+ | 0+ ||  Error
        ///               || 0+ | 0  | 0  | 1+ | 0+ | 0+ ||  Unknown
        ///               || 0+ | 0  | 0  | 0  | 1+ | 0+ ||  Not Evaluated
        ///               || 0  | 0  | 0  | 0  | 0  | 1+ ||  Not Applicable
        ///---------------||-----------------------------||------------------
        /// </summary>
        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Combine_Results_With_Operator_AND()
        {
            var resultAllTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true };
            var resultAllFalse = new ResultEnumeration[] { ResultEnumeration.@false };
            var resultTrueAndFalse = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@false };
            var resultTrueAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.error };
            var resultFalseAndError = new ResultEnumeration[] { ResultEnumeration.@false, ResultEnumeration.error };

            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByOperator(resultAllTrue, OperatorEnumeration.AND));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultAllFalse, OperatorEnumeration.AND));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultTrueAndFalse, OperatorEnumeration.AND));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByOperator(resultTrueAndError, OperatorEnumeration.AND));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultFalseAndError, OperatorEnumeration.AND));
        }

        /// <summary>
        ///               ||  num of individual results  || 
        ///  operator is  ||                             ||  final result is
        ///               || T  | F  | E  | U  | NE | NA ||
        ///---------------||-----------------------------||------------------
        ///               || 1  | 0+ | 0  | 0  | 0  | 0+ ||  True
        ///               || 2+ | 0+ | 0+ | 0+ | 0+ | 0+ ||  ** False **
        ///               || 0  | 1+ | 0  | 0  | 0  | 0+ ||  ** False **
        ///      ONE      ||0,1 | 0+ | 1+ | 0+ | 0+ | 0+ ||  Error
        ///               ||0,1 | 0+ | 0  | 1+ | 0+ | 0+ ||  Unknown
        ///               ||0,1 | 0+ | 0  | 0  | 1+ | 0+ ||  Not Evaluated
        ///               || 0  | 0  | 0  | 0  | 0  | 1+ ||  Not Applicable
        ///---------------||-----------------------------||------------------
        /// </summary>
        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Combine_Results_With_Operator_ONE()
        {
            var resultAllTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true };
            var resultAllFalse = new ResultEnumeration[] { ResultEnumeration.@false };
            var resultTrueAndFalse = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@false };
            var resultTrueAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.error };
            var resultFalseAndError = new ResultEnumeration[] { ResultEnumeration.@false, ResultEnumeration.error };
            var resultTwoTruesAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true, ResultEnumeration.error };

            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultAllTrue, OperatorEnumeration.ONE));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultAllFalse, OperatorEnumeration.ONE));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByOperator(resultTrueAndFalse, OperatorEnumeration.ONE));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByOperator(resultTrueAndError, OperatorEnumeration.ONE));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByOperator(resultFalseAndError, OperatorEnumeration.ONE));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultTwoTruesAndError, OperatorEnumeration.ONE));
        }

        /// <summary>
        ///               ||  num of individual results  || 
        ///  operator is  ||                             ||  final result is
        ///               || T  | F  | E  | U  | NE | NA ||
        ///---------------||-----------------------------||------------------
        ///               || 1+ | 0+ | 0+ | 0+ | 0+ | 0+ ||  True
        ///               || 0  | 1+ | 0  | 0  | 0  | 0+ ||  False
        ///      OR       || 0  | 0+ | 1+ | 0+ | 0+ | 0+ ||  Error
        ///               || 0  | 0+ | 0  | 1+ | 0+ | 0+ ||  Unknown
        ///               || 0  | 0+ | 0  | 0  | 1+ | 0+ ||  Not Evaluated
        ///               || 0  | 0  | 0  | 0  | 0  | 1+ ||  Not Applicable
        ///---------------||-----------------------------||------------------
        /// </summary>
        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Combine_Results_With_Operator_OR()
        {
            var resultAllTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true };
            var resultAllFalse = new ResultEnumeration[] { ResultEnumeration.@false };
            var resultTrueAndFalse = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@false };
            var resultTrueAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.error };
            var resultFalseAndError = new ResultEnumeration[] { ResultEnumeration.@false, ResultEnumeration.error };

            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByOperator(resultAllTrue, OperatorEnumeration.OR));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultAllFalse, OperatorEnumeration.OR));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByOperator(resultTrueAndFalse, OperatorEnumeration.OR));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByOperator(resultTrueAndError, OperatorEnumeration.OR));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByOperator(resultFalseAndError, OperatorEnumeration.OR));
        }

        /// <summary>
        ///               ||  num of individual results  ||
        ///  operator is  ||                             ||  final result is
        ///               || T  | F  | E  | U  | NE | NA ||
        ///---------------||-----------------------------||------------------
        ///               ||odd | 0+ | 0  | 0  | 0  | 0+ ||  True
        ///               ||even| 0+ | 0  | 0  | 0  | 0+ ||  False
        ///      XOR      || 0+ | 0+ | 1+ | 0+ | 0+ | 0+ ||  Error
        ///               || 0+ | 0+ | 0  | 1+ | 0+ | 0+ ||  Unknown
        ///               || 0+ | 0+ | 0  | 0  | 1+ | 0+ ||  Not Evaluated
        ///               || 0  | 0  | 0  | 0  | 0  | 1+ ||  Not Applicable
        ///---------------||-----------------------------||------------------
        /// </summary>
        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Combine_Results_With_Operator_XOR()
        {
            var resultOneTrue = new ResultEnumeration[] { ResultEnumeration.@true, };
            var resultTwoTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true };
            var resultThreeTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true, ResultEnumeration.@true };
            var resultAllFalse = new ResultEnumeration[] { ResultEnumeration.@false };
            var resultTrueAndFalse = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@false };
            var resultTrueAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.error };
            var resultFalseAndError = new ResultEnumeration[] { ResultEnumeration.@false, ResultEnumeration.error };

            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByOperator(resultOneTrue, OperatorEnumeration.XOR));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultTwoTrue, OperatorEnumeration.XOR));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByOperator(resultThreeTrue, OperatorEnumeration.XOR));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByOperator(resultAllFalse, OperatorEnumeration.XOR));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByOperator(resultTrueAndFalse, OperatorEnumeration.XOR));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByOperator(resultTrueAndError, OperatorEnumeration.XOR));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByOperator(resultFalseAndError, OperatorEnumeration.XOR));
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_Return_Unknown_When_Combining_By_Check_An_Empty_Result_Set()
        {
            var result = oval_results.CombineResultsByCheck(new ResultEnumeration[] { }, CheckEnumeration.all);
            Assert.AreEqual(ResultEnumeration.unknown, result);
        }

        /// <summary>
        ///               ||  num of individual results  ||
        /// check attr is ||                             ||  final result is
        ///               || T  | F  | E  | U  | NE | NA ||
        ///---------------||-----------------------------||------------------
        ///               || 1+ | 0  | 0  | 0  | 0  | 0+ ||  True
        ///               || 0+ | 1+ | 0+ | 0+ | 0+ | 0+ ||  False
        ///     ALL       || 0+ | 0  | 1+ | 0+ | 0+ | 0+ ||  Error
        ///               || 0+ | 0  | 0  | 1+ | 0+ | 0+ ||  Unknown
        ///               || 0+ | 0  | 0  | 0  | 1+ | 0+ ||  Not Evaluated
        ///               || 0  | 0  | 0  | 0  | 0  | 1+ ||  Not Applicable
        ///---------------||-----------------------------||------------------
        /// </summary>
        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Combine_Results_With_Check_all()
        {
            var resultOneTrue = new ResultEnumeration[] { ResultEnumeration.@true, };
            var resultTwoTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true };
            var resultThreeTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true, ResultEnumeration.@true };
            var resultAllFalse = new ResultEnumeration[] { ResultEnumeration.@false };
            var resultTrueAndFalse = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@false };
            var resultTrueAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.error };
            var resultFalseAndError = new ResultEnumeration[] { ResultEnumeration.@false, ResultEnumeration.error };

            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultOneTrue, CheckEnumeration.all));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultTwoTrue, CheckEnumeration.all));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultThreeTrue, CheckEnumeration.all));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultAllFalse, CheckEnumeration.all));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultTrueAndFalse, CheckEnumeration.all));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByCheck(resultTrueAndError, CheckEnumeration.all));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultFalseAndError, CheckEnumeration.all));
        }

        /// <summary>
        ///               ||  num of individual results  ||
        /// check attr is ||                             ||  final result is
        ///               || T  | F  | E  | U  | NE | NA ||
        ///---------------||-----------------------------||------------------
        ///               || 1+ | 0+ | 0+ | 0+ | 0+ | 0+ ||  True
        ///               || 0  | 1+ | 0  | 0  | 0  | 0+ ||  False
        ///  AT LEAST ONE || 0  | 0+ | 1+ | 0+ | 0+ | 0+ ||  Error
        ///               || 0  | 0+ | 0  | 1+ | 0+ | 0+ ||  Unknown
        ///               || 0  | 0+ | 0  | 0  | 1+ | 0+ ||  Not Evaluated
        ///               || 0  | 0  | 0  | 0  | 0  | 1+ ||  Not Applicable
        ///---------------||-----------------------------||------------------
        /// </summary>
        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Combine_Results_With_Check_atleastone()
        {
            var resultOneTrue = new ResultEnumeration[] { ResultEnumeration.@true, };
            var resultTwoTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true };
            var resultThreeTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true, ResultEnumeration.@true };
            var resultAllFalse = new ResultEnumeration[] { ResultEnumeration.@false };
            var resultTrueAndFalse = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@false };
            var resultTrueAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.error };
            var resultFalseAndError = new ResultEnumeration[] { ResultEnumeration.@false, ResultEnumeration.error };

            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultOneTrue, CheckEnumeration.atleastone));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultTwoTrue, CheckEnumeration.atleastone));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultThreeTrue, CheckEnumeration.atleastone));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultAllFalse, CheckEnumeration.atleastone));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultTrueAndFalse, CheckEnumeration.atleastone));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultTrueAndError, CheckEnumeration.atleastone));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByCheck(resultFalseAndError, CheckEnumeration.atleastone));
        }

        /// <summary>
        ///                ||  num of individual results  ||
        /// check attr is ||                             ||  final result is
        ///               || T  | F  | E  | U  | NE | NA ||
        ///---------------||-----------------------------||------------------
        ///               || 1  | 0+ | 0  | 0  | 0  | 0+ ||  True
        ///               || 2+ | 0+ | 0+ | 0+ | 0+ | 0+ ||  ** False **
        ///               || 0  | 1+ | 0  | 0  | 0  | 0+ ||  ** False **
        ///   ONLY ONE    ||0,1 | 0+ | 1+ | 0+ | 0+ | 0+ ||  Error
        ///               ||0,1 | 0+ | 0  | 1+ | 0+ | 0+ ||  Unknown
        ///               ||0,1 | 0+ | 0  | 0  | 1+ | 0+ ||  Not Evaluated
        ///               || 0  | 0  | 0  | 0  | 0  | 1+ ||  Not Applicable
        ///---------------||-----------------------------||------------------
        /// </summary>
        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Combine_Results_With_Check_onlyone()
        {
            var resultOneTrue = new ResultEnumeration[] { ResultEnumeration.@true, };
            var resultTwoTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true };
            var resultThreeTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true, ResultEnumeration.@true };
            var resultAllFalse = new ResultEnumeration[] { ResultEnumeration.@false };
            var resultTrueAndFalse = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@false };
            var resultTrueAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.error };
            var resultFalseAndError = new ResultEnumeration[] { ResultEnumeration.@false, ResultEnumeration.error };

            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultOneTrue, CheckEnumeration.onlyone));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultTwoTrue, CheckEnumeration.onlyone));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultThreeTrue, CheckEnumeration.onlyone));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultAllFalse, CheckEnumeration.onlyone));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultTrueAndFalse, CheckEnumeration.onlyone));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByCheck(resultTrueAndError, CheckEnumeration.onlyone));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByCheck(resultFalseAndError, CheckEnumeration.onlyone));
        }

        /// <summary>
        ///               ||  num of individual results  ||
        /// check attr is ||                             ||  final result is
        ///               || T  | F  | E  | U  | NE | NA ||
        ///---------------||-----------------------------||------------------
        ///               || 0  | 1+ | 0  | 0  | 0  | 0+ ||  True
        ///               || 1+ | 0+ | 0+ | 0+ | 0+ | 0+ ||  False
        ///  NONE SATISFY || 0  | 0+ | 1+ | 0+ | 0+ | 0+ ||  Error
        ///               || 0  | 0+ | 0  | 1+ | 0+ | 0+ ||  Unknown
        ///               || 0  | 0+ | 0  | 0  | 1+ | 0+ ||  Not Evaluated
        ///               || 0  | 0  | 0  | 0  | 0  | 1+ ||  Not Applicable
        ///---------------||-----------------------------||------------------
        /// </summary>
        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Combine_Results_With_Check_nonesatisfy()
        {
            var resultOneTrue = new ResultEnumeration[] { ResultEnumeration.@true, };
            var resultTwoTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true };
            var resultThreeTrue = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@true, ResultEnumeration.@true };
            var resultAllFalse = new ResultEnumeration[] { ResultEnumeration.@false };
            var resultTrueAndFalse = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.@false };
            var resultTrueAndError = new ResultEnumeration[] { ResultEnumeration.@true, ResultEnumeration.error };
            var resultFalseAndError = new ResultEnumeration[] { ResultEnumeration.@false, ResultEnumeration.error };

            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultOneTrue, CheckEnumeration.nonesatisfy));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultTwoTrue, CheckEnumeration.nonesatisfy));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultThreeTrue, CheckEnumeration.nonesatisfy));
            Assert.AreEqual(ResultEnumeration.@true, oval_results.CombineResultsByCheck(resultAllFalse, CheckEnumeration.nonesatisfy));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultTrueAndFalse, CheckEnumeration.nonesatisfy));
            Assert.AreEqual(ResultEnumeration.@false, oval_results.CombineResultsByCheck(resultTrueAndError, CheckEnumeration.nonesatisfy));
            Assert.AreEqual(ResultEnumeration.error, oval_results.CombineResultsByCheck(resultFalseAndError, CheckEnumeration.nonesatisfy));
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Create_A_Result_Object_From_Definitions_And_System_Characteristics()
        {
            IEnumerable<string> errors;
            var definitionsDoc = GetType().Assembly.GetManifestResourceStream(
                            GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.def.5368.xml");
            var definitions = oval_definitions.GetOvalDefinitionsFromStream(definitionsDoc, out errors);
            var scDoc = GetType().Assembly.GetManifestResourceStream(
                            GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.sc.5368.xml");
            var systemcharacteristics = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(scDoc, out errors);
            var result = oval_results.CreateFromDocuments(definitions, systemcharacteristics, null);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.generator);
            Assert.AreEqual(definitions, result.oval_definitions);
            Assert.AreEqual(systemcharacteristics, result.results[0].oval_system_characteristics);
            Assert.AreEqual(definitions.definitions.Count(), result.results[0].definitions.Count);
            Assert.AreEqual(definitions.tests.Count(), result.results[0].tests.Count);
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Analyze_A_Whole_Result_Document()
        {
            IEnumerable<string> errors;
            var definitionsDoc = GetType().Assembly.GetManifestResourceStream(
                            GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.def.5368.xml");
            
            var definitions = oval_definitions.GetOvalDefinitionsFromStream(definitionsDoc, out errors);
            var scDoc = GetType().Assembly.GetManifestResourceStream(
                            GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.sc.5368.xml");
            
            var systemcharacteristics = 
                oval_system_characteristics
                    .GetOvalSystemCharacteristicsFromStream(scDoc, out errors);
            

            var result = oval_results.CreateFromDocuments(definitions, systemcharacteristics, null);
            result.Analyze();


            this.DoBasicResultAssert(result);
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_Be_Possible_To_Generate_A_Valid_Result_Document_XML()
        {
            IEnumerable<string> errors;
            var definitionsDoc = GetType().Assembly.GetManifestResourceStream(
                            GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.def.5368.xml");
            
            var definitions = oval_definitions.GetOvalDefinitionsFromStream(definitionsDoc, out errors);
            
            var scDoc = GetType().Assembly.GetManifestResourceStream(
                            GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.sc.5368.xml");
            
            var systemcharacteristics = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(scDoc, out errors);
            
            var result = oval_results.CreateFromDocuments(definitions, systemcharacteristics, null);
            result.Analyze();

            //generate the xml from the Results object
            string resultsXML = result.GetResultsXML();
            Assert.IsFalse(string.IsNullOrEmpty(resultsXML));

            //creates a stream for the xml generated
            MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(resultsXML));
            IEnumerable<string> loadErrors;

            // load the oval_results from the xml generated
            oval_results ovalResults = oval_results.GetOvalResultsFromStream(m, out loadErrors);

            Assert.IsNotNull(ovalResults);
            Assert.IsTrue(loadErrors.Count() == 0, "the errors occurs in the load results object");
        }

        /// <summary>
        /// 	<registry_state id="oval:com.modulo.VISTAUL:ste:35859" version="1">
		///         <value />
	    ///     </registry_state>	
        /// </summary>
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_analyze_an_oval_definitions_document_that_contains_an_entity_state_with_undefinied_value()
        {
            var definitionsID = "oval:com.modulo.VISTAUL:def:19086";
            var moduloOvalResultsSample = GetModuloOvalResultsSample();
                
            moduloOvalResultsSample.Analyze();

            var definitionResult = this.GetResultDefinitionByID(moduloOvalResultsSample, definitionsID);
            Assert.AreEqual(ResultEnumeration.@false, definitionResult.result, "The result of definition with null state and not null item must be false.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_analyse_a_definition_of_inventory_class()
        {
            var OVAL_DEFINITIONS_FILE = "oval_definitions_with_inventory_definition.xml";
            var SYSTEM_CHARACTERISTICS_FILE = "system_characteristics_with_inventory_definition.xml";
            var ovalResults = 
                this.GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    OVAL_DEFINITIONS_FILE, 
                    SYSTEM_CHARACTERISTICS_FILE,
                    null);

            ovalResults.Analyze();
            var definitionResult = this.GetResultDefinitionByID(ovalResults, "oval:com.modulo.XPEN:def:1");

            this.DoBasicResultAssert(ovalResults);
            Assert.AreEqual(ResultEnumeration.@true, definitionResult.result, "An unexpected result of definition of inventory class was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_answer_a_definition_that_has_no_state_associated()
        {
            var moduloOvalResultsSample = GetModuloOvalResultsSample();

            moduloOvalResultsSample.Analyze();

            var definitionToAssert = this.GetResultDefinitionByID(moduloOvalResultsSample, "oval:test.modulo:def:1");
            Assert.AreEqual(ResultEnumeration.@true, definitionToAssert.result, "An unexpected definition result was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_analyse_oval_result_document_that_contains_state_refering_external_variables()
        {
            var ovalVariables1 =
                new OvalVariablesBuilder()
                    .AddVariable("oval:modulo:var:1", "...", SimpleDatatypeEnumeration.@string, "3gp")
                    .AddVariable("oval:modulo:var:2", "...", SimpleDatatypeEnumeration.boolean, "1")
                    .AddVariable("oval:modulo:var:3", "...", SimpleDatatypeEnumeration.@string, "rm7.exe")
                .Build();

            var ovalVariables2 =
                new OvalVariablesBuilder()
                    .AddVariable("oval:modulo:var:1", "...", SimpleDatatypeEnumeration.@string, "3gp1")
                    .AddVariable("oval:modulo:var:2", "...", SimpleDatatypeEnumeration.boolean, "0")
                    .AddVariable("oval:modulo:var:3", "...", SimpleDatatypeEnumeration.@string, "rm.exe")
                .Build();


            var ovalResults1 = Analyse(ovalVariables1);
            var ovalResults2 = Analyse(ovalVariables2);


            Assert.IsNotNull(ovalResults1);
            Assert.AreEqual(ResultEnumeration.@true, ovalResults1.results[0].definitions[0].result);
            Assert.AreEqual(ResultEnumeration.@true, ovalResults1.results[0].definitions[1].result);
            Assert.AreEqual(ResultEnumeration.@true, ovalResults1.results[0].definitions[2].result);

            Assert.IsNotNull(ovalResults2);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults2.results[0].definitions[0].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults2.results[0].definitions[1].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults2.results[0].definitions[2].result);
        }

        [TestMethod, Owner("lfernandes, lfalcao")]
        public void Should_be_possible_analyse_unix_objects()
        {
            var ovalResults = GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments("definitions_all_unix.xml", "unix-system-characteristics.xml", null);

            ovalResults.Analyze();

            Assert.IsNotNull(ovalResults);
            Assert.AreEqual(ResultEnumeration.@true, ovalResults.results[0].definitions[0].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[1].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[2].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[3].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[4].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[5].result);
            Assert.AreEqual(ResultEnumeration.@true, ovalResults.results[0].definitions[6].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[7].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[8].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[9].result);
        }

        [TestMethod, Owner("lfalcao")]
        public void Should_be_possible_analyse_linux_objects()
        {
            var ovalResults = GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments("definitions_all_linux.xml", "linux-system-characteristics.xml", null);

            ovalResults.Analyze();

            Assert.IsNotNull(ovalResults);
            Assert.AreEqual(ResultEnumeration.@true, ovalResults.results[0].definitions[0].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[1].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[2].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResults.results[0].definitions[3].result);
        }

        #region StateEntityNameResolver Tests
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_undecorated_state_entities()
        {
            var solarisOvalResults =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "definitions_all_solaris.xml", "solaris-system-characteristics.xml", null);

            var isaInfoState = solarisOvalResults.oval_definitions.states.Single(ste => ste.id.Equals("oval:modulo:ste:1"));

            AssertStateEntityNameResolution(isaInfoState, "bits", "bits");
            AssertStateEntityNameResolution(isaInfoState, "kernel_isa", "kernel_isa");
            AssertStateEntityNameResolution(isaInfoState, "application_isa", "application_isa");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_decorated_state_entities()
        {
            var solarisOvalResults =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "definitions_all_solaris.xml", "solaris-system-characteristics.xml", null);

            var patchState = solarisOvalResults.oval_definitions.states.Single(ste => ste.id.Equals("oval:modulo:ste:3"));

            AssertStateEntityNameResolution(patchState, "version1", "version");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_decorated_and_undecorated_state_entities()
        {
            var solarisOvalResults =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "definitions_all_solaris.xml", "solaris-system-characteristics.xml", null);

            var isaInfoState = solarisOvalResults.oval_definitions.states.Single(ste => ste.id.Equals("oval:modulo:ste:2"));

            AssertStateEntityNameResolution(isaInfoState, "pkginst", "pkginst");
            AssertStateEntityNameResolution(isaInfoState, "name", "name");
            AssertStateEntityNameResolution(isaInfoState, "category", "category");
            AssertStateEntityNameResolution(isaInfoState, "version1", "version");
            AssertStateEntityNameResolution(isaInfoState, "vendor", "vendor");
            AssertStateEntityNameResolution(isaInfoState, "description", "description");

        }

        private void AssertStateEntityNameResolution(StateType stateType, string entityName, string expectedResolvedEntityName)
        {
            var entity = GetPropertyInfoByName(stateType, entityName);
            var resolvedEntityName = new StateEntityNameResolver().Resolve(entity);
            Assert.AreEqual(expectedResolvedEntityName, resolvedEntityName, string.Format(FAILED_ENTITY_NAME_RESOLVING, resolvedEntityName));
        }
        #endregion

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_analyse_solaris_objects()
        {
            var solarisOvalResults =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "definitions_all_solaris.xml", "solaris-system-characteristics.xml", null);

            solarisOvalResults.Analyze();

            Assert.IsNotNull(solarisOvalResults);
            // Negative definitions tests
            Assert.AreEqual(ResultEnumeration.@false, solarisOvalResults.results[0].definitions[0].result);
            Assert.AreEqual(ResultEnumeration.@false, solarisOvalResults.results[0].definitions[1].result);
            Assert.AreEqual(ResultEnumeration.@false, solarisOvalResults.results[0].definitions[2].result);
            Assert.AreEqual(ResultEnumeration.@false, solarisOvalResults.results[0].definitions[3].result);
            Assert.AreEqual(ResultEnumeration.@false, solarisOvalResults.results[0].definitions[4].result);
            // Positive definitions tests
            Assert.AreEqual(ResultEnumeration.@true, solarisOvalResults.results[0].definitions[5].result);
            Assert.AreEqual(ResultEnumeration.@true, solarisOvalResults.results[0].definitions[6].result);
            Assert.AreEqual(ResultEnumeration.@true, solarisOvalResults.results[0].definitions[7].result);
            Assert.AreEqual(ResultEnumeration.@true, solarisOvalResults.results[0].definitions[8].result);
            Assert.AreEqual(ResultEnumeration.@true, solarisOvalResults.results[0].definitions[9].result);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_pattern_match_operation_in_state_so_that_is_possible_to_match_a_string_negating_digits_occurence()
        {
            var redhatOvalResultsDocument =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "RM7-scap-rhel5-definitions.xml", "RM7-scap-rhel5-system-characteristics.xml", null);

            Assert.IsNotNull(redhatOvalResultsDocument);
            redhatOvalResultsDocument.Analyze();

            // Negative definitions tests
            Assert.AreEqual(ResultEnumeration.@true, redhatOvalResultsDocument.results[0].definitions[0].result);
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_analyse_tests_that_use_record_type()
        {
            var ovalResultsSample =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "definitions_with_record_type.xml", "system_characteristics_with_record_type.xml", null);

            Assert.IsNotNull(ovalResultsSample);
            ovalResultsSample.Analyze();

            Assert.AreEqual(ResultEnumeration.@false, ovalResultsSample.results[0].definitions[0].result);
            Assert.AreEqual(ResultEnumeration.@true, ovalResultsSample.results[0].definitions[1].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResultsSample.results[0].definitions[2].result);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_analyse_tests_that_use_record_type_with_patternMatch_operation()
        {
            var ovalResultsSample =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "definitions_with_record_type.xml", "system_characteristics_with_record_type.xml", null);

            Assert.IsNotNull(ovalResultsSample);
            ovalResultsSample.Analyze();

            Assert.AreEqual(ResultEnumeration.@true, ovalResultsSample.results[0].definitions[3].result);
            Assert.AreEqual(ResultEnumeration.@false, ovalResultsSample.results[0].definitions[4].result);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_analyse_max_connections()
        {
            var ovalResultsSample =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "modulo-iis6-lite.xml", "modulo-iis6-lite-system-characteristics.xml", null);

            Assert.IsNotNull(ovalResultsSample);
            ovalResultsSample.Analyze();

            Assert.AreEqual(ResultEnumeration.@true, ovalResultsSample.results[0].definitions[0].result);
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_analyze_cisco_ios_definitions()
        {
            var ovalResultsSample =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "modulo-ios122-oval.xml", "modulo-ios122-system-characteristics.xml", null);

            ovalResultsSample.Analyze();

            Assert.AreEqual(ResultEnumeration.@true, ovalResultsSample.results[0].definitions[0].result);
            Assert.IsNull(
                ovalResultsSample.results.First()
                    .definitions
                        .FirstOrDefault(
                            def => def.result.Equals(ResultEnumeration.error) ||
                                    def.result.Equals(ResultEnumeration.notevaluated) ||
                                    def.result.Equals(ResultEnumeration.unknown)));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_analyse_a_definition_which_on_state_has_external_variable_reference()
        {
            var variableFactory = new ExternalVariableFactory("modulo-Exchange2003-oval.xml");
            var fakeVariableValues = new Dictionary<string, string[]>();
            fakeVariableValues.Add("oval:com.modulo.exchange2003:var:1304801", new[] { "False" });
            fakeVariableValues.Add("oval:com.modulo.exchange2003:var:1304802", new[] { "True" });
            var externalVariables = variableFactory.CreateOvalVariablesDocument(fakeVariableValues);
            var ovalResultsSample =
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "modulo-Exchange2003-oval.xml", "modulo-Exchange2003-sc.xml", externalVariables);

            ovalResultsSample.Analyze();

            var allDefinitions = ovalResultsSample.results[0].definitions;
            var definitionMustBeTrue = allDefinitions.Single(def => def.definition_id == "oval:com.modulo.exchange2003:def:13048001");
            Assert.AreEqual(ResultEnumeration.@true, definitionMustBeTrue.result);
            var definitionMustBeFalse = allDefinitions.Single(def => def.definition_id == "oval:com.modulo.exchange2003:def:13048002");
            Assert.AreEqual(ResultEnumeration.@false, definitionMustBeFalse.result);
        }

        private PropertyInfo GetPropertyInfoByName(StateType stateType, string entityName)
        {
            var allStateEntities = stateType.GetType().GetProperties();
            var entity = allStateEntities.FirstOrDefault(property => property.Name.Equals(entityName));
            Assert.IsNotNull(entity, string.Format("'{0}' entity was not found.", entityName));
            return entity;
        }

        private oval_results Analyse(oval_variables ovalVariables)
        {
            var ovalResultsDoc = 
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    "definitionsWithStateWithExternalVariables.xml",
                    "system_characteristics_with_state_with_external_variables.xml",
                    ovalVariables);

            ovalResultsDoc.Analyze();
            
            return ovalResultsDoc;
        }

        private Results.DefinitionType GetResultDefinitionByID(oval_results ovalResults, string definitionID)
        {
            return 
                ovalResults
                    .results.First()
                        .definitions
                            .Where(def => def.definition_id.Equals(definitionID)).Single();
        }

        private oval_results GetModuloOvalResultsSample()
        {
            return 
                GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
                    MODULO_OVAL_DEFINITIONS_VISTA, MODULO_OVAL_SYS_CHARACTERISTICS_VISTA, null);
        }

        private oval_results GetOvalResultsFromDefinitionsAndSystemCharacteristicsDocuments(
            string definitionsResourceName, 
            string systemCharacteristicsResourceName,
            oval_variables evaluatedExternalVariables)
        {
            try
            {
                var ovalDocumentLoader = new OvalDocumentLoader();
                var definitions = ovalDocumentLoader.GetFakeOvalDefinitions(definitionsResourceName);
                var systemCharacteristics = ovalDocumentLoader.GetFakeOvalSystemCharacteristics(systemCharacteristicsResourceName);

                return oval_results.CreateFromDocuments(definitions, systemCharacteristics, evaluatedExternalVariables);
            }
            catch (Exception ex)
            {
                Assert.Fail(String.Format(CREATE_OVAL_RESULTS_DOC_ERROR_MSG, ex.Message));
                return null;
            }
        }

        private void DoBasicResultAssert(oval_results ovalResults)
        {
            Assert.AreEqual(0, ovalResults.results[0].definitions.Where(d => d.result == ResultEnumeration.unknown).Count());
            Assert.AreEqual(0, ovalResults.results[0].tests.Where(t => t.result == ResultEnumeration.unknown).Count());
        }
    }
}
