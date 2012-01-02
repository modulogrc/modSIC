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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics.Solaris;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Common.Helpers;


namespace Modulo.Collect.Probe.Solaris.Probes.smf
{
    public class SMFItemTypeGenerator: IItemTypeGenerator
    {
        private const string NOT_SUPPORTED_OPERATION_MESSAGE = "The '{0}' operation is not supported for this entity.";

        public IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var fmriEntity = ((OVAL.Definitions.Solaris.smf_object)objectType).GetFmriEntity();
            
            if (fmriEntity.operation != OperationEnumeration.equals)
                return CreateSmfItemWithErrorStatus(fmriEntity);
            

            var fmriValues = new VariableEntityEvaluator(variables).EvaluateVariableForEntity(fmriEntity);
            var itemsToCollect = new List<ItemType>();
            foreach(var fmriValue in fmriValues)
            {
                var newSmfItem = new smf_item() { fmri = OvalHelper.CreateItemEntityWithStringValue(fmriValue) };
                itemsToCollect.Add(newSmfItem);
            }

            return itemsToCollect;
        }

        private IEnumerable<ItemType> CreateSmfItemWithErrorStatus(OVAL.Definitions.EntitySimpleBaseType fmriEntity)
        {
            var newItemWithErrorStatus = new smf_item()
            {
                status = StatusEnumeration.error,
                message = MessageType.FromErrorString(String.Format(NOT_SUPPORTED_OPERATION_MESSAGE, fmriEntity.operation.ToString())),
                fmri = new EntityItemStringType() { status = StatusEnumeration.error }
            };


            return new ItemType[] { newItemWithErrorStatus };
        }
    }
}
