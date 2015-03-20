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
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.FileAuditedPermissions53
{
    public class FileAuditedPermissionsVariableEvaluator
    {
        public VariableEntityEvaluator VariableEntityEvaluator { get; set; }

        public FileAuditedPermissionsVariableEvaluator(VariablesEvaluated variables)
        {
            this.VariableEntityEvaluator = new VariableEntityEvaluator(variables);
        }

        public  IEnumerable<ObjectType> ProcessVariables(ObjectType objectType)
        {
            var fileAuditedPermissionsObject = (fileauditedpermissions53_object)objectType;

            string[] filepaths = null;
            string[] paths = null;
            string[] filenames = null;

            if (fileAuditedPermissionsObject.IsFilePathDefined())
                filepaths = this.EvaluateEntity(fileAuditedPermissionsObject, fileauditedpermissions53_objectItemsChoices.filepath).ToArray();
            else
            {
                paths = this.EvaluateEntity(fileAuditedPermissionsObject, fileauditedpermissions53_objectItemsChoices.path).ToArray();
                filenames = this.EvaluateEntity(fileAuditedPermissionsObject, fileauditedpermissions53_objectItemsChoices.filename).ToArray();
            }

            var trusteeSIDs = this.EvaluateEntity(fileAuditedPermissionsObject, fileauditedpermissions53_objectItemsChoices.trustee_sid).ToArray();

            return new FileAuditedPermissions53ObjectFactory().CreateFileAuditedPermissions53Objects(
                fileAuditedPermissionsObject, filepaths, paths, filenames, trusteeSIDs);
        }

        private IEnumerable<String> EvaluateEntity(
            fileauditedpermissions53_object fileAuditedPermissions53Object, 
            fileauditedpermissions53_objectItemsChoices entityName)
        {
            var entity = fileAuditedPermissions53Object.GetAllObjectEntities()[entityName.ToString()];
            return this.VariableEntityEvaluator.EvaluateVariableForEntity(entity);
        }
        
    }
}
