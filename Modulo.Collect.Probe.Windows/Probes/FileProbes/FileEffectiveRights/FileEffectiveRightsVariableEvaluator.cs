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
using System.Text;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions;
using System.Collections;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.FileEffectiveRights
{
    public class FileEffectiveRightsVariableEvaluator
    {

        private VariablesEvaluated variables;
        private VariableEntityEvaluator variableEvaluator;

        public FileEffectiveRightsVariableEvaluator(VariablesEvaluated variable)
        {
            this.variables = variable;
            variableEvaluator = new VariableEntityEvaluator(this.variables);
        }

        public IEnumerable<ObjectType> ProcessVariables(ObjectType objectType)
        {
            List<ObjectType> fileEffectiveRightObjects = new List<ObjectType>();
            fileeffectiverights_object fileEffectiveRights = (fileeffectiverights_object)objectType;


            IEnumerable<string> fileNames = this.processVariablesFromEntity(fileEffectiveRights,fileeffectiverights_object_ItemsChoices.filename);
            IEnumerable<string> paths = this.processVariablesFromEntity(fileEffectiveRights,fileeffectiverights_object_ItemsChoices.path);
            IEnumerable<string> trustee_names = this.processVariablesFromEntity(fileEffectiveRights, fileeffectiverights_object_ItemsChoices.trustee_name);

            if (this.IsVariablesWasProcessed(fileEffectiveRights,fileNames, paths, trustee_names))
            {
                FileEffectiveRightsObjectTypeFactory factory = new FileEffectiveRightsObjectTypeFactory();
                var objectTypes = factory.CreateObjectTypeByCombinationOfEntities(fileEffectiveRights, paths, fileNames, trustee_names);
                fileEffectiveRightObjects.AddRange(objectTypes);
            }
            return fileEffectiveRightObjects;

        }

        private bool IsVariablesWasProcessed(fileeffectiverights_object fileEffectiveRights,IEnumerable<string> fileNames, IEnumerable<string> paths, IEnumerable<string> trustee_names)
        {
            EntitySimpleBaseType path = (EntitySimpleBaseType)fileEffectiveRights.GetItemValue(fileeffectiverights_object_ItemsChoices.path);
            EntitySimpleBaseType fileName = (EntitySimpleBaseType)fileEffectiveRights.GetItemValue(fileeffectiverights_object_ItemsChoices.filename);
            EntitySimpleBaseType trustee_name = (EntitySimpleBaseType)fileEffectiveRights.GetItemValue(fileeffectiverights_object_ItemsChoices.trustee_name);
            bool result = true;
            if (!string.IsNullOrEmpty(path.var_ref))
                result = result && ((paths.Count() > 0) && (!string.IsNullOrEmpty(paths.First())));
            
            if (!string.IsNullOrEmpty(fileName.var_ref))
                result = result && ((fileNames.Count() > 0) && (!string.IsNullOrEmpty(fileNames.First())));
            
            if (!string.IsNullOrEmpty(trustee_name.var_ref)) 
                result = result && ((trustee_names.Count() > 0) && (!string.IsNullOrEmpty(trustee_names.First())));

            return result;
        }

        private IEnumerable<string> processVariablesFromEntity(fileeffectiverights_object fileEffectiveRights,fileeffectiverights_object_ItemsChoices itemChoice)
        {
            List<string> values = new List<string>();
            EntityObjectStringType entity = (EntityObjectStringType)fileEffectiveRights.GetItemValue(itemChoice);
            if (entity != null)
            {
                values.AddRange(this.processVariableForEntity(entity));
            }
            return values;
        }

                

        private IEnumerable<string> processVariableForEntity(EntityObjectStringType entity)
        {
            List<string> variables = new List<string>();
            if (entity == null)
                return variables;
            
            variables.AddRange(variableEvaluator.EvaluateVariableForEntity(entity));
            if (variables.Count() == 0)
                variables.Add(entity.Value);
            return variables;
        }

        

    }
}
