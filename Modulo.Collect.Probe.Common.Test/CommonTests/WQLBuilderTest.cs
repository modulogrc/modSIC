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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.WMI;
using System.Text.RegularExpressions;

namespace Modulo.Collect.Probe.Common.Test
{
    [TestClass()]
    public class WQLBuilderTest
    {

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

        [TestMethod(), Owner("lfernandes")]
        public void Should_be_possible_to_build_a_WMI_Query_Language()
        {
            string wql = new WQLBuilder()
                .WithWmiClass("cim_logicalfile")
                    .AddParameter("drive", "c:")
                    .AddParameter("path", "\\temp\\")
                .Build().ToLower().Trim();


            List<String> wqlLines = this.GetSplittedWQL(wql);
            
            Assert.AreEqual(3, wqlLines.Count);
            Assert.AreEqual("select * from cim_logicalfile", wqlLines[0].ToLower().Trim());
            Assert.AreEqual("where drive = 'c:'", wqlLines[1].ToLower().Trim());
            Assert.AreEqual("and path = '\\temp\\'", wqlLines[2].ToLower().Trim());
        }


        [TestMethod(), Owner("lfernandes")]
        public void Should_be_possible_to_build_a_WMI_Query_Language_adding_parameters_before_WmiClass()
        {
            string wql = 
                new WQLBuilder()
                    .AddParameter("drive", "c:")
                    .AddParameter("path", "\\temp\\")
                        .WithWmiClass("cim_logicalfile")
                    .Build();

            var wqlLines = this.GetSplittedWQL(wql.ToLower().Trim());
            Assert.AreEqual(3, wqlLines.Count);
            Assert.AreEqual("select * from cim_logicalfile", wqlLines[0].ToLower().Trim());
            Assert.AreEqual("where drive = 'c:'", wqlLines[1].ToLower().Trim());
            Assert.AreEqual("and path = '\\temp\\'", wqlLines[2].ToLower().Trim());
        }

        [TestMethod(), Owner("lfernandes")]
        public void Should_be_possible_to_build_a_WMI_Query_Language_adding_all_parameters_at_once()
        {
            Dictionary<string, string> allParameters = new Dictionary<string, string>();
            allParameters.Add("drive", "d:");
            allParameters.Add("path", "\\fakedirectory\\");
            allParameters.Add("filename", "fakefile");
            allParameters.Add("extension", "fex");

            string wql = new WQLBuilder().WithWmiClass("cim_logicalfile").AddParameters(allParameters).Build();

            List<String> wqlLines = this.GetSplittedWQL(wql);
            Assert.AreEqual(5, wqlLines.Count);
            Assert.AreEqual("select * from cim_logicalfile", wqlLines[0].ToLower().Trim());
            Assert.AreEqual("where drive = 'd:'", wqlLines[1].ToLower().Trim());
            Assert.AreEqual("and path = '\\fakedirectory\\'", wqlLines[2].ToLower().Trim());
            Assert.AreEqual("and filename = 'fakefile'", wqlLines[3].ToLower().Trim());
            Assert.AreEqual("and extension = 'fex'", wqlLines[4].ToLower().Trim());
        }

        private List<String> GetSplittedWQL(string wql)
        {
            return new List<String>(wql.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
