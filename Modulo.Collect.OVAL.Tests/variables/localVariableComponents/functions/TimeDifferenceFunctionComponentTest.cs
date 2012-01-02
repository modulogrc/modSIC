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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Tests.variables.localVariableComponents.Builders;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Tests.helpers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.functions
{
    /// <summary>
    /// Summary description for TimeDifferenceFunctionComponentTest
    /// </summary>
    [TestClass]
    public class TimeDifferenceFunctionComponentTest
    {
        public TimeDifferenceFunctionComponentTest()
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
        private oval_definitions definitions;
        private oval_system_characteristics systemCharacteristics;
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            OvalDocumentLoader ovalDocument = new OvalDocumentLoader();
            definitions = ovalDocument.GetFakeOvalDefinitions("definitionsWithLocalVariable.xml");
            systemCharacteristics = ovalDocument.GetFakeOvalSystemCharacteristics("system_characteristics_with_local_variable.xml");
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_calculate_the_difference_between_dates_with_literal_component()
        {
            VariablesTypeVariableLocal_variable localVariable = 
                LocalVariableBuilder
                    .CreateTheLocalVariable()
                        .WithTimeDifference()
                            .WithFirstFormat(DateTimeFormatEnumeration.day_month_year)
                            .WithSecondFormat(DateTimeFormatEnumeration.day_month_year)
                            .AddLiteralComponent("25/09/2009 00:00:00")
                            .AddLiteralComponent("25/09/2009 00:00:10")
                            .SetInLocalVariable()
                    .Build();


            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics,definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            IEnumerable<string> values = component.GetValue();

            Assert.IsTrue(values.Count() > 0, "the quantity is not expected");
            Assert.IsTrue(values.ElementAt(0) == "10"); // in seconds
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_calculate_the_difference_between_dates_with_literal_component_with_differents_formats()
        {
            VariablesTypeVariableLocal_variable localVariable =
                LocalVariableBuilder
                    .CreateTheLocalVariable()
                        .WithTimeDifference()
                            .WithFirstFormat(DateTimeFormatEnumeration.day_month_year)
                            .WithSecondFormat(DateTimeFormatEnumeration.month_day_year)
                            .AddLiteralComponent("25/09/2009 00:00:00")
                            .AddLiteralComponent("09/25/2009 00:00:10")
                            .SetInLocalVariable()
                    .Build();


            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            IEnumerable<string> values = component.GetValue();

            Assert.IsTrue(values.Count() > 0, "the quantity is not expected");
            Assert.IsTrue(values.ElementAt(0) == "10"); // in seconds
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_calculate_the_difference_between_dates_with_literal_Component_with_seconds_since_epoch()
        {
            VariablesTypeVariableLocal_variable localVariable =
                LocalVariableBuilder
                    .CreateTheLocalVariable()
                        .WithTimeDifference()
                            .WithFirstFormat(DateTimeFormatEnumeration.year_month_day)
                            .WithSecondFormat(DateTimeFormatEnumeration.seconds_since_epoch)
                            .AddLiteralComponent("2009-09-03 17:43:12")
                            .AddLiteralComponent("1251996192")
                            .SetInLocalVariable()
                    .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            IEnumerable<string> values = component.GetValue();
            
            TimeDifferenceFormatter formatter = new TimeDifferenceFormatter();
            DateTime dateFromSeconds = formatter.GetDateInFormat("1251996192", DateTimeFormatEnumeration.seconds_since_epoch);
            DateTime firstDate = new DateTime(2009, 09, 03, 17, 43, 12);
            TimeSpan difference = dateFromSeconds - firstDate;


            Assert.IsTrue(values.Count() > 0, "the quantity is not expected");
            Assert.IsTrue(values.ElementAt(0) == difference.TotalSeconds.ToString()); // in seconds

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_calculate_the_difference_between_dates_with_objectComponent()
        {
            VariablesTypeVariableLocal_variable localVariable =
                LocalVariableBuilder
                    .CreateTheLocalVariable()
                        .WithTimeDifference()
                            .WithFirstFormat(DateTimeFormatEnumeration.day_month_year)
                            .WithSecondFormat(DateTimeFormatEnumeration.day_month_year)
                            .AddObjectComponent("value","oval:org.mitre.oval:obj:10000")
                            .AddObjectComponent("value","oval:org.mitre.oval:obj:10001")
                            .SetInLocalVariable()
                    .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            IEnumerable<string> values = component.GetValue();
            DateTime firstDate = new DateTime(2009, 10, 25, 0, 0, 0);
            DateTime secondDate = new DateTime(2009, 09, 25, 0, 0, 0);
            TimeSpan difference = firstDate - secondDate;

            Assert.IsTrue(values.Count() > 0, "the quantity is not expected");
            Assert.AreEqual(values.ElementAt(0),difference.TotalSeconds.ToString(), "the difference is not expected"); // in seconds
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_calculate_the_difference_between_dates_with_objectComponent_with_multiples_values()
        {
            VariablesTypeVariableLocal_variable localVariable =
                LocalVariableBuilder
                    .CreateTheLocalVariable()
                        .WithTimeDifference()
                            .WithFirstFormat(DateTimeFormatEnumeration.day_month_year)
                            .WithSecondFormat(DateTimeFormatEnumeration.day_month_year)
                            .AddObjectComponent("value", "oval:org.mitre.oval:obj:10001")
                            .AddObjectComponent("value", "oval:org.mitre.oval:obj:10002")
                            .SetInLocalVariable()
                    .Build();

            LocalVariableComponentsFactory factory = new LocalVariableComponentsFactory(systemCharacteristics, definitions.variables);
            LocalVariableComponent component = factory.GetLocalVariableComponent(localVariable);
            IEnumerable<string> values = component.GetValue();

            DateTime firstDate = new DateTime(2009, 10, 25, 0, 0, 0);
            DateTime secondDate = new DateTime(2009, 11, 25, 0, 0, 0);
            TimeSpan firstElement = secondDate - firstDate;
            secondDate = new DateTime(2009, 12, 25, 0, 0, 0);
            TimeSpan secondElement = secondDate - firstDate;

            Assert.IsTrue(values.Count() > 0, "the quantity is not expected");
            Assert.AreEqual(values.ElementAt(0), firstElement.TotalSeconds.ToString(), "the difference is not expected"); // in seconds
            Assert.AreEqual(values.ElementAt(1), secondElement.TotalSeconds.ToString(), "the difference is not expected"); // in seconds
        }


    }
}
