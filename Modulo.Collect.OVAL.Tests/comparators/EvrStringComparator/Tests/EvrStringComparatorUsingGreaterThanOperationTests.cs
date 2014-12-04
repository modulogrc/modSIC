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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Tests.comparators.EvrStringComparatorTests;

namespace Modulo.Collect.OVAL.Tests.comparators
{
    [TestClass]
    public class EvrStringComparatorUsingGreaterThanOperationTests : EvrStringComparatorTestsBase
    {
        public EvrStringComparatorUsingGreaterThanOperationTests() : base(OperationEnumeration.greaterthan) { }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_compare_two_EvrStrings_using_GreaterThan_OvalOperation()
        {
            // When the first EVR String is greater than second one, the EvrComparator must return TRUE.
            MakeSureThatThe
                .ComparingEvrString("4:12.9-1")
                    .To("4:12.9-0")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("4:13.0-1")
                    .To("4:12.9-1")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("1:12.9-1")
                    .To("12.9-1")
            .IsTrue();

            // When the first EVR String is equal to second one, the EvrComparator must return FALSE.
            MakeSureThatThe
                .ComparingEvrString("4:12.9-0")
                    .To("4:12.9-0")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("4:13.0-1")
                    .To("4:13.0-1")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("1:12.9-1")
                    .To("1:12.9-1")
            .IsFalse();

            // When the first EVR String is less than second one, the EvrComparator must return FALSE.
            MakeSureThatThe
                .ComparingEvrString("4:12.9-0")
                    .To("4:12.9-1")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("4:11.0-1")
                    .To("4:13.0-1")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("12.9-1")
                    .To("1:12.9-1")
            .IsFalse();
        }

        [TestMethod, Owner("jcastro")]
        public void Should_be_possible_to_compare_two_EvrStrings_with_different_dot_count_using_GreaterThan_OvalOperation()
        {
            // More fragments in the version: the EvrComparator must return TRUE.
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1.2-2.47.1.3")
                    .To("1:2.17.4.2.149.1-2.47.1.3")
            .IsTrue();
            // Less fragments in the version: the EvrComparator must return FALSE.
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1-2.47.1.3")
                    .To("1:2.17.4.2.149.1.3-2.47.1.3")
            .IsFalse();
            // More fragments in the release: the EvrComparator must return TRUE.
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1-2.47.1.3.02")
                    .To("1:2.17.4.2.149.1-2.47.1.3")
            .IsTrue();
            // Less fragments in the release: the EvrComparator must return FALSE.
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1-2.47.1.3")
                    .To("1:2.17.4.2.149.1-2.47.1.3.6")
            .IsFalse();
        }

        [TestMethod, Owner("jcastro")]
        public void Should_be_possible_to_compare_two_EvrStrings_with_lots_of_dots_using_GreaterThan_OvalOperation()
        {
            // All equal: the EvrComparator must return FALSE.
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.1-2.47.1.3")
                    .To("1:2.17.4.2.149.1-2.47.1.3")
            .IsFalse();
            // A greater fragment in the version: the EvrComparator must return TRUE.
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.151.1-2.47.1.3")
                    .To("1:2.17.4.2.149.1-2.47.1.3")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.11-2.47.1.3")
                    .To("1:2.17.4.2.149.7-2.47.1.3")
            .IsTrue();
            // A lesser fragment in the version: the EvrComparator must return FALSE.
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.91.1-2.47.1.3")
                    .To("1:2.17.4.2.149.1-2.47.1.3")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.2-2.47.1.3")
                    .To("1:2.17.4.2.149.7-2.47.1.3")
            .IsFalse();
            // A greater fragment in the release: the EvrComparator must return TRUE.
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.151.1-2.47.1.3")
                    .To("1:2.17.4.2.151.1-2.27.1.3")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("1:2.17.4.2.149.7-2.47.1.3")
                    .To("1:2.17.4.2.149.7-2.47.1.1")
            .IsTrue();
        }

        [TestMethod, Owner("jcastro")]
        public void Should_be_possible_to_compare_two_EvrStrings_with_mixed_alpha_and_num_using_GreaterThan_OvalOperation()
        {
            // All equal: the EvrComparator must return FALSE.
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2011foo")
                    .To("1:2.7.21-2011foo")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("3.3a-1.9283")
                    .To("3.3a-1.9283")
            .IsFalse();

            // A different in the first subfragment, first string is greater: the EvrComparator must return TRUE.
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2011foo")
                    .To("1:2.7.21-2010foo")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2.el5")
                    .To("1:2.7.21-2.arkanoid")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("3.4a-1.9283")
                    .To("3.3d-1.9283")
            .IsTrue();

            // A different in the first subfragment, first string is less: the EvrComparator must return FALSE.
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2009foo")
                    .To("1:2.7.21-2010foo")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2.el5")
                    .To("1:2.7.21-2.xevious")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("3.1a-1.9283")
                    .To("3.3d-1.9283")
            .IsFalse();

            // Differences in the second or third subfragment
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2010foo2")
                    .To("1:2.7.21-2010foo")
            .IsTrue();
            MakeSureThatThe
                .ComparingEvrString("1:2.7.21-2.el4")
                    .To("1:2.7.21-2.el5")
            .IsFalse();
            MakeSureThatThe
                .ComparingEvrString("3.3f-1.9283")
                    .To("3.3d-1.9283")
            .IsTrue();
        }
    }
}
