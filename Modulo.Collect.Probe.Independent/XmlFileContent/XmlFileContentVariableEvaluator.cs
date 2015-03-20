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
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Independent.XmlFileContent
{
    public class XmlFileContentVariableEvaluator
    {
        private VariableEntityEvaluator VariableEntityEvaluator;

        public XmlFileContentVariableEvaluator(VariablesEvaluated variables)
        {
            this.VariableEntityEvaluator = new VariableEntityEvaluator(variables);
        }

        public IEnumerable<ObjectType> ProcessVariables(ObjectType objectType)
        {
            var xmlfilecontentObject = (xmlfilecontent_object)objectType;
            
            string[] filepaths = null;
            string[] paths = null;
            string[] filenames = null;

            if (xmlfilecontentObject.IsFilePathDefined())
                filepaths = this.EvaluateEntity(xmlfilecontentObject, xmlfilecontent_ItemsChoices.filepath).ToArray();
            else
            {
                paths = this.EvaluateEntity(xmlfilecontentObject, xmlfilecontent_ItemsChoices.path).ToArray();
                filenames = this.EvaluateEntity(xmlfilecontentObject, xmlfilecontent_ItemsChoices.filename).ToArray();
            }

            var xpaths = this.EvaluateEntity(xmlfilecontentObject, xmlfilecontent_ItemsChoices.xpath).ToArray();

            
            return new XmlFileContentObjectFactory()
                .CreateXmlFileContentObjects(xmlfilecontentObject, filepaths, paths, filenames, xpaths);
                
        }

        private IEnumerable<String> EvaluateEntity(
            xmlfilecontent_object xmlfilecontentObject, xmlfilecontent_ItemsChoices entityName)
        {
            var entity = xmlfilecontentObject.GetAllObjectEntities()[entityName.ToString()];
            return this.VariableEntityEvaluator.EvaluateVariableForEntity(entity);
        }
    }
}
