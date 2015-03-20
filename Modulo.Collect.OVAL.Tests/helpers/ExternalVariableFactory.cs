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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Variables;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.OVAL.Tests.helpers
{
    public class ExternalVariableFactory
    {
        private  oval_definitions OvalDefinitions;

        public ExternalVariableFactory(string definitionsResourceNameOnly)
        {
            this.OvalDefinitions = new OvalDocumentLoader().GetFakeOvalDefinitions(definitionsResourceNameOnly);
        }

        public VariablesTypeVariableExternal_variable GetExternalVariableFromDefinitionsById(string externalVariableId)
        {
            var externalVaribles = OvalDefinitions.variables.OfType<VariablesTypeVariableExternal_variable>();
            return externalVaribles.Single(x => x.id == externalVariableId);
        }

        public oval_variables CreateOvalVariablesDocument()
        {
            var externalVariables = OvalDefinitions.variables.OfType<VariablesTypeVariableExternal_variable>();
            var newVariables = NewOvalVariables();
            newVariables.variables = 
                externalVariables
                    .Select(v =>
                        new OVAL.Variables.VariableType(v.datatype, v.id, GetVariableDefaultValue(v.datatype))).ToList();

            return newVariables;
        }

        public oval_variables CreateOvalVariablesDocument(Dictionary<string, string[]> variableValues)
        {
            var newVariables = NewOvalVariables();
            
            var externalVariables = OvalDefinitions.variables.OfType<VariablesTypeVariableExternal_variable>();
            foreach (var variable in externalVariables)
            {
                var values = variableValues.Single(var => var.Key.Equals(variable.id)).Value;
                newVariables.variables.Add(new OVAL.Variables.VariableType(variable.datatype, variable.id, values));
            }
            
            return newVariables;
        }

        private oval_variables NewOvalVariables()
        {
            return new oval_variables 
            { 
                generator = DocumentHelpers.GetDefaultGenerator(), 
                variables = new List<OVAL.Variables.VariableType>() 
            };
        }


        private string GetVariableDefaultValue(SimpleDatatypeEnumeration datatype)
        {
            switch (datatype)
            {
                case SimpleDatatypeEnumeration.binary:
                    return "0";
                case SimpleDatatypeEnumeration.boolean:
                    return "0";
                case SimpleDatatypeEnumeration.@float:
                    return "0.0";
                case SimpleDatatypeEnumeration.@int:
                    return "0";
                case SimpleDatatypeEnumeration.@string:
                    return string.Empty;
                
                case SimpleDatatypeEnumeration.evr_string:
                case SimpleDatatypeEnumeration.fileset_revision:
                case SimpleDatatypeEnumeration.ios_version:
                case SimpleDatatypeEnumeration.ipv4_address:
                case SimpleDatatypeEnumeration.ipv6_address:
                case SimpleDatatypeEnumeration.version:
                default:
                    return string.Empty;
            }
        }
    }
}
