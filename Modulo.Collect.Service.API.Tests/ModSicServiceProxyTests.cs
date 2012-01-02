using Modulo.Collect.Service.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Modulo.Collect.Service.Client.Internal;
using Modulo.Collect.Service.Contract;
using Rhino.Mocks;

namespace Modulo.Collect.Service.API.Tests
{
    [TestClass]
    public class ModSicServiceProxyTests
    {
        private const string FAKE_MODSIC_URL = "https://localhost:1000/CollectService";
        private const string FAKE_COLLECT_REQUEST_ID = "collectrequests/1";
        private const string FAKE_TOKEN = "FakeToken";
        private const string FAKE_MODSIC_USERNAME = "admin";
        private const string FAKE_MODSIC_PASSWORD = "123456";

        private ModSicServiceProxy ModSicProxy;
        private ICollectService FakeCollectService;

        public ModSicServiceProxyTests()
        {
            var mocks = new MockRepository();
            this.FakeCollectService = mocks.DynamicMock<ICollectService>();
            mocks.ReplayAll();

            this.ModSicProxy = new ModSicServiceProxy(FAKE_MODSIC_URL, FakeCollectService);
        }


        [TestMethod, Owner("lfernandes")]
        public void ModSicServiceProxyConstructorTest()
        {
            Assert.IsNotNull(ModSicProxy.ServiceConfigurationHelper, "Service Configuration Helper was not initialized.");
            Assert.AreEqual(FAKE_MODSIC_URL, ModSicProxy.ServiceConfigurationHelper.ServiceURL, "Unexpected modSIC URL.");
        }

        [TestMethod, Owner("lfernandes")]
        public void HeartbeatTest()
        {
            ModSicProxy.Heartbeat();

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.Heartbeat());
        }

        [TestMethod, Owner("lfernandes")]
        public void LoginTest()
        {
            ModSicProxy.Login(FAKE_MODSIC_USERNAME, FAKE_MODSIC_PASSWORD);

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.Login(FAKE_MODSIC_USERNAME, FAKE_MODSIC_PASSWORD));
        }

        [TestMethod, Owner("lfernandes")]
        public void LogoutTest()
        {
            ModSicProxy.Logout(FAKE_TOKEN);

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.Logout(FAKE_TOKEN));
        }

        [TestMethod, Owner("lfernandes")]
        public void SendRequestTest()
        {
            var fakePackage = new Package();

            ModSicProxy.SendRequest(fakePackage, FAKE_TOKEN);

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.SendRequest(fakePackage, FAKE_TOKEN));
        }

        [TestMethod, Owner("lfernandes")]
        public void CancelCollectTest()
        {
            ModSicProxy.CancelCollect(FAKE_COLLECT_REQUEST_ID, FAKE_TOKEN);

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.CancelCollect(FAKE_COLLECT_REQUEST_ID, FAKE_TOKEN));
        }

        [TestMethod, Owner("lfernandes")]
        public void GetCertificateTest()
        {
            ModSicProxy.GetCertificate(FAKE_TOKEN);

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.GetCertificate(FAKE_TOKEN));
        }

        [TestMethod, Owner("lfernandes")]
        public void GetCollectRequestsInExecutionTest()
        {
            ModSicProxy.GetCollectRequestsInExecution(FAKE_TOKEN);

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.GetCollectRequestsInExecution(FAKE_TOKEN));
        }

        [TestMethod, Owner("lfernandes")]
        public void GetCollectedResultDocumentTest()
        {
            ModSicProxy.GetCollectedResultDocument(FAKE_COLLECT_REQUEST_ID, FAKE_TOKEN);

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.GetCollectedResultDocument(FAKE_COLLECT_REQUEST_ID, FAKE_TOKEN));
        }

        [TestMethod, Owner("lfernandes")]
        public void GetOvalResultDocumentTest()
        {
            ModSicProxy.GetOvalResultDocument(FAKE_COLLECT_REQUEST_ID, FAKE_TOKEN);

            FakeCollectService
                .AssertWasCalled<ICollectService>(
                    modSIC => modSIC.GetOvalResultDocument(FAKE_COLLECT_REQUEST_ID, FAKE_TOKEN));
        }
    }
}
