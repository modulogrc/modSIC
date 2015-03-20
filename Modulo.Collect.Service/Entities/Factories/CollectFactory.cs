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
using System.Collections.Generic;
using Modulo.Collect.Probe.Common.BasicClasses;
using Raven.Client;

namespace Modulo.Collect.Service.Entities
{
    public class CollectFactory
    {

        public CollectFactory()
        {

        }

        public CollectFactory(IDocumentSession session)
        {

        }

        public CollectExecution CreateCollectExecution(IDocumentSession session, CollectRequest collectRequest)
        {
            CollectExecution collect = new CollectExecution();
            collect.RequestId = collectRequest.Oid;

            session.Store(collect);
            session.SaveChanges();

            return collect;
        }

        public ProbeExecution CreateAProbeExecution(ProbeResult probeResult, string capability)
        {
            ProbeExecution probe = new ProbeExecution();
            probe.Capability = capability;
            this.SetExecutionLog(probeResult.ExecutionLog, probe);
            return probe;
        }

        /// <summary>
        /// Creates a probe execution with error status.
        /// This is necessary because in the execution of the collect, can be happen an 
        ///  error and should be possible to track the error.
        /// </summary>
        /// <param name="capability">The capability.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public ProbeExecution CreateAProbeExecutionWithError(string capability, string errorMessage)
        {
            ProbeExecution probe = new ProbeExecution();
            probe.Capability = capability;
            probe.ExecutionLogs.Add(this.CreateExecutionLog(DateTime.Now, errorMessage, LogType.Error));
            return probe;
        }

        /// <summary>
        /// Create a probe execution with error status and detail log entries, provided by LogEntries parameter;
        /// </summary>
        /// <param name="p">The probe capabality</param>
        /// <param name="iEnumerable">the set of log entries.</param>
        /// <returns></returns>
        public ProbeExecution CreateAProbeExecutionWithError(string capability, IEnumerable<ProbeLogItem> logEntries)
        {
            ProbeExecution probe = new ProbeExecution();
            probe.Capability = capability;
            this.SetExecutionLog(logEntries, probe);
            return probe;

        }

        private void SetExecutionLog(IEnumerable<ProbeLogItem> probeLogs, ProbeExecution probeExecution)
        {
            foreach (ProbeLogItem logItem in probeLogs)
            {
                if (logItem == null)
                    continue;

                try
                {
                    LogType type = (LogType)Enum.Parse(typeof(LogType), logItem.Type.ToString(), true);
                    CollectExecutionLog executionLog = CreateExecutionLog(logItem.Date, logItem.Message, type);
                    probeExecution.ExecutionLogs.Add(executionLog);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        private CollectExecutionLog CreateExecutionLog(DateTime date, string message, LogType type)
        {
            CollectExecutionLog executionLog = new CollectExecutionLog();
            executionLog.Date = date;
            executionLog.Message = message;
            executionLog.Type = type;
            return executionLog;
        }
    }
}
