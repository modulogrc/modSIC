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
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Windows.WMI;

namespace Modulo.Collect.Probe.Windows.SystemInformationService
{
    public class WindowsSystemInformationFactory
    {

        public SystemInformation CreateSystemInformation(IEnumerable<WmiObject> informationAboutHostName, 
                                                            IEnumerable<WmiObject> informationAboutOperationalSystem, 
                                                            IEnumerable<WmiObject> informationAboutNetworkAdapter)
        {

            SystemInformation systemInformation = new SystemInformation();
            this.SetInformationAboutHostName(systemInformation,informationAboutHostName);
            this.SetInformationAboutOperationalSystem(systemInformation, informationAboutOperationalSystem);
            this.SetInformationAboutNetworkAdapters(systemInformation, informationAboutNetworkAdapter);

            return systemInformation;

        }

        private void SetInformationAboutNetworkAdapters(SystemInformation systemInformation, IEnumerable<WmiObject> informationAboutNetworkAdapter)
        {
            if (!this.TheListIsEmpty(informationAboutNetworkAdapter))
            {
                WmiObject wmiObject = informationAboutNetworkAdapter.First();
                NetworkInterface networkInterface = new NetworkInterface();
                networkInterface.IpAddress = ((string[])wmiObject.GetValueOf("IPAddress"))[0];
                networkInterface.MacAddress = wmiObject.GetValueOf("MACAddress").ToString();
                networkInterface.Name = wmiObject.GetValueOf("Description").ToString();
                systemInformation.Interfaces.Add(networkInterface);
                
            }
        }

        private void SetInformationAboutOperationalSystem(SystemInformation systemInformation, IEnumerable<WmiObject> informationAboutOperationalSystem)
        {
            if (! this.TheListIsEmpty(informationAboutOperationalSystem))
            {
                WmiObject wmiObject = informationAboutOperationalSystem.First();
                systemInformation.SystemName = this.GetSystemName(wmiObject);
                systemInformation.SystemVersion = wmiObject.GetValueOf("Version").ToString();                
            }
        }

        
        private void SetInformationAboutHostName(SystemInformation systemInformation,IEnumerable<WmiObject> informationAboutHostName)
        {
            if (! this.TheListIsEmpty(informationAboutHostName))
            {
                WmiObject wmiObject = informationAboutHostName.First();
                systemInformation.PrimaryHostName = this.GetHostName(wmiObject);
                systemInformation.Architecture = this.GetArchitecture(wmiObject);
            }
            
        }

        
        /// <summary>
        /// Return the name of host name concatenated with the domain name.
        /// </summary>
        /// <param name="wmiObject">The WMI object.</param>
        /// <returns></returns>
        private string GetHostName(WmiObject wmiObject)
        {            
            string hostName = wmiObject.GetFieldValueAsString("DNSHostName").ToLower();
            if (string.IsNullOrEmpty(hostName))
                hostName = wmiObject.GetFieldValueAsString("Name").ToLower();

            object partOfDomain = wmiObject.GetValueOf("PartOfDomain");
            if ((partOfDomain != null) && ((bool)partOfDomain))
            {
                hostName += "." + wmiObject.GetFieldValueAsString("Domain");
            }

            return hostName;           
        }

        /// <summary>
        /// Gets the name of the system.
        /// </summary>
        /// <param name="wmiObject">The WMI object.</param>
        /// <returns></returns>
        private string GetSystemName(WmiObject wmiObject)
        {            
            string osName = wmiObject.GetValueOf("Name").ToString();
            osName = osName.Split('|')[0].Trim();
            osName = osName.Replace("®", "");
            osName = osName.Replace("™", "");
            ushort spmajor = (ushort)wmiObject.GetValueOf("ServicePackMajorVersion");
            ushort spminor = (ushort)wmiObject.GetValueOf("ServicePackMinorVersion");
            if (spmajor > 0)
            {
                osName += " SP" + spmajor.ToString();
                if (spminor > 0)
                    osName += "." + spminor.ToString();
            }
            return osName;
        }


        public bool TheListIsEmpty(IEnumerable<WmiObject> list)
        {
            return ((list == null) || (list.Count() == 0));
        }

        private string GetArchitecture(WmiObject wmiObject)
        {
            string systemType = wmiObject.GetValueOf("SystemType").ToString();
            switch (systemType)
            {
                case "X86-based PC":
                case "X86-Nec98 PC":
                    systemType = "INTEL32";
                    break;
                case "MIPS-based PC":
                    systemType = "MIPS";
                    break;
                case "Alpha-based PC":
                    systemType = "ALPHA32";
                    break;
                case "Power PC":
                    systemType = "POWERPC32";
                    break;
                case "SH-x PC":
                    systemType = "SUPERH";
                    break;
                case "StrongARM PC":
                    systemType = "STRONGARM";
                    break;
                case "64-bit Intel PC":
                    systemType = "INTEL64";
                    break;
                case "64-bit Alpha PC":
                    systemType = "ALPHA64";
                    break;
                default:
                    systemType = "UNKNOWN";
                    break;
            }
            return systemType;
        }

    }
}
