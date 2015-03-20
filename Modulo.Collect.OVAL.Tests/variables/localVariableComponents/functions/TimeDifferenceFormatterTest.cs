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
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;
using Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.functions
{
    /// <summary>
    /// Summary description for TimeDifferenceFormatterTest
    /// </summary>
    [TestClass]
    public class TimeDifferenceFormatterTest
    {
        public TimeDifferenceFormatterTest()
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
        public void Should_be_possible_to_get_a_date_in_year_month_day_format()
        {            
            string aDate = "2009/09/28 00:00:00";           
                
            TimeDifferenceFormatter formatter = new TimeDifferenceFormatter();
            DateTime formattedDate = formatter.GetDateInFormat(aDate,DateTimeFormatEnumeration.year_month_day);
            DateTime expectedDate = new DateTime(2009, 09, 28);
            Assert.AreEqual(expectedDate,formattedDate, "the date format is no expected.");

            aDate = "2009-09-03 17:43:12";
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.year_month_day);
            expectedDate = new DateTime(2009, 09, 03, 17,43,12);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");

            aDate = "2009-09-28";
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.year_month_day);
            expectedDate = new DateTime(2009, 09, 28);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");

            aDate = "2009/09/28";
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.year_month_day);
            expectedDate = new DateTime(2009, 09, 28);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_date_in_month_day_year()
        {
            string aDate = "09/29/2009 12:05:15";
            TimeDifferenceFormatter formatter = new TimeDifferenceFormatter();
            DateTime formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.month_day_year);
            DateTime expectedDate = new DateTime(2009, 09, 29,12,05,15);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");

            aDate = "09/29/2009";            
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.month_day_year);
            expectedDate = new DateTime(2009, 09, 29);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");

            aDate = "09-29-2009";
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.month_day_year);
            expectedDate = new DateTime(2009, 09, 29);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");

            aDate = "09-29-2009 15:10:10";
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.month_day_year);
            expectedDate = new DateTime(2009, 09, 29,15,10,10);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");
        }

        [TestMethod,Owner("lcosta")]
        public void Should_be_possible_to_get_a_date_in_day_month_year()
        {
            string aDate = "29/09/2009 12:05:15";
            TimeDifferenceFormatter formatter = new TimeDifferenceFormatter();
            DateTime formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.day_month_year);
            DateTime expectedDate = new DateTime(2009, 09, 29, 12, 05, 15);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");

            aDate = "29/09/2009";
            formatter = new TimeDifferenceFormatter();
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.day_month_year);
            expectedDate = new DateTime(2009, 09, 29);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");

            aDate = "29-09-2009";
            formatter = new TimeDifferenceFormatter();
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.day_month_year);
            expectedDate = new DateTime(2009, 09, 29);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");

            aDate = "29-09-2009 12:56:20";
            formatter = new TimeDifferenceFormatter();
            formattedDate = formatter.GetDateInFormat(aDate, DateTimeFormatEnumeration.day_month_year);
            expectedDate = new DateTime(2009, 09, 29,12,56,20);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_date_given_seconds_since_epoch()
        {            
            string secondsSinceEpoch = "10";
            TimeDifferenceFormatter formatter = new TimeDifferenceFormatter();
            DateTime formattedDate = formatter.GetDateInFormat(secondsSinceEpoch, DateTimeFormatEnumeration.seconds_since_epoch);         
            // the unix epoch is 01/01/1970 00:00:00
            DateTime expectedDate = new DateTime(1970, 01, 01, 00, 00,10);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_date_given_win_filetime()
        {
            string fileTime = "99900000000";
            //string fileTime = "111900000000";
            TimeDifferenceFormatter formatter = new TimeDifferenceFormatter();
            DateTime formattedDate = formatter.GetDateInFormat(fileTime, DateTimeFormatEnumeration.win_filetime);
            // the win_FileTime is 01/01/1601 00:00:00
            DateTime expectedDate = new DateTime(1601, 01, 01, 00, 46, 30);
            //DateTime expectedDate = new DateTime(1601, 01, 01, 00, 06, 30);
            Assert.AreEqual(expectedDate, formattedDate, "the date format is no expected.");
        }

        


    }
}
