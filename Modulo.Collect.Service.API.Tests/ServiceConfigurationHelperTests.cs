using Modulo.Collect.Service.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ServiceModel;
using Modulo.Collect.Service.Client.Internal;
using Rhino.Mocks;

namespace Modulo.Collect.Service.API.Tests
{
    [TestClass]
    public class ServiceConfigurationHelperTests
    {
        private const string FAKE_SERVICE_URL_WITH_SSL = "https://localhost:1000/CollectService";
        private const string FAKE_SERVICE_URL_WITHOUT_SSL = "http://localhost:1000/CollectService";
        private const string UNEXPECTED_BINDING_PROPERTY_FAIL_MSG = "Unexpected {0} binding property was found.";
        private const string UNEXPECTED_BINDING_READER_QUOTAS_PROPERTY_FAIL_MSG = "Unexpected {0} binding reader quptas property was found.";


        private ServiceConfigurationHelper ServiceConfigHelperWithSslUrl;
        private ServerCertificateManager CertificateManagerMock;

        public ServiceConfigurationHelperTests()
        {
            this.CertificateManagerMock = this.CreateCertificateManagerMock();
            this.ServiceConfigHelperWithSslUrl = new ServiceConfigurationHelper(FAKE_SERVICE_URL_WITH_SSL, CertificateManagerMock);
        }


        [TestMethod, Owner("lfernandes")]
        public void ServiceConfigurationHelperConstructorTest()
        {
            Assert.AreEqual(FAKE_SERVICE_URL_WITH_SSL, ServiceConfigHelperWithSslUrl.ServiceURL, "Unexpected service URL was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_new_endpoint_address_for_given_url()
        {
            var endpointAddress = ServiceConfigHelperWithSslUrl.CreateEndpointAddressFromURL();
            
            Assert.IsNotNull(endpointAddress, "No Endpoint Address was created.");
            Assert.IsNotNull(endpointAddress.Uri, "There is no URI in endpoint address.");
            Assert.AreEqual(FAKE_SERVICE_URL_WITH_SSL, endpointAddress.Uri.AbsoluteUri, "Unexpected uri was found.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_find_out_secure_channel_from_given_URL()
        {
            var FAIL_TEST_MESSAGE = "Service helper could not find out channel security.";

            Assert.IsTrue(ServiceConfigHelperWithSslUrl.IsSecureChannel(), FAIL_TEST_MESSAGE);
            Assert.IsFalse(new ServiceConfigurationHelper(FAKE_SERVICE_URL_WITHOUT_SSL).IsSecureChannel(), FAIL_TEST_MESSAGE);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_new_WsHttpBinding()
        {
            var wsHttpBinding = ServiceConfigHelperWithSslUrl.CreateWsHttpBinding();

            Assert.IsNotNull(wsHttpBinding, "No WS HTTP Binding was created.");
            Assert.AreEqual(Int32.MaxValue, wsHttpBinding.MaxBufferPoolSize, UNEXPECTED_BINDING_PROPERTY_FAIL_MSG, "MaxBufferPoolSize");
            Assert.AreEqual(Int32.MaxValue, wsHttpBinding.MaxReceivedMessageSize, UNEXPECTED_BINDING_PROPERTY_FAIL_MSG, "MaxReceivedMessageSize");
            Assert.AreEqual(SecurityMode.Transport, wsHttpBinding.Security.Mode, UNEXPECTED_BINDING_PROPERTY_FAIL_MSG, "Security Mode");
            
            var readerQuotas = wsHttpBinding.ReaderQuotas;
            Assert.IsNotNull(wsHttpBinding, "No ReaderQuotas was found in created binding.");
            Assert.AreEqual(Int32.MaxValue, readerQuotas.MaxArrayLength, UNEXPECTED_BINDING_READER_QUOTAS_PROPERTY_FAIL_MSG, "MaxArrayLength");
            Assert.AreEqual(Int32.MaxValue, readerQuotas.MaxBytesPerRead, UNEXPECTED_BINDING_READER_QUOTAS_PROPERTY_FAIL_MSG, "MaxBytesPerRead");
            Assert.AreEqual(Int32.MaxValue, readerQuotas.MaxNameTableCharCount, UNEXPECTED_BINDING_READER_QUOTAS_PROPERTY_FAIL_MSG, "MaxNameTableCharCount");
            Assert.AreEqual(Int32.MaxValue, readerQuotas.MaxStringContentLength, UNEXPECTED_BINDING_READER_QUOTAS_PROPERTY_FAIL_MSG, "MaxStringContentLength");

            this.CertificateManagerMock.AssertWasCalled<ServerCertificateManager>(svc => svc.CreateCertificateValidationCallback());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_new_WsHttpBinding_for_non_ssl_connection()
        {
            var certificateManagerMock = CreateCertificateManagerMock();
            var svcConfigHelper = new ServiceConfigurationHelper(FAKE_SERVICE_URL_WITHOUT_SSL, certificateManagerMock);

            var wsHttpBinding = svcConfigHelper.CreateWsHttpBinding();

            Assert.AreEqual(SecurityMode.None, wsHttpBinding.Security.Mode, "The security mode must be 'none' for no ssl connections.");
            certificateManagerMock.AssertWasNotCalled<ServerCertificateManager>(svc => svc.CreateCertificateValidationCallback());

            
        }

        private ServerCertificateManager CreateCertificateManagerMock()
        {
            var mocks = new MockRepository();
            var fakeCertificateManager = mocks.DynamicMock<ServerCertificateManager>();
            mocks.ReplayAll();

            return fakeCertificateManager;
        }

    }
}
