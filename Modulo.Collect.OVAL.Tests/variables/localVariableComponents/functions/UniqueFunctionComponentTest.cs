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
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.variables.localVariableComponents.Builders;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.functions
{
    /// <summary>
    /// Summary description for ConcatFunctionComponentTest
    /// </summary>
    [TestClass]
    public class UniqueFunctionComponentTest
    {
 
        private OvalDocumentLoader ovalDocument;   
         [TestInitialize()]
         public void MyTestInitialize() {
             this.ovalDocument = new OvalDocumentLoader();
         }
       

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_get_same_values_for_unique_list()
        {                      

            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                CreateTheLocalVariable().
                    WithUnique().
                        AddLiteralComponent(@"banana").
                        AddLiteralComponent(@"apple").
                    SetInLocalVariable().
                Build();

            oval_definitions definitions = this.ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            Assert.IsNotNull(definitions, "the definitions was loaded");

            oval_system_characteristics systemCharacteristics = this.ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            Assert.IsNotNull(systemCharacteristics,"the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics,definitions.variables);

            var uniqueFunctionComponent = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(uniqueFunctionComponent, typeof(UniqueFunctionComponent));
            IEnumerable<string> values = uniqueFunctionComponent.GetValue();
            Assert.AreEqual(2, values.Count());
            Assert.AreEqual("banana", values.ElementAt(0));
            Assert.AreEqual("apple", values.ElementAt(1));
        }

        [TestMethod, Owner("mgaspar")]
        public void Should_be_possible_to_get_only_unique_values_for_a_list_with_duplicates()
        {

            VariablesTypeVariableLocal_variable localVariable = LocalVariableBuilder.
                CreateTheLocalVariable().
                    WithUnique().
                        AddLiteralComponent(@"banana").
                        AddLiteralComponent(@"banana").
                        AddLiteralComponent(@"apple").
                        AddLiteralComponent(@"banana").
                    SetInLocalVariable().
                Build();

            oval_definitions definitions = this.ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            Assert.IsNotNull(definitions, "the definitions was loaded");

            oval_system_characteristics systemCharacteristics = this.ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics was not loaded");

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);

            var uniqueFunctionComponent = factory.GetLocalVariableComponent(localVariable);
            Assert.IsInstanceOfType(uniqueFunctionComponent, typeof(UniqueFunctionComponent));
            IEnumerable<string> values = uniqueFunctionComponent.GetValue();
            Assert.AreEqual(2, values.Count());
            Assert.AreEqual("banana", values.ElementAt(0));
            Assert.AreEqual("apple", values.ElementAt(1));
        }

        
    }
}
