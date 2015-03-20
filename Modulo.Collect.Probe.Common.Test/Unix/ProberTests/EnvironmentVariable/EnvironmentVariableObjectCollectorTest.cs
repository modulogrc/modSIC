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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Unix.EnvironmentVariable;
using Modulo.Collect.Probe.Common.BasicClasses;
using Rhino.Mocks;
using Modulo.Collect.Probe.Unix.SSHCollectors;

namespace Modulo.Collect.Probe.Unix.Test.ProberTests.EnvironmentVariable
{
    [TestClass]
    public class EnvironmentVariableObjectCollectorTest
    {
        private const string FAKE_VARIABLE_NAME = "PATH";
        private const string FAKE_VARIABLE_VALUE = "/home/cpaiva:/bin/usr/bin";


        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_to_collect_a_environmentvariable_item()
        {
            // Arrange
            var fakeEnvironmentVariableItem = this.CreateFakeEnvironmentVariableItem(FAKE_VARIABLE_NAME);
            var fakeEnvironVarsToReturn = this.CreateFakeEnvironmentVariables();
            var objectCollector = this.CreateMockedObjectCollector(fakeEnvironVarsToReturn);
 
            // Act
            var collectedItems = objectCollector.CollectDataForSystemItem(fakeEnvironmentVariableItem);

            // Asserts
            var collectedItem = (environmentvariable_item)collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.exists, collectedItem.status);
            AssertEnvironmentVariableItemEntity(collectedItem.name, FAKE_VARIABLE_NAME);
            AssertEnvironmentVariableItemEntity(collectedItem.value, FAKE_VARIABLE_VALUE);
        }

        private void AssertEnvironmentVariableItemEntity(EntityItemSimpleBaseType entityToAssert, string expectedValue)
        {
            Assert.IsNotNull(entityToAssert);
            Assert.IsFalse(string.IsNullOrEmpty(entityToAssert.Value));
            Assert.AreEqual(entityToAssert.Value, expectedValue);
        }

        private environmentvariable_item CreateFakeEnvironmentVariableItem(string fakeVariable)
        {
            return new environmentvariable_item() 
            {
                name = OvalHelper.CreateItemEntityWithStringValue(fakeVariable)
            };               
        }

        private Dictionary<string, string> CreateFakeEnvironmentVariables()
        {
            var fakeEnvironmentVariables = new Dictionary<string, string>();
            fakeEnvironmentVariables.Add("BASH", "/bin/bash");
            fakeEnvironmentVariables.Add(FAKE_VARIABLE_NAME, FAKE_VARIABLE_VALUE);
            fakeEnvironmentVariables.Add("COLORS", "/etc/DIR_COLORS");
            
            return fakeEnvironmentVariables;
        }

        private EnvironmentVariableObjectCollector CreateMockedObjectCollector(
            Dictionary<string, string> fakeEnvironmentVariables)
        {
            MockRepository mocks = new MockRepository();
            var fakeEnvironVarsCollector = mocks.DynamicMock<EnvironmentVariableCollector>();
            Expect.Call(fakeEnvironVarsCollector.GetTargetEnvironmentVariables()).Return(fakeEnvironmentVariables);
            mocks.ReplayAll();

            return new EnvironmentVariableObjectCollector() { EnvironmentVariablesCollector = fakeEnvironVarsCollector };
        }
    }
}
