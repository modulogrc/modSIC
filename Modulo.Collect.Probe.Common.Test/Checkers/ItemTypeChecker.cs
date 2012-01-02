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
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Common.Test.Checkers
{
    /// <summary>
    /// It does the necessary asserts for collected items.
    /// The correct order of method calling is:
    ///     1. DoBasicAssertForCollectedItems()
    ///     2. AssertItemTypeEntity()
    /// </summary>
    public class ItemTypeChecker
    {
        private static string UNEXPECTED_ITEM_INSTANCE_TYPE_MSG = "An item with unexpected item type instance was found.";
        private static string UNEXPECTED_ITEM_STATUS_MSG = "An item with status different of exists was found.";
        private static string ITEM_WITH_UNEXPECTED_ERROR_STATUS = "The expected item status is {0}, but error status was returned with follow message: '{1}'";

        /// <summary>
        /// It verifies the nullability of the collected items that be not null. 
        /// Futhermore, this method checks the number of collected items
        /// </summary>
        /// <param name="collectedItems">The list of collected items.</param>
        /// <param name="expectedItemCount">The expected number of collected items.</param>
        public static void DoBasicAssertForCollectedItems(
            IEnumerable<CollectedItem> collectedItems, 
            int expectedItemCount, 
            Type expectedInstanceTypeOfItems, 
            bool withExistsStatus = true)
        {
            Assert.IsNotNull(collectedItems, "There is no collected items.");
            Assert.AreEqual(expectedItemCount, collectedItems.Count(), "Unexpected collected items count was found.");
            foreach(var collectedItem in collectedItems)
            {
                Assert.IsNotNull(collectedItem.ExecutionLog, "The execution log cannot be null, but an item with no log excecution was found.");
                // Assert.IsTrue(collectedItem.ExecutionLog.Count() > 1, "The execution log must has one log items at least.");
                if (withExistsStatus)
                    Assert.AreEqual(StatusEnumeration.exists, collectedItem.ItemType.status, UNEXPECTED_ITEM_STATUS_MSG);

                Assert.IsNotNull(collectedItem.ItemType, "The item type cannot be null.");
                Assert.IsInstanceOfType(collectedItem.ItemType, expectedInstanceTypeOfItems, UNEXPECTED_ITEM_INSTANCE_TYPE_MSG);
            }
        }

        public static void DoBasicAssertForItems(
            IEnumerable<ItemType> itemsToAssert, 
            int expectedItemsCount, 
            Type expectedInstanceOfItems, 
            StatusEnumeration expectedStatus = StatusEnumeration.exists)
        {
            Assert.IsNotNull(itemsToAssert, "There is no items.");
            Assert.AreEqual(expectedItemsCount, itemsToAssert.Count(), "Unexpected items count was found.");
            foreach (var item in itemsToAssert)
            {
                if (item.status != expectedStatus)
                {
                    if (item.status == StatusEnumeration.error)
                        Assert.Fail(ITEM_WITH_UNEXPECTED_ERROR_STATUS, expectedStatus, item.message.First().Value);
                    else
                        Assert.AreEqual(expectedStatus, item.status, UNEXPECTED_ITEM_STATUS_MSG);
                }

                Assert.IsInstanceOfType(item, expectedInstanceOfItems, UNEXPECTED_ITEM_INSTANCE_TYPE_MSG);
            }
        }

        public static void AssertItemTypeWithErrorStatus(ItemType itemToAssert, string expectedChunkOfMessageTypeValue)
        {
            Assert.AreEqual(StatusEnumeration.error, itemToAssert.status, "Unexpected item status was found.");
            if (!string.IsNullOrEmpty(expectedChunkOfMessageTypeValue))
            {
                Assert.IsNotNull(itemToAssert.message, "The message type was not created.");
                Assert.IsTrue(itemToAssert.message.First().Value.Contains(expectedChunkOfMessageTypeValue));
            }
        }

    }
}
