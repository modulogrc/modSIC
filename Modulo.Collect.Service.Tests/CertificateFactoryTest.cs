using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Contract.Security;
using Modulo.Collect.Service.Tests.Helpers;

namespace Modulo.Collect.Service.Tests
{
    [TestClass]
    public class CertificateFactoryTest
    {
        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_export_the_public_part_of_the_certificate_in_pfx_format()
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
        public void Should_be_possible_to_use_ServerCertificate_to_do_assimetric_cryptography()
        {
            var certificate = new CertificateHelper().GetCertificateOfServer();
            var cryptoProvider = new CollectServiceCryptoProvider();
            var encryptedCredentials = 
                cryptoProvider
                    .EncryptCredentialBasedOnCertificateOfServer(new CredentialFactory().GetCredential(), certificate);
            var plainCredentials = 
                cryptoProvider
                    .DecryptCredentialBasedOnCertificateOfServer(encryptedCredentials, certificate);

            Assert.IsNotNull(plainCredentials, "The credentials after decryptation cannot be nuul.");
            Assert.AreEqual("fake_domain", plainCredentials.Domain);
            Assert.AreEqual("john_doe", plainCredentials.UserName);
            Assert.AreEqual("******", plainCredentials.Password);
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(CryptographicException))]
        public void Expect_CryptographicException_when_a_invalid_certificate_is_used_to_encrypt_credentials()
        {
            var certificate = new CertificateHelper().GetCertificateOfServerByName("LocalhostCertificate.pfx");
            var cryptoProvider = new CollectServiceCryptoProvider();

            var encryptedCredentials = 
                cryptoProvider
                    .EncryptCredentialBasedOnCertificateOfServer(new CredentialFactory().GetCredential(), certificate);
            
            var plainCredentials = 
                cryptoProvider
                    .DecryptCredentialBasedOnCertificateOfServer(encryptedCredentials, certificate);
        }
    }
}
