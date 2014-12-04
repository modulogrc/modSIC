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

using System;
using System.Management;
using System.Runtime.InteropServices;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Exceptions;

namespace Modulo.Collect.Probe.Windows
{
    public class WMIConnectionProvider : IConnectionProvider
    {
        private const string PROVIDER_CONNECTING_ERROR_MESSAGE = "[WMIConnectionProvider] - Error connecting to host: {0}: {1}";
        private const string INVALID_CREDENTIALS_ERROR_MESSAGE = "[WMIConnectionProvider] - Error while trying to connect to '{0}': Invalid Credentials (domain = {1} / username = {2}";

        private string managementPath;
        public String ManagementPath { get { return this.managementPath; } set { this.managementPath = value; } }

        public ManagementScope ConnectionScope { get; private set; }


        public WMIConnectionProvider() { }

        public WMIConnectionProvider(string managementPath)
        {
            this.managementPath = managementPath;
        }

        /// <summary>
        /// Tries to connect to the specified host trought WMI.
        /// </summary>
        /// <param name="target">Host to Connect</param>
        public void Connect(TargetInfo target)
        {
            try
            {
                var targetAddress = target.GetAddress();
                var credentials = target.IsLocalTarget() ? null : target.credentials;
                var options = this.GetConnectionOptions(credentials);

                this.ConnectionScope = this.GetManagementScope(options, targetAddress);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                var credentials = target.credentials;
                string errorMessage = string.Format(INVALID_CREDENTIALS_ERROR_MESSAGE, target.GetAddress(), credentials.GetDomain(), credentials.GetUserName() + " FullyQualified: '" + credentials.GetFullyQualifiedUsername());
                throw new InvalidCredentialsException(target.credentials, errorMessage, unauthorizedAccessException);
            }
            catch (COMException comException)
            {
                string errorMessage = string.Format(PROVIDER_CONNECTING_ERROR_MESSAGE, target.GetAddress(), comException.Message);
                throw new CannotConnectToHostException(target, errorMessage, comException);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format(PROVIDER_CONNECTING_ERROR_MESSAGE, target.GetAddress(), ex.Message);
                throw new ProbeException(errorMessage, ex);
            }
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsConnected()
        {
            return ((this.ConnectionScope != null) && (this.ConnectionScope.IsConnected));
        }

        private bool IsThereCredential(Credentials credentials)
        {
            return ((credentials != null) && (credentials.Count > 0));
        }

        private ConnectionOptions GetConnectionOptions(Credentials credentials)
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;

            if (this.IsThereCredential(credentials))
            {
                options.Username = credentials.GetFullyQualifiedUsername();
                options.Password = credentials.GetPassword();
            }

            return options;
        }

        private ManagementScope GetManagementScope(ConnectionOptions options, string address)
        {
            var managementPath = new ManagementPath(string.Format(@"\\{0}\root\{1}", address, this.managementPath));
            var connectScope = new ManagementScope() { Path = managementPath, Options = options };
            
            connectScope.Connect();                
            
            return connectScope;
        }
       
    }    
}
