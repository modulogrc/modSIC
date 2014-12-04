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
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Independent.XmlFileContent;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Common.Test
{
    public class GenericXmlFileContentProberBehaviorCreator<ConnectionProviderType> : ProberBehaviorCreator
        where ConnectionProviderType : IConnectionProvider, new()
    {
        public override void CreateBehaviorForNormalFlowExecution(
            ProbeBase probeToConfigure, 
            ItemType[] fakeItemsToReturnByItemTypeGenerator, 
            CollectedItem[] fakeCollectedItemsToReturnByObjectCollector
            )
        {
            base.CreateBehaviorForNormalFlowExecution(
                probeToConfigure, 
                fakeItemsToReturnByItemTypeGenerator, 
                fakeCollectedItemsToReturnByObjectCollector
            );

            var probe = (GenericXmlFileContentProber<ConnectionProviderType>)probeToConfigure;
            
            probe.CustomFileProvider = mocksRepository.DynamicMock<IFileProvider>();

            mocksRepository.ReplayAll();
        }
    }

    public class ProberBehaviorCreator
    {
        public ProberBehaviorCreator()
        {
            mocksRepository = new MockRepository();
        }

        protected MockRepository mocksRepository { get; set; }

        public const string FAKE_EXCEPTION_MESSAGE = "Fake Exception was thrown.";

        public virtual void CreateBehaviorForNormalFlowExecution(
            ProbeBase probeToConfigure,
            ItemType[] fakeItemsToReturnByItemTypeGenerator,
            CollectedItem[] fakeCollectedItemsToReturnByObjectCollector)
        {
            var fakeConnectionManager = mocksRepository.DynamicMock<IConnectionManager>();
            var fakeObjectCollector = mocksRepository.DynamicMock<BaseObjectCollector>();
            var fakeItemTypeGenerator = mocksRepository.DynamicMock<IItemTypeGenerator>();

            CreateExpectationForItemTypeGenerator(
                fakeItemTypeGenerator, 
                fakeItemsToReturnByItemTypeGenerator);

            CreateExpectationForObjectCollector(
                fakeObjectCollector, 
                fakeCollectedItemsToReturnByObjectCollector);

            mocksRepository.ReplayAll();

            ConfigureProber(
                probeToConfigure,
                fakeConnectionManager,
                fakeItemTypeGenerator,
                fakeObjectCollector);
        }

        public void CreateBehaviorWithExceptionThrowing(ProbeBase probeToConfigure)
        {
            var fakeConnectionManager = mocksRepository.DynamicMock<IConnectionManager>();
            var fakeObjectCollector = mocksRepository.DynamicMock<BaseObjectCollector>();
            var fakeItemTypeGenerator = mocksRepository.DynamicMock<IItemTypeGenerator>();

            CreateItemTypeGeneratorWithExceptionThrowingOnGetItemsToCollect(
                fakeItemTypeGenerator,
                new Exception(FAKE_EXCEPTION_MESSAGE));

            ConfigureObjectCollectorInOrderToKeepTheNormalFlow(
                fakeObjectCollector);

            mocksRepository.ReplayAll();

            ConfigureProber(
                probeToConfigure,
                fakeConnectionManager,
                fakeItemTypeGenerator,
                fakeObjectCollector);
        }

        private void CreateExpectationForItemTypeGenerator(
            IItemTypeGenerator itemTypeGenerator,
            ItemType[] fakeItemsToReturn)
        {
            Expect.Call(itemTypeGenerator.GetItemsToCollect(null, null))
                .IgnoreArguments()
                    .Return(fakeItemsToReturn);
        }

        private void CreateExpectationForObjectCollector(
            BaseObjectCollector objectCollector,
            CollectedItem[] fakeCollectedItemsToReturn)
        {
            Expect.Call(objectCollector.CollectDataForSystemItem(null))
                .IgnoreArguments()
                    .Return(fakeCollectedItemsToReturn);
        }

        private void CreateItemTypeGeneratorWithExceptionThrowingOnGetItemsToCollect(
            IItemTypeGenerator itemTypeGenerator,
            Exception fakeExceptionThrow)
        {
            Expect.Call(itemTypeGenerator.GetItemsToCollect(null, null))
                .IgnoreArguments()
                    .Throw(fakeExceptionThrow);
        }

        private void ConfigureObjectCollectorInOrderToKeepTheNormalFlow(
            BaseObjectCollector fakeObjectCollector)
        {
            Expect.Call(fakeObjectCollector.CollectDataForSystemItem(null))
                .IgnoreArguments()
                    .CallOriginalMethod(OriginalCallOptions.NoExpectation);
        }

        private void ConfigureProber(
            ProbeBase probeToConfigure,
            IConnectionManager fakeConnectionManager,
            IItemTypeGenerator fakeItemTypeGenerator,
            BaseObjectCollector fakeObjectCollector)
        {
            probeToConfigure.ConnectionManager = fakeConnectionManager;
            probeToConfigure.ItemTypeGenerator = fakeItemTypeGenerator;
            probeToConfigure.ObjectCollector = fakeObjectCollector;
        }
    }
}
