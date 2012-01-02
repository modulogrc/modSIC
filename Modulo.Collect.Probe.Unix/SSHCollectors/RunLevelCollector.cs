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
using Tamir.SharpSsh;

namespace Modulo.Collect.Probe.Unix.SSHCollectors
{
    public class RunLevelInfo
    {
        public string Service { get; set; }
        public string RunLevel { get; set; }
        public bool Start { get; set; }
        public bool Kill { get; set; }

        public override string ToString()
        {
            return String.Format(
                "Service '{0}'; Runlevel {1}{2}{3}", Service, RunLevel, Start ? "; Start" : "", Kill ? "; Kill" : "");
        }
    }

    public class RunLevelCollector
    {
        public SshExec SSHExec { get; set; }

        public virtual List<string> GetTargetServices(SshExec exec)
        {
            string cmdOutput = exec.RunCommand("ls -l /etc/init.d/ 2>/dev/null | grep '^-..x'");
            return UnixTerminalParser.GetServicesFromTerminalOutput(cmdOutput).ToList();
        }

        public virtual Dictionary<string, RunLevelInfo> GetTargetRunLevelInfo(string runLevel)
        {
            Dictionary<string, RunLevelInfo> retList = new Dictionary<string, RunLevelInfo>();
            string cmdOutput = SSHExec.RunCommand(String.Format("ls /etc/rc{0}.d", runLevel));
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                RunLevelInfo thisInfo = parseRunLevelInfo(line, runLevel);
                if (thisInfo != null)
                    retList[thisInfo.Service] = thisInfo;
            }

            return retList;
        }

        private RunLevelInfo parseRunLevelInfo(string line, string runLevel)
        {
            RunLevelInfo retInfo = null;

            if (line.Length > 3)
            {
                char cStartKill = line[0];
                char cDigit1 = line[1];
                char cDigit2 = line[2];
                retInfo = new RunLevelInfo();

                if (char.IsDigit(cDigit1) && char.IsDigit(cDigit2))
                {
                    if (cStartKill == 'S')
                        retInfo.Start = true;
                    if (cStartKill == 'K')
                        retInfo.Kill = true;
                    retInfo.RunLevel = runLevel;
                    retInfo.Service = line.Substring(3);
                }
            }

            return retInfo;
        }

    }
}
