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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;

namespace Modulo.Collect.Probe.Unix.Test
{
    [TestClass]
    public class UnixTerminalParserTests
    {

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_parse_services_on_terminal()
        {
            var fakServices = FakeTerminalOutputFactory.GetAllServices();

            var parsedServices = UnixTerminalParser.GetServicesFromTerminalOutput(fakServices).ToArray();

            Assert.AreEqual(10, parsedServices.Count());
            Assert.AreEqual("abrtd", parsedServices[0]);
            Assert.AreEqual("avahi-daemon", parsedServices[1]);
            Assert.AreEqual("bluetooth", parsedServices[2]);
            Assert.AreEqual("cups", parsedServices[3]);
            Assert.AreEqual("killall", parsedServices[4]);
            Assert.AreEqual("openvpn", parsedServices[5]);
            Assert.AreEqual("sendmail", parsedServices[6]);
            Assert.AreEqual("sshd", parsedServices[7]);
            Assert.AreEqual("vboxadd-service", parsedServices[8]);
            Assert.AreEqual("wpa_supplicant", parsedServices[9]);
        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_map_uname_command_output_to_uname_item()
        {
            var itemToMap = new uname_item();

            UnixTerminalParser.MapUnameCommandOutputToUnameItem(
                itemToMap, FakeTerminalOutputFactory.GetUnameReturn());

            Assert.AreEqual("Linux", itemToMap.os_name.Value);
            Assert.AreEqual("Fedora13VM", itemToMap.node_name.Value);
            Assert.AreEqual("2.6.34.7-56.fc13.x86_64", itemToMap.os_release.Value);
            Assert.AreEqual("#1 SMP Wed Sep 15 03:36:55 UTC 2010", itemToMap.os_version.Value);
            Assert.AreEqual("x86_64", itemToMap.machine_class.Value);
            Assert.AreEqual("x86_64", itemToMap.processor_type.Value);
        }
    }
}




