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
using System.Text.RegularExpressions;
using Modulo.Collect.Probe.Common.Extensions;

namespace Modulo.Collect.Probe.Unix.SSHCollectors
{
    public class TextFileContent
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Pattern { get; set; }
        public uint Instance { get; set; }
        public string Line { get; set; }
        public string Text { get; set; }
        public List<string> SubExpressions { get; set; }

        public override string ToString()
        {
            string retVal = String.Format("{0}: {1}\n    Found {2}\n", Instance, Line, Text);
            foreach (string subExpr in SubExpressions)
            {
                retVal += "    Subexpression: " + subExpr;
            }
            return retVal;
        }
    }

    public class FileContentCollector
    {
        public SshCommandLineRunner CommandRunner { get; set; }


        public FileContentCollector(SshCommandLineRunner sshCommandRunner)
        {
            this.CommandRunner = sshCommandRunner;
        }

        public virtual string GetTextFileFullContent(string pathspec)
        {
            var commandText = String.Format("cat {0} 2>&1", pathspec);
            string retVal = CommandRunner.ExecuteCommand(commandText);

            if (CommandRunner.LastCommandExitCode > 0)
            {
                // if (retVal.Contains("Permis"))
                    throw new UnauthorizedAccessException("Access denied to '" + pathspec + "'");
                // else
                //     throw new ApplicationException(retVal);
            }

            return retVal;
        }

        public virtual IEnumerable<String> GetTextFileFullContentInLines(string pathspec)
        {
            var fileLines = GetTextFileFullContent(pathspec).SplitStringByDefaultNewLine();
            if (fileLines == null)
                return new string[] { };

            return fileLines;
        }


        public virtual List<TextFileContent> GetTextFileContent(string pathspec, string pattern)
        {
            var retList = new List<TextFileContent>();
            
            var cookedPattern = pattern.Replace("\\", "\\\\");
            cookedPattern = cookedPattern.Replace("/", "\\/");
            cookedPattern = cookedPattern.Replace("'", "'\"'\"'");
            //var command = "awk '/" + cookedPattern + "/ {print}' <" + pathspec;

            var commandText = "awk '/" + cookedPattern + "/ {print}' <" + pathspec + " 2>&1"; ; // String.Format(@"awk '/{0}/ {print}' <{1}", cookedPattern, pathspec);

            string outputStr = CommandRunner.ExecuteCommand(commandText);
            var output = outputStr.SplitStringByDefaultNewLine();
            if (CommandRunner.LastCommandExitCode > 0)
            {
                // if (outputStr.Contains("Permis"))
                    throw new UnauthorizedAccessException("Access denied to '" + pathspec + "'");
                // else
                //     throw new ApplicationException(outputStr);
            }

            string dir, fname;
            uint instance = 0;
            int whereSlash = pathspec.LastIndexOf('/');

            if (whereSlash > 0)
            {
                dir = pathspec.Substring(0, whereSlash);
                fname = pathspec.Substring(whereSlash + 1);
            }
            else if (whereSlash == 0)
            {
                dir = "/";
                fname = pathspec.Substring(1);
            }
            else
            {
                dir = ".";
                fname = pathspec;
            }

            foreach (var line in output)
            {
                var retPartialList = parseMatches(dir, fname, line, cookedPattern, ref instance);
                retList.AddRange(retPartialList);
            }

            return retList;
        }


        private List<TextFileContent> parseMatches(
            string dir, string fname, string line, string pattern, ref uint instance)
        {
            var retList = new List<TextFileContent>();
            var myRegex = new Regex(pattern);
            var myMatches = myRegex.Matches(line);
            foreach (Match myMatch in myMatches)
            {
                var retItem = new TextFileContent()
                {
                    Line = line,
                    Pattern = pattern,
                    Text = myMatch.ToString(),
                    Path = dir,
                    FileName = fname,
                    Instance = ++instance,
                    SubExpressions = new List<string>()
                };

                foreach (var group in myMatch.Groups)
                    retItem.SubExpressions.Add(group.ToString());

                retList.Add(retItem);
            }

            return retList;
        }
    }
}
