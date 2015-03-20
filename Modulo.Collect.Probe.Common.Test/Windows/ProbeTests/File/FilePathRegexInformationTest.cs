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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Independent.Common;
using System.Collections.Generic;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Independent.Common.Operators;


namespace Modulo.Collect.Probe.Windows.Test.File
{
    [TestClass]
    public class FilePathRegexInformationTest
    {
        [TestMethod, Owner("lcosta, lfernandes")]
        public void Should_be_possible_to_get_patternMatch_levels()
        {
            {
                var fileLevelsWithRegexWin = new FilePathRegexInformation(@"c:\.*").GetPathLevelsWithRegex();
                AssertFilePathRegexInformation(fileLevelsWithRegexWin, 1, new string[] { ".*" }, new int[] { 1 }); 
            }

            {
                var fileLevelsWithRegexUnix = new FilePathRegexInformation("/.*").GetPathLevelsWithRegex();
                AssertFilePathRegexInformation(fileLevelsWithRegexUnix, 1, new string[] { ".*" }, new int[] { 0 }); 
            }
        }

        private void AssertFilePathRegexInformation(
            IEnumerable<PathLevelWithRegex> itemsToAssert, int expectedRegexCount, string[] expectedPattern, int[] expectedLevels)
        {
            Assert.AreEqual(expectedRegexCount, itemsToAssert.Count());
            for (int i = 0; i < itemsToAssert.Count(); i++)
            {
                Assert.AreEqual(expectedPattern.ElementAt(i), itemsToAssert.ElementAt(i).Pattern, "the pattern is not expected");
                Assert.AreEqual(expectedLevels.ElementAt(i), itemsToAssert.ElementAt(i).Level, "the level is not expected");
            }
        }

        [TestMethod, Owner("lcosta, lfernandes")]
        public void Should_be_possible_to_get_patternMatch_levels_in_multiples_levels()
        {
            //  Windows
            {
                var fileLevelsWithRegex = new FilePathRegexInformation(@"c:\.*\oval.*").GetPathLevelsWithRegex();
                AssertFilePathRegexInformation(fileLevelsWithRegex, 2, new string[] { ".*", "oval.*" }, new int[] { 1, 2 } );
            }
            {
                var fileLevelsWithRegex = new FilePathRegexInformation(@"c:\temp\oval.*\support\^[win]").GetPathLevelsWithRegex();
                AssertFilePathRegexInformation(fileLevelsWithRegex, 2, new string[] { "oval.*", "^[win]" }, new int[] { 2, 4 });
                
            }

            //  Unix
            {
                var fileLevelsWithRegex = new FilePathRegexInformation("/.*/oval.*").GetPathLevelsWithRegex();
                AssertFilePathRegexInformation(fileLevelsWithRegex, 2, new string[] { ".*", "oval.*" }, new int[] { 0, 1 });
            }
            {
                var fileLevelsWithRegex = new FilePathRegexInformation("/temp/oval.*/support/^[win]").GetPathLevelsWithRegex();
                AssertFilePathRegexInformation(fileLevelsWithRegex, 2, new string[] { "oval.*", "^[win]" }, new int[] { 1, 3 });

            }
        }

        [TestMethod, Owner("lcosta, lfernandes")]
        public void Should_return_the_empty_list_when_there_is_not_a_regex_pattern_in_the_path()
        {
            var fileLevelsWithRegexWin = new FilePathRegexInformation(@"c:\windows\temp").GetPathLevelsWithRegex();
            var fileLevelsWithRegexUnix = new FilePathRegexInformation("/lib/modules").GetPathLevelsWithRegex();

            Assert.AreEqual(0, fileLevelsWithRegexWin.Count());
            Assert.AreEqual(0, fileLevelsWithRegexUnix.Count());
        }

        [TestMethod, Owner("lcosta, lfernandes")]
        public void Should_be_possible_to_get_the_unit_of_a_path()
        {
            var pathUnitWin = new FilePathRegexInformation(@"c:\windows\temp").GetUnitOfPath();
            var pathUnitUnix = new FilePathRegexInformation("/lib/modules").GetUnitOfPath();

            Assert.AreEqual(@"c:\", pathUnitWin, "the path unit is not expected");
            Assert.IsNull(pathUnitUnix, "the path unit for unix system should always be null.");
        }

        [TestMethod, Owner("lcosta, lfernandes")]
        public void Should_be_possible_to_get_the_fixed_path_with_first_regex_pattern()
        {
            {
                var pathWithFirstRegexWin = new FilePathRegexInformation(@"c:\.*\oval.*").GetPathWithFirstRegex();
                var pathWithFirstRegexUnix = new FilePathRegexInformation("/.*/oval.*").GetPathWithFirstRegex();
                AssertPathWithPattern(pathWithFirstRegexWin, @"c:\.*", 1);
                AssertPathWithPattern(pathWithFirstRegexUnix, "/.*", 0);
            }

            {
                var pathWithFirstRegexWin = new FilePathRegexInformation(@"c:\temp\oval.*").GetPathWithFirstRegex();
                var pathWithFirstRegexUnix = new FilePathRegexInformation("/temp/oval.*").GetPathWithFirstRegex();
                AssertPathWithPattern(pathWithFirstRegexWin, @"c:\temp\oval.*", 2);
                AssertPathWithPattern(pathWithFirstRegexUnix, "/temp/oval.*", 1);
            }

            {
                var pathWithFirstRegexWin = new FilePathRegexInformation(@"c:\temp\oval.*\xml").GetPathWithFirstRegex();
                var pathWithFirstRegexUnix = new FilePathRegexInformation("/temp/oval.*/xml").GetPathWithFirstRegex();
                AssertPathWithPattern(pathWithFirstRegexWin, @"c:\temp\oval.*", 2);
                AssertPathWithPattern(pathWithFirstRegexUnix, "/temp/oval.*", 1);
            }
        }

        private void AssertPathWithPattern(PathLevelWithRegex itemToAssert, string expectedPathWithPattern, int expectedLevel)
        {
            Assert.AreEqual(expectedPathWithPattern, itemToAssert.PathWithPattern, "the path is not expected");
            Assert.AreEqual(expectedLevel, itemToAssert.Level, "the path level is not expected");
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_concatenate_the_windows_path_with_next_regex_level()
        {
            {
                var pathWithNextRegex = 
                    new FilePathRegexInformation(@"c:\.*\oval.*")
                        .ConcatPathWithNextLeveRegex(@"c:\temp\", 1);

                Assert.AreEqual(@"c:\temp\oval.*", pathWithNextRegex, "the path is not expected");
            }

            {
                var pathWithNextRegex =
                    new FilePathRegexInformation(@"c:\temp\file\.*\teste\^.txt")
                        .ConcatPathWithNextLeveRegex(@"c:\temp\file\temp\teste\", 3);
                
                Assert.AreEqual(@"c:\temp\file\temp\teste\^.txt", pathWithNextRegex, "the path is not expected");
            }
        }

        [TestMethod, Owner("lcosta")]
        public void Should_be_possible_to_concatenate_the_unix_path_with_next_regex_level()
        {
            {
                var pathWithNextRegex =
                    new FilePathRegexInformation("/.*/oval.*")
                        .ConcatPathWithNextLeveRegex("/temp/", 0);

                Assert.AreEqual("/temp/oval.*", pathWithNextRegex, "the path is not expected");
            }

            {
                var pathWithNextRegex =
                    new FilePathRegexInformation("/temp/file/.*/teste/^.txt")
                        .ConcatPathWithNextLeveRegex("/temp/file/temp/teste/", 2);

                Assert.AreEqual("/temp/file/temp/teste/^.txt", pathWithNextRegex, "the path is not expected");
            }
        }
    }
}
