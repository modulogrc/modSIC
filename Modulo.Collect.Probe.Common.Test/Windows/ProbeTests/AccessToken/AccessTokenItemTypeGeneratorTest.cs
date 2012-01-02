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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.WMI;

using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Definitions = Modulo.Collect.OVAL.Definitions;
using SysCharac = Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Windows.AccessToken;
using Modulo.Collect.Probe.Windows.Providers;


namespace Modulo.Collect.Probe.Windows.Test.AccessToken
{
    [TestClass]
    public class AccessTokenItemTypeGeneratorTest
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_AccessTokenItems_to_collect()
        {
            var objectType  = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "840");

            var generatedItems = new AccessTokenItemTypeGenerator(null).GetItemsToCollect(objectType, null);

            this.DoBasicAssert(generatedItems, 1);
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(0), @"mss\lfernandes");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_PatternMatchOperation_to_SecurityPrincipleEntity()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "850");
            // <security_principle operation="pattern match">mss\\.*fernandes$</security_principle>
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(new string[] { "mss\\lfernandes", "lfernandes", "mss\\cpaiva", "mss\\bfernandes" }, null);
            
            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectType, null);

            this.DoBasicAssert(generatedItems, 2);
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(0), "mss\\lfernandes");
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(1), "mss\\bfernandes");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_from_SecurityPrinciple_referencing_a_local_variable()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "860");
            var fakeVariables = VariableHelper.CreateVariableWithOneValue(objectType.id, "oval:modulo:var:860", "ADMINISTRATOR");
            
            var generatedItems =  new AccessTokenItemTypeGenerator(null).GetItemsToCollect(objectType, fakeVariables);

            this.DoBasicAssert(generatedItems, 1);
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(0), "ADMINISTRATOR");

        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_items_from_AccessTokenObject_with_referenced_variable_AND_NotEqualOperation()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "870");
            var fakeVariables = VariableHelper.CreateVariableWithOneValue(objectType.id, "oval:modulo:var:10", "mss\\lfernandes");
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(new string[] { "LAB\\admin", "Guest", "mss\\bfernandes", "mss\\lfernandesa" }, null);

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectType, fakeVariables);

            this.DoBasicAssert(generatedItems, 6);
            generatedItems = generatedItems.OrderBy(item => ((accesstoken_item)item).security_principle.Value);

            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(0), "Group1");
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(1), "Group2");
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(2), "Guest");
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(3), "LAB\\admin");
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(4), "mss\\bfernandes");
            this.AssertGeneratedAccessTokenItem(generatedItems.ElementAt(5), "mss\\lfernandesa");

        }


        private AccessTokenItemTypeGenerator CreateMockedItemTypeGenerator(IEnumerable<String> fakeSIDsToReturn, Exception exceptionToThrow)
        {
            var mocks = new MockRepository();
            var fakeAccountProvider = mocks.DynamicMock<WindowsUsersProvider>(new object[] { null, null } );

            if (exceptionToThrow == null)
            {
                var fakeWindowsAccount = fakeSIDsToReturn.Select(sid => new WindowsAccount(sid, true, sid, AccountType.User)).ToList();
                foreach (var account in fakeWindowsAccount)
                {
                    account.AddMember("Group1", (bool?)true, "S-1-1");
                    account.AddMember("Group2", (bool?)true, "S-1-0");
                }

                Expect.Call(fakeAccountProvider.GetAllGroupByUsers()).IgnoreArguments().Return(fakeWindowsAccount);
            }
            else
                Expect.Call(fakeAccountProvider.GetAllGroupByUsers()).IgnoreArguments().Throw(exceptionToThrow);

            mocks.ReplayAll();

            return new AccessTokenItemTypeGenerator(fakeAccountProvider);

        }

        private void DoBasicAssert(IEnumerable<ItemType> generatedItems, int expectedItemsCount)
        {
            Assert.IsNotNull(generatedItems, "The return of GetItemsToCollect cannot be null.");
            Assert.AreEqual(expectedItemsCount, generatedItems.Count(), "Unexpected quantity of generated items.");
            foreach (var item in generatedItems)
            {
                Assert.IsInstanceOfType(item, typeof(accesstoken_item), "Unxpected generated item type was found.");
                Assert.AreEqual(StatusEnumeration.exists, item.status, "Unexpected status was found in some generated item.");
            }
        }

        private void AssertGeneratedAccessTokenItem(ItemType itemToAssert, string expectedSecurityPrinciple)
        {
            accesstoken_item generatedItem = (accesstoken_item)itemToAssert;
            // Asserting SECURITY_PRINCIPLE item entity value
            Assert.IsNotNull(generatedItem.security_principle, "The entity item 'security_principle' cannot be null");
            Assert.AreEqual(expectedSecurityPrinciple, generatedItem.security_principle.Value, "An entity item ('security_principle') with unexpected value was found.");
        }


    }
}
