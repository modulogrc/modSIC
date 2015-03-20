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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Tests.helpers;

namespace Modulo.Collect.OVAL.Tests
{
    [TestClass]
    public class External_variableTest
    {
        private const string EXPECTED_POSSIBLE_VALUE_NOT_FOUND = "Expected possible value was not found.";
        private const string UNEXPECTED_POSSIBLE_VALUE_FOUND = "Unexpected possible value was found. First value: '{0}'";
        private const string UNEXPECTED_POSSIBLE_VALUES_COUNT_FOUND = "Unexpected possible values amount was found.";
        private const string EXPECTED_RESTRICTION_VALUE_NOT_FOUND = "Expected restriction value was not found.";
        private const string UNEXPECTED_RESTRICTION_VALUE_FOUND = "Unexpected restriction value was found. First value: '{0}'";
        private const string UNEXPECTED_POSSIBLE_RESTRICTIONS_COUNT_FOUND = "Unexpected possible restriction amount was found.";

        private ExternalVariableFactory ExternalVariableFactory;

        public External_variableTest()
        {
            this.ExternalVariableFactory = new ExternalVariableFactory("fdcc_xpfirewall_oval.xml");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_verify_if_external_variable_has_possible_values_defined()
        {
            {
                var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:6672");
                Assert.IsTrue(externalVariable.HasPossibleValues(), EXPECTED_POSSIBLE_VALUE_NOT_FOUND);
            }
            {
                var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xpfirewall:var:51131");
                if (externalVariable.HasPossibleValues())
                {
                    var firstValue = externalVariable.GetPossibleValues().First().Value;
                    Assert.Fail(UNEXPECTED_POSSIBLE_VALUE_FOUND, firstValue);
                }
            }
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_verify_fi_external_variable_has_possible_restriction()
        {
            {
                var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:261");
                Assert.IsTrue(externalVariable.HasPossibleRestriction(), EXPECTED_RESTRICTION_VALUE_NOT_FOUND);
            }
            {
                var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:6672");
                Assert.IsFalse(externalVariable.HasPossibleRestriction(), UNEXPECTED_RESTRICTION_VALUE_FOUND);
            }
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_the_possible_values_from_external_variable()
        {
            {
                var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:6672");
                Assert.AreEqual(5, externalVariable.GetPossibleValues().Count(), UNEXPECTED_POSSIBLE_VALUES_COUNT_FOUND);
            }
            {
                var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xpfirewall:var:51131");
                Assert.AreEqual(0, externalVariable.GetPossibleValues().Count(), UNEXPECTED_POSSIBLE_VALUES_COUNT_FOUND);
            }
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_the_possible_restrictions_from_external_variable()
        {
            {
                var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:261");
                Assert.AreEqual(1, externalVariable.GetPossibleRestrictions().Count(), UNEXPECTED_POSSIBLE_RESTRICTIONS_COUNT_FOUND);
            }
            {
                var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xpfirewall:var:51131");
                Assert.AreEqual(0, externalVariable.GetPossibleRestrictions().Count(), UNEXPECTED_POSSIBLE_RESTRICTIONS_COUNT_FOUND);
            }
        }
    }
}
