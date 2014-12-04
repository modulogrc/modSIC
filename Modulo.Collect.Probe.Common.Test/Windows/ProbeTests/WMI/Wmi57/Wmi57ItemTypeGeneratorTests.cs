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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.Probes.WMI.Wmi57;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.WMI.Wmi57
{
    [TestClass]
    public class Wmi57ItemTypeGeneratorTests
    {
        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_generate_wmi57_item_to_collect()
        {
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:2201");
            var itemTypeGenerator = new Wmi57ItemTypeGenerator();

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeObject, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(wmi57_item));
            var wmiItemType = (wmi57_item)itemsToCollect.Single();
            ItemTypeEntityChecker.AssertItemTypeEntity(wmiItemType.@namespace, "root\\cimv2");
            ItemTypeEntityChecker.AssertItemTypeEntity(wmiItemType.wql, "select caption, name from Win32_OperatingSystem");
            

        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_generate_wmi57_item_to_collect_from_an_object_with_referenced_variable_in_namespace_entity()
        {
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:2202");
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("oval:modulo:obj:2202", "oval:modulo:var:2202", "root\\default");
            
            var itemsToCollect = new Wmi57ItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(wmi57_item));
            var wmiItemType = (wmi57_item)itemsToCollect.Single();
            ItemTypeEntityChecker.AssertItemTypeEntity(wmiItemType.@namespace, "root\\default");
            ItemTypeEntityChecker.AssertItemTypeEntity(wmiItemType.wql, "select caption, name from Win32_OperatingSystem");
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_generate_wmi57_item_to_collect_from_an_object_with_referenced_variable_in_wql_entity()
        {
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:2203");
            var fakeVariableValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariableValues.Add("oval:modulo:var:2202", new string[] { "root\\default", "root\\cimv2" });
            fakeVariableValues.Add("oval:modulo:var:2203", new string[] { "Select domain, name From Win32_Account" });
            var fakeVariables = VariableHelper.CreateEvaluatedVariables("oval:modulo:obj:2203", fakeVariableValues);

            var itemsToCollect = new Wmi57ItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 2, typeof(wmi57_item));
            ItemTypeEntityChecker.AssertItemTypeEntity(((wmi57_item)itemsToCollect[0]).@namespace, "root\\default");
            ItemTypeEntityChecker.AssertItemTypeEntity(((wmi57_item)itemsToCollect[0]).wql, "Select domain, name From Win32_Account");
            ItemTypeEntityChecker.AssertItemTypeEntity(((wmi57_item)itemsToCollect[1]).@namespace, "root\\cimv2");
            ItemTypeEntityChecker.AssertItemTypeEntity(((wmi57_item)itemsToCollect[1]).wql, "Select domain, name From Win32_Account");
        }
    }
}
