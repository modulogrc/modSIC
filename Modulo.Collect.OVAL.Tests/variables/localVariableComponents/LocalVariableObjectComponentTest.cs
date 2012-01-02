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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;


namespace Modulo.Collect.OVAL.Tests.Variables.LocalVariableComponents

{
    [TestClass]
    public class LocalVariableObjectComponentTest
    {
        private const string UNEXPECTED_VARIABLE_VALUE_FOUND = "An unexpected variable value was found.";
        private const string UNEXPECTED_VARIABLE_VALUE_AMOUNT = "Unexpected amount of variable values was found.";
        private oval_system_characteristics FakeSystemCharacteristics;


        public LocalVariableObjectComponentTest()
        {
            this.FakeSystemCharacteristics = new OvalDocumentLoader().GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_ro_get_value_to_objectComponentType()
        {
            var component = GetObjectComponentType("oval:org.mitre.oval:obj:3000", "key");
            var values = new LocalVariableObjectComponent(component, FakeSystemCharacteristics).GetValue();
            Assert.IsTrue(values.Count() > 0, UNEXPECTED_VARIABLE_VALUE_AMOUNT);
            Assert.AreEqual(@"Software\Microsoft\Windows NT\CurrentVersion", values.ElementAt(0), UNEXPECTED_VARIABLE_VALUE_FOUND);

            component = GetObjectComponentType("oval:org.mitre.oval:obj:1000", "family");
            values = new LocalVariableObjectComponent(component, FakeSystemCharacteristics).GetValue();
            Assert.IsTrue(values.Count() > 0, UNEXPECTED_VARIABLE_VALUE_AMOUNT);
            Assert.AreEqual("windows", values.ElementAt(0), UNEXPECTED_VARIABLE_VALUE_FOUND);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_value_to_objectComponentType_if_EntityItemBaseType_is_an_array()
        {
            var component = GetObjectComponentType("oval:org.mitre.oval:obj:8000", "value");

            var values = new LocalVariableObjectComponent(component, FakeSystemCharacteristics).GetValue();

            Assert.IsTrue(values.Count() > 0, UNEXPECTED_VARIABLE_VALUE_AMOUNT);
            Assert.AreEqual("4.0", values.ElementAt(0), UNEXPECTED_VARIABLE_VALUE_FOUND);            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_value_to_from_an_objectComponentType_that_contains_a_record_entity_type()
        {
            var component = GetObjectComponentType("oval:org.mitre.oval:obj:10100", "result", "uid");

            var values = new LocalVariableObjectComponent(component, FakeSystemCharacteristics).GetValue();

            Assert.AreEqual(1, values.Count(), UNEXPECTED_VARIABLE_VALUE_AMOUNT);
            Assert.AreEqual("2048", values.First(), UNEXPECTED_VARIABLE_VALUE_FOUND);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_value_to_from_an_objectComponentType_that_contains_a_record_entity_type_array()
        {
            var component = GetObjectComponentType("oval:org.mitre.oval:obj:10101", "result", "type");

            var values = new LocalVariableObjectComponent(component, FakeSystemCharacteristics).GetValue();

            Assert.AreEqual(2, values.Count(), UNEXPECTED_VARIABLE_VALUE_AMOUNT);
            Assert.AreEqual("TABLE", values.First(), UNEXPECTED_VARIABLE_VALUE_FOUND);
            Assert.AreEqual("VIEW", values.Last(), UNEXPECTED_VARIABLE_VALUE_FOUND);

            
        }

        private static ObjectComponentType GetObjectComponentType(string objectReference, string item_ref, string recordField = null)
        {
            return new ObjectComponentType()
            {
                object_ref = objectReference,
                item_field = item_ref,
                record_field = recordField
            };
        }
    }
}
