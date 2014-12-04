/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Independent.Sql57
{
    public class Sql57ItemTypeGenerator: IItemTypeGenerator
    {
        public IEnumerable<ItemType> GetItemsToCollect(
            OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {

            var engineValues = ProcessVariableForEntity(objectType, sql57_object_choices.engine, variables);
            var versionValues = ProcessVariableForEntity(objectType, sql57_object_choices.version, variables);
            var connectionStringValues = ProcessVariableForEntity(objectType, sql57_object_choices.connection_string, variables);
            var sqlValues = ProcessVariableForEntity(objectType, sql57_object_choices.sql, variables);

            IList<ItemType> itemsToCollect = new List<ItemType>();
            foreach (var engineValue in engineValues)
                foreach (var versionValue in versionValues)
                    foreach (var connectionStringValue in connectionStringValues)
                        foreach (var sqlValue in sqlValues)
                            itemsToCollect.Add(sql57_item.CreateSqlItem(engineValue, versionValue, connectionStringValue, sqlValue));

            return itemsToCollect;
        }

        private IEnumerable<string> ProcessVariableForEntity(
            OVAL.Definitions.ObjectType sourceObject,
            sql57_object_choices entityName,
            VariablesEvaluated evaluatedVariables)
        {
            var entity = ((sql57_object)sourceObject).GetObjectEntityByName(entityName);
            return new VariableEntityEvaluator(evaluatedVariables).EvaluateVariableForEntity(entity);
        }
    }
}
