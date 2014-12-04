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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test.File
{
    [TestClass]
    public class FileObjectTests
    {
        private const string OBJ_ID_FORMAT = "oval:modulo:obj:{0}";
        private const string ASSERT_GET_FULL_FILEPATH_FAIL_MESSAGE = "Invalid full file path was found.";
        private oval_definitions SimpleDefinitions;


        public FileObjectTests()
        {
            this.SimpleDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_full_filepath_from_file_object()
        {
            string EXPECTED_FULL_FILEPATH = @"c:\temp\file.txt";
            
            var objWithFilepathEntity = this.GetFileObjectFromDefinitions("1");
            var objWithPathAndFilenameEntities = this.GetFileObjectFromDefinitions("2");

            Assert.AreEqual(EXPECTED_FULL_FILEPATH, objWithFilepathEntity.GetFullFilepath(), "Invalid full file path was found.");
            Assert.AreEqual(EXPECTED_FULL_FILEPATH, objWithPathAndFilenameEntities.GetFullFilepath(), "Invalid full file path was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_full_filepath_from_file_object_with_referenced_variables()
        {
            var fileObject = this.GetFileObjectFromDefinitions("3");

            
            Assert.AreEqual("file.txt", fileObject.GetFullFilepath(), ASSERT_GET_FULL_FILEPATH_FAIL_MESSAGE);
        }



        private file_object GetFileObjectFromDefinitions(string objectTypeID)
        {
            string objectID = objectTypeID.Contains(":") ? objectTypeID : string.Format(OBJ_ID_FORMAT, objectTypeID);
            return (file_object)ProbeHelper.GetOvalComponentByOvalID(this.SimpleDefinitions, objectID);
        }
    }
}
