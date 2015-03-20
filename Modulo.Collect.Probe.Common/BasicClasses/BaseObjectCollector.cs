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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using ObjectType = Modulo.Collect.OVAL.Definitions.ObjectType;

namespace Modulo.Collect.Probe.Common
{
    public abstract class BaseObjectCollector
    {
        public BaseObjectCollector() { this.ExecutionLogBuilder = new ExecutionLogBuilder(); }

        protected ExecutionLogBuilder ExecutionLogBuilder { set; get; }

        private string ObjectCollectorName { get { return this.GetType().Name; } }

        private const string UNEXPECTED_LOG_ERROR_MESSAGE = "[{0}] - An unexpected error occurred while collecting item: '{1}'";
        private const string ITEM_COULD_NOT_BE_COLLECTED = "[{0}] - This item could not be collected: '{1}'";
        private const string ITEM_NOT_FOUND_LOG_MESSAGE = "[{0}] - There is no object identified as '{1}' in target system.";


        #region Public Methods

        public abstract IList<String> GetValues(Dictionary<string, object> parameters);

        public virtual IEnumerable<CollectedItem> CollectDataForSystemItem(ItemType systemItem)
        {
            try
            {
                if (systemItem.status.Equals(StatusEnumeration.exists) || systemItem.status.Equals(StatusEnumeration.notcollected))
                    return this.collectDataForSystemItem(systemItem);
                else
                    return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
            }
            catch (Exception ex)
            {
                AddAWarningMessageInExecutionLog(ex.Message);
                ConfigureItemTypeAsErrorItem(systemItem, ex.Message);
                return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
            }
        }

        #endregion


        #region Protected Methods

        protected abstract IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem);

        protected void SetDoesNotExistStatusForItemType(ItemType systemItem, string itemIdentifier)
        {
            systemItem.status = StatusEnumeration.doesnotexist;

            if (!string.IsNullOrEmpty(itemIdentifier))
            {
                var messageToLog = string.Format(ITEM_NOT_FOUND_LOG_MESSAGE, ObjectCollectorName, itemIdentifier);
                this.ExecutionLogBuilder.AddInfo(messageToLog);
            }
        }

        protected IEnumerable<ProbeLogItem> BuildExecutionLog()
        {
            return this.ExecutionLogBuilder.BuildExecutionLogs();
        }

        #endregion


        private void SetErrorStatusForItemType(ItemType systemItem, string exceptionMessage)
        {
            systemItem.status = StatusEnumeration.error;
            systemItem.message = MessageType.FromErrorString(exceptionMessage);
        }

        private List<MessageType> CreateMessageType(string exceptionMessage)
        {
            var messages = new List<MessageType>();
            if (!string.IsNullOrEmpty(exceptionMessage))
                messages.Add(new MessageType() { Value = exceptionMessage });
            return messages;
        }

        private void AddAWarningMessageInExecutionLog(string exceptionMessage)
        {
            var warningMessage = string.Format(UNEXPECTED_LOG_ERROR_MESSAGE, ObjectCollectorName, exceptionMessage);
            this.ExecutionLogBuilder.Warning(warningMessage);
        }

        private void CreateAnErrorOccurredExecutionLog(string exceptionMessage)
        {
            this.ExecutionLogBuilder.AnErrorOccurred(string.Format(UNEXPECTED_LOG_ERROR_MESSAGE, ObjectCollectorName, exceptionMessage));
        }

        private void ConfigureItemTypeAsErrorItem(ItemType systemItem, string exceptionMessage)
        {
            var messageValue = string.Format(ITEM_COULD_NOT_BE_COLLECTED, ObjectCollectorName, exceptionMessage);
            systemItem.message = MessageType.FromErrorString(messageValue);
            systemItem.status = StatusEnumeration.error;
        }

        /// <summary>
        /// This method is called by the probe just before iterating the object-to-collect list
        /// giving objectCollector implementations the possibility to prepare and optimize 
        /// the collection (adding objects to cache, initializing connections, etc)
        /// </summary>
        public virtual void PrepareCollectionOfObjects(IEnumerable<ObjectType> allItemsToCollect, VariablesEvaluated variables)
        {
            // By default, do nothing
        }
    }
}
