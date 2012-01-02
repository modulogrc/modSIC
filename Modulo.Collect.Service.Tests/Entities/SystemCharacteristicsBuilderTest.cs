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
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Entities.Factories;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Service.Tests.Entities
{
    /// <summary>
    /// Summary description for SystemCharacteristicsBuilderTest
    /// </summary>
    [TestClass]
    public class SystemCharacteristicsBuilderTest
    {
        public SystemCharacteristicsBuilderTest()
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
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_build_a_system_characteristics_in_steps()
        {
            SystemCharacteristicsBuilder builder = new SystemCharacteristicsBuilder();

            oval_system_characteristics sc = this.GetSystemCharacteristics("oval.org.mitre.oval.sc.5368.xml");

            oval_system_characteristics systemCharacteristics = builder.WithGenerator(sc.generator)
                                                        .WithSystemInfo(sc.system_info)
                                                        .AddCollectedObjectsWithSystemData(sc.collected_objects, sc.system_data)
                                                        .Build();

            Assert.IsNotNull(systemCharacteristics);
            Assert.IsTrue(systemCharacteristics.system_data.Count() == sc.system_data.Count(), "the system data is not expected");
            Assert.IsTrue(IsSequenceOfItemTypeExpected(systemCharacteristics), "the sequence in the system data is no expected");

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_build_a_new_system_characteristics_with_more_than_one_system_characteristics()
        {
             SystemCharacteristicsBuilder builder = new SystemCharacteristicsBuilder();

            oval_system_characteristics systemCharacteristics = this.GetSystemCharacteristics("oval.org.mitre.oval.sc.5368.xml");
            oval_system_characteristics ohterSystemCharacteristics = this.GetSystemCharacteristics("fdcc_xpfirewall_oval.sc.xml");

            oval_system_characteristics newSystemCharacteristics = builder.WithGenerator(systemCharacteristics.generator)
                                                                            .WithSystemInfo(systemCharacteristics.system_info)
                                                                            .AddCollectedObjectsWithSystemData(systemCharacteristics.collected_objects, systemCharacteristics.system_data)
                                                                            .AddCollectedObjectsWithSystemData(ohterSystemCharacteristics.collected_objects, ohterSystemCharacteristics.system_data)                          
                                                                            .Build();

            int expectedQuantity = systemCharacteristics.collected_objects.Count() + ohterSystemCharacteristics.collected_objects.Count();
            Assert.IsNotNull(newSystemCharacteristics);
            Assert.IsTrue(newSystemCharacteristics.collected_objects.Count() == expectedQuantity,"the quantity collectedObject is not expected");
            Assert.IsTrue(IsSequenceOfItemTypeExpected(newSystemCharacteristics), "the sequence is not expected.");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_build_a_system_characteristics_in_steps_and_generate_a_xml_from_this()
        {
            SystemCharacteristicsBuilder builder = new SystemCharacteristicsBuilder();

            oval_system_characteristics sc = this.GetSystemCharacteristics("oval.org.mitre.oval.sc.5368.xml");

            oval_system_characteristics systemCharacteristics = builder.WithGenerator(sc.generator)
                                                        .WithSystemInfo(sc.system_info)
                                                        .AddCollectedObjectsWithSystemData(sc.collected_objects, sc.system_data)
                                                        .Build();
            Assert.IsNotNull(systemCharacteristics);

            string systemCharacteristicsInXml = systemCharacteristics.GetSystemCharacteristicsXML();

            Assert.IsNotNull(systemCharacteristicsInXml);           
        }

        private bool IsSequenceOfItemTypeExpected(oval_system_characteristics systemCharacteristics)
        {

            int max = systemCharacteristics.system_data.Max<ItemType>(x => int.Parse(x.id));
            int min = systemCharacteristics.system_data.Min<ItemType>(x => int.Parse(x.id));

            int total =  systemCharacteristics.system_data.Count() - 1;

            return ((max - min) == total);

        }

        private oval_system_characteristics  GetSystemCharacteristics(string xmlFile)
        {
            IEnumerable<string> errors;
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(
                GetType().Assembly.GetName().Name + ".system_characteristics." + xmlFile);
            
            var target = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);

            return target;            
        }

    }
}
