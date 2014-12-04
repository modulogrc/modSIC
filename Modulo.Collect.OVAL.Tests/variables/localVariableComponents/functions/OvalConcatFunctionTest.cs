/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.helpers;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.functions
{

    [TestClass]
    public class OvalConcatFunctionTest
    {
        private const string NOT_EXPECTED_VALUE_WAS_FOUND = "The value is not expected";

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_concat_the_values_of_components_in_the_list()
        {
            var systemCharacteristics = new OvalDocumentLoader().GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            var components = this.GetLocalVariableComponentList(systemCharacteristics);
            var concat = new OvalConcatFunction(components);
            var concatValues = concat.ConcatValues();

            Assert.AreEqual(4, concatValues.Count, "the quantity of values is not expected");
            Assert.AreEqual(@"BuildLabEx\SoftwareType", concatValues.ElementAt(0), NOT_EXPECTED_VALUE_WAS_FOUND);
            Assert.AreEqual(@"BuildLabEx\SystemRoot", concatValues.ElementAt(1), NOT_EXPECTED_VALUE_WAS_FOUND);
            Assert.AreEqual(@"CSDBuildNumber\SoftwareType",concatValues.ElementAt(2), NOT_EXPECTED_VALUE_WAS_FOUND);
            Assert.AreEqual(@"CSDBuildNumber\SystemRoot", concatValues.ElementAt(3), NOT_EXPECTED_VALUE_WAS_FOUND);
        }


        private List<LocalVariableComponent> GetLocalVariableComponentList(oval_system_characteristics systemCharacteristics)
        {
            var components = new List<LocalVariableComponent>();
            var literalComponent = new LiteralComponentType() { Value = @"\" };
            var objectComponent1 = new ObjectComponentType() { object_ref = "oval:org.mitre.oval:obj:5000", item_field = "name" };
            var objectComponent2 = new ObjectComponentType() { object_ref = "oval:org.mitre.oval:obj:6000", item_field = "name" };
            
            var objectComponentEvaluator1 = new  LocalVariableObjectComponent(objectComponent1, systemCharacteristics);
            var literalComponentEvaluator = new LocalVariableLiteralComponent(literalComponent);
            var objectComponentEvaluator2 = new  LocalVariableObjectComponent(objectComponent2, systemCharacteristics);

            components.Add(objectComponentEvaluator1);
            components.Add(literalComponentEvaluator);
            components.Add(objectComponentEvaluator2);
            
            return components;
        }   
    }
}
