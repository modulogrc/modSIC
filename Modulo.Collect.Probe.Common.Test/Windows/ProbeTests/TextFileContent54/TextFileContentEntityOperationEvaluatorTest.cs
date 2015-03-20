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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.TextFileContent54;
using Rhino.Mocks;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Windows.Test.TextFileContent54
{
    [TestClass]
    public class TextFileContentEntityOperationEvaluatorTest
    {
        [Ignore, Owner("lcosta")]
        public void Should_be_possible_to_evaluate_operation_of_textFileContent()
        {
            var textFileContent = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "980"); 
            var paths = new List<string>() { @"c:\windows" };
            var fileNames = new List<string>() { @"c:\windows\win.ini" };

            var operationEvaluator = this.CreateMockForSimpleFileContent(fileNames, paths);
            var itemTypes = operationEvaluator.ProcessOperation((textfilecontent54_object)textFileContent).ToArray();

            Assert.IsTrue(itemTypes.Count() == 1);
            var itemType = (textfilecontent_item)itemTypes.ElementAt(0);
            Assert.AreEqual(@"c:\windows\win.ini",itemType.filepath.Value);
            Assert.AreEqual(@"c:\windows",itemType.path.Value);
            Assert.AreEqual("win.ini",itemType.filename.Value);
            Assert.AreEqual("3gp",itemType.pattern.Value);
            Assert.AreEqual("1",itemType.instance.Value);
            Assert.AreEqual("3gp=MPEGVideo", itemType.text.Value.Trim());
        }

        [Ignore,Owner("lcosta")]
        public void Should_be_possible_to_evaluate_operation_of_textFileContent_path()
        {
            var textFileContent = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "990");
            var paths = new List<string>() { @"c:\windows" };
            var fileNames = new List<string>() { @"c:\windows\win.ini", @"c:\windows\system.ini" };

            var operationEvaluator = this.CreateMockForSimpleFileContent(fileNames, paths);
            var result = operationEvaluator.ProcessOperation((textfilecontent54_object)textFileContent);

            Assert.IsTrue(result.Count() == 2);
            var textFileContentItemToAssert = (textfilecontent_item)result.ElementAt(0);
            Assert.AreEqual(@"c:\windows\win.ini", textFileContentItemToAssert.filepath.Value);
            Assert.AreEqual(@"c:\windows", textFileContentItemToAssert.path.Value);
            Assert.AreEqual("win.ini", textFileContentItemToAssert.filename.Value);
            Assert.AreEqual("3gp", textFileContentItemToAssert.pattern.Value);
            Assert.AreEqual("2", textFileContentItemToAssert.instance.Value);
            Assert.AreEqual("3gp2=MPEGVideo", textFileContentItemToAssert.text.Value);        
        }



        private TextFileContentEntityOperationEvaluator CreateMockForSimpleFileContent(
            IEnumerable<string> fileNames, IEnumerable<string> paths)
        {
            MockRepository mocks = new MockRepository();
            var fakeFileContentProvider = mocks.DynamicMock<IFileProvider>();
            var fakePathOperator = mocks.DynamicMock<PathOperatorEvaluator>();
            
            Expect.Call(fakeFileContentProvider.GetFileLinesContentFromHost("")).IgnoreArguments().Repeat.Any()
                .Return(this.CreateFakeFileLines());
            Expect.Call(fakePathOperator.ProcessOperationFileName(null, null, true)).IgnoreArguments()
                .Return(fileNames);
            Expect.Call(fakePathOperator.ProcessOperationsPaths(null)).IgnoreArguments()
                .Return(paths);
            
            mocks.ReplayAll();

            var objectCollector = new TextFileContentObjectCollector()
            {
                TargetInfo = ProbeHelper.CreateFakeTarget(),
                FileContentProvider = fakeFileContentProvider
            };

            var operationEvaluator = new TextFileContentEntityOperationEvaluator(objectCollector, null, FamilyEnumeration.windows);
            operationEvaluator.PathOperatorEvaluator = fakePathOperator;

            return operationEvaluator;

        }

        private string[] CreateFakeFileLines()
        {
            var list = new List<String>();
            list.Add("CMCDLLNAME32=mapi32.dll");
            list.Add("CMC=1");
            list.Add("MAPIX=1");
            list.Add("MAPIXVER=1.0.0.1");
            list.Add("[MCI Extensions.BAK]");
            list.Add("3g2=MPEGVideo");
            list.Add("3gp=MPEGVideo");
            list.Add("3gp2=MPEGVideo");
            list.Add("3gpp=MPEGVideo");
            list.Add("aac=MPEGVideo");
            
            return list.ToArray();
        }


    }
}
