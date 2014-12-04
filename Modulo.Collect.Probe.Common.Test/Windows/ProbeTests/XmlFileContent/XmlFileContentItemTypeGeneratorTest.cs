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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.XmlFileContent;
using Rhino.Mocks;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Independent.XmlFileContent;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.XmlFileContent
{
    [TestClass]
    public class XmlFileContentItemTypeGeneratorTest
    {
        [TestMethod, Owner("cpaiva")]
        public void Should_be_to_generate_xmlfilecontent_items_from_an_simple_object()
        {
            var fakeOvalObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2300");
            var mocks = new MockRepository();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            var pathOperator = new PathOperatorEvaluator(fakeFileProvider, FamilyEnumeration.windows);
            Expect.Call(fakeFileProvider.FileExists(null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();

            var itemsToCollect = new XmlFileContentItemTypeGenerator(pathOperator).GetItemsToCollect(fakeOvalObject, null).ToArray();

            DoBasicAssert(itemsToCollect, 1, typeof(xmlfilecontent_item));
            AssertXmlFileItem(itemsToCollect.Single(), @"c:\temp\definitions\bookstore.xml", @"//bookstore/book/title/text()");
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_to_generate_xmlfilecontent_items_from_an_object_with_referenced_variable_on_filepath_entity()
        {
            MockRepository mocks = new MockRepository();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(fakeFileProvider.FileExists(null)).IgnoreArguments().Return(true);
            var pathOperator = new PathOperatorEvaluator(fakeFileProvider, FamilyEnumeration.windows);
            var itemTypeGenerator = new XmlFileContentItemTypeGenerator(pathOperator);
            var expectedFilepath = @"c:\temp\definitions\bookstore.xml";
            var fakeOvalObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2310");
            var fakeEvaluatedVariables = VariableHelper.CreateVariableWithOneValue(fakeOvalObject.id, "oval:modulo:var:2310", expectedFilepath);
            mocks.ReplayAll();

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeOvalObject, fakeEvaluatedVariables);

            DoBasicAssert(itemsToCollect.ToArray(), 1, typeof(xmlfilecontent_item));
            AssertXmlFileItem(itemsToCollect.Single(), expectedFilepath, "//bookstore/book/price/text()");
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_to_generate_xmlfilecontent_items_from_an_object_with_referenced_variable_on_xpath_entity()
        {
            var expectedXpath = @"//bookstore/book/author/text()";
            var fakeOvalObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2315");
            var fakeEvaluatedVariables = VariableHelper.CreateVariableWithOneValue(fakeOvalObject.id, "oval:modulo:var:2315", expectedXpath);
            var mocks = new MockRepository();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            var pathOperator = new PathOperatorEvaluator(fakeFileProvider, FamilyEnumeration.windows);
            var itemTypeGenerator = new XmlFileContentItemTypeGenerator(pathOperator);
            Expect.Call(fakeFileProvider.FileExists(null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeOvalObject, fakeEvaluatedVariables).ToArray();

            DoBasicAssert(itemsToCollect, 1, typeof(xmlfilecontent_item));
            AssertXmlFileItem(itemsToCollect.Single(), @"c:\temp\bools.xml", expectedXpath);

        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_to_generate_xmlfilecontent_items_from_an_object_with_patternmatch_operation()
        {
            var fakeFilepaths = new string[] { @"c:\temp\book.txt", @"c:\temp\book.xml", @"c:\temp\bookstore.xml", @"c:\temp\readme.txt" };
            var fakeOvalObject = (xmlfilecontent_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2320");
            var xpathObjectEntity = (EntitySimpleBaseType)fakeOvalObject.GetItemValue(xmlfilecontent_ItemsChoices.xpath);
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(fakeFilepaths);

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeOvalObject, null).ToArray();

            DoBasicAssert(itemsToCollect, 2, typeof(xmlfilecontent_item));
            AssertXmlFileItem(itemsToCollect.ElementAt(0), fakeFilepaths.ElementAt(1), xpathObjectEntity.Value);
            AssertXmlFileItem(itemsToCollect.ElementAt(1), fakeFilepaths.ElementAt(2), xpathObjectEntity.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_xmlfilecontent_items_from_an_object_with_patternMatch_operation_in_the_middle_of_a_windows_path()
        {
            //<xmlfilecontent_object id="oval:modulo:obj:2321">
            //    <filepath operation="pattern match">c:\temp\.*\usb\devices.xml</filepath>
            //    <xpath>//bookstore/book/price/text()</xpath>
            //</xmlfilecontent_object>
            var fakeFilepaths = new string[] { @"c:\temp\hub1\", @"c:\temp\hub2\", @"c:\temp\hub3\" };
            var fakeOvalObject = (xmlfilecontent_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2321");
            var xpathObjectEntity = (EntitySimpleBaseType)fakeOvalObject.GetItemValue(xmlfilecontent_ItemsChoices.xpath);
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(fakeFilepaths);

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeOvalObject, null).ToArray();

            DoBasicAssert(itemsToCollect, 3, typeof(xmlfilecontent_item));
            AssertXmlFileItem(itemsToCollect.ElementAt(0), @"c:\temp\hub1\usb\devices.xml", xpathObjectEntity.Value);
            AssertXmlFileItem(itemsToCollect.ElementAt(1), @"c:\temp\hub2\usb\devices.xml", xpathObjectEntity.Value);
            AssertXmlFileItem(itemsToCollect.ElementAt(2), @"c:\temp\hub3\usb\devices.xml", xpathObjectEntity.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_xmlfilecontent_items_from_an_object_with_patternMatch_operation_in_the_middle_of_a_unix_path()
        {
            //<xmlfilecontent_object id="oval:modulo:obj:2322">
            //    <path>/etc/gconf/gconf.xml.mandatory</path>
            //    <filename operation="pattern match">.*</filename>
            //    <xpath>/desktop/gnome/volume_manager/automount_media</xpath>
            //</xmlfilecontent_object>
            var fakeFilepaths = new string[] { @"file1", @"file2" };
            var fakeOvalObject = (xmlfilecontent_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "2322");
            var xpathObjectEntity = (EntitySimpleBaseType)fakeOvalObject.GetItemValue(xmlfilecontent_ItemsChoices.xpath);
            var itemTypeGenerator = this.CreateMockedItemTypeGenerator(fakeFilepaths, false);

            var itemsToCollect = itemTypeGenerator.GetItemsToCollect(fakeOvalObject, null).ToArray();

            DoBasicAssert(itemsToCollect, 2, typeof(xmlfilecontent_item));
            AssertXmlFileItem(itemsToCollect.ElementAt(0), @"/etc/gconf/gconf.xml.mandatory/file1", xpathObjectEntity.Value);
            AssertXmlFileItem(itemsToCollect.ElementAt(1), @"/etc/gconf/gconf.xml.mandatory/file2", xpathObjectEntity.Value);
        }

 

        private XmlFileContentItemTypeGenerator CreateMockedItemTypeGenerator(
            IList<String> fakeFilepaths, 
            bool isForWindows = true)
        {
            MockRepository mocks = new MockRepository();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(fakeFileProvider.GetFileChildren(null)).IgnoreArguments().Return(fakeFilepaths);
            Expect.Call(fakeFileProvider.FileExists(null)).IgnoreArguments().Return(true);
            mocks.ReplayAll();

            var targetFamily = isForWindows ? FamilyEnumeration.windows : FamilyEnumeration.unix;
            var pathOperator = new PathOperatorEvaluator(fakeFileProvider, targetFamily);
            return new XmlFileContentItemTypeGenerator(pathOperator);
        }

        private void DoBasicAssert(ItemType[] itemsToCollect, int expectedItemsCount, Type expectedInstanceOfItemTypes)
        {
            Assert.IsNotNull(itemsToCollect);
            Assert.AreEqual(expectedItemsCount, itemsToCollect.Count());
            foreach (var item in itemsToCollect)
                Assert.IsInstanceOfType(item, expectedInstanceOfItemTypes);
        }

        private void AssertXmlFileItem(ItemType itemToAssert, string expectedFilepath, string expectedXPath)
        {
            var xmlFileContentItem = (xmlfilecontent_item)itemToAssert;
            Assert.IsNotNull(xmlFileContentItem.filepath);
            Assert.AreEqual(expectedFilepath, xmlFileContentItem.filepath.Value);
            Assert.IsNotNull(xmlFileContentItem.xpath);
            Assert.AreEqual(expectedXPath, xmlFileContentItem.xpath.Value);
        }


    }
}
