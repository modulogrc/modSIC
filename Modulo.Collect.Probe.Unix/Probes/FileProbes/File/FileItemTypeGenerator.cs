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
using System.Linq;
using Modulo.Collect.OVAL.Definitions.Unix;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using SystemCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using System.Text;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Unix.File
{
    public class FileItemTypeGenerator : AbstractFileItemTypeGenerator, IItemTypeGenerator
    {
        public new IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            return base.GetItemsToCollect(objectType, variables);
        }

        protected override ItemType CreateFileItem(string fullFilePath, bool containsFilePathEntity)
        {
            var newFileItem = new SystemCharacteristics.Unix.file_item() 
            { 
                filepath = new EntityItemStringType() { Value = fullFilePath } 
            };

            if (!containsFilePathEntity)
            {
                var pathParts = fullFilePath.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
                var stringBuilder = new StringBuilder();
                for (int i = 0; i < pathParts.Count() - 1; i++)
                    stringBuilder.Append(pathParts[i] + "/");
                
                newFileItem.filepath = null;
                newFileItem.path = base.CreateEntityItemWithValue(stringBuilder.ToString());
                newFileItem.filename = this.CreateEntityItemWithValue(pathParts.Last());
            }

            return newFileItem;
        }

        protected override bool HasVariableDefined(OVAL.Definitions.ObjectType objectType)
        {
            return ((OVAL.Definitions.Unix.file_object)objectType).HasVariableDefined();
        }

        protected override FamilyEnumeration GetPlatform()
        {
            return FamilyEnumeration.unix;
        }
    }

    //public class FileItemTypeGenerator : IItemTypeGenerator
    //{
    //    public virtual IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
    //    {
    //        var pathEntity = ((file_object)objectType).GetItemValue("path");
    //        var pathValues = this.EvaluateVariable((Modulo.Collect.OVAL.Definitions.EntityObjectStringType)pathEntity, variables);

    //        var filenameEntity = ((file_object)objectType).GetItemValue("filename");
    //        var filenameValues = this.EvaluateVariable((Modulo.Collect.OVAL.Definitions.EntityObjectStringType)filenameEntity, variables);

    //        var itemsToCollect = new List<ItemType>();
    //        foreach (var pathName in pathValues)
    //        {
    //            foreach (var fileName in filenameValues)
    //            {
    //                var newItemToCollect = new Modulo.Collect.OVAL.SystemCharacteristics.Unix.file_item()
    //                {
    //                    path = OvalHelper.CreateItemEntityWithStringValue(pathName),
    //                    filename = OvalHelper.CreateItemEntityWithStringValue(fileName)
    //                };
    //                itemsToCollect.Add(newItemToCollect);
    //            }
    //        }

    //        return itemsToCollect;
    //    }

    //    private IEnumerable<string> EvaluateVariable(Modulo.Collect.OVAL.Definitions.EntityObjectStringType variableNameEntity, VariablesEvaluated variables)
    //    {
    //        var variableEvaluator = new VariableEntityEvaluator(variables);
    //        return variableEvaluator.EvaluateVariableForEntity(variableNameEntity);
    //    }
    //}
}
