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
using System.Text.RegularExpressions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;

namespace Modulo.Collect.OVAL.Definitions.Helpers
{
    public struct LevelPathName
    {
        private string path;
        private bool isARegexPattern;

        public LevelPathName(string path)
        {
            this.path = path;
            this.isARegexPattern = RegexHelper.IsPathLevelARegexPattern(path);
        }

        public String Path { get { return this.path; } }
        public bool IsARegexPattern { get { return this.isARegexPattern; } }
    }


    public class RegexHelper
    {
        private String fixedPathPart;
        private String fullPathNamePattern;
        private Dictionary<int, LevelPathName> PatternPathNamesByLevel;
        private IOvalComparator comparator = new StringComparator();
        
        public String FixedPathNamePart { get { return this.fixedPathPart; } }


        public RegexHelper() { }

        public RegexHelper(String pathNamePattern)
        {
            this.initializeInternalFields(pathNamePattern);
            this.sortPathByLevel();
        }

        public static Boolean IsPathLevelARegexPattern(string pathNameLevel)
        {
            pathNameLevel = pathNameLevel.Replace(' ', '_');
            string escapedKeyLevelName = Regex.Escape(pathNameLevel);

            return (escapedKeyLevelName != pathNameLevel);
        }





        public IList<String> MatchPathNamesToPattern(IList<String> pathNamesToMatch, string pattern)
        {
            IList<String> foundKeyNames = new List<String>();
            foreach (var keyName in pathNamesToMatch)
                if (this.isFullPathNameMatchForeachLevel(keyName))
                    foundKeyNames.Add(keyName);

            return foundKeyNames;
        }

        public IList<String> GetMatchPathNamesFromCurrentPathPattern(IList<String> pathNamesToMatch)
        {
            IList<String> foundKeyNames = new List<String>();
            foreach (var keyName in pathNamesToMatch)
                if (isFullPathNameMatchForeachLevel(keyName))
                    foundKeyNames.Add(keyName);
            
            return foundKeyNames;
        }

        private bool isFullPathNameMatchForeachLevel(string fullPathNameToMatch)
        {
            IList<String> fullKeyNameToMatchByLevels = this.splitFullPathNameByLevel(fullPathNameToMatch);
            
            bool result = true;
            int level = 1;
            while (level <= fullKeyNameToMatchByLevels.Count)
            {
                String currentKeyNameToMatch = fullKeyNameToMatchByLevels[level - 1];
                result = result && this.isLevelPathNameMatch(currentKeyNameToMatch, level);
                
                level++;
                if (!result)
                    break;
            }
            return result;
        }

        private bool isLevelPathNameMatch(String currentPathNameToMatch, int level)
        {
            LevelPathName currentKeyNamePattern = this.PatternPathNamesByLevel[level];
            
            bool isLevelKeyNameMatch = currentKeyNamePattern.Path.Equals(currentPathNameToMatch);
            if (currentKeyNamePattern.IsARegexPattern)
                isLevelKeyNameMatch = comparator.Compare(currentPathNameToMatch, currentKeyNamePattern.Path, OperationEnumeration.patternmatch);
            
            return isLevelKeyNameMatch;
        }

        private void initializeInternalFields(String pathNamePattern)
        {
            this.fixedPathPart = string.Empty;
            this.fullPathNamePattern = pathNamePattern;
            this.PatternPathNamesByLevel = new Dictionary<int, LevelPathName>();
        }

        private void sortPathByLevel()
        {
            IList<String> allKeyLevels = this.splitFullPathNameByLevel(this.fullPathNamePattern);
            
            for (int level = 1; level <= allKeyLevels.Count; level++)
            {
                LevelPathName newKeyNameLevel = new LevelPathName(allKeyLevels[level-1]);
                this.PatternPathNamesByLevel.Add(level, newKeyNameLevel);
            }

            this.updateFixedPathPart();
        }

        private IList<String> splitFullPathNameByLevel(string fullPathName)
        {
            string[] keyLevelSeparator = new string[] { "\\" };
            return fullPathName.Split(keyLevelSeparator, StringSplitOptions.RemoveEmptyEntries).ToList<String>();
        }

        private void updateFixedPathPart()
        {
            this.fixedPathPart = string.Empty;
            
            foreach (var keyLevel in this.PatternPathNamesByLevel)
            {
                LevelPathName currentKeyLevel = keyLevel.Value;
                if (currentKeyLevel.IsARegexPattern)
                    break;

                this.fixedPathPart += string.Format(@"\{0}", currentKeyLevel.Path);
            }
        }
    }
}
