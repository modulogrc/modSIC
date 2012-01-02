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
using Modulo.Collect.OVAL.Definitions.VariableEvaluators;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.exceptions;

namespace Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents
{
    public class LocalVariableVariablesComponent : LocalVariableComponent
    {
        private VariableComponentType variable;
        private IEnumerable<VariableType> variablesOfDefinitions;
        private oval_system_characteristics systemCharacteristics;
        private VariableEvaluator variableEvaluator;
        private VariableType localVariable;

        public LocalVariableVariablesComponent(
            VariableType localVariable, 
            VariableComponentType variable, 
            IEnumerable<VariableType> variablesOfOvalDefinitions, 
            oval_system_characteristics systemCharacteristics,
            Modulo.Collect.OVAL.Variables.oval_variables externalVariables = null)
        {
            this.variablesOfDefinitions = variablesOfOvalDefinitions;
            this.variable = variable;
            this.systemCharacteristics = systemCharacteristics;
            this.localVariable = localVariable;
            this.variableEvaluator = new VariableEvaluator(variablesOfDefinitions, systemCharacteristics, externalVariables);
        }

        /// <summary>
        /// Gets the value from a variableComponent. 
        /// This method goes to get value of an other variable, that was referenced by the variableComponent.
        /// This method use the VariableEvaluator object for execute an evaluate.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetValue()
        {
            if (this.TheVariableReferencedIsEqualToLocalVariable())
                throw new VariableCircularReferenceException(String.Format("Circular Reference Error. The localVariable with id {0} is referenced by the variableComponent own.", localVariable.id));

            return this.variableEvaluator.EvaluateVariable(variable.var_ref);
        }

        private bool TheVariableReferencedIsEqualToLocalVariable()
        {
            return localVariable.id == variable.var_ref;
        }
    }
}
