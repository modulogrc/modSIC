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
using Modulo.Collect.Probe.Windows.XmlFileContent;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Rhino.Mocks;
using System.IO;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Independent.XmlFileContent;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.XmlFileContent
{
    [TestClass]
    public class XmlFileContentObjectCollectorTests
    {
        [TestMethod, Owner("cpaiva")]
        public void How_to_transform_a_list_of_strings_in_a_single_string()
        {
            var stringArray = new string[] { "claudio", "paiva" };
            
            Assert.AreEqual("claudio paiva", string.Join(" ", stringArray));
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_a_xmlfilecontent_item()
        {
            //<xmlfilecontent_object id="oval:com.modulo.IIS7:obj:44110" version="1">
            //    <path datatype="string" var_ref="oval:com.modulo.IIS7:var:5161"/>
            //    <filename>applicationHost.config</filename>
            //    <xpath>.//processModel/@pingingEnabled</xpath>
            //</xmlfilecontent_object>

            var mockedXPathOperator = this.CreateMockedXPathOperator(CreateFakeXmlLines(), null);
            var systemDataSource = new XmlFileContentObjectCollector() { XPathOperator = mockedXPathOperator };
            
            var collectedItems = systemDataSource.CollectDataForSystemItem(CreateFakeXmlFileContentItem()).ToArray();

            DoBasicAssertForXmlFileContentCollectedItems(collectedItems, StatusEnumeration.exists);
            var valueOfEntity = ((xmlfilecontent_item)collectedItems.Single().ItemType).value_of;
            Assert.AreEqual(1, valueOfEntity.Count(), "Unexpected value_of entity count was found.");
            ItemTypeEntityChecker.AssertItemTypeEntity(valueOfEntity.Single(), "The OVAL Repository");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_filenotfoundexception()
        {
            var mockedPathOperator = this.CreateMockedXPathOperator(null,  new FileNotFoundException());
            var systemDataSource = new XmlFileContentObjectCollector() { XPathOperator = mockedPathOperator };

            var collectedItems = systemDataSource.CollectDataForSystemItem(CreateFakeXmlFileContentItem()).ToArray();

            DoBasicAssertForXmlFileContentCollectedItems(collectedItems, StatusEnumeration.doesnotexist);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_XPathNoResultException()
        {
            var mockedPathOperator = this.CreateMockedXPathOperator(null, new XPathNoResultException());
            var systemDataSource = new XmlFileContentObjectCollector() { XPathOperator = mockedPathOperator };

            var collectedItems = systemDataSource.CollectDataForSystemItem(CreateFakeXmlFileContentItem()).ToArray();

            DoBasicAssertForXmlFileContentCollectedItems(collectedItems, StatusEnumeration.doesnotexist);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_unexpected_exception()
        {
            var mockedPathOperator = this.CreateMockedXPathOperator(null, new Exception("Test Exception"));
            var systemDataSource = new XmlFileContentObjectCollector() { XPathOperator = mockedPathOperator };

            var collectedItems = systemDataSource.CollectDataForSystemItem(CreateFakeXmlFileContentItem()).ToArray();

            DoBasicAssertForXmlFileContentCollectedItems(collectedItems, StatusEnumeration.error);
        }

        private XPathOperator CreateMockedXPathOperator(string[] fakeFileLines, Exception exceptionToThrow)
        {
            MockRepository mocks = new MockRepository();
            var fakeFileContentProvider = mocks.DynamicMock<IFileProvider>();

            if (exceptionToThrow == null)
                Expect.Call(
                    fakeFileContentProvider.GetFileLinesContentFromHost(null))
                        .IgnoreArguments()
                            .Return(fakeFileLines);
            else
                Expect.Call(
                    fakeFileContentProvider.GetFileLinesContentFromHost(null))
                        .IgnoreArguments()
                            .Throw(exceptionToThrow);
            
            mocks.ReplayAll();

            return new XPathOperator() { FileContentProvider = fakeFileContentProvider };
        }
        
        private xmlfilecontent_item CreateFakeXmlFileContentItem()
        {
            return new xmlfilecontent_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue("c:\\temp"),
                xpath = OvalHelper.CreateItemEntityWithStringValue("//generator/product_name")
            };
        }

        private string[] CreateFakeXmlLines()
        {
            return new string[] 
            {
                @"<?xml version=""1.0"" encoding=""ISO8859-1""?>",
                @"   <oval_definitions oval-def=""oval.mitre.org/XMLSchema/oval-definitions-5"">",
                @"   <generator>",
                @"       <product_name>The OVAL Repository</product_name>",
                @"       <schema_version>5.5</schema_version>",
                @"       <timestamp>2009-07-20T21:13:42.715-04:00</timestamp>",
                @"   </generator>",
                @"</oval_definitions>"
            };
        }

        private void DoBasicAssertForXmlFileContentCollectedItems(CollectedItem[] collectedItems, StatusEnumeration expectedStatus)
        {
            var collectedItemType = collectedItems.Single().ItemType;
            var withExistsStatus = expectedStatus == StatusEnumeration.exists;
            var expectedType = typeof(xmlfilecontent_item);
            
            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, expectedType, withExistsStatus);
            

            if (expectedStatus == StatusEnumeration.error)
                ItemTypeChecker.AssertItemTypeWithErrorStatus(collectedItemType, "XmlFileContentObjectCollector");
            else
                Assert.AreEqual(expectedStatus, collectedItemType.status, "Unexpected collected item status was found.");

            var filepathEntity = ((xmlfilecontent_item)collectedItemType).filepath;
            var xpathEntity = ((xmlfilecontent_item)collectedItemType).xpath;
            ItemTypeEntityChecker.AssertItemTypeEntity(filepathEntity, "c:\\temp");
            ItemTypeEntityChecker.AssertItemTypeEntity(xpathEntity, "//generator/product_name");
        }

    }
}
