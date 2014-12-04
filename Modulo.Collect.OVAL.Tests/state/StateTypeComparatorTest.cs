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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.helpers;

namespace Modulo.Collect.OVAL.Tests.state
{
    /// <summary>
    /// Summary description for StateTypeComparatorTest
    /// </summary>
    [TestClass]
    public class StateTypeComparatorTest
    {
        private oval_system_characteristics systemCharacteristics;
        
        private IEnumerable<StateType> states;

        [TestInitialize]
        public void MyTestInitialize() 
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            this.systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");
            oval_definitions definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithSet.xml");
            this.states = definitions.states;
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_compare_the_state_of_itemType()
        {
            ItemType itemType = this.systemCharacteristics.GetSystemDataByReferenceId("3");
            StateType state = this.states.Where(obj => obj.id == "oval:org.mitre.oval:ste:99").SingleOrDefault();
            StateTypeComparator comparator = new StateTypeComparator(state,itemType,null);
            Assert.AreEqual(true, comparator.IsEquals());
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(NotSupportedException))]
        public void Should_not_possible_to_evaluate_variables_that_has_multiples_values()
        {
            List<string> values = new List<string>() { "1.0","1.1","1.2" };
            VariableValue variable = new VariableValue("oval:org.mitre.oval:obj:3000", "oval:org.mitre.oval:var:3000", values);
            VariablesEvaluated variables = new VariablesEvaluated(new List<VariableValue>() { variable });
            ItemType itemType = this.systemCharacteristics.GetSystemDataByReferenceId("3");
            StateType state = this.states.Where(obj => obj.id == "oval:org.mitre.oval:ste:100").SingleOrDefault();
            StateTypeComparator comparator = new StateTypeComparator(state, itemType, variables);
            comparator.IsEquals();

        }
    }
}
