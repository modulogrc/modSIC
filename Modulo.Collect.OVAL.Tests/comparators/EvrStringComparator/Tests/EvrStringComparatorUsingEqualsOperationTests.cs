/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Common;
using System.Text.RegularExpressions;
using Modulo.Collect.OVAL.Tests.comparators.EvrStringComparatorTests;

namespace Modulo.Collect.OVAL.Tests.comparators
{
    [TestClass]
    public class EvrStringComparatorUsingEqualsOperationTests: EvrStringComparatorTestsBase
    {
        public EvrStringComparatorUsingEqualsOperationTests() : base(OperationEnumeration.equals) { }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_compare_two_EvrStrings_using_Equals_OvalOperation()
        {
            MakeSureThatThe
                .ComparingEvrString("4:12.9-2")
                    .To("4:12.9-2")
            .IsTrue();

            MakeSureThatThe
                .ComparingEvrString("4:12.9-2")
                    .To("4:12.9-1")
            .IsFalse();

            MakeSureThatThe
                .ComparingEvrString("4:12.9-2")
                    .To("4:129-2")
            .IsFalse();

            MakeSureThatThe
                .ComparingEvrString("04:129-2")
                    .To("4:129-2")
            .IsTrue();
        }

        [TestMethod, Owner("jcastro")]
        public void Should_be_possible_to_compare_EvrStrings_with_mixed_alpha_and_numeric()
        {
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2011foo")
                    .To("1:2.7.21-2011foo")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2011foo")
                    .To("1:2.7.21-2011bar")
            .IsFalse();

            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2.el5")
                    .To("1:2.7.21-2.el5")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2.el5")
                    .To("1:2.7.21-2.el4")
            .IsFalse();
        }

        [TestMethod, Owner("jcastro")]
        public void Should_be_possible_to_compare_EvrStrings_with_lots_of_dots()
        {
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1-2")
                    .To("1:2.17.4.2.149.1-2")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1-2")
                    .To("1:2.17.4.2.149.2-2")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1-2")
                    .To("1:2.17.4.2.14.1-2")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1-2")
                    .To("1:2.17.4.2.149-2")
            .IsFalse();

            MakeSureThatThe
                .ComparingEvrString("0:12.9-2.5.78.3.1.3")
                    .To("0:12.9-2.5.78.3.1.3")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("0:12.9-2.5.78.3.1.301")
                    .To("0:12.9-2.5.78.3.1.3")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("0:12.9-2.5.78.3.1.3")
                    .To("0:12.9-2.5.78.301.1.3")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("0:12.9-2.5.78.3.1.3")
                    .To("0:12.9-2.5.78.3.1.3.01")
            .IsFalse();
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_compare_EvrStrings_without_epoch_part()
        {
            MakeSureThatThe.ComparingEvrString("1.0-0").To("1.0-0").IsTrue();
            MakeSureThatThe.ComparingEvrString("1.0-0").To("1.0-1").IsFalse();
        }
    }
}
