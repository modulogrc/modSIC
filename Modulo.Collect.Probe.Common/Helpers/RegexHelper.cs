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
using System.Text.RegularExpressions;


namespace Modulo.Collect.Probe.Common
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
        private Boolean isUnix;
        private String fixedPathPart;
        private String fullPathNamePattern;
        private Dictionary<int, LevelPathName> PatternPathNamesByLevel; 
        
        
        public String FixedPathNamePart { get { return this.fixedPathPart; } }


        public RegexHelper() { }

        public RegexHelper(String pathNamePattern, bool isUnix)
        {
            this.isUnix = isUnix;
            this.initializeInternalFields(pathNamePattern);
            this.sortPathByLevel();
        }

        public static Boolean IsPathLevelARegexPattern(string pathNameLevel)
        {
            pathNameLevel = 
                pathNameLevel
                    .Replace(' ', '_')
                    .Replace(".", string.Empty)
                    .Replace(@"{", string.Empty)
                    .Replace(@"}", string.Empty);

            string escapedKeyLevelName = Regex.Escape(pathNameLevel);

            return (escapedKeyLevelName != pathNameLevel);
        }

        private static string[] GetSeparator(string completePath, bool isUnix)
        {
            var separator = isUnix ? "/" : "\\";
            return new string[] { separator };
        }

        private static List<String> SplitPath(string completePath, bool isUnix) 
        {
            return 
                completePath
                    .Split(GetSeparator(completePath, isUnix), StringSplitOptions.RemoveEmptyEntries)
                    .ToList<String>();
        }

        public static string GetFixedPartFromPathWithPattern(string path, bool isUnixPath)
        {
            var fixedPart = string.Empty;
            var splittedPath = SplitPath(path, isUnixPath);
            var separator = GetSeparator(path, isUnixPath).First();
            foreach (var pathLevel in splittedPath)
                if (!RegexHelper.IsPathLevelARegexPattern(pathLevel))
                    fixedPart += pathLevel + separator;
                else
                    break;
            
            if (string.IsNullOrEmpty(fixedPart))
                return path;

            if (isUnixPath)
                fixedPart = separator + fixedPart;

            return fixedPart;
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
            {
                if (isFullPathNameMatchForeachLevel(keyName))
                    foundKeyNames.Add(keyName);                
            }
            
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
            if (this.PatternPathNamesByLevel.Count() == 1)
                level = 1;

            LevelPathName currentKeyNamePattern = this.PatternPathNamesByLevel[level];
            
            bool isLevelKeyNameMatch = currentKeyNamePattern.Path.Equals(currentPathNameToMatch,StringComparison.CurrentCultureIgnoreCase);
            if (currentKeyNamePattern.IsARegexPattern)
            {
                String escapedKeyNameToMatch = Regex.Escape(currentPathNameToMatch);
                isLevelKeyNameMatch = Regex.IsMatch(currentPathNameToMatch, currentKeyNamePattern.Path, RegexOptions.IgnoreCase);
            }
            
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

        private string[] GetKeyLevelSeparator(string fullPathname)
        {
            var separator = this.isUnix ? "/" : @"\";
            return new string[] { separator };
        }

        private IList<String> splitFullPathNameByLevel(string fullPathName)
        {
            var keyLevelSeparator = GetKeyLevelSeparator(fullPathName);
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

                var separator = GetSeparator(this.fullPathNamePattern, this.isUnix).First();
                this.fixedPathPart += string.Format(@"{0}{1}",  separator, currentKeyLevel.Path);
            }
        }
    }
}
