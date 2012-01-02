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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents;
using Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions;

namespace Modulo.Collect.OVAL.Definitions.VariableEvaluators.Evaluators.LocalVariableComponents
{
    public class LocalVariableComponentsFactory
    {
        private oval_system_characteristics systemCharacteristics;
        private IEnumerable<VariableType> variablesOfDefinitions;

        public LocalVariableComponentsFactory(oval_system_characteristics systemCharacteristics, IEnumerable<VariableType> variablesOfDefinitions)
        {
            this.variablesOfDefinitions = variablesOfDefinitions;
            this.systemCharacteristics = systemCharacteristics;
        }

        /// <summary>
        /// Gets the correct instance of localVariableComponent.
        /// </summary>
        /// <param name="localVariable">The local variable.</param>
        /// <returns></returns>
        public LocalVariableComponent GetLocalVariableComponent(VariablesTypeVariableLocal_variable localVariable)
        {
            return this.InstantiateTheCorrectTypeOfComponent(localVariable.Item, localVariable);
        }

        /// <summary>
        /// Instantiates the correct type of component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="localVariable">The local variable.</param>
        /// <returns></returns>
        private LocalVariableComponent InstantiateTheCorrectTypeOfComponent(Object component,VariableType localVariable)
        {
            if (component is ObjectComponentType)
                return new LocalVariableObjectComponent((ObjectComponentType)component, systemCharacteristics);
            if (component is VariableComponentType)
                return new LocalVariableVariablesComponent(localVariable, (VariableComponentType)component, this.variablesOfDefinitions, this.systemCharacteristics);
            if (component is LiteralComponentType)
                return new LocalVariableLiteralComponent((LiteralComponentType)component);
            if (component is ConcatFunctionType)
                return this.CreateConcatFunctionComponent(component,localVariable);
            if (component is ArithmeticFunctionType)
                return this.CreateArithmeticFunctionComponent(component, localVariable);
            if (component is BeginFunctionType)
                return this.CreateBeginFunctionComponent(component, localVariable);
            if (component is EndFunctionType)
                return this.CreateEndFunctionComponent(component, localVariable);
            if (component is SubstringFunctionType)
                return this.CreateSubstringFunctionComponent(component, localVariable);
            if (component is SplitFunctionType)
                return this.CreateSplitFunctionComponent(component, localVariable);
            if (component is TimeDifferenceFunctionType)
                return this.CreateTimeDifferenceComponent(component, localVariable);
            if (component is RegexCaptureFunctionType)
                return this.CreateRegexCaptureFunctionComponent(component, localVariable);
            if (component is EscapeRegexFunctionType)
                return this.CreateEscapeRegexFunctionComponent(component, localVariable);
            return null;
        }               
   

        /// <summary>
        /// Creates the concat function component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="localVariable">The local variable.</param>
        /// <returns></returns>
        private LocalVariableComponent CreateConcatFunctionComponent(Object component, VariableType localVariable)
        {
            ConcatFunctionType concatFunctionType = (ConcatFunctionType)component;
            ConcatFunctionComponent concatFunctionComponent = new ConcatFunctionComponent(concatFunctionType, this.variablesOfDefinitions,this.systemCharacteristics);

            for (int i = 0; i < concatFunctionType.Items.Count(); i++)
            {
                LocalVariableComponent variableComponent = this.InstantiateTheCorrectTypeOfComponent(concatFunctionType.Items[i],localVariable);
                concatFunctionComponent.AddComponent(variableComponent);
            }
            return concatFunctionComponent;
        }

        private LocalVariableComponent CreateArithmeticFunctionComponent(Object component, VariableType localVariable)
        {
            ArithmeticFunctionType arithmeticFunctionType = (ArithmeticFunctionType)component;
            ArithmeticFunctionComponent arithmeticFunctionComponent = new ArithmeticFunctionComponent(arithmeticFunctionType);

            for (int i = 0; i < arithmeticFunctionType.Items.Count(); i++)
            {
                LocalVariableComponent variableComponent = this.InstantiateTheCorrectTypeOfComponent(arithmeticFunctionType.Items[i],localVariable);
                arithmeticFunctionComponent.AddComponent(variableComponent);
            }
            return arithmeticFunctionComponent;
        }

        private LocalVariableComponent CreateBeginFunctionComponent(object component, VariableType localVariable)
        {
            BeginFunctionType beginFunctionType = (BeginFunctionType)component;
            BeginFunctionComponent beginFunctionComponent = new BeginFunctionComponent(beginFunctionType);
            LocalVariableComponent localVariableComponent = this.InstantiateTheCorrectTypeOfComponent(beginFunctionType.Item, localVariable);
            beginFunctionComponent.AddComponent(localVariableComponent);
            return beginFunctionComponent;
        }

        private LocalVariableComponent CreateEndFunctionComponent(object component, VariableType localVariable)
        {
            EndFunctionType endFunctionType = (EndFunctionType)component;
            EndFunctionComponent endFunctionComponent = new EndFunctionComponent(endFunctionType);
            LocalVariableComponent localVariableComponent = this.InstantiateTheCorrectTypeOfComponent(endFunctionType.Item, localVariable);
            endFunctionComponent.AddComponent(localVariableComponent);
            return endFunctionComponent;

        }

        private LocalVariableComponent CreateSubstringFunctionComponent(Object component, VariableType localVariable)
        {
            SubstringFunctionType substringFunctionType = (SubstringFunctionType)component;
            SubStringFunctionComponent subStringFunctionComponent = new SubStringFunctionComponent(substringFunctionType);
            LocalVariableComponent variableComponent = this.InstantiateTheCorrectTypeOfComponent(substringFunctionType.Item, localVariable);
            subStringFunctionComponent.AddComponent(variableComponent);
            return subStringFunctionComponent;
        }


        private LocalVariableComponent CreateSplitFunctionComponent(object component, VariableType localVariable)
        {
            SplitFunctionType splitFunctionType = (SplitFunctionType)component;
            SplitFunctionComponent splitFunctionComponent = new SplitFunctionComponent(splitFunctionType);
            LocalVariableComponent variableComponent = this.InstantiateTheCorrectTypeOfComponent(splitFunctionType.Item, localVariable);
            splitFunctionComponent.AddComponent(variableComponent);
            return splitFunctionComponent;
        }

        private LocalVariableComponent CreateTimeDifferenceComponent(object component, VariableType localVariable)
        {
            TimeDifferenceFunctionType timeDifferenceType = (TimeDifferenceFunctionType)component;
            TimeDifferenceFunctionComponent timeDifferenceComponent = new TimeDifferenceFunctionComponent(timeDifferenceType);

            for (int i = 0; i < timeDifferenceType.Items.Count(); i++)
            {
                LocalVariableComponent variableComponent = this.InstantiateTheCorrectTypeOfComponent(timeDifferenceType.Items[i],localVariable);
                timeDifferenceComponent.AddComponent(variableComponent);
            }
            return timeDifferenceComponent;
        }

        private LocalVariableComponent CreateRegexCaptureFunctionComponent(object component, VariableType localVariable)
        {
            RegexCaptureFunctionType regexCaptureFunctionType = (RegexCaptureFunctionType)component;
            RegexCaptureFunctionComponent regexCaptureFunctionComponent = new RegexCaptureFunctionComponent(regexCaptureFunctionType);
            LocalVariableComponent variableComponent = this.InstantiateTheCorrectTypeOfComponent(regexCaptureFunctionType.Item, localVariable);
            regexCaptureFunctionComponent.AddComponent(variableComponent);
            return regexCaptureFunctionComponent;
        }

        private LocalVariableComponent CreateEscapeRegexFunctionComponent(object component, VariableType localVariable)
        {
            EscapeRegexFunctionType escapeRegexFunctionType = (EscapeRegexFunctionType)component;
            EscapeRegexFunctionComponent escapeRegexFunctionComponent = new EscapeRegexFunctionComponent(escapeRegexFunctionType);
            LocalVariableComponent variableComponent = this.InstantiateTheCorrectTypeOfComponent(escapeRegexFunctionType.Item, localVariable);
            escapeRegexFunctionComponent.AddComponent(variableComponent);
            return escapeRegexFunctionComponent;
        }   
    }
}
