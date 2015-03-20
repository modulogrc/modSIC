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

using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Modulo.Collect.Service.Assemblers;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Entities;
using Raven.Client;

namespace Modulo.Collect.Service.Data
{
    public class CollectRequestRepository : ICollectRequestRepository
    {
        private IDataProvider DataProvider;

        [InjectionConstructor]
        public CollectRequestRepository(IDataProvider dataProvider)
        {
            DataProvider = dataProvider;
        }

        public IDocumentSession GetSession()
        {
            return DataProvider.GetSession();
        }

        public IQueryable<CollectRequest> GetOpenCollectRequests(IDocumentSession session)
        {
            return session.Query<CollectRequest>("OpenedRequests").Customize(x => x.WaitForNonStaleResults());               
        }

        public CollectPackage[] GetCollectPackages(IDocumentSession session, params string[] ids)
        {
            return session.Load<CollectPackage>(ids.Distinct());
        }
        
        public void SaveChanges(IDocumentSession session)
        {
            session.SaveChanges();
        }

        #region ICollectRequestRepository Members

        public string GetCollectResult(IDocumentSession session, string id)
        {
            var collectRequest = this.GetCollectRequest(session, id);
            if ((collectRequest.Result != null) && (collectRequest.Result.Status != CollectStatus.Error))
            {
                return collectRequest.Result.SystemCharacteristics;
            }
            return string.Empty;
        }

        public CollectRequest GetCollectRequest(IDocumentSession session, string id)
        {
            if (id != null)
            {
                CollectRequest collectRequest = session.Load<CollectRequest>(id);
                return collectRequest;
            }
            else
            {
                return null;
            }
        }

        public CollectRequest[] GetCollectRequests(IDocumentSession session, params string[] ids)
        {
            var oids = ids.Distinct();
            var requests = session.Load<CollectRequest>(oids);
            return requests;
        }

        public DefinitionDocument GetDefinitionByOriginalId(IDocumentSession session, string id)
        {
            return session.Query<DefinitionDocument>().FirstOrDefault(x => x.OriginalId == id);
        }

        public DefinitionDocument GetDefinitionByDocumentId(IDocumentSession session, string id)
        {
            return session.Load<DefinitionDocument>(id);
        }

        public OvalResultsDocument GetOvalResults(IDocumentSession session, string id)
        {
            return session.Query<OvalResultsDocument>().FirstOrDefault(x => x.RequestId == id);
        }

        public void SaveOvalResults(IDocumentSession session, string id, string text)
        {
            var ovalResult = new OvalResultsDocument();
            ovalResult.RequestId = id;
            ovalResult.Text = text;

            session.Store(ovalResult);
            session.SaveChanges();
        }

        public AuthorizationInfo GetAuthorizationInfo(IDocumentSession session, string token, string address)
        {
            return session.Query<AuthorizationInfo>().Customize(x => x.WaitForNonStaleResults()).FirstOrDefault(x => x.Token == token && x.Address.Equals(address));
        }

        public void SaveAuthorizationInfo(string username, string token, DateTime expiration, string address, string clientId)
        {
            using (var session = DataProvider.GetSession())
            {
                var authorization = new AuthorizationInfo();

                authorization.Username = username;
                authorization.Token = token;
                authorization.Expiration = expiration;
                authorization.Address = address;
                authorization.ClientId = clientId;

                session.Store(authorization);
                session.SaveChanges();
            }
        }

        public void DeleteAuthorizationInfo(string token, string address)
        {
            using (var session = DataProvider.GetSession())
            {
                var authorization = session.Query<AuthorizationInfo>().FirstOrDefault(x => x.Token == token && x.Address.Equals(address));
                if (authorization != null)
                {
                    session.Delete<AuthorizationInfo>(authorization);
                    session.SaveChanges();
                }
            }
        }
       
        #endregion
    }
}
