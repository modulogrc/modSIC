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

namespace Modulo.Collect.Probe.Common
{
    public enum TargetParameters { HostName, IPAddr, PortNum };

    public enum ConnectionType { SSH, Telnet };

    public class TargetInfo : Dictionary<string, object>
    {

        public Credentials credentials { get; set; }

        public override string ToString()
        {
            object representation;
            this.TryGetValue(TargetParameters.HostName.ToString(), out representation);
            if (representation == null)
                this.TryGetValue(TargetParameters.IPAddr.ToString(), out representation);
            if (representation == null)
                return String.Format("Target with {0} info elements", this.Count);
            else
                return representation.ToString();
        }

        public int GetPort(ConnectionType connectionType = ConnectionType.SSH)
        {
            object portno = null;
            this.TryGetValue(TargetParameters.PortNum.ToString(), out portno);
            if (String.IsNullOrEmpty(portno as String))
            {
                portno = connectionType == ConnectionType.SSH ? "22" : "23";
            }

            return int.Parse(portno.ToString());
        }

        public string GetAddress()
        {
            object host = null;
            this.TryGetValue(TargetParameters.HostName.ToString(), out host);
            if (host == null)
                host = (string)this[TargetParameters.IPAddr.ToString()];

            return (string)host;
        }

        public string GetRemoteUNC()
        {
            return string.Format(@"\\{0}", this.GetAddress());
        }

        public bool IsLocalTarget()
        {
            string targetAddress = this.GetAddress();
            string machineName = System.Environment.MachineName.ToLower();
            string domainName = System.Environment.UserDomainName.ToLower();
            string fullQualifyName = string.Format("{0}.{1}", machineName, domainName);

            return ( targetAddress.Equals("127.0.0.1") ||
                     targetAddress.ToLower().Equals("localhost") ||
                     targetAddress.ToLower().Equals(machineName) ||
                     targetAddress.ToLower().Equals(fullQualifyName)
                    );
        }

        public bool IsThereCredential()
        {
            if (this.credentials == null) 
                return false;
            
            string username = this.credentials.GetUserName();    
            string password = this.credentials.GetPassword();

            return !((string.IsNullOrEmpty(username)) || (string.IsNullOrEmpty(password)));
        }
    }

    public class TargetInfoFactory
    {
        private string IpAddress;
        private string Domain;
        private string Username;
        private string Password;
        private string AdminPassword;
        private string PortNum;
        
        public TargetInfoFactory(String ipAddress, String domain, String username, String password, String administrativePassword = null, String portNum = null)
        {
            this.IpAddress = ipAddress;
            this.Domain = domain;
            this.Username = username;
            this.Password = password;
            this.AdminPassword = administrativePassword;
            this.PortNum = portNum;
        }

        public TargetInfo Create()
        {
            var newTargetInfo = new TargetInfo();
            newTargetInfo.Add(TargetParameters.IPAddr.ToString(), this.IpAddress);
            newTargetInfo.Add(TargetParameters.PortNum.ToString(), this.PortNum);
            newTargetInfo.credentials = new Credentials(this.Domain, this.Username, this.Password, this.AdminPassword);
            
            return newTargetInfo;
        }
    }
}
