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
using Modulo.Collect.OVAL.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
namespace Modulo.Collect.OVAL.Tests
{
    
    
    /// <summary>
    ///This is a test class for DocumentHelpersTest and is intended
    ///to contain all DocumentHelpersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DocumentHelpersTest
    {

        public TestContext TestContext { get; set; }
        

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetDefaultGenerator
        ///</summary>
        [TestMethod, Owner("mgaspar")]
        public void Test_Generator_Is_Created()
        {
            // Arrange
            DateTime dtJustBefore = DateTime.Now; System.Threading.Thread.Sleep(100);
            
            // Act
            GeneratorType generator = DocumentHelpers.GetDefaultGenerator();

            // Pre-Assert Arrange
            System.Threading.Thread.Sleep(100); DateTime dtJustAfter = DateTime.Now;
            
            // Assert
            Assert.IsNotNull(generator);
            Assert.IsTrue((generator.timestamp > dtJustBefore) && (generator.timestamp < dtJustAfter));
            Assert.IsFalse(string.IsNullOrEmpty(generator.product_name));
            Assert.IsFalse(string.IsNullOrEmpty(generator.product_version));
            Assert.IsTrue(generator.schema_version >= (decimal)5.6);
        }

        /// <summary>
        ///A test for GetDefaultGenerator
        ///</summary>
        [TestMethod, Owner("mgaspar")]
        public void Test_Add_Messages()
        {
            // Act
            List<MessageType> msgs = null;
            msgs = msgs.AddMessage(MessageLevelEnumeration.fatal, "Message A1");
            
            // Assert
            Assert.AreEqual(1,msgs.Count);
        }
    }
}
