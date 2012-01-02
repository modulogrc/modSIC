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
using System.Collections.Generic;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Common;
using System;
using System.Linq;

namespace Modulo.Collect.Probe.Independent.Common.Operators
{
    public class MultiLevelPatternMatchOperation
    {
        private FamilyEnumeration Platform;

        public MultiLevelPatternMatchOperation(FamilyEnumeration platform)
        {
            this.Platform = platform;
        }

        public IEnumerable<string> applyPatternMatch(string entityValue, IEnumerable<string> valuesToMatch)
        {
            var result = new List<string>();
            if (valuesToMatch != null)
            {
                foreach (var valueToMatch in valuesToMatch)
                {
                    var regexHelper = new RegexHelper(entityValue, this.Platform.Equals(FamilyEnumeration.unix));
                    var matchValues = regexHelper.GetMatchPathNamesFromCurrentPathPattern(new string[] { valueToMatch });
                    var concatenatedValue = this.mergePatternAndMatch(entityValue, matchValues.SingleOrDefault());
                    if (!string.IsNullOrWhiteSpace(concatenatedValue))
                        result.Add(concatenatedValue);
                }
            }

            return result;
        }

        private string mergePatternAndMatch(string pattern, string matchValue)
        {
            if (string.IsNullOrEmpty(matchValue))
                return null;

            var separator = new string[] { Platform.Equals(FamilyEnumeration.windows) ? @"\" : "/" };
            var splittedPattern = pattern.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            var splittedMatchValue = matchValue.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            for (int level = 0; level < splittedPattern.Count(); level++)
                if (RegexHelper.IsPathLevelARegexPattern(splittedPattern[level]))
                    splittedPattern[level] = splittedMatchValue[level];

            return string.Join(separator.First(), splittedPattern);
        }
    }
}
