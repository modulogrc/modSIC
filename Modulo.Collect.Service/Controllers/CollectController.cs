#region License
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
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Modulo.Collect.Service.Assemblers;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Entities.Factories;
using Modulo.Collect.Service.Exceptions;
using Modulo.Collect.Service.Probes;
using Modulo.Collect.Service.Security;
using Quartz;
using Raven.Client;
using Modulo.Collect.OVAL.Definitions;
using System.Net;
using NLog;

namespace Modulo.Collect.Service.Controllers
{
    public class CollectController
    {
        private const int EXPIRY_SECONDS = 180;

        public IProbeManager ProbeManager { get; set; }
        public ICollectRequestRepository Repository { get; set; }
        public IScheduleController ScheduleController { get; set; }
        public IScheduler Scheduler { get; set; }
        ICollectResultAssembler ResultAssembler { get; set; }
        ICollectRequestAssembler CollectAssembler { get; set; }
        ICollectPackageAssembler PackageAssembler { get; set; }
        IDefinitionDocumentFactory DefinitionFactory { get; set; }
        private UsersCollection users { get; set; }

        

        public CollectController(
            ICollectRequestRepository repository,
            IScheduler scheduler,
            IScheduleController schedulerController,
            IProbeManager probeManager,
            ICollectRequestAssembler collectAssembler,
            ICollectResultAssembler resultAssembler,
            ICollectPackageAssembler packageAssembler,
            IDefinitionDocumentFactory definitionDocumentFactory)
        {
            this.Scheduler = scheduler;
            this.Repository = repository;
            this.ProbeManager = probeManager;
            this.ScheduleController = schedulerController;
            this.CollectAssembler = collectAssembler;
            this.ResultAssembler = resultAssembler;
            this.PackageAssembler = packageAssembler;
            this.DefinitionFactory = definitionDocumentFactory;
            this.ProbeManager.LoadProbes();

            if (!this.ScheduleController.ReschedulingAlreadyExecuted)
                this.ReScheduleCollectRequestWasNotExecuted();
            else
                this.CheckForTimeout();


            ReadUsersSection();
        }


        /// <summary>
        /// Requests the service to execute the collect and return the data of requested collect. 
        /// The format of data is: [RequestID], [CollectID], where:
        /// - RequestId - Identifier the origin of collect
        /// - CollectId - Identifier of collect requested
        /// </summary>
        /// <param name="collectRequest">The collect request.</param>
        /// <returns></returns>
        public Dictionary<string, string> CollectRequest(Package collectPackage, string clientId)
        {
            using (var session = Repository.GetSession())
            {
                ValidatePackage(collectPackage);

                var result = SaveTheCollectRequest(session, collectPackage, clientId);
                ScheduleExecution(result, collectPackage);

                return result;
            }
        }

        private void ValidatePackage(Package collectPackage)
        {
            var validateSchematron = ModsicConfigurationHelper.IsSchematronValidationSet();
            var collectRequestValidator = new CollectRequestValidator(collectPackage);
            collectRequestValidator.ValidateOvalDefinitions(validateSchematron);
        }

        /// <summary>
        /// TO BE RELEASED...
        /// Return all capabilities of the collect service. A Capabilitie is defined by
        /// the type of Oval Object and the plataform.
        /// </summary>
        /// <returns>A dictionary with the format [OvalType],[Plataform] </returns>
        public Dictionary<string, string> GetCapabilities()
        {
            var probeCababilities = ProbeManager.GetCapabilities();
            return this.CreateADictionaryOfProbeCapabilities(probeCababilities);
        }

        /// <summary>
        /// Return the digital certificate of the collect server.
        /// </summary>
        /// <returns></returns>
        public byte[] GetCertificate()
        {
            return new CertificateFactory().GetPublicCertificateInBytes();
        }

        /// <summary>
        /// Gets the Complete Collected Document.
        /// </summary>
        public virtual Result GetCollectedResultDocument(string requestId)
        {
            using (var session = Repository.GetSession())
            {
                if (requestId != null)
                {
                    var collectRequest = Repository.GetCollectRequest(session, requestId);
                    if ((collectRequest != null) && (collectRequest.HasResult()))
                    {
                        return ResultAssembler.CreateDTOFromCollectResult(session, collectRequest);
                    }
                }
                return null;
            }
        }

        public string GetOvalResultDocument(string requestId)
        {
            using (var session = Repository.GetSession())
            {
                var ovalResults = Repository.GetOvalResults(session, requestId);
                if (ovalResults == null)
                {
                    var collectRequest = Repository.GetCollectRequest(session, requestId);
                    if ((collectRequest != null) && (collectRequest.HasResult()))
                    {
                        var collectResult = GetCollectedResultDocument(requestId);
                        if (collectResult != null)
                        {
                            var definitionDocument = this.Repository.GetDefinitionByDocumentId(session, collectRequest.OvalDefinitionsId);
                            if (collectResult.Status == CollectStatus.Error)
                            {
                                throw new Exception(collectResult.ExecutionLogs.FirstOrDefault().Message);
                            }
                            else
                            {
                                var ovalResult = new OvalDefinitionDocumentGenerator().GetDocument(collectRequest, definitionDocument.Text, collectResult);
                                if (ovalResult != null)
                                {
                                    string text = ovalResult.GetResultsXML();
                                    Repository.SaveOvalResults(session, requestId, text);
                                    return text;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return ovalResults.Text;
                }

            }
            return null;
        }

        /// <summary>
        /// Return the collectRequest in execution on scheduler
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CollectInfo> GetCollectRequestsInExecution(string clientId = null)
        {
            var collectInfos = new List<CollectInfo>();
            // var collectRequestIds = this.ScheduleController.GetCollectRequestIdRunning();
            var collectRequestIds = this.ScheduleController.GetCollectRequestIdList();

            if (collectRequestIds != null)
            {
                using (var session = Repository.GetSession())
                {
                    foreach (var id in collectRequestIds)
                    {
                        CollectRequest collectRequest = Repository.GetCollectRequest(session, id.Key);
                        CollectInfo collectInfo = CollectAssembler.CreateCollectInfoFromCollectRequest(collectRequest);
                        collectInfo.StartTime = id.Value;
                        if (String.IsNullOrEmpty(clientId) || String.IsNullOrEmpty(collectInfo.ClientId) || collectInfo.ClientId == clientId)
                            collectInfos.Add(collectInfo);
                    }
                }

            }
            return collectInfos;
        }

        private Dictionary<string, string> CreateADictionaryOfProbeCapabilities(IList<ProbeCapabilities> probeCababilities)
        {
            var capabilities = new Dictionary<string, string>();
            if (probeCababilities != null)
            {
                foreach (ProbeCapabilities capability in probeCababilities)
                {
                    var plataformName = capability.PlataformName.ToString();
                    var capabilityName = capability.OvalObject;
                    var capabilityKey = string.Format("{0}.{1}", plataformName, capabilityName);
                    capabilities.Add(capabilityKey, capabilityKey);
                }
            }

            return capabilities;
        }



        private Dictionary<string, string> SaveTheCollectRequest(IDocumentSession session, Package collectPackageDTO, string clientId)
        {
            try
            {
                var collectPackage = PackageAssembler.CreateCollectPackageFromDTO(session, collectPackageDTO);
                var identifiers = this.SaveListOfCollectRequest(session, collectPackage, collectPackageDTO.CollectRequests, collectPackageDTO.Definitions, clientId);

                return identifiers;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error ocurred when saving the Collect: {0}", ex.Message), ex);
            }
        }

        private void ReScheduleCollectRequestWasNotExecuted()
        {
            using (var session = Repository.GetSession())
            {
                var collectRequestWasNotExecuted = this.Repository.GetOpenCollectRequests(session);
                this.ScheduleController.ReScheduleCollectRequests(session, collectRequestWasNotExecuted, Repository);
                this.ScheduleController.ReschedulingAlreadyExecuted = true;
                session.SaveChanges();
            }
        }

        private Dictionary<string, string> SaveListOfCollectRequest(IDocumentSession session, CollectPackage collectPackage, Request[] collectRequests, DefinitionInfo[] definitions, string clientId)
        {
            var identifiers = new Dictionary<string, string>();
            foreach (var req in collectRequests)
            {
                var definitionDocument = Repository.GetDefinitionByOriginalId(session, req.DefinitionId);
                if (definitionDocument == null)
                {
                    definitionDocument = DefinitionFactory.CreateDefinitionDocumentFromInfo(session, definitions.First(x => x.Id == req.DefinitionId));
                }
                CollectRequest request = this.CollectAssembler.CreateCollectRequestFromDTO(req, definitionDocument.Oid);
                request.ClientId = clientId;
                session.Store(request);
                request.CollectPackageId = collectPackage.Oid;
                session.SaveChanges();

                identifiers.Add(req.RequestId, request.Oid.ToString());
            }

            return identifiers;
        }

        /// <summary>
        /// Schedules the execution using schedule information from CollectPackage.
        /// </summary>
        /// <param name="collectPackage">The collect package.</param>
        private void ScheduleExecution(Dictionary<string, string> ids, Package collectPackage)
        {
            foreach (var collectRequest in collectPackage.CollectRequests)
            {
                var collectRequestId = ids[collectRequest.RequestId];
                var collectionDueTo = collectPackage.ScheduleInformation.ScheduleDate;
                ScheduleController.ScheduleCollection(collectRequestId, collectRequest.Address, collectionDueTo);
                Wait();
            }
        }

        internal bool CancelCollection(string requestId)
        {
            using (var session = Repository.GetSession())
            {
                Entities.CollectRequest collectRequest = Repository.GetCollectRequest(session, requestId);
                if (collectRequest != null)
                {
                    var oldStatus = collectRequest.Status;

                    collectRequest.Status = CollectRequestStatus.Canceled;
                    session.SaveChanges();

                    if (oldStatus == CollectRequestStatus.Open || oldStatus == CollectRequestStatus.Executing)
                    {
                        return this.ScheduleController.CancelCollection(requestId);
                    }

                    return true;
                }

                return false;
            }
        }

        public void ReadUsersSection()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var usersSection = config.GetSection("UsersSection") as UsersSection;
            if (usersSection != null)
            {
                users = usersSection.Users;
            }
            else
            {
                users = null;
            }
        }

        public bool Authenticate(string token)
        {
            string clientId;
            return Authenticate(token, out clientId);
        }

        public bool Authenticate(string token, out string clientId)
        {
            string address = GetAddress();
            return VerifyAuthorizationToken(token, address, out clientId);
        }

        public string Login(string username, string password, string clientId)
        {
            if (username == null || password == null || users == null)
            {
                return null;
            }

            var hash = new Hash().GetAuthorizationHash(username, password);
            var userRecord = users[username];

            if ((userRecord == null) || (userRecord.Hash != hash))
            {
                return null;
            }
            else
            {
                return GetAuthorizationToken(username, hash, GetAddress(), clientId);
            }
        }

        public void Logout(string token)
        {
            Repository.DeleteAuthorizationInfo(token, GetAddress());
        }

        private string GetAuthorizationToken(string username, string hash, string address, string clientId)
        {
            Random rand = new Random((int)(DateTime.Now.Ticks % Int32.MaxValue));
            DateTime expiryTime = DateTime.Now.Add(new TimeSpan(0, 0, 0, EXPIRY_SECONDS));
            string expiryTimeStr = String.Format("{0:X4}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}",
                                          expiryTime.Year, expiryTime.Month, expiryTime.Day,
                                          expiryTime.Hour, expiryTime.Minute, expiryTime.Second, rand.Next(1000));

            var token = new Hash().GetAuthorizationHash(username, hash + expiryTimeStr + address.ToString());
            Repository.SaveAuthorizationInfo(username, token, expiryTime, address, clientId);

            return token;
        }

        private bool VerifyAuthorizationToken(string token, string address, out string clientId)
        {
            if (token == null)
            {
                clientId = null;
                return false;
            }

            using (var session = Repository.GetSession())
            {
                var authorizationInfo = Repository.GetAuthorizationInfo(session, token, address);
                if (authorizationInfo != null)
                {
                    clientId = authorizationInfo.ClientId;

                    DateTime whenExpires = authorizationInfo.Expiration;
                    DateTime dtNow = DateTime.Now;
                    if (dtNow < whenExpires)
                    {
                        DateTime aliveUntil = dtNow.Add(new TimeSpan(0, 0, 0, EXPIRY_SECONDS / 2));
                        if (aliveUntil > whenExpires)
                        {
                            authorizationInfo.Expiration = aliveUntil;
                            session.SaveChanges();
                        }

                        return true;
                    }
                }
                else
                {
                    clientId = null;
                }

                return false;
            }
        }

        private string GetAddress()
        {
            var messageProperties = OperationContext.Current.IncomingMessageProperties;
            if (messageProperties != null)
            {
                var endpointProperty = (RemoteEndpointMessageProperty)messageProperties[RemoteEndpointMessageProperty.Name];
                if (endpointProperty != null && !String.IsNullOrEmpty(endpointProperty.Address))
                {
                    if (endpointProperty.Address == "::1")
                        return "127.0.0.1";

                    return endpointProperty.Address;
                }
            }

            return "0.0.0.0";
        }

        public string GetResultExecutionLog(string requestId)
        {
            using (var session = Repository.GetSession())
            {
                if (requestId != null)
                {
                    var collectRequest = Repository.GetCollectRequest(session, requestId);
                    if ((collectRequest != null) && (collectRequest.HasResult()))
                    {
                        return ResultAssembler.CreateCompressedSerializedExecutionLog(session, collectRequest);
                    }
                }
                return null;
            }
        }

        private void Wait()
        {
            try
            {
                if (ModsicConfigurationHelper.IsSchedulerIntervalSet())
                    System.Threading.Thread.Sleep(1000);
            }
            catch
            {

            }
        }

        private void CheckForTimeout()
        {
            try
            {
                var collectiontimeout = ModsicConfigurationHelper.GetCollectionTimeout();
                if (collectiontimeout != null)
                {
                    using (var session = Repository.GetSession())
                    {
                        var collectRequestsInExecution = session.Query<CollectRequest>().Where(x => x.Status == CollectRequestStatus.Executing).ToList();
                        var runningCollections = this.ScheduleController.GetCollectRequestIdRunning().ToList();
                        foreach (var collectRequest in collectRequestsInExecution)
                        {
                            if (!runningCollections.Contains(collectRequest.Oid))
                            {
                                var collectionStartDate = GetCollectionStartDate(session, collectRequest);
                                var collectionDurationInMinutes = GetCollectionDurationInMinutes(collectionStartDate);
                                if (collectionDurationInMinutes > collectiontimeout)
                                {
                                    LogInfo("The {0} timeout expired ({1} minutes) and it will be reschedule...\r\nCurrent time: {2}\r\nCollection started on {3}",
                                        collectRequest.Oid,
                                        collectionDurationInMinutes,
                                        String.Format("{0} ( {1} )", DateTime.UtcNow.ToLongDateString(), DateTime.UtcNow.ToLongTimeString()),
                                        String.Format("{0} ( {1} )", collectionStartDate.ToLongDateString(), collectionStartDate.ToLongTimeString()));

                                    this.ScheduleController.CancelCollection(collectRequest.Oid);
                                    this.ScheduleController.ScheduleCollection(collectRequest.Oid, collectRequest.Target.Address, DateTime.UtcNow);
                                    LogInfo("  The {0} was scheduled.", collectRequest.Oid);
                                    System.Threading.Thread.Sleep(5000);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("An error occurred while trying to check timeout: '{0}'\r\nStack:\r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private DateTime GetCollectionStartDate(IDocumentSession session, CollectRequest collectRequest)
        {
            var firstExecution = collectRequest.GetCollectExecutions(session).FirstOrDefault();
            if (firstExecution != null)
                return firstExecution.StartDate.ToUniversalTime();
            
            var collectPackage = Repository.GetCollectPackages(session, new[] { collectRequest.CollectPackageId }).First();
            return collectPackage.ScheduleInformation.ExecutionDate;
        }

        private void LogError(string message, params object[] args)
        {
            Logger Logger = LogManager.GetCurrentClassLogger();
            var logMessage = String.Format(message, args);
            Logger.Error(logMessage);
        }

        private void LogInfo(string message, params object[] args)
        {
            Logger Logger = LogManager.GetCurrentClassLogger();
            var logMessage = String.Format(message, args);
            Logger.Info(logMessage);
        }


        private static int GetCollectionDurationInMinutes(DateTime collectionStartDate)
        {
            var utcNow = DateTime.UtcNow;
            var utcNowSpan = new TimeSpan(utcNow.Hour, utcNow.Minute, utcNow.Second);
            var collectionStartSpan = new TimeSpan(collectionStartDate.Hour, collectionStartDate.Minute, collectionStartDate.Second);
            var collectionTimeSpan = utcNowSpan.Subtract(collectionStartSpan);
            var collectionDurationInMinutes = Convert.ToInt32(collectionTimeSpan.TotalMinutes);
            return collectionDurationInMinutes;
        }

    }
}
