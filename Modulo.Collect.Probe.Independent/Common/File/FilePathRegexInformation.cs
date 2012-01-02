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
using Modulo.Collect.Probe.Common;
using System.Text.RegularExpressions;
using Modulo.Collect.Probe.Independent.Common.Operators;



namespace Modulo.Collect.Probe.Independent.Common.File
{
    public class FilePathRegexInformation
    {
        //private FamilyEnumeration Platform;
        private string originalPath;
        private string levelSeparator;
        private List<string> pathLevels;

        public FilePathRegexInformation(string path)
        {
            this.originalPath = path;
            this.levelSeparator = IsWindowsFilepath() ? @"\" : "/";
            this.SeparatePathInLevels();
        }

        public bool IsWindowsFilepath()
        {
            return this.originalPath.ElementAt(1).Equals(':');
        }

        /// <summary>
        /// This method return the levels of path that has regex defined.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PathLevelWithRegex> GetPathLevelsWithRegex()
        {
            List<PathLevelWithRegex> levelsWithRegex = new List<PathLevelWithRegex>();
            if ((this.pathLevels.Count() == 1) && (this.levelSeparator.Equals(@"\")))
                return levelsWithRegex;

            var startLevel = this.levelSeparator.Equals(@"\") ? 1 : 0;
            for (int i = startLevel; i <= this.pathLevels.Count() - 1; i++)
            {
                string levelPath = this.pathLevels[i];
                if (RegexHelper.IsPathLevelARegexPattern(levelPath))
                {
                    levelsWithRegex.Add(new PathLevelWithRegex(i, levelPath, ""));
                }

            }

            return levelsWithRegex;
        }
        /// <summary>
        /// This method returns the logical unit of path. 
        /// Example: 
        ///     For the path = c:\windows\system
        ///     the logical unit is c:\
        /// </summary>
        /// <returns></returns>
        public string GetUnitOfPath()
        {
            if (this.levelSeparator.Equals("/"))
                return null;

            return String.Format("{0}{1}", this.pathLevels.First(), this.levelSeparator);
        }

        public PathLevelWithRegex GetPathWithFirstRegex()
        {
            PathLevelWithRegex pathLevelWithRegex = null;
            string fixedPart = "";

            for (int i = 0; i <= this.pathLevels.Count() - 1; i++)
            {
                string pathLevel = this.pathLevels[i];


                if (!RegexHelper.IsPathLevelARegexPattern(pathLevel.Replace(".", "")))
                    fixedPart += String.Format("{0}{1}", pathLevel, this.levelSeparator);
                else
                {
                    fixedPart += pathLevel;
                    if (this.levelSeparator.Equals("/"))
                        fixedPart = "/" + fixedPart;

                    return new PathLevelWithRegex(i, pathLevel, fixedPart);
                }
            }
            return pathLevelWithRegex;
        }

       

        private void SeparatePathInLevels()
        {            
            pathLevels = 
                this.originalPath
                    .Split(
                        new string[] { this.levelSeparator }, 
                        StringSplitOptions.RemoveEmptyEntries)
                    .ToList<String>();
        }

        private bool ExistsEscapeCharacterInOriginalPath()
        {
            return this.originalPath.Contains("\\.") || this.originalPath.Contains("\\$") || this.originalPath.Contains("\\^") ||
                   this.originalPath.Contains("\\?") || this.originalPath.Contains("\\*") || this.originalPath.Contains("\\[") ||
                   this.originalPath.Contains("\\]") || this.originalPath.Contains("\\|");
        }

      
        public string ConcatPathWithNextLeveRegex(string path, int currentLevel)
        {
            string newPath = "";
            List<PathLevelWithRegex> regexLevels = this.GetPathLevelsWithRegex().OrderBy(levels => levels.Level).ToList();            
            foreach (PathLevelWithRegex pathLevel in regexLevels)
            {
                if (pathLevel.Level > currentLevel)
                {
                    newPath = String.Format("{0}{1}{2}", path, "", pathLevel.Pattern);
                    break;
                }
            }
            return newPath;          
        }
    }
}
