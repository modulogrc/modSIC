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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Windows.TextFileContent54;

namespace Modulo.Collect.Probe.Windows.Test.TextFileContent54
{
    //[TestClass]
    //public class TextFileContentItemTypeFactoryTests
    //{
    //    [TestMethod, Owner("lcosta")]
    //    public void Should_be_possible_to_create_an_itemType()
    //    {
    //        //var filePath = new List<string>() { @"c:\windows\win.ini" };
    //        //var pattern = "3g2=MPEGVideo";
    //        //var texts = new List<string>() { "3g2=MPEGVideo" };

    //        //var itemTypes = new TextFileContentItemTypeFactory().CreateTextFileContentItemTypesByCombinationOfEntitiesFrom(
    //        //                                                                                    filePath, pattern, texts);
    //        //Assert.IsTrue(itemTypes.Count() == 1);

    //        //textfilecontent_item itemType = (textfilecontent_item)itemTypes.ElementAt(0);
    //        //Assert.AreEqual(@"c:\windows\win.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("win.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.pattern.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("1", itemType.instance.Value);

    //    }

    //    [TestMethod, Owner("lcosta")]
    //    public void Should_possible_to_create_itemTypes_with_multiples_paths()
    //    {
    //        //List<string> filePath = new List<string>() { @"c:\windows\win.ini", @"c:\windows\mspt.ini", @"c:\windows\system.ini" };
    //        //var pattern = "3g2=MPEGVideo";
    //        //List<string> texts = new List<string>() { "3g2=MPEGVideo" };

    //        //IEnumerable<ItemType> itemTypes = new TextFileContentItemTypeFactory().CreateTextFileContentItemTypesByCombinationOfEntitiesFrom(
    //        //                                                                                    filePath, pattern, texts);
    //        //Assert.IsTrue(itemTypes.Count() == 3);

    //        //textfilecontent_item itemType = (textfilecontent_item)itemTypes.ElementAt(0);            
    //        //Assert.AreEqual(@"c:\windows\win.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("win.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.pattern.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("1", itemType.instance.Value);

    //        //itemType = (textfilecontent_item)itemTypes.ElementAt(1);
    //        //Assert.AreEqual(@"c:\windows\mspt.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("mspt.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.pattern.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("1", itemType.instance.Value);

    //    }

    //    [TestMethod, Owner("lcosta")]
    //    public void Should_be_possible_to_create_itemTypes_with_multiples_texts()
    //    {
    //        //List<string> filePath = new List<string>() { @"c:\windows\win.ini" };
    //        //var pattern = "3g2=MPEG";
    //        //List<string> texts = new List<string>() { "3g2=MPEGVideo", "3gp2=MPEGVideo" };

    //        //IEnumerable<ItemType> itemTypes = new TextFileContentItemTypeFactory().CreateTextFileContentItemTypesByCombinationOfEntitiesFrom(
    //        //                                                                                    filePath, pattern, texts);
    //        //Assert.IsTrue(itemTypes.Count() == 2);

    //        //textfilecontent_item itemType = (textfilecontent_item)itemTypes.ElementAt(0);
    //        //Assert.AreEqual(@"c:\windows\win.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("win.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEG", itemType.pattern.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("1", itemType.instance.Value);

    //        //itemType = (textfilecontent_item)itemTypes.ElementAt(1);
    //        //Assert.AreEqual(@"c:\windows\win.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("win.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEG", itemType.pattern.Value);
    //        //Assert.AreEqual("3gp2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("2", itemType.instance.Value);

    //    }

    //    [TestMethod, Owner("lcosta")]
    //    public void Should_be_possible_to_create_itemType_with_multiples_text_and_multiples_paths()
    //    {
    //        //List<string> filePath = new List<string>() { @"c:\windows\win.ini" , @"c:\windows\system.ini"};
    //        //var pattern = "3g2=MPEG";
    //        //List<string> texts = new List<string>() { "3g2=MPEGVideo", "3gp2=MPEGVideo" };

    //        //IEnumerable<ItemType> itemTypes = new TextFileContentItemTypeFactory().CreateTextFileContentItemTypesByCombinationOfEntitiesFrom(
    //        //                                                                                    filePath, pattern, texts);
    //        //Assert.IsTrue(itemTypes.Count() == 4);

    //        //textfilecontent_item itemType = (textfilecontent_item)itemTypes.ElementAt(0);
    //        //Assert.AreEqual(@"c:\windows\win.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("win.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEG", itemType.pattern.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("1", itemType.instance.Value);

    //        //itemType = (textfilecontent_item)itemTypes.ElementAt(1);
    //        //Assert.AreEqual(@"c:\windows\system.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("system.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEG", itemType.pattern.Value);
    //        //Assert.AreEqual("3g2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("1", itemType.instance.Value);

    //        //itemType = (textfilecontent_item)itemTypes.ElementAt(2);
    //        //Assert.AreEqual(@"c:\windows\win.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("win.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEG", itemType.pattern.Value);
    //        //Assert.AreEqual("3gp2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("2", itemType.instance.Value);

    //        //itemType = (textfilecontent_item)itemTypes.ElementAt(3);
    //        //Assert.AreEqual(@"c:\windows\system.ini", itemType.filepath.Value);
    //        //Assert.AreEqual("system.ini", itemType.filename.Value);
    //        //Assert.AreEqual("3g2=MPEG", itemType.pattern.Value);
    //        //Assert.AreEqual("3gp2=MPEGVideo", itemType.text.Value);
    //        //Assert.AreEqual("2", itemType.instance.Value);

    //    }



    //}
}
