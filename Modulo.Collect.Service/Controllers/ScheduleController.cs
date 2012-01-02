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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Service.Entities;
using Quartz;
using Raven.Client;

namespace Modulo.Collect.Service.Controllers
{
    public class JobInfoFields
    {
        public static string GROUP_NAME = "CollectService";
        public static string REQUEST_ID = "RequestId";
        public static string TARGET_ADDRESS = "TargetAddress";
        public static string TARGET_IP = "TargetIp";
        public static string TARGET_HOSTNAME = "TargetHostname";
    }
    

    public class ScheduleController : Modulo.Collect.Service.Controllers.IScheduleController
    {
        public Type TypeOfExecutionJob { get; set; }

        public Boolean ReschedulingAlreadyExecuted { get; set; }

        private IScheduler Scheduler;
        
        public ScheduleController(IScheduler scheduler)
        {
            this.TypeOfExecutionJob = typeof(CollectExecutionJob);
            this.Scheduler = scheduler;
            if (!this.Scheduler.IsStarted)
                this.Scheduler.Start();
        }

        public void ScheduleCollection(
            string collectRequestId, string targetAddress, DateTime date)
        {
            if (!this.IsThereJobForCollectRequest(collectRequestId))
            {
                var job = this.NewJob(collectRequestId, targetAddress);
                var trigger = new SimpleTrigger(collectRequestId, JobInfoFields.GROUP_NAME, date);
                this.Scheduler.ScheduleJob(job, trigger);
            }
        }

                /// <summary>
        /// Reschedule the collectRequest based on in scheduleInformation of CollectPackage
        /// </summary>
        /// <param name="collectRequests">The request collects.</param>
        public void ReScheduleCollectRequests(
            IDocumentSession session, 
            IEnumerable<CollectRequest> collectRequests, 
            ICollectRequestRepository requestRepository)
        {
            var packageIds = collectRequests.Select(x => x.CollectPackageId).ToArray();
            var packages = requestRepository.GetCollectPackages(session, packageIds);
            foreach (var collectRequest in collectRequests)
            {
                var package = packages.Where(x => x.Oid == collectRequest.CollectPackageId).SingleOrDefault();
                if (package != null)
                {
                    var collectRequestID = collectRequest.Oid.ToString();
                    var targetAddress = collectRequest.Target.Address;
                    var startsOn =package.ScheduleInformation.ExecutionDate;
                    this.ScheduleCollection(collectRequestID, targetAddress, startsOn);
                }
            }
        }

        public bool CancelCollection(string IdOfCollectRequest)
        {
            var collectRequestIds = GetCollectRequestIdRunning();

            if (collectRequestIds.Contains(IdOfCollectRequest))
                return this.Scheduler.Interrupt(IdOfCollectRequest, JobInfoFields.GROUP_NAME);
            else
                return this.Scheduler.DeleteJob(IdOfCollectRequest, JobInfoFields.GROUP_NAME);
        }

        /// <summary>
        /// Get the requestcollect currently running on Scheduler
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCollectRequestIdRunning()
        {
            var runningJobs = this.Scheduler.GetCurrentlyExecutingJobs();
            var runningJobsContext = runningJobs.OfType<JobExecutionContext>();
            return
                from job in runningJobsContext
                select job.JobDetail.JobDataMap[JobInfoFields.REQUEST_ID].ToString();
        }

        /// <summary>
        /// Get all requestcollect currently on Scheduler
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DateTime> GetCollectRequestIdList()
        {
            var groupName = JobInfoFields.GROUP_NAME;
            var jobNames = this.Scheduler.GetJobNames(groupName);

            return
                jobNames
                    .ToDictionary(
                        x => x,
                        x => Scheduler.GetTrigger(x, groupName).StartTimeUtc);
        }

        /// <summary>
        /// Get all scheduled requests
        /// </summary>
        /// <returns>
        /// A dictionary which on the key is the Collect Request ID and the value is
        /// a key/value pair which on the key is the target address and the value is the fire time.
        /// </returns>
        public Dictionary<string, KeyValuePair<string, DateTime>> GetAllScheduledRequests()
        {
            Func<string, string> getTargetAddress = 
                (jobName) => 
                    Scheduler.GetJobDetail(jobName, JobInfoFields.GROUP_NAME)
                        .JobDataMap
                            .GetString(JobInfoFields.TARGET_ADDRESS);

            Func<string, DateTime> getFireTime =
                (jobName) => 
                    Scheduler.GetTrigger(jobName, JobInfoFields.GROUP_NAME)
                        .StartTimeUtc;

            return 
                this.Scheduler
                    .GetJobNames(JobInfoFields.GROUP_NAME)
                        .ToDictionary(
                            jobName => 
                                jobName,
                            jobName => 
                                new KeyValuePair<string, DateTime>(
                                    getTargetAddress(jobName), getFireTime(jobName)));
        }

        public int GetNumberOfCollectRequestScheduled()
        {
            return this.Scheduler.GetTriggerNames(JobInfoFields.GROUP_NAME).Count();
        }


        private bool IsThereJobForCollectRequest(String IdOfCollectRequest)
        {
            return this.Scheduler.GetJobDetail(IdOfCollectRequest, JobInfoFields.GROUP_NAME) != null;
        }

        private JobDetail NewJob(string idOfCollectRequest, string targetAddress)
        {
            var jobDetail = new JobDetail(idOfCollectRequest, JobInfoFields.GROUP_NAME, this.TypeOfExecutionJob);
            jobDetail.JobDataMap[JobInfoFields.REQUEST_ID] = idOfCollectRequest;
            jobDetail.JobDataMap[JobInfoFields.TARGET_ADDRESS] = targetAddress;
            // #ToDo : change to resolve the address in order to find out the IP address and the Target hostname.
            jobDetail.JobDataMap[JobInfoFields.TARGET_IP] = targetAddress;
            jobDetail.JobDataMap[JobInfoFields.TARGET_HOSTNAME] = targetAddress;

            return jobDetail;
        }
    }
}
