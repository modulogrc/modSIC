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
using System.Net;

namespace Modulo.Collect.Service.Client.Internal
{
    public class ServiceConfigurationHelper
    {
        public string ServiceURL { get; private set; }

        public ServerCertificateManager ServerCertificateManager { get; set; }

        /// <summary>
        /// It creates necessary structs to create a .NET Service Channel.
        /// </summary>
        /// <param name="serviceURL">The complete url of desired service.</param>
        /// <param name="certificateManager">In a unit testi pass a mock for Server Certificate Manager. Do not use this argument in production code.</param>
        public ServiceConfigurationHelper(string serviceURL, ServerCertificateManager certificateManager = null)
        {
            this.ServiceURL = serviceURL;
            this.ServerCertificateManager = certificateManager ?? new ServerCertificateManager();
        }

        public EndpointAddress CreateEndpointAddressFromURL()
        {
            return new EndpointAddress(new Uri(ServiceURL).ToString());
        }

        public WSHttpBinding CreateWsHttpBinding()
        {
            var newBinding = new WSHttpBinding()
            {
                MaxBufferPoolSize = Int32.MaxValue,
                MaxReceivedMessageSize = Int32.MaxValue,
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                {
                    MaxArrayLength = Int32.MaxValue,
                    MaxBytesPerRead = Int32.MaxValue,
                    MaxNameTableCharCount = Int32.MaxValue,
                    MaxStringContentLength = Int32.MaxValue
                }
            };

            if (this.IsSecureChannel())
            {
                this.ServerCertificateManager.CreateCertificateValidationCallback();
                newBinding.Security.Mode = SecurityMode.Transport;
            }
            else
            {
                newBinding.Security.Mode = SecurityMode.None;
            }

            return newBinding;
        }

        public bool IsSecureChannel()
        {
            return this.ServiceURL.Contains("https");
        }

        
    }

    public class ServerCertificateManager
    {
        public virtual void CreateCertificateValidationCallback()
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
        }
    
    }
}
