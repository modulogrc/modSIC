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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
//using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Unix.File;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Tamir.SharpSsh;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common.Test.Checkers;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.File
{
    [TestClass]
    public class FileObjectCollectorTests
    {
        private Type TypeOfUnixFileItem = typeof(OVAL.SystemCharacteristics.Unix.file_item);

        public FileObjectCollectorTests()
        {
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_file_item_for_unix_systems()
        {
            var fileCollector = new FileCollector() { LsCommand = CreateFakeLsCommand(true, false) };
            var fileItemToCollect = CreateFakeUnixFileItem(false);

            var collectedItem = new FileObjectCollector(fileCollector).CollectDataForSystemItem(fileItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItem, 1, TypeOfUnixFileItem, true);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_unix_directory()
        {
            var fileCollector = new FileCollector() { LsCommand = CreateFakeLsCommand(true, true) };
            var fileItemToCollect = CreateFakeUnixFileItem(true);

            var collectedItem = new FileObjectCollector(fileCollector).CollectDataForSystemItem(fileItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItem, 1, TypeOfUnixFileItem, true);
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_month_number_from_short_month_description()
        {
            Assert.AreEqual("01", new FileInfo().GetMonthNumberByShortMonth("JaN"));
            Assert.AreEqual("02", new FileInfo().GetMonthNumberByShortMonth("FEV"));
            Assert.AreEqual("03", new FileInfo().GetMonthNumberByShortMonth("mar"));
            Assert.AreEqual("04", new FileInfo().GetMonthNumberByShortMonth("aPr"));
            Assert.AreEqual("05", new FileInfo().GetMonthNumberByShortMonth("mAI"));
            Assert.AreEqual("06", new FileInfo().GetMonthNumberByShortMonth("JuN"));
            Assert.AreEqual("07", new FileInfo().GetMonthNumberByShortMonth("JuL"));
            Assert.AreEqual("08", new FileInfo().GetMonthNumberByShortMonth("aGO"));
            Assert.AreEqual("09", new FileInfo().GetMonthNumberByShortMonth("SEt"));
            Assert.AreEqual("10", new FileInfo().GetMonthNumberByShortMonth("Out"));
            Assert.AreEqual("11", new FileInfo().GetMonthNumberByShortMonth("noV"));
            Assert.AreEqual("12", new FileInfo().GetMonthNumberByShortMonth("Dez"));
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(Exception))]
        public void When_a_invalid_short_month_description_was_be_passed_an_exception_should_be_thrown()
        {
            new FileInfo().GetMonthNumberByShortMonth("XXX");
        }

        private LsCommand CreateFakeLsCommand(bool returnOnlyOneFile, bool forDirectory)
        {
            var mocks = new MockRepository();
            var fakeLsCommand = mocks.DynamicMock<LsCommand>(new object[] { null } );
            Expect.Call(fakeLsCommand.Run(""))
                .IgnoreArguments()
                .Return(FakeTerminalOutputFactory.GetLsCommandReturn(returnOnlyOneFile, forDirectory));

            mocks.ReplayAll();
            
            return fakeLsCommand;
        }

        private OVAL.SystemCharacteristics.Unix.file_item CreateFakeUnixFileItem(bool forDirectory)
        {
            return new OVAL.SystemCharacteristics.Unix.file_item()
            {
                path = OvalHelper.CreateItemEntityWithStringValue("/home/oval/"),
                filename = forDirectory ? null : OvalHelper.CreateItemEntityWithStringValue("results.xml")
            };
        }
           
    }
}
