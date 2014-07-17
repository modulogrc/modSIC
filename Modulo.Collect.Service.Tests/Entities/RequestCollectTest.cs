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
using System.Xml;
using System.IO;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Exceptions;
using Modulo.Collect.Service.Tests.Helpers;
using Raven.Client;


namespace Modulo.Collect.Service.Tests.Entities
{
    /// <summary>
    /// Summary description for CollectRequestTest
    /// </summary>
    [TestClass]
    public class CollectRequestTest
    {
        public CollectRequestTest()
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

        private LocalDataProvider dataprovider;
        [TestInitialize()]
         public void MyTestInitialize() 
         {
             dataprovider = new LocalDataProvider();
         }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_objectTypes_from_a_collectRequest()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            IEnumerable<ObjectType> ovalObjects = collectRequest.GetObjectTypes(fakeSession); 
            Assert.IsNotNull(ovalObjects);
            Assert.IsTrue(ovalObjects.Count() > 0, "the oval object is empty");
            Assert.IsTrue(ovalObjects.Count() == 47, "the oval object count is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_not_load_the_objectTypes_if_the_objectType_attribute_was_already_set()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequest(fakeSession).Item2;
            IEnumerable<ObjectType> ovalObjects = collectRequest.GetObjectTypes(fakeSession); 
            Assert.IsNotNull(ovalObjects);

            IEnumerable<ObjectType> sameObjectTypes = collectRequest.GetObjectTypes(fakeSession);
            Assert.AreSame(sameObjectTypes,ovalObjects, "the objectTypes were loaded again");
            
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(InvalidXMLOvalDefinitionsException))]
        public void Should_raise_an_exception_if_ovalDefinitions_xml_schema_is_invalid()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithInvalidDefinitions(fakeSession);
            IEnumerable<ObjectType> ovalObjects = collectRequest.GetObjectTypes(fakeSession);
        }

        [TestMethod, Owner("lcosta")]
        [ExpectedException(typeof(InvalidXMLOvalDefinitionsException))]
        public void Should_raise_an_exception_if_the_list_of_errors_of_the_schema_validation_is_not_empty()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithInvalidSchemaDefinitions(fakeSession);
            IEnumerable<ObjectType> ovalObjects = collectRequest.GetObjectTypes(fakeSession);        
        }
            

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_number_of_times_that_was_executed_the_collectRequest()
        {
            CollectRequestFactory factory = new CollectRequestFactory();
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = factory.CreateCollectRequest(fakeSession).Item2;
            Assert.IsTrue(collectRequest.GetNumberOfExecutions(fakeSession) == 0, "this request collect still not executed");

            //add two executions 
            CollectExecution firstExecution = factory.CreateCollect(fakeSession);
            firstExecution.RequestId = collectRequest.Oid;
            firstExecution.ProbeExecutions.Add(factory.CreateProbeExecution(fakeSession, "registry" ));
            firstExecution.ProbeExecutions.Add(factory.CreateProbeExecution(fakeSession, "family"));

            CollectExecution secondExecution = factory.CreateCollect(fakeSession);
            secondExecution.RequestId = collectRequest.Oid;
            secondExecution.ProbeExecutions.Add(factory.CreateProbeExecution(fakeSession, "registry"));
            fakeSession.SaveChanges();
            Assert.AreEqual(2, collectRequest.GetNumberOfExecutions(fakeSession));
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_save_a_request_collect_without_system_characteristics_defined()
        {
            IDocumentSession session = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequest(session).Item2;
            Assert.IsNull(collectRequest.Result, "the systemcharacteristics is not null");

            //collectRequest.Save();
            session.SaveChanges();
            CollectRequest newCollectRequest = session.Load<CollectRequest>(collectRequest.Oid.ToString());  

            Assert.IsNull(collectRequest.Result, "the result is null");           
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_all_system_characteristcs_objects_of_collectRequest()
        {
            var session = GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestCompleteAfterCollect(session);
            session.SaveChanges();
            IEnumerable<Modulo.Collect.OVAL.SystemCharacteristics.oval_system_characteristics> systemCharacteristics 
                = collectRequest.GetExecutedSystemCharacteristics(session);

            Assert.IsNotNull(systemCharacteristics, "the system characteristics is null");
            Assert.AreEqual(1, systemCharacteristics.Count());

        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_variables_from_a_collectRequest()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithDefinitionsWithVariables(fakeSession);
            IEnumerable<VariableType> ovalVariables = collectRequest.GetOvalVariables(fakeSession);
            Assert.IsNotNull(ovalVariables);
            Assert.IsTrue(ovalVariables.Count() > 0, "the oval object is empty");
            Assert.IsTrue(ovalVariables.Count() == 3, "the oval object count is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_not_load_the_variables_if_the_variable_attribute_was_already_set()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithDefinitionsWithVariables(fakeSession);
            IEnumerable<VariableType> ovalVariables = collectRequest.GetOvalVariables(fakeSession);
            Assert.IsNotNull(ovalVariables);

            IEnumerable<VariableType> variables = collectRequest.GetOvalVariables(fakeSession);
            Assert.AreSame(variables, ovalVariables, "the variables were loaded again");
        }

        [TestMethod, Owner("lcosta")]
        public void should_not_return_a_null_list_of_variables_if_the_CollectRequest_not_have_variables_defined()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithSpecificDefinitions(fakeSession, "definitionsSimple.xml");
            IEnumerable<VariableType> ovalVariables = collectRequest.GetOvalVariables(fakeSession);
            Assert.IsNotNull(ovalVariables);
            Assert.IsTrue(ovalVariables.Count() == 0, "the oval object is not empty");            
        }            

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_all_objectTypes_that_have_references_for_variableId_informed()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithSpecificDefinitions(fakeSession, "definitionsWithLocalVariable.xml");
            IEnumerable<ObjectType> objectTypesOfVariable = collectRequest.GetObjectTypesByVariableId(fakeSession, "oval:org.mitre.oval:var:4000");
            Assert.IsTrue(objectTypesOfVariable.Count() == 1, "the quantity of objectTypes is not Expected");

            objectTypesOfVariable = collectRequest.GetObjectTypesByVariableId(fakeSession, "oval:org.mitre.oval:var:4005");
            Assert.IsTrue(objectTypesOfVariable.Count() == 0, "the quantity of objectTypes is not Expected");
        }

        [TestMethod,Owner("lcosta")]
        public void Should_not_be_possible_to_close_an_request_collect_if_not_collected_all_objects_of_the_definitions()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestIncompleteAfterCollect(fakeSession);
            Assert.AreEqual(true,collectRequest.isOpen(),"the initial state of collectRequest is not expected.");
            collectRequest.TryClose(fakeSession);
            Assert.AreEqual(true, collectRequest.isOpen(), "the state of collectRequest is no expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_the_objects_that_still_was_not_collected()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestIncompleteAfterCollect(fakeSession);
            fakeSession.SaveChanges();
            IEnumerable<ObjectType> objectTypes = collectRequest.GetObjectTypesWasNotCollected(fakeSession);
            Assert.AreEqual(2, objectTypes.Count(), "the quantity of objects is not expected");

            ObjectType objectType = objectTypes.Where<ObjectType>(obj => obj.id == "oval:gov.nist.fdcc.xpfirewall:obj:50111").SingleOrDefault();
            Assert.IsNotNull(objectType, "the object is not expected");
            objectType = objectTypes.Where<ObjectType>(obj => obj.id == "oval:gov.nist.fdcc.xpfirewall:obj:51062").SingleOrDefault();
            Assert.IsNotNull(objectType, "the object is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_the_states_of_a_collectRequest()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithSpecificDefinitions(fakeSession, "definitionsWithLocalVariable.xml"); 
            IEnumerable<StateType> states = collectRequest.GetStates(fakeSession);
            Assert.IsNotNull(states);
            Assert.AreEqual(5, states.Count(), "the quantity of objects is not expected" );
            StateType state = states.Where(obj => obj.id == "oval:org.mitre.oval:ste:3794").SingleOrDefault();
            Assert.IsNotNull(state);
        }
   
        [TestMethod, Owner("lcosta")]
        public void Should_not_load_the_states_if_the_state_attribute_was_already_set()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithSpecificDefinitions(fakeSession, "definitionsWithLocalVariable.xml"); 
            IEnumerable<StateType> states = collectRequest.GetStates(fakeSession);
            Assert.IsNotNull(states);
            Assert.AreEqual(5, states.Count(), "the quantity of objects is not expected" );
            IEnumerable<StateType> sameState = collectRequest.GetStates(fakeSession);
            Assert.AreSame(sameState, states, "the states were loaded again");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_all_StateType_that_have_references_for_variableId_informed()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestWithSpecificDefinitions(fakeSession, "definitionsWithLocalVariable.xml");
            IEnumerable<StateType> stateTypeOfVariables = collectRequest.GetStateTypeByVariableId(fakeSession, "oval:org.mitre.oval:var:5000");
            Assert.IsTrue(stateTypeOfVariables.Count() == 1, "the quantity of objectTypes is not Expected");

            stateTypeOfVariables = collectRequest.GetStateTypeByVariableId(fakeSession, "oval:org.mitre.oval:var:5001");
            Assert.IsTrue(stateTypeOfVariables.Count() == 0, "the quantity of objectTypes is not Expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_get_a_complete_log_of_collect()
        {
            IDocumentSession fakeSession = this.GetSession();
            CollectRequest collectRequest = new CollectRequestFactory().CreateCollectRequestIncompleteAfterCollect(fakeSession);
            IEnumerable<CollectExecutionLog> completeLog = collectRequest.GetExecutionLog(fakeSession);
            Assert.IsTrue(completeLog.Count() > 0, "the quantity of executionLogs is not expected");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_check_that_all_object_types_is_being_referenced_by_variable()
        {
            var fakeSession = this.GetSession();
            var collectRequest = new CollectRequestFactory().CreateCollectRequestWithSpecificDefinitions(fakeSession, "public.microsoft.windows.xp_only_objects.xml");
            foreach (var @object in collectRequest.GetObjectTypes(fakeSession))
            {
                try
                {
                    @object.HasReferenceForVariable("");
                }
                catch (Exception ex)
                {
                    Assert.Fail("It was not possible to evaluate {0} type. Error: {1}({2})", @object.ComponentString, ex.Message, ex.GetType().FullName);
                }
            }
        }

        private IDocumentSession GetSession()
        {
            return dataprovider.GetSession();

        }

       

    }
}
