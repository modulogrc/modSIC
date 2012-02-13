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
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Service.Contract.Security;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Service.Security;
using Quartz;
using Raven.Client;
using Modulo.Collect.Service.Infra;
using NLog;
using Modulo.Collect.OVAL.Helper;

namespace Modulo.Collect.Service.Controllers
{
    public class CollectExecutionJob : IJob, IInterruptableJob
    {
        private const string COLLECT_JOB_STARTING_ERROR = 
            "An error occurred while trying to start collect job:\r\nMesage:'{0}'\r\nStack:\r\n{1}";
        private const string ERROR_ON_CHECKING_IF_TARGET_IS_ALREADY_BEING_COLLECTED =
            "An error occurred while trying to check if a target is already being collected. Thus, the target will be collected.Collect Request ID:{0}\r\nTarget Address:{1}\r\nError:'{2}'\r\nStack:{3}.";
        private static Logger Logger = LogManager.GetCurrentClassLogger();


        public ICollectRequestRepository Repository { get; set; }
        
        public CollectExecutionManager CollectManager { get; set; }

        public CollectExecutionJob(ICollectRequestRepository repository, CollectExecutionManager collectManager)
        {
            this.Repository = repository;
            this.CollectManager = collectManager;
        }

        public void Execute(JobExecutionContext context)
        {
            var requestId = context.MergedJobDataMap.GetString(JobInfoFields.REQUEST_ID);
            try
            {
                if (string.IsNullOrEmpty(requestId))
                    return;

                if (IsTargetAlreadyBeingCollected(context))
                    Postpone(context);
                else
                    ExecuteCollect(requestId);
            }
            catch (Exception ex)
            {
                LogErrorOnJobStarting(ex);
                CreateCollectionExecutionWithError(requestId, ex.Message);
            }
        }

        private bool IsTargetAlreadyBeingCollected(JobExecutionContext jobExecutionContext)
        {
            var candidateCollectRequestId = string.Empty;
            var candidateTargetAddress = string.Empty;
            try
            {
                candidateCollectRequestId = jobExecutionContext.MergedJobDataMap.GetString(JobInfoFields.REQUEST_ID);
                candidateTargetAddress = jobExecutionContext.MergedJobDataMap.GetString(JobInfoFields.TARGET_ADDRESS);
                Func<string, string, bool> requestsByTargetAddres =
                    (requestId, targetAddress) =>
                        requestId != candidateCollectRequestId && targetAddress == candidateTargetAddress;

                var schedulerController = new ScheduleController(jobExecutionContext.Scheduler);
                var allScheduledCollectionByTargetAddress =
                    schedulerController
                        .GetAllScheduledRequests()
                            .Where(req => requestsByTargetAddres(req.Key, req.Value.Key));
                
                if (allScheduledCollectionByTargetAddress.Count() == 0)
                    return false;

                var runningCollectRequestsIds = allScheduledCollectionByTargetAddress.Select(x => x.Key);
                using (var session = Repository.GetSession())
                {
                    var collectRequestsInExecution =
                        Repository
                            .GetCollectRequests(session, runningCollectRequestsIds.ToArray())
                            .Where(req => req.Status == Contract.CollectRequestStatus.Executing);

                    return collectRequestsInExecution.Count() > 0;
                }
            }
            catch (Exception ex)
            {
                LogError(
                    ERROR_ON_CHECKING_IF_TARGET_IS_ALREADY_BEING_COLLECTED, 
                    candidateCollectRequestId, 
                    candidateTargetAddress, 
                    ex.Message, 
                    ex.StackTrace);
                
                return false;
            }
        }

        private void Postpone(JobExecutionContext context)
        {
            var jobsByTargetAddress =
                context.Scheduler
                    .GetCurrentlyExecutingJobs()
                    .OfType<JobExecutionContext>()
                    .Where(job => job.MergedJobDataMap.GetString(JobInfoFields.TARGET_ADDRESS)
                                    .Equals(context.MergedJobDataMap.GetString(JobInfoFields.TARGET_ADDRESS)));

            var nextFiretime = DateTime.UtcNow.AddMinutes(1); 
            if (jobsByTargetAddress.Count() > 0)
                nextFiretime = jobsByTargetAddress.Max(job => job.Trigger.StartTimeUtc).AddMinutes(1);
            context.Trigger.StartTimeUtc = nextFiretime;

            var triggerName = context.Trigger.Name;
            var groupName = context.Trigger.Group;
            var nextFire = context.Scheduler.RescheduleJob(triggerName, groupName, context.Trigger);
            if (nextFire == null)
                throw new ApplicationException("Unable to postpone Job " + triggerName);

            var postponedCollectRequest = context.MergedJobDataMap.GetString(JobInfoFields.REQUEST_ID);
            UpdateCollectRequestStatus(postponedCollectRequest);
            
            var localTime = ((DateTime)nextFire).ToLocalTime().ToString();
            Console.WriteLine("INFO  - Job {0} postponed until {1:yyyy-MM-dd HH:mm:ss.f} UTC (Local Time: {2})", triggerName, nextFire, localTime);
        }

        private void ExecuteCollect(string collectRequestID)
        {
            var localNow = DateTime.UtcNow.ToLocalTime().ToString();
            Console.WriteLine("INFO - Job {0} will be started now ({1})...", collectRequestID, localNow);
            using (var session = Repository.GetSession())
            {
                var collectRequest = Repository.GetCollectRequest(session, collectRequestID);
                if (collectRequest != null)
                {
                    collectRequest.SetExecutingStatus();
                    session.SaveChanges();
                    
                    var targetInfo = GetTargetInformation(collectRequest);
                    var ovalDefinitonsObjects = collectRequest.GetObjectTypes(session).ToArray();
                    var targetPlataform = new TargetPlatformDiscoverer(ovalDefinitonsObjects).Discover();
                    
                    this.CollectManager.Target = targetInfo;
                    this.CollectManager.connectionContext = new List<IConnectionProvider>();
                    this.CollectManager.ExecuteCollect(session, collectRequest, targetPlataform);
                    session.SaveChanges();                    
                }
            }
            Console.WriteLine("INFO - Collection {0} just finished.", collectRequestID);
        }

        private void UpdateCollectRequestStatus(string collectRequestId)
        {
            using (var session = Repository.GetSession())
            {
                var collectRequest = Repository.GetCollectRequest(session, collectRequestId);
                collectRequest.Reopen();
                session.SaveChanges();
            }
        }

        private TargetInfo GetTargetInformation(Entities.CollectRequest collectRequest)
        {
            var portParam = collectRequest.Target.GetTargetParameterByName("sshPort");
            var portNum = string.Empty;
            if (portParam != null && !String.IsNullOrWhiteSpace(portParam.ParameterValue))
                portNum = portParam.ParameterValue;

            return
                new Modulo.Collect.Service.Server.TargetInfoExtractor()
                    .GetTargetInformation(
                        collectRequest.Target.Credential.CredentialInfo,
                        collectRequest.Target.Address, portNum);

            //var credentialInfo = collectRequest.Target.Credential.CredentialInfo;
            //var certificate = new CertificateFactory().GetCertificate();
            
            //var deserializedCredentials = 
            //    new CollectServiceCryptoProvider()
            //        .DecryptCredentialBasedOnCertificateOfServer(credentialInfo, certificate);

            //return 
            //    new TargetInfoFactory(
            //        collectRequest.Target.Address,
            //        deserializedCredentials.Domain,
            //        deserializedCredentials.UserName,
            //        deserializedCredentials.Password,
            //        deserializedCredentials.AdministrativePassword).Create();
        }

        private void LogErrorOnJobStarting(Exception ex)
        {
            var logMessage = String.Format(COLLECT_JOB_STARTING_ERROR, ex.Message, ex.StackTrace);
            Logger.Error(logMessage);
        }

        private void LogError(string logMessageTemplate, params string[] args)
        {
            var logMessage = String.Format(logMessageTemplate, args);
            Logger.Error(logMessage);
        }


        private void CreateCollectionExecutionWithError(string requestId, string errorMessage)
        {
            using (var session = Repository.GetSession())
            {
                var executionLogBuilder = new Probe.Common.BasicClasses.ExecutionLogBuilder();
                executionLogBuilder.AnErrorOccurred(String.Format("An error occurred while collect execution (see server log for more details): '{0}'", errorMessage));
                executionLogBuilder.EndCollect();
                var logEntries = executionLogBuilder.BuildExecutionLogs();
                var collectFactory = new Entities.CollectFactory(session);
                var executionWithError = collectFactory.CreateAProbeExecutionWithError("", logEntries);

                var collectRequest = Repository.GetCollectRequest(session, requestId);
                var collectExecution = collectFactory.CreateCollectExecution(session, collectRequest);
                collectExecution.ProbeExecutions.Add(executionWithError);
                collectRequest.SetResultError();
                collectRequest.Close();

                session.SaveChanges();
            }
        }


        #region IInterruptableJob Members

        public void Interrupt()
        {
            if (CollectManager != null )
            {
                CollectManager.Interrupted = true;
            }
        }

        #endregion
    }
}
