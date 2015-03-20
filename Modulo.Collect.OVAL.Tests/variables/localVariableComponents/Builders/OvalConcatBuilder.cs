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

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.Builders
{
    
   public class OvalUniqueBuilder
    {
        private OvalLocalVariableBuilder variableBuilder;
        private UniqueFunctionType concatFunction;
        private List<object> items;

        public OvalUniqueBuilder(OvalLocalVariableBuilder localVariableBuilder)
        {
            this.variableBuilder = localVariableBuilder;
            this.concatFunction = new UniqueFunctionType();
            this.items = new List<object>();
        }

        public OvalUniqueBuilder AddLiteralComponent(string value)
        {
            LiteralComponentType literalComponent = new LiteralComponentType() { Value = value };
            this.items.Add(literalComponent);
            return this;
        }

        public OvalUniqueBuilder AddObjectComponent(string objectRef, string itemField)
        {
            ObjectComponentType objectComponent = new ObjectComponentType() { object_ref = objectRef, item_field = itemField };
            this.items.Add(objectComponent);
            return this;
        }

        public OvalLocalVariableBuilder SetInLocalVariable()
        {
            this.concatFunction.Items = this.items.ToArray();
            this.variableBuilder.AddItemInTheLocalVariable(concatFunction);
            return this.variableBuilder;
        }


    }
   public class OvalCountBuilder
   {
       private OvalLocalVariableBuilder variableBuilder;
       private CountFunctionType concatFunction;
       private List<object> items;

       public OvalCountBuilder(OvalLocalVariableBuilder localVariableBuilder)
       {
           this.variableBuilder = localVariableBuilder;
           this.concatFunction = new CountFunctionType();
           this.items = new List<object>();
       }

       public OvalCountBuilder AddLiteralComponent(string value)
       {
           LiteralComponentType literalComponent = new LiteralComponentType() { Value = value };
           this.items.Add(literalComponent);
           return this;
       }

       public OvalCountBuilder AddObjectComponent(string objectRef, string itemField)
       {
           ObjectComponentType objectComponent = new ObjectComponentType() { object_ref = objectRef, item_field = itemField };
           this.items.Add(objectComponent);
           return this;
       }

       public OvalLocalVariableBuilder SetInLocalVariable()
       {
           this.concatFunction.Items = this.items.ToArray();
           this.variableBuilder.AddItemInTheLocalVariable(concatFunction);
           return this.variableBuilder;
       }


   }

    public class OvalConcatBuilder
    {
        private OvalLocalVariableBuilder variableBuilder;
        private ConcatFunctionType concatFunction;
        private List<object> items;

        public OvalConcatBuilder(OvalLocalVariableBuilder localVariableBuilder)
        {
            this.variableBuilder = localVariableBuilder;
            this.concatFunction = new ConcatFunctionType();
            this.items = new List<object>();
        }

        public OvalConcatBuilder AddLiteralComponent(string value)
        {
            LiteralComponentType literalComponent = new LiteralComponentType() { Value = value };
            this.items.Add(literalComponent);
            return this;
        }

        public OvalConcatBuilder AddObjectComponent(string objectRef, string itemField)
        {
            ObjectComponentType objectComponent = new ObjectComponentType() { object_ref = objectRef, item_field = itemField };
            this.items.Add(objectComponent);
            return this;
        }

        public OvalLocalVariableBuilder SetInLocalVariable()
        {
            this.concatFunction.Items = this.items.ToArray();
            this.variableBuilder.AddItemInTheLocalVariable(concatFunction);
            return this.variableBuilder;
        }


    }
}
