#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
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
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Xml.Schema;
using System.Xml;

namespace Modulo.Collect.GraphicalConsole.Tests
{
    [TestClass]
    public class SchemaControllerTest
    {
        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_raising_a_schema_event_on_a_view()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = mocks.DynamicMock<ISchemaView>();

            mocks.ReplayAll();

            var controller = new SchemaController(fakeView);

            var e = new SchemaEventArgs();
            fakeView.Raise(x => x.OnValidateSchema += null, this, e);

            mocks.VerifyAll();

            Assert.IsTrue(controller.OnValidadeSchemaCalled);
            Assert.IsFalse(e.Result);
            Assert.AreEqual(Modulo.Collect.GraphicalConsole.Resource.EmptyDefinitionFilename, e.ShortErrorMessage);
            Assert.IsNull(e.LongErrorMessage);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_not_be_possible_validate_a_schema_from_a_non_existent_file()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<ISchemaView>();
            var fakeController = mocks.StrictMock<SchemaController>(new object[] { fakeView });

            using (mocks.Record())
            {
                Expect.Call(fakeController.DefinitionsFileExists(null)).IgnoreArguments().Return(false);
            }

            mocks.ReplayAll();

            var e = new SchemaEventArgs();
            e.DefinitionFilename = "sample.xml";
            fakeController.view_OnValidateSchema(this, e);

            mocks.VerifyAll();

            Assert.IsFalse(e.Result);
            Assert.AreEqual(Modulo.Collect.GraphicalConsole.Resource.OVALDefinitionsFileNotFound, e.ShortErrorMessage);
            Assert.IsNull(e.LongErrorMessage);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_validate_a_schema_from_a_valid_file()
        {
            var e = CreateSchemaTestScenario("samples.oval.org.mitre.oval.def.5368.xml");
            Assert.IsTrue(e.Result);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_not_be_possible_validate_a_schema_from_a_incorret_file()
        {
            var e = CreateSchemaTestScenario("samples.oval.org.mitre.oval.def.5368.invalid.xml");
            Assert.IsFalse(e.Result);
            Assert.AreEqual(Resource.SchemaValidationFailure, e.ShortErrorMessage);
        }

        private void AddSchema(XmlSchemaSet schemas, string targetNamespace, string schemaUri)
        {
            var xmlReader = XmlReader.Create(GetResourceStream(schemaUri));
            schemas.Add(targetNamespace, xmlReader);
        }

        private Stream GetResourceStream(string filename)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Modulo.Collect.GraphicalConsole.Tests.{0}", filename));
            return stream;
        }

        private SchemaEventArgs CreateSchemaTestScenario(string definitionFilename)
        {
            var schemaSet = new XmlSchemaSet();
            AddSchema(schemaSet, "http://oval.mitre.org/XMLSchema/oval-common-5", "xml.oval-common-schema.xsd");
            AddSchema(schemaSet, "http://oval.mitre.org/XMLSchema/oval-definitions-5", "xml.oval-definitions-schema.xsd");
            AddSchema(schemaSet, "http://oval.mitre.org/XMLSchema/oval-definitions-5#independent", "xml.independent-definitions-schema.xsd");
            AddSchema(schemaSet, "http://www.w3.org/2000/09/xmldsig#", "xml.xmldsig-core-schema.xsd");

            var stream = GetResourceStream(definitionFilename);

            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<ISchemaView>();
            var fakeController = mocks.StrictMock<SchemaController>(new object[] { fakeView });

            using (mocks.Record())
            {
                Expect.Call(fakeController.DefinitionsFileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeController.CreateXmlSchemaSet()).IgnoreArguments().Return(schemaSet);
                Expect.Call(fakeController.GetStream(null)).IgnoreArguments().Return(stream);
            }

            mocks.ReplayAll();

            var e = new SchemaEventArgs();
            e.DefinitionFilename = definitionFilename;
            fakeController.view_OnValidateSchema(this, e);

            mocks.VerifyAll();
            return e;
        }
    }
}
