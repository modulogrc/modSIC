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
using Modulo.Collect.Probe.Windows.UserSID55;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Windows.Providers;
using Modulo.Collect.Probe.Common.Test.Checkers;

namespace Modulo.Collect.Probe.Windows.Test.UserSID55
{
    [TestClass]
    public class UserSID55ItemTypeGeneratorTest
    {
        private IItemTypeGenerator DefaultUserSidItemTypeGenerator;
        private OVAL.Definitions.ObjectType UserSidObject1080;
        private OVAL.Definitions.ObjectType UserSidObject1090;
        private OVAL.Definitions.ObjectType UserSidObject1100;
        private OVAL.Definitions.ObjectType UserSidObject1101;
        private OVAL.Definitions.ObjectType UserSidObject1102;

        public UserSID55ItemTypeGeneratorTest()
        {
            this.DefaultUserSidItemTypeGenerator = new UserSID55ItemTypeGenerator(null);
            
            var objectSamples = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml").objects;
            this.UserSidObject1080 = objectSamples.Single(obj => obj.id.Equals("oval:modulo:obj:1080"));
            this.UserSidObject1090 = objectSamples.Single(obj => obj.id.Equals("oval:modulo:obj:1090"));
            this.UserSidObject1100 = objectSamples.Single(obj => obj.id.Equals("oval:modulo:obj:1100"));
            this.UserSidObject1101 = objectSamples.Single(obj => obj.id.Equals("oval:modulo:obj:1101"));
            this.UserSidObject1102 = objectSamples.Single(obj => obj.id.Equals("oval:modulo:obj:1102"));
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_for_a_simple_objectType()
        {
            var result = this.DefaultUserSidItemTypeGenerator.GetItemsToCollect(UserSidObject1080, null);

            ItemTypeChecker.DoBasicAssertForItems(result, 1, typeof(user_sid_item), StatusEnumeration.notcollected);
            ItemTypeEntityChecker.AssertItemTypeEntity(((user_sid_item)result.Single()).user_sid, "S-1-15-18", "user_sid");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_for_a_objectType_with_referenced_variable()
        {
            var fakeEvaluatedVariables = 
                VariableHelper.CreateVariableWithOneValue(UserSidObject1090.id, "oval:modulo:var:201", "S-1-1");

            var result = DefaultUserSidItemTypeGenerator.GetItemsToCollect(UserSidObject1090, fakeEvaluatedVariables);

            ItemTypeChecker.DoBasicAssertForItems(result, 1, typeof(user_sid_item), StatusEnumeration.notcollected);
            ItemTypeEntityChecker.AssertItemTypeEntity(((user_sid_item)result.Single()).user_sid, "S-1-1", "user_sid");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_for_a_objectType_with_referenced_variable_with_multiple_values()
        {
            var fakeVariableValues = new string[] { "S-1-1", "S-1-2", "S-1-10" };
            var fakeEvaluatedVariables = 
                VariableHelper.CreateVariableWithMultiplesValue(
                    UserSidObject1090.id, "oval:modulo:var:201", fakeVariableValues);

            var result = DefaultUserSidItemTypeGenerator.GetItemsToCollect(UserSidObject1090, fakeEvaluatedVariables);

            ItemTypeChecker.DoBasicAssertForItems(result, 3, typeof(user_sid_item), StatusEnumeration.notcollected);
            var userSidItemToAssert = result.OfType<user_sid_item>();
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidItemToAssert.ElementAt(0).user_sid, "S-1-1", "user_sid");
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidItemToAssert.ElementAt(1).user_sid, "S-1-2", "user_sid");
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidItemToAssert.ElementAt(2).user_sid, "S-1-10", "user_sid");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_for_a_objectType_with_PatternMatchOperation_on_UserSIDEntity()
        {
            var fakeAllUserSIDsToReturn = new string[] { "S-1-5-18", "S-1-55", "S-1-1", "S-1-1-500", "S-1-6", "S-1-1", "S-1-0" };
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(fakeAllUserSIDsToReturn);

            var result = itemTypeGenerator.GetItemsToCollect(UserSidObject1100, null);

            ItemTypeChecker.DoBasicAssertForItems(result, 2, typeof(user_sid_item));
            var userSidItemToAssert = result.OfType<user_sid_item>();
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidItemToAssert.ElementAt(0).user_sid, "S-1-5-18", "user_sid");
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidItemToAssert.ElementAt(1).user_sid, "S-1-55", "user_sid");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_for_a_ObjectType_with_PatternMatchOperation_and_ReferencedToMultiVariableValues_on_UserSIDEntity()
        {
            var fakeAllUserSIDsToReturn = 
                new string[] { "S-1-5-18", "S-1-55", "S-1-1", "S-1-1-500", "S-1-6", "S-1-1", "S-1-0" };
            var fakeVariables = 
                VariableHelper.CreateVariableWithMultiplesValue(
                    UserSidObject1101.id, 
                    "oval:modulo:var:1000", 
                    new string[] { "S-1-5.*", "S-1-1-.*" });
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(fakeAllUserSIDsToReturn);

            var result = itemTypeGenerator.GetItemsToCollect(UserSidObject1101, fakeVariables);

            ItemTypeChecker.DoBasicAssertForItems(result, 3, typeof(user_sid_item));
            var userSidItemToAssert = result.OfType<user_sid_item>();
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidItemToAssert.ElementAt(0).user_sid, "S-1-5-18", "user_sid");
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidItemToAssert.ElementAt(1).user_sid, "S-1-55", "user_sid");
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidItemToAssert.ElementAt(2).user_sid, "S-1-1-500", "user_sid");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_not_exists_items_with_pattern_match_or_equals_operation()
        {
            var fakeAllUserSIDsToReturn = new string[] { "S-1-1-500", "S-1-1", "S-1-0" };
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(fakeAllUserSIDsToReturn);

            var result = itemTypeGenerator.GetItemsToCollect(UserSidObject1100, null);

            ItemTypeChecker.DoBasicAssertForItems(result, 1, typeof(user_sid_item), StatusEnumeration.doesnotexist);
            var expectedSidValue = ((user_sid55_object)UserSidObject1100).GetUserSIDEntity().Value;
            var userSidEntity = ((user_sid_item)result.Single()).user_sid;
            ItemTypeEntityChecker.AssertItemTypeEntity(userSidEntity, expectedSidValue, "user_sid");
            Assert.AreEqual(result.Single().status, userSidEntity.status, "user_sid_entity has unexpected status.");
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_get_user_entity_by_reflection()
        {
            {
                var userSIDEntity = ((user_sid55_object)this.UserSidObject1100).GetUserSIDEntity(); 
                Assert.IsNotNull(userSIDEntity);
                Assert.AreEqual(OperationEnumeration.patternmatch, userSIDEntity.operation);
                Assert.AreEqual("S-1-5.*", userSIDEntity.Value);
            }

            
            var userEntity = ((user_sid_object)this.UserSidObject1102).GetUserEntity();
            Assert.IsNotNull(userEntity);
            Assert.AreEqual(OperationEnumeration.equals, userEntity.operation);
            Assert.AreEqual("S-1-5-21-3547645784-3438639615-1391272690-1015", userEntity.Value);
        }

        private UserSID55ItemTypeGenerator CreateMockedItemTypeGenerator(string[] fakeAllUserSIDsToReturn)
        {
            var fakeWindowsAccounts = fakeAllUserSIDsToReturn.Select(sid => new WindowsAccount("", true, sid, AccountType.User));
            
            var mocks = new MockRepository();
            var fakeAccountProvider = mocks.DynamicMock<WindowsUsersProvider>(new object[] { null, null } );
            Expect.Call(fakeAccountProvider.GetAllGroupByUsers()).Return(fakeWindowsAccounts);
            Expect.Call(fakeAccountProvider.GetUserGroups(null, AccountSearchReturnType.SID)).IgnoreArguments().Return(new string[] { "S-1-500" });
            mocks.ReplayAll();

            return new UserSID55ItemTypeGenerator(fakeAccountProvider); 
        }

        private void AssetUserItem(user_sid_item itemToAssert, string expectedUserSidValue, StatusEnumeration status)
        {
        }


    }
}
