/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Windows.AccessToken;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows.Test.AccessToken
{
    [TestClass]
    public class AccessTokenObjectCollectorTests
    {
        private const string ABSENT = "0";
        private const string EXISTENT = "1";
        private const string TEST_EXCEPTION_MESSAGE = "Test Exception was thrown";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_simple_accesstoken_object()
        {
            var fakeItemToCollect = new accesstoken_item() { security_principle = new EntityItemStringType() { Value = "mss\\lfernandes" } };
            var systemDataSource = this.CreateMockedSystemDataSource(new string[] { "SeDebugPrivilege", "SeDenyRemoteInteractiveLogonRight", "SeDenyBatchLogonRight" }.ToList(), null);

            var collectedItems = systemDataSource.CollectDataForSystemItem(fakeItemToCollect);

            this.DoBasicAssert(collectedItems, 1);
            var collectedAccessTokenItem = (accesstoken_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.exists, collectedAccessTokenItem.status, "Unexpected item type status was found.");
            this.AssertSecurityPrincipalEntity(collectedAccessTokenItem);
            this.AssertExistentAccessTokens(collectedAccessTokenItem);
            this.AssertAbsentAccessTokens(collectedAccessTokenItem);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_collected_accesstoken_item_if_an_error_occurs()
        {
            var fakeItemToCollect = new accesstoken_item() { security_principle = new EntityItemStringType() { Value = "mss\\lfernandes" } };
            var systemDataSource = this.CreateMockedSystemDataSource(new string[] { }.ToList(), new Exception(TEST_EXCEPTION_MESSAGE));


            var collectedItems = systemDataSource.CollectDataForSystemItem(fakeItemToCollect);


            this.DoBasicAssert(collectedItems, 1);
            var collectedItem = collectedItems.Single().ItemType;
            
            Assert.AreEqual(StatusEnumeration.error, collectedItem.status);
            Assert.IsNotNull(collectedItem.message);
            Assert.IsTrue(collectedItem.message.First().Value.Contains("AccessTokenObjectCollector"));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_an_accesstoken_item_with_error_status()
        {
            var fakeItemWithError = new accesstoken_item() { status = StatusEnumeration.error, security_principle = new EntityItemStringType() { Value = "lfernandes" } };
            
            var collectedItems = new AccessTokenObjectCollector().CollectDataForSystemItem(fakeItemWithError);

            Assert.AreEqual(StatusEnumeration.error, collectedItems.Single().ItemType.status);
        }


        private BaseObjectCollector CreateMockedSystemDataSource(List<String> accountNamesToReturn, Exception exceptionToThrow)
        {
            var mocks = new MockRepository();
            var fakeAccessTokenProvider = mocks.DynamicMock<AccessTokenProvider>();
            if (exceptionToThrow == null)
                Expect.Call(fakeAccessTokenProvider.GetAccessTokens(null, null)).IgnoreArguments().Return(accountNamesToReturn);
            else
                Expect.Call(fakeAccessTokenProvider.GetAccessTokens(null, null)).IgnoreArguments().Throw(exceptionToThrow);

            mocks.ReplayAll();

            return new AccessTokenObjectCollector() { AccessTokenProvider = fakeAccessTokenProvider };
        }

        private void DoBasicAssert(IEnumerable<CollectedItem> collectedItems, int expectedCount)
        {
            Assert.IsNotNull(collectedItems, "The result of item collection cannot be null");
            Assert.AreEqual(expectedCount, collectedItems.Count(), "An unexpected collected items count was found.");
            foreach (var collectedItem in collectedItems)
            {
                Assert.IsNotNull(collectedItem.ItemType, "A collected item with null ItemType was found.");
                Assert.IsInstanceOfType(collectedItem.ItemType, typeof(accesstoken_item), "An item with unexpected type was found.");
            }
        }

        private void AssertSecurityPrincipalEntity(accesstoken_item collectedAccessTokenItem)
        {
            Assert.IsNotNull(collectedAccessTokenItem.security_principle);
            Assert.AreEqual("mss\\lfernandes", collectedAccessTokenItem.security_principle.Value);
        }

        private void AssertExistentAccessTokens(accesstoken_item collectedAccessTokenItem)
        {
            this.assertAccessToken(collectedAccessTokenItem.sedebugprivilege, EXISTENT);
            this.assertAccessToken(collectedAccessTokenItem.sedenyremoteInteractivelogonright, EXISTENT);
            this.assertAccessToken(collectedAccessTokenItem.sedenybatchLogonright, EXISTENT);
        }

        private void AssertAbsentAccessTokens(accesstoken_item collectedAccessTokenItem)
        {
            this.assertAccessToken(collectedAccessTokenItem.seassignprimarytokenprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seauditprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sebackupprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sebatchlogonright, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sechangenotifyprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.secreateglobalprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.secreatepagefileprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.secreatepermanentprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.secreatesymboliclinkprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.secreatetokenprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sedenyinteractivelogonright, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sedenynetworklogonright, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sedenyservicelogonright, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seenabledelegationprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seimpersonateprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seincreasebasepriorityprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seincreasequotaprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seincreaseworkingsetprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seinteractivelogonright, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seloaddriverprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.selockmemoryprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.semachineaccountprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.semanagevolumeprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.senetworklogonright, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seprofilesingleprocessprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.serelabelprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seremoteinteractivelogonright, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seremoteshutdownprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.serestoreprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sesecurityprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seservicelogonright, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seshutdownprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sesyncagentprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sesystemenvironmentprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sesystemprofileprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.sesystemtimeprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.setakeownershipprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.setcbprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.setimezoneprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seundockprivilege, ABSENT);
            this.assertAccessToken(collectedAccessTokenItem.seunsolicitedinputprivilege, ABSENT);
        }

        private void assertAccessToken(EntityItemBoolType accessTokenEntityItem, string expectedExistence)
        {
            Assert.IsNotNull(accessTokenEntityItem, "A null access token entity item was found.");
            Assert.AreEqual(expectedExistence, accessTokenEntityItem.Value, "An access token entity item with unexpected value was found");
        }

        
    }
}
