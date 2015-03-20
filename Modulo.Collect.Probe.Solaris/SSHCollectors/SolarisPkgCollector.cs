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
using Modulo.Collect.Probe.Common.Extensions;


namespace Modulo.Collect.Probe.Unix.SSHCollectors
{
    public class SolarisPkgInfo
    {
        public string PkgInst { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Version { get; set; }
        public string Vendor { get; set; }
        public string Description { get; set; }

        public string NameOrDesc
        {
            get
            {
                if (String.IsNullOrEmpty(this.Name))
                    return this.Description;
                else
                    return this.Name;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}{2}{3}{4}", this.PkgInst, this.Version,
                String.IsNullOrEmpty(this.NameOrDesc) ? "" : " (" + this.NameOrDesc + ")",
                String.IsNullOrEmpty(this.Category) ? "" : "; Category " + this.Category,
                String.IsNullOrEmpty(this.Vendor) ? "" : "; By " + this.Vendor
                );
        }
    }

    public class SolarisPkgCollector
    {
        public SshCommandLineRunner CommandRunner { get; set; }

        public Dictionary<string, string> CollectAllPackages(string pkg)
        {
            var retList = new Dictionary<string, string>();
            var cmdText = String.Format("pkginfo -l '{0}'", pkg);
            var cmdOutput = CommandRunner.ExecuteCommand(cmdText).SplitStringByDefaultNewLine();
            foreach (var line in cmdOutput)
            {
                var comps = line.Split(new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (comps.Length == 2)
                {
                    if (comps[0].EndsWith(":"))
                        comps[0] = comps[0].Remove(comps[0].Length - 1);

                    retList[comps[0]] = comps[1];
                }
            }

            return retList;
        }

        public SolarisPkgInfo CollectPackageInfo(string pkg)
        {
            string tmpVal;
            Dictionary<string, string> allProps = CollectAllPackages(pkg);
            SolarisPkgInfo retVal = new SolarisPkgInfo() { PkgInst = pkg };
            allProps.TryGetValue("NAME", out tmpVal);
            retVal.Name = tmpVal;
            allProps.TryGetValue("CATEGORY", out tmpVal);
            retVal.Category = tmpVal;
            allProps.TryGetValue("VERSION", out tmpVal);
            retVal.Version = tmpVal;
            allProps.TryGetValue("VENDOR", out tmpVal);
            retVal.Vendor = tmpVal;
            allProps.TryGetValue("DESC", out tmpVal);
            retVal.Description = tmpVal;
            return retVal;
        }
    }
}
