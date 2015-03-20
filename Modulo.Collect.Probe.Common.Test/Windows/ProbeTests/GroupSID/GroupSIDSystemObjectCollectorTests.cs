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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.GroupSID;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;

namespace Modulo.Collect.Probe.Windows.Test.GroupSID
{
    [TestClass]
    public class GroupSIDObjectCollectorTests
    {
        private TargetInfo FakeTargetInfo;

        public GroupSIDObjectCollectorTests()
        {
            this.FakeTargetInfo = ProbeHelper.CreateFakeTarget();
        }
            
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_simple_groupSID_object()
        {
            var fakeItemToCollect = new group_sid_item() { group_sid = OvalHelper.CreateItemEntityWithStringValue("S-1-32-500") };
            var fakeGroupAccount = new WindowsGroupAccount() { UserSIDs = new string[] { "S-1-0", "S-1-1" }.ToList() };
            GroupSIDObjectCollector systemDataSource = this.createMockedGroupSIDSystemDataSource(fakeGroupAccount);

            var result = systemDataSource.CollectDataForSystemItem(fakeItemToCollect);

            Assert.IsNotNull(result, "The result of collect cannot be null.");
            Assert.AreEqual(1, result.Count(), "Unexpected number of collected objects was found.");
            Assert.IsNotNull(result.ElementAt(0).ExecutionLog, "The execution log for collected item cannot be null.");

            group_sid_item collectedItem = (group_sid_item)result.ElementAt(0).ItemType;
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status, "Invalid Item Type Status was found.");
            Assert.AreEqual("S-1-32-500", collectedItem.group_sid.Value, "Unexpected item type value was found.");
            Assert.AreEqual(2, collectedItem.user_sid.Count(), "Unexpected number of user SIDs was found in collected group_sid_item.");
            Assert.AreEqual("S-1-0", collectedItem.user_sid.ElementAt(0).Value, "A unexpected group SID was found in collected group_sid_item.");
            Assert.AreEqual("S-1-1", collectedItem.user_sid.ElementAt(1).Value, "A unexpected group SID was found in collected group_sid_item.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_create_a_ItemType_with_NotExistsStatus_when_GroupSIDNotFoundException_was_raised()
        {
            var groupSIDToCollect = new group_sid_item() { group_sid = OvalHelper.CreateItemEntityWithStringValue("S-1-32") };
            var groupNotFoundEx = new GroupSIDNotFoundException(groupSIDToCollect.group_sid.Value);
            var mockedGroupSystemDataSource = this.createMockedGroupSIDSystemDataSourceWithException(groupNotFoundEx);

            var collectedItems = mockedGroupSystemDataSource.CollectDataForSystemItem(groupSIDToCollect);

            Assert.IsNotNull(collectedItems, "The result of collect cannot be null.");
            Assert.AreEqual(1, collectedItems.Count(), "Unexpected number of collected objects was found.");
            Assert.IsNotNull(collectedItems.ElementAt(0).ExecutionLog, "The execution log for collected item cannot be null.");
            group_sid_item collectedItem = (group_sid_item)collectedItems.ElementAt(0).ItemType;
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.status, "Invalid Item Type Status was found.");
        }


        private GroupSIDObjectCollector createMockedGroupSIDSystemDataSource(WindowsGroupAccount fakeWindowsGroupAccountToReturn)
        {
            MockRepository mocks = new MockRepository();
            var fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            var fakeGroupProvider = mocks.DynamicMock<WindowsGroupAccountProvider>(null, null);
            Expect.Call(fakeGroupProvider.CollectWindowsGroupAccountInfoBySID(null)).IgnoreArguments().Return(fakeWindowsGroupAccountToReturn);
            mocks.ReplayAll();

            return new GroupSIDObjectCollector() 
            { 
                TargetInfo = FakeTargetInfo,
                WmiDataProvider = fakeWmiDataProvider, 
                WindowsGroupProvider = fakeGroupProvider 
            };
        }

        private GroupSIDObjectCollector createMockedGroupSIDSystemDataSourceWithException(Exception groupNotFoundException)
        {
            MockRepository mocks = new MockRepository();
            var fakeGroupProvider = mocks.DynamicMock<WindowsGroupAccountProvider>(null, null);
            var fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeGroupProvider.CollectWindowsGroupAccountInfoBySID(null)).IgnoreArguments().Throw(groupNotFoundException);
            mocks.ReplayAll();

            return new GroupSIDObjectCollector() 
            { 
                TargetInfo = FakeTargetInfo,
                WindowsGroupProvider = fakeGroupProvider, 
                WmiDataProvider = fakeWmiDataProvider 
            };

        }

    }
}
