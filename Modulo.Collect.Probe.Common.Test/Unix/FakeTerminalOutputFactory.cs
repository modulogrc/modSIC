/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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

namespace Modulo.Collect.Probe.Unix.Test
{
    public class FakeTerminalOutputFactory
    {
        public static string GetAllServices()
        {
            var outputBuilder = new StringBuilder();
            
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root  1707 Jul 19 16:17 abrtd");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root  1967 Jun 29 21:05 avahi-daemon");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root   709 Apr 29 11:14 bluetooth");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root  2947 Jun 28 09:13 cups");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root   652 Jun 24 16:12 killall");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root  6256 Jan  5  2010 openvpn");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root  3702 Feb 15  2010 sendmail");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root  3702 Feb 15  2010 sshd");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root  5467 Jul 26 18:57 vboxadd-service");
            outputBuilder.AppendLine("-rwxr-xr-x. 1 root root  1866 May  6 22:24 wpa_supplicant");

            return outputBuilder.ToString();
        }

        public static string GetUnameReturn()
        {
            var outputBuilder = new StringBuilder();
            outputBuilder.AppendLine("Linux");
            outputBuilder.AppendLine("Fedora13VM");
            outputBuilder.AppendLine("2.6.34.7-56.fc13.x86_64");
            outputBuilder.AppendLine("#1 SMP Wed Sep 15 03:36:55 UTC 2010");
            outputBuilder.AppendLine("x86_64");
            outputBuilder.AppendLine("x86_64");
            
            return outputBuilder.ToString();
        }

        public static string GetLsCommandReturn(bool onlyOneFile, bool forDirectory)
        {
            if (onlyOneFile)
            {
                var filename = forDirectory ? string.Empty : "results.xml";
                return string.Format("-rw-r--r-- 1 500 500 1408809 Mar 10 14:49 /home/oval/{0}", filename);
            }

            var outputBuilder = new StringBuilder();
            outputBuilder.AppendLine("drwxr-xr-x 2 500 500    4096 Dec  2 10:17 /home/oval/Desktop");
            outputBuilder.AppendLine("-rw-rw-r-- 1 500 500  652849 Mar  1 17:28 /home/oval/RM7-scap-rhel5-oval.xml");
            outputBuilder.AppendLine("drwxrwxr-x 2 500 500    4096 Dec 23 15:15 /home/oval/coletores");
            outputBuilder.AppendLine("-rw-rw-r-- 1 500 500   20625 Mar 10 14:50 /home/oval/ovaldi.log");
            outputBuilder.AppendLine("-rw-r--r-- 1 500 500  114699 Mar 10 14:49 /home/oval/results.html");
            outputBuilder.AppendLine("-rw-r--r-- 1 500 500 1408809 Mar 10 14:49 /home/oval/results.xml");
            outputBuilder.AppendLine("-rw-r--r-- 1 500 500  403425 Mar 10 14:49 /home/oval/system-characteristics.xml");
            return outputBuilder.ToString();
        }

    }
}
