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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows.Test
{
    [TestClass()]
    public class RegistryRegexHelperTest
    {
        private RegexHelper CreateRegexHelperForWindows(string path)
        {
            return new RegexHelper(path, false);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_the_current_fix_part_to_a_given_inner_key_name_pattern()
        {
            var regexHelper = CreateRegexHelperForWindows(@"\Software\Microsoft\Direct.*\Compability");
            Assert.AreEqual(@"\Software\Microsoft", regexHelper.FixedPathNamePart);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_the_current_fix_part_to_a_given_multiple_inner_key_name_pattern()
        {
            var regexHelper = CreateRegexHelperForWindows(@"\Software\Micros.*\Direct.*\Compability");
            Assert.AreEqual(@"\Software", regexHelper.FixedPathNamePart);
        }

        [TestMethod, Owner("lfernandes")]
        public void When_pattern_is_first_level_the_current_fixed_key_part_must_be_empty()
        {
            var regexHelper = CreateRegexHelperForWindows(@"\Soft.*\Micros.*\Direct.*\Compability");
            Assert.AreEqual(@"", regexHelper.FixedPathNamePart);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_the_current_fix_part_to_a_given_last_level_key_pattern_name()
        {
            var regexHelper = CreateRegexHelperForWindows(@"\Software\Microsoft\Direct3D\C.*");
            Assert.AreEqual(@"\Software\Microsoft\Direct3D", regexHelper.FixedPathNamePart);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_extract_keys_from_pattern_in_last_level()
        {
            #region Arrange
            List<String> notExpectedKeyNames =  new List<String>() 
            {  
                @"\Software\Microsoft\yyyyDirectX",  
                @"\Software\Microsoft\Visual Studio" 
            };
            List<String> expectedKeyNames =  new List<String>() 
            { 
                @"\Software\Microsoft\DirectX",  
                @"\Software\Microsoft\Direct3D", 
                @"\Software\Microsoft\DirectPlay" 
            };

            List<String> fakeKeyNames = new List<String>();
            fakeKeyNames.Add(notExpectedKeyNames[0]);
            fakeKeyNames.AddRange(expectedKeyNames);
            fakeKeyNames.Add(notExpectedKeyNames[1]);
            #endregion

            // Act
            
            var regexHelper = CreateRegexHelperForWindows(@"\Software\Microsoft\^Direct.*");
            var foundKeyNames = regexHelper.GetMatchPathNamesFromCurrentPathPattern(fakeKeyNames);
            
            // Assert
            this.AssertRegexSearch(foundKeyNames, expectedKeyNames, notExpectedKeyNames);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_extract_keys_from_keyname_with_pattern_in_all_levels()
        {
            #region Arrange
            List<String> notExpectedKeyNames = new List<String>() 
            {  
                @"\Components\Microsoft\DirectFoo",
                @"\Software\Microboard\Direct3D",
                @"\Software\Microsoft\BuildDirect",
                @"\Software\xxxMicros\DirectX",
                @"\Hardware\Microboard\DirectX",
                @"\Hardware\Microsoft\BlaDirect"
            };
            List<String> expectedKeyNames = new List<String>() 
            { 
                @"\Software\Microsoft\Direct3D",
                @"\Software\Microsoft\DirectPlay",
                @"\Hardware\Microsoft\DirectX",
                @"\Hardware\Microsoft\DirectPlay",            
                @"\Software\Microselva\Direct3D",
                @"\Hardware\Microselva\DirectPlay"
            };

            List<String> fakeKeyNames = new List<String>();
            for (int i = 0; i < expectedKeyNames.Count; i++)
			{
			    fakeKeyNames.Add(expectedKeyNames[i]);
                fakeKeyNames.Add(notExpectedKeyNames[i]);
            }
            #endregion

            // Act
            var regexHelper = CreateRegexHelperForWindows(@"\.*ware$\^Micros.*\^Direct.*");
            var foundKeyNames = regexHelper.GetMatchPathNamesFromCurrentPathPattern(fakeKeyNames);

            // Assert
            this.AssertRegexSearch(foundKeyNames, expectedKeyNames, notExpectedKeyNames);
        }

        [TestMethod, Owner("lfernandes")]
        public void should_be_possible_to_extract_keys_from_keyname_with_pattern_at_ends()
        {
            #region Arrange
            List<String> notExpectedKeyNames = new List<String>() 
            {  
                @"\Components\Microsoft\DirectFoo",
                @"\Software\Microboard\Direct3D",
                @"\Software\Microsoft\BuildDirect",
                @"\Software\xxxMicros\DirectX",
                @"\Hardware\Microboard\DirectX",
                @"\Hardware\Microsoft\BlaDirect",
                @"\Software\Microselva\Direct3D",
                @"\Hardware\Microselva\DirectPlay",
                @"\HardwareTemp\Microsoft\DirectPlay"
            };
            List<String> expectedKeyNames = new List<String>() 
            { 
                @"\Software\Microsoft\Direct3D",
                @"\Software\Microsoft\DirectPlay",
                @"\Hardware\Microsoft\DirectX",
                @"\Hardware\Microsoft\DirectPlay",
            };

            List<String> fakeKeyNames = new List<String>();
            for (int i = 0; i < expectedKeyNames.Count; i++)
            {
                fakeKeyNames.Add(notExpectedKeyNames[i]); 
                fakeKeyNames.Add(expectedKeyNames[i]);
            }
            for (int i = expectedKeyNames.Count; i < notExpectedKeyNames.Count; i++)
                fakeKeyNames.Add(notExpectedKeyNames[i]);
            
            Assert.AreEqual(13, fakeKeyNames.Count);
            #endregion

            // Act
            var regexHelper = CreateRegexHelperForWindows(@"\.*ware$\Microsoft\^Direct.*");
            var foundKeyNames = regexHelper.GetMatchPathNamesFromCurrentPathPattern(fakeKeyNames);

            // Assert
            this.AssertRegexSearch(foundKeyNames, expectedKeyNames, notExpectedKeyNames);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_extract_keys_from_keyname_with_pattern_at_mixed_levels()
        {
            #region Arrange
            List<String> notExpectedKeyNames = new List<String>() 
            {  
                @"\Components\Microsoft\DirectFoo\A\A\D",
                @"\Software\Microsoft\Graphics\DirectX\XPTO\Build",
                @"\Software\Microsoft\Graphics\DirectX\xDirecty\Build",
                @"\Software\Microsoft\Graphic\DirectX\DirectY\Build",
                @"\Software\Microsoft\Graphics\DirectX\DirectY\Builde",
                @"\Software\Microsoft\Graphics\DirectZ\DirectY\Build",
                @"\Hardware\Maicrosofete\Graphics\DirectX\DirectW\Build"
            };
            List<String> expectedKeyNames = new List<String>() 
            { 
                @"\Software\Microsoft\Graphics\DirectX\DirectZ\Build",
                @"\Software\Microsoft\Graphics\DirectX\Direct123\Build",
                @"\Hardware\Microsoft\Graphics\DirectX\DirectW\Build"
            };

            List<String> fakeKeyNames = new List<String>();
            fakeKeyNames.AddRange(notExpectedKeyNames);
            fakeKeyNames.AddRange(expectedKeyNames);
            #endregion

            // Act
            var regexHelper = CreateRegexHelperForWindows(@"\.*ware$\Microsoft\Graphics\DirectX\^Direct.*\Build");
            IList<String> foundKeyNames = regexHelper.GetMatchPathNamesFromCurrentPathPattern(fakeKeyNames);

            // Assert
            this.AssertRegexSearch(foundKeyNames, expectedKeyNames, notExpectedKeyNames);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_fixed_part_from_a_string_with_regular_expression_pattern()
        {
            Assert.AreEqual(@"c:\temp\", RegexHelper.GetFixedPartFromPathWithPattern(@"c:\temp\.*\usb\devices", false));
            Assert.AreEqual("/temp/", RegexHelper.GetFixedPartFromPathWithPattern("/temp/.*/usb/devices", true));
            Assert.AreEqual("/etc/cron.d/", RegexHelper.GetFixedPartFromPathWithPattern("/etc/cron.d/.*", true));
        }

        private void AssertRegexSearch(IList<String> foundKeys, IList<String> expectedKeys, IList<String> notExpectedKeys)
        {
            int allKeysCount = expectedKeys.Count + notExpectedKeys.Count;
            Assert.AreEqual(expectedKeys.Count, foundKeys.Count, "The number of keys found does not match expected by the test.");
            Assert.AreEqual(notExpectedKeys.Count, allKeysCount - foundKeys.Count);
            foreach (var notExpectedKey in notExpectedKeys)
                Assert.IsFalse(foundKeys.Contains(notExpectedKey), string.Format("The unexpected Key Name '{0}' was found.", notExpectedKey));
        }
    }
}
