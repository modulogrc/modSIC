/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.CiscoIOS.Probes.Line;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Rhino.Mocks;
using Modulo.Collect.OVAL.SystemCharacteristics.Ios;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.CiscoIOS.Tests.ProbeTests.Line
{
    [TestClass]
    public class LineObjectCollectorTests
    {
        private const string FAKE_PASSWORD = "123456";
        private const string FAKE_ENABLE_PASSWORD = "12345";
        private const string FAKE_RETURN = "example show running";
        private const string FAKE_LINE_COMMAND = "show running";

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_collect_a_line_item()
        {
            var objectCollector = new LineObjectCollector(CreateTelnetConnectionMock());
            var fakeLineItem = new line_item() { show_subcommand = OvalHelper.CreateItemEntityWithStringValue(FAKE_LINE_COMMAND) };
            
            var collectedItems = objectCollector.CollectDataForSystemItem(fakeLineItem);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems.ToArray(), 1, typeof(line_item));
            var collectedLineItem = (line_item)collectedItems.Single().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedLineItem.config_line, FAKE_RETURN, "config_line");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_empty_command_output()
        {
            var objectCollector = new LineObjectCollector(CreateTelnetConnectionMock(true));
            var fakeLineItem = new line_item() { show_subcommand = OvalHelper.CreateItemEntityWithStringValue(FAKE_LINE_COMMAND) };

            var collectedItems = objectCollector.CollectDataForSystemItem(fakeLineItem);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems.ToArray(), 1, typeof(line_item), false);
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItems.Single().ItemType.status, "The expected item status is 'does not exist'");


        }

        private TelnetConnection CreateTelnetConnectionMock(bool returnEmptyCommand = false)
        {
            var mocks = new MockRepository();
            var fakeTelnetConnection = mocks.DynamicMock<TelnetConnection>();

            if (returnEmptyCommand)
                Expect.Call(fakeTelnetConnection.CiscoCommand(FAKE_LINE_COMMAND)).Throw(new NoCommandOutputException());
            else
                Expect.Call(fakeTelnetConnection.CiscoCommand(FAKE_LINE_COMMAND)).Return(FAKE_RETURN);

            mocks.ReplayAll();

            return fakeTelnetConnection;
        }
    }
}
