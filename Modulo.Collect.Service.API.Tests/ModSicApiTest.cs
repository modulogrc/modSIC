using Modulo.Collect.Service.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Modulo.Collect.Service.Client;
using Modulo.Collect.Service.Client.Internal;
using Modulo.Collect.Service.Contract;
using Rhino.Mocks;
using System.IO;
using System.Text;
using Modulo.Collect.TestHelper;

namespace Modulo.Collect.Service.API.Tests
{
    [TestClass]
    public class ModSicApiTest
    {
        private const string FAKE_MODSIC_USERNAME = "admin";
        private const string FAKE_MODSIC_PASSWORD = "12345";
        private const string FAKE_MODSIC_CLIENTID = "Client2469171";

        private const string FAKE_IP_ADDRESS = "10.1.0.15";
        private const string FAKE_CREDENTIALS = "AaAajaklJakljaijh";
        private const string FAKE_COLLECT_REQUEST_ID = "CollectRequests/1";
        //private const string FAKE_TOKEN = "qwerQWERTY#$%667#$%BGKBTYfgtfg&GHUIi(H98H9HGH*ByuFVCRDR";
        private APIVersion FakeApiVersion;
        private LoginInfo FakeLoginInfoToReturn;


        public ModSicApiTest()
        {
            this.FakeApiVersion = new APIVersion("1.0");
            this.FakeLoginInfoToReturn = new LoginInfo() { APIVersion = FakeApiVersion, Token = Guid.NewGuid().ToString() };
        }

        [TestMethod, Owner("lfernandes")]
        public void When_using_production_constructor_a_proxy_for_modsicService_must_be_created()
        {
            var modsicAPI = new ModSicConnection("https://localhost:1000/CollectService");

            Assert.IsNotNull(modsicAPI.ModSicProxyService, "A proxy for modSIC Service must be initialized.");

        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_request_collect_through_modSIC()
        {
            var fakeDefinitionInfo = CreateDefinitionInfo(Guid.NewGuid().ToString());
            var fakeRequest = CreateRequest(fakeDefinitionInfo.Id, Guid.NewGuid().ToString());
            var fakePackage = CreatePackage(fakeRequest, fakeDefinitionInfo, new DateTime(2011, 1, 1, 11, 35, 0));
            var fakeModSicProxy = CreateModSicProxyWithBehavior();

            var sendCollectResult = CreateModSicAPI(fakeModSicProxy).SendCollect(fakePackage);

            ModSicLoginMustBeCalling(fakeModSicProxy);
            fakeModSicProxy.AssertWasCalled<ModSicServiceProxy>(p => p.SendRequest(fakePackage, FakeLoginInfoToReturn.Token));
            ModSicLogoutMustBeCalling(fakeModSicProxy);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_request_collect_trhough_modSIC_easily()
        {
            var fakeTargetAddress = "10.1.1.2";
            var fakeTargetEncryptedCredentials = "AunJDShgsd9820";
            var fakeOvalDefinitions = "<oval_definitions>...</oval_definitions>";
            var fakeExternalVariables = "<oval_variables>...</oval_variables>";
            var fakeModSicProxy = CreateModSicProxyWithBehavior();
            var modSicApi = CreateModSicAPI(fakeModSicProxy);

            var sendCollectResult = 
                modSicApi.SendCollect(
                    fakeTargetAddress, 
                    new Credential(), 
                    fakeOvalDefinitions, 
                    fakeExternalVariables);

            ModSicLoginMustBeCalling(fakeModSicProxy);
            fakeModSicProxy
                .AssertWasCalled<ModSicServiceProxy>(
                    p => p.SendRequest(null, FakeLoginInfoToReturn.Token), 
                    o => o.IgnoreArguments());
            ModSicLogoutMustBeCalling(fakeModSicProxy);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_verify_modSIC_availability()
        {
            var mocks = new MockRepository();
            var fakeModSicProxy = mocks.DynamicMock<ModSicServiceProxy>(new object[] { "", null });
            Expect.Call(fakeModSicProxy.Heartbeat()).Return("");
            mocks.ReplayAll();

            CreateModSicAPI(fakeModSicProxy).Heartbeat();

            fakeModSicProxy.AssertWasCalled<ModSicServiceProxy>(p => p.Heartbeat());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_cancel_a_collection_through_modSIC()
        {
            var mocks = new MockRepository();
            var fakeModSicProxy = mocks.DynamicMock<ModSicServiceProxy>(new object[] { "", null });
            Expect.Call(fakeModSicProxy.LoginEx("admin", "12345", FAKE_MODSIC_CLIENTID, FakeApiVersion)).Return(FakeLoginInfoToReturn);
            Expect.Call(fakeModSicProxy.CancelCollect(FAKE_COLLECT_REQUEST_ID, FakeLoginInfoToReturn.Token)).Return(true);
            mocks.ReplayAll();

            var cancelOperationResult = CreateModSicAPI(fakeModSicProxy).CancelCollect(FAKE_COLLECT_REQUEST_ID);

            ModSicLoginMustBeCalling(fakeModSicProxy);
            fakeModSicProxy
                .AssertWasCalled<ModSicServiceProxy>(
                    p => p.CancelCollect(FAKE_COLLECT_REQUEST_ID, FakeLoginInfoToReturn.Token));
            ModSicLogoutMustBeCalling(fakeModSicProxy);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_modSIC_certificate_to_encrypt_credentials()
        {
            var mocks = new MockRepository();
            var fakeModSicProxy = mocks.DynamicMock<ModSicServiceProxy>(new object[] { "", null });
            Expect.Call(fakeModSicProxy.LoginEx("admin", "12345", FAKE_MODSIC_CLIENTID, FakeApiVersion)).Return(FakeLoginInfoToReturn);
            Expect.Call(fakeModSicProxy.GetCertificate(FakeLoginInfoToReturn.Token)).Return(new byte[] { });
            mocks.ReplayAll();

            var cancelOperationResult = CreateModSicAPI(fakeModSicProxy).GetCertificate();

            ModSicLoginMustBeCalling(fakeModSicProxy);
            fakeModSicProxy.AssertWasCalled<ModSicServiceProxy>(p => p.GetCertificate(FakeLoginInfoToReturn.Token));
            ModSicLogoutMustBeCalling(fakeModSicProxy);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_oval_results_through_modSIC()
        {
            var fakeOvalResults = "<oval_results>...</oval_results>";
            var mocks = new MockRepository();
            var fakeModSicProxy = mocks.DynamicMock<ModSicServiceProxy>(new object[] { "", null });
            Expect.Call(fakeModSicProxy.LoginEx("admin", "12345", FAKE_MODSIC_CLIENTID, FakeApiVersion)).Return(FakeLoginInfoToReturn);
            Expect.Call(fakeModSicProxy.GetOvalResultDocument(FAKE_COLLECT_REQUEST_ID, FakeLoginInfoToReturn.Token)).Return(fakeOvalResults);
            mocks.ReplayAll();

            var ovalResults = CreateModSicAPI(fakeModSicProxy).GetOvalResults(FAKE_COLLECT_REQUEST_ID);

            ModSicLoginMustBeCalling(fakeModSicProxy);
            fakeModSicProxy
                .AssertWasCalled<ModSicServiceProxy>(
                    p => p.GetOvalResultDocument(FAKE_COLLECT_REQUEST_ID, FakeLoginInfoToReturn.Token));
            ModSicLogoutMustBeCalling(fakeModSicProxy);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_collections_in_execution_through_modSIC()
        {
            var mocks = new MockRepository();
            var fakeModSicProxy = mocks.DynamicMock<ModSicServiceProxy>(new object[] { "", null });
            Expect.Call(fakeModSicProxy.LoginEx("admin", "12345", FAKE_MODSIC_CLIENTID, FakeApiVersion)).Return(FakeLoginInfoToReturn);
            Expect.Call(
                fakeModSicProxy
                    .GetCollectRequestsInExecution(FakeLoginInfoToReturn.Token))
                    .Return(new CollectInfo[] { new CollectInfo() });
            mocks.ReplayAll();

            var collectionsInExecution = CreateModSicAPI(fakeModSicProxy).GetCollectionsInExecution();

            ModSicLoginMustBeCalling(fakeModSicProxy);
            fakeModSicProxy
                .AssertWasCalled<ModSicServiceProxy>(
                    p => p.GetCollectRequestsInExecution(FAKE_MODSIC_CLIENTID, FakeLoginInfoToReturn.Token));
            ModSicLogoutMustBeCalling(fakeModSicProxy);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_result_document_in_execution_through_modSIC()
        {
            var mocks = new MockRepository();
            var fakeModSicProxy = mocks.DynamicMock<ModSicServiceProxy>(new object[] { "", null });
            Expect.Call(fakeModSicProxy.LoginEx("admin", "12345", FAKE_MODSIC_CLIENTID, FakeApiVersion)).Return(FakeLoginInfoToReturn);
            Expect.Call(
                fakeModSicProxy
                    .GetCollectedResultDocument(FAKE_COLLECT_REQUEST_ID, FakeLoginInfoToReturn.Token))
                    .Return(new Result());
            mocks.ReplayAll();

            var collectionsInExecution = CreateModSicAPI(fakeModSicProxy).GetResultDocument(FAKE_COLLECT_REQUEST_ID);

            ModSicLoginMustBeCalling(fakeModSicProxy);
            fakeModSicProxy
                .AssertWasCalled<ModSicServiceProxy>(
                    p => p.GetCollectedResultDocument(FAKE_COLLECT_REQUEST_ID, FakeLoginInfoToReturn.Token));
            ModSicLogoutMustBeCalling(fakeModSicProxy);
        }

        private ModSicServiceProxy CreateModSicProxyWithBehavior()
        {
            var mocks = new MockRepository();
            var fakeModSicProxy = mocks.DynamicMock<ModSicServiceProxy>(new object[] { "", null });

            fakeModSicProxy
                .Expect(
                    (proxy) => proxy.LoginEx(FAKE_MODSIC_USERNAME, FAKE_MODSIC_PASSWORD, FAKE_MODSIC_CLIENTID, FakeApiVersion))
                    .IgnoreArguments()
                .Return(FakeLoginInfoToReturn);

            fakeModSicProxy
                .Expect((proxy) => proxy.SendRequest(null, null))
                    .IgnoreArguments()
                .Return(new SendRequestResult());

            fakeModSicProxy
                .Expect((proxy) => proxy.GetCertificate(null))
                    .IgnoreArguments()
                .Return(new FileContentLoader().GetFileContentBytes(GetType().Assembly, "CollectService.pfx"));


            mocks.ReplayAll();
            
            return fakeModSicProxy;
        }


        private Request CreateRequest(string definitionID = null, string requestID = null)
        {
            return new Request()
            {
                Address = FAKE_IP_ADDRESS,
                Credential = FAKE_CREDENTIALS,
                DefinitionId = definitionID ?? Guid.NewGuid().ToString(),
                RequestId = requestID ?? Guid.NewGuid().ToString()
            };
        }

        private DefinitionInfo CreateDefinitionInfo(string definitionID = null, string definitionContents = "")
        {
            return new DefinitionInfo() 
            { 
                Id = definitionID ?? Guid.NewGuid().ToString(), 
                Text = definitionContents 
            };
        }

        private Package CreatePackage(Request request, DefinitionInfo definitionInfo, DateTime? fakeDate = null)
        {
            var date = fakeDate ?? DateTime.Now;
            var fakePackage = new Package()
            {
                Date = date,
                Definitions = new DefinitionInfo[] { definitionInfo },
                CollectRequests = new Request[] { request },
                ScheduleInformation = new ScheduleInformation() { ScheduleDate = date }
            };

            return fakePackage;
        }

        private ModSicConnection CreateModSicAPI(ModSicServiceProxy modSicProxy)
        {
            return new ModSicConnection(modSicProxy, FAKE_MODSIC_USERNAME, FAKE_MODSIC_PASSWORD, FAKE_MODSIC_CLIENTID, this.FakeApiVersion);
        }

        private void ModSicLoginMustBeCalling(ModSicServiceProxy fakeModSicProxy)
        {
            fakeModSicProxy
                .AssertWasCalled<ModSicServiceProxy>(
                    p => p.LoginEx(FAKE_MODSIC_USERNAME, FAKE_MODSIC_PASSWORD, FAKE_MODSIC_CLIENTID, FakeApiVersion));
        }

        private void ModSicLogoutMustBeCalling(ModSicServiceProxy fakeModSicProxy)
        {
            fakeModSicProxy.AssertWasCalled<ModSicServiceProxy>(p => p.Logout(FakeLoginInfoToReturn.Token));
        }

    }
}
