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
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Unix = Modulo.Collect.OVAL.Definitions.Unix;
using Linux = Modulo.Collect.OVAL.Definitions.Linux;
using Solaris = Modulo.Collect.OVAL.Definitions.Solaris;

namespace Modulo.Collect.OVAL.Tests
{
    [TestClass()]
    public class oval_definitions_Test
    {
        [TestMethod, Owner("mgaspar")]
        public void Test_Load_Sample_Document_Float_Test()
        {
            IEnumerable<string> errors;
            var sampleDoc = GetOvalDocumentAsStream("samples.oval.org.mitre.oval.def.5368.xml");

            var target = oval_definitions.GetOvalDefinitionsFromStream(sampleDoc, out errors);

            Assert.IsNotNull(target);
            Assert.AreEqual(0, errors.Count());
            Assert.AreEqual(3, target.variables.OfType<VariablesTypeVariableConstant_variable>().Count());
            Assert.AreEqual(3, target.objects.Count());
            Assert.AreEqual(16, target.states.Count());
            Assert.AreEqual(16, target.tests.Count());
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Load_Invalid_Definitions_Document()
        {
            IEnumerable<string> errors;
            var sampleDoc = GetOvalDocumentAsStream("samples.oval.org.mitre.oval.def.5368.invalid.xml");
            
            var target = oval_definitions.GetOvalDefinitionsFromStream(sampleDoc, out errors);

            Assert.IsNull(target);
            Assert.IsTrue(errors.Count() > 0);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Load_FDCC_windows_xp_firewall_Document()
        {
            IEnumerable<string> errors;
            var sampleDoc = GetOvalDocumentAsStream("samples.fdcc_xpfirewall_oval.xml");
            
            var target = oval_definitions.GetOvalDefinitionsFromStream(sampleDoc, out errors);
            
            Assert.IsNotNull(target);
            Assert.AreEqual(0, errors.Count());
            Assert.AreEqual(28, target.definitions.Count());
            Assert.AreEqual(71, target.tests.Count());
            Assert.AreEqual(47, target.objects.Count());
            Assert.AreEqual(31, target.states.Count());
            Assert.AreEqual(23, target.variables.Count());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_load_all_objects_from_unix_schema()
        {
            IEnumerable<string> errors;
            var ovalDefinitionsDocument = GetOvalDocumentAsStream("oval_definitions.definitions_all_unix.xml");

            var ovalDefinitions = oval_definitions.GetOvalDefinitionsFromStream(ovalDefinitionsDocument, out errors);

            Assert.IsNotNull(ovalDefinitions);
            Assert.AreEqual(0, errors.Count());
            AssertDefinitionUnixObjects(ovalDefinitions.objects);
            AssertDefinitionUnixStates(ovalDefinitions.states);
            AssertDefinitionsUnixTests(ovalDefinitions.tests);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_load_all_objects_from_linux_schema()
        {
            IEnumerable<string> errors;
            var ovalDefinitionsDocument = GetOvalDocumentAsStream("oval_definitions.definitions_all_linux.xml");

            var ovalDefinitions = oval_definitions.GetOvalDefinitionsFromStream(ovalDefinitionsDocument, out errors);

            Assert.IsNotNull(ovalDefinitions);
            Assert.AreEqual(0, errors.Count());
            AssertDefinitionLinuxObjects(ovalDefinitions.objects);
            AssertDefinitionLinuxStates(ovalDefinitions.states);
            AssertDefinitionsLinuxTests(ovalDefinitions.tests);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_load_all_objects_from_solaris_schema()
        {
            IEnumerable<string> errors;
            var solarisDefinitionsDocument = GetOvalDocumentAsStream("oval_definitions.definitions_all_solaris.xml");

            var solarisDefinitions = oval_definitions.GetOvalDefinitionsFromStream(solarisDefinitionsDocument, out errors);

            Assert.IsNotNull(solarisDefinitions);
            Assert.AreEqual(0, errors.Count());
            AssertDefinitionSolarisObjects(solarisDefinitions.objects);
            AssertDefinitionSolarisStates(solarisDefinitions.states);
            AssertDefinitionsSolarisTests(solarisDefinitions.tests);
        }

        private void AssertDefinitionsUnixTests(IEnumerable<TestType> tests)
        {
            Assert.AreEqual(10, tests.Count());
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:1")), typeof(Unix.runlevel_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:2")), typeof(Unix.file_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:3")), typeof(Unix.inetd_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:4")), typeof(Unix.interface_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:5")), typeof(Unix.password_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:6")), typeof(Unix.process_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:7")), typeof(Unix.sccs_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:8")), typeof(Unix.shadow_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:9")), typeof(Unix.uname_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:10")), typeof(Unix.xinetd_test));
        }

        private void AssertDefinitionUnixObjects(IEnumerable<ObjectType> objects)
        {
            Assert.AreEqual(10, objects.Count());
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:1")), typeof(Unix.runlevel_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:2")), typeof(Unix.file_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:3")), typeof(Unix.inetd_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:4")), typeof(Unix.interface_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:5")), typeof(Unix.password_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:6")), typeof(Unix.process_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:7")), typeof(Unix.sccs_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:8")), typeof(Unix.shadow_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:9")), typeof(Unix.uname_object));
            Assert.IsInstanceOfType(objects.Single(obj => obj.id.Equals("oval:modulo:obj:10")), typeof(Unix.xinetd_object));
        }

        private void AssertDefinitionUnixStates(IEnumerable<StateType> states)
        {
            Assert.AreEqual(10, states.Count());
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:1")), typeof(Unix.runlevel_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:2")), typeof(Unix.file_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:3")), typeof(Unix.inetd_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:4")), typeof(Unix.interface_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:5")), typeof(Unix.password_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:6")), typeof(Unix.process_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:7")), typeof(Unix.sccs_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:8")), typeof(Unix.shadow_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:9")), typeof(Unix.uname_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:10")), typeof(Unix.xinetd_state));
        }

        private void AssertDefinitionsLinuxTests(IEnumerable<TestType> tests)
        {
            Assert.AreEqual(4, tests.Count());
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:1")), typeof(Linux.rpminfo_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:2")), typeof(Linux.dpkginfo_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:3")), typeof(Linux.inetlisteningservers_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:4")), typeof(Linux.slackwarepkginfo_test));
        }

        private void AssertDefinitionLinuxObjects(IEnumerable<ObjectType> objects)
        {
            Assert.AreEqual(4, objects.Count());
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:1")), typeof(Linux.rpminfo_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:2")), typeof(Linux.dpkginfo_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:3")), typeof(Linux.inetlisteningservers_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:4")), typeof(Linux.slackwarepkginfo_object));
        }

        private void AssertDefinitionLinuxStates(IEnumerable<StateType> states)
        {
            Assert.AreEqual(4, states.Count());
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:1")), typeof(Linux.rpminfo_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:2")), typeof(Linux.dpkginfo_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:3")), typeof(Linux.inetlisteningservers_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:4")), typeof(Linux.slackwarepkginfo_state));
        }


        private void AssertDefinitionsSolarisTests(IEnumerable<TestType> tests)
        {
            Assert.AreEqual(10, tests.Count());
            
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:1")), typeof(Solaris.isainfo_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:2")), typeof(Solaris.package_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:3")), typeof(Solaris.patch_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:4")), typeof(Solaris.patch54_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:5")), typeof(Solaris.smf_test));

            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:10")), typeof(Solaris.isainfo_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:20")), typeof(Solaris.package_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:30")), typeof(Solaris.patch_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:40")), typeof(Solaris.patch54_test));
            Assert.IsInstanceOfType(tests.Single(tst => tst.id.Equals("oval:modulo:tst:50")), typeof(Solaris.smf_test));
        }

        private void AssertDefinitionSolarisObjects(IEnumerable<ObjectType> objects)
        {
            Assert.AreEqual(10, objects.Count());
            
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:1")), typeof(Solaris.isainfo_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:2")), typeof(Solaris.package_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:3")), typeof(Solaris.patch_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:4")), typeof(Solaris.patch54_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:5")), typeof(Solaris.smf_object));

            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:10")), typeof(Solaris.isainfo_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:20")), typeof(Solaris.package_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:30")), typeof(Solaris.patch_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:40")), typeof(Solaris.patch54_object));
            Assert.IsInstanceOfType(objects.Single(tst => tst.id.Equals("oval:modulo:obj:50")), typeof(Solaris.smf_object));
        }

        private void AssertDefinitionSolarisStates(IEnumerable<StateType> states)
        {
            Assert.AreEqual(8, states.Count());
            
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:1")), typeof(Solaris.isainfo_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:2")), typeof(Solaris.package_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:3")), typeof(Solaris.patch_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:5")), typeof(Solaris.smf_state));

            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:10")), typeof(Solaris.isainfo_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:20")), typeof(Solaris.package_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:30")), typeof(Solaris.patch_state));
            Assert.IsInstanceOfType(states.Single(ste => ste.id.Equals("oval:modulo:ste:50")), typeof(Solaris.smf_state));
        }

        private Stream GetOvalDocumentAsStream(string resourceName)
        {
            var assemblyName = GetType().Assembly.GetName().Name;
            var completeDocumentFilename = string.Format("{0}.{1}", assemblyName, resourceName);

            return GetType().Assembly.GetManifestResourceStream(completeDocumentFilename);
        }

    }
}
