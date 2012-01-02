using Modulo.Collect.ClientConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography.X509Certificates;
using Rhino.Mocks;

namespace Modulo.Collect.ClientConsole.Tests
{
    [TestClass]
    public class TargetParametersTest
    {
        private const string UNEXPECTED_TARGET_ADDRESS_FOUND = "Unexpected modSIC URL was found.";
        private const string FAKE_ENCRYPTED_CREDENTIALS = "sd fnsiudy fgdvfsdzlyvfedyifgeuiywquioedyq78yD^sa}FD^s]d]ã";

        private const string FAKE_ADDRESS = "10.1.1.1";
        private const string FAKE_DOMAIN = "acme";
        private const string FAKE_USERNAME = "admin";
        private const string FAKE_PASSWORD = "123456";
        private const string EMPTY_DOMAIN = "";
        
        private X509Certificate2 FakeCertificate;


        public TargetParametersTest()
        {
            this.FakeCertificate = new X509Certificate2();
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_encapsulate_target_info()
        {
            var mockCryptoProvider = CreateMockCryptoProvider(EMPTY_DOMAIN);

            var targetParameters = 
                new TargetParameters(
                    FAKE_ADDRESS, FAKE_USERNAME, FAKE_PASSWORD, "", FakeCertificate, mockCryptoProvider);

            Assert.AreEqual("10.1.1.1", targetParameters.Address, UNEXPECTED_TARGET_ADDRESS_FOUND);
            Assert.IsFalse(String.IsNullOrWhiteSpace(targetParameters.EncryptedCredentials), "The encrypted credentials cannot be null.");
            mockCryptoProvider
                .AssertWasCalled(
                    crypto => crypto.EncryptCredential(FakeCertificate, EMPTY_DOMAIN, FAKE_USERNAME, FAKE_PASSWORD, ""));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_encapsulate_target_info_with_domain()
        {
            var mockCryptoProvider = CreateMockCryptoProvider(FAKE_DOMAIN);
            var username = String.Format(@"{0}\{1}", FAKE_DOMAIN, FAKE_USERNAME);

            var targetParameters =
                new TargetParameters(
                    FAKE_ADDRESS, username, FAKE_PASSWORD, "", FakeCertificate, mockCryptoProvider);

            Assert.AreEqual("10.1.1.1", targetParameters.Address, UNEXPECTED_TARGET_ADDRESS_FOUND);
            Assert.IsFalse(String.IsNullOrWhiteSpace(targetParameters.EncryptedCredentials), "The encrypted credentials cannot be null.");
            mockCryptoProvider
                .AssertWasCalled(
                    crypto => crypto.EncryptCredential(FakeCertificate, FAKE_DOMAIN, FAKE_USERNAME, FAKE_PASSWORD, ""));
        }

        private ClientConsoleCryptoProvider CreateMockCryptoProvider(string fakeDomain)
        {
            var mocks = new MockRepository();
            var mockCryptoProvider = mocks.DynamicMock<ClientConsoleCryptoProvider>(new object[] { null });
            
            Expect.Call(
                mockCryptoProvider
                    .EncryptCredential(FakeCertificate, fakeDomain, FAKE_USERNAME, FAKE_PASSWORD, ""))
                    .Return(FAKE_ENCRYPTED_CREDENTIALS);
            mocks.ReplayAll();

            return mockCryptoProvider;
        }
    }
}
