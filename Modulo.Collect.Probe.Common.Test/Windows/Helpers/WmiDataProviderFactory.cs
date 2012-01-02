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
using Modulo.Collect.Probe.Windows.WMI;
using Rhino.Mocks;

namespace Modulo.Collect.Probe.Windows.Test.helpers
{
    class WmiDataProviderExpectFactory
    {
        public WmiDataProvider GetFakeWmiDataProviderForTestExecutingQueriesForSystemInfo()
        {
            MockRepository mocks = new MockRepository();
            WmiDataProvider wmiDataProvider = mocks.StrictMock<WmiDataProvider>();
            Expect.Call(wmiDataProvider.SearchWmiObjects("Win32_ComputerSystem", null)).Return(this.GetWmiObjectsForComputerSystemQuery());
            Expect.Call(wmiDataProvider.SearchWmiObjects("Win32_OperatingSystem", null)).Return(this.GetWmiObjectsForOperatingSystemQuery());
            Expect.Call(wmiDataProvider.SearchWmiObjects("Win32_NetworkAdapterConfiguration", null)).IgnoreArguments().Return(this.GetWmiObjectsForNetworkInterfaces());
            Expect.Call(wmiDataProvider.SearchWmiObjects("Win32_NetworkAdapter", null)).IgnoreArguments(). Return(this.GetWmiObjectsForNetworkInterfacesAdapter());
            mocks.ReplayAll();            

            return wmiDataProvider;
        }

        public WmiDataProvider GetFakeWmiDataProviderForTestInvokeMethodEnumKeyWithReturnSuccess()
        {
            MockRepository mocks = new MockRepository();
            WmiDataProvider wmiDataProvider = mocks.DynamicMock<WmiDataProvider>();
            Expect.Call(wmiDataProvider.InvokeMethod("EnumKey", null)).IgnoreArguments().Repeat.Any().Return(this.GetReturnOfInvokeMethodEnumKey());
            mocks.ReplayAll();

            return wmiDataProvider;
        }

        private Dictionary<string, object> GetReturnOfInvokeMethodEnumKey()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("ReturnValue", (uint)1);
            return result;
        }

        public IEnumerable<WmiObject> GetWmiObjectsForNetworkInterfacesAdapter()
        {
            WmiObject networkInterfaceAdapter = new WmiObject();
            networkInterfaceAdapter.Add("Index", 6);
            networkInterfaceAdapter.Add("PhysicalAdapter", true);
            networkInterfaceAdapter.Add("AdapterTypeId", 0);            
            List<WmiObject> wmiObjects = new List<WmiObject>();
            wmiObjects.Add(networkInterfaceAdapter);
            return wmiObjects;        
        }

        public IEnumerable<WmiObject> GetWmiObjectsForNetworkInterfaces()
        {
            WmiObject networkInterface = new WmiObject();
            networkInterface.Add("Index", 6);
            networkInterface.Add("InterfaceIndex",10);
            networkInterface.Add("Description", "Intel(R) 82567LM-3 Gigabit Network Connection");
            networkInterface.Add("MACAddress", "00 - 23 - AE - B6 - 6F - BF"); 
            networkInterface.Add("DHCPEnabled", true);
            networkInterface.Add("IPAddress", new String[] { "172.16.3.166", "fe80::99ab:a003:a78e:103b" });
            networkInterface.Add("IPSubnet", new String[] { "255.255.254.0", "64" });            
            networkInterface.Add("IPUseZeroBroadcast", false);

            List<WmiObject> wmiObjects = new List<WmiObject>();
            wmiObjects.Add(networkInterface);
            return wmiObjects;        
        }

        public IEnumerable<WmiObject> GetWmiObjectsForOperatingSystemQuery()
        {
            WmiObject os = new WmiObject();
            os.Add("Version", "6.0.6002");
            os.Add("Name", "Microsoft® Windows Server® 2008 Enterprise |C:\\Windows|\\Device\\Harddisk0\\Partition2");
            os.Add("ServicePackMajorVersion", (ushort) 2);
            os.Add("ServicePackMinorVersion", (ushort) 0);

            List<WmiObject> wmiObjects = new List<WmiObject>();
            wmiObjects.Add(os);
            return wmiObjects;
        }

        public IEnumerable<WmiObject> GetWmiObjectsForComputerSystemQuery()
        {
            WmiObject system = new WmiObject();
            system.Add("SystemType", "X86-based PC");
            system.Add("DNSHostName", "MSS-RJ-220");
            system.Add("PartOfDomain", true);
            system.Add("Domain", "mss.modulo.com.br");
            system.Add("Name","MSS-RJ-220");


            List<WmiObject> wmiObjects = new List<WmiObject>();
            wmiObjects.Add(system);
            return wmiObjects;
        }
        
    }
}
