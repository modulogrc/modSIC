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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.Definitions.validators;

namespace Modulo.Collect.OVAL.Tests.validators
{
    [TestClass]
    public class ExternalVariableValidatorTest
    {
        private ExternalVariableFactory ExternalVariableFactory;
        private ExternalVariableValidator Validator;

        public ExternalVariableValidatorTest()
        {
            this.ExternalVariableFactory = new ExternalVariableFactory("fdcc_xpfirewall_oval.xml");
            this.Validator = new ExternalVariableValidator();
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_validate_a_value_based_on_possibleValues_defined_in_externalVariable()
        {
            var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:6672");

            Assert.AreEqual(1, Validator.ValidateValue(externalVariable, "50").Count());
            Assert.AreEqual(0, Validator.ValidateValue(externalVariable, "0").Count());
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_validate_a_value_based_on_PossibleRestrictionValues_defined_in_externalVariable()
        {
            var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:261");

            Assert.AreEqual(1, Validator.ValidateValue(externalVariable, "4294967298").Count());
            Assert.AreEqual(0, Validator.ValidateValue(externalVariable, "0").Count());
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_validate_a_value_based_on_PossibleRestrictionValues_using_the_PossibleValue_as_execption_value()
        {
            var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:261");

            Assert.AreEqual(0, Validator.ValidateValue(externalVariable, "4294967295").Count());
            Assert.AreEqual(0, Validator.ValidateValue(externalVariable, "0").Count());
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_validate_a_value_if_external_variable_there_is_not_possibleValues_or_Restrictions()
        {
            var externalVariable = ExternalVariableFactory.GetExternalVariableFromDefinitionsById("oval:gov.nist.fdcc.xp:var:262");

            Assert.AreEqual(0, Validator.ValidateValue(externalVariable, "test").Count());
            Assert.AreEqual(0, Validator.ValidateValue(externalVariable, "test333").Count());
        }
    }
}
