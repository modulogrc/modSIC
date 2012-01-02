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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.Providers;
using Modulo.Collect.Probe.Windows.UserSID55;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Modulo.Collect.Probe.Windows.Test.UserSID55
{
    [TestClass]
    public class UserSID55ObjectCollectorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_UserSID55Item()
        {
            var fakeItemToCollect = new user_sid_item() 
            { 
                status = StatusEnumeration.notcollected,
                user_sid = OvalHelper.CreateItemEntityWithStringValue("S-1-18") 
            };
            var fakeAccount = GetFakeWindowsAccount(true, new string[] { "S-1-5-32-500", "S-1-5-32-501" });
            var objectCollector = CreateMockedObjectCollector(fakeAccount);

            var collectedItems = objectCollector.CollectDataForSystemItem(fakeItemToCollect);

            Assert.IsNotNull(collectedItems, "The result of collect cannot be null.");
            Assert.AreEqual(1, collectedItems.Count(), "Unexpected number of collected objects was found.");
            Assert.IsNotNull(collectedItems.ElementAt(0).ExecutionLog, "The execution log for collected item cannot be null.");

            var collectedItem = (user_sid_item)collectedItems.ElementAt(0).ItemType;
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status, "Invalid Item Type Status was found.");
            Assert.AreEqual("S-1-18", collectedItem.user_sid.Value, "Unexpected item type value was found.");
            Assert.AreEqual("1", collectedItem.enabled.Value, "Unexpected item type value was found.");
            Assert.AreEqual(2, collectedItem.group_sid.Count(), "Unexpected number of group SIDs was found in collected user_sid_item.");
            Assert.AreEqual("S-1-5-32-500", collectedItem.group_sid.ElementAt(0).Value, "A unexpected group SID was found in collected user_sid_item.");
            Assert.AreEqual("S-1-5-32-501", collectedItem.group_sid.ElementAt(1).Value, "A unexpected group SID was found in collected user_sid_item.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_create_a_ItemType_with_NotExistsStatus_when_UserSIDNotFoundException_was_raised()
        {
            var fakeItemToCollect = new user_sid_item() 
            { 
                status = StatusEnumeration.notcollected,
                user_sid = OvalHelper.CreateItemEntityWithStringValue("S-1-18") 
            }
            ;
            var userSIDNotFoundEx = new UserSIDNotFoundException(fakeItemToCollect.user_sid.Value);
            var systemDataSource = CreateMockedObjectCollector(null);

            var collectedItems = systemDataSource.CollectDataForSystemItem(fakeItemToCollect);

            Assert.IsNotNull(collectedItems, "The result of collect cannot be null.");
            Assert.AreEqual(1, collectedItems.Count(), "Unexpected number of collected objects was found.");
            Assert.IsNotNull(collectedItems.ElementAt(0).ExecutionLog, "The execution log for collected item cannot be null.");
            var collectedItem = (user_sid_item)collectedItems.ElementAt(0).ItemType;
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.status, "Invalid Item Type Status was found.");
        }

        private UserSID55ObjectCollector CreateMockedObjectCollector(WindowsAccount fakeUserAccount)
        {
            var fakeWindowsAccounts = new List<WindowsAccount>();
            if (fakeUserAccount != null)
                fakeWindowsAccounts.Add(fakeUserAccount);

            var mocks = new MockRepository();
            var fakeUserProvider = mocks.DynamicMock<WindowsUsersProvider>(null, null);
            Expect.Call(fakeUserProvider.GetAllGroupByUsers()).Return(fakeWindowsAccounts);
            mocks.ReplayAll();

            return new UserSID55ObjectCollector(fakeUserProvider);
        }

        private WindowsAccount GetFakeWindowsAccount(bool userEnabled, string[] fakeUserGroups)
        {
            var fakeUserAccount = new WindowsAccount("Fake User", (bool?)userEnabled, "S-1-18", AccountType.User);
            foreach (var fakeGroup in fakeUserGroups)
                fakeUserAccount.AddMember(string.Format("Fake Group {0}", fakeGroup), (bool?)true, fakeGroup);
            
            return fakeUserAccount;
        }
    }
}
