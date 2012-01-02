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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Independent.Common.File
{
    public class FileEntityVariableEvaluator
    {
        private FilePathEntityVariableEvaluator FilePathVarEvaluator = null;

        public FileEntityVariableEvaluator(VariablesEvaluated variables)
        {
            this.FilePathVarEvaluator = new FilePathEntityVariableEvaluator(variables);
        }

        public IEnumerable<ObjectType> ProcessVariables(OVAL.Definitions.Windows.file_object fileObject)
        {
            //var fileObject = (file_object)objectType;
            var variablesFromFilePath = this.evaluateVariableForFileEntity(fileObject, file_object_ItemsChoices.filepath);
            if ((variablesFromFilePath != null) && (variablesFromFilePath.Count() > 0))
                return new FileObjectTypeFactory().CreateObjectTypeByCombinationOfEntities(fileObject, variablesFromFilePath, null, null);

            var variablesFromPath = this.evaluateVariableForFileEntity(fileObject, file_object_ItemsChoices.path);
            var variablesFromFilename = this.evaluateVariableForFileEntity(fileObject, file_object_ItemsChoices.filename);

            var isTherePath = (variablesFromPath != null) && (variablesFromPath.Count() > 0);
            if (isTherePath && (variablesFromFilename == null))
                variablesFromFilename = new string[] { "" };

            return new FileObjectTypeFactory().CreateObjectTypeByCombinationOfEntities(fileObject, null, variablesFromPath, variablesFromFilename);
        }

        public IEnumerable<ObjectType> ProcessVariables(OVAL.Definitions.Unix.file_object fileObject)
        {
            //var fileObject = (file_object)objectType;
            var variablesFromFilePath = this.evaluateVariableForFileEntity(fileObject, OVAL.Definitions.Unix.ItemsChoiceType3.filepath);
            if ((variablesFromFilePath != null) && (variablesFromFilePath.Count() > 0))
                return new FileObjectTypeFactory().CreateObjectTypeByCombinationOfEntities(fileObject, variablesFromFilePath, null, null);

            var variablesFromPath = this.evaluateVariableForFileEntity(fileObject, OVAL.Definitions.Unix.ItemsChoiceType3.path);
            var variablesFromFilename = this.evaluateVariableForFileEntity(fileObject, OVAL.Definitions.Unix.ItemsChoiceType3.filename);

            var isTherePath = (variablesFromPath != null) && (variablesFromPath.Count() > 0) && (variablesFromPath.Where<String>(v => v == null).Count() == 0);
            if (isTherePath && (variablesFromFilename == null))
                variablesFromFilename = new string[] { "" };

            return new FileObjectTypeFactory().CreateObjectTypeByCombinationOfEntities(fileObject, null, variablesFromPath, variablesFromFilename);
        }


        private IEnumerable<String> evaluateVariableForFileEntity(OVAL.Definitions.Windows.file_object fileObject, file_object_ItemsChoices fileObjectEntityItem)
        {
            var fileObjectEntity = (EntityObjectStringType)fileObject.GetItemValue(fileObjectEntityItem);
            var evaluatedEntity = this.FilePathVarEvaluator.EvaluateEntityVariable(fileObjectEntity);
            return evaluatedEntity == null ? null : evaluatedEntity.Distinct().ToArray();
        }

        private IEnumerable<String> evaluateVariableForFileEntity(OVAL.Definitions.Unix.file_object fileObject, OVAL.Definitions.Unix.ItemsChoiceType3 fileObjectEntityItem)
        {
            var fileObjectEntity = (EntityObjectStringType)fileObject.GetItemValue(fileObjectEntityItem);
            var evaluatedEntity = this.FilePathVarEvaluator.EvaluateEntityVariable(fileObjectEntity);
            return evaluatedEntity == null ? null : evaluatedEntity.Distinct().ToArray();
        }

        private bool IsVariablesWasProcessed(IEnumerable<string> variablesFromFilePath, IEnumerable<string> variablesFromPath, IEnumerable<string> variablesFromFileName)
        {
            return (variablesFromFilePath.Count() > 0) || (variablesFromPath.Count() > 0) || (variablesFromFileName.Count() > 0);
        }
    }
}

