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

namespace Modulo.Collect.Probe.Common.BasicClasses
{
    /// <summary>
    /// This class represents the informations of the system that was collected data.
    /// This informations is necessary for the create the system characteristics
    /// </summary>
    public class SystemInformation
    {
        public string SystemName { get; set; }
        public string SystemVersion { get; set; }
        public string Architecture { get; set; }
        public string PrimaryHostName { get; set; }
        public IList<NetworkInterface> Interfaces { get; set; }

        public SystemInformation()
        {
            this.SystemVersion = "";
            this.SystemName = "";
            this.PrimaryHostName = "";
            this.Architecture = "";
            this.Interfaces = new List<NetworkInterface>();
        }

        public override bool Equals(object obj)
        {
            if ( ! (obj is SystemInformation)) 
                return false;

            SystemInformation other = (SystemInformation)obj;

            return (    this.SystemName.Equals(other.SystemName) &&
                        this.SystemVersion.Equals(other.SystemVersion) &&
                        this.Architecture.Equals(other.Architecture) &&
                        this.PrimaryHostName.Equals(other.PrimaryHostName) &&
                        this.CheckInterfacesEquals(other.Interfaces)
                   );                       

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool CheckInterfacesEquals(IList<NetworkInterface> otherInterfaces)
        {
            if ((otherInterfaces == null))
                return false;

            foreach (NetworkInterface network in this.Interfaces)
            {
                var otherNetwork = otherInterfaces.SingleOrDefault(nt => nt.Equals(network));
                if (otherNetwork == null)
                    return false;
            }

            return true;
        }
    }
}
