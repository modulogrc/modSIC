using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Client.Internal;
using Rhino.Mocks;
using Modulo.Collect.Service.Contract;

namespace Modulo.Collect.ClientConsole.Tests
{
    [TestClass]
    public class ModSicServiceTests
    {
        private const string EXPECTED_EXCEPTION_WAS_NOT_THROWN = "An expected exception was not thrown.";
        private const string UNEXPECTED_COLLECTION_IN_EXECUTION_WAS_FOUND = "Unexpected collection in execution was found.";

        private ModSicServiceMocker ModSicServiceMocker;
        private string FakeTargetAddress;
        private string FakeCredentials;
        private string FakeDefinitions;

        public ModSicServiceTests()
        {
            this.ModSicServiceMocker = new ModSicServiceMocker();
            this.FakeTargetAddress = ModSicServiceMocker.FAKE_TARGET_ADDRESS;
            this.FakeCredentials = ModSicServiceMocker.FAKE_TARGET_CREDENTIALS;
            this.FakeDefinitions = ModSicServiceMocker.FAKE_OVAL_DEFINITIONS;
        }

        #region Sending Collect Tests
        [TestMethod, Owner("lfernandes"), TestCategory("Sending Collect")]
        public void Should_be_possible_to_request_collect_async()
        {
            var fakeClientRequestID = Guid.NewGuid().ToString();
            var fakeServerRequestID = Guid.NewGuid().ToString();
            var mockModSicService = 
                ModSicServiceMocker.CreateModSicServiceToSendCollect(
                    fakeClientRequestID, fakeServerRequestID);

            var requestCollectResult = 
                mockModSicService.SendCollect(FakeTargetAddress, new Credential(), FakeDefinitions);

            Assert.IsNotNull(requestCollectResult);
            Assert.AreEqual(2, requestCollectResult.Count);
            
            var firstRequest = requestCollectResult.First();
            Assert.AreEqual(fakeClientRequestID, firstRequest.Key);
            Assert.AreEqual(fakeServerRequestID, firstRequest.Value);
            
            var secondRequest = requestCollectResult.Last();
            Assert.AreEqual(fakeServerRequestID, secondRequest.Key);
            Assert.AreEqual(fakeClientRequestID, secondRequest.Value);
        }

        [TestMethod, Owner("lfernandes"), TestCategory("Sending Collect")]
        [ExpectedException(typeof(ModSicCallingException), EXPECTED_EXCEPTION_WAS_NOT_THROWN)]
        public void If_there_are_errors_in_send_request_result_an_exception_is_expected()
        {
            var mockModSic = ModSicServiceMocker.CreateModSicServiceToReturnErrorInSendRequestResult();

            mockModSic.SendCollect(null, null, null);
        }

        [TestMethod, Owner("lfernandes"), TestCategory("Sending Collect")]
        [ExpectedException(typeof(ModSicCallingException), EXPECTED_EXCEPTION_WAS_NOT_THROWN)]
        public void If_there_an_error_occurs_while_trying_to_call_send_collect_through_modSIC_API_an_exception_must_be_thrown()
        {
            var mockModSic = ModSicServiceMocker.CreateModSicServiceToThrowExceptionWhileSendCollectCalling();
            
            mockModSic.SendCollect(null, null, null);
        }
        #endregion

        #region Getting Oval Results Tests
        [TestMethod, Owner("lfernandes"), TestCategory("Getting Oval Results")]
        public void Should_be_possible_to_get_oval_results_through_modSIC()
        {
            var expectedOvalResults = ModSicServiceMocker.FAKE_OVAL_RESULTS;
            var mockModSic = ModSicServiceMocker.CreateModSicServiceToGetOvalResults();

            var ovalResults = mockModSic.GetOvalResults(ModSicServiceMocker.FAKE_COLLECT_REQUEST_ID);

            Assert.IsFalse(String.IsNullOrWhiteSpace(ovalResults), "The Oval Results cannot be null or empty.");
            Assert.AreEqual(expectedOvalResults, ovalResults, "The oval results returned by ModSicService is unexpected.");
        }

        [TestMethod, Owner("lfernandes"), TestCategory("Getting Oval Results")]
        [ExpectedException(typeof(ArgumentNullException), EXPECTED_EXCEPTION_WAS_NOT_THROWN)]
        public void Should_not_possible_to_get_oval_results_passing_a_null_collect_request_ID()
        {
            new ModSicService(new CollectServerParameters()).GetOvalResults(null);
        }

        [TestMethod, Owner("lfernandes"), TestCategory("Getting Oval Results")]
        [ExpectedException(typeof(ModSicCallingException), EXPECTED_EXCEPTION_WAS_NOT_THROWN)]
        public void If_there_an_error_occurs_while_trying_to_call_get_oval_results_through_modSIC_API_an_exception_must_be_thrown()
        {
            var mockModSic = ModSicServiceMocker.CreateModSicServiceToThrowExceptionWhileGetOvalResultsCalling();

            mockModSic.GetOvalResults(ModSicServiceMocker.FAKE_COLLECT_REQUEST_ID);
        }
        #endregion

        #region Canceling Collect Tests
        [TestMethod, Owner("lfernandes"), TestCategory("Canceling collect")]
        public void Should_be_possible_to_cancel_collect_through_modSIC()
        {
            var mockModSic = ModSicServiceMocker.CreateModSicServiceToCancelCollect(true);
            var ovalResults = mockModSic.CancelCollect(ModSicServiceMocker.FAKE_COLLECT_REQUEST_ID);
            Assert.IsTrue(ovalResults, "Unexpected cancel collect operation return.");

            mockModSic = ModSicServiceMocker.CreateModSicServiceToCancelCollect(false);
            ovalResults = mockModSic.CancelCollect(ModSicServiceMocker.FAKE_COLLECT_REQUEST_ID);
            Assert.IsFalse(ovalResults, "Unexpected cancel collect operation return.");
        }

        [TestMethod, Owner("lfernandes"), TestCategory("Canceling collect")]
        [ExpectedException(typeof(ArgumentNullException), EXPECTED_EXCEPTION_WAS_NOT_THROWN)]
        public void Should_not_possible_to_cancel_collect_passing_a_null_collect_request_ID()
        {
            new ModSicService(new CollectServerParameters()).CancelCollect(null);
        }

        [TestMethod, Owner("lfernandes"), TestCategory("Canceling collect")]
        [ExpectedException(typeof(ModSicCallingException), EXPECTED_EXCEPTION_WAS_NOT_THROWN)]
        public void If_there_an_error_occurs_while_trying_to_call_cancel_collect_through_modSIC_API_an_exception_must_be_thrown()
        {
            var mockModSic = ModSicServiceMocker.CreateModSicServiceToThrowExceptionWhileCancelCollectCalling();

            mockModSic.CancelCollect(ModSicServiceMocker.FAKE_COLLECT_REQUEST_ID);
        }

        #endregion

        #region Getting Collections in Executions Tests
        [TestMethod, Owner("lfernandes"), TestCategory("Getting collections in execution.")]
        public void Should_be_possible_to_get_collections_in_execution_through_modSIC()
        {
            var mockModSic = ModSicServiceMocker.CreateModSicServiceToGetCollectionsInExecutions();
            
            var collectionsInExecutions = mockModSic.GetCollectionsInExecution(false);
            
            Assert.IsNotNull(collectionsInExecutions, "The return of GetCollectionsInExecution operation cannot be null.");
            Assert.AreEqual(2, collectionsInExecutions.Count(), "Unexpected collections amount was returned by GetCollectionsInExecutions operation.");
            Assert.AreEqual("CollectRequests/2048", collectionsInExecutions.First().Key, UNEXPECTED_COLLECTION_IN_EXECUTION_WAS_FOUND);
            Assert.AreEqual("10.1.0.1", collectionsInExecutions.First().Value, UNEXPECTED_COLLECTION_IN_EXECUTION_WAS_FOUND);
            Assert.AreEqual("CollectRequests/2049", collectionsInExecutions.Last().Key, UNEXPECTED_COLLECTION_IN_EXECUTION_WAS_FOUND);
            Assert.AreEqual("10.1.0.2", collectionsInExecutions.Last().Value, UNEXPECTED_COLLECTION_IN_EXECUTION_WAS_FOUND);
        }

        [TestMethod, Owner("lfernandes"), TestCategory("Getting collections in execution.")]
        [ExpectedException(typeof(ModSicCallingException), EXPECTED_EXCEPTION_WAS_NOT_THROWN)]
        public void If_there_an_error_occurs_while_trying_to_call_get_collections_in_excutions_through_modSIC_API_an_exception_must_be_thrown()
        {
            var mockModSic = ModSicServiceMocker.CreateModSicServiceToThrowExceptionWhileGetCollectionsInExecutionCalling();

            mockModSic.GetCollectionsInExecution(false);
        }
        #endregion


        [TestMethod, Owner("lfernandes"), TestCategory("Sending Syncronous Collect")]
        public void Should_be_possible_to_request_collect_sync()
        {
            var mockModSicApi = ModSicServiceMocker.CreateModSicServiceToSendCollectAndWaitItsCompletion();
            var collectRequestID = string.Empty;
            var fakeCredential = new Credential();
            var requestCollectResult =
                new ModSicService(mockModSicApi)
                    .SendCollectSynchronous(
                        FakeTargetAddress, fakeCredential, FakeDefinitions, out collectRequestID, 0, null);

            Assert.IsFalse(String.IsNullOrEmpty(requestCollectResult), "The result of send collect syncronous cannot be null.");
            mockModSicApi.AssertWasCalled(api => api.SendCollect(FakeTargetAddress, fakeCredential, FakeDefinitions, null));
            mockModSicApi.AssertWasCalled(api => api.GetCollectionsInExecution(), api => api.Repeat.Times(6));
            mockModSicApi.AssertWasCalled(api => api.GetOvalResults(ModSicServiceMocker.FAKE_COLLECT_REQUEST_ID));
            Assert.AreEqual(ModSicServiceMocker.FAKE_OVAL_RESULTS, requestCollectResult, "Unexpected Oval Results was returned.");
        }


    }
}
