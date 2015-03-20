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
using Quartz;
using Quartz.Impl;
using Modulo.Collect.Service.Contract;
using System.Threading;
using Modulo.Collect.Service.Tests.Helpers;

namespace Modulo.Collect.Service.Tests
{
    /// <summary>
    /// Summary description for LearningToUseQuartzTest
    /// </summary>
    [TestClass]
    public class LearningToUseQuartzTest
    {
        public LearningToUseQuartzTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private JobExecution jobExecution;
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             this.jobExecution = JobExecution.GetInstance();
             jobExecution.Executions.Clear();
         }
        
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

         [Ignore, Owner("lcosta")]
        public void Shoud_be_possible_to_schedule_a_job()
        {
            IScheduler scheduler = GetScheduler();
            scheduler.Start();
            
            //building a job
            JobDetail job = new JobDetail("FirstJob", null, typeof(TestJob));            

            //create and configure a Trigger            
            Trigger trigger = TriggerUtils.MakeImmediateTrigger(1, TimeSpan.FromSeconds(1));  // this job will be executed 2 times. Because the repeate parameter is 1.
            trigger.StartTimeUtc = DateTime.UtcNow;
            trigger.Name = "TriggerForTests";

            Assert.AreEqual(0, jobExecution.Executions.Count());
            //start process
            scheduler.ScheduleJob(job, trigger);            
            Thread.Sleep(5000);
            Assert.IsTrue(this.jobExecution.Executions.Count() > 0);
            Assert.AreEqual(2,jobExecution.Executions.Count());           
        }

        private IScheduler GetScheduler()
        {
            //creating and configuring the scheduler 
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = schedulerFactory.GetScheduler();
            return scheduler;
        }

        [Ignore, Owner("lcosta")]
        public void Should_be_possible_to_schedule_a_job_to_3_seconds_from_now()
        {
            IScheduler scheduler = this.GetScheduler();
            scheduler.Start();
            JobDetail job = new JobDetail("Scheduler job", null, typeof(TestJob));

            Trigger trigger = TriggerUtils.MakeSecondlyTrigger(3, 0);
            trigger.Name = "TriggerForTest";
            trigger.StartTimeUtc = DateTime.UtcNow.AddSeconds(3);

            scheduler.ScheduleJob(job, trigger);            
            Thread.Sleep(8000); // because the time of the job is 4 seconds
            Assert.IsTrue(this.jobExecution.Executions.Count() > 0);
            Assert.AreEqual(1, this.jobExecution.Executions.Count());

        }

        [Ignore, Owner("lcosta")]
        [ExpectedException(typeof(SchedulerException))]
        public void The_trigger_name_cannot_be_null()
        {
            IScheduler scheduler = this.GetScheduler();
            JobDetail job = new JobDetail("Scheduler job", null, typeof(TestJob));

            Trigger trigger = TriggerUtils.MakeSecondlyTrigger(3, 0);
            trigger.StartTimeUtc = DateTime.UtcNow;

            scheduler.ScheduleJob(job, trigger);
        }

    }           

}
