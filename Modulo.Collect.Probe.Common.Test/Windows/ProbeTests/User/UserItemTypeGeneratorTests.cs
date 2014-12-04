/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.Windows.User;
using Modulo.Collect.Probe.Windows.Test.Factories.Objects;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Common;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.Providers;


namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.User
{
    [TestClass]
    public class UserItemTypeGeneratorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_user_object()
        {
            var fakeObject = new UserObjectFactory().NewObject("Guest");
            
            var itemsToCollect = new UserItemTypeGenerator(null).GetItemsToCollect(fakeObject, null);

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(user_item), StatusEnumeration.notcollected);
            ItemTypeEntityChecker.AssertItemTypeEntity(itemsToCollect.OfType<user_item>().Single().user, "Guest");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_user_object_with_variable_reference()
        {
            var variableID = "oval:org.modsic:var:1";
            var fakeObject = new UserObjectFactory().NewObjectWithVariable(variableID);
            var fakeVariables = VariableHelper.CreateVariableWithOneValue(fakeObject.id, variableID, "Everyone");
            
            var itemsToCollect = new UserItemTypeGenerator(null).GetItemsToCollect(fakeObject, fakeVariables);

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(user_item), StatusEnumeration.notcollected);
            ItemTypeEntityChecker.AssertItemTypeEntity(itemsToCollect.OfType<user_item>().Single().user, "Everyone");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_user_object_with_pattern_match_operation()
        {
            var variableID = "oval:org.modsic:var:1";
            var fakeObject = new UserObjectFactory().NewObjectWithVariable(variableID, OperationEnumeration.patternmatch);
            var fakeVariables = VariableHelper.CreateVariableWithOneValue(fakeObject.id, variableID, ".*");
            var itemTypeGenerator = CreateItemTypeGeneratorForPatternMatchOperation();

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeObject, fakeVariables);

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 2, typeof(user_item));
            var firstCollectedItem = itemsToCollect.OfType<user_item>().First();
            var secondCollectedItem = itemsToCollect.OfType<user_item>().Last();
            AssertCollectedUserItemEntities(firstCollectedItem, "Guest", "0", "group1", "group2");
            AssertCollectedUserItemEntities(secondCollectedItem, "Everyone", "1", "group1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_user_object_with_not_equal_operation()
        {
            var variableID = "oval:org.modsic:var:1";
            var fakeObject = new UserObjectFactory().NewObjectWithVariable(variableID, OperationEnumeration.notequal);
            var fakeVariables = VariableHelper.CreateVariableWithOneValue(fakeObject.id, variableID, "Guest");
            var itemTypeGenerator = CreateItemTypeGeneratorForPatternMatchOperation();

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeObject, fakeVariables);

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 1, typeof(user_item));
            var firstCollectedItem = itemsToCollect.OfType<user_item>().First();
            AssertCollectedUserItemEntities(firstCollectedItem, "Everyone", "1", "group1");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_accounts_with_null_disabled_property()
        {
            var fakeUseObject = new UserObjectFactory().NewObject(".*", OperationEnumeration.patternmatch);
            var itemTypeGenerator = CreateItemTypeGeneratorForPatternMatchOperation(true);

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeUseObject, null);

            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect, 3, typeof(user_item));

        }


        private void AssertCollectedUserItemEntities(
            user_item collectedItem, string expectedName, string expectedEnabled, params string[] expectedGroups)
        {
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.user, expectedName, "user");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.enabled, expectedEnabled, "enabled");
            Assert.AreEqual(expectedGroups.Count(), collectedItem.group.Count(), "Unexpected amount of group was found.");
            for(int i = 0; i < expectedGroups.Count(); i++)
                ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.group.ElementAt(i), expectedGroups.ElementAt(i), "group");
        }


        private UserItemTypeGenerator CreateItemTypeGeneratorForPatternMatchOperation(bool includeUserWithNullEnabledProperty = false)
        {
            var fakeUserAccounts = new List<WindowsAccount>(new[] { NewGuestAccount(), NewEveryoneAccount() });
            if (includeUserWithNullEnabledProperty)
                fakeUserAccounts.Add(new WindowsAccount("DomainAccount", null, "", AccountType.User));
                    
            var mocks = new MockRepository();
            var fakeAccountProvider = mocks.DynamicMock<WindowsUsersProvider>(new object[] { null, null });
            Expect.Call(fakeAccountProvider.GetAllGroupByUsers()).Return(fakeUserAccounts);
            mocks.ReplayAll();
            
            return new UserItemTypeGenerator(fakeAccountProvider);
        }

        private WindowsAccount NewGuestAccount()
        {
            var guestAccount = new WindowsAccount("Guest", false, "", AccountType.User);
            guestAccount.AddMember("group1", true, "");
            guestAccount.AddMember("group2", null, "");
            return guestAccount;
        }

        private WindowsAccount NewEveryoneAccount()
        {
            var everyoneAccount = new WindowsAccount("Everyone", true, "", AccountType.User);
            everyoneAccount.AddMember("group1", true, "");
            return everyoneAccount;
        }
    }
}
