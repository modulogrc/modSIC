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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Entities.Factories;

namespace Modulo.Collect.Service.Entities
{
    public class CollectResultFactory
    {
        private SystemCharacteristicsFactory systemCharacteristicsFactory;

        public CollectResultFactory()
        {
            this.systemCharacteristicsFactory = new SystemCharacteristicsFactory();
        }

        /// <summary>
        /// Creates the CollectResult specific for the ProbeExecution.
        /// </summary>
        /// <param name="probeResult">The probe result.</param>
        /// <returns></returns>
        public CollectResult CreateCollectResultForTheProbeExecution(ProbeResult probeResult)
        {   
            var systemCharacteristics = systemCharacteristicsFactory.CreateSystemCharacteristicsInXMLFormat(probeResult);
            return this.CreateCollectResult(GetCollectResultStatus(probeResult), systemCharacteristics);
        }

        /// <summary>
        /// Creates the collect result from the SystemCharacteristics List.
        /// The collectResult systemCharacteristics properties will be the merge of all the systemCharacteristics in the list.
        /// </summary>
        /// <param name="systemCharacteristics">The system characteristics.</param>
        /// <param name="status">The status of the collect result</param>
        /// <returns></returns>
        public CollectResult CreateCollectResultFromOvalSystemCharacteristics(IEnumerable<oval_system_characteristics> systemCharacteristics, CollectStatus status)
        {
            var systemCharacteristicsComplete = systemCharacteristicsFactory.CreateSystemCharacteristicsBy(systemCharacteristics);
            var systemCharacteristicsInXML = systemCharacteristicsComplete.GetSystemCharacteristicsXML();
            var collectResult = this.CreateCollectResult(status, systemCharacteristicsInXML);
            return null;   
        }

        public CollectResult CreateCollectResult(CollectStatus status, String systemCharacteristics)
        {
            return new CollectResult()
            {
                Date = DateTime.Now,
                Status = status,
                SystemCharacteristics = systemCharacteristics
            };
        }

        /// <summary>
        /// Gets the collect result status.
        /// </summary>
        /// <param name="probeResult">The probe result.</param>
        /// <returns></returns>
        private CollectStatus GetCollectResultStatus(ProbeResult probeResult)
        {
            return probeResult.HasErrors() ? CollectStatus.Error : CollectStatus.Complete;
        }        
    }
}
