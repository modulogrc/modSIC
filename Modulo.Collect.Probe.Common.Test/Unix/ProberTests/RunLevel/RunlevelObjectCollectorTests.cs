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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Unix.RunLevel;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Rhino.Mocks;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.RunLevel
{
    [TestClass]
    public class RunlevelObjectCollectorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_runlevel_item()
        {
            var fakeRunLevelInfoToReturn = CreateFakeRunlevelInfo(false);
            var runlevelObjectCollector = CreateRunlevelObjectCollector(fakeRunLevelInfoToReturn);
            var fakeItemToCollect = CreateFakeItemToCollect();

            var collectedItems = runlevelObjectCollector.CollectDataForSystemItem(fakeItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(runlevel_item), true);
            var collectedItem = (runlevel_item)collectedItems.Single().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.service_name, "ssh");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.runlevel, "2");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.start, "1");
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedItem.kill, "0");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_ServiceNotExistsException()
        {
            var fakeRunLevelInfoToReturn = CreateFakeRunlevelInfo(true);
            var runlevelObjectCollector = CreateRunlevelObjectCollector(fakeRunLevelInfoToReturn);
            var fakeItemToCollect = CreateFakeItemToCollect();

            var collectedItems = runlevelObjectCollector.CollectDataForSystemItem(fakeItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(runlevel_item), false);
            var collectedItem = (runlevel_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.status);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_NoRunLevelDataException()
        {
            var runlevelObjectCollector = CreateRunlevelObjectCollector(new Dictionary<string, RunLevelInfo>());
            var fakeItemToCollect = CreateFakeItemToCollect();

            var collectedItems = runlevelObjectCollector.CollectDataForSystemItem(fakeItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(runlevel_item), false);
            var collectedItem = (runlevel_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedItem.status);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_unexpected_exception()
        {
            var objectCollector = CreateRunlevelObjectCollectorWithExceptionThrowing();
            var fakeItemToCollect = CreateFakeItemToCollect();

            var collectedItems = objectCollector.CollectDataForSystemItem(fakeItemToCollect);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(runlevel_item), false);
            var collectedItem = (runlevel_item)collectedItems.Single().ItemType;
            ItemTypeChecker.AssertItemTypeWithErrorStatus(collectedItem, "Timeout");
        }


        private runlevel_item CreateFakeItemToCollect()
        {
            return new runlevel_item()
            {
                service_name = OvalHelper.CreateItemEntityWithStringValue("ssh"),
                runlevel = OvalHelper.CreateItemEntityWithStringValue("2")
            };
        }
            
        private Dictionary<string, RunLevelInfo> CreateFakeRunlevelInfo(bool noRunlevelInfo)
        {
            var newRunlevelInfo = 
                noRunlevelInfo ? null :
                    new RunLevelInfo() 
                    { 
                        Kill = false, 
                        Start = true, 
                        RunLevel = "2", 
                        Service = "ssh" 
                    };
            
            var fakeRunlevelInfoToReturn = new Dictionary<string, RunLevelInfo>();
            fakeRunlevelInfoToReturn.Add("ssh", newRunlevelInfo);
            return fakeRunlevelInfoToReturn;
        }

        private BaseObjectCollector CreateRunlevelObjectCollector(
            Dictionary<string, RunLevelInfo> fakeRunlevelInfoToReturn)
        {
            var mocks = new MockRepository();
            var fakeRunLevelCollector = mocks.DynamicMock<RunLevelCollector>();
            Expect.Call(fakeRunLevelCollector.GetTargetRunLevelInfo(null))
                .IgnoreArguments()
                    .Return(fakeRunlevelInfoToReturn);
            
            mocks.ReplayAll();

            return new RunLevelObjectCollector()
            {
                RunLevelsCollector = fakeRunLevelCollector
            };
        }

        private BaseObjectCollector CreateRunlevelObjectCollectorWithExceptionThrowing()
        {
            var mocks = new MockRepository();
            var fakeRunLevelCollector = mocks.DynamicMock<RunLevelCollector>();
            Expect.Call(fakeRunLevelCollector.GetTargetRunLevelInfo(null))
                .IgnoreArguments()
                    .Throw(new Exception("Timeout"));

            mocks.ReplayAll();

            return new RunLevelObjectCollector()
            {
                RunLevelsCollector = fakeRunLevelCollector
            };
        }
    }
}
