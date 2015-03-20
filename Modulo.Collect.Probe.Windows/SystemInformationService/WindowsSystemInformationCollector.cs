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
using System.Collections.Generic;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Windows.WMI;

namespace Modulo.Collect.Probe.Windows.SystemInformationService
{
    public class WindowsSystemInformationCollector
    {

        
        private WmiDataProvider wmiDataProvider;
        
        public WindowsSystemInformationCollector(WmiDataProvider wmiDataProvider)
        {
            this.wmiDataProvider = wmiDataProvider;
        }

        public SystemInformation GetSystemInformation()
        {
            IEnumerable<WmiObject> informationAboutHostName = wmiDataProvider.SearchWmiObjects("Win32_ComputerSystem", null);
            IEnumerable<WmiObject> informationAboutOperationalSystem = wmiDataProvider.SearchWmiObjects("Win32_OperatingSystem", null);
            IEnumerable<WmiObject> informationAboutNetworkAdapter = this.GetInformationsAboutNetworkAdapter();

            return new WindowsSystemInformationFactory().CreateSystemInformation(informationAboutHostName, 
                                                                                 informationAboutOperationalSystem, 
                                                                                 informationAboutNetworkAdapter);
        }        
        private IEnumerable<WmiObject> GetInformationsAboutNetworkAdapter()
        {
            Dictionary<string, string> networkAdaptersParameters = new Dictionary<string, string>();
            networkAdaptersParameters.Add("IPEnabled", "True");
            return wmiDataProvider.SearchWmiObjects("Win32_NetworkAdapterConfiguration", networkAdaptersParameters);
        }        
    }
}
