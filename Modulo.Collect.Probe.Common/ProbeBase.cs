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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.setEvaluator;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Common
{
    public abstract class ProbeBase
    {
        public IConnectionManager ConnectionManager { get; set; }

        public BaseObjectCollector ObjectCollector { get; set; }

        public IItemTypeGenerator ItemTypeGenerator { get; set; }

        protected IConnectionProvider ConnectionProvider = null;
        
        protected ExecutionLogBuilder ExecutionLogBuilder = null;


        /// <summary>
        /// The base class for all probes.
        /// The Execute method describes the common collection logic.
        /// The specific probes should extend this class and override the abstracts methods for add specific details for the collection.
        /// The basic logic for all probes is:
        ///   - Connect to host.
        ///   - Verifies if the objectType has a setElement and then process the set.
        ///   or
        ///   - Get items to collect - This method is responsible for evaluates all the variables and operations from the object types.
        ///   - Collect items through the SystemDataSouce implementation. A SystemDataSource is the object responsible by collect
        ///     information in the specific technology. For instance, for the registry collect, use a RegistrySystemDataSource. 
        ///   - Build the Probe Result.
        /// </summary>
        protected ProbeBase()
        {
            if (this.ConnectionManager == null)
                this.ConnectionManager = new ConnectionManager();

            this.ExecutionLogBuilder = new ExecutionLogBuilder();
        }

        protected virtual void ExecuteAfterOpenConnectionProvider()
        {
            // Should be overriden in derived classes if necessary...
        }
        
        public ProbeResult Execute(
            IList<IConnectionProvider> connectionContext, 
            TargetInfo target, 
            CollectInfo collectInfo)
        {
            this.ExecutionLogBuilder = new ExecutionLogBuilder();
            this.ExecutionLogBuilder.TryConnectToHost(target.GetAddress());
            this.OpenConnectionProvider(connectionContext, target);

            ExecuteAfterOpenConnectionProvider();

            this.ExecutionLogBuilder.ConnectedToHostWithUserName(target.credentials.GetFullyQualifiedUsername());
            this.ConfigureObjectCollector();
            
            ProbeResultBuilder probeResultBuilder = this.CollectInformation(collectInfo);
            probeResultBuilder.AddExecutionLogs(this.ExecutionLogBuilder.BuildExecutionLogs());
            return probeResultBuilder.ProbeResult;
        }

        public ProbeResult CreateCollectedObjectForNotSupportedObjects(
            IEnumerable<Definitions.ObjectType> objectNotSupported)
        {
            ProbeResultBuilder probeResultBuilder = new ProbeResultBuilder();
            foreach (var objectType in objectNotSupported)
            {
                var collectedObject = new CollectedObject(objectType.id);
                collectedObject.SetEspecificObjectStatus(FlagEnumeration.notcollected);
                probeResultBuilder.AddCollectedObject(collectedObject);
            }
            return probeResultBuilder.ProbeResult;
        }
        

        protected abstract set GetSetElement(Definitions.ObjectType objectType);
        
        protected abstract void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target);
        
        protected abstract void ConfigureObjectCollector();
        
        protected abstract IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes);
        
        protected abstract ItemType CreateItemTypeWithErrorStatus(string errorMessage);

        protected virtual ProbeResultBuilder CollectInformation(CollectInfo collectInfo)
        {
            CollectedObject collectedObject = null;
            var probeResultBuilder = new ProbeResultBuilder();
            
            int idOfItems = 1;
          //  var objectsOfAnEspecificType = this.GetObjectsOfType(collectInfo.ObjectTypes);

            ObjectCollector.PrepareCollectionOfObjects(collectInfo.ObjectTypes, collectInfo.Variables);
            foreach (var ovalObject in collectInfo.ObjectTypes)
            {
                ExecutionLogBuilder.CollectingInformationFrom(ovalObject.id);
                
                if (this.ObjectTypeHasSet(ovalObject))
                    collectedObject = this.ProcessSet(ovalObject, collectInfo);
                else
                    collectedObject = this.ProcessCollect(ovalObject, collectInfo, probeResultBuilder, ref idOfItems);
                
                probeResultBuilder.AddCollectedObject(collectedObject);
            }

            return probeResultBuilder;
        }

        
        /// <summary>
        /// Executes a normal collect, using the system datasource for data collecting.
        /// </summary>
        /// <param name="ovalComponent">The oval component.</param>
        /// <param name="collectInfo">The collect info.</param>
        /// <param name="id">The id parameter is 'a sequencial number controlled by external scope.</param>
        /// <returns></returns>
        private CollectedObject ProcessCollect(Definitions.ObjectType ovalComponent, CollectInfo collectInfo, ProbeResultBuilder probeResultBuilder, ref int id)
        {
            CollectedObject collectedObject = null;
            var allItemsToCollect = this.TryToGetItemsToCollect(ovalComponent, collectInfo.Variables);
            
            if (allItemsToCollect.Count() > 0)
            {
                collectedObject = new CollectedObject(ovalComponent.id);
                foreach (var itemToCollect in allItemsToCollect)
                {
                    var collectedItems = ObjectCollector.CollectDataForSystemItem(itemToCollect);
                    foreach (var collectedItem in collectedItems)
                    {
                        var itemType = probeResultBuilder.GetItemType(collectedItem.ItemType);
                        if (itemType == null)
                        {
                            collectedItem.ItemType.id = id.ToString();
                            id++;
                        }
                        else
                        {
                            collectedItem.ItemType = itemType;
                        }

                        collectedObject.AddItemToSystemData(collectedItem.ItemType);
                        var variables = collectInfo.GetVariableValueForOvalComponent(collectedObject.ObjectType.id);
                        collectedObject.AddVariableReference(variables);
                        ExecutionLogBuilder.AddDetailInformation(collectedItem.ExecutionLog);
                    }
                }

                collectedObject.UpdateCollectedObjectStatus();
            }

            return collectedObject;
        }

        /// <summary>
        /// This method executes the process of  a setElement in the ObjectType.
        /// For the objectType that has a setElement the process is a some different. 
        /// Actually, the setElement uses the objectType already was  collected. 
        /// In this process the setElement process uses a systemCharacteristics for the get a reference for the objectType 
        /// collected and makes the references are used in the new CollectedObject of the process.
        /// </summary>
        /// <param name="ovalObject">The Oval Object.</param>
        /// <param name="collectInfo">The collect info.</param>
        /// <returns></returns>
        private  CollectedObject ProcessSet(Definitions.ObjectType ovalObject, CollectInfo collectInfo)        
        {
            CollectedObject collectedObject = null;
            try
            {
                var setElement = this.GetSetElement(ovalObject);
                var setEvaluator = new SetEvaluator(collectInfo.SystemCharacteristics, collectInfo.States, collectInfo.Variables);
                var resultOfSet = setEvaluator.Evaluate(setElement);
                var objectReferences = resultOfSet.Result;

                if (objectReferences.Count() > 0)
                {
                    collectedObject = new CollectedObject(ovalObject.id);
                    foreach (string reference in objectReferences)
                    {
                        var itemType = collectInfo.SystemCharacteristics.GetSystemDataByReferenceId(reference);
                        collectedObject.AddItemToSystemData(itemType);
                    }

                    collectedObject.SetEspecificObjectStatus(resultOfSet.ObjectFlag);
                }

                return collectedObject;
            }
            catch (Exception ex)
            {
                collectedObject = new CollectedObject(ovalObject.id);
                collectedObject.SetEspecificObjectStatus(FlagEnumeration.error);
                collectedObject.ObjectType.message = MessageType.FromErrorString(String.Format("An error occurred while set processing: '{0}'", ex.Message));
                return collectedObject;
            }
        }

        private bool ObjectTypeHasSet(Definitions.ObjectType ovalComponent)
        {
            var setElement = this.GetSetElement(ovalComponent);
            return (setElement != null);
        }

        private IEnumerable<ItemType> TryToGetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            try
            {
                var itemsToCollect = this.ItemTypeGenerator.GetItemsToCollect(objectType, variables);
                
                if (itemsToCollect.Count() > 0)
                    return itemsToCollect;

                return new ItemType[] { };
            }
            catch (Exception ex)
            {
                return new ItemType[] { this.CreateItemTypeWithErrorStatus(ex.Message) };
            }
        }

        protected MessageType[] PrepareErrorMessage(string errorMessage)
        {
            return MessageType.FromString(errorMessage, MessageLevelEnumeration.error);
        }
    }
}
