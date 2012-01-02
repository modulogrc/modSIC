/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Rhino.Mocks;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Unix.TextFileContent54;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.TextFileContent54
{
    [TestClass]
    public class TextFileContentItemTypeGeneratorTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_textfilecontent_items_from_textfilecontent54_object()
        {
            var objectSample = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "3000");
            var itemTypeGenerator = CreateMockedItemTypeGenerator();

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectSample, null).ToArray();

            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(textfilecontent_item));
            var textFileContentItem = (textfilecontent_item)generatedItems.Single();
            Assert.AreEqual("noflags:lo", textFileContentItem.text.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_from_an_textfilecontent54_object_with_GreaterThanOrEqual_operation()
        {
            var objectSample = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "oval:gov.irs.sol10:obj:86");
            var itemTypeGenerator = CreateMockedItemTypeGenerator();

            var generatedItems = itemTypeGenerator.GetItemsToCollect(objectSample, null).ToArray();
            
            ItemTypeChecker.DoBasicAssertForItems(generatedItems, 1, typeof(textfilecontent_item));
            var textFileContentItem = (textfilecontent_item)generatedItems.Single();
            Assert.AreEqual("noflags:lo", textFileContentItem.text.Value);
        }


        private IItemTypeGenerator CreateMockedItemTypeGenerator()
        {
            var mocks = new MockRepository();
            var fakeObjectCollector = mocks.DynamicMock<BaseObjectCollector>();
            var fakeFileProvider = mocks.DynamicMock<IFileProvider>();
            Expect.Call(fakeObjectCollector.GetValues(null))
                .IgnoreArguments()
                    .Return(new String[] { "noflags:lo" });

            mocks.ReplayAll();

            return new TextFileContentItemTypeGenerator()
            {
                OperationEvaluator =
                    new TextFileContentEntityOperationEvaluator(
                        fakeObjectCollector, fakeFileProvider, FamilyEnumeration.unix)
            };
        }
    }
}
