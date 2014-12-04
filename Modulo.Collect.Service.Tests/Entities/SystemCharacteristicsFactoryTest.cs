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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Entities.Factories;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Service.Tests.Helpers;
using System.Xml.Serialization;
using System.IO;
using Modulo.Collect.Service.Entities;

namespace Modulo.Collect.Service.Tests.Entities
{
    /// <summary>
    /// Summary description for SystemCharacteristicsTest
    /// </summary>
    [TestClass]
    public class SystemCharacteristicsFactoryTest
    {
        public SystemCharacteristicsFactoryTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private LocalDataProvider localDataProvider;

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             localDataProvider = new LocalDataProvider();
         }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_system_characteristics_given_a_probeResult()
        {
            SystemCharacteristicsFactory factory = new SystemCharacteristicsFactory();

            ProbeResult probe = new ProbeResultFactory().CreateProbeResultForRegistryCollect();

            oval_system_characteristics systemCharacteristics = factory.CreateSystemCharacteristics(probe);          

            Assert.IsNotNull(systemCharacteristics);
            Assert.IsNotNull(systemCharacteristics.collected_objects, "the collected object is null");
            Assert.IsTrue(systemCharacteristics.collected_objects[0].id == ProbeResultFactory.ID_REGISTRY_OBJECT, "the oval id is not expected");
            Assert.IsTrue(systemCharacteristics.collected_objects.Count() == 1);
            Assert.IsNotNull(systemCharacteristics.system_data, "the system data is null");
            Assert.IsTrue(systemCharacteristics.system_data.Count() > 0, "the system data is empty.");
            Assert.IsNotNull(systemCharacteristics.generator, "the generator is null");
            Assert.IsNotNull(systemCharacteristics.system_info, "the system info is not null.");
            Assert.IsTrue(systemCharacteristics.system_info.interfaces.Count() == 1, "the interfaces is not expected");

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_system_characteristics_given_a_probeResult_with_variables()
        {
            SystemCharacteristicsFactory factory = new SystemCharacteristicsFactory();
            ProbeResult probe = new ProbeResultFactory().CreateProbeResultForRegistryCollectWithVariables();
 
            oval_system_characteristics systemCharacteristics = factory.CreateSystemCharacteristics(probe);

            Assert.IsNotNull(systemCharacteristics);
            Assert.IsNotNull(systemCharacteristics.collected_objects, "the collected object is null");
            Assert.IsTrue(systemCharacteristics.collected_objects[0].id == ProbeResultFactory.ID_REGISTRY_OBJECT, "the oval id is not expected");
            Assert.AreEqual(systemCharacteristics.collected_objects[0].variable_value[0].variable_id,"oval:com.hp:var:4", "the variable Id is not expected");
            Assert.AreEqual(systemCharacteristics.collected_objects[0].variable_value[0].Value, "Microsoft\\WindowsNT\\", "the variable value is not expected");

            Assert.IsTrue(systemCharacteristics.collected_objects.Count() == 1);
            Assert.IsNotNull(systemCharacteristics.system_data, "the system data is null");
            Assert.IsTrue(systemCharacteristics.system_data.Count() > 0, "the system data is empty.");
            Assert.IsNotNull(systemCharacteristics.generator, "the generator is null");
            Assert.IsNotNull(systemCharacteristics.system_info, "the system info is not null.");
            Assert.IsTrue(systemCharacteristics.system_info.interfaces.Count() == 1, "the interfaces is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_system_characteristics_in_xml_format_given_a_probeResult()
        {
            SystemCharacteristicsFactory factory = new SystemCharacteristicsFactory();

            ProbeResult probe = new ProbeResultFactory().CreateProbeResultForRegistryCollect();

            string systemCharacteristicsXML = factory.CreateSystemCharacteristicsInXMLFormat(probe);
            Assert.IsNotNull(systemCharacteristicsXML);

            //creates a stream for the xml generated
            MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(systemCharacteristicsXML));
            IEnumerable<string> loadErrors;
            oval_system_characteristics systemCharacteristicsFromXmlGenerated = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(m, out loadErrors);

            Assert.IsNotNull(systemCharacteristicsFromXmlGenerated, "Not was possible to get a SystemCharacteristis given the xml generated");
            Assert.IsTrue(loadErrors.Count() == 0, "there are errors in the xml");
           
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_get_a_system_characteristics_in_xml_format_given_a_probeResult_with_variables_defined()
        {
            SystemCharacteristicsFactory factory = new SystemCharacteristicsFactory();

            ProbeResult probe = new ProbeResultFactory().CreateProbeResultForRegistryCollectWithVariables();

            string systemCharacteristicsXML = factory.CreateSystemCharacteristicsInXMLFormat(probe);
            Assert.IsNotNull(systemCharacteristicsXML);

            //creates a stream for the xml generated
            MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(systemCharacteristicsXML));
            IEnumerable<string> loadErrors;
            oval_system_characteristics systemCharacteristicsFromXmlGenerated = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(m, out loadErrors);

            Assert.IsNotNull(systemCharacteristicsFromXmlGenerated, "Not was possible to get a SystemCharacteristis given the xml generated");
            Assert.IsTrue(loadErrors.Count() == 0, "there are errors in the xml");
            Assert.AreEqual(systemCharacteristicsFromXmlGenerated.collected_objects[0].variable_value[0].variable_id, "oval:com.hp:var:4", "the variable Id is not expected");
            Assert.AreEqual(systemCharacteristicsFromXmlGenerated.collected_objects[0].variable_value[0].Value, "Microsoft\\WindowsNT\\", "the variable value is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_new_systemCharacteristics_from_the_combination_with_others()
        {
            SystemCharacteristicsFactory factory = new SystemCharacteristicsFactory();
            
            oval_system_characteristics systemCharacteristics = this.GetSystemCharacteristics("oval.org.mitre.oval.sc.5368.xml");
            oval_system_characteristics otherSystemCharacteristics = this.GetSystemCharacteristics("fdcc_xpfirewall_oval.sc.xml");
            List<oval_system_characteristics> scList = new List<oval_system_characteristics>();
            scList.Add(systemCharacteristics);
            scList.Add(otherSystemCharacteristics);


            oval_system_characteristics newSystemCharacteristics = factory.CreateSystemCharacteristicsBy(scList);
            
            int quantityCollectedObject = systemCharacteristics.collected_objects.Count() + otherSystemCharacteristics.collected_objects.Count();
            int quantitySystemData = systemCharacteristics.system_data.Count() + otherSystemCharacteristics.system_data.Count();

            Assert.IsNotNull(newSystemCharacteristics, "the systemCharacteristics expected is null");
            Assert.IsTrue(newSystemCharacteristics.collected_objects.Count() == quantityCollectedObject," the quantity of collected objects is not expected");
            Assert.IsTrue(newSystemCharacteristics.system_data.Count() == quantitySystemData, "the quantity of system data is no expected");

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_new_systemCharacteristics_from_the_combination_with_others_and_the_systemInformation_by_SystemInfo()
        {           

            SystemCharacteristicsFactory factory = new SystemCharacteristicsFactory();

            oval_system_characteristics systemCharacteristics = this.GetSystemCharacteristics("oval.org.mitre.oval.sc.5368.xml");
            oval_system_characteristics otherSystemCharacteristics = this.GetSystemCharacteristics("fdcc_xpfirewall_oval.sc.xml");
            List<oval_system_characteristics> scList = new List<oval_system_characteristics>();
            scList.Add(systemCharacteristics);
            scList.Add(otherSystemCharacteristics);

            Target target = this.CreateTargetWithSystemInformation();

            oval_system_characteristics newSystemCharacteristics = factory.CreateSystemCharacteristicsBy(scList, target.SystemInformation);

            Assert.IsNotNull(newSystemCharacteristics, "the systemCaracteristics expected is null");
            Assert.IsNotNull(newSystemCharacteristics.system_info, "the system_info is null");
            Assert.AreEqual(newSystemCharacteristics.system_info.architecture, target.SystemInformation.Architecture);
            Assert.AreEqual(newSystemCharacteristics.system_info.os_name, target.SystemInformation.SystemName);
            Assert.AreEqual(newSystemCharacteristics.system_info.os_version, target.SystemInformation.SystemVersion);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_systemCharacteristics_eliminating_duplicate_references_for_itemType_in_the_systemData_property()
        {
            SystemCharacteristicsFactory factory = new SystemCharacteristicsFactory();
            ProbeResult probeResult = new ProbeResultFactory().CreateProbeResultForRegistryCollectWithSytemDataDuplicated();

            oval_system_characteristics systemCharacteristics = factory.CreateSystemCharacteristics(probeResult);

            Assert.IsNotNull(systemCharacteristics, "the systemCharacteristics expected is null");
            Assert.AreEqual(2, systemCharacteristics.system_data.Count(), "the quantity of system data is not expected");
        }

        private oval_system_characteristics GetSystemCharacteristics(string xmlFile)
        {
            IEnumerable<string> errors;
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name + ".system_characteristics." + xmlFile);

            var target = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);

            return target;
        }

        private Target CreateTargetWithSystemInformation()
        {
            Target target = new Target();
            target.Address = "172.16.3.166";
            target.Credential = new TargetCredential() { CredentialInfo = new CredentialFactory().GetEncryptCredentialInBytes() };
            SystemInfo systemInfo = new SystemInfo();
            systemInfo.SystemName = "Microsoft Windows Server 2008 Enterprise SP2";
            systemInfo.SystemVersion = "6.0.6002";
            systemInfo.Architecture = "INTEL32";
            systemInfo.PrimaryHostName = "mss-rj-220.mss.modulo.com.br";
            target.SystemInformation = systemInfo;
            NetworkInfo networkInfo = new NetworkInfo() { IpAddress = "172.16.3.166", MacAddress = "00 - 23 - AE - B6 - 6F - BF", Name = "Intel(R) 82567LM-3 Gigabit Network Connection" };
            systemInfo.NetworkInterfaces.Add(networkInfo);

            return target;
            
        }

            

       
    }
}
