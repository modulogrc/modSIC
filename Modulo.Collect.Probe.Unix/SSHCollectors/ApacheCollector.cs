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
    public class ApacheInfo
    {
        public string Path { get; set; }
        public string BinaryName { get; set; }
        public string Version { get; set; }

        public string FullBinaryPath
        {
            get
            {
                if (this.Path.EndsWith("/"))
                    return this.Path + this.BinaryName;
                else
                    return this.Path + "/" + this.BinaryName;
            }
        }

        public override string ToString()
        {
            return this.FullBinaryPath + " Version: " + (String.IsNullOrEmpty(this.Version) ? "unknown" : this.Version);
        }
    }

    public class ApacheCollector
    {
        public SshExec SSHExec { get; set; }

        public List<ApacheInfo> getApacheInfo()
        {
            List<ApacheInfo> retList = new List<ApacheInfo>();
            List<string> pathList = Util.getPath(SSHExec);
            Util.AddIfUnique<string>(pathList, "/sbin");
            Util.AddIfUnique<string>(pathList, "/usr/sbin");
            Util.AddIfUnique<string>(pathList, "/usr/local/sbin");
            Util.AddIfUnique<string>(pathList, "/usr/local/apache/bin");
            Util.AddIfUnique<string>(pathList, "/usr/local/apache/sbin");
            Util.AddIfUnique<string>(pathList, "/usr/local/apache2/bin");
            Util.AddIfUnique<string>(pathList, "/usr/local/apache2/sbin");
            Util.AddIfUnique<string>(pathList, "/usr/local/httpd/bin");
            Util.AddIfUnique<string>(pathList, "/usr/local/httpd/sbin");

            foreach (string pathComp in pathList)
            {
                ApacheInfo thisInfo = new ApacheInfo { Path = pathComp, BinaryName = "httpd" };
                string cmdOutput = SSHExec.RunCommand(thisInfo.FullBinaryPath + " -v || echo ERROR");
                string[] lines = cmdOutput.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if ((lines.Length >= 2) && (lines[lines.Length - 1] != "ERROR"))
                {
                    string[] versComps = lines[0].Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (versComps.Length == 2)
                    {
                        thisInfo.Version = versComps[1].Trim();
                        retList.Add(thisInfo);
                    }
                }
            }

            return retList;
        }
    }
}
