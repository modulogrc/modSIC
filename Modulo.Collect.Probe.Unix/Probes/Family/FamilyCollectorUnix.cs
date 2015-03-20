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
using Modulo.Collect.Probe.Independent.Family;
using Modulo.Collect.Probe.Unix.Extensions;
using Modulo.Collect.Probe.Common.Extensions;

namespace Modulo.Collect.Probe.Unix.Family
{
    public class FamilyCollectorUnix: IFamilyCollector
    {
        public SshCommandLineRunner CommandRunner { get; set; }

        public virtual string GetOperatingSystemFamily()
        {
            // Due to be ovalDI compliant the Family Collector always returns "unix"
            try
            {
                CommandRunner.SshClient.UnameCommand();
                return "unix";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string RunCustomUname()
        {
            //uname
            //-s system
            //-r release
            //-v version
            // uname -s ou uname -sr

            var distributions = new[] { "fedora", "redhat", "debian", "slackware" };
            foreach (var distribution in distributions)
            {
                var commandText = String.Format("[ -e /etc/{0}-release ]", distribution);
                var commandResult = CommandRunner.ExecuteCommand(commandText);
                if (CommandRunner.LastCommandExitCode == 0)
                {
                    commandText = String.Format("cat /etc/{0}-release", distribution);
                    return CommandRunner.ExecuteCommand(commandText).SplitStringByDefaultNewLine().FirstOrDefault();
                }
            }

            //  Ubuntu
            var command1stLine = CommandRunner.ExecuteCommand("lsb_release -d").SplitStringByDefaultNewLine().FirstOrDefault();
            if (CommandRunner.LastCommandExitCode == 0 && command1stLine != null)
                return command1stLine.Replace("Description:", String.Empty).Trim();
            
            var commandResultLines = CommandRunner.ExecuteCommand("uname -sv").SplitStringByDefaultNewLine();
            if (CommandRunner.LastCommandExitCode == 0)
                return commandResultLines.FirstOrDefault();
            
            return string.Empty;
        }
    }
}
