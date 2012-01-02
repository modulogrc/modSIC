/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Reflection;
using Microsoft.Practices.Unity;
using Modulo.Collect.Service.Assemblers;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Controllers;
using Modulo.Collect.Service.Data;
using Modulo.Collect.Service.Entities.Factories;
using Quartz;
using Modulo.Collect.Service.Infra;

namespace Modulo.Collect.Service
{
    public class CollectService : ICollectService
    {
        private const string INVALID_AUTHORIZATION_TOKEN = "Invalid authorization token";

        [Dependency]
        public CollectController CollectController { get; set; }

        private static ScheduleController ScheduleController = null;
        private static IUnityContainer SchedulerContainer = null;
        private static object ScheduleInitLock = new object();
        
        public CollectService()
        {
            var diContainer = new UnityContainer().CreateChildContainer();
            lock (ScheduleInitLock)
            {
                var scheduler = new Quartz.Impl.StdSchedulerFactory().GetScheduler();
                if (ScheduleController == null)
                {
                    SchedulerContainer = diContainer.CreateChildContainer();
                    scheduler.JobFactory = new UnityJobFactory(SchedulerContainer);
                    ScheduleController = new ScheduleController(scheduler);
                    ScheduleController.ReschedulingAlreadyExecuted = false;
                }

                diContainer.RegisterType<IDataProvider, DataProvider>(new ContainerControlledLifetimeManager());
                diContainer.RegisterType<ICollectRequestRepository, CollectRequestRepository>(new ContainerControlledLifetimeManager());
                diContainer.RegisterType<IProbeManager, ProbeManager>();
                diContainer.RegisterType<ICollectRequestAssembler, CollectRequestAssembler>(new ContainerControlledLifetimeManager());
                diContainer.RegisterType<ICollectPackageAssembler, CollectPackageAssembler>(new ContainerControlledLifetimeManager());
                diContainer.RegisterType<ICollectResultAssembler, CollectResultAssembler>(new ContainerControlledLifetimeManager());
                diContainer.RegisterType<IDefinitionDocumentFactory, DefinitionDocumentFactory>(new ContainerControlledLifetimeManager());
                diContainer.RegisterInstance<IScheduler>(scheduler);
                diContainer.RegisterInstance<IScheduleController>(ScheduleController);

                CollectController = diContainer.Resolve<CollectController>();
            }
        }

        public SendRequestResult SendRequest(Package collectPackage, string token)
        {
            var result = new SendRequestResult();

            try
            {
                string clientId;

                if (!CollectController.Authenticate(token, out clientId))
                {
                    throw new FaultException(INVALID_AUTHORIZATION_TOKEN);
                }

                var requestIds = CollectController.CollectRequest(collectPackage, clientId);
                result.Requests =
                    requestIds
                        .Select(x => new RequestInfo() { ClientRequestId = x.Key, ServiceRequestId = x.Value }).ToArray();
            }
            catch (Exception ex)
            {
                result.HasErrors = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public Byte[] GetCertificate(string token)
        {
            try
            {
                if (!CollectController.Authenticate(token))
                {
                    throw new FaultException(INVALID_AUTHORIZATION_TOKEN);
                }

                return CollectController.GetCertificate();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public string GetOvalResultDocument(string requestId, string token)
        {
            try
            {
                if (!CollectController.Authenticate(token))
                {
                    throw new FaultException(INVALID_AUTHORIZATION_TOKEN);
                }

                return CollectController.GetOvalResultDocument(requestId);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public Result GetCollectedResultDocument(string requestId, string token)
        {
            try
            {
                if (!CollectController.Authenticate(token))
                {
                    throw new FaultException(INVALID_AUTHORIZATION_TOKEN);
                }

                return CollectController.GetCollectedResultDocument(requestId);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public string GetFullCompressedExecutionLog(string requestId, string token)
        {
            try
            {
                if (!CollectController.Authenticate(token))
                {
                    throw new FaultException(INVALID_AUTHORIZATION_TOKEN);
                }

                return CollectController.GetResultExecutionLog(requestId);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public CollectInfo[] GetCollectRequestsInExecution(string token)
        {
            try
            {
                if (!CollectController.Authenticate(token))
                {
                    throw new FaultException(INVALID_AUTHORIZATION_TOKEN);
                }

                return CollectController.GetCollectRequestsInExecution().ToArray();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public CollectInfo[] GetClientCollectRequestsInExecution(string clientId, string token)
        {
            try
            {
                if (!CollectController.Authenticate(token))
                {
                    throw new FaultException(INVALID_AUTHORIZATION_TOKEN);
                }

                return CollectController.GetCollectRequestsInExecution(clientId).ToArray();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public bool CancelCollect(string requestId, string token)
        {
            try
            {
                if (!CollectController.Authenticate(token))
                {
                    throw new FaultException(INVALID_AUTHORIZATION_TOKEN);
                }

                return CollectController.CancelCollection(requestId);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public string Login(string username, string password)
        {
            LoginInfo loginInfo = LoginEx(username, password, String.Empty, null);

            if (string.IsNullOrEmpty(loginInfo.Token))
            {
                throw new FaultException(loginInfo.ErrorMessage);
            }
            else
            {
                return loginInfo.Token;
            }
        }

        public LoginInfo LoginEx(string username, string password, string clientId, APIVersion required)
        {
            LoginInfo retVal = new LoginInfo();
            retVal.APIVersion = new APIVersion(APIVersion.CURRENT_VERSION);
            retVal.ServerVersion = Assembly.GetExecutingAssembly().GetName().Version;

            if ((required != null) && 
                ((required.Major > retVal.APIVersion.Major) ||
                ((required.Major == retVal.APIVersion.Major) && (required.Minor > retVal.APIVersion.Minor))))
            {
                retVal.ErrorType = "ActionNotSupportedException";
                retVal.ErrorMessage =
                    String.Format(
                        "This client uses a modSIC API version ({0}) not supported by this modSIC Server ({1}).",
                        required.ToString(),
                        retVal.APIVersion);
                //retVal.ErrorMessage = "Client requires API " + required.ToString() + "; Server supports " + retVal.APIVersion;
            }
            else
            {
                var result = CollectController.Login(username, password, clientId);
                if (result == null)
                {
                    retVal.ErrorType = "UnauthorizedAccessException";
                    retVal.ErrorMessage = "Invalid username or password";
                }
                else
                {
                    retVal.Token = result;
                }
            }
            
            return retVal;
        }

        public void Logout(string token)
        {
            try
            {
                CollectController.Logout(token);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public String Heartbeat()
        {
            var modsicServer = Assembly.GetExecutingAssembly().GetName().Version;
            var majorVersion = modsicServer.Major.ToString();
            var minorVersion = modsicServer.Minor.ToString();

            return String.Format("{0}.{1}", majorVersion, minorVersion);
        }

        private String GetModsicCurrentVersion()
        {
            return new ModSicVersionHelper().GetCurrentVersion();
        }
    }
}
