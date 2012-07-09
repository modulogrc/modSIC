using System.Security.Cryptography.X509Certificates;
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Contract.Security;
using Modulo.Collect.Service.Tests.Helpers;
using System.Text;

namespace Modulo.Collect.Service.Tests
{
    [TestClass]
    public class CollectServiceCryptoProviderTest
    {
        private byte[] EncryptedCredentialInBytes;

        public CollectServiceCryptoProviderTest()
        {
            this.EncryptedCredentialInBytes = Encoding.Default.GetBytes(new CredentialFactory().GetEncryptCredentialInString());            
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_encrypt_credential_based_on_certificate_of_server()
        {
            var encryptCredential = 
                new CollectServiceCryptoProvider()
                    .EncryptCredentialBasedOnCertificateOfServer(
                        new CredentialFactory().GetCredential(), 
                        new CertificateHelper().GetCertificateOfServer());

            Assert.IsNotNull(encryptCredential);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_decrypt_credential_based_on_certificate_of_server()
        {
            var credential = 
                new CollectServiceCryptoProvider()
                    .DecryptCredentialBasedOnCertificateOfServer(
                        this.EncryptedCredentialInBytes, 
                        new CertificateHelper().GetCertificateOfServer());

            Assert.IsNotNull(credential);
            Assert.AreEqual("john_doe", credential.UserName);
            Assert.AreEqual("******", credential.Password);
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_encrypt_credential_based_on_public_part_of_certificate_of_server()
        {
            var certificateInBytes = new CertificateHelper().GetCertificateOfServer().Export(X509ContentType.Cert);
            var otherCertificate = new X509Certificate2(certificateInBytes);
            Assert.IsNotNull(otherCertificate);

            var credentialInBytes = 
                new CollectServiceCryptoProvider()
                    .EncryptCredentialBasedOnCertificateOfServer(new CredentialFactory().GetCredential(), otherCertificate);
            Assert.IsNotNull(credentialInBytes);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_a_null_credential_was_passed_the_crypto_provider_must_return_a_new_Credential_instance()
        {
            var credential =
                new CollectServiceCryptoProvider()
                    .DecryptCredentialBasedOnCertificateOfServer(null, null);

            Assert.IsNotNull(credential, "Crypto Provider must return a instance of Credential class.");
            Assert.IsNull(credential.Domain, "The Domain field must be null.");
            Assert.IsNull(credential.UserName, "The UserName field must be null.");
            Assert.IsNull(credential.Password, "The Password field must be null.");
            Assert.IsNull(credential.AdministrativePassword, "The AdministrativePassword field must be null.");
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(NoPrivateKeyException))]
        public void Should_throw_a_typed_exception_when_no_private_key_was_found()
        {
            var certificateWithNoPrivateKey = CreateCertificateWithNoPrivateKey();

            new CollectServiceCryptoProvider().DecryptCredentialBasedOnCertificateOfServer(this.EncryptedCredentialInBytes, certificateWithNoPrivateKey);
        }

        private X509Certificate2 CreateCertificateWithNoPrivateKey()
        {
            var certificateInBytes = new CertificateHelper().GetCertificateOfServer().Export(X509ContentType.Cert);
            var certificate = new X509Certificate2(certificateInBytes);
            certificate.PrivateKey = null;
            return certificate;
        }

        
    }
}

