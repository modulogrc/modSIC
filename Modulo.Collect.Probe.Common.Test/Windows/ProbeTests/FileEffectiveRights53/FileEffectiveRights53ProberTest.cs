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
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.FileEffectiveRights53;
using Modulo.Collect.Probe.Windows.Test.Helpers;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Windows.File;

namespace Modulo.Collect.Probe.Windows.Test.FileEffectiveRights53
{
    [TestClass]
    public class FileEffectiveRights53ProberTest: ProberTestBase
    {
        private ItemType[] FakeItemTypes;
        private CollectedItem[] FakeCollectedItems;

        public FileEffectiveRights53ProberTest()
        {
            FakeItemTypes = new ItemType[] { CreateFakeTextFileContentItem(@"c:\temp\file1.txt", null, null, "S-1-15-18") };
            FakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(FakeItemTypes.First()) };
        }

        /// <summary>
        /// It needs to mock FileEffectiveRights53Prober FileSystemDataSource 
        /// </summary>
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_FileEffectiveRights53Object()
        {
            #region 
            //<fileeffectiverights53_object id="oval:modulo:obj:1000" version="1">
            //    <path>c:\temp</path>
            //    <filename>file1.txt</filename>
            //    <trustee_sid>S-1-15-18</trustee_sid>
            //</fileeffectiverights53_object>
            #endregion

            var prober = new FileEffectiveRights53Prober() { FileSystemDataSource = new FileObjectCollector(), FileProvider = new WindowsFileProvider(FakeTargetInfo) };
            CreateBehaviorForNormalFlowExecution(prober, FakeItemTypes, FakeCollectedItems);

            var result = prober.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("1000"));

            DoAssertForSingleCollectedObject(result, typeof(fileeffectiverights_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned()
        {
            var prober = new FileEffectiveRights53Prober() { FileSystemDataSource = new FileObjectCollector(), FileProvider = new WindowsFileProvider(FakeTargetInfo) };
            CreateBehaviorWithExceptionThrowing(prober);
            
            var result = prober.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("1000"));

            DoAssertForExecutionWithErrors(result, typeof(fileeffectiverights_item));
        }

        public void CreateBehaviorForNormalFlowExecution(
            ProbeBase probeToConfigure,
            ItemType[] fakeItemsToReturnByItemTypeGenerator,
            CollectedItem[] fakeCollectedItemsToReturnByObjectCollector)
        {
            var mocksRepository = new MockRepository();
            var fakeConnectionManager = mocksRepository.DynamicMock<IConnectionManager>();
            var fakeObjectCollector = mocksRepository.DynamicMock<FileEffectiveRights53ObjectCollector>();
            var fakeItemTypeGenerator = mocksRepository.DynamicMock<IItemTypeGenerator>();

            Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null))
                 .IgnoreArguments()
                     .Return(fakeItemsToReturnByItemTypeGenerator);

            Expect.Call(fakeObjectCollector.CollectDataForSystemItem(null))
                 .IgnoreArguments()
                     .Return(fakeCollectedItemsToReturnByObjectCollector);

            mocksRepository.ReplayAll();

            probeToConfigure.ConnectionManager = fakeConnectionManager;
            probeToConfigure.ItemTypeGenerator = fakeItemTypeGenerator;
            probeToConfigure.ObjectCollector = fakeObjectCollector;
        }

        public void CreateBehaviorWithExceptionThrowing(ProbeBase probeToConfigure)
        {
            var mocksRepository = new MockRepository();
            var fakeConnectionManager = mocksRepository.DynamicMock<IConnectionManager>();
            var fakeObjectCollector = mocksRepository.DynamicMock<FileEffectiveRights53ObjectCollector>();
            var fakeItemTypeGenerator = mocksRepository.DynamicMock<IItemTypeGenerator>();

            Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null))
                .IgnoreArguments()
                .Throw(new Exception(ProberBehaviorCreator.FAKE_EXCEPTION_MESSAGE));

            Expect.Call(fakeObjectCollector.CollectDataForSystemItem(null))
                .IgnoreArguments()
                    .CallOriginalMethod(OriginalCallOptions.NoExpectation);

            mocksRepository.ReplayAll();

            probeToConfigure.ConnectionManager = fakeConnectionManager;
            probeToConfigure.ItemTypeGenerator = fakeItemTypeGenerator;
            probeToConfigure.ObjectCollector = fakeObjectCollector;
        }

        private fileeffectiverights_item CreateFakeTextFileContentItem(string filepath, string path, string filename, string trusteeSID)
        {
            return new fileeffectiverights_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(filepath),
                path = (path == null) ? null : OvalHelper.CreateItemEntityWithStringValue(path),
                filename = (filename == null) ? null : OvalHelper.CreateItemEntityWithStringValue(filename),
                trustee_sid = OvalHelper.CreateItemEntityWithStringValue(trusteeSID)
            };
        }
    }
}