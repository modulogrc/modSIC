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
using System.Linq;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Tamir.SharpSsh;




namespace Modulo.Collect.Probe.Unix
{
    [SystemInformation(PlataformName = FamilyEnumeration.unix)]
    public class UnixSysteformationService: ISystemInformationService
    {
        public SystemInformation GetSystemInformationFrom(TargetInfo target)
        {
            var sshExec = new SshExec(target.GetAddress(), target.credentials.GetUserName(), target.credentials.GetPassword());
            sshExec.Connect(target.GetPort());

            var collectedUnixSystemInformation = SystemInformationCollector.getSysInfo(sshExec);
            
            return this.CreateSystemInformationInstance(collectedUnixSystemInformation);
        }

        private SystemInformation CreateSystemInformationInstance(SysInfo collectedUnixSystemInformation)
        {
            var newSystemInformation = 
                new SystemInformation()
                {
                    Architecture = collectedUnixSystemInformation.Architecture,
                    PrimaryHostName = collectedUnixSystemInformation.Hostname,
                    SystemName = collectedUnixSystemInformation.OS,
                    SystemVersion = collectedUnixSystemInformation.OSVersion
                };
            
            foreach (var netInterface in collectedUnixSystemInformation.Interfaces)
            {
                var newNetworkInterface = this.CreateNetworkInterfaceInstanceFromInterfaceState(netInterface);
                newSystemInformation.Interfaces.Add(newNetworkInterface);
            }

            return newSystemInformation;
        }

        private NetworkInterface CreateNetworkInterfaceInstanceFromInterfaceState(InterfaceState interfaceState)
        {
            string ipAddress = string.Empty;
            try { ipAddress = interfaceState.InetAddr.First().IPAddr.ToString(); } catch (Exception) { }

            return new NetworkInterface() { Name = interfaceState.Name, MacAddress = interfaceState.HWAddr, IpAddress = ipAddress };
        }
    }
}
