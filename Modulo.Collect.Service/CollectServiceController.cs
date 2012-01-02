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

using System;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using Modulo.Collect.OVAL.Plugins;
using Quartz.Impl;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.Net;

namespace Modulo.Collect.Service
{
    public class CollectServiceController : IDisposable
    {
        private static IUnityContainer uContainer = null;
        
        private ServiceHost serviceHost = null;

        public String ListeningURL { get; set; }

        public void start()
        {
            ListeningURL = String.Empty;
            CreateUnityContainer();
            
            if (System.IO.Directory.Exists("Plugins"))
                PluginContainer.SetPluginDirectoryPath("Plugins");

            serviceHost = new ServiceHost(typeof(Modulo.Collect.Service.CollectService));
            try
            {
                serviceHost.Open();
                ListeningURL = serviceHost.Description.Endpoints[0].ListenUri.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private WSHttpBinding CreateBindingWithCertificate()
        {
            return new WSHttpBinding()
            {
                Security = new WSHttpSecurity()
                {
                    Mode = SecurityMode.Transport,
                    Transport = new HttpTransportSecurity()
                    {
                        ClientCredentialType = HttpClientCredentialType.Certificate
                    }
                }
            };
        }

        private void CreateUnityContainer()
        {
            uContainer = new UnityContainer();
            uContainer.RegisterInstance<Quartz.ISchedulerFactory>(new StdSchedulerFactory());
            InjectFactory.SetContainer(uContainer);
        }

        public void Dispose()
        {
            this.stop();
        }

        public void stop()
        {
            if(serviceHost != null)
                serviceHost.Close();
        }
    }
}
