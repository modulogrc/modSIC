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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.GroupSID;
using Rhino.Mocks;

namespace Modulo.Collect.Probe.Windows.Test.GroupSID
{
    [TestClass]
    public class GroupSIDItemTypeGeneratorTest
    {
        private TargetInfo FakeTargetInfo;

        public GroupSIDItemTypeGeneratorTest()
        {
            this.FakeTargetInfo = ProbeHelper.CreateFakeTarget();
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_items_to_collect_from_a_default_GroupSidObject()
        {
            var objectToCollect = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1110");
            var itemTypeGenerator = new GroupSIDItemTypeGenerator() { TargetInfo = FakeTargetInfo };

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectToCollect, null);

            this.DoBasicAssert(generatedItems, 1);
            this.AssertGeneratedItemsToCollect(generatedItems, new string[] { "S-1-5-32-544" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_items_to_collect_from_a_GroupSidObject_with_referenced_variable()

        {
            var objectToCollect = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1111");
            var fakeEvaluatedVariables = VariableHelper.CreateVariableWithOneValue(objectToCollect.id, "oval:modulo:var:201", "S-1-5-18");
            var itemTypeGenerator = new GroupSIDItemTypeGenerator() { TargetInfo =  FakeTargetInfo };
            
            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectToCollect, fakeEvaluatedVariables);

            this.DoBasicAssert(generatedItems, 1);
            Assert.AreEqual("S-1-5-18", ((group_sid_item)generatedItems.First()).group_sid.Value, "An unexpected item entity was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_items_to_collect_from_a_GroupSidObject_with_referenced_to_a_MultiValuedVariable()
        {
            var fakeMultiVaribleValues = new string[] { "S-1-5", "S-1-5-18" };
            var objectToCollect = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1112");
            var fakeEvaluatedVars = VariableHelper.CreateVariableWithMultiplesValue(objectToCollect.id, "oval:modulo:var:831", fakeMultiVaribleValues);
            var itemTypeGenerator = new GroupSIDItemTypeGenerator() { TargetInfo = FakeTargetInfo };

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectToCollect, fakeEvaluatedVars);

            this.DoBasicAssert(generatedItems, 2);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_items_to_collect_from_a_GroupSIDObject_with_PatternMatchOperation()
        {
            var allGroupSIDsFakeToReturn = new string[] { "S-1-1", "S-1-18", @"S-1-5-32-100", @"S-1-5-32-500", @"S-1-5-32-999", "S-1-0" };
            var objectToCollect = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1113");
            GroupSIDItemTypeGenerator mockedItemTypeGenerator = this.CreateMockedGroupSIDItemTypeGenerator(allGroupSIDsFakeToReturn);

            var generatedItems = mockedItemTypeGenerator.GetItemsToCollect(objectToCollect, null);
                
            this.DoBasicAssert(generatedItems, 3);
            this.AssertGeneratedItemsToCollect(generatedItems, new string[] { "S-1-5-32-100", "S-1-5-32-500", "S-1-5-32-999" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_generate_items_to_collect_from_a_GroupSIDObject_with_NotEqualOperation_and_referenced_variable()
        {
            var allGroupSIDsFakeToReturn = new string[] { @"S-1-1-18", "S-1-1", @"S-1-0" };
            var objectToCollect = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:1114");
            var fakeEvaluatedVariables = VariableHelper.CreateVariableWithOneValue(objectToCollect.id, "oval:modulo:var:1001", "S-1-1");
            GroupSIDItemTypeGenerator mockedItemTypeGenerator = this.CreateMockedGroupSIDItemTypeGenerator(allGroupSIDsFakeToReturn);

            var generatedItems = mockedItemTypeGenerator.GetItemsToCollect(objectToCollect, fakeEvaluatedVariables);

            this.DoBasicAssert(generatedItems, 2);
            this.AssertGeneratedItemsToCollect(generatedItems, new string[] { "S-1-1-18", "S-1-0" });
        }


        private GroupSIDItemTypeGenerator CreateMockedGroupSIDItemTypeGenerator(string[] allGroupSIDsFakeToReturn)
        {
            MockRepository mocks = new MockRepository();
            var fakeSystemDataSource = mocks.DynamicMock<BaseObjectCollector>();
            Expect.Call(fakeSystemDataSource.GetValues(null)).Return(allGroupSIDsFakeToReturn);
            mocks.ReplayAll();

            return new GroupSIDItemTypeGenerator() { TargetInfo = FakeTargetInfo, SystemDataSource = fakeSystemDataSource };
        }
    
        private void DoBasicAssert(IEnumerable<ItemType> generatedItems, int expectedItemsCount)
        {
            Assert.IsNotNull(generatedItems, "The generated items cannot be null.");
            Assert.AreEqual(expectedItemsCount, generatedItems.Count(), "An unexpected number of generated items was found.");
        }

        private void AssertGeneratedItemsToCollect(IEnumerable<ItemType> itemsToAssert, string[] expectedGroupSIDValues)
        {
            for (int i = 0; i < expectedGroupSIDValues.Count(); i++)
            {
                var itemToAssert =  (group_sid_item)itemsToAssert.ElementAt(i);
                var failTestMessage = string.Format("An unexpected item entity ('{0}') was found.", "group_sid");
                Assert.AreEqual(expectedGroupSIDValues[i], itemToAssert.group_sid.Value, failTestMessage);
            }
        }
    }
}
