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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common;
using Rhino.Mocks;
using System.Collections.Generic;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Independent.Common.Operators;

namespace Modulo.Collect.Probe.Independent.Tests
{
    
    
    /// <summary>
    ///This is a test class for PathOperatorEvaluatorTest and is intended
    ///to contain all PathOperatorEvaluatorTest Unit Tests
    ///</summary>
    [TestClass]
    public class PathOperatorEvaluatorTest
    {
        private string UNEXPECTED_PATH_VALUE_WAS_FOUND = "Unexpected path value was found after processation";
        private string UNEXPECTED_MATCH_VALUE_WAS_FOUND = "Unexpected match value was found.";

        [TestMethod]
        public void Should_be_possible_to_apply_regex_on_windows_filepath_strings()
        {
            var valuesToMatch = new string[] { "c:\\temp\\hub1\\", "c:\\temp\\hub2\\", "c:\\temp\\hub3\\" };
            var multiLevelPatterOperator = new MultiLevelPatternMatchOperation(FamilyEnumeration.windows);

            var result = multiLevelPatterOperator.applyPatternMatch(@"c:\temp\.*\usb\", valuesToMatch);

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("c:\\temp\\hub1\\usb", result.ElementAt(0), UNEXPECTED_MATCH_VALUE_WAS_FOUND);
            Assert.AreEqual("c:\\temp\\hub2\\usb", result.ElementAt(1), UNEXPECTED_MATCH_VALUE_WAS_FOUND);
            Assert.AreEqual("c:\\temp\\hub3\\usb", result.ElementAt(2), UNEXPECTED_MATCH_VALUE_WAS_FOUND);
        }

        [TestMethod]
        public void Should_be_possible_to_apply_regex_on_unix_filepath_strings()
        {
            var valuesToMatch = new string[] { "/temp/hub1/", "/temp/hub2/", "/temp/hub3/" };
            var multiLevelPatterOperator = new MultiLevelPatternMatchOperation(FamilyEnumeration.unix);

            var result = multiLevelPatterOperator.applyPatternMatch(@"/temp/.*/usb/", valuesToMatch);

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("temp/hub1/usb", result.ElementAt(0), UNEXPECTED_MATCH_VALUE_WAS_FOUND);
            Assert.AreEqual("temp/hub2/usb", result.ElementAt(1), UNEXPECTED_MATCH_VALUE_WAS_FOUND);
            Assert.AreEqual("temp/hub3/usb", result.ElementAt(2), UNEXPECTED_MATCH_VALUE_WAS_FOUND);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_patternOperation_on_filepathes_for_windows_systems()
        {
            var fileEntities = CreateFileEntities(@"c:\temp\.*\usb\", "devices.xml");
            var pathOperator = CreatePathOperatorWithBehavior(
                new string[] { "c:\\temp\\hub1\\", "c:\\temp\\hub2\\", "c:\\temp\\hub3\\" });

            var result = pathOperator.ProcessOperationsPaths(fileEntities);

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("c:\\temp\\hub1\\usb", result.ElementAt(0), UNEXPECTED_PATH_VALUE_WAS_FOUND);
            Assert.AreEqual("c:\\temp\\hub2\\usb", result.ElementAt(1), UNEXPECTED_PATH_VALUE_WAS_FOUND);
            Assert.AreEqual("c:\\temp\\hub3\\usb", result.ElementAt(2), UNEXPECTED_PATH_VALUE_WAS_FOUND);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_patternOperation_on_filepathes_for_unix_systems()
        {
            var fileEntities = CreateFileEntities(@"/temp/.*/usb/", "devices.xml");
            var pathOperator = CreatePathOperatorWithBehavior(
                new string[] { "/temp/hub1/", "/temp/hub2/", "/temp/hub3/" });

            var result = pathOperator.ProcessOperationsPaths(fileEntities);

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("temp/hub1/usb", result.ElementAt(0), UNEXPECTED_PATH_VALUE_WAS_FOUND);
            Assert.AreEqual("temp/hub2/usb", result.ElementAt(1), UNEXPECTED_PATH_VALUE_WAS_FOUND);
            Assert.AreEqual("temp/hub3/usb", result.ElementAt(2), UNEXPECTED_PATH_VALUE_WAS_FOUND);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_patternMatchOperation_only_in_fileNameEntity()
        {
            //<xmlfilecontent_object id="oval:gov.irs.rhel5:obj:39">
            //    <path>/etc/gconf/gconf.xml.mandatory</path>
            //    <filename operation="pattern match">.*</filename>
            //    <xpath>/desktop/gnome/volume_manager/automount_media</xpath>
            //</xmlfilecontent_object>
            var pathOperator = CreatePathOperatorWithBehavior(new string[] { "dev1", "dev2" });
            var fileEntities =
                CreateFileEntitiesWithPatternMatchOnlyInFilenameEntity(
                    "/etc/gconf/gconf.xml.mandatory", ".*");

            var result = pathOperator.ProcessOperationFileName(fileEntities, new string[] { "/etc/gconf/gconf.xml.mandatory" }, true);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("/etc/gconf/gconf.xml.mandatory/dev1", result.ElementAt(0), UNEXPECTED_PATH_VALUE_WAS_FOUND);
            Assert.AreEqual("/etc/gconf/gconf.xml.mandatory/dev2", result.ElementAt(1), UNEXPECTED_PATH_VALUE_WAS_FOUND);
            
        }

        private Dictionary<string, EntityObjectStringType> CreateFileEntities(string valueForFilepathEntity)
        {
            var filePathEntity = new EntityObjectStringType() 
            {  
                Value = valueForFilepathEntity,
                operation = OperationEnumeration.patternmatch
            };
            
            var fileEntities = new Dictionary<string, EntityObjectStringType>();
            fileEntities.Add("filepath", filePathEntity);
            return fileEntities;
        }
        
        private Dictionary<string, EntityObjectStringType> CreateFileEntities(string path, string filename)
        {
            var pathEntity = CreateEntityWithPatternMatch(path, true);
            var filenameEntity = CreateEntityWithPatternMatch(filename, true);

            var fileEntities = new Dictionary<string, EntityObjectStringType>();
            fileEntities.Add("path", pathEntity);
            fileEntities.Add("filename", filenameEntity);
            
            return fileEntities;
        }

        private Dictionary<string, EntityObjectStringType> CreateFileEntitiesWithPatternMatchOnlyInFilenameEntity(
            string path, string filename)
        {
            var pathEntity = CreateEntityWithPatternMatch(path, false);
            var filenameEntity = CreateEntityWithPatternMatch(filename, true);

            var fileEntities = new Dictionary<string, EntityObjectStringType>();
            fileEntities.Add("path", pathEntity);
            fileEntities.Add("filename", filenameEntity);

            return fileEntities;
        }

        private EntityObjectStringType CreateEntityWithPatternMatch(string entityValue, bool isPatternMatch)
        {
            return new EntityObjectStringType()
            {
                Value = entityValue,
                operation = isPatternMatch ? OperationEnumeration.patternmatch : OperationEnumeration.equals
            };
        }
        
        private PathOperatorEvaluator CreatePathOperatorWithBehavior(string[] childrenFilesToBeReturnByFileProvider)
        {
            var mocks = new MockRepository();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(
                fakeFileProvider.GetFileChildren(null))
                    .IgnoreArguments()
                    .Return(childrenFilesToBeReturnByFileProvider);
            mocks.ReplayAll();


            var family = childrenFilesToBeReturnByFileProvider.First().Contains(":") ? FamilyEnumeration.windows : FamilyEnumeration.unix;
            return new PathOperatorEvaluator(fakeFileProvider, family);
        }
    }
}
