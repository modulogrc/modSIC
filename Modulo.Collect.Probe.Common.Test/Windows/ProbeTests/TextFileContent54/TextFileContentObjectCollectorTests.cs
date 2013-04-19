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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.TextFileContent54;
using Rhino.Mocks;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Independent.Common.File;
using System.Text.RegularExpressions;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.TextFileContent54
{

    [TestClass]
    public class TextFileContentObjectCollectorTests
    {

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_textfilecontent_instance_as_specified_by_OVAL()
        {
            var parameters = 
                TextFileContentObjectCollector
                    .GetDictionaryWithParametersToSearchTextFileConten("c:\\file.ini", "3gp.*?\r\n", 1);

            var result = GetObjectCollectorWithBehavior().GetValues(parameters);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("3gp=MPEGVideo", result.Single().Trim());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_textfilecontent_with_notequal_operation_on_instance_entity()
        {

            var parameters =
                TextFileContentObjectCollector
                    .GetDictionaryWithParametersToSearchTextFileConten("c:\\file.ini", "3gp", 3);

            var result = GetObjectCollectorWithBehavior().GetValues(parameters);

        
        }

        [TestMethod, Owner("lfernandes")]
        public void Checking_if_is_possible_to_split_stringbuilder_content()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Line 1");
            stringBuilder.AppendLine("Line 2");
            stringBuilder.AppendLine("Line 3");
            stringBuilder.Append("Line 4");

            var splittedString = 
                stringBuilder.ToString()
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.AreEqual(4, splittedString.Count());
        }

        [TestMethod, Owner("lfalcao")]
        public void Should_be_possible_to_get_one_line_with_pattern_match()
        {
            var fileContentSearchingParameters =
                TextFileContentObjectCollector.GetDictionaryWithParametersToSearchTextFileConten("c:\\windows\\win.ini", @"CMCDL.*?\r\n", 1);
            var textFileContentObjectCollector =
                new TextFileContentObjectCollector() { FileContentProvider = CreateFakeFileContentProvider(CreateFakeFileContent()) };

            var result = textFileContentObjectCollector.GetValues(fileContentSearchingParameters);

            Assert.IsNotNull(result);
            Assert.AreEqual(1,result.Count);
            Assert.AreEqual("CMCDLLNAME32=mapi32.dll",result[0].Trim());
        }

        [TestMethod, Owner("lfalcao")]
        public void Should_be_possible_to_get_more_than_one_line_with_pattern_match()
        {
            var fileContentSearchingParameters =
                TextFileContentObjectCollector.GetDictionaryWithParametersToSearchTextFileConten("c:\\windows\\win.ini", @"MAPI.*CMC=1", 1, true, true);
            var textFileContentObjectCollector =
                new TextFileContentObjectCollector() { FileContentProvider = CreateFakeFileContentProvider(CreateFakeFileContent()) };

            var result = textFileContentObjectCollector.GetValues(fileContentSearchingParameters);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("MAPI=1\r\nCMCDLLNAME32=mapi32.dll\r\nCMC=1", result[0].Trim());
        }

        [TestMethod, Owner("lfalcao")]
        public void Should_be_possible_to_get_the_second_instance_of_pattern_match()
        {
            var fileContentSearchingParameters =
                TextFileContentObjectCollector.GetDictionaryWithParametersToSearchTextFileConten("c:\\windows\\win.ini", @"<title>.*?</title>", 2);
            var textFileContentObjectCollector =
                new TextFileContentObjectCollector() { FileContentProvider = CreateFakeFileContentProvider(GetFakeLines()) };

            var result = textFileContentObjectCollector.GetValues(fileContentSearchingParameters);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(@"<title>Bible</title>", result[0].Trim());
        }

        [TestMethod, Owner("lfalcao")]
        public void Should_be_possible_to_get_xml_entries_with_pattern_match()
        {
            var fileContentSearchingParameters =
                  TextFileContentObjectCollector.GetDictionaryWithParametersToSearchTextFileConten("c:\\windows\\win.ini", @"<book>.*?</book>", 1, true, true);
            var textFileContentObjectCollector =
                new TextFileContentObjectCollector() { FileContentProvider = CreateFakeFileContentProvider(GetFakeLines()) };

            var result = textFileContentObjectCollector.GetValues(fileContentSearchingParameters);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].Contains("<title>The Lord of the Rings</title>"));
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_get_textfilecontent_entries_with_pattern_match()
        {            
            string regex = @"\s*net\.ipv4\.conf\.all\.accept_redirects\s*=\s*0\s*(#.*)?";
            var fileContentSearchingParameters =
                TextFileContentObjectCollector.GetDictionaryWithParametersToSearchTextFileConten
                ("c:\\temp\\filecontent_base.xml", regex, 1);
            var textFileContentObjectCollector =
                new TextFileContentObjectCollector() { FileContentProvider = CreateFakeFileContentProvider(GetFakeLinesInFileContent()) };

            var result = textFileContentObjectCollector.GetValues(fileContentSearchingParameters);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].Contains("net.ipv4.conf.all.accept_redirects = 0"));
        }

        private IFileProvider CreateFakeFileContentProvider(string[] fakeLinesToReturn)
        {
            MockRepository mocks = new MockRepository();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(fakeFileProvider.GetFileLinesContentFromHost(null))
                .IgnoreArguments()
                    .Return(fakeLinesToReturn);
            mocks.ReplayAll();

            return fakeFileProvider;

        }

        private TextFileContentObjectCollector GetObjectCollectorWithBehavior()
        {
            MockRepository mocks = new MockRepository();
            var fakeFileContentProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(fakeFileContentProvider.GetFileLinesContentFromHost(null))
                .IgnoreArguments()
                    .Return(this.CreateFakeFileContent());
            
            mocks.ReplayAll();

            return new TextFileContentObjectCollector()
            {
                FileContentProvider = fakeFileContentProvider,
                TargetInfo = ProbeHelper.CreateFakeTarget()
            };
        }

        private string[] CreateFakeFileContent()
        {
            var lineSeparator = new string[] { Environment.NewLine };
            return GetWinIniFakeLines().Split(lineSeparator, StringSplitOptions.None);
        }

        private string GetWinIniFakeLines()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("; for 16-bit app support");
            stringBuilder.AppendLine("[fonts]");
            stringBuilder.AppendLine("[extensions]");
            stringBuilder.AppendLine("[mci extensions]");
            stringBuilder.AppendLine("[files]");
            stringBuilder.AppendLine("[Mail]");
            stringBuilder.AppendLine("MAPI=1");
            stringBuilder.AppendLine("CMCDLLNAME32=mapi32.dll");
            stringBuilder.AppendLine("CMC=1");
            stringBuilder.AppendLine("MAPIX=1");
            stringBuilder.AppendLine("MAPIXVER=1.0.0.1");
            stringBuilder.AppendLine("OLEMessaging=1");
            stringBuilder.AppendLine("[MCI Extensions.BAK]");
            stringBuilder.AppendLine("3g2=MPEGVideo");
            stringBuilder.AppendLine("3gp=MPEGVideo");
            stringBuilder.AppendLine("3gp2=MPEGVideo");
            stringBuilder.AppendLine("3gpp=MPEGVideo");
            stringBuilder.AppendLine("aac=MPEGVideo");
            stringBuilder.AppendLine("adt=MPEGVideo");
            stringBuilder.AppendLine("adts=MPEGVideo");
            stringBuilder.AppendLine("m2t=MPEGVideo");
            stringBuilder.AppendLine("m2ts=MPEGVideo");
            stringBuilder.AppendLine("m2v=MPEGVideo");
            stringBuilder.AppendLine("m4a=MPEGVideo");
            stringBuilder.AppendLine("m4v=MPEGVideo");
            stringBuilder.AppendLine("mod=MPEGVideo");
            stringBuilder.AppendLine("mov=MPEGVideo");
            stringBuilder.AppendLine("mp4=MPEGVideo");
            stringBuilder.AppendLine("mp4v=MPEGVideo");
            stringBuilder.AppendLine("mts=MPEGVideo");
            stringBuilder.AppendLine("ts=MPEGVideo");
            stringBuilder.AppendLine("tts=MPEGVideo");

            return stringBuilder.ToString();
        }

        private string[] GetFakeLines()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml>");
            stringBuilder.AppendLine("<book>");
            stringBuilder.AppendLine("  <title>The Lord of the Rings</title>");
            stringBuilder.AppendLine("</book>");
            stringBuilder.AppendLine("<book>");
            stringBuilder.AppendLine("  <title>Bible</title>");
            stringBuilder.AppendLine("</book>");
            stringBuilder.AppendLine("<magazine>");
            stringBuilder.AppendLine("  <title>Playboy</title>");
            stringBuilder.AppendLine("</magazine>");
            stringBuilder.AppendLine("<book>");
            stringBuilder.AppendLine("  <title>Harry Potter</title>");
            stringBuilder.AppendLine("</book>");

            var lineSeparator = new string[] { Environment.NewLine };
            return stringBuilder.ToString().Split(lineSeparator, StringSplitOptions.None);

        }

        private string[] GetFakeLinesInFileContent()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("# Do not accept source routing");            
            stringBuilder.AppendLine("net.ipv4.conf.default.accept_source_route = 0");            
            stringBuilder.AppendLine("net.ipv4.conf.all.accept_redirects = 0");            
            stringBuilder.AppendLine("# Controls the System Request debugging functionality of the kernel");

            var lineSeparator = new string[] { Environment.NewLine };
            return stringBuilder.ToString().Split(lineSeparator, StringSplitOptions.None);
        }
    }
}
