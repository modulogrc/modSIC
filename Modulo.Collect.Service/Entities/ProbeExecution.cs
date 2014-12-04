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
using System.Text;
using Modulo.Collect.OVAL.SystemCharacteristics;
using System.IO;
using Modulo.Collect.Service.Exceptions;
using Modulo.Collect.Service.Entities.Factories;

namespace Modulo.Collect.Service.Entities
{
    public class ProbeExecution
    {       
        public ProbeExecution()
        {
            ExecutionLogs = new List<CollectExecutionLog>();
        }
        public string Capability {get;set;}
        public CollectExecution Collect {get;set;}
        public IList<CollectExecutionLog> ExecutionLogs { get; set; }
        public string SystemCharacteristics {get;set;}
        
        public bool IsComplete()
        {
            return ! this.HasErrors();
        }

        /// <summary>
        /// Determines whether in the execution some error occured.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </returns>
        public bool HasErrors()
        {
            var logsWithError =
                  from log in this.ExecutionLogs
                  where log.Type == LogType.Error
                  select log;

            return logsWithError.Count() > 0;
        }

        /// <summary>
        /// Gets the oval system characteristic in object format.
        /// </summary>
        /// <returns></returns>
        public oval_system_characteristics GetSystemCharacteristics()
        {
            if (this.SystemCharacteristics != null)
            {
                SystemCharacteristicsFactory factory = new SystemCharacteristicsFactory();
                return factory.CreateSystemCharacteristicsByXML(this.SystemCharacteristics);
            }
            return null;
        }


    }
}
