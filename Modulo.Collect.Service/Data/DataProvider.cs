#region License
/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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

using Raven.Client;
using Raven.Client.Indexes;
using Raven.Database.Server;
using System.Configuration;
using System.Net.NetworkInformation;
using Raven.Client.Embedded;

namespace Modulo.Collect.Service.Data
{
    public class DataProvider : IDataProvider
    {
        static object isStoreSet = false;
        
        static EmbeddableDocumentStore documentStore;

        private bool CheckPortIsAvailable(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            
            // Getting listening ports
            foreach (var tcpi in ipGlobalProperties.GetActiveTcpListeners())
                if (tcpi.Port == port)
                    return false;
            // Getting established ports
            foreach (var tcpi in ipGlobalProperties.GetActiveTcpConnections())
                if (tcpi.LocalEndPoint.Port == port)
                    return false;

            return true;
        }
        
        public IDocumentSession GetSession()
        {
            lock (isStoreSet)
            {
                if (!(bool)isStoreSet)
                {
                    RavenConfiguration ravenConfiguration = null;

                    bool webUIEnabled = false;
                    int webUIPort = 0;

                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var serviceConfigurationSection = config.GetSection("ServiceConfigurationSection") as ServiceConfigurationSection;
                    if (serviceConfigurationSection != null)
                    {
                        ravenConfiguration = serviceConfigurationSection.ravendb;
                        if (ravenConfiguration != null)
                        {
                            webUIEnabled = ravenConfiguration.WebUIEnabled;
                            webUIPort = ravenConfiguration.WebUIPort;

                            if (webUIEnabled)
                            {
                                if (webUIPort < 0 || webUIPort > 65535)
                                {
                                    webUIEnabled = false;
                                }
                                else
                                {
                                    webUIEnabled = CheckPortIsAvailable(webUIPort);
                                }
                            }
                        }
                    }

                    documentStore = new EmbeddableDocumentStore();
                    documentStore.Configuration.DataDirectory = "Data\\RavenDB";
                    documentStore.Configuration.DefaultStorageTypeName = typeof(Raven.Storage.Esent.TransactionalStorage).AssemblyQualifiedName;
                    documentStore.Conventions.FindIdentityProperty = x => x.Name == "Oid";
                    documentStore.Conventions.MaxNumberOfRequestsPerSession = 1000;
                    documentStore.Initialize();
                    
                    IndexCreation.CreateIndexes(GetType().Assembly, documentStore);

                    if (webUIEnabled)
                    {
                        documentStore.Configuration.Port = webUIPort;
                        var httpServer = new HttpServer(documentStore.Configuration, documentStore.DocumentDatabase);
                        httpServer.Init();
                        httpServer.StartListening();
                    }

                    isStoreSet = true;
                }
            }

            return documentStore.OpenSession();
        }

        public ITransaction GetTransaction(IDocumentSession session)
        {
            return new Transaction(session);
        }     
    }
}
