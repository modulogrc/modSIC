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
using Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Rhino.Mocks;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Microsoft.Win32;


namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.RegKeyEffectiveRights
{
    [TestClass]
    public class RegKeyEffectiveRightsObjectCollectorTests
    {
        private const string UNEXPECTED_REGISTRY_KEY_PERMISSION = "An unexpected registry key permission was found: '{0}'";
        private const int ONLY_ONE_COLLECTED_ITEMS_COUNT = 1;
        private const uint KEY_CREATE_LINK_PERMISSION = 32;
        private const string OVAL_FALSE_STRING = "0";
        private const string OVAL_TRUE_STRING = "1";
        


        [TestMethod, Ignore, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_regkeyeffectiverights_item()
        {
            var fakeItemToCollect = CreateFakeRegKeyEffectiveRightsItem();
            var objectCollector = CreateObjectCollectorWithBehavior();

            var collectedItems = objectCollector.CollectDataForSystemItem(fakeItemToCollect).ToArray();

            ItemTypeChecker
                .DoBasicAssertForCollectedItems(
                    collectedItems, ONLY_ONE_COLLECTED_ITEMS_COUNT, typeof(regkeyeffectiverights_item), true);

            var collectedItem = (regkeyeffectiverights_item)collectedItems.Single().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.key_create_link, OVAL_TRUE_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.key_create_sub_key, OVAL_FALSE_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.key_enumerate_sub_keys, OVAL_FALSE_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.key_notify, OVAL_FALSE_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.key_query_value, OVAL_FALSE_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.key_set_value, OVAL_FALSE_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.key_wow64_32key, OVAL_FALSE_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.key_wow64_64key, OVAL_FALSE_STRING);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_disassembly_windows_security_descriptor()
        {
            var winACLDisassembler = new WindowsSecurityDescriptorDisassembler(SecurityDescriptorType.DACL);

            var winACE = winACLDisassembler.GetSecurityDescriptorFromAccessMask(KEY_CREATE_LINK_PERMISSION);

            Assert.IsTrue(winACE.KEY_CREATE_LINK, string.Format(UNEXPECTED_REGISTRY_KEY_PERMISSION, "KEY_CREATE_LINK"));
            Assert.IsFalse(winACE.KEY_CREATE_SUB_KEY, string.Format(UNEXPECTED_REGISTRY_KEY_PERMISSION, "KEY_CREATE_SUB_KEY"));
            Assert.IsFalse(winACE.KEY_ENUMERATE_SUB_KEYS, string.Format(UNEXPECTED_REGISTRY_KEY_PERMISSION, "KEY_ENUMERATE_SUB_KEYS"));
            Assert.IsFalse(winACE.KEY_NOTIFY, string.Format(UNEXPECTED_REGISTRY_KEY_PERMISSION, "KEY_NOTIFY"));
            Assert.IsFalse(winACE.KEY_QUERY_VALUE, string.Format(UNEXPECTED_REGISTRY_KEY_PERMISSION, "KEY_QUERY_VALUE"));
            Assert.IsFalse(winACE.KEY_SET_VALUE, string.Format(UNEXPECTED_REGISTRY_KEY_PERMISSION, "KEY_SET_VALUE"));
            Assert.IsFalse(winACE.KEY_WOW64_32KEY, string.Format(UNEXPECTED_REGISTRY_KEY_PERMISSION, "KEY_WOW64_32KEY"));
            Assert.IsFalse(winACE.KEY_WOW64_64KEY, string.Format(UNEXPECTED_REGISTRY_KEY_PERMISSION, "KEY_WOW64_64KEY"));
        }

        private static regkeyeffectiverights_item CreateFakeRegKeyEffectiveRightsItem()
        {
            return new regkeyeffectiverights_item()
            {
                hive = new EntityItemRegistryHiveType() { Value = "HKEY_LOCAL_MACHINE" },
                key = OvalHelper.CreateItemEntityWithStringValue("Software\\Modulo"),
                trustee_sid = OvalHelper.CreateItemEntityWithStringValue("S-1-5-32-544")
            };
        }

        private RegKeyEffectiveRightsObjectCollector CreateObjectCollectorWithBehavior()
        {
            MockRepository mocks = new MockRepository();
            var fakeWmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            var fakeACLProvider = AccessControlListProvider.CreateInstance();
            
            Expect.Call(fakeACLProvider.GetRegistryKeyEffectiveRights(null, 0, null, null))
                .IgnoreArguments()
                    .Return(KEY_CREATE_LINK_PERMISSION);

            mocks.ReplayAll();

            return new RegKeyEffectiveRightsObjectCollector()
            {
                TargetInfo = ProbeHelper.CreateFakeTarget(),
                WmiDataProvider = fakeWmiDataProvider,
                AccessControlListProvider = fakeACLProvider
            };
        }
    }
}
