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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Exceptions;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Service.Entities;
using Modulo.Collect.Service.Entities.Factories;
using Modulo.Collect.Service.Probes;
using Modulo.Collect.Service.Contract;
using Raven.Client;
using NLog;

namespace Modulo.Collect.Service.Controllers
{
    public class CollectExecutionManager
    {
        public IProbeManager ProbeManager { get; set; }

        public IList<IConnectionProvider> connectionContext { get; set; }
        public TargetInfo Target { get; set; }
        public bool Interrupted { get; set; }

        private const string EXECUTION_ERROR_MESSAGE =
            "[CollectExecutionManager] - An error occurred collection execution:\r\nMessage:'{0}'\r\nStack:\r\n{1}\r\n\r\n";

        private const string EXECUTION_ERROR_MESSAGE_WITH_INNER_EXCEPTION =
            "[CollectExecutionManager] - An error occurred collection execution:\r\nMessage:'{0}'\r\nInnerException Stack:\r\n{1}\r\nStack:\r\n{2}\r\n\r\n";

        private CollectFactory collectFactory;
        private VariableEvaluatorService variableEvaluatorService;
        private CollectTimeOutController collectTimeOut;
        private static Logger Logger = LogManager.GetCurrentClassLogger();



        public CollectExecutionManager(IProbeManager probeManager)
        {
            ProbeManager = probeManager;
            variableEvaluatorService = new VariableEvaluatorService();
            collectTimeOut = new CollectTimeOutController();
            Interrupted = false;
        }

        /// <summary>
        /// This method is responsible to start of collect process.        
        /// </summary>
        /// <param name="objectTypes">The object types.</param>
        public void ExecuteCollect(IDocumentSession session, CollectRequest collectRequest, FamilyEnumeration plataform)
        {
            ExecutionLogBuilder executionLog = new ExecutionLogBuilder();

            this.SetStatusToExecuting(collectRequest);

            CollectExecution collectExecution = this.CreateCollectExecution(session, collectRequest);
            try
            {
                collectExecution.SetDateStartCollect();
                this.StartCollect(session, collectRequest, collectExecution, plataform, executionLog);
                this.EndCollect(session, collectRequest, collectExecution);
            }
            catch (Exception ex)
            {
                var logMessage = String.Format(EXECUTION_ERROR_MESSAGE, ex.Message, ex.StackTrace);
                Logger.Error(logMessage);
                this.EndsACollectRequestBecauseThisErrorIsUnrecoverable(collectRequest, "Collect Manager", collectExecution, ex, executionLog);
            }

            session.SaveChanges();
        }

        private void StartCollect(IDocumentSession session, CollectRequest collectRequest, CollectExecution collectExecution, FamilyEnumeration plataform, ExecutionLogBuilder executionLog)
        {
            int attemptsOfEvaluateVariables = 1;
            bool anErrorOccured = false;
            bool allObjectWasCollect = false;

            IEnumerable<ObjectType> objectTypes = collectRequest.GetObjectTypesWasNotCollected(session);
            session.SaveChanges();

            while (!collectTimeOut.IsExceededTheMaxAttemptsOfEvaluateVariables(attemptsOfEvaluateVariables) &&
                   !allObjectWasCollect &&
                   !anErrorOccured &&
                   !Interrupted)
            {
                IEnumerable<StateType> states = collectRequest.GetStates(session);
                IEnumerable<SelectedProbe> selectedProbes = ProbeManager.GetProbesFor(objectTypes, plataform);
                VariablesEvaluated variables = variableEvaluatorService.Evaluate(collectRequest, session);

                session.SaveChanges();

                try
                {
                    this.UpdateSystemInformationOfTarget(session, collectRequest, plataform, executionLog);
                    this.ExecuteCollectWithProbes(session, selectedProbes, collectRequest, variables, states, collectExecution, executionLog);

                    session.SaveChanges();
                }
                catch (SystemInformationException ex)
                {
                    string logMessage;
                    if (ex.InnerException == null)
                    {
                        logMessage = String.Format(EXECUTION_ERROR_MESSAGE, ex.Message, ex.StackTrace);
                    }
                    else
                    {
                        logMessage = String.Format(EXECUTION_ERROR_MESSAGE_WITH_INNER_EXCEPTION, ex.Message, ex.InnerException.StackTrace, ex.StackTrace);
                    }
                    Logger.Error(logMessage);
                    collectRequest.SetResultError();
                    collectRequest.Close();
                    anErrorOccured = true;

                    session.SaveChanges();

                    break;
                }
                catch
                {
                    // only ends the loop. The error already was treated
                    anErrorOccured = true;
                }

                objectTypes = collectRequest.GetObjectTypesWasNotCollected(session);
                allObjectWasCollect = objectTypes.Count() == 0;

                attemptsOfEvaluateVariables++;
            }

            if (Interrupted)
            {
                collectRequest.Status = CollectRequestStatus.Canceled;
            }
            else
            {
                if ((!allObjectWasCollect) && (!anErrorOccured))
                {
                    CloseExecutionIfIncomplete(session, collectRequest, collectExecution, plataform, objectTypes);
                }
            }

            session.SaveChanges();
        }

        private void CloseExecutionIfIncomplete(IDocumentSession session, CollectRequest collectRequest, CollectExecution collectExecution, FamilyEnumeration plataform, IEnumerable<ObjectType> objectTypes)
        {
            IEnumerable<SelectedProbe> objectsNotSupported = ProbeManager.GetNotSupportedObjects(objectTypes, plataform);
            var objectNotSupported = this.GetObjectTypesFromSelectedProbes(objectsNotSupported);
            var objectSupportedNotCollect = objectTypes.Except(objectNotSupported);
            this.CreateCollectedObjectsForNotSupportedObjects(session, objectsNotSupported, collectRequest, collectExecution);
            if (objectSupportedNotCollect.Count() > 0)
            {
                IEnumerable<SelectedProbe> objectsSupportedNotCollected = ProbeManager.GetProbesFor(objectSupportedNotCollect, plataform);
                this.CreateCollectedObjectsForNotSupportedObjects(session, objectsSupportedNotCollected, collectRequest, collectExecution);
            }
            collectRequest.SetResultComplete(session);
            collectRequest.Close();
            session.SaveChanges();
        }

        private bool AllObjectThatWasNotCollectAreNotSupported(IEnumerable<ObjectType> objectsNotCollected, IEnumerable<ObjectType> objectsNotSupported)
        {
            if (objectsNotCollected.Count() != objectsNotSupported.Count())
                return false;

            var differences = objectsNotSupported.Except(objectsNotCollected);
            return differences.Count() == 0;
        }

        private void CreateCollectedObjectsForNotSupportedObjects(IDocumentSession session, IEnumerable<SelectedProbe> objectsNotSupported, CollectRequest collectRequest, CollectExecution collectExecution)
        {
            foreach (var probe in objectsNotSupported)
            {
                ProbeResult probeResult = probe.CreateCollectedObjectForNotSupportedObjects(probe.ObjectTypes);
                ProbeExecution probeExecution = this.CreateTheProbeExecution(probeResult, probe);
                collectRequest.UpdateSystemCharacteristics(session);
                collectExecution.ProbeExecutions.Add(probeExecution);

                session.SaveChanges();
            }
        }

        private void EndCollect(IDocumentSession session, CollectRequest collectRequest, CollectExecution collectExecution)
        {
            if (collectTimeOut.IsExceededTheMaxOfExecutionsDefined(collectRequest.GetNumberOfExecutions(session)))
            {
                collectRequest.Close();
            }
            else
            {
                if (!collectRequest.isClosed())
                {
                    if (!collectExecution.ExistsExecutionsWithError())
                    {
                        collectRequest.TryClose(session);
                    }
                    else
                    {
                        collectRequest.Reopen();
                    }
                }
            }
            collectExecution.SetDateEndCollect();
            session.SaveChanges();
        }

        private void UpdateSystemInformationOfTarget(IDocumentSession session, CollectRequest collectRequest, FamilyEnumeration plataform, ExecutionLogBuilder executionLog)
        {
            if (!collectRequest.Target.IsSystemInformationDefined())
            {
                executionLog.CollectSystemInformation();
                ISystemInformationService systemInformationService = ProbeManager.GetSystemInformationService(plataform);
                if (systemInformationService != null)
                {
                    try
                    {
                        this.GetSystemInformationFromTarget(collectRequest, systemInformationService);
                        session.SaveChanges();
                    }
                    catch (RecoverableProbeException ex)
                    {
                        CollectExecution collectExecution = this.CreateCollectExecution(session, collectRequest);
                        collectExecution.SetDateStartCollect();
                        this.ConfigureTheCollectRequestWithAnErrorProbeExecute(collectRequest, "SystemInformation", collectExecution, ex, executionLog);
                        session.SaveChanges();
                        throw new SystemInformationException(ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        CreateCollectionExcutionWithError(session, collectRequest, "SystemInformation", ex, executionLog);
                        session.SaveChanges();
                        throw new SystemInformationException(ex.Message, ex);
                    }

                }
            }
        }

        private void CreateCollectionExcutionWithError(IDocumentSession session, CollectRequest collectRequest, string capability, Exception ex, ExecutionLogBuilder executionLog)
        {
            CollectExecution collectExecution = this.CreateCollectExecution(session, collectRequest);
            collectExecution.SetDateStartCollect();
            EndsACollectRequestBecauseThisErrorIsUnrecoverable(collectRequest, capability, collectExecution, ex, executionLog);
        }

        private void GetSystemInformationFromTarget(CollectRequest collectRequest, ISystemInformationService systemInformationService)
        {
            SystemInformation systemInformation = systemInformationService.GetSystemInformationFrom(Target);
            SystemInfo systemInfo = new SystemInfoFactory().CreateSystemInfo(systemInformation);
            collectRequest.Target.SystemInformation = systemInfo;
        }

        /// <summary>
        /// Execute the collect for the probes selected
        /// </summary>
        /// <param name="selectedProbes">The selected probes.</param>
        /// <param name="collectRequest">The request collect.</param>
        private void ExecuteCollectWithProbes(IDocumentSession session, IEnumerable<SelectedProbe> selectedProbes, CollectRequest collectRequest,
                                              VariablesEvaluated variables, IEnumerable<StateType> states, CollectExecution collectExecution, ExecutionLogBuilder executionLog)
        {
            foreach (SelectedProbe probe in selectedProbes)
            {
                if (Interrupted)
                {
                    break;
                }

                try
                {
                    ProbeExecution probeExecution = this.ExecuteCollect(session, probe, collectRequest, variables, states, executionLog);
                    collectExecution.ProbeExecutions.Add(probeExecution);
                }
                catch (RecoverableProbeException ex)
                {
                    this.ConfigureTheCollectRequestWithAnErrorProbeExecute(collectRequest, probe.Capability.OvalObject, collectExecution, ex, executionLog);
                    session.SaveChanges();
                    throw ex;
                }
                catch (Exception ex)
                {
                    EndsACollectRequestBecauseThisErrorIsUnrecoverable(collectRequest, probe.Capability.OvalObject, collectExecution, ex, executionLog);
                    session.SaveChanges();
                    throw ex;
                }
            }

            session.SaveChanges();
        }

        private void SetEndInstrumentationLog(ExecutionLogBuilder executionLog, DateTime initialTimeStamp, string probeName)
        {
            String INSTRUMENTATION_LOG_MSG_FORMAT = "The collect of [{0}] probe was finished at {1} (in {2} secs)";

            var endTimeStamp = DateTime.Now;
            var totalTimeInSeconds = Convert.ToInt32(endTimeStamp.Subtract(initialTimeStamp).TotalSeconds);

            var executionLogMsg = string.Format(INSTRUMENTATION_LOG_MSG_FORMAT, probeName, endTimeStamp, totalTimeInSeconds);
            executionLog.AddInfo(executionLogMsg);

        }

        private void SetStartInstrumentationLog(ExecutionLogBuilder executionLog, DateTime startTime, string probeName)
        {
            String INSTRUMENTATION_LOG_MSG_FORMAT = "The collect of [{0}] probe was start at {1}";
            var intrumentationLogMsg = string.Format(INSTRUMENTATION_LOG_MSG_FORMAT, probeName, startTime.ToLocalTime());
            executionLog.AddInfo(intrumentationLogMsg);
        }

        private CollectExecution CreateCollectExecution(IDocumentSession session, CollectRequest collectRequest)
        {
            collectFactory = new CollectFactory();
            CollectExecution collectExecution = collectFactory.CreateCollectExecution(session, collectRequest);
            return collectExecution;
        }

        /// <summary>
        /// Executes the collect for the one probe.
        /// </summary>
        /// <param name="collectRequest">The request collect.</param>
        /// <param name="probe">The probe.</param>
        /// <param name="collectExecution">the object that represents the execution of a collect</param>
        /// <returns></returns>
        private ProbeExecution ExecuteCollect(IDocumentSession session, SelectedProbe probe, CollectRequest collectRequest, VariablesEvaluated variables,
                                              IEnumerable<StateType> states, ExecutionLogBuilder executionLog)
        {
            ProbeResult probeResult = null;

            executionLog.StartCollectOf(probe.Capability.OvalObject);

            var initialTimeStamp = DateTime.Now;
            this.SetStartInstrumentationLog(executionLog, initialTimeStamp, probe.Capability.OvalObject);

            probeResult = probe.Execute(connectionContext, Target, variables, collectRequest.GetSystemCharacteristics(session), states);

            this.SetEndInstrumentationLog(executionLog, initialTimeStamp, probe.Capability.OvalObject);

            this.MergeExecutionLogs(executionLog, probe, probeResult);
            return this.CreateTheProbeExecution(probeResult, probe);
        }
        /// <summary>
        /// this method makes the combination between probeLogs and contextLog, provided by ExecutionManager. 
        /// This process normally occours in the end of collect of a probe. 
        /// Because this, in this process is included the End entry in the log.
        /// </summary>
        /// <param name="probe">The probe.</param>
        /// <param name="probeResult">The probe result.</param>
        private void MergeExecutionLogs(ExecutionLogBuilder executionLog, SelectedProbe probe, ProbeResult probeResult)
        {
            executionLog.AddDetailInformation(probeResult.ExecutionLog);
            executionLog.EndCollectOf(probe.Capability.OvalObject);
            probeResult.ExecutionLog = executionLog.BuildExecutionLogs();
        }

        /// <summary>
        /// Creates the probe execution with his collect result
        /// </summary>
        /// <param name="probeResult">The probe result.</param>
        /// <param name="probe">The probe.</param>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        private ProbeExecution CreateTheProbeExecution(ProbeResult probeResult, SelectedProbe probe)
        {
            CollectResultFactory collectResultFactory = new CollectResultFactory();

            ProbeExecution executionOfCurrentProbe = collectFactory.CreateAProbeExecution(probeResult, probe.Capability.OvalObject);
            CollectResult probeExecutionResult = collectResultFactory.CreateCollectResultForTheProbeExecution(probeResult);
            executionOfCurrentProbe.SystemCharacteristics = probeExecutionResult.SystemCharacteristics;

            return executionOfCurrentProbe;
        }

        /// <summary>
        /// Ends the request collect because this error is unrecoverable.
        /// This request was not execute because exists some error that is not possible perform the collect.
        /// Ex.: Invalid credentials, bad format of oval xmls, etc.
        /// </summary>
        /// <param name="collectRequest">The request collect.</param>
        /// <param name="probe">The probe.</param>
        /// <param name="collectExecution">the Collect execution object.</param>
        /// <param name="error">the exception that represents an error.</param>
        private void EndsACollectRequestBecauseThisErrorIsUnrecoverable(CollectRequest collectRequest, string probeCapability, CollectExecution collectExecution, Exception error, ExecutionLogBuilder executionLog)
        {
            ConfigureTheCollectRequestWithAnErrorProbeExecute(collectRequest, probeCapability, collectExecution, error, executionLog);
            collectRequest.Close();
        }

        /// <summary>
        /// Configures the request collect with one probeExecute with the error status.
        /// </summary>
        /// <param name="collectRequest">The request collect.</param>
        /// <param name="probe">The probe.</param>
        /// <param name="collectExecution">The collect execution.</param>
        /// <param name="error">The error.</param>
        private void ConfigureTheCollectRequestWithAnErrorProbeExecute(CollectRequest collectRequest, string probeCapability, CollectExecution collectExecution, Exception error, ExecutionLogBuilder executionLog)
        {
            executionLog.AnErrorOccurred(error.Message);
            executionLog.EndCollect();
            ProbeExecution executionWithError = collectFactory.CreateAProbeExecutionWithError(probeCapability, executionLog.BuildExecutionLogs());
            collectExecution.ProbeExecutions.Add(executionWithError);
        }

        private void SetStatusToExecuting(CollectRequest collectRequest)
        {
            collectRequest.SetExecutingStatus();
        }

        private IEnumerable<ObjectType> GetObjectTypesFromSelectedProbes(IEnumerable<SelectedProbe> selectedProbes)
        {
            List<ObjectType> objectTypes = new List<ObjectType>();
            foreach (var selectedProbe in selectedProbes)
                objectTypes.AddRange(selectedProbe.ObjectTypes);


            return objectTypes;
        }
    }
}
