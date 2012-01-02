#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Contract.Security;
using System.Security.Cryptography.X509Certificates;

namespace Modulo.Collect.GraphicalConsole
{
    public class ClientCryptoProvider
    {
        private CryptoProviderProxy CryptoProviderProxy;

        public ClientCryptoProvider()
        {
            this.CryptoProviderProxy = new CryptoProviderProxy();
        }

        /// <summary>
        /// This constructor is only for unit tests purposes. In production code use default constructor.
        /// </summary>
        /// <param name="cryptoProviderProxy">A mock Crypto Provider Proxy.</param>
        public ClientCryptoProvider(CryptoProviderProxy cryptoProviderProxy)
        {
            this.CryptoProviderProxy = cryptoProviderProxy;
        }

        public virtual String EncryptCredential(X509Certificate2 certificate, string domain, string username, string password, string administrativePassword)
        {
            var credential = new Credential() { Domain = domain, UserName = username, Password = password, AdministrativePassword = administrativePassword };
            var certificateInBytes = CryptoProviderProxy.EncryptCredential(credential, certificate);           
            return System.Text.Encoding.Default.GetString(certificateInBytes);
        }
    }
}