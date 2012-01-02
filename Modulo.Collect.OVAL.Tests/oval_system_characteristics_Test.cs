/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
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
using Modulo.Collect.OVAL.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
using Modulo.Collect.OVAL.Common;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System;
using System.Collections.Generic;
using Modulo.Collect.OVAL.Schema;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Tests.helpers;
using sc = Modulo.Collect.OVAL.SystemCharacteristics;
namespace Modulo.Collect.OVAL.Tests
{
    
    /// <summary>
    ///This is a test class for oval_system_characteristics and is intended
    ///to contain all oval_system_characteristics Unit Tests
    ///</summary>
    [TestClass()]
    public class oval_system_characteristics_Test
    {

        [TestMethod, Owner("mgaspar")]
        public void Test_Load_Sample_Document()
        {
            IEnumerable<string> errors;
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.sc.5368.xml");
            var target = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);
            Assert.IsNotNull(target);
            Assert.AreEqual(0, errors.Count());
            Assert.AreEqual(3, target.collected_objects.Count());
            Assert.AreEqual(3, target.system_data.Count());
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Load_Invalid_SC_Document()
        {
            IEnumerable<string> errors;
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
               GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.sc.5368.invalid.xml");
            var target = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);

            Assert.IsNull(target);
            Assert.IsTrue(errors.Count() > 0);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Load_FDCC_windows_xp_firewall_SC_Document()
        {
            IEnumerable<string> errors;
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name + ".samples.fdcc_xpfirewall_oval.sc.xml");
            var target = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);
            Assert.IsNotNull(target);
            Assert.AreEqual(0, errors.Count());
            Assert.AreEqual(47, target.collected_objects.Count());
            Assert.AreEqual(20, target.system_data.Count());
        }

        [TestMethod, Owner("lcosta")]
        public void Test_To_Generate_XML_System_Characteristics_From_The_Object_OvalSystemCharacteristics()
        {
            IEnumerable<string> errors;
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name + ".samples.oval.org.mitre.oval.sc.5368.xml");
            var target = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);          
            
            //generate the xml from the SystemCharacteristics object
            string systemCharacteristicsXML = target.GetSystemCharacteristicsXML();
            Assert.IsNotNull(systemCharacteristicsXML);

            //creates a stream for the xml generated
            MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(systemCharacteristicsXML));
            IEnumerable<string> loadErrors;

            // load the oval_system_characteristics_object from the xml generated
            oval_system_characteristics systemCharacteristics = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(m, out loadErrors);

            Assert.IsNotNull(systemCharacteristics, "the system characteristics is null");
            Assert.IsTrue(loadErrors.Count() == 0, "the errors occurs in the load systemCharacteristics object");
            Assert.IsTrue(systemCharacteristics.collected_objects.Count() == target.collected_objects.Count(),"the collected object is not the expected");
            Assert.IsTrue(systemCharacteristics.system_data.Count() == target.system_data.Count(), "the system_data is not the expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_an_ObjectType_by_the_OvalId()
        {
            oval_system_characteristics systemCharacteristics = new OvalDocumentLoader().GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            sc::ObjectType objectType = systemCharacteristics.GetCollectedObjectByID("oval:org.mitre.oval:obj:1000");

            Assert.IsNotNull(objectType, "the objectype was not found");
            Assert.AreEqual("oval:org.mitre.oval:obj:1000", objectType.id, "the objectType is not expected");
            Assert.AreEqual(1, objectType.reference.Count(), "the item_ref count is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_SystemData_by_the_reference()
        {
            oval_system_characteristics systemCharacteristics = new OvalDocumentLoader().GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
            ItemType itemtype = systemCharacteristics.GetSystemDataByReferenceId("8");
            Assert.IsNotNull(itemtype, "the itemType was not found");
            Assert.IsInstanceOfType(itemtype, typeof(registry_item), "the object is not type expeceted");

        }
        
    }
}
