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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Windows.SystemInformationService;
using Modulo.Collect.Probe.Windows.Test.helpers;

namespace Modulo.Collect.Probe.Windows.Test.SystemInformationService
{
    /// <summary>
    /// Summary description for WindowsSystemInformationFactoryTest
    /// </summary>
    [TestClass]
    public class WindowsSystemInformationFactoryTest
    {
        public WindowsSystemInformationFactoryTest()
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
        public void Should_be_possible_to_create_a_system_information_with_data_about_the_hostName()
        {
            WmiDataProviderExpectFactory wmiFactory = new WmiDataProviderExpectFactory();

            WindowsSystemInformationFactory factory = new WindowsSystemInformationFactory();
            SystemInformation systemInformation = factory.CreateSystemInformation(
                                                        wmiFactory.GetWmiObjectsForComputerSystemQuery(),
                                                        wmiFactory.GetWmiObjectsForOperatingSystemQuery(),
                                                        wmiFactory.GetWmiObjectsForNetworkInterfaces());

            Assert.AreEqual("mss-rj-220.mss.modulo.com.br", systemInformation.PrimaryHostName, "the primaryHostName is not expected");
            Assert.AreEqual("INTEL32", systemInformation.Architecture, "the architecture is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_system_information_with_data_about_the_operationalSystem()
        {
            WmiDataProviderExpectFactory wmiFactory = new WmiDataProviderExpectFactory();

            WindowsSystemInformationFactory factory = new WindowsSystemInformationFactory();
            SystemInformation systemInformation = factory.CreateSystemInformation(
                                                        wmiFactory.GetWmiObjectsForComputerSystemQuery(),
                                                        wmiFactory.GetWmiObjectsForOperatingSystemQuery(),
                                                        wmiFactory.GetWmiObjectsForNetworkInterfaces());

            Assert.AreEqual("Microsoft Windows Server 2008 Enterprise SP2", systemInformation.SystemName, "the systemName is not expected");
            Assert.AreEqual("6.0.6002", systemInformation.SystemVersion, "the systemVersion is not expected");            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_create_a_system_information_with_data_about_the_networkInterfaces()
        {
            WmiDataProviderExpectFactory wmiFactory = new WmiDataProviderExpectFactory();

            WindowsSystemInformationFactory factory = new WindowsSystemInformationFactory();
            SystemInformation systemInformation = factory.CreateSystemInformation(
                                                        wmiFactory.GetWmiObjectsForComputerSystemQuery(),
                                                        wmiFactory.GetWmiObjectsForOperatingSystemQuery(),
                                                        wmiFactory.GetWmiObjectsForNetworkInterfaces());

            Assert.IsNotNull(systemInformation.Interfaces, "the interfaces is not created");
            Assert.AreEqual(systemInformation.Interfaces.Count,1, "the quantity of interfaces is not expected");
            Assert.AreEqual("172.16.3.166", systemInformation.Interfaces[0].IpAddress, "the ip address is not expected");
            Assert.AreEqual("00 - 23 - AE - B6 - 6F - BF", systemInformation.Interfaces[0].MacAddress, "the mac address is not expected");
            Assert.AreEqual("Intel(R) 82567LM-3 Gigabit Network Connection", systemInformation.Interfaces[0].Name, "the name is not expected");
        }
    }
}
