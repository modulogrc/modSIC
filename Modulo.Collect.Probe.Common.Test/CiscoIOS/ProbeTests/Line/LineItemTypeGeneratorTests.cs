/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.CiscoIOS.Probes.Line;
using Modulo.Collect.OVAL.Definitions.Ios;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics.Ios;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.CiscoIOS.Tests.ProbeTests.Line
{
    [TestClass]
    public class LineItemTypeGeneratorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_line_item()
        {
            var fakeLineObject = CreateLineObject();
            var itemTypeGenerator = new LineItemTypeGenerator();

            var generatedItems = itemTypeGenerator.GetItemsToCollect(fakeLineObject, null);

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(line_item));
            var lineItem = generatedItems.OfType<line_item>().Single();
            ItemTypeEntityChecker.AssertItemTypeEntity(lineItem.show_subcommand, "show running-config", "show_subcommand");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_process_variables_for_line_object()
        {
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("oval:modsic.tests:obj:1", "oval:modsic.tests:var:1", "show snmp");
            var fakeLineObject = this.CreateLineObjectWithVariable();

            var generatedItems = new LineItemTypeGenerator().GetItemsToCollect(fakeLineObject, fakeVariables);

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(line_item));
            var lineItem = generatedItems.OfType<line_item>().Single();
            ItemTypeEntityChecker.AssertItemTypeEntity(lineItem.show_subcommand, "show snmp", "show_subcommand");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_process_multiple_variables_for_line_object()
        {
            var fakeLineObject = this.CreateLineObjectWithVariable();
            var fakeVariables = 
                VariableHelper
                    .CreateVariableWithMultiplesValue(
                        "oval:modsic.tests:obj:1", "oval:modsic.tests:var:1", new string[] { "show snmp", "show ipconfig" });
            
            var generatedItems = new LineItemTypeGenerator().GetItemsToCollect(fakeLineObject, fakeVariables);

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 2, typeof(line_item));
            var lineItems = generatedItems.OfType<line_item>();
            ItemTypeEntityChecker.AssertItemTypeEntity(lineItems.ElementAt(0).show_subcommand, "show snmp", "show_subcommand");
            ItemTypeEntityChecker.AssertItemTypeEntity(lineItems.ElementAt(1).show_subcommand, "show ipconfig", "show_subcommand");
        }

        private line_object CreateLineObject()
        {
            return
                new line_object()
                {
                    id = "oval:modsic.tests:obj:1",
                    Items = new object[] { new EntityObjectStringType() { Value = "show running-config" } }
                };
        }

        private line_object CreateLineObjectWithVariable()
        {
                return new line_object
                {
                    id = "oval:modsic.tests:obj:1",
                    Items = new object[] { new EntityObjectStringType() { var_ref = "oval:modsic.tests:var:1" } }
                };

        }
    }
}
