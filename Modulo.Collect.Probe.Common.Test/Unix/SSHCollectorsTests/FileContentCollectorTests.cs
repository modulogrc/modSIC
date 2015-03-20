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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Rhino.Mocks;

namespace Modulo.Collect.Probe.Unix.Test.SSHCollectorsTests
{
    [TestClass]
    public class FileContentCollectorTests
    {
        private const string FAKE_FILEPATH = "/tmp";
        private const string FAKE_FILENAME = "app";
        private const string FAKE_PATTERN = "title";
        private const string FAKE_FOUND_LINE_CONTENT_1 = "    title=VirtualBox Additions ";
        private const string FAKE_FOUND_LINE_CONTENT_2 = "    title=SSH ";
        private const string FAKE_FOUND_LINE_CONTENT_3 = "    title=Firefox ";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_parse_awk_to_command()
        {
            var fakeCompleteFilepath = string.Format("{0}/{1}", FAKE_FILEPATH, FAKE_FILENAME);
            var fakeSSH = CreateFakeSSHProvider();
            var sshFileContentCollector = new FileContentCollector(fakeSSH);

            var fileContentCollectionResult = 
                sshFileContentCollector
                    .GetTextFileContent(fakeCompleteFilepath, FAKE_PATTERN);

            Assert.IsNotNull(fileContentCollectionResult);
            Assert.AreEqual(3, fileContentCollectionResult.Count);
            AssertReturnedFileLine(fileContentCollectionResult.ElementAt(0), FAKE_FOUND_LINE_CONTENT_1, (uint)1);
            AssertReturnedFileLine(fileContentCollectionResult.ElementAt(1), FAKE_FOUND_LINE_CONTENT_2, (uint)2);
            AssertReturnedFileLine(fileContentCollectionResult.ElementAt(2), FAKE_FOUND_LINE_CONTENT_3, (uint)3);
        }

        private void AssertReturnedFileLine(
            TextFileContent textFileContent, 
            string expectedFileLineContent, 
            uint expectedInstance)
        {
            Assert.IsNotNull(textFileContent);
            Assert.AreEqual(FAKE_FILENAME, textFileContent.FileName);
            Assert.AreEqual(expectedInstance, textFileContent.Instance);
            Assert.AreEqual(expectedFileLineContent, textFileContent.Line);
            Assert.AreEqual(FAKE_FILEPATH, textFileContent.Path);
            Assert.AreEqual(FAKE_PATTERN, textFileContent.Pattern);
            Assert.AreEqual(FAKE_PATTERN, textFileContent.Text);
        }

        private SshCommandLineRunner CreateFakeSSHProvider()
        {
            var mocks = new MockRepository();
            var fakeAWKReturn = GetFakeAWKReturn();
            var fakeSshCommandRunner = mocks.DynamicMock<SshCommandLineRunner>();
            Expect
                .Call(fakeSshCommandRunner.ExecuteCommand("awk '/title/ {print}' </tmp/app"))
                .IgnoreArguments()
                .Return(fakeAWKReturn);
            
            mocks.ReplayAll();

            return fakeSshCommandRunner;
        }

        private string GetFakeAWKReturn()
        {
            var sbFakeCommand = new StringBuilder();
            sbFakeCommand.AppendLine(FAKE_FOUND_LINE_CONTENT_1);
            sbFakeCommand.AppendLine(FAKE_FOUND_LINE_CONTENT_2);
            sbFakeCommand.AppendLine(FAKE_FOUND_LINE_CONTENT_3);

            return sbFakeCommand.ToString();
        }
    }
}
