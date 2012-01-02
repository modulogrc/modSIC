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
    public class SolarisSmfInfo
    {
        public string frmi { get; set; }
        public string ServiceName { get; set; }
        public string ServiceState { get; set; }
        public string Protocol { get; set; }
        public string ServerExecutable { get; set; }
        public string ServerArgs { get; set; }
        public string ExecAsUser { get; set; }

        public string CommandLine
        {
            get
            {
                if (String.IsNullOrEmpty(this.ServerExecutable) || String.IsNullOrEmpty(this.ServerArgs))
                    return this.ServerExecutable;
                else
                    return this.ServerExecutable + ' ' + this.ServerArgs;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}{2}{3}{4}", this.ServiceName,
                                    this.ServiceState,
                                    String.IsNullOrEmpty(this.Protocol) ? "" : ", Proto " + this.Protocol,
                                    String.IsNullOrEmpty(this.CommandLine) ? "" : ", Cmd {" + this.CommandLine + "}",
                                    String.IsNullOrEmpty(this.ExecAsUser) ? "" : ", Run as " + this.ExecAsUser
                                    );
        }
    }

    public class SolarisSvcPropInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return String.Format("{0} = [{1}] (type: {2})", this.Name, this.Value, this.Type);
        }
    }

    public class SolarisSmfCollector
    {
        public SshExec SSHExec { get; set; }

        public static SolarisSvcPropInfo parsePropInfo(string line)
        {
            SolarisSvcPropInfo retInfo = null;
            char[] elemseps = { ' ', '\t' };
            string[] ffields = line.Split(elemseps, 3, StringSplitOptions.RemoveEmptyEntries);
            if (ffields.Length == 3)
            {
                retInfo = new SolarisSvcPropInfo();
                retInfo.Name = ffields[0];
                retInfo.Type = ffields[1];
                retInfo.Value = ffields[2];
            }
            return retInfo;
        }

        public Dictionary<string, SolarisSvcPropInfo> getAllProps(string frmi)
        {
            Dictionary<string, SolarisSvcPropInfo> retList = new Dictionary<string, SolarisSvcPropInfo>();
            string cmdOutput = SSHExec.RunCommand("svcprop '" + frmi + "'");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                SolarisSvcPropInfo thisInfo = parsePropInfo(line);
                if (thisInfo != null)
                    retList[thisInfo.Name] = thisInfo;
            }

            return retList;
        }

        public virtual SolarisSmfInfo GetSmfInfo(string frmi)
        {
            SolarisSmfInfo retVal = null;
            string[] svcComps = frmi.Split(new char[] { ':' }, 3);
            if (svcComps.Length < 2)
                return null;
            string[] nameComps = svcComps[1].Split(new char[] { '/' });
            if (nameComps.Length < 1)
                return null;
            retVal = new SolarisSmfInfo();
            retVal.frmi = frmi;
            retVal.ServiceName = nameComps[nameComps.Length - 1];

            SolarisSvcPropInfo thisProp = null;
            Dictionary<string, SolarisSvcPropInfo> props = getAllProps(frmi);

            if (props.TryGetValue("restarter/state", out thisProp))
                retVal.ServiceState = thisProp.Value.ToUpper();

            if (props.TryGetValue("inetd/proto", out thisProp))
                retVal.Protocol = thisProp.Value;

            if (props.TryGetValue("inetd_start/exec", out thisProp) || props.TryGetValue("start/exec", out thisProp))
            {
                int slashSpacePos = thisProp.Value.IndexOf("\\ ");
                if (slashSpacePos < 0)
                    retVal.ServerExecutable = thisProp.Value;
                else
                {
                    retVal.ServerExecutable = thisProp.Value.Substring(0, slashSpacePos);
                    retVal.ServerArgs = thisProp.Value.Substring(slashSpacePos + 2);
                }
            }

            if (props.TryGetValue("inetd_start/user", out thisProp))
                retVal.ExecAsUser = thisProp.Value;

            return retVal;
        }
    }
}
