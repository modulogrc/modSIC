/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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

namespace Modulo.Collect.Probe.Common.BasicClasses
{
    public class ExecutionLogBuilder
    {
        private List<ProbeLogItem> executionLogs;
        

        public ExecutionLogBuilder()
        {
            executionLogs = new List<ProbeLogItem>();
        }

        public void StartCollectOf(string probeName)
        {
            this.CreateExecutionLog(String.Format("Starting {0} Collect", probeName),TypeItemLog.Info);            
        }

        public void TryConnectToHost(string address)
        {
           this.CreateExecutionLog(String.Format("Try connect to host {0}", address), TypeItemLog.Info);
        }

        public void CollectingInformationFrom(string idObject)
        {
            this.CreateExecutionLog(String.Format("Collecting information from {0} object", idObject), TypeItemLog.Info);
        }

        public void AnErrorOccurred(string message)
        {
            this.CreateExecutionLog(message, TypeItemLog.Error);
        }

        public void ConnectedWithSuccess()
        {
            this.CreateExecutionLog("Connected with success", TypeItemLog.Info);
        }

        public void CollectingDataFrom(string element)
        {
            this.CreateExecutionLog(string.Format("Collecting data from {0}", element), TypeItemLog.Info);
        }

        public void Warning(string message)
        {
            this.CreateExecutionLog(message, TypeItemLog.Warning);
        }

        public void AddInfo(string message)
        {
            this.CreateExecutionLog(message, TypeItemLog.Info);
        }

        public void AddDetailInformation(IEnumerable<ProbeLogItem> executionLogDetail)
        {
            executionLogs.AddRange(executionLogDetail);
        }
        
        public void EndCollect()
        {
            this.CreateExecutionLog("End Collect", TypeItemLog.Info);
        }

        public void CollectSystemInformation()
        {
            this.CreateExecutionLog("Collecting system information", TypeItemLog.Info);
        }

        public void EndCollectOf(string probeName)
        {
            this.CreateExecutionLog(string.Format("End Collect of {0}", probeName), TypeItemLog.Info);
        }

        public IEnumerable<ProbeLogItem> BuildExecutionLogs()
        {
            List<ProbeLogItem> logs = new List<ProbeLogItem>(executionLogs);
            executionLogs.Clear();

            return logs;
        }

        public void ConnectedToHostWithUserName(string userName)
        {
            var userNameConfigured = string.IsNullOrEmpty(userName) ? "Default User" : userName;
            this.CreateExecutionLog(string.Format("Connected to Host with UserName: {0} ", userNameConfigured), TypeItemLog.Info);
        }

        private void CreateExecutionLog(string message, TypeItemLog type)
        {
            ProbeLogItem log = this.CreateProbeLogItem(message,type);
            executionLogs.Add(log);
        }

        private ProbeLogItem CreateProbeLogItem(string message, TypeItemLog type)
        {
            ProbeLogItem log = new ProbeLogItem();
            log.Date = DateTime.Now;
            log.Message = message;
            log.Type = type;
            return log;
        }       
    }
}
