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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Windows.TextFileContent54;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Windows.Test.ProbeTests.TextFileContent54
{
    [TestClass]
    public class TextFileContentObjectFactoryTests
    {
        private const string UNEXPECTED_ENTITY_ORDER = "Unexpected file object entity name order was found.";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_itemType_defined_with_path_and_filename_entities()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "910");
            var paths = new string[] { @"c:\windows" };
            var filenames = new string[] { @"win.ini" };
            var patterns = new string[] { "3g2=MPEG" };
            var instances = new string[] { "3g2=MPEGVideo", "3gp2=MPEGVideo" };

            var createdFileObjects =
                new TextFileContentObjectFactory()
                    .CreateFileObjects((textfilecontent54_object)objectType, null, filenames, paths, patterns, instances);

            Assert.IsNotNull(createdFileObjects, "The file objects were not created.");
            Assert.AreEqual(2, createdFileObjects.Count());

            var firstObject = (textfilecontent54_object)createdFileObjects.First();
            var secondObject = (textfilecontent54_object)createdFileObjects.ElementAt(1);

            Assert.IsNull(firstObject.GetItemValue(textfilecontent54_ItemsChoices.filepath));
            AssertEntityNamesOrder(firstObject);
            AssertTextFileContentObjectEntity(firstObject, textfilecontent54_ItemsChoices.path, paths.Single());
            AssertTextFileContentObjectEntity(firstObject, textfilecontent54_ItemsChoices.filename, filenames.Single());
            AssertTextFileContentObjectEntity(firstObject, textfilecontent54_ItemsChoices.pattern, patterns.Single());
            AssertTextFileContentObjectEntity(firstObject, textfilecontent54_ItemsChoices.instance, instances.First());

            Assert.IsNull(secondObject.GetItemValue(textfilecontent54_ItemsChoices.filepath));
            AssertEntityNamesOrder(secondObject);
            AssertTextFileContentObjectEntity(secondObject, textfilecontent54_ItemsChoices.path, paths.Single());
            AssertTextFileContentObjectEntity(secondObject, textfilecontent54_ItemsChoices.filename, filenames.Single());
            AssertTextFileContentObjectEntity(secondObject, textfilecontent54_ItemsChoices.pattern, patterns.Single());
            AssertTextFileContentObjectEntity(secondObject, textfilecontent54_ItemsChoices.instance, instances.ElementAt(1));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_itemtype_defined_with_filepath_entity()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "920");
            var filepaths = new string[] { @"c:\windows\win.ini" };
            var patterns = new string[] { "VERSION.*" };
            var instances = new string[] { "1" };

            var createdFileObjects =
                new TextFileContentObjectFactory()
                    .CreateFileObjects((textfilecontent54_object)objectType, filepaths, null, null, patterns, instances);

            Assert.IsNotNull(createdFileObjects, "The file objects were not created.");
            Assert.AreEqual(1, createdFileObjects.Count());

            var firstObject = (textfilecontent54_object)createdFileObjects.First();
            Assert.IsNull(firstObject.GetItemValue(textfilecontent54_ItemsChoices.path));
            Assert.IsNull(firstObject.GetItemValue(textfilecontent54_ItemsChoices.filename));

            AssertEntityNamesOrder(firstObject);
            AssertTextFileContentObjectEntity(firstObject, textfilecontent54_ItemsChoices.filepath, filepaths.Single());
            AssertTextFileContentObjectEntity(firstObject, textfilecontent54_ItemsChoices.pattern, patterns.Single());
            AssertTextFileContentObjectEntity(firstObject, textfilecontent54_ItemsChoices.instance, instances.First());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_new_object_type_keeping_the_behavior_entity_from_source_object()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "920");
            var filepaths = new string[] { @"c:\windows\win.ini" };
            var patterns = new string[] { "VERSION.*" };
            var instances = new string[] { "1" };

            var createdFileObjects = 
                new TextFileContentObjectFactory()
                    .CreateFileObjects((textfilecontent54_object)objectType, filepaths, null, null, patterns, instances);

            Assert.IsNotNull(createdFileObjects, "The file objects were not created.");
            Assert.AreEqual(1, createdFileObjects.Count());
            var textFileObject = (textfilecontent54_object)createdFileObjects.Single();
            var multiLineBehavior = (Textfilecontent54Behaviors)((textfilecontent54_object)textFileObject).Items.OfType<Textfilecontent54Behaviors>().Single();
            Assert.IsFalse(multiLineBehavior.multiline);
            Assert.IsFalse(textFileObject.IsMultiline(), "Unexpected behavior value was found.");
            
        }


        private void AssertEntityNamesOrder(textfilecontent54_object textFileContentObject)
        {
            if (textFileContentObject.IsFilePathDefined())
            {
                var firstElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(0);
                var secondElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(1);
                var thirdElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(2);
                var fourthElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(3);

                Assert.AreEqual(textfilecontent54_ItemsChoices.behaviors, firstElement, UNEXPECTED_ENTITY_ORDER);
                Assert.AreEqual(textfilecontent54_ItemsChoices.filepath, secondElement, UNEXPECTED_ENTITY_ORDER);
                Assert.AreEqual(textfilecontent54_ItemsChoices.pattern, thirdElement, UNEXPECTED_ENTITY_ORDER);
                Assert.AreEqual(textfilecontent54_ItemsChoices.instance, fourthElement, UNEXPECTED_ENTITY_ORDER);
            }
            else
            {
                var firstElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(0);
                var secondElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(1);
                var thirdElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(2);
                var fourthElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(3);
                var fifthElement = textFileContentObject.Textfilecontent54ItemsElementName.ElementAt(4);

                Assert.AreEqual(textfilecontent54_ItemsChoices.behaviors, firstElement, UNEXPECTED_ENTITY_ORDER);
                Assert.AreEqual(textfilecontent54_ItemsChoices.path, secondElement, UNEXPECTED_ENTITY_ORDER);
                Assert.AreEqual(textfilecontent54_ItemsChoices.filename, thirdElement, UNEXPECTED_ENTITY_ORDER);
                Assert.AreEqual(textfilecontent54_ItemsChoices.pattern, fourthElement, UNEXPECTED_ENTITY_ORDER);
                Assert.AreEqual(textfilecontent54_ItemsChoices.instance, fifthElement, UNEXPECTED_ENTITY_ORDER);
            }
        }

        private void AssertTextFileContentObjectEntity(
            textfilecontent54_object textFileContentObject,
            textfilecontent54_ItemsChoices entityName,
            string expectedEntityValue)
        {
            var entityToAssert = (EntitySimpleBaseType)textFileContentObject.GetItemValue(entityName);
            Assert.IsNotNull(entityToAssert, "The entity cannot be null");
            Assert.AreEqual(expectedEntityValue, entityToAssert.Value, "An unexpected entity value was found.");
        }
    }
}
