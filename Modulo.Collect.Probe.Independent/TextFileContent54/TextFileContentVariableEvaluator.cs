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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions;
using Definitions = Modulo.Collect.OVAL.Definitions;

using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Independent.TextFileContent54
{
    public class TextFileContentVariableEvaluator
    {
        private FilePathEntityVariableEvaluator FilePathVarEvaluator = null;

        public TextFileContentVariableEvaluator(VariablesEvaluated variables)
        {
            this.FilePathVarEvaluator = new FilePathEntityVariableEvaluator(variables);
        }

        public IEnumerable<String> ProcessVariablesForTypeFilePathEntities(Definitions.ObjectType objectType)
        {
            textfilecontent54_object fileContentObj = (textfilecontent54_object)objectType;

            var variablesFromFilePath = this.EvaluateVariableForEntity(fileContentObj, textfilecontent54_ItemsChoices.filepath);
            if ((variablesFromFilePath != null) && (variablesFromFilePath.Count() > 0))
                return variablesFromFilePath;

            var variablesFromPath = this.EvaluateVariableForEntity(fileContentObj, textfilecontent54_ItemsChoices.path);
            var variablesFromFilename = this.EvaluateVariableForEntity(fileContentObj, textfilecontent54_ItemsChoices.filename);

            return this.CreateFilePathsListByCombinationOfEntities(variablesFromPath, variablesFromFilename);
        }

        public IEnumerable<String> EvaluateVariableForEntity(textfilecontent54_object fileContentObj, textfilecontent54_ItemsChoices entityName)
        {
            var fileObjectEntity = (EntitySimpleBaseType)fileContentObj.GetItemValue(entityName);
            var evaluatedEntity = this.FilePathVarEvaluator.EvaluateEntityVariable(fileObjectEntity);
            return evaluatedEntity == null ? null : evaluatedEntity.Distinct().ToArray();
        }

        private IEnumerable<string> CreateFilePathsListByCombinationOfEntities(
            IEnumerable<string> variablesFromPath, IEnumerable<string> variablesFromFilename)
        {
            List<String> result = new List<String>();
            foreach (var path in variablesFromPath)
                foreach (var filename in variablesFromFilename)
                    result.Add(string.Format(@"{0}\{1}", path, filename));

            return result;
        }
    }
}
