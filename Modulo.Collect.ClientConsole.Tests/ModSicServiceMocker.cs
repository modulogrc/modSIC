using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Service.Client;
using Modulo.Collect.Service.Contract;
using Rhino.Mocks;
using Modulo.Collect.TestHelper;

namespace Modulo.Collect.ClientConsole.Tests
{
    public class ModSicServiceMocker
    {
        public const string FAKE_TARGET_ADDRESS = "10.1.1.1";
        public const string FAKE_TARGET_CREDENTIALS = "Fake Credentials";
        public const string FAKE_OVAL_DEFINITIONS = "<oval_definitions>...</oval_definitions>";
        public const string FAKE_COLLECT_REQUEST_ID = "CollectRequests/1024";
        public const string FAKE_OVAL_RESULTS = "<oval_results>...</oval_results>";

        public ModSicService CreateModSicServiceToSendCollect(
            string fakeClientRequestID, string fakeServiceRequestID)
        {
            var fakeRequestInfo1 = new RequestInfo() { ClientRequestId = fakeClientRequestID, ServiceRequestId = fakeServiceRequestID };
            var fakeRequestInfo2 = new RequestInfo() { ClientRequestId = fakeServiceRequestID, ServiceRequestId = fakeClientRequestID };
            var fakeSendCollectResult = new SendRequestResult() { Requests = new RequestInfo[] { fakeRequestInfo1, fakeRequestInfo2 } };
            var fakeCertificate = new FileContentLoader().GetFileContentBytes(GetType().Assembly, "CollectService.pfx");
            var fakeCredentials = new Credential();
            var mocks = new MockRepository();
            
            var api = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            Expect.Call(
                api.SendCollect(FAKE_TARGET_ADDRESS, fakeCredentials, FAKE_OVAL_DEFINITIONS, null, null))
                       .IgnoreArguments()
                    .Return(fakeSendCollectResult);
            Expect.Call(
                api.GetCertificate())
                    .Return(fakeCertificate);
            mocks.ReplayAll();

            return new ModSicService(api);
        }

        public ModSicConnection CreateModSicServiceToSendCollectAndWaitItsCompletion()
        {
            var fakeRequestInfo = CreateRequestInfo(Guid.NewGuid().ToString(), FAKE_COLLECT_REQUEST_ID);
            var fakeSendCollectResult = new SendRequestResult() { Requests = new RequestInfo[] { fakeRequestInfo } };

            var fakeCollectInfo0 = CreateCollectInfo("10.1.0.1", "CollectRequests/1");
            var fakeCollectInfo1 = CreateCollectInfo("10.1.1.1", FAKE_COLLECT_REQUEST_ID);
            var fakeCollectInfo2 = CreateCollectInfo("10.1.1.2", "CollectRequests/1025");
            var fakeCollectionsInExecution = new CollectInfo[] { fakeCollectInfo0, fakeCollectInfo1, fakeCollectInfo2 };

            var mocks = new MockRepository();
            var api = mocks.StrictMock<ModSicConnection>(new string[] { "", "", "", "" });
            Expect.Call(api.SendCollect(null, null, null, null, null)).IgnoreArguments().Return(fakeSendCollectResult);
            Expect.Call(api.GetCollectionsInExecution()).Repeat.Times(5).Return(fakeCollectionsInExecution);
            Expect.Call(api.GetCollectionsInExecution()).Return(new CollectInfo[] { fakeCollectInfo2 });
            Expect.Call(api.GetOvalResults(FAKE_COLLECT_REQUEST_ID)).Return(FAKE_OVAL_RESULTS);
            mocks.ReplayAll();

            return api;
        }

        public ModSicService CreateModSicServiceToReturnErrorInSendRequestResult()
        {
            var mocks = new MockRepository();
            var mockAPI = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            var fakeRequestResultWithErrors = new SendRequestResult() { HasErrors = true };
            Expect.Call(mockAPI.SendCollect(null, null, null, null, null)).IgnoreArguments().Return(fakeRequestResultWithErrors);
            mocks.ReplayAll();

            return new ModSicService(mockAPI);
        }

        public ModSicService CreateModSicServiceToThrowExceptionWhileSendCollectCalling()
        {
            var mocks = new MockRepository();
            var mockAPI = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            Expect.Call(mockAPI.SendCollect(null, null, null, null, null)).IgnoreArguments().Throw(new Exception());
            mocks.ReplayAll();

            return new ModSicService(mockAPI);
        }

        public ModSicService CreateModSicServiceToGetOvalResults()
        {
            var mocks = new MockRepository();
            var mockAPI = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            Expect.Call(mockAPI.GetOvalResults(FAKE_COLLECT_REQUEST_ID)).Return(FAKE_OVAL_RESULTS);
            mocks.ReplayAll();

            return new ModSicService(mockAPI);
        }

        public ModSicService CreateModSicServiceToThrowExceptionWhileGetOvalResultsCalling()
        {
            var mocks = new MockRepository();
            var mockAPI = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            Expect.Call(mockAPI.GetOvalResults(null)).IgnoreArguments().Throw(new Exception());
            mocks.ReplayAll();

            return new ModSicService(mockAPI);
        }

        public ModSicService CreateModSicServiceToCancelCollect(Boolean operationReturn)
        {
            var mocks = new MockRepository();
            var mockAPI = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            Expect.Call(mockAPI.CancelCollect(FAKE_COLLECT_REQUEST_ID)).Return(operationReturn);
            mocks.ReplayAll();

            return new ModSicService(mockAPI);
        }

        public ModSicService CreateModSicServiceToThrowExceptionWhileCancelCollectCalling()
        {
            var mocks = new MockRepository();
            var mockAPI = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            Expect.Call(mockAPI.CancelCollect(null)).IgnoreArguments().Throw(new Exception());
            mocks.ReplayAll();

            return new ModSicService(mockAPI);
        }

        public ModSicService CreateModSicServiceToGetCollectionsInExecutions()
        {
            var mocks = new MockRepository();
            var mockAPI = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            var fakeCollectionsInExecutions =
                new CollectInfo[]
                {
                    CreateCollectInfo("10.1.0.1", "CollectRequests/2048"),
                    CreateCollectInfo("10.1.0.2", "CollectRequests/2049")
                };
            Expect.Call(mockAPI.GetCollectionsInExecution()).Return(fakeCollectionsInExecutions);
            mocks.ReplayAll();

            return new ModSicService(mockAPI);

        }

        public ModSicService CreateModSicServiceToThrowExceptionWhileGetCollectionsInExecutionCalling()
        {
            var mocks = new MockRepository();
            var mockAPI = mocks.DynamicMock<ModSicConnection>(new string[] { "", "", "", "" });
            Expect.Call(mockAPI.GetCollectionsInExecution()).Throw(new Exception());
            mocks.ReplayAll();

            return new ModSicService(mockAPI);
        }

        private RequestInfo CreateRequestInfo(string clientID, string serviceID)
        {
            return new RequestInfo { ClientRequestId = clientID, ServiceRequestId = serviceID };
        }

        private CollectInfo CreateCollectInfo(string address, string requestID)
        {
            return new CollectInfo { Address = address, CollectRequestId = requestID, ReceivedOn = DateTime.Now };
        }
    }
}
