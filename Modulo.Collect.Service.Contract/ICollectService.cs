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

using System;
using System.ServiceModel;

namespace Modulo.Collect.Service.Contract
{
    [ServiceContract]
    public interface ICollectService
    {
        [OperationContract]
        [FaultContract(typeof(FaultException))]
        SendRequestResult SendRequest(Package collectPackage, string token);

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        Byte[] GetCertificate(string token);

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        Result GetCollectedResultDocument(string requestId, string token);

        [OperationContract]
        [FaultContract(typeof(UnauthorizedServiceAccessException))]
        string GetFullCompressedExecutionLog(string requestId, string token);

        [OperationContract]
        [FaultContract(typeof(UnauthorizedServiceAccessException))]
        bool CancelCollect(string requestId, string token);

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        CollectInfo[] GetCollectRequestsInExecution(string token);

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        CollectInfo[] GetClientCollectRequestsInExecution(string clientId, string token);
        
        [OperationContract]
        [FaultContract(typeof(FaultException))]
        string GetOvalResultDocument(string requestId, string token);

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        string Login(string username, string password);

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        LoginInfo LoginEx(string username, string password, string clientId, APIVersion required);

        [OperationContract]
        void Logout(string token);

        [OperationContract]
        TargetCheckingResult CheckTargetAvailability(TargetPlatforms targetPlatform, string address, string encryptedCredentials, string sshPort, string token);

        [OperationContract]
        string Heartbeat();

    }
}
