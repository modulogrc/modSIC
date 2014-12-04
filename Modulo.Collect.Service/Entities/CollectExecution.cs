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
using System.Linq;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Service.Entities
{
    public class CollectExecution : BaseGuidEntity
    {
        public CollectExecution ()
        {
            ProbeExecutions = new List<ProbeExecution>();
        }

        public DateTime StartDate{ get; set; } 
        
        public DateTime EndDate{ get; set; }
        
        public string RequestId { get; set; }
        
        public IList<ProbeExecution> ProbeExecutions { get; set; }

        public void SetDateStartCollect()
        {
            this.StartDate = DateTime.Now;
        }

        public void SetDateEndCollect()
        {
            this.EndDate = DateTime.Now;
        }



        /// <summary>
        /// Gets the system characteristics of all probe execution of this collectExecution.
        /// </summary>
        /// <returns>a list of object oval_system_characteristics</returns>
        public IEnumerable<oval_system_characteristics> GetSystemCharacteristics()
        {
            
            List<oval_system_characteristics> systemCharacteristics = new List<oval_system_characteristics>();
            foreach (ProbeExecution probeExecution in ProbeExecutions)
            {
                oval_system_characteristics resultSystemCharacteristics = probeExecution.GetSystemCharacteristics();
                if (resultSystemCharacteristics != null)
                {
                    systemCharacteristics.Add(resultSystemCharacteristics);
                }
            }
            return  systemCharacteristics;
        }

        /// <summary>
        /// Verifies if exists some probeExecution with errors.
        /// </summary>
        /// <returns></returns>
        public bool ExistsExecutionsWithError()
        {
            var executionsWithErrors = this.ProbeExecutions.Where(p => p.HasErrors());
            return executionsWithErrors.Count() > 0;
        }


        /// <summary>
        /// Gets the execution logs.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CollectExecutionLog> GetExecutionLogs()
        {
            List<CollectExecutionLog> executionLogs = new List<CollectExecutionLog>();
            foreach (var probeExecution in this.ProbeExecutions)
            {
                executionLogs.AddRange(probeExecution.ExecutionLogs);
            }
            return executionLogs;
        }
    }
}
