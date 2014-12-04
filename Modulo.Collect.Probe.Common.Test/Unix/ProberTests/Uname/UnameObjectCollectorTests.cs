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
using Modulo.Collect.Probe.Unix.Probes.Uname;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Rhino.Mocks;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.Uname
{
    [TestClass]
    public class UnameObjectCollectorTests
    {
        [TestMethod, Owner("CollectorTeam")]
        public void Should_be_possible_to_collect_a_uname_item()
        {
            var objectCollector = new UnameObjectCollector() { UnameCollector = GetMockedUnameCollector() };
  
            var collectedItems = objectCollector.CollectDataForSystemItem(new uname_item());

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(uname_item), true);
            var collectedUnameItem = (uname_item)collectedItems.Single().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedUnameItem.os_name, "Linux");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedUnameItem.machine_class, "x86_64");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedUnameItem.node_name, "Fedora13VM");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedUnameItem.os_release, "2.6.34.7-56.fc13.x86_64");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedUnameItem.os_version, "#1 SMP Wed Sep 15 03:36:55 UTC 2010");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedUnameItem.processor_type, "x86_64");

        }
        

        private UnameCollector GetMockedUnameCollector()
        {
            MockRepository mocks = new MockRepository();
            var fakeUnameCollector = mocks.DynamicMock<UnameCollector>();
            Expect.Call(fakeUnameCollector.RunUnameCommand())
                .Return(FakeTerminalOutputFactory.GetUnameReturn());
            mocks.ReplayAll();
            return fakeUnameCollector;
        }

    }
}
