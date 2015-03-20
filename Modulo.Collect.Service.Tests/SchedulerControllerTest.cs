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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Tests.Entities;
using Modulo.Collect.Service.Tests.Helpers;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Controllers;
using System.Threading;
using Quartz;
using Quartz.Impl;
using Raven.Client;
using Modulo.Collect.Service.Data;

namespace Modulo.Collect.Service.Tests
{
    /// <summary>
    /// Summary description for SchedulerControllerTest
    /// </summary>
    [TestClass]
    public class SchedulerControllerTest
    {
        private LocalDataProvider DataProvider = new LocalDataProvider();

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_schedule_a_request_collect_in_specific_date()
        {
            var collectRequest = new CollectRequestFactory().CreateCollectRequest(DataProvider.GetSession()).Item2;
            var scheduleInformation = new ScheduleInformation() { ScheduleDate = DateTime.Now.AddSeconds(1) };
            var scheduler = new StdSchedulerFactory().GetScheduler();
            var scheduleController = new ScheduleController(scheduler) { TypeOfExecutionJob = typeof(TestJob) };
            
            scheduleController.ScheduleCollection(collectRequest.Oid.ToString(), "", scheduleInformation.ScheduleDate);
            
            Assert.AreEqual(1, scheduleController.GetNumberOfCollectRequestScheduled());
            scheduler.Shutdown();
        }

        [TestMethod, Ignore, Owner("lcosta")]
        public void Should_be_possible_to_reschedule_the_collectRequest_that_was_not_collect()
        {
            var fakeSession = DataProvider.GetSession();
            var package1 = new CollectRequestFactory().CreateCollectRequest(fakeSession);
            var collectRequest = package1.Item2;
            this.SaveCollectRequest(collectRequest);
            var package2 = new CollectRequestFactory().CreateCollectRequest(fakeSession);
            var otherCollectRequest = package2.Item2;
            this.SaveCollectRequest(otherCollectRequest);
            package1.Item1.ScheduleInformation.ExecutionDate.AddSeconds(100);
            package2.Item1.ScheduleInformation.ExecutionDate.AddSeconds(100);
            var fakeRepository = new CollectRequestRepository(DataProvider);
            var scheduler = new StdSchedulerFactory().GetScheduler();
            var scheduleController = new ScheduleController(scheduler);
            scheduleController.TypeOfExecutionJob = typeof(TestJob);
            

            scheduleController.ReScheduleCollectRequests(
                fakeSession, 
                new[] { collectRequest, otherCollectRequest }, 
                fakeRepository);

            
            Assert.AreEqual(2, scheduleController.GetNumberOfCollectRequestScheduled(), "the number of collectRequest schedule is not expected");
            scheduler.Shutdown();
        }

        [TestMethod, Owner("lcosta")]
        public void Should_not_possible_schedule_the_collectRequest_if_already_exists_a_job_defined()
        {
            var scheduleInformation = new ScheduleInformation() { ScheduleDate = DateTime.Now.AddSeconds(1) };
            var collectRequest = new CollectRequestFactory().CreateCollectRequest(DataProvider.GetSession()).Item2;
            this.SaveCollectRequest(collectRequest);
            var scheduler = new StdSchedulerFactory().GetScheduler();
            var scheduleController = new ScheduleController(scheduler) { TypeOfExecutionJob = typeof(TestJob) };
            var collectRequestId = collectRequest.Oid.ToString();
            
            scheduleController.ScheduleCollection(collectRequestId, "", scheduleInformation.ScheduleDate);
            scheduleController.ScheduleCollection(collectRequestId, "", scheduleInformation.ScheduleDate);
            
            Assert.AreEqual(1, scheduleController.GetNumberOfCollectRequestScheduled());
            scheduler.Shutdown();
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_get_the_collectRequestIds_that_are_executing_in_the_scheduler()
        {
            var collectRequest = new CollectRequestFactory().CreateCollectRequest(DataProvider.GetSession()).Item2;
            var scheduleInformation = new ScheduleInformation() { ScheduleDate = DateTime.Now.AddSeconds(1) };
            this.SaveCollectRequest(collectRequest);
            var scheduler = new StdSchedulerFactory().GetScheduler();
            var scheduleController = new ScheduleController(scheduler) { TypeOfExecutionJob = typeof(TestJob) };
            
            scheduleController.ScheduleCollection(collectRequest.Oid.ToString(), "", scheduleInformation.ScheduleDate);
            Thread.Sleep(1000);
            
            var collectRequestIds = scheduleController.GetCollectRequestIdRunning();
            Assert.IsTrue(collectRequestIds.Count() > 0);
            scheduler.Shutdown();
        }

        private string SaveCollectRequest(CollectRequest collectRequest)
        {
            try
            {
                return collectRequest.Oid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
