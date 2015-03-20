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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Definitions = Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.Probe.Windows.Test.Helpers
{
    public class WindowsTestHelper
    {
        public static Definitions.ObjectType GetObjectFromDefinitions(string definitionsFileName, string componentID)
        {
            oval_definitions fakeDefinitions = new LoadOvalDocument().GetFakeOvalDefinitions(definitionsFileName);
            return fakeDefinitions.objects.Where(obj => obj.id.Equals(componentID)).Single();
        }

        public static void AssertItemWithErrorStatus(IEnumerable<ItemType> itemsType, Type expectedType, string expectedMessageChunk)
        {
            Assert.IsNotNull(itemsType, "The return of GetItemsToCollect cannot be null.");
            Assert.AreEqual(1, itemsType.Count(), "Unexpected quantity of generated items.");

            var systemItem = itemsType.Single();
            Assert.IsInstanceOfType(systemItem, expectedType, "Unxpected generated item type was found.");
            Assert.AreEqual(StatusEnumeration.error, systemItem.status, "An unexpected item status was found.");
            Assert.IsNotNull(systemItem.message, "The Entity Item Message Type cannot be null");
            Assert.IsTrue(systemItem.message.First().Value.Contains(expectedMessageChunk), string.Format("The exception message cannot be found in entity item message. Found message: '{0}'", systemItem.message.First().Value));
        }

        public static void AssertItemsWithExistsStatus(ItemType[] itemsToAssert, int expectedItemCount, Type expectedTypeInstanceOfItems)
        {
            Assert.AreEqual(expectedItemCount, itemsToAssert.Count(), "Unexpected system data items count was found.");
            foreach (var itemType in itemsToAssert) 
            {
                Assert.IsInstanceOfType(itemType, expectedTypeInstanceOfItems, "An item with unexpected instance type was found.");
                Assert.AreEqual(StatusEnumeration.exists, itemType.status, "An item with unexpected status was found.");
            }
        }

        public static BaseObjectCollector GetDataSourceFakewithoutRegex()
        {
            return new SystemDataSourceFactory().GetDataSourceFakewithoutRegex();
        }

        public static BaseObjectCollector GetDataSourceFakeWithRegex(string startKey, int mockGetValuesCount)
        {
            return new SystemDataSourceFactory().GetDataSourceFakeWithRegex(startKey, mockGetValuesCount);
        }
    }
}
