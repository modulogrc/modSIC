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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Windows.Test.File
{
    [TestClass]
    public class FileObjectTypeFactoryTest
    {
        [TestMethod,  Owner("lfernandes")]
        public void Should_be_possible_to_create_a_file_object_from_given_another_file_object_without_filepath()
        {
            var definitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            var fileObject = definitions.objects.Where(obj => obj.id == "oval:modulo:obj:2").SingleOrDefault();

            var factory = new FileObjectTypeFactory();
            var filePaths = new List<string>();
            var paths = new List<string>() { @"c:\temp" };
            var fileNames = new List<string>() { "file.txt" };

            var fileObjects = factory.CreateObjectTypeByCombinationOfEntities((file_object)fileObject, filePaths, paths, fileNames);
            var newFileObject = (file_object)fileObjects.ElementAt(0);
            Assert.AreEqual(1, fileObjects.Count());
            Assert.AreEqual(@"c:\temp", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file.txt", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_file_object_from_given_another_file_object_with_filepath()
        {

            var definitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            var fileObject = definitions.objects.Where(obj => obj.id == "oval:modulo:obj:1").SingleOrDefault();

            var factory = new FileObjectTypeFactory();
            var filePaths = new List<string>() { @"c:\temp\file.txt"};
            var paths = new List<string>();
            var fileNames = new List<string>();

            var fileObjects = factory.CreateObjectTypeByCombinationOfEntities((file_object)fileObject, filePaths, paths, fileNames);
            var newFileObject = (file_object)fileObjects.ElementAt(0);
            Assert.AreEqual(1, fileObjects.Count());
            Assert.AreEqual(@"c:\temp\file.txt", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filepath)).Value);
            Assert.AreEqual(null, ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.path)));
            Assert.AreEqual(null, ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filename)));            

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_file_object_based_on_in_combination_of_entities_without_filepath()
        {
            var definitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            var fileObject = definitions.objects.Where(obj => obj.id == "oval:modulo:obj:3").SingleOrDefault();

            var factory = new FileObjectTypeFactory();
            var filePaths = new List<string>();
            var paths = new List<string>() { @"c:\temp", @"c:\windows\temp"};
            var fileNames = new List<string>() { "file.txt" };
            
            var fileObjects = factory.CreateObjectTypeByCombinationOfEntities((file_object)fileObject, filePaths, paths, fileNames);
            var newFileObject = (file_object)fileObjects.ElementAt(0);
            Assert.AreEqual(2, fileObjects.Count());
            Assert.AreEqual(null, ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filepath)));
            Assert.AreEqual(@"c:\temp", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file.txt", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);            
            
            newFileObject = (file_object)fileObjects.ElementAt(1);
            Assert.AreEqual(null, ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filepath)));
            Assert.AreEqual(@"c:\windows\temp", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.path)).Value);
            Assert.AreEqual("file.txt", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filename)).Value);            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_file_object_based_on_in_combination_of_entities_witht_filepath()
        {
            var definitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple.xml");
            var fileObject = definitions.objects.Where(obj => obj.id == "oval:modulo:obj:10").SingleOrDefault();

            var factory = new FileObjectTypeFactory();
            var filePaths = new List<string>() { @"c:\temp\file.txt", @"c:\windows\temp\file.txt"};;
            var paths = new List<string>();
            var fileNames = new List<string>();
            
            var fileObjects = factory.CreateObjectTypeByCombinationOfEntities((file_object)fileObject, filePaths, paths, fileNames);
            var newFileObject = (file_object)fileObjects.ElementAt(0);
            Assert.AreEqual(2, fileObjects.Count());
            Assert.AreEqual(@"c:\temp\file.txt", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filepath)).Value);
            Assert.AreEqual(null, ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.path)));
            Assert.AreEqual(null, ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filename)));            
            
            newFileObject = (file_object)fileObjects.ElementAt(1);
            Assert.AreEqual(@"c:\windows\temp\file.txt", ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filepath)).Value);
            Assert.AreEqual(null, ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.path)));
            Assert.AreEqual(null, ((EntityObjectStringType)newFileObject.GetItemValue(file_object_ItemsChoices.filename)));            
        }
    }
}
