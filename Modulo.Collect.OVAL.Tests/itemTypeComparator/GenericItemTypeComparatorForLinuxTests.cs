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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.comparators;
using Linux = Modulo.Collect.OVAL.SystemCharacteristics.Linux;


namespace Modulo.Collect.OVAL.Tests.itemTypeComparator
{
    [TestClass]
    public class GenericItemTypeComparatorForLinuxTests
    {
        private const int FIRST_POSITION = 0;
        private const int SECOND_POSITION = 1;

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_compare_two_rpminfo_items_when_they_were_created_manually()
        {
            var differentRunlevelItems = CreateDifferentRpmInfoItems();
            var equalRunLevelItems = CreateEqualRpmInfoItems();

            Assert.IsFalse(
                new GenericItemTypeComparator()
                    .IsEquals(
                        differentRunlevelItems.ElementAt(FIRST_POSITION),
                        differentRunlevelItems.ElementAt(SECOND_POSITION)));

            Assert.IsTrue(
                new GenericItemTypeComparator()
                    .IsEquals(
                        equalRunLevelItems.ElementAt(FIRST_POSITION),
                        equalRunLevelItems.ElementAt(SECOND_POSITION)));
        }

        private Linux.rpminfo_item[] CreateEqualRpmInfoItems()
        {
            return new Linux.rpminfo_item[]
            {
                new Linux.rpminfo_item() 
                { 
                    name = EntityItemStringType("vbox"),
                    arch = EntityItemStringType("x86"),
                    epoch = new Linux.rpminfo_itemEpoch() { Value = "2001"},
                    evr = EntityItemEvrType("102050"),
                    release = new Linux.rpminfo_itemRelease() { Value = "1"},
                    signature_keyid = EntityItemStringType("5"),
                    version = new Linux.rpminfo_itemVersion() { Value = "2119"}
                },
                
                new Linux.rpminfo_item() 
                { 
                    name = EntityItemStringType("vbox"),
                    arch = EntityItemStringType("x86"),
                    epoch = new Linux.rpminfo_itemEpoch() { Value = "2001"},
                    evr = EntityItemEvrType("102050"),
                    release = new Linux.rpminfo_itemRelease() { Value = "1"},
                    signature_keyid = EntityItemStringType("5"),
                    version= new Linux.rpminfo_itemVersion() { Value = "2119"}
                }
            };
        }

        private Linux.rpminfo_item[] CreateDifferentRpmInfoItems()
        {
            return new Linux.rpminfo_item[]
            {
                new Linux.rpminfo_item() { name = EntityItemStringType("vbox") },
                new Linux.rpminfo_item() { name = EntityItemStringType("firefox") }
            };
        }

        private EntityItemStringType EntityItemStringType(string value)
        {
            return new EntityItemStringType() { Value = value };
        }

        private EntityItemBoolType EntityItemBoolType(string value)
        {
            return new EntityItemBoolType() { Value = value };
        }

        private EntityItemIntType EntityItemIntType(string value)
        {
            return new EntityItemIntType() { Value = value };
        }
        private EntityItemEVRStringType EntityItemEvrType(string value)
        {
            return new EntityItemEVRStringType() { Value = value };
        }
    }
}
