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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.WQL;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Common.Test.Checkers;
using System.Collections.Generic;

namespace Modulo.Collect.Probe.Windows.Test.WMI
{
    [TestClass]
    public class WmiItemTypeGeneratorTest
    {
        private oval_definitions OvalDefinitions;
        private const string ROOT_CIMV2_NAMESPACE = "root\\cimv2";
        private const string WQL_TO_GET_FILESYSTEM = "select FileSystem from Win32_LogicalDisk where DeviceId = 'C:'";

        public WmiItemTypeGeneratorTest()
        {
            this.OvalDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_from_wmi_object()
        {
            #region wmi_object
            //<wmi_object id="oval:modulo:obj:2200" version="1">
            //    <namespace>root\cimv2</namespace>
            //    <wql>select caption from Win32_OperatingSystem</wql>
            //</wmi_object>
            #endregion

            var generatedItems = new WmiItemTypeGenerator().GetItemsToCollect(GetOvalObjectByID("2200"), null);

            AssertGeneratedWmiItem(
                generatedItems.ToArray(), 
                "root\\cimv2", 
                "select caption from Win32_OperatingSystem");
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_from_wmi_object_with_referenced_variable_in_namespace_entity()
        {
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("2222", "22221", ROOT_CIMV2_NAMESPACE);   
            
            var generatedItems = new WmiItemTypeGenerator().GetItemsToCollect(GetOvalObjectByID("2222"), fakeVariables);

            AssertGeneratedWmiItem(
                generatedItems.ToArray(), 
                ROOT_CIMV2_NAMESPACE, 
                "select caption from Win32_OperatingSystem");
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_from_wmi_object_with_referenced_variable_in_wql_entity()
        {
            
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("2223", "22222", WQL_TO_GET_FILESYSTEM);

            var generatedItems = new WmiItemTypeGenerator().GetItemsToCollect(GetOvalObjectByID("2223"), fakeVariables);

            AssertGeneratedWmiItem(
                generatedItems.ToArray(), 
                "root\\default",
                WQL_TO_GET_FILESYSTEM);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_from_wmi_object_with_referenced_variable_in_all_entities()
        {
            var variableValues = new Dictionary<string, IEnumerable<string>>();
            variableValues.Add("oval:modulo:var:22221", new string[] { ROOT_CIMV2_NAMESPACE });
            variableValues.Add("oval:modulo:var:22222", new string[] { WQL_TO_GET_FILESYSTEM });
            var fakeVariables = VariableHelper.CreateEvaluatedVariables("2224", variableValues);

            var generatedItems = new WmiItemTypeGenerator().GetItemsToCollect(GetOvalObjectByID("2224"), fakeVariables);

            AssertGeneratedWmiItem(
                generatedItems.ToArray(),
                ROOT_CIMV2_NAMESPACE,
                WQL_TO_GET_FILESYSTEM);
        }

        private OVAL.Definitions.ObjectType GetOvalObjectByID(string objectID)
        {
            var id = objectID.Contains(":") ? objectID : string.Format("oval:modulo:obj:{0}", objectID);
            return this.OvalDefinitions.objects.Where(obj => obj.id.Equals(id)).Single();
        }

        private void AssertGeneratedWmiItem(ItemType[] itemToAssert, string expectedNamespace, string expectedWql)
        {
            ItemTypeChecker.DoBasicAssertForItems(itemToAssert, 1, typeof(wmi_item));
            var wmiItemToAssert = (wmi_item)itemToAssert.Single();
            ItemTypeEntityChecker.AssertItemTypeEntity(wmiItemToAssert.@namespace, expectedNamespace);
            ItemTypeEntityChecker.AssertItemTypeEntity(wmiItemToAssert.wql, expectedWql);
        }


    }
}
