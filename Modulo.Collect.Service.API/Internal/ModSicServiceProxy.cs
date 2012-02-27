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
using System.ServiceModel;
using Modulo.Collect.Service.Contract;

namespace Modulo.Collect.Service.Client.Internal
{
    public class ModSicServiceProxy
    {
        private ICollectService ModSicChannel;
        public ServiceConfigurationHelper ServiceConfigurationHelper { get; private set; }


        /// <summary>
        /// It creates a new proxy for modSIC Service access.
        /// </summary>
        /// <param name="modSicServiceURL">modSIC Service URL.</param>
        /// <param name="collectServiceMock">Only for unit tests purpose. Do not use this in production code.</param>
        public ModSicServiceProxy(string modSicServiceURL, ICollectService collectServiceMock = null)
        {
            this.ServiceConfigurationHelper = new ServiceConfigurationHelper(modSicServiceURL);
            if (collectServiceMock != null)
                this.ModSicChannel = collectServiceMock;
        }

        public virtual String Heartbeat()
        {
            CreateModSicChannel();
            return this.ModSicChannel.Heartbeat();
        }

        public virtual string Login(string username, string password)
        {
            CreateModSicChannel();
            return this.ModSicChannel.Login(username, password);
        }

        public virtual LoginInfo LoginEx(string username, string password, string clientId, APIVersion required)
        {
            CreateModSicChannel();
            return this.ModSicChannel.LoginEx(username, password, clientId, required);
        }

        public virtual SendRequestResult SendRequest(Package collectPackage, string token)
        {
            
            return this.ModSicChannel.SendRequest(collectPackage, token);
        }

        public virtual void Logout(string token)
        {
            this.ModSicChannel.Logout(token);
        }

        public virtual Boolean CancelCollect(string collectRequestID, string token)
        {
            return this.ModSicChannel.CancelCollect(collectRequestID, token);
        }

        public virtual byte[] GetCertificate(string token)
        {
            return this.ModSicChannel.GetCertificate(token);
        }

        public virtual Result GetCollectedResultDocument(string collectRequestID, string token)
        {
            return this.ModSicChannel.GetCollectedResultDocument(collectRequestID, token);
        }

        public virtual string GetOvalResultDocument(string collectRequestID, string token)
        {
            return this.ModSicChannel.GetOvalResultDocument(collectRequestID, token);
        }

        public virtual CollectInfo[] GetCollectRequestsInExecution(string token)
        {
            return this.ModSicChannel.GetCollectRequestsInExecution(token);
        }

        public virtual CollectInfo[] GetCollectRequestsInExecution(string clientId, string token)
        {
            return this.ModSicChannel.GetClientCollectRequestsInExecution(clientId, token);
        }

        public virtual String GetFullCompressedLog(string collectRequestID, string token)
        {
            return this.ModSicChannel.GetFullCompressedExecutionLog(collectRequestID, token);
        }

        private void CreateModSicChannel()
        {
            if (this.ModSicChannel == null)
            {
                var binding = ServiceConfigurationHelper.CreateWsHttpBinding();
                var endpoint = ServiceConfigurationHelper.CreateEndpointAddressFromURL();
                this.ModSicChannel = new ChannelFactory<ICollectService>(binding, endpoint).CreateChannel();
            }
        }

        public virtual TargetCheckingResult CheckTargetAvailability(TargetPlatforms targetPlatform, string address, string encryptedCredentials, string sshPort, string token)
        {
            return this.ModSicChannel.CheckTargetAvailability(targetPlatform, address, encryptedCredentials, sshPort, token);
        }
    }

    public class ModSicCallingException : Exception 
    {
        public ModSicCallingException(string originalExceptionMessage)
            : base(originalExceptionMessage) { } 
    }
}
