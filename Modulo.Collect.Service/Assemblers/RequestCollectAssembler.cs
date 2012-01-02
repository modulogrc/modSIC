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

using Microsoft.Practices.Unity;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Service.Entities;


namespace Modulo.Collect.Service.Assemblers
{
    public class CollectRequestAssembler : Modulo.Collect.Service.Assemblers.ICollectRequestAssembler
    {
        IDataProvider dataProvider;
        private TargetAssembler targetAssembler;
 
        [InjectionConstructor]
        public CollectRequestAssembler(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            targetAssembler = new TargetAssembler(this.dataProvider);            
        }


        public CollectRequest CreateCollectRequestFromDTO(Request collectRequestDTO, string definitionId)
        {
            var collectRequest = new CollectRequest();
            targetAssembler.AddTargetInCollectRequest(collectRequestDTO, collectRequest);
            collectRequest.OvalDefinitionsId = definitionId;
            collectRequest.ExternalVariables = collectRequestDTO.ExternalVariables;

            return collectRequest;
        }

        public CollectInfo CreateCollectInfoFromCollectRequest(CollectRequest collectRequest)
        {
            CollectInfo collectInfoDTO = new CollectInfo();
            collectInfoDTO.Address = collectRequest.Target.Address;
            collectInfoDTO.ReceivedOn = collectRequest.ReceivedOn;
            collectInfoDTO.CollectRequestId = collectRequest.Oid.ToString();
            collectInfoDTO.ClientId = collectRequest.ClientId;
            collectInfoDTO.Status = collectRequest.Status;

            return collectInfoDTO;
        }
    }
}