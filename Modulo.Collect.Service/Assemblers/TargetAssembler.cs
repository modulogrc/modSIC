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

using System.Collections.Generic;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Service.Entities;

namespace Modulo.Collect.Service.Assemblers
{
    public class TargetAssembler
    {
        private IDataProvider dataProvider;

        public TargetAssembler(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public void AddTargetInCollectRequest(Request collectRequestDTO, CollectRequest collectRequest)
        {
            Target target = new Target();
            target.Address = collectRequestDTO.Address;
            collectRequest.Target = target;

            this.AddCredentialToTarget(collectRequestDTO, target);
            this.AddTargetParametersInCollectRequest(collectRequestDTO, target);
        }

        private void AddCredentialToTarget(Request collectRequestDTO, Target target)
        {            
            var TargetCredential = new TargetCredential() { CredentialInfo = System.Text.Encoding.Default.GetBytes(collectRequestDTO.Credential) };
            target.Credential = TargetCredential;
        }

        private void AddTargetParametersInCollectRequest(Request collectRequestDTO, Modulo.Collect.Service.Entities.Target target)
        {
            if (collectRequestDTO.TargetParameters != null)
            {
                foreach (KeyValuePair<string, string> parameter in collectRequestDTO.TargetParameters)
                {
                    TargetParameter targetParameter = CreateTargetParametersFromCollectRequestDTO(parameter.Key, parameter.Value);
                    target.Parameters.Add(targetParameter);
                }
            }
        }

        private Modulo.Collect.Service.Entities.TargetParameter CreateTargetParametersFromCollectRequestDTO(string name, string value)
        {
            TargetParameter parameter = new TargetParameter();
            parameter.Name = name;
            parameter.ParameterValue = value;
            return parameter;
        }
    }
}
