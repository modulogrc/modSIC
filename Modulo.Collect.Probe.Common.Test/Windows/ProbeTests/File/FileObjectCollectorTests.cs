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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass()]
    public class FileObjectCollectorTests
    {
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private const string FAKE_DIR_PATH = "c:\\temp\\directory";
        private const string FAKE_FILE_PATH = "c:\\temp\\file1.txt";
        private const string FILE_TYPE_DIRECTORY = "Directory";
        private const string FILE_TYPE_TEXTFILE = "Text Document";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_files_from_given_directory_path_using_FileSystemDataSource()
        {
            // Arrange
            var fakeTarget = ProbeHelper.CreateFakeTarget();
            var fakeWmiDataProvider = this.GetMockedWmiDataProvider();
            var fileDataSource = new WindowsFileProvider(fakeTarget) { WmiDataProvider = fakeWmiDataProvider };
            // Act
            var returnedFiles = fileDataSource.GetFileChildren("c:\\temp").ToList();
            // Assert
            this.AssertReturnedFiles(returnedFiles);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_fileItem_without_filename_information()
        {
            var fileItem =
                new file_item()
                {
                    path = OvalHelper.CreateItemEntityWithStringValue("c:\\windows"),
                    filename = OvalHelper.CreateItemEntityWithStringValue(string.Empty)
                };
            var fileObjectCollector = 
                new FileObjectCollector() 
                { 
                    WmiDataProvider = GetMockedWmiDataProvider() 
                };

            var collectedObjects = fileObjectCollector.CollectDataForSystemItem(fileItem);


        }

        private void AssertReturnedFiles(IList<string> files)
        {
            Assert.AreEqual(2, files.Count, "The returned file count is unexpected.");
            Assert.AreEqual(FAKE_DIR_PATH, files[0], "The fake directory was unexpected.");
            Assert.AreEqual(FAKE_FILE_PATH, files[1], "The fake file path was unexpected.");
        }


        private WmiDataProvider GetMockedWmiDataProvider()
        {
            var fakeWmiObjects = new WmiObject[]
            {   this.CreateFakeWmiObject(FAKE_DIR_PATH, FILE_TYPE_DIRECTORY, -1),
                this.CreateFakeWmiObject(FAKE_FILE_PATH, FILE_TYPE_TEXTFILE, 50)
            };

            var mocks = new MockRepository();
            var fakeWmiProvider = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(fakeWmiProvider.SearchWmiObjects(null, null)).IgnoreArguments().Return(fakeWmiObjects);
            mocks.ReplayAll();

            return fakeWmiProvider;
        }

        private WmiObject CreateFakeWmiObject(string completeFileName, string fileType, int fileSize)
        {
            var fakeWmiObject = new WmiObject();
            fakeWmiObject.Add("Name", completeFileName);
            fakeWmiObject.Add(GeneratedFileItemAttributes.FileSize.ToString(), fileSize);
            fakeWmiObject.Add(GeneratedFileItemAttributes.CreationDate.ToString(), "20081217165939.013961 - 120");
            fakeWmiObject.Add(GeneratedFileItemAttributes.LastModified.ToString(), "20090116171121.673899 - 120");
            fakeWmiObject.Add(GeneratedFileItemAttributes.LastAccessed.ToString(), "20081217171525.895263 - 120");
            fakeWmiObject.Add(GeneratedFileItemAttributes.Version.ToString(), "1");
            fakeWmiObject.Add(GeneratedFileItemAttributes.FileType.ToString(), fileType);
            fakeWmiObject.Add(GeneratedFileItemAttributes.Manufacturer.ToString(), "Modulo");
            return fakeWmiObject;
        }
    }
}
