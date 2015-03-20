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
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Service.Tests.Helpers
{
    public class ProbeResultFactory
    {

        public const string ID_REGISTRY_OBJECT = "oval:org.mitre.oval:obj:717";

        public ProbeResult CreateProbeResultForRegistryCollect()
        {
            ProbeResult probeResult = RegistryProbeFactory.CreateProbeResultForRegistryCollect();
            probeResult.SystemInformation = this.CreateSystemInformation();
            return probeResult;
        }       

        public ProbeResult CreateProbeResultForRegostryCollectWithError()
        {
            ProbeResult probeResult = RegistryProbeFactory.CreateProbeResultForRegostryCollectWithError();
            probeResult.SystemInformation = this.CreateSystemInformation();
            return probeResult;
        }

        public ProbeResult CreateProbeResultForRegistryCollectWithVariables()
        {
            ProbeResult probeResult = RegistryProbeFactory.CreateProbeResultForRegistryCollectWithVariables();
            probeResult.SystemInformation = this.CreateSystemInformation();
            return probeResult;
        }

        public ProbeResult CreateProbeREsultForFamilyCollect()
        {
            ProbeResult probeResult = FamilyProbeResultFactory.CreateProbeResultForFamilyollect();
            probeResult.SystemInformation = this.CreateSystemInformation();
            return probeResult;
        }

        public ProbeResult CreateProbeResultForRegistryCollectWithSytemDataDuplicated()
        {
            ProbeResult probeResult = RegistryProbeFactory.CreateProbeResultForRegistryCollectWithSytemDataDuplicated();
            probeResult.SystemInformation = this.CreateSystemInformation();
            return probeResult;
        }

        public ProbeResult CreateProbeResultForRegistryWithSpecificObjectTypes(IEnumerable<definitions.ObjectType> objectTypes, List<string> resultsForObjects)
        {
            ProbeResult probeResult = RegistryProbeFactory.CreateProbeResultWithSpecificObject(objectTypes, resultsForObjects);
            probeResult.SystemInformation = this.CreateSystemInformation();
            return probeResult;
            
        }

        public ProbeResult CreateProbeResultForFamilyWithSpecificObjectTypes(IEnumerable<definitions.ObjectType> objectTypes, List<string> resultsForObjects)
        {
            ProbeResult probeResult = FamilyProbeResultFactory.CreateProbeResultWithSpecificObject(objectTypes,resultsForObjects);
            probeResult.SystemInformation = this.CreateSystemInformation();
            return probeResult;
        }
         

       
        private SystemInformation CreateSystemInformation()
        {
            SystemInformation systemInformation = new SystemInformation();
            systemInformation.SystemName = "unknown Service Pack 1";
            systemInformation.SystemVersion = "6.0.6001";
            systemInformation.PrimaryHostName = "mss-rj-007.mss.modulo.com.br";
            systemInformation.Architecture = "INTEL32";

            NetworkInterface networkInfterface = new NetworkInterface()
            {
                Name = "Intel(R) 82566DM Gigabit Network Connection",
                IpAddress = "172.16.3.33",
                MacAddress = "00-1E-C9-1D-72-4E"
            };

            systemInformation.Interfaces.Add(networkInfterface);

            return systemInformation;           
        }
    }
}
