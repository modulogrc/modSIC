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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test.File
{
    [TestClass]
    public class FileProberUnitTest
    {

        private IList<IConnectionProvider> fakeContext = ProbeHelper.CreateFakeContext();
        private TargetInfo fakeTargetInfo = ProbeHelper.CreateFakeTarget();

        [Ignore, Owner("lfernandes, lcosta")]
        public void Should_be_possible_to_execute_a_simple_file_collect()
        {
            // Arrange
            file_item fakeFileItem = this.CreateFakeFileItem("c:\\windows\\ODBC.ini", null, null);
            CollectInfo fakeCollectInfo = this.getFakeCollectInfo("definitionsWithOnlyObjects.xml");
            FileProber fileProber = this.GetMockedFileProber(fakeFileItem);
            
            // Act
            ProbeResult collectResult = fileProber.Execute(fakeContext, fakeTargetInfo, fakeCollectInfo);

            // Assert
            Assert.IsNotNull(collectResult, "The result of probe execution cannot be null.");
            Assert.IsNotNull(collectResult.CollectedObjects, "There are no collected objects.");
            Assert.AreEqual(2, collectResult.CollectedObjects.Count(), "Unexpected quantity of collected objects");
            
            CollectedObject fileObjectToAssert = collectResult.CollectedObjects.ElementAt(0);
            Assert.IsNotNull(fileObjectToAssert.ObjectType, "The first collected object cannot null.");
            Assert.AreEqual("oval:modulo:obj:9000", fileObjectToAssert.ObjectType.id, "The identificator of [collected object] and [definitions object] must be equal.");
            Assert.AreEqual(FlagEnumeration.complete, fileObjectToAssert.ObjectType.flag, "A successfully gathering must generate a collected object with flag equal to 'complete'.");
            Assert.IsNotNull(fileObjectToAssert.ObjectType.reference, "A successfully gathering must generate at least one item referenced");
            Assert.AreEqual(1, fileObjectToAssert.ObjectType.reference.Count(), "Unexpected number of generated items for this object collected");
            Assert.AreEqual(fileObjectToAssert.SystemData.Count(), fileObjectToAssert.ObjectType.reference.Count(), "The number of referenced items in collected object should be equal to generated items quantity.");
            
        }


        private FileProber GetMockedFileProber(file_item fakeItem)
        {
            IList<String> fakeValues = new List<String>(new string[] { "FakeValue" });
            CollectedItem fakeCollectedItem = ProbeHelper.CreateFakeCollectedItem(fakeItem);

            MockRepository mocks = new MockRepository();
            IConnectionManager fakeConnection = mocks.DynamicMock<IConnectionManager>();
            ISystemInformationService fakeSystemInformation = mocks.DynamicMock<ISystemInformationService>();
            FileConnectionProvider fakeProvider = mocks.DynamicMock<FileConnectionProvider>();
            WmiDataProvider fakeWmiProvider = mocks.DynamicMock<WmiDataProvider>();
            FileObjectCollector fakeDataCollector = mocks.DynamicMock<FileObjectCollector>();
            fakeDataCollector.WmiDataProvider = fakeWmiProvider;

            //Expect.Call(fakeConnection.Connect<FileConnectionProvider>(null, null)).IgnoreArguments().Repeat.Any().Return(fakeProvider);
            Expect.Call(fakeDataCollector.CollectDataForSystemItem(fakeItem)).IgnoreArguments().Repeat.Any().Return(new List<CollectedItem>() {fakeCollectedItem});
            Expect.Call(fakeDataCollector.GetValues(null)).IgnoreArguments().Repeat.Any().Return(fakeValues);
            Expect.Call(fakeSystemInformation.GetSystemInformationFrom(null)).IgnoreArguments().Return(SystemInformationFactory.GetExpectedSystemInformation());
            mocks.ReplayAll();

            return new FileProber() { ConnectionManager = fakeConnection, ObjectCollector = fakeDataCollector };
        }

        private CollectInfo getFakeCollectInfo(string definitionsFileName)
        {
            file_object[] fileObjects = ProbeHelper.GetFakeOvalDefinitions(definitionsFileName).objects.OfType<file_object>().ToArray();
            return ProbeHelper.CreateFakeCollectInfo(fileObjects, null, null);
        }

        private file_item CreateFakeFileItem(string filepath, string path, string filename)
        {
            string fullFilePath = filepath;
            string collectSuccessfully = "The File, which fullPath is '{0}', was collected sucessfully.";

            file_item newFileItem = new file_item();
            newFileItem.status = StatusEnumeration.exists;
            newFileItem.message = MessageType.FromString(string.Format(collectSuccessfully, fullFilePath));

            return newFileItem;
        }
    }
}
