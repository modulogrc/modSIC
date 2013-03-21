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
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.Probe.Common.Extensions;

namespace Modulo.Collect.Probe.Unix
{
    public class UnixTerminalParser
    {
        private static char[] FIELD_SEPARATOR = new char[2] { '\t', ' ' };

        public static IEnumerable<string> GetServicesFromTerminalOutput(string terminalOutput)
        {
            var services = new List<string>();
            var lines = terminalOutput.SplitStringByDefaultNewLine();
            foreach (string line in lines)
            {
                var tokens = line.Split(FIELD_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
                var lastToken = tokens.GetUpperBound(0);
                if (lastToken >= 0)
                    services.Add(tokens[lastToken]);
            }

            return services;
        }

        public static void MapUnameCommandOutputToUnameItem(uname_item item, string terminalOutput)
        {
            var unameParts = terminalOutput.SplitStringByDefaultNewLine().ToArray();

            item.os_name = OvalHelper.CreateItemEntityWithStringValue(unameParts[0]);
            item.node_name = OvalHelper.CreateItemEntityWithStringValue(unameParts[1]);
            item.os_release = OvalHelper.CreateItemEntityWithStringValue(unameParts[2]);
            item.os_version = OvalHelper.CreateItemEntityWithStringValue(unameParts[3]);
            item.machine_class = OvalHelper.CreateItemEntityWithStringValue(unameParts[4]);
            item.processor_type = OvalHelper.CreateItemEntityWithStringValue(unameParts[5]);
        }
    }
}
