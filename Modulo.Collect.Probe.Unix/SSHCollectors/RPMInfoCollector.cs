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
using Modulo.Collect.Probe.Common.Extensions;

namespace Modulo.Collect.Probe.Unix.SSHCollectors
{
    public class RPMInfoCollector
    {
        public SshCommandLineRunner CommandRunner { get; set; }

        public virtual LinuxPackageInfo GetTargetRPMByPackageName(string packageName)
        {
            var commandText = "rpm -q --qf '%{NAME}\t%{VERSION}\t%{RELEASE}\t%{ARCH}\t%{EPOCH}\n' " + packageName;
            var commandResultLines = CommandRunner.ExecuteCommand(commandText).SplitStringByDefaultNewLine();
            foreach (var line in commandResultLines)
            {
                var thisInfo = parseRedHatPackage(line);
                if (thisInfo != null)
                    if (thisInfo.Name == packageName)
                        return thisInfo;
            }
            return null;
        }

        public virtual IEnumerable<string> GetAllTargetRpmNames()
        {
            var rpmNames = new List<String>();
            var commandResultLines = CommandRunner.ExecuteCommand("rpm -qa --qf '%{NAME}\t%{VERSION}\t%{RELEASE}\t%{ARCH}\t%{EPOCH}\n'").SplitStringByDefaultNewLine();
            var allLinuxPackageInfo = commandResultLines.Select(cmdLine => parseRedHatPackage(cmdLine));
            return allLinuxPackageInfo.Where(info => info != null).Select(pkg => pkg.Name);
            //foreach (string line in lines)
            //{
            //    LinuxPackageInfo thisInfo = parseRedHatPackage(line);
            //    if (thisInfo != null)
            //        rpmNames.Add(thisInfo.Name);
            //}

            //return rpmNames;
        }


        private LinuxPackageInfo parseRedHatPackage(string line)
        {
            char[] elemseps = { '\t' };
            string[] ffields = line.Split(elemseps, StringSplitOptions.None);
            if (ffields.GetUpperBound(0) < 4)
                return null;
            LinuxPackageInfo retInfo = new LinuxPackageInfo { Name = ffields[0], Version = ffields[1], Release = ffields[2], Arch = ffields[3] };
            if ((ffields[4][0] >= '0') && (ffields[4][0] <= '9'))
                retInfo.Epoch = uint.Parse(ffields[4]);
            return retInfo;
        }
    }
}