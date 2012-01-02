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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.EntityOperations;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions.Windows;
// using System.Management;



namespace Modulo.Collect.Probe.Common.Test
{

    [TestClass()]
    public class EntityOperationTest
    {
        private string DEFINITIONS_FILENAME = "fdcc_xpfirewall_oval_regex_on_value.xml";
        private string REGISTRY_OBJECT_ID = "oval:gov.nist.fdcc.xpfirewall:obj:50000";
        private string REGISTRY_KEY_NAME = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        private string MSG_INVALID_OPERATION_TYPE = "The Oval Entity Factory returns a invalid Entity Operation Type.";
        private string MSG_INVALID_OPERATION_RESULT_COUNT = "The operation generates an unexpected quantity of elements.";
        private string MSG_UNEXPECTED_FOUND_ITEM = "An unexpected item was found in operation result.";


        // ===========================================================================================================
        // ====================== Equals Operation Tests =============================================================
        // ===========================================================================================================
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_Equals_operation_on_RegistryObjectEntities()
        {
            registry_object registryObject = (registry_object)this.getFakeObj(DEFINITIONS_FILENAME, REGISTRY_OBJECT_ID);
            EntityObjectStringType keyEntity = this.getRegistryObjectEntity(registryObject, registry_object_ItemsChoices.key.ToString());


            // Assert Key Entity Operation
            OvalEntityOperationBase operationForKeyEntity = OvalEntityOperationFactory.CreateOperationForOvalEntity(keyEntity.operation);

            Dictionary<string, object> keyOperationParameters = this.getOperationParameters(keyEntity.Value, new string[] { REGISTRY_KEY_NAME });
            IEnumerable<string> derivedKeys = operationForKeyEntity.Apply(keyOperationParameters);
            Assert.IsInstanceOfType(operationForKeyEntity, typeof(EqualsEntityOperation), MSG_INVALID_OPERATION_TYPE);
            Assert.IsNotNull(derivedKeys);
            Assert.AreEqual(1, derivedKeys.Count(), MSG_INVALID_OPERATION_RESULT_COUNT);
            Assert.AreEqual(keyEntity.Value, derivedKeys.ElementAt(0), MSG_UNEXPECTED_FOUND_ITEM);
        }

        [TestMethod, Owner("lfernandes")]
        public void Equals_operation_should_be_CaseSensitive()
        {
            #region User Object
            // <user_object id="oval:modulo:obj:5" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //     <user>lfernandes</user>
            // </user_object>
            #endregion

            user_object userObject = (user_object)this.getFakeObj("definitionsSimple.xml", "oval:modulo:obj:5");
            EntityObjectStringType userEntity = (EntityObjectStringType)userObject.User;
            Dictionary<string, object> operationParameters;

            OvalEntityOperationBase equalsOperation = OvalEntityOperationFactory.CreateOperationForOvalEntity(userEntity.operation);

            operationParameters = this.getOperationParameters(userEntity.Value, new string[] { "LFernandes" });
            IEnumerable<string> operationResult = equalsOperation.Apply(operationParameters);
            Assert.IsInstanceOfType(equalsOperation, typeof(EqualsEntityOperation), MSG_INVALID_OPERATION_TYPE);
            Assert.AreEqual(0, operationResult.Count(), MSG_INVALID_OPERATION_RESULT_COUNT);
        }

        // ===========================================================================================================
        // ====================== Not Equal Operation Tests ==========================================================
        // ===========================================================================================================
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_NotEqual_operation_on_OvalObjectEntity()
        {
            string definitionFileName = "fdcc_xpfirewall_oval_regex_on_value.xml";
            registry_object obj50006 = (registry_object)this.getFakeObj(definitionFileName, "oval:modulo:obj:50006");
            Dictionary<string, EntityObjectStringType> entities = OvalHelper.GetRegistryEntitiesFromObjectType(obj50006);
            EntityObjectStringType nameEntity = entities[registry_object_ItemsChoices.name.ToString()];
            Dictionary<string, object> operationParameters = getOperationParameters("CSDVersion", new string[] { "BuildVersion", "CSDVersion", "XPTO" });

            OvalEntityOperationBase notEqualsOperation = OvalEntityOperationFactory.CreateOperationForOvalEntity(nameEntity.operation);
            IEnumerable<string> resultAfterApplyOperation = notEqualsOperation.Apply(operationParameters);

            Assert.IsInstanceOfType(notEqualsOperation, typeof(NotEqualsEntityOperation), MSG_INVALID_OPERATION_TYPE);
            Assert.AreEqual(2, resultAfterApplyOperation.Count(), MSG_INVALID_OPERATION_RESULT_COUNT);
            Assert.AreEqual("BuildVersion", resultAfterApplyOperation.ElementAt(0), MSG_UNEXPECTED_FOUND_ITEM);
            Assert.AreEqual("XPTO", resultAfterApplyOperation.ElementAt(1), MSG_UNEXPECTED_FOUND_ITEM);
        }

        [TestMethod, Owner("lfernandes")]
        public void NotEqual_operation_should_be_CaseSensitive()
        {
            #region Registry Object
            // <registry_object id="oval:modulo:obj:50006" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //   <hive>HKEY_LOCAL_MACHINE</hive>
            //   <key>SOFTWARE\Microsoft\Windows\CurrentVersion</key>
            //   <name operation="not equal">CSDVersion</name>
            // </registry_object>
            #endregion

            registry_object registryObject = (registry_object)this.getFakeObj(DEFINITIONS_FILENAME, "oval:modulo:obj:50006");
            Dictionary<string, EntityObjectStringType> registryEntities = OvalHelper.GetRegistryEntitiesFromObjectType(registryObject);
            EntityObjectStringType nameEntity = registryEntities[registry_object_ItemsChoices.name.ToString()];

            OvalEntityOperationBase NotEqualOperation = OvalEntityOperationFactory.CreateOperationForOvalEntity(nameEntity.operation);

            Dictionary<string, object> operationParameters = this.getOperationParameters(nameEntity.Value, new string[] { "CSDversion", "CSDVersion", "csdVersion" });
            IEnumerable<string> operationResult = NotEqualOperation.Apply(operationParameters);
            Assert.IsInstanceOfType(NotEqualOperation, typeof(NotEqualsEntityOperation), MSG_INVALID_OPERATION_TYPE);
            Assert.AreEqual(2, operationResult.Count(), MSG_INVALID_OPERATION_RESULT_COUNT);
            Assert.AreNotEqual(nameEntity.Value, operationResult.ElementAt(0), MSG_UNEXPECTED_FOUND_ITEM);
            Assert.AreNotEqual(nameEntity.Value, operationResult.ElementAt(1), MSG_UNEXPECTED_FOUND_ITEM);
        }



        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_PatternMatch_operation_on_OvalObjectEntity()
        {
            string definitionFileName = "fdcc_xpfirewall_oval_regex_on_value.xml";
            registry_object obj50002 = (registry_object)this.getFakeObj(definitionFileName, "oval:modulo:obj:50001");
            string keyPattern = "SOFTWARE\\Microsoft\\Windows\\^Current.*";
            string[] fakeSubKeys = new string[] { "XBuildX", "CurrentBuild", "CurrentBuildXPTO", "Build" };
            IList<string> fakeSystemValues = createFakeSystemValues("SOFTWARE\\Microsoft\\Windows", fakeSubKeys);
            Dictionary<string, object> operationParameters = getOperationParameters(keyPattern, fakeSystemValues.ToArray());

            PatternMatchEntityOperation patternOperation = new PatternMatchEntityOperation();
            IEnumerable<string> operationResult = patternOperation.Apply(operationParameters);

            Assert.IsNotNull(operationResult);
            Assert.AreEqual(2, operationResult.Count(), MSG_INVALID_OPERATION_RESULT_COUNT);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_CaseInsensitiveEqual_operation_on_OvalObjectEntity()
        {
            #region User Object
            // <user_object id="oval:modulo:obj:4" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //     <user operation="case insensitive equals">lfernandes</user>
            // </user_object>
            #endregion

            user_object userObject = (user_object)this.getFakeObj("definitionsSimple.xml", "oval:modulo:obj:4");
            EntityObjectStringType userEntity = (EntityObjectStringType)userObject.User;
            Dictionary<string, object> operationParameters;

            OvalEntityOperationBase caseInsensitiveEqualsOperation = OvalEntityOperationFactory.CreateOperationForOvalEntity(userEntity.operation);
            
            operationParameters = this.getOperationParameters(userEntity.Value, new string[] { "LFernandes", "LFERNANDES", userEntity.Value, "XPTO" });
            IEnumerable<string> operationResult = caseInsensitiveEqualsOperation.Apply(operationParameters);
            Assert.IsInstanceOfType(caseInsensitiveEqualsOperation, typeof(CaseInsentiveEqualsOperation), MSG_INVALID_OPERATION_TYPE);
            Assert.AreEqual(3, operationResult.Count(), MSG_INVALID_OPERATION_RESULT_COUNT);
            Assert.AreEqual("LFernandes", operationResult.ElementAt(0), MSG_UNEXPECTED_FOUND_ITEM);
            Assert.AreEqual("LFERNANDES", operationResult.ElementAt(1), MSG_UNEXPECTED_FOUND_ITEM);
            Assert.AreEqual(userEntity.Value, operationResult.ElementAt(2), MSG_UNEXPECTED_FOUND_ITEM);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_apply_CaseInsensitiveNotEqual_operation_on_OvalObjectEntity()
        {
            #region User Object
            //  <user_object id="oval:modulo:obj:6" version="1" xmlns="http://oval.mitre.org/XMLSchema/oval-definitions-5#windows">
            //      <user operation="case insensitive not equal">lfernandes</user>
            //  </user_object>
            #endregion

            user_object userObject = (user_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple.xml", "oval:modulo:obj:6");
            EntityObjectStringType userEntity = (EntityObjectStringType)userObject.User;
            Dictionary<string, object> operationParams = getOperationParameters(userEntity.Value, new string[] { "lfernandes", @"XPTO", @"lucaf", "LFERNANDES", "lFeRnAnDeS" });

            
            OvalEntityOperationBase operation = OvalEntityOperationFactory.CreateOperationForOvalEntity(userEntity.operation);
            Assert.IsInstanceOfType(operation, typeof(CaseInsensitiveNotEqualsOperation), MSG_INVALID_OPERATION_TYPE);


            IEnumerable<string> operationResult = operation.Apply(operationParams);
            Assert.AreEqual(2, operationResult.Count(), MSG_INVALID_OPERATION_RESULT_COUNT);
            Assert.AreEqual("XPTO", operationResult.ElementAt(0), MSG_UNEXPECTED_FOUND_ITEM);
            Assert.AreEqual("lucaf", operationResult.ElementAt(1), MSG_UNEXPECTED_FOUND_ITEM);
        }


        //[TestMethod]
        //public void TestFoo()
        //{
        //    string strComputer = ".";
        //    ManagementObjectSearcher wmiObjectSearcher = new ManagementObjectSearcher();
        //    wmiObjectSearcher.Scope = new ManagementScope("\\\\" + strComputer + "\\root\\directory\\LDAP");
        //    wmiObjectSearcher.Query = new ObjectQuery("Select * from ads_domaindns");
        //    wmiObjectSearcher.Options = new EnumerationOptions() { Rewindable = false, ReturnImmediately = true };

        //    string result = string.Empty;
        //    foreach (var obj in wmiObjectSearcher.Get())
        //        foreach (PropertyData property in obj.Properties)
        //        {
        //            string propertyValue = (property.Value == null) ? "[NULL]" : property.Value.ToString();
        //            result += string.Format("Property Name: '{0}' / Property Value: {1}\r\n", property.Name, propertyValue);
        //        }
        //    System.IO.File.WriteAllText("c:\\temp\\DomainDNS.txt", result);
            
        //    Assert.IsTrue(true);
        //}






        private EntityObjectStringType getRegistryObjectEntity(registry_object registryObject, string entityName)
        {
            Dictionary<string, EntityObjectStringType> entities = OvalHelper.GetRegistryEntitiesFromObjectType(registryObject);
            return entities[entityName];
        }

        private Dictionary<string, object> getOperationParameters(string entityValue, IList<string> valuesToApply)
        {
            Dictionary<string, object> operationParameters = new Dictionary<string, object>();
            operationParameters.Add(EntityOperationParameters.EntityValue.ToString(), entityValue);
            operationParameters.Add(EntityOperationParameters.ValuesToApply.ToString(), valuesToApply);
            return operationParameters;
        }

        private oval_definitions getFakeDefinitions(string fileName)
        {
            LoadOvalDocument docLoader = new LoadOvalDocument();
            return docLoader.GetFakeOvalDefinitions(fileName);
        }

        private IList<string> createFakeSystemValues(string keyEntityValue, string[] keyNameValues)
        {
            IList<string> fakeSystemValues = new List<string>();
            foreach(var keyNameValue in keyNameValues)
                ((List<string>)fakeSystemValues).Add(string.Format("{0}\\{1}", keyEntityValue, keyNameValue));
            
            return fakeSystemValues;
        }

        private ObjectType getFakeObj(string definitionsFileName, string objectID)
        {
            oval_definitions fakeDefs = this.getFakeDefinitions(definitionsFileName);
            return fakeDefs.objects.SingleOrDefault(obj => obj.id.Equals(objectID));
        }

        private Dictionary<string, object> getSearchValuesParameters(string keyName, string valueName, string entityValue)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("hiveName", "HKEY_LOCAL_MACHINE");
            parameters.Add("keyEntityName", keyName);

            if (!string.IsNullOrEmpty(valueName))
                parameters.Add("valueName", valueName);

            if (!string.IsNullOrEmpty(entityValue))
                parameters.Add("entityValue", entityValue);

            return parameters;
        }
    }
}
