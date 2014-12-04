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
using System.IO;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Entities.Factories;
using Modulo.Collect.Service.Exceptions;
using Raven.Client;
using Definitions = Modulo.Collect.OVAL.Definitions;
using systemCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Service.Entities
{
    public class CollectRequest : BaseGuidEntity
    {
        public CollectRequest()
        {
            this.ReceivedOn = DateTime.Now;
        }

        #region Entity Properties
        public Target Target {get;set;}
        
        public string OvalDefinitionsId { get; set; }

        public string ExternalVariables { get; set; }

        public DateTime ReceivedOn { get; set; }

        public CollectResult Result { get; set; }
       
        public CollectRequestStatus Status { get; set; }

        public string CollectPackageId { get; set; }

        public string ClientId { get; set; }
        
        #endregion

        #region Fields
        private Definitions.ObjectType[] ObjectTypes;
        
        private VariableType[] Variables;
        
        private StateType[] States;
        #endregion

        public bool isOpen()
        {
            return Status == CollectRequestStatus.Open;
        }

        public bool isClosed()
        {
            return Status == CollectRequestStatus.Close || Status == CollectRequestStatus.Canceled;
        }

        public bool isExecuting()
        {
            return Status == CollectRequestStatus.Executing;
        }

        /// <summary>
        /// This method set the status of CollectRequest for open again.
        /// This happens when in the execution, happens an RecoverableError and  
        /// the collectRequestion should be execution again.
        /// </summary>
        public void Reopen()
        {
            Status = CollectRequestStatus.Open;
        }

        /// <summary>
        /// This method ends a CollectRequest.
        /// </summary>
        public void Close()
        {
            Status = CollectRequestStatus.Close;                        
        }

        public void SetResultComplete(IDocumentSession session)
        {
            this.BuildSystemCharacteristics(session);            
            this.Result.SetComplete();
        }

        public void SetResultError()
        {
            var collectResultFactory = new CollectResultFactory();
            var collectResult = collectResultFactory.CreateCollectResult(CollectStatus.Error, null);
            this.Result = collectResult;
        }  

        /// <summary>
        /// This method return the list of ObjectTypes by the OvalDefinitions property.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Definitions.ObjectType> GetObjectTypes(IDocumentSession session)
        {
            try
            {
                if (ObjectTypes == null)
                {
                    oval_definitions oval = this.GetOvalDefinitions(session);
                    ObjectTypes = oval.objects.ToArray();
                    return ObjectTypes;
                }
                else
                {
                    return ObjectTypes;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = String.Format("The Oval Definitions XML is Invalid: {0}", ex.Message);
                throw new InvalidXMLOvalDefinitionsException(errorMessage, ex);
            }           
        }

        private oval_definitions GetOvalDefinitions(IDocumentSession session)
        {
            IEnumerable<string> errors;
            var definitionsInMemoryStream = GetMemoryStreamFromDefinitionsXML(session);
            var oval = oval_definitions.GetOvalDefinitionsFromStream(definitionsInMemoryStream, out errors);
            
            if (errors.Count() > 0)
                throw new InvalidXMLOvalDefinitionsException(errors.First());
            
            return oval;
        }

        /// <summary>
        /// Gets the oval variables from the OvalDefinitions property.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VariableType> GetOvalVariables(IDocumentSession session)
        {
            try
            {
                if (Variables == null)
                {
                    oval_definitions oval = this.GetOvalDefinitions(session);
                    if (oval.variables != null)
                    {
                        Variables = oval.variables.ToArray();
                    }

                    if (Variables == null)
                        Variables = new List<VariableType>().ToArray();
                    
                    return Variables;
                }
                
                return Variables;
            }
            catch (Exception ex)
            {
                var errorMessage = String.Format("The Oval Definitions XML is Invalid: {0}", ex.Message);
                throw new InvalidXMLOvalDefinitionsException(errorMessage, ex);
            }         
        }

        /// <summary>
        /// Return the number of executions of this request Collect.         
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfExecutions(IDocumentSession session)
        {
            return GetCollectExecutions(session).Count();            
        }

        /// <summary>
        /// Gets the list of systemCharacteristics of all collects.
        /// It not is the result of requetCollect. For the result use the Result property.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<oval_system_characteristics> GetExecutedSystemCharacteristics(IDocumentSession session)
        {
            List<Modulo.Collect.OVAL.SystemCharacteristics.oval_system_characteristics> systemCharacteristics = new List<Modulo.Collect.OVAL.SystemCharacteristics.oval_system_characteristics>();
            var collectExecutions = GetCollectExecutions(session);
            foreach (CollectExecution collect in collectExecutions)
            {
                systemCharacteristics.AddRange(collect.GetSystemCharacteristics());
            }
            return systemCharacteristics;
        }

        public List<CollectExecution> GetCollectExecutions(IDocumentSession session)
        {
            var collectExecutions = session.Query<CollectExecution>().Customize(x=>x.WaitForNonStaleResults()).Where(x => x.RequestId == this.Oid).ToList();
            return collectExecutions;
        }

        /// <summary>
        /// Returns the systemCharacteristics of a CollectRequest. If the CollectRequest was not executed, the return is null.
        /// If the CollectRequest was already executed, but still be open, the return is a partial SystemCharacteristics, 
        /// just result of executed probes.
        /// </summary>
        /// <returns></returns>
        public Modulo.Collect.OVAL.SystemCharacteristics.oval_system_characteristics GetSystemCharacteristics(IDocumentSession session)
        {
            this.BuildSystemCharacteristics(session);
            //////session.SaveChanges();
            return this.Result != null ? this.Result.GetSystemCharacteristic() : null;           
        }

        /// <summary>
        /// Returns all object that has some entity that references the variableId informed
        /// </summary>
        /// <param name="variableId">The variable id.</param>
        /// <returns></returns>
        public IEnumerable<Definitions.ObjectType> GetObjectTypesByVariableId(IDocumentSession session, string variableId)
        {
            var objectThatHasReferences = new List<Definitions.ObjectType>();
            var objectTypes = this.GetObjectTypes(session);
            foreach (var objectType in objectTypes)
                if (objectType.HasReferenceForVariable(variableId))
                    objectThatHasReferences.Add(objectType);
            
            return objectThatHasReferences;
        }

        /// <summary>
        /// Gets the object types was not collected.
        /// This objects exits in the ovalDefinitions and was not possible of collect.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Definitions.ObjectType> GetObjectTypesWasNotCollected(IDocumentSession session)
        {
            var objectTypeWasNotCollect = new List<Definitions.ObjectType>();
            var definitionsObjects = this.GetObjectTypes(session);
            var systemCharacteristics = this.GetCompleteSystemCharacteristics(session);

            if (systemCharacteristics == null)
                return GetObjectTypes(session);

            foreach (var objectType in definitionsObjects)
            {
                var collectedObject = systemCharacteristics
                    .collected_objects
                    .Where<systemCharacteristics.ObjectType>(obj => obj.id == objectType.id)
                    .SingleOrDefault();

                if (collectedObject == null)
                    objectTypeWasNotCollect.Add(objectType);
            }

            return objectTypeWasNotCollect;
        }

        public void TryClose(IDocumentSession session)
        {
            var objectTypesWasNotCollected = this.GetObjectTypesWasNotCollected(session);
            if (objectTypesWasNotCollected.Count() == 0)
            {
                this.SetResultComplete(session);
                this.Close();
            }
            else
                this.Reopen();
        }

        public bool HasResult()
        {
            return this.Result != null;
        }

        /// <summary>
        /// Gets the external variables.
        /// </summary>
        /// <returns></returns>
        public OVAL.Variables.oval_variables GetExternalVariables()
        {
            if (string.IsNullOrEmpty(ExternalVariables))
                return null;

            IEnumerable<string> errors;
            return OVAL.Variables.oval_variables.GetOvalVariablesFromText(ExternalVariables, out errors);
        }

        /// <summary>
        /// Gets the states of the oval definitions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StateType> GetStates(IDocumentSession session)
        {
            try
            {
                if (States == null)
                {
                    oval_definitions oval = this.GetOvalDefinitions(session);
                    if ((States == null) && (oval.states != null))
                        States = oval.states;
                    
                    return States;
                }
                else
                {
                    return States;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = String.Format("The Oval Definitions XML is Invalid: {0}", ex.Message);
                throw new InvalidXMLOvalDefinitionsException(errorMessage, ex);
            }
        }

        public IEnumerable<StateType> GetStateTypeByVariableId(IDocumentSession session, string variableId)
        {
            var StateThatHasReferences = new List<StateType>();
            var stateTypes = this.GetStates(session);
            
            if (stateTypes == null)
                return StateThatHasReferences;

            foreach (var state  in stateTypes)
                if (state.HasReferenceForVariable(variableId))
                    StateThatHasReferences.Add(state);

            return StateThatHasReferences;
        }

        /// <summary>
        /// Gets the execution log.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CollectExecutionLog> GetExecutionLog(IDocumentSession session)
        {
            var collectExecutions = GetCollectExecutions(session);
            var logOfExecution = new List<CollectExecutionLog>();
            foreach (var collectExection in collectExecutions)
                logOfExecution.AddRange(collectExection.GetExecutionLogs());
            
            return logOfExecution;
        }

        public void SetExecutingStatus()
        {
            this.Status = CollectRequestStatus.Executing;
        }

        public void UpdateSystemCharacteristics(IDocumentSession session)
        {
            this.BuildSystemCharacteristics(session);
        }

        //protected override void OnSaving()
        //{
        //    //base.OnSaving();
        //    this.ConfigureResultOfCollect();
        //}

        private void ConfigureResultOfCollect(IDocumentSession session)
        {
            if ((this.isOpen()) || (this.Result == null))
                this.BuildSystemCharacteristics(session);
        }

        private MemoryStream GetMemoryStreamFromDefinitionsXML(IDocumentSession session)
        {
            var definitionDocument = session.Load<DefinitionDocument>(this.OvalDefinitionsId);
            return new MemoryStream(Encoding.UTF8.GetBytes(definitionDocument.Text));
        }

        /// <summary>
        /// Build the SystemCharacteristics for this collectRequest.
        /// It is necessary because the SystemCharacteristics, in this case,
        /// must be a merge of results of the all probeExecutions.
        /// The systemCharacteristics, must be have all the object collected by all probes.
        /// </summary>
        private void BuildSystemCharacteristics(IDocumentSession session)
        {
            var systemCharacteristics = this.GetCompleteSystemCharacteristics(session);
            var collectResultFactory = new CollectResultFactory();

            if (systemCharacteristics != null)
            {               
                var systemCharacteristicsInXML = systemCharacteristics.GetSystemCharacteristicsXML();
                var result = collectResultFactory.CreateCollectResult(CollectStatus.Partial, systemCharacteristicsInXML);

                //result.CollectRequestId = Oid;
                //if (Results.Count() > 0)
                //    Results.Remove(Result);

                Result = result;
                session.SaveChanges();
            }           
        }

        private systemCharacteristics.oval_system_characteristics GetCompleteSystemCharacteristics(IDocumentSession session)
        {
            return 
                new SystemCharacteristicsFactory()
                    .CreateSystemCharacteristicsBy(
                        this.GetExecutedSystemCharacteristics(session),        
                        this.Target.SystemInformation);
        }
    }
}
