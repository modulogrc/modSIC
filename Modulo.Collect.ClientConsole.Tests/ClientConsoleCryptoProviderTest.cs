using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Modulo.Collect.ClientConsole.Tests
{


    [TestClass]
    public class ClientConsoleCryptoProviderTest
    {
        [TestMethod, Owner("lfernandes")]
        public void EncryptCredentialTest()
        {
            var cryptoProvider = CreateMockForClientConsoleCryptoProvider();

            var encryptedCredentials = cryptoProvider.EncryptCredential(null, null, null, null, null);

            Assert.IsNotNull(encryptedCredentials, "The result of credentials encryptation cannot be null.");
            Assert.AreEqual("modSIC", encryptedCredentials);
        }

        private ClientConsoleCryptoProvider CreateMockForClientConsoleCryptoProvider()
        {
            var mocks = new MockRepository();
            var proxy = mocks.DynamicMock<CryptoProviderProxy>();
            Expect.Call(
                proxy.EncryptCredential(null, null))
                    .IgnoreArguments()
                .Return(new byte[] { 0x6D, 0x6F, 0x64, 0x53, 0x49, 0x43 });
            mocks.ReplayAll();

            return new ClientConsoleCryptoProvider(proxy);
        }
    }
}
