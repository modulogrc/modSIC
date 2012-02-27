/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using Modulo.Collect.Service.Client.Internal;
using Modulo.Collect.Service.Contract;
using System.Reflection;
using Modulo.Collect.Service.Contract.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

namespace Modulo.Collect.Service.Client
{
    public class ModSicConnection
    {
        private string Username;
        private string Password;
        private string ClientId;
        private APIVersion RequiredVersion;

        private LoginInfo loginInfo;
        private string CertificatePassword;

        /// <summary>
        /// modSIC version running on server.
        /// </summary>
        public Version ServerProgramVersion
        {
            get
            {
                if ((loginInfo == null) || String.IsNullOrEmpty(loginInfo.Token))
                    return null;
                return loginInfo.ServerVersion;
            }
        }
        /// <summary>
        /// modSIC API version provided by server.
        /// </summary>
        public APIVersion ServerAPIVersion
        {
            get
            {
                if ((loginInfo == null) || String.IsNullOrEmpty(loginInfo.Token))
                    return null;
                return loginInfo.APIVersion;
            }
        }

        /// <summary>
        /// You do not need use this member in production code. Only for unit tests porposes.
        /// </summary>
        public ModSicServiceProxy ModSicProxyService { get; private set; }

        /// <summary>
        /// Constructor for modSIC API. This is the constructor that be used in production code.
        /// </summary>
        /// <param name="username">modSIC user name. It´s optional, because you do not need to pass username in order to call Heartbeat operation.</param>
        /// <param name="username">modSIC password. Like username parameter.</param>
        public ModSicConnection(string modSicServiceURL, string username = null, string password = null, string clientId = null)
        {
            this.Username = username;
            this.Password = password;
            this.ClientId = clientId;
            this.RequiredVersion = new APIVersion(APIVersion.CURRENT_VERSION);
         
            this.ModSicProxyService = new ModSicServiceProxy(modSicServiceURL);
        }


        /// <summary>
        /// This constructor must be use only for unit tests purposes.
        /// </summary>
        /// <param name="modSicProxy">An instance of ModSicServiceProxy with mock behavior.</param>
        /// <param name="username">modSIC user name. It´s optional, because you do not need to pass username in order to call Heartbeat operation.</param>
        /// <param name="username">modSIC password. Like username parameter.</param>
        public ModSicConnection(ModSicServiceProxy modSicProxyMock, string username = null, string password = null, string clientId = null, APIVersion apiVersion = null, string fakeCertificatePassword = "12")
        {
            this.Username = username;
            this.Password = password;
            this.ClientId = clientId;
            this.RequiredVersion = apiVersion;
            this.ModSicProxyService = modSicProxyMock;
            this.CertificatePassword = fakeCertificatePassword;
        }

        /// <summary>
        /// Check if modSIC is available
        /// </summary>
        public virtual String Heartbeat()
        {
            return ModSicProxyService.Heartbeat();
        }

        /// <summary>
        /// It requests a new collection to modSIC Server.
        /// </summary>
        /// <param name="fakePackage">
        ///     <see cref="http://modsic.codeplex.com/wikipage?title=Contract&referringTitle=Documentation#package"/>
        /// </param>
        /// <returns><see cref="http://modsic.codeplex.com/wikipage?title=Contract&referringTitle=Documentation#sendRequestResult"/></returns>
        public virtual SendRequestResult SendCollect(Package fakePackage)
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.SendRequest(fakePackage, token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
        }

        /// <summary>
        /// It requests a new collection to modSIC Server.
        /// </summary>
        /// <param name="targetAddress">The target IP address or hostname.</param>
        /// <param name="targetCredentials">The target encrypted and serialized credentials.</param>
        /// <param name="ovalDefinitions">The Oval Definitions document.</param>
        /// <param name="externalVariables">The optional Oval Variables document.</param>
        /// <returns><see cref="http://modsic.codeplex.com/wikipage?title=Contract&referringTitle=Documentation#sendRequestResult"/></returns>
        public virtual SendRequestResult SendCollect(
            string targetAddress, Credential targetCredentials, string ovalDefinitions, string externalVariables = null, Dictionary<string, string> targetParameters = null)
        {
            var newRequest = CreateRequest(targetAddress, targetCredentials, externalVariables, targetParameters);
            var package = CreatePackage(ovalDefinitions, newRequest);
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.SendRequest(package, token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
        }

        private Package CreatePackage(string ovalDefinitions, Request newRequest)
        {
            var package = new Package()
            {
                CollectRequests = new Request[] { newRequest },
                Date = DateTime.UtcNow,
                Definitions = new DefinitionInfo[] { new DefinitionInfo() { Id = newRequest.DefinitionId, Text = ovalDefinitions } },
                ScheduleInformation = new ScheduleInformation() { ScheduleDate = DateTime.UtcNow }
            };
            return package;
        }

        private Request CreateRequest(string targetAddress, Credential credentials, string externalVariables, Dictionary<string, string> targetParameters = null)
        {
            var encrytedCredentials = this.EncryptCredentials(credentials);

            var newRequest = new Request()
            {
                Address = targetAddress,
                Credential = encrytedCredentials,
                DefinitionId = Guid.NewGuid().ToString(),
                ExternalVariables = externalVariables,
                RequestId = Guid.NewGuid().ToString(),
                TargetParameters = targetParameters
            };
            return newRequest;
        }

        private String EncryptCredentials(Credential credentials)
        {
            var certificateInBytes = this.GetCertificate();
            var encryptedBytes = 
                new CollectServiceCryptoProvider()
                    .EncryptCredentialBasedOnCertificateOfServer(
                        credentials, 
                        new X509Certificate2(certificateInBytes, this.CertificatePassword));

            return System.Text.Encoding.Default.GetString(encryptedBytes);
        }

        /// <summary>
        /// It cancels a collection or scheduled collection.
        /// </summary>
        /// <param name="collectRequestID">The collect request ID.</param>
        /// <returns>
        ///     Returns "True" if the collection or scheduled collection was cancelled successfully.
        ///     Returns "False" if the requestId does not exist or if an unexpected error occurred.
        /// </returns>
        public virtual Boolean CancelCollect(string collectRequestID)
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.CancelCollect(collectRequestID, token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
        }

        /// <summary>
        /// Gets modSIC Server Certificate.
        /// </summary>
        /// <returns>A X509 Certificate as byte array.</returns>
        public virtual byte[] GetCertificate()
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.GetCertificate(token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
            
        }

        /// <summary>
        /// Gets a Oval Results of a collection.
        /// </summary>
        /// <param name="collectRequestID">Collect Request ID.</param>
        /// <returns>Oval Results Document as String.</returns>
        public virtual string GetOvalResults(string collectRequestID)
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.GetOvalResultDocument(collectRequestID, token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
        }

        public virtual Result GetResultDocument(string collectRequestID)
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.GetCollectedResultDocument(collectRequestID, token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
        }

        /// <summary>
        /// Gets collections in execution from this client.
        /// </summary>
        /// <returns></returns>
        public virtual CollectInfo[] GetCollectionsInExecution()
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.GetCollectRequestsInExecution(this.ClientId, token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
        }

        /// <summary>
        /// Gets collections in execution from all clients.
        /// </summary>
        /// <returns></returns>
        public virtual CollectInfo[] GetAllCollectionsInExecution()
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.GetCollectRequestsInExecution(token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
        }

        public virtual String GetFullCompressedLog(string collectRequestID)
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.GetFullCompressedLog(collectRequestID, token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }
        }

        public virtual String GetModsicApiVersion()
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}", assemblyVersion.Major, assemblyVersion.Minor);
        }

        public virtual TargetCheckingResult CheckTargetAvailability(TargetPlatforms targetPlatform, string address, string encryptedCredentials, string sshPort)
        {
            var token = LogonModSic();
            try
            {
                return ModSicProxyService.CheckTargetAvailability(targetPlatform, address, encryptedCredentials, sshPort, token);
            }
            finally
            {
                ModSicProxyService.Logout(token);
            }

        }

        private string LogonModSic()
        {
            loginInfo = this.ModSicProxyService.LoginEx(this.Username, this.Password, this.ClientId, this.RequiredVersion);
            if (!String.IsNullOrEmpty(loginInfo.Token))
            {
                return loginInfo.Token;
            }
            else
            {
                throw new ApplicationException(String.Format("{0}: {1}", loginInfo.ErrorType, loginInfo.ErrorMessage));
            }
        }
    }
}
