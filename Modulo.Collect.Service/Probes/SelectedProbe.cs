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

using System.Collections.Generic;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using SC = Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Service.Probes
{
    public class SelectedProbe
    {
        private IProbe probe;
        public IEnumerable<ObjectType> ObjectTypes {get; set;}
        public ProbeCapabilities Capability { get; set; }

        public SelectedProbe(IProbe probe, IEnumerable<ObjectType> objectTypes, ProbeCapabilities capability)
        {
            this.probe = probe;
            this.ObjectTypes = objectTypes;
            this.Capability = capability;
        }

        public ProbeResult Execute(IList<IConnectionProvider> connectionContext, 
                                   TargetInfo target, VariablesEvaluated variables,  
                                   SC::oval_system_characteristics  systemCharacteristics,
                                   IEnumerable<StateType> states)
        {
            CollectInfo collectInfo = new CollectInfo() 
            { 
                ObjectTypes = this.ObjectTypes, 
                Variables = variables, 
                SystemCharacteristics =  systemCharacteristics,
                States = states
            };

            return probe.Execute(connectionContext, target, collectInfo);
        }

        public ProbeResult CreateCollectedObjectForNotSupportedObjects(IEnumerable<ObjectType> objectNotSupported)
        {
            return probe.CreateCollectedObjectForNotSupportedObjects(objectNotSupported);
        }
    }
}
