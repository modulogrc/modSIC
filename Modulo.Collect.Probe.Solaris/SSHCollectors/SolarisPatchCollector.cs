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
    public class SolarisPatchInfo
    {
        public ulong Base { get; set; }
        public ulong Version { get; set; }

        public override string ToString()
        {
            return String.Format("{0}-{1:D2}", this.Base, this.Version);
        }
    }

    public class SolarisPatchCollector
    {
        public SshExec SSHExec { get; set; }

        private static SolarisPatchInfo parsePatchInfo(string myLine)
        {
            SolarisPatchInfo retVal = new SolarisPatchInfo();
            bool gotToPatch = false;
            string patchName = "";

            foreach (char c in myLine)
            {
                bool isPatchChar = (((c >= '0') && (c <= '9')) || (c == '-'));

                if (gotToPatch && !isPatchChar)
                    break;

                if (isPatchChar)
                {
                    gotToPatch = true;
                    patchName += c;
                }
            }

            string[] comps = patchName.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            retVal.Base = ulong.Parse(comps[0]);
            if (comps.Length > 1)
                retVal.Version = ulong.Parse(comps[1]);
            else
                retVal.Version = 0;

            return retVal;
        }

        public Dictionary<ulong, SolarisPatchInfo> getPatchInfo()
        {
            Dictionary<ulong, SolarisPatchInfo> retList = new Dictionary<ulong, SolarisPatchInfo>();
            string cmdOutput = SSHExec.RunCommand("showrev -p");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                SolarisPatchInfo thisInfo = parsePatchInfo(line);
                if (thisInfo != null)
                    retList[thisInfo.Base] = thisInfo;
            }

            return retList;
        }
    }
}
