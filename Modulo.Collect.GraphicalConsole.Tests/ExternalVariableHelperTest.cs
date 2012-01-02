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
using System.IO;
using System.Reflection;
using Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.GraphicalConsole.Tests
{
    [TestClass]
    public class ExternalVariableHelperTest
    {
        [TestMethod, Owner("cpaiva")]
        public void Should_not_be_possible_check_external_variable_existence_with_a_empty_definition_filename()
        {
            MockRepository mocks = new MockRepository();

            var fakeHelper = mocks.DynamicMock<ExternalVariableHelper>();
            string errors;

            mocks.ReplayAll();

            fakeHelper.IsThereExternalVariable(null, out errors);

            mocks.VerifyAll();

            Assert.IsNotNull(errors);
            Assert.AreEqual(Resource.EmptyDefinitionFilename, errors);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_not_be_possible_check_external_variable_existence_with_a_non_existent_definition_file()
        {
            MockRepository mocks = new MockRepository();

            var helper = new ExternalVariableHelper();
            string errors;

            mocks.ReplayAll();

            helper.IsThereExternalVariable("definitions.xml", out errors);

            mocks.VerifyAll();

            Assert.IsNotNull(errors);
            Assert.AreEqual(Resource.OVALDefinitionsFileNotFound, errors);
        }

        [TestMethod, Owner("cpaiva")]
        public void If_there_are_errors_checking_external_variable_existence_an_error_message_is_expected()
        {
            MockRepository mocks = new MockRepository();

            var fakeHelper = mocks.DynamicMock<ExternalVariableHelper>();

            using (mocks.Record())
            {
                Expect.Call(fakeHelper.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeHelper.GetStream(null)).IgnoreArguments().Throw(new Exception(Resource.CannotReadFromFile));
            }

            mocks.ReplayAll();

            string errors;
            fakeHelper.IsThereExternalVariable("definitions.xml", out errors);

            mocks.VerifyAll();

            Assert.IsNotNull(errors);
            Assert.AreEqual(Resource.CannotReadFromFile, errors);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_check_external_variable_existence_in_a_file_with_external_variables()
        {
            MockRepository mocks = new MockRepository();

            var definitionFilename = "oval_file.xml";
            var stream = GetResourceStream(definitionFilename);
            var fakeHelper = mocks.DynamicMock<ExternalVariableHelper>();
            
            using (mocks.Record())
            {
                Expect.Call(fakeHelper.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeHelper.GetStream(null)).IgnoreArguments().Return(stream);
            }

            mocks.ReplayAll();

            string errors;
            var result = fakeHelper.IsThereExternalVariable(definitionFilename, out errors);

            mocks.VerifyAll();

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count());
            Assert.IsTrue(result);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_check_external_variable_existence_in_a_file_without_external_variables()
        {
            MockRepository mocks = new MockRepository();

            var definitionFilename = "oval.org.mitre.oval.def.5368.xml";
            var stream = GetResourceStream(definitionFilename);
            var fakeHelper = mocks.DynamicMock<ExternalVariableHelper>();

            using (mocks.Record())
            {
                Expect.Call(fakeHelper.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeHelper.GetStream(null)).IgnoreArguments().Return(stream);
            }

            mocks.ReplayAll();

            string errors;
            var result = fakeHelper.IsThereExternalVariable(definitionFilename, out errors);

            mocks.VerifyAll();

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count());
            Assert.IsFalse(result);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_get_external_variables_from_a_file_with_external_variables()
        {
            MockRepository mocks = new MockRepository();

            var definitionFilename = "oval_file.xml";
            var stream = GetResourceStream(definitionFilename);
            var fakeHelper = mocks.DynamicMock<ExternalVariableHelper>();

            using (mocks.Record())
            {
                Expect.Call(fakeHelper.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeHelper.GetStream(null)).IgnoreArguments().Return(stream);
            }

            mocks.ReplayAll();

            string errors;
            IEnumerable<VariablesTypeVariableExternal_variable> externalVariables = fakeHelper.GetExternalVariablesFromFile(definitionFilename, out errors);

            mocks.VerifyAll();

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count());
            Assert.IsNotNull(externalVariables);
            Assert.AreEqual(2, externalVariables.Count());

            var items = externalVariables.ToArray();
            Assert.AreEqual("oval:tutorial:var:1", items[0].id);
            Assert.AreEqual("File Path", items[0].comment);
            Assert.AreEqual("oval:tutorial:var:2", items[1].id);
            Assert.AreEqual("File Name", items[1].comment);
        }

        private Stream GetResourceStream(string filename)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Modulo.Collect.GraphicalConsole.Tests.samples.{0}", filename));
            return stream;
        }
    }
}
