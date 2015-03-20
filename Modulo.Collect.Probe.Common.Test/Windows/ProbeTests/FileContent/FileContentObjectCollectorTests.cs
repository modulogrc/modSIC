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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.FileContent;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Windows.Test.FileContent
{
    /// <summary>
    /// Summary description for FileContentSystemDataSourceTest
    /// </summary>
    [TestClass]
    public class FileContentObjectCollectorTests
    {


        private const string FAKE_DIR_PATH = "c:\\temp\\directory";
        private const string FAKE_FILE_PATH = "c:\\temp\\file1.txt";
        private const string FILE_TYPE_DIRECTORY = "Directory";
        private const string FILE_TYPE_TEXTFILE = "Text Document";
        private TargetInfo FakeTarget;

        [Ignore, Owner("imenescal")]
        public void Should_be_possible_to_get_files_from_given_directory_path_using_FileContentSystemDataSource()
        {
            FakeTarget = ProbeHelper.CreateFakeTarget();
            var fileContentSystemDataSource = new FileContentSystemDataSource(FakeTarget.GetAddress());
            /*IList<string> returnedFiles = fileContentSystemDataSource.GetValues(GetFakeWmiParameters());

            Assert.AreEqual(2, returnedFiles.Count, "The returned file count is unexpected.");
            Assert.AreEqual(FAKE_DIR_PATH, returnedFiles[0], "The fake directory was unexpected.");
            Assert.AreEqual(FAKE_FILE_PATH, returnedFiles[1], "The fake file path was unexpected."); */
            
        }

        private Dictionary<string, object> GetFakeWmiParameters()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("path", "c:\\temp");
            return parameters;
        }
        

    }
}
