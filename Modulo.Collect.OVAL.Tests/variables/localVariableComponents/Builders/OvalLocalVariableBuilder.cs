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

namespace Modulo.Collect.OVAL.Tests.variables.localVariableComponents.Builders
{
    public class OvalLocalVariableBuilder
    {

        private VariablesTypeVariableLocal_variable localVariable;
        private OvalTimeDifferenceBuilder timeDifferenceBuilder;
        private OvalSplitBuilder splitBuilder;
        private OvalConcatBuilder concatBuilder;
        private OvalUniqueBuilder uniqueBuilder; 
        private OvalRegexCaptureBuilder regexCaptureBuilder;
        private OvalSubstringBuilder substringBuilder;
        private OvalArithmeticBuilder arithmeticBuilder;
        private OvalEndBuilder endBuilder;
        private OvalBeginBuilder beginBuilder;
        private OvalEscapeRegexBuilder escapeRegexBuilder;
        private OvalCountBuilder countBuilder ; 
        public OvalLocalVariableBuilder CreateTheLocalVariable()
        {
            this.localVariable = new VariablesTypeVariableLocal_variable();
            return this;
        }

        public OvalLocalVariableBuilder AddItemInTheLocalVariable(object component)
        {
            this.localVariable.Item = component;
            return this;
        }

        public OvalTimeDifferenceBuilder WithTimeDifference()
        {
            this.timeDifferenceBuilder = new OvalTimeDifferenceBuilder(this);
            return this.timeDifferenceBuilder;
        }

        public VariablesTypeVariableLocal_variable Build()
        {
            return localVariable;
        }

        public OvalSplitBuilder WithSplit()
        {
            this.splitBuilder = new OvalSplitBuilder(this);
            return this.splitBuilder;
        }

        public OvalConcatBuilder WithConcat()
        {
            this.concatBuilder = new OvalConcatBuilder(this);
            return this.concatBuilder;
        }
        
        public OvalUniqueBuilder WithUnique()
        {
            this.uniqueBuilder = new OvalUniqueBuilder(this);
            return this.uniqueBuilder;
        }

        public OvalRegexCaptureBuilder WithRegexCapture()
        {
            this.regexCaptureBuilder = new OvalRegexCaptureBuilder(this);
            return this.regexCaptureBuilder;
        }

        public OvalSubstringBuilder WithSubstring()
        {
            this.substringBuilder = new OvalSubstringBuilder(this);
            return this.substringBuilder;
        }

        public OvalArithmeticBuilder WithArithmetic()
        {
            this.arithmeticBuilder = new OvalArithmeticBuilder(this);
            return this.arithmeticBuilder;
        }

        public OvalEndBuilder WithEnd()
        {
            this.endBuilder = new OvalEndBuilder(this);
            return this.endBuilder;
        }

        public OvalBeginBuilder WithBegin()
        {
            this.beginBuilder = new OvalBeginBuilder(this);
            return this.beginBuilder;
        }

        public OvalEscapeRegexBuilder WithEscapeRegex()
        {
            this.escapeRegexBuilder = new OvalEscapeRegexBuilder(this);
            return this.escapeRegexBuilder;
        }

        public OvalCountBuilder WithCount()
        {
            this.countBuilder = new OvalCountBuilder(this);
            return this.countBuilder;
        }
    }
}
