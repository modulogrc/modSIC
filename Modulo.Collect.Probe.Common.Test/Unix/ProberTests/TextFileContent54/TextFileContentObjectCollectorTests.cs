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
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Rhino.Mocks;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.Common.File;
using System.Text;
using System;


namespace Modulo.Collect.Probe.Unix.Test.ProberTests.TextFileContent54
{
    [TestClass]
    public class TextFileContentObjectCollectorTests
    {
        private const string FAKE_FILEPATH = @"c:\temp\file1.txt";
        private const int ALL_FILE_LINES = 0;
        private const int FIRST_FILE_LINE = 1;
        private const int LAST_FILE_LINE = 4;
        private BaseObjectCollector ObjectCollector;

        public TextFileContentObjectCollectorTests()
        {
            this.ObjectCollector = 
                GetTextFileContentObjectCollectorWithMockBehavior(
                    new string[] { "Line 1", "Line 2", "Line 3", "Line N", "" });
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_collect_TextFileContentItem()
        {
            var collectedItems = this.ObjectCollector.CollectDataForSystemItem(GetTextFileContentItem());

            ItemTypeChecker
                .DoBasicAssertForCollectedItems(
                    collectedItems, 1, typeof(textfilecontent_item), true);
            
            var textFileContentItem = (textfilecontent_item)collectedItems.Single().ItemType;
            ItemTypeEntityChecker.AssertItemTypeEntity(
                textFileContentItem.filepath, 
                FAKE_FILEPATH);
            ItemTypeEntityChecker.AssertItemTypeEntity(
                textFileContentItem.path, 
                Path.GetDirectoryName(FAKE_FILEPATH));
            ItemTypeEntityChecker.AssertItemTypeEntity(
                textFileContentItem.filename,
                Path.GetFileName(FAKE_FILEPATH));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_all_file_lines()
        {
            var parametersForGetAllFileLines = GetSearchParametersFileLines(ALL_FILE_LINES);

            var result = this.ObjectCollector.GetValues(parametersForGetAllFileLines);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_the_first_file_line()
        {
            var parametersForGetAllFileLines = GetSearchParametersFileLines(FIRST_FILE_LINE);

            var result = this.ObjectCollector.GetValues(parametersForGetAllFileLines);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Line 1", result[0].Trim());
        }


        [TestMethod,Owner("lfernandes")]
        public void Should_be_possible_to_get_the_last_file_line()
        {
            var parametersForGetAllFileLines = GetSearchParametersFileLines(LAST_FILE_LINE);

            var result = this.ObjectCollector.GetValues(parametersForGetAllFileLines);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Line N", result[0].Trim());
        }

        [TestMethod, Owner("lfernandes")]
        public void When_the_instance_parameters_is_greater_than_number_of_file_lines_the_result_must_be_an_empty_list()
        {
            var parametersForGetAllFileLines = GetSearchParametersFileLines(5);

            var result = this.ObjectCollector.GetValues(parametersForGetAllFileLines);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod, Owner("lfernandes")]
        public void The_regex_must_be_applied_line_by_line()
        {
            var fakeFileLines = GetFakeFileContent();
            var objectCollector = GetTextFileContentObjectCollectorWithMockBehavior(fakeFileLines);
            var regex = @"^\s*net\.ipv4\.conf\.all\.accept_redirects\s*=\s*0\s*(#.*)?$";
            var parameters = GetSearchParametersFileLines(1, regex, true);

            var result = objectCollector.GetValues(parameters);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("net.ipv4.conf.all.accept_redirects = 0", result.First().Trim());
        }

        private string[] GetFakeFileContent()
        {
            var fakeFileLines = new List<String>();
            fakeFileLines.Add("# Kernel sysctl configuration file for Red Hat Linux");
            fakeFileLines.Add("#");
            fakeFileLines.Add("# For binary values, 0 is disabled, 1 is enabled.  See sysctl(8) and");
            fakeFileLines.Add("# sysctl.conf(5) for more details.");
            fakeFileLines.Add("");
            fakeFileLines.Add("# Controls IP packet forwarding");
            fakeFileLines.Add("net.ipv4.ip_forward = 0");
            fakeFileLines.Add("");
            fakeFileLines.Add("# Controls source route verification");
            fakeFileLines.Add("net.ipv4.conf.default.rp_filter = 1");
            fakeFileLines.Add("");
            fakeFileLines.Add("# Do not accept source routing");
            fakeFileLines.Add("net.ipv4.conf.default.accept_source_route = 0");
            fakeFileLines.Add("");
            fakeFileLines.Add("# Controls the System Request debugging functionality of the kernel");
            fakeFileLines.Add("kernel.sysrq = 0");
            fakeFileLines.Add("");
            fakeFileLines.Add("# Controls whether core dumps will append the PID to the core filename.");
            fakeFileLines.Add("# Useful for debugging multi-threaded applications.");
            fakeFileLines.Add("kernel.core_uses_pid = 1");
            fakeFileLines.Add("");
            fakeFileLines.Add("net.ipv4.conf.all.accept_redirects = 0");
            fakeFileLines.Add("");

            return fakeFileLines.ToArray();
        }

        private Dictionary<string, object> GetSearchParametersFileLines(int instanceLine, string regex = @"Line.*?\r\n", bool multiline = true, bool singleline = false)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(SearchTextFileContentParameters.filepath.ToString(), "/temp");
            parameters.Add(SearchTextFileContentParameters.pattern.ToString(), regex);
            parameters.Add(SearchTextFileContentParameters.instance.ToString(), instanceLine);
            parameters.Add(SearchTextFileContentParameters.multiline.ToString(), multiline);
            parameters.Add(SearchTextFileContentParameters.singleline.ToString(), singleline);
            
            return parameters;
        }


        private textfilecontent_item GetTextFileContentItem()
        {
            return new textfilecontent_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(FAKE_FILEPATH)
            };
        }

        private BaseObjectCollector GetTextFileContentObjectCollectorWithMockBehavior(string[] fakeFileLinesToBeReturn)
        {
            var mocks = new MockRepository();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(
                fakeFileProvider.GetFileLinesContentFromHost(null))
                    .IgnoreArguments()
                        .Return(fakeFileLinesToBeReturn);
            mocks.ReplayAll();

            return new TextFileContentObjectCollector()
            {
                TargetInfo = ProbeHelper.CreateFakeTarget(),
                FileContentProvider = fakeFileProvider
            };
        }


    }
}
