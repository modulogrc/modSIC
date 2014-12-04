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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Variables;
using Modulo.Collect.Service.Controllers;
using Modulo.Collect.Service.Tests.Entities;
using Modulo.Collect.Service.Tests.Helpers;
using Raven.Client;

namespace Modulo.Collect.Service.Tests
{
    [TestClass]
    public class VariableEvaluatorServiceTest
    {
        private const string FDCC_XPFIREWALL = "fdcc_xpfirewall_oval.xml";

        private LocalDataProvider DataProvider;

        
        protected IDocumentSession GetSession()
        {
            return DataProvider.GetSession();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            DataProvider = new LocalDataProvider();
        }


        [TestMethod, Owner("imenescal")]
        public void Should_be_possible_to_evaluate_variables_from_request_collect()
        {
            var session = GetSession();
            var collectRequest = 
                new CollectRequestFactory()
                    .CreateCollectRequestWithSpecificDefinitions(
                        session, 
                        "definitionsWithLocalVariable.xml");

            var evaluatedVariables = new VariableEvaluatorService().Evaluate(collectRequest, session);

            Assert.AreEqual(3, evaluatedVariables.GetQuantityOfVariables(), "The quantity of variables is not expected.");

            var variables = evaluatedVariables.GetVariableValueForVariableId("oval:org.mitre.oval:var:5000");
            Assert.AreEqual(2, variables.Count(), "The quantity of variable of id 5000 is not expected.");
            Assert.AreEqual("oval:org.mitre.oval:obj:5000", variables.ElementAt(0).OvalComponentId, "The variable oval component id is not expected.");
            Assert.AreEqual(1, variables.ElementAt(0).values.Count(), 1, "The quantity of values from variable of id 5000 is not expected.");
            Assert.AreEqual("CurrentType", variables.ElementAt(0).values.ElementAt(0),  "The value from variable of id 5000 is not expected.");

            variables = evaluatedVariables.GetVariableValueForVariableId("oval:org.mitre.oval:var:4000");
            Assert.AreEqual(1, variables.Count(), "The quantity of variable of id 4000 is not expected");
            Assert.AreEqual("oval:org.mitre.oval:obj:4000", variables.ElementAt(0).OvalComponentId, "The variable oval component id is not expected.");
            // not exists value because the system characteristics was not generate yet
            Assert.AreEqual(0, variables.ElementAt(0).values.Count(), "The quantity of values from variable of id 4000 is not expected");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variables()
        {
            var session = GetSession();
            string externalVariables = CreateOvalVariablesWithFakeValues();
            var fakeCollectRequest = 
                new CollectRequestFactory()
                    .CreateCompleteCollectRequestAfterCollectWithSystemCharacteristics(
                        session, 
                        string.Format(".definitions.{0}", FDCC_XPFIREWALL), 
                        ".system_characteristics.fdcc_xpfirewall_oval.sc.xml",
                        CreateOvalVariablesWithFakeValues());
            session.SaveChanges();
            var evaluatedVariables = new VariableEvaluatorService().Evaluate(fakeCollectRequest, session);

            Assert.IsNotNull(evaluatedVariables);
            Assert.AreEqual(23, evaluatedVariables.GetQuantityOfVariables());
            Assert.AreEqual("50001", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50001").Single().values.Single());
            Assert.AreEqual("50031", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50031").Single().values.Single());
            Assert.AreEqual("50041", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50041").Single().values.Single());
            Assert.AreEqual("50051", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50051").Single().values.Single());
            Assert.AreEqual("50071", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50071").Single().values.Single());
            Assert.AreEqual("50081", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50081").Single().values.Single());
            Assert.AreEqual("50091", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50091").Single().values.Single());
            Assert.AreEqual("50111", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50111").Single().values.Single());
            Assert.AreEqual("50131", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50131").Single().values.Single());
            Assert.AreEqual("50141", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50141").Single().values.Single());
            Assert.AreEqual("50151", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50151").Single().values.Single());
            Assert.AreEqual("50161", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50161").Single().values.Single());
            Assert.AreEqual("50171", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:50171").Single().values.Single());
            Assert.AreEqual("51001", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51001").Single().values.Single());
            Assert.AreEqual("51011", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51011").Single().values.Single());
            Assert.AreEqual("51031", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51031").Single().values.Single());
            Assert.AreEqual("51041", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51041").Single().values.Single());
            Assert.AreEqual("51051", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51051").Single().values.Single());
            Assert.AreEqual("51071", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51071").Single().values.Single());
            Assert.AreEqual("51081", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51081").Single().values.Single());
            Assert.AreEqual("51091", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51091").Single().values.Single());
            Assert.AreEqual("51111", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51111").Single().values.Single());
            Assert.AreEqual("51131", evaluatedVariables.GetVariableValueForVariableId("oval:gov.nist.fdcc.xpfirewall:var:51131").Single().values.Single());
        }


        private string CreateOvalVariablesWithFakeValues()
        {
            var ovalDefinitions = new OvalDocumentLoader().GetFakeOvalDefinitions(FDCC_XPFIREWALL);
            var externalVariables = ovalDefinitions.variables.OfType<VariablesTypeVariableExternal_variable>();

            var externalVariablesWithFakeValue = new oval_variables();
            foreach(var declaredExternalVariable in externalVariables)
            {
                var newFakeVariable = CreateVariableType(declaredExternalVariable);
                externalVariablesWithFakeValue.variables.Add(newFakeVariable);
            }

            return externalVariablesWithFakeValue.GetXmlDocument();
        }

        private OVAL.Variables.VariableType CreateVariableType(VariablesTypeVariableExternal_variable declaredExternalVariable)
        {
            var fakeVariableValue = declaredExternalVariable.id.Replace("oval:gov.nist.fdcc.xpfirewall:var:", string.Empty);
            return new OVAL.Variables.VariableType
            {
                id = declaredExternalVariable.id,
                comment = declaredExternalVariable.comment,
                datatype = declaredExternalVariable.datatype,
                value = new string[] { fakeVariableValue }.ToList()
            };
        }

    }
}
