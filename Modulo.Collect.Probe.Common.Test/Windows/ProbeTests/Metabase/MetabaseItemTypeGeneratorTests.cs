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
using Modulo.Collect.Probe.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.Probes.Metabase;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.File;

namespace Modulo.Collect.Probe.Windows.Tests.Probes.Metabase
{
    [TestClass]
    public class MetabaseItemTypeGeneratorTests
    {
        private OVAL.Definitions.oval_definitions FakeOvalDefinitions;
        private VariablesEvaluated FakeEvaluatedVariables;
        

        public MetabaseItemTypeGeneratorTests()
        {
            this.FakeOvalDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            
            var evaluatedVariables = new Dictionary<string, Dictionary<string, IList<string>>>();
            foreach (var obj in FakeOvalDefinitions.objects.OfType<metabase_object>())
            {
                {
                    var idEntity = obj.GetIdEntity();
                    if (idEntity == null)
                        continue;

                    if (!string.IsNullOrEmpty(idEntity.var_ref))
                    {
                        var refVariable = FakeOvalDefinitions.variables.Where(v => v.id.Equals(idEntity.var_ref)).Single();
                        var variableValues = ((VariablesTypeVariableConstant_variable)refVariable).value.Select(v => v.Value);
                        var variables = new Dictionary<string, IList<string>>();
                        variables.Add(refVariable.id, variableValues.ToList());

                        evaluatedVariables.Add(obj.id, variables);
                    }
                }
                {
                    var keyEntityVarRef = obj.GetKeyEntity().var_ref;
                    if (!string.IsNullOrEmpty(keyEntityVarRef))
                    {
                        var refVariable = FakeOvalDefinitions.variables.Where(v => v.id.Equals(keyEntityVarRef)).Single();
                        var variableValues = ((VariablesTypeVariableConstant_variable)refVariable).value.Select(v => v.Value);
                        var variables = new Dictionary<string, IList<string>>();
                        variables.Add(refVariable.id, variableValues.ToList());

                        if (evaluatedVariables.ContainsKey(obj.id))
                            evaluatedVariables[obj.id].Add(refVariable.id, variableValues.ToList());
                        else
                            evaluatedVariables.Add(obj.id, variables);
                    }
                }
            }

            FakeEvaluatedVariables = VariableHelper.CreateEvaluatedVariables(evaluatedVariables);

            //{
            //    var metabaseObject6001 =
            //        (metabase_object)this.FakeOvalDefinitions.objects.Where(obj => obj.id.Equals("oval:modulo:obj:6001")).Single();
            //    var varRef = ((VariablesTypeVariableConstant_variable)
            //        FakeOvalDefinitions.variables.Where(v => v.id.Equals(metabaseObject6001.GetIdEntity().var_ref)).Single());
            //    var variableValues = new Dictionary<string, IEnumerable<string>>();
            //    variableValues.Add(varRef.id, varRef.value.Select(v => v.Value));
            //    this.FakeEvaluatedVariables6001 = VariableHelper.CreateEvaluatedVariables(metabaseObject6001.id, variableValues);
            //}

            //{
            //    var metabaseObject6003 =
            //        (metabase_object)this.FakeOvalDefinitions.objects.Where(obj => obj.id.Equals("oval:modulo:obj:6003")).Single();
            //    var varRef1 = ((VariablesTypeVariableConstant_variable)
            //        FakeOvalDefinitions.variables.Where(v => v.id.Equals(metabaseObject6003.GetIdEntity().var_ref)).Single());
            //    var variableValues6003 = new Dictionary<string, IEnumerable<string>>();
            //    variableValues6003.Add(varRef1.id, varRef1.value.Select(v => v.Value));
            //    this.FakeEvaluatedVariables6003 = VariableHelper.CreateEvaluatedVariables(metabaseObject6003.id, variableValues6003);
            //}
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_to_generate_metabase_items_from_an_simple_object()
        {
            var fakeOvalMetabaseObject = ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6000");
             
            var itemsToCollect = new MetabaseItemTypeGenerator().GetItemsToCollect(fakeOvalMetabaseObject, null);

            Assert.IsNotNull(itemsToCollect);
            Assert.AreEqual(1, itemsToCollect.Count());
            Assert.IsInstanceOfType(itemsToCollect.ElementAt(0), typeof(metabase_item));
            var itemToAssert = (metabase_item)itemsToCollect.ElementAt(0);
            AssertMetabaseEntity(itemToAssert.id1, "6030");
            AssertMetabaseEntity(itemToAssert.key, "W3SVC");
        }



        [TestMethod]
        public void Should_be_to_generate_metabase_items_from_an_object_with_variable_in_id_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6001");

            var itemsToCollect = new MetabaseItemTypeGenerator().GetItemsToCollect(fakeOvalMetabaseObject, FakeEvaluatedVariables);

            Assert.AreEqual(1, itemsToCollect.Count());
            var itemToAssert = (metabase_item)itemsToCollect.ElementAt(0);
            AssertMetabaseEntity(itemToAssert.id1, "1");
            AssertMetabaseEntity(itemToAssert.key, "W3SVC");
        }

        [TestMethod]
        public void Should_be_to_generate_metabase_items_from_an_object_with_multiple_variables_in_id_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6003");

            var itemsToCollect = new MetabaseItemTypeGenerator().GetItemsToCollect(fakeOvalMetabaseObject, FakeEvaluatedVariables);

            Assert.AreEqual(3, itemsToCollect.Count());
            var firstItem = (metabase_item)itemsToCollect.ElementAt(0);
            var secondItem = (metabase_item)itemsToCollect.ElementAt(1);
            var thirdItem = (metabase_item)itemsToCollect.ElementAt(2);

            AssertMetabaseEntity(firstItem.id1, "1000");
            AssertMetabaseEntity(secondItem.id1, "1001");
            AssertMetabaseEntity(thirdItem.id1, "1002");
        }

        [TestMethod]
        public void Should_be_to_generate_metabase_items_from_an_object_with_variable_in_id_and_key_entities()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6004");

            var itemsToCollect = new MetabaseItemTypeGenerator().GetItemsToCollect(fakeOvalMetabaseObject, FakeEvaluatedVariables);

            Assert.AreEqual(1, itemsToCollect.Count());
            var metabaseItem = ((metabase_item)itemsToCollect.ElementAt(0));
            AssertMetabaseEntity(metabaseItem.key, @"c:\temp");
            AssertMetabaseEntity(metabaseItem.id1, "9");
        }

        [TestMethod]
        public void Should_be_to_generate_metabase_items_from_an_object_with_variable_with_multiple_values_in_key_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6005");

            var itemsToCollect = new MetabaseItemTypeGenerator().GetItemsToCollect(fakeOvalMetabaseObject, FakeEvaluatedVariables);

            Assert.AreEqual(2, itemsToCollect.Count());
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_generate_metabase_items_from_an_object_with_notequal_operation_in_id_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6006");

            var itemsToCollect = CreateMetabaseItemTypeGeneratorWithBehaviorInID(new string[] { "1001", "1002", "1003" }).GetItemsToCollect(fakeOvalMetabaseObject, null);

            Assert.AreEqual(2, itemsToCollect.Count(), "Unexpected amount of items to collect.");
            var firstItem = (metabase_item)itemsToCollect.ElementAt(0);
            var secondItem = (metabase_item)itemsToCollect.ElementAt(1);
            AssertMetabaseEntity(firstItem.key, "/");
            AssertMetabaseEntity(secondItem.key, "/");
            AssertMetabaseEntity(firstItem.id1, "1001");
            AssertMetabaseEntity(secondItem.id1, "1003");
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_generate_metabase_items_from_an_object_with_greaterthan_operation_in_id_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6007");

            var itemsToCollect = CreateMetabaseItemTypeGeneratorWithBehaviorInID(new string[] { "1001", "1002", "1003" })
                .GetItemsToCollect(fakeOvalMetabaseObject, null);

            Assert.AreEqual(1, itemsToCollect.Count(), "Unexpected amount of items to collect.");
            var firstItem = (metabase_item)itemsToCollect.ElementAt(0);
            AssertMetabaseEntity(firstItem.key, "/");
            AssertMetabaseEntity(firstItem.id1, "1003");
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_generate_metabase_items_from_an_object_with_greaterthanorequal_operation_in_id_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6008");

            var itemsToCollect = CreateMetabaseItemTypeGeneratorWithBehaviorInID(new string[] { "1001", "1002", "1003", "1004" })
                .GetItemsToCollect(fakeOvalMetabaseObject, null);

            Assert.AreEqual(3, itemsToCollect.Count(), "Unexpected amount of items to collect.");
            var firstItem = (metabase_item)itemsToCollect.ElementAt(0);
            var secondItem = (metabase_item)itemsToCollect.ElementAt(1);
            var thirdItem = (metabase_item)itemsToCollect.ElementAt(2);
            AssertMetabaseEntity(firstItem.key, "/");
            AssertMetabaseEntity(firstItem.id1, "1002");
            AssertMetabaseEntity(secondItem.key, "/");
            AssertMetabaseEntity(secondItem.id1, "1003");
            AssertMetabaseEntity(thirdItem.key, "/");
            AssertMetabaseEntity(thirdItem.id1, "1004");            
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_generate_metabase_items_from_an_object_with_lessthan_operation_in_id_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6009");

            var itemsToCollect = CreateMetabaseItemTypeGeneratorWithBehaviorInID(new string[] { "1001", "1002", "1003", "1004" })
                .GetItemsToCollect(fakeOvalMetabaseObject, null);

            Assert.AreEqual(4, itemsToCollect.Count(), "Unexpected amount of items to collect.");
            var firstItem = (metabase_item)itemsToCollect.ElementAt(0);
            var secondItem = (metabase_item)itemsToCollect.ElementAt(1);
            var thirdItem = (metabase_item)itemsToCollect.ElementAt(2);
            var fourthItem = (metabase_item)itemsToCollect.ElementAt(3);
            AssertMetabaseEntity(firstItem.key, "/");
            AssertMetabaseEntity(firstItem.id1, "1001");
            AssertMetabaseEntity(secondItem.key, "/");
            AssertMetabaseEntity(secondItem.id1, "1002");
            AssertMetabaseEntity(thirdItem.key, "/");
            AssertMetabaseEntity(thirdItem.id1, "1003");
            AssertMetabaseEntity(fourthItem.key, "/");
            AssertMetabaseEntity(fourthItem.id1, "1004"); 
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_generate_metabase_items_from_an_object_with_lessthanorequal_operation_in_id_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6010");

            var itemsToCollect = CreateMetabaseItemTypeGeneratorWithBehaviorInID(new string[] { "1000", "1001" })
                .GetItemsToCollect(fakeOvalMetabaseObject, null);

            Assert.AreEqual(2, itemsToCollect.Count(), "Unexpected amount of items to collect.");
            var firstItem = (metabase_item)itemsToCollect.ElementAt(0);
            var secondItem = (metabase_item)itemsToCollect.ElementAt(1);  
            AssertMetabaseEntity(firstItem.key, "/");
            AssertMetabaseEntity(firstItem.id1, "1000");
            AssertMetabaseEntity(secondItem.key, "/");
            AssertMetabaseEntity(secondItem.id1, "1001");
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_generate_metabase_items_from_an_object_with_notequal_operation_in_key_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6011");

            var itemsToCollect = CreateMetabaseItemTypeGeneratorWithBehaviorInKey(new string[] { "/", "a" })
                .GetItemsToCollect(fakeOvalMetabaseObject, null);

            Assert.AreEqual(1, itemsToCollect.Count(), "Unexpected amount of items to collect.");
            var firstItem = (metabase_item)itemsToCollect.ElementAt(0);
            AssertMetabaseEntity(firstItem.key, "a");
            AssertMetabaseEntity(firstItem.id1, "1001");
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_generate_metabase_items_from_an_object_with_patternmatch_operation_in_key_entity()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6012");

            var itemsToCollect = CreateMetabaseItemTypeGeneratorWithBehaviorInKey(new string[] { "/LM/W3SVC/1/", "aaa" })
                .GetItemsToCollect(fakeOvalMetabaseObject, null);

            Assert.AreEqual(1, itemsToCollect.Count(), "Unexpected amount of items to collect.");
            var firstItem = (metabase_item)itemsToCollect.ElementAt(0);
            AssertMetabaseEntity(firstItem.key, "/LM/W3SVC/1/");
            AssertMetabaseEntity(firstItem.id1, "5512");
        }

        [TestMethod]
        public void Should_be_possible_to_get_IdEntity_from_a_metabase_object()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6000");

            var idEntity = fakeOvalMetabaseObject.GetIdEntity();

            Assert.IsNotNull(idEntity, "The Id Entity was not found in metabase object.");
            Assert.AreEqual("6030", idEntity.Value, "Unexpected entity value was found in id entity of given metabase object.");
        }

        [TestMethod]
        public void Should_be_possible_to_get_KeyEntity_from_a_metabase_object()
        {
            var fakeOvalMetabaseObject = (metabase_object)ProbeHelper.GetOvalComponentByOvalID(FakeOvalDefinitions, "6000");

            var keyEntity = fakeOvalMetabaseObject.GetKeyEntity();

            Assert.IsNotNull(keyEntity, "The Key Entity was not found in metabase object.");
            Assert.AreEqual("W3SVC", keyEntity.Value, "Unexpected entity value was found in id entity of given metabase object.");
        }        

        private void AssertMetabaseEntity(EntityItemSimpleBaseType entityToAssert, string expectedEntityValue)
        {

            Assert.IsNotNull(entityToAssert, "A null metabase entity was found.");
            Assert.AreEqual(expectedEntityValue, entityToAssert.Value, "An unexpected entity value was found..");
        }

        private MetabaseItemTypeGenerator CreateMetabaseItemTypeGeneratorWithBehaviorInID(string[] itemsToEvaluate)
        {
            var mocks = new MockRepository();
            
            var fakeWindowsFileProvider = mocks.DynamicMock<WindowsFileProvider>(new object[] { null });
            Expect.Call(fakeWindowsFileProvider.GetFileLinesContentFromHost(null)).IgnoreArguments().Repeat.Any().Return(null);
            mocks.ReplayAll();
            
            var fakeObjectCollector = mocks.DynamicMock<MetabaseObjectCollector>(new object[] { fakeWindowsFileProvider });
            Expect.Call(fakeObjectCollector.GetAllMetabaseIDs()).Return(itemsToEvaluate);
            mocks.ReplayAll();

            return new MetabaseItemTypeGenerator() { ObjectCollector = fakeObjectCollector };
        }

        private MetabaseItemTypeGenerator CreateMetabaseItemTypeGeneratorWithBehaviorInKey(string[] itemsToEvaluate)
        {
            var mocks = new MockRepository();
            
            var fakeWindowsFileProvider = mocks.DynamicMock<WindowsFileProvider>(new object[] { null });
            Expect.Call(fakeWindowsFileProvider.GetFileLinesContentFromHost(null)).IgnoreArguments().Repeat.Any().Return(null);
            mocks.ReplayAll();
            
            var fakeObjectCollector = mocks.DynamicMock<MetabaseObjectCollector>(new object[] { fakeWindowsFileProvider });
            Expect.Call(fakeObjectCollector.GetAllMetabaseKeys()).Return(itemsToEvaluate);
            mocks.ReplayAll();

            return new MetabaseItemTypeGenerator() { ObjectCollector = fakeObjectCollector };
        }
    }
}
