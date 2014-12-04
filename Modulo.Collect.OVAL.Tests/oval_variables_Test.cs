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
namespace Modulo.Collect.OVAL.Tests
{
    
    /// <summary>
    ///This is a test class for oval_variablesTest and is intended
    ///to contain all oval_variablesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class oval_variablesTest
    {


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {get;set;}
        

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


        string samplexml1 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <oval-var:oval_variables xsi:schemaLocation=""http://oval.mitre.org/XMLSchema/oval-variables-5 oval-variables-schema.xsd"" xmlns:oval=""http://oval.mitre.org/XMLSchema/oval-common-5"" xmlns:oval-var=""http://oval.mitre.org/XMLSchema/oval-variables-5"" xmlns:n1=""http://www.altova.com/samplexml/other-namespace"" xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	                                <oval-var:generator>
		                                <oval:product_name>Risk Manager</oval:product_name>
		                                <oval:product_version>7.1</oval:product_version>
		                                <oval:schema_version>5.6</oval:schema_version>
		                                <oval:timestamp>2001-12-17T09:30:47Z</oval:timestamp>
	                                </oval-var:generator>
	                                <oval-var:variables>
		                                <oval-var:variable comment=""Text variable with single value"" datatype=""string"" id=""oval:-:var:1"">
			                                <oval-var:value>The book is on the table.</oval-var:value>
		                                </oval-var:variable>
		                                <oval-var:variable comment=""Text variable with multiple values"" datatype=""string"" id=""oval:-:var:2"">
			                                <oval-var:value>Red</oval-var:value>
			                                <oval-var:value>Green</oval-var:value>
			                                <oval-var:value>Lemon Green</oval-var:value>
		                                </oval-var:variable>
	                                </oval-var:variables>
                                </oval-var:oval_variables>";
        string samplexml2 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <oval-var:oval_variables xsi:schemaLocation=""http://oval.mitre.org/XMLSchema/oval-variables-5 oval-variables-schema.xsd"" xmlns:oval=""http://oval.mitre.org/XMLSchema/oval-common-5"" xmlns:oval-var=""http://oval.mitre.org/XMLSchema/oval-variables-5"" xmlns:n1=""http://www.altova.com/samplexml/other-namespace"" xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	                                <oval-var:generator>
		                                <oval:product_name>Risk Manager</oval:product_name>
		                                <oval:product_version>7.1</oval:product_version>
		                                <oval:schema_version>5.6</oval:schema_version>
		                                <oval:timestamp>2001-12-17T09:30:47Z</oval:timestamp>
	                                </oval-var:generator>
	                                <oval-var:variables>
		                                <oval-var:variable datatype=""string"" id=""oval:-:var:1"">
			                                <oval-var:value>The book is on the table.</oval-var:value>
		                                </oval-var:variable>
		                                <oval-var:variable comment=""Text variable with multiple values"" datatype=""string"" id=""oval:-:var:2"">
			                                <oval-var:value>Red</oval-var:value>
			                                <oval-var:value>Green</oval-var:value>
			                                <oval-var:value>Lemon Green</oval-var:value>
		                                </oval-var:variable>
	                                </oval-var:variables>
                                </oval-var:oval_variables>";

        [TestMethod, Owner("mgaspar")]
        public void Test_Load_Variables_Document()
        {
            IEnumerable<string> errors;
            oval_variables target = oval_variables.GetOvalVariablesFromText(samplexml1, out errors);

            Assert.IsNotNull(target);
            Assert.AreEqual(0, errors.Count());
            Assert.AreEqual(2, target.variables.Count);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Load_Invalid_Variables_Document()
        {
            IEnumerable<string> errors;
            oval_variables target = oval_variables.GetOvalVariablesFromText(samplexml2, out errors);

            Assert.IsNull(target);
            Assert.IsTrue(errors.Count() > 0);
        }

        [TestMethod, Owner("mgaspar")]
        public void Test_Create_Variables_Document()
        {
            // Arrange / Act
            oval_variables variables = new oval_variables();
            variables.variables.Add(new VariableType(SimpleDatatypeEnumeration.@string, "oval:XX:var:1", "Red", "Green"));
            variables.variables.Add(new VariableType(SimpleDatatypeEnumeration.boolean, "oval:XX:var:2", "true"));

            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(oval_variables));
            //StringBuilder sb = new StringBuilder();
            //StringWriter sw = new StringWriter(sb);
            //xmlSerializer.Serialize(sw, variables);
            
            IEnumerable<string> schemaErrors;
            oval_variables result = oval_variables.GetOvalVariablesFromText(variables.GetXmlDocument(), out schemaErrors);

            // Assert
            Assert.AreEqual(0, schemaErrors.Count(), string.Join(Environment.NewLine, schemaErrors.ToArray()));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.variables.Count);
        }

    }
}
