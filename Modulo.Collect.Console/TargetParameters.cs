#region License
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Modulo.Collect.ClientConsole
{
    public class TargetParameters
    {
        private ClientConsoleCryptoProvider CryptoProvider;
        private string p;
        private string p_2;
        private string p_3;
        private X509Certificate2 x509Certificate2;
        private X509Certificate2 certificate;

        public String Address { get; private set; }
        
        public String EncryptedCredentials { get; private set; }

        
        
        /// <summary>
        /// This contains the target address and encrypted credentials.
        /// </summary>
        /// <param name="address">The target IP Address or host name.</param>
        /// <param name="username">The target admin user name.</param>
        /// <param name="password">The target admin password.</param>
        /// <param name="certificate">The certificate that contains the public key to encrypt target credentials.</param>
        /// <param name="cryptoProvider">Only for unit tests purposes. Do not use it production code (pass always null on it).</param>
        public TargetParameters(
            string address, 
            string username, 
            string password,
            string administrativePassword,
            X509Certificate2 certificate, 
            ClientConsoleCryptoProvider cryptoProvider = null)
        {
            this.Address = address;
            this.CryptoProvider = cryptoProvider;
            this.EncryptedCredentials = this.EncryptCredentials(username, password, administrativePassword, certificate);

        }

        private string EncryptCredentials(string username, string password, string administrativePassword, X509Certificate2 certificate)
        {
            if (this.CryptoProvider == null)
                this.CryptoProvider = new ClientConsoleCryptoProvider();
            
            var resolvedUsername = this.ResolveUsername(username);
            return 
                this.CryptoProvider.EncryptCredential(
                    certificate, resolvedUsername.Key, resolvedUsername.Value, password, administrativePassword);
        }

        /// <summary>
        /// It gets a domain and username from a full qualified user name.
        /// </summary>
        /// <param name="username">Full Qualified User Name.</param>
        /// <returns>A key value pair which on the key is the user domain and the value is the user name.</returns>
        private KeyValuePair<String, String> ResolveUsername(string username)
        {
            var domain = string.Empty;
            var splittedUsername = username.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            if (splittedUsername.Count() == 2)
            {
                domain = splittedUsername.First();
                username = splittedUsername.Last();
            }

            return new KeyValuePair<String, String>(domain, username);
        }



 
    }
}
