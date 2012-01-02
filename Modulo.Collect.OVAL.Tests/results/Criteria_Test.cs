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
using Modulo.Collect.OVAL.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
using Modulo.Collect.OVAL.Common;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System;
using System.Collections.Generic;
using Modulo.Collect.OVAL.Schema;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.helpers;
using sc = Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Results;
using Rhino.Mocks;
namespace Modulo.Collect.OVAL.Tests
{
    
    /// <summary>
    ///This is a test class for oval_system_characteristics and is intended
    ///to contain all oval_system_characteristics Unit Tests
    ///</summary>
    [TestClass()]
    public class Criteria_Test
    {
        [TestMethod, Owner("mgaspar")]
        public void A_Criteria_Analyze_Result_Should_Be_The_Combination_Of_All_Children_Criteria_Results()
        {
            var childA = MockRepository.GenerateMock<CriteriaBase>();
            childA.Expect(x => x.Analyze(null)).Return(ResultEnumeration.@true);
            var childB = MockRepository.GenerateMock<CriteriaBase>();
            childB.Expect(x => x.Analyze(null)).Return(ResultEnumeration.@false);

            var target = new Modulo.Collect.OVAL.Results.CriteriaType();
            target.@operator = OperatorEnumeration.AND;
            target.Items = new List<object> { childA, childB };

            var result = target.Analyze(null);

            Assert.AreEqual(ResultEnumeration.@false, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void A_Criteria_Analyze_Should_Evaluate_Result_Only_Once()
        {
            var childA = MockRepository.GenerateMock<CriteriaBase>();
            var defaultResult = default(ResultEnumeration);
            childA.Expect(x => x.Analyze(null)).Return(oval_results.NegateResult(defaultResult, true));
            var target = new Modulo.Collect.OVAL.Results.CriteriaType();
            target.@operator = OperatorEnumeration.AND;
            target.Items = new List<object> { childA };

            // First Call - should Evaluate Children
            var result = target.Analyze(null);
            // Second Call - Should Return Stored Result 
            result = target.Analyze(null);

            Assert.AreNotEqual(defaultResult, result);
        }

        [TestMethod, Owner("mgaspar")]
        public void A_Criteria_Analyze_Result_Should_Be_Negated_If_Defined()
        {
            var childA = MockRepository.GenerateMock<CriteriaBase>();
            childA.Expect(x => x.Analyze(null)).Return(ResultEnumeration.@true);

            var target = new Modulo.Collect.OVAL.Results.CriteriaType() { @operator = OperatorEnumeration.AND };
            target.negate = true;
            target.Items = new List<object> { childA };

            var result = target.Analyze(null);

            Assert.AreEqual(ResultEnumeration.@false, result);
        }

    }
}
