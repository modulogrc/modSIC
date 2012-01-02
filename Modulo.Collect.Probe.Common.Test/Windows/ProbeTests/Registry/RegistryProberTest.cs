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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Exceptions;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Windows.Registry;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.Probe.Windows.Test.Helpers;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Interfaces;
using Definitions = Modulo.Collect.OVAL.Definitions;
using SystemCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.Windows;




namespace Modulo.Collect.Probe.Windows.Test
{
    /// <summary>
    ///This is a test class for RegistryProberTest and is intended
    ///to contain all RegistryProberTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RegistryProberTest
    {
        [TestInitialize()]
        public void MyTestInitialize()
        {
            fakeContext = ProbeHelper.CreateFakeContext();
            fakeTarget = ProbeHelper.CreateFakeTarget();
        }


        private TargetInfo fakeTarget;
        private List<IConnectionProvider> fakeContext;

        private const string OBJ_3000_ID = "oval:org.mitre.oval:obj:3000";
        private const string VAR_3000_ID = "oval:org.mitre.oval:var:3000";

        
        [TestMethod, Owner("lfernandes")]
        public void Should_Be_Possible_To_Execute_A_Default_Registry_Collect()
        {
            // Arrange
            oval_definitions fakeDefinitions = ProbeHelper.GetFakeOvalDefinitions("fdcc_xpfirewall_oval.xml");
            CollectInfo fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(fakeDefinitions.objects.OfType<registry_object>().ToArray(), null, null);
            registry_item fakeItem = this.GetFakeRegistryItem("Software\\Modulo", "NG", eValueTypes.STRING, "Secret");
            List<registry_item> expectedRegistryItems = new List<registry_item>(new registry_item[] { fakeItem });
            
            RegistryProber registryProber = this.GetMockedRegistryProber(fakeItem);
            
            
            // Act
            IEnumerable<CollectedObject> result = registryProber.Execute(this.fakeContext, this.fakeTarget, fakeCollectInfo).CollectedObjects;
            
            // Assert
            Assert.AreEqual(46, result.Count());
            this.AssertProbeResultItem(result.ElementAt(0), fakeDefinitions.objects[1].id, null, expectedRegistryItems);
            this.AssertProbeResultItem(result.ElementAt(10), fakeDefinitions.objects[11].id, null, expectedRegistryItems);
            this.AssertProbeResultItem(result.ElementAt(45), fakeDefinitions.objects[46].id, null, expectedRegistryItems);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_execute_a_registry_collect_with_variables()
        {
            // Arrange
            string[] fakeVarValues = new string[] { @"Software\Microsoft\Windows NT\CurrentVersion" };
            oval_definitions fakeDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsWithConstantVariable.xml");
            var fakeObjects = fakeDefinitions.objects.OfType<registry_object>().ToArray();
            VariablesEvaluated variables = VariableHelper.CreateVariableWithMultiplesValue(OBJ_3000_ID, VAR_3000_ID, fakeVarValues);
            CollectInfo fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(fakeObjects, variables, null);
            registry_item fakeRegistryItem = this.GetFakeRegistryItem(@"Software\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", eValueTypes.STRING, "6.0");
            RegistryProber registryProber = this.GetMockedRegistryProber(fakeRegistryItem);
            
            // Act
            IEnumerable<CollectedObject> result = registryProber.Execute(fakeContext, fakeTarget, fakeCollectInfo).CollectedObjects;

            // Assert
            Assert.AreEqual(1, result.Count(), "the quantity is not expected");
            this.AssertProbeResultItem(result.ElementAt(0), fakeDefinitions.objects[1].id, fakeVarValues, new List<registry_item>() { fakeRegistryItem });
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_execute_a_registry_collect_with_Set_element()
        {
            var fakeDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsWithSet.xml");
            var fakeObjects = fakeDefinitions.objects.OfType<registry_object>().ToArray();
            oval_system_characteristics fakeSystemCharacteristics = new LoadOvalDocument().GetFakeOvalSystemCharacteristics("system_characteristics_with_sets.xml");
            CollectInfo fakeCollectedInfo = ProbeHelper.CreateFakeCollectInfo(fakeObjects, null, fakeSystemCharacteristics);
            registry_item fakeRegistryItem = this.GetFakeRegistryItem(@"Software\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", eValueTypes.STRING, "6.0");            
            RegistryProber registryProber = this.GetMockedRegistryProber(fakeRegistryItem);


            IEnumerable<CollectedObject> result = registryProber.Execute(fakeContext, fakeTarget, fakeCollectedInfo).CollectedObjects;
            

            Assert.IsNotNull(result, "the result is not expected");
            CollectedObject collectedObject = result.Where<CollectedObject>(obj => obj.ObjectType.id == "oval:org.mitre.oval:obj:6000").SingleOrDefault();
            Assert.IsNotNull(collectedObject, "the element was not found");
            Assert.AreEqual(collectedObject.ObjectType.reference.Count(),3, "the quantity of object referenced is not expected");
        }

        [TestMethod(), Owner("lcosta")]
        [ExpectedException(typeof(InvalidCredentialsException))]
        public void Should_Not_Be_Possible_To_Execute_A_Registry_Collect_If_The_Credentials_Are_Invalid()
        {
            IConnectionManager connectionManagerWithInvalidCredentials = GetConnectionMangerWithInvalidCredentialBehavior();
            RegistryProber probe = new RegistryProber();
            probe.ConnectionManager = connectionManagerWithInvalidCredentials;

            IList<registry_object> registryObjects = new List<registry_object>();
            oval_definitions fakeDefinitions = ProbeHelper.GetFakeOvalDefinitions("fdcc_xpfirewall_oval.xml");
            
            ProbeResult collectedItems = probe.Execute(
                ProbeHelper.CreateFakeContext(), ProbeHelper.CreateFakeTarget(),  new CollectInfo() { ObjectTypes = fakeDefinitions.objects });            
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(CannotConnectToHostException))]
        public void Should_Not_Be_Possible_To_Execute_A_Registry_Collect_If_The_Host_is_Unavailable()
        {
            IConnectionManager connectionManagerWithInvalidCredentials = GetConnectionMangerWithUnavailableBehavior();
            RegistryProber probe = new RegistryProber();
            probe.ConnectionManager = connectionManagerWithInvalidCredentials;

            IList<registry_object> registryObjects = new List<registry_object>();
            oval_definitions fakeDefinitions = ProbeHelper.GetFakeOvalDefinitions("fdcc_xpfirewall_oval.xml");
            
            ProbeResult probeResult = probe.Execute(
              ProbeHelper.CreateFakeContext(), ProbeHelper.CreateFakeTarget(), new CollectInfo() { ObjectTypes = fakeDefinitions.objects });            
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_Be_Possible_To_Extract_From_OVAL_Definitions_Objects_That_Not_Contains_A_Test_Or_DefinitionAssociated()
        {
            oval_definitions fakeDefinitions = ProbeHelper.GetFakeOvalDefinitions("definitionsWithOnlyObjects.xml");
            
            Assert.IsNotNull(fakeDefinitions.objects);
            Assert.AreEqual(6, fakeDefinitions.objects.Count());
            Assert.IsNotNull(fakeDefinitions.objects.SingleOrDefault(obj => obj.id.Equals("oval:gov.nist.fdcc.xpfirewall:obj:50000")));

            registry_object obj50001 = (registry_object)fakeDefinitions.objects.SingleOrDefault(obj => obj.id.Equals("oval:gov.nist.fdcc.xpfirewall:obj:50001"));
            Assert.IsNotNull(obj50001);
            
            string foundHiveName = ((EntityObjectRegistryHiveType)obj50001.Items[0]).Value;
            string foundKeyName = ((EntityObjectStringType)obj50001.Items[1]).Value;
            EntityObjectStringType foundValueEntity = (EntityObjectStringType)obj50001.Items[2];
            
            Assert.AreEqual("HKEY_LOCAL_MACHINE", foundHiveName);
            Assert.AreEqual("SOFTWARE\\Microsoft\\Windows\\CurrentVersion", foundKeyName);
            Assert.AreEqual(OperationEnumeration.patternmatch, foundValueEntity.operation);
            Assert.AreEqual(".*Build$", foundValueEntity.Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned()
        {
            var objectType = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", OBJ_3000_ID);
            var fakeCollectInfo = ProbeHelper.CreateFakeCollectInfo(new Definitions.ObjectType[] { objectType }, null, null);
            var fakeException = new Exception("Registry Access Denied");
            var registryProber = this.CreateMockedProberWithGetItemsErrorBehavior(fakeException);

            var proberResult = registryProber.Execute(fakeContext, fakeTarget, fakeCollectInfo);

            Assert.IsNotNull(proberResult, "The result of probe execution cannot be null.");
            Assert.AreEqual(1, proberResult.CollectedObjects.Count());
            
            var collectedItems = proberResult.CollectedObjects.Single().SystemData;
            WindowsTestHelper.AssertItemWithErrorStatus(collectedItems, typeof(registry_item), fakeException.Message);
        }


        private RegistryProber CreateMockedProberWithGetItemsErrorBehavior(Exception fakeExceptionToThrow)
        {
            MockRepository mocks = new MockRepository();
            var fakeConnectionManager = mocks.DynamicMock<IConnectionManager>();
            var fakeItemTypeGenerator = mocks.DynamicMock<RegistryItemTypeGenerator>();
            var fakeSystemDataSource = mocks.DynamicMock<RegistryObjectCollector>();

            Expect.Call(fakeItemTypeGenerator.GetItemsToCollect(null, null)).IgnoreArguments().Throw(fakeExceptionToThrow);
            Expect.Call(fakeSystemDataSource.CollectDataForSystemItem(null)).IgnoreArguments().CallOriginalMethod(OriginalCallOptions.NoExpectation);
            
            mocks.ReplayAll();

            
            return new RegistryProber() { ConnectionManager = fakeConnectionManager, ItemTypeGenerator = fakeItemTypeGenerator, ObjectCollector = fakeSystemDataSource };
        }

        private RegistryProber GetMockedRegistryProber(registry_item fakeItem)
        {
            var fakeValues = new List<String>(new string[] { "FakeValue" });
            var fakeCollectedItems = new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(fakeItem) };
            
            MockRepository mocks = new MockRepository();
                var fakeConnection = mocks.DynamicMock<IConnectionManager>();
                var fakeSystemInformation = mocks.DynamicMock<ISystemInformationService>();
                var fakeProvider = mocks.DynamicMock<RegistryConnectionProvider>();
                var fakeWmiProvider = new WmiDataProviderExpectFactory().GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess();
                var fakeDataCollector = mocks.DynamicMock<RegistryObjectCollector>();
                fakeDataCollector.WmiDataProvider = fakeWmiProvider;
                var  registryItemTypeGeneration = new RegistryItemTypeGenerator() { SystemDataSource = fakeDataCollector, WmiDataProvider = fakeWmiProvider };

                Expect.Call(fakeConnection.Connect<RegistryConnectionProvider>(null, null)).IgnoreArguments().Repeat.Any().Return(fakeProvider);
                Expect.Call(fakeDataCollector.CollectDataForSystemItem(fakeItem)).IgnoreArguments().Repeat.Any().Return(fakeCollectedItems);
                Expect.Call(fakeDataCollector.GetValues(null)).IgnoreArguments().Repeat.Any().Return(fakeValues);
                Expect.Call(fakeSystemInformation.GetSystemInformationFrom(null)).IgnoreArguments().Return(SystemInformationFactory.GetExpectedSystemInformation());
            mocks.ReplayAll();

            return new RegistryProber() { ConnectionManager = fakeConnection, ObjectCollector = fakeDataCollector, ItemTypeGenerator = registryItemTypeGeneration};
        }

        private registry_item GetFakeRegistryItem(string key, string name, eValueTypes dataType, string dataValue)
        {
            string hive = Enum.GetName(typeof(eHiveNames), eHiveNames.HKEY_LOCAL_MACHINE);
            string keyCollectedSuccessfully = "The Key, which fullPath is '{0}\\{1}\\{2}', was collected sucessfully.";

            registry_item registryItem = new registry_item();
            registryItem.hive = new EntityItemRegistryHiveType() { Value = hive };
            registryItem.key = new EntityItemStringType() { Value = key };
            registryItem.name = new EntityItemStringType() { Value = name };
            registryItem.type = new EntityItemRegistryTypeType() { Value = RegistryHelper.GetValueTypeAsString(dataType) };
            registryItem.value = new EntityItemAnySimpleType[] { new EntityItemAnySimpleType() { Value = dataValue } };

            registryItem.status = StatusEnumeration.exists;
            registryItem.message = MessageType.FromString(string.Format(keyCollectedSuccessfully, hive, key, name));

            return registryItem;
        }

        private void AssertProbeResultItem(CollectedObject collectedObject, string objectID, string[] expectedVariables, List<registry_item> expectedItems)
        {
            SystemCharacteristics::ObjectType createdObjectType = collectedObject.ObjectType;
            List<ItemType> createdItemsType = (List<ItemType>)collectedObject.SystemData;

            Assert.AreEqual(objectID, createdObjectType.id);
            if (expectedVariables != null)
            {
                Assert.IsNotNull(createdObjectType.variable_value, "There is no variable values for this collected object");
                Assert.AreEqual(expectedVariables.Count(), createdObjectType.variable_value.Count(), "The number of variable values for this collected object is unexpected.");
                for (int i = 0; i < expectedVariables.Count(); i++)
                    Assert.AreEqual(expectedVariables[i], createdObjectType.variable_value[i].Value, "An unexpected variable value was found.");
            }


            Assert.AreEqual(expectedItems.Count, createdObjectType.reference.Count());
            Assert.AreEqual(expectedItems.Count, createdItemsType.Count);

            for (int i = 0; i < createdItemsType.Count; i++)
            {
                registry_item createdRegistryItem = (registry_item)createdItemsType[i];

                Assert.AreEqual(expectedItems[i].status, createdRegistryItem.status);
                Assert.AreEqual(expectedItems[i].message.First().Value, createdRegistryItem.message.First().Value);

                Assert.AreEqual(expectedItems[i].hive.Value, createdRegistryItem.hive.Value);
                Assert.AreEqual(expectedItems[i].key.Value, createdRegistryItem.key.Value);
                Assert.AreEqual(expectedItems[i].name.Value, createdRegistryItem.name.Value);
                Assert.AreEqual(expectedItems[i].type.Value, createdRegistryItem.type.Value);

                Assert.AreEqual(expectedItems[i].value.Count(), createdRegistryItem.value.Count());
                for (int j = 0; j < expectedItems[i].value.Count(); j++)
                    Assert.AreEqual(expectedItems[i].value[j].Value, createdRegistryItem.value[j].Value);
            }
        }

        private IConnectionManager GetConnectionMangerWithUnavailableBehavior()
        {
            TargetInfo fakeTargetInfo = ProbeHelper.CreateFakeTarget();
            IList<IConnectionProvider> fakeContext = ProbeHelper.CreateFakeContext();
            string errorMessage = string.Format("Error connecting to {0}", fakeTargetInfo.GetAddress());

            MockRepository mock = new MockRepository();
            IConnectionManager connectionManagerWithInvalidCredentials = mock.DynamicMock<IConnectionManager>();
            Expect.Call(connectionManagerWithInvalidCredentials.Connect<RegistryConnectionProvider>(fakeContext, fakeTargetInfo))
                .IgnoreArguments().Throw(new CannotConnectToHostException(fakeTargetInfo, errorMessage));

            mock.ReplayAll();
            return connectionManagerWithInvalidCredentials;

        }

        private IConnectionManager GetConnectionMangerWithInvalidCredentialBehavior()
        {
            TargetInfo fakeTargetInfo = ProbeHelper.CreateFakeTarget();
            IList<IConnectionProvider> fakeContext = ProbeHelper.CreateFakeContext();
            string errorMessage = string.Format("Error connecting to {0}", fakeTargetInfo.GetAddress());

            MockRepository mock = new MockRepository();
            IConnectionManager connectionManagerWithInvalidCredentials = mock.DynamicMock<IConnectionManager>();
            Expect.Call(connectionManagerWithInvalidCredentials.Connect<RegistryConnectionProvider>(fakeContext, fakeTargetInfo))
                .IgnoreArguments().Throw(new InvalidCredentialsException(fakeTargetInfo.credentials, errorMessage));

            mock.ReplayAll();

            return connectionManagerWithInvalidCredentials;
        }
    }
}
