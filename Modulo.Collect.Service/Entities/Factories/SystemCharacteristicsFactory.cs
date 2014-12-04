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
using System.IO;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Service.Exceptions;

namespace Modulo.Collect.Service.Entities.Factories
{
    public class SystemCharacteristicsFactory
    {

        /// <summary>
        /// Creates the system characteristics given the probeResult.
        /// </summary>
        /// <param name="probeResult">result of the probe execution.</param>
        /// <returns></returns>
        public oval_system_characteristics CreateSystemCharacteristics(ProbeResult probeResult)
        {
            oval_system_characteristics systemCharacteristics = new oval_system_characteristics();
            systemCharacteristics.collected_objects = this.GetCollectedObjects(probeResult);
            systemCharacteristics.system_data = this.GetSystemType(probeResult);
            systemCharacteristics.generator = this.GetGenerator();
            systemCharacteristics.system_info = this.GetSystemInfo(probeResult);            
            return systemCharacteristics;
        }

        /// <summary>
        /// Creates the system characteristics in XML format.
        /// </summary>
        /// <param name="probeResult">result of the probe execution.</param>
        /// <returns></returns>
        public string CreateSystemCharacteristicsInXMLFormat(ProbeResult probeResult)
        {
            oval_system_characteristics systemCharacteristics = this.CreateSystemCharacteristics(probeResult);
            return systemCharacteristics.GetSystemCharacteristicsXML();
        }

        /// <summary>
        /// This method creates a new System Characteriscts with base in others system Characteristics.
        /// The generator and system_info of the new systemCharacteristics, will be based on the first element of the systemCharacteristics list.
        /// </summary>
        /// <param name="systemCharacteristics">The system characteristics.</param>
        /// <returns></returns>
        public oval_system_characteristics CreateSystemCharacteristicsBy(IEnumerable<oval_system_characteristics> systemCharacteristics)
        {
            if (systemCharacteristics.Count() > 0)
            {
                SystemInfoType systemInfoType = systemCharacteristics.ElementAt(0).system_info;
                return  this.CreateSystemCharacteristicsBySpecificSystemInfo(systemCharacteristics, systemInfoType);
            }

            return null;
        }

        public oval_system_characteristics CreateSystemCharacteristicsBy(IEnumerable<oval_system_characteristics> systemCharacteristics, SystemInfo systemInfo)
        {
            SystemInfoType systemInfoType = this.GetSystemInfo(systemInfo);
            return this.CreateSystemCharacteristicsBySpecificSystemInfo(systemCharacteristics, systemInfoType);
        }

        /// <summary>
        /// Creates the system characteristics by XML.
        /// </summary>
        /// <param name="systemCharacteristicsInXML">The system characteristics in XML.</param>
        /// <returns></returns>
        public oval_system_characteristics CreateSystemCharacteristicsByXML(string systemCharacteristicsInXML)
        {
            try
            {
                MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(systemCharacteristicsInXML));
                IEnumerable<string> errors;
                oval_system_characteristics ovalSystemCharacteristics = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(m, out errors);
                return ovalSystemCharacteristics;
            }
            catch (Exception ex)
            {
                //throw new InvalidXMLOvalSystemCharacteristicsException(String.Format("The Oval System Characteristics XML is Invalid: {0}", ex.Message), ex);
                throw new InvalidXMLOvalSystemCharacteristicsException(
                    String.Format("The Oval System Characteristics XML is Invalid: {0}", ex.ToString()), ex);
            }      
        }
        /// <summary>
        /// This method returns the CollectedObjects from the ProbeResult.
        /// </summary>
        /// <param name="probeResult">The probe result.</param>
        /// <returns></returns>
        private ObjectType[] GetCollectedObjects(ProbeResult probeResult)
        {
            ObjectType[] objectTypes = new ObjectType[probeResult.CollectedObjects.Count()];
            for(int i = 0; i <= (probeResult.CollectedObjects.Count() - 1); i++)
            {
                CollectedObject collectedObject = probeResult.CollectedObjects.ElementAt(i);
                objectTypes[i] = collectedObject.ObjectType;
            }
            return objectTypes;            
        }

        /// <summary>
        /// Returns the oval ItemType from the ProbeResult.
        /// </summary>
        /// <param name="probeResult">The probe result.</param>
        /// <returns></returns>
        private ItemType[] GetSystemType(ProbeResult probeResult)
        {
            List<ItemType> itemTypes = new List<ItemType>();
            foreach (CollectedObject collectedObject in probeResult.CollectedObjects)
            {   
                //itemTypes.AddRange(collectedObject.SystemData);
                this.AddItemTypesInTheList(itemTypes, collectedObject.SystemData);                
            }           
            
            return itemTypes.ToArray();
        }

        private void AddItemTypesInTheList(List<ItemType> itemTypeList, IList<ItemType> systemData)
        {
            foreach (ItemType itemTypeFromSystemData in systemData)
            {
                IEnumerable<ItemType> existingItemTypes = itemTypeList.Where<ItemType>(item => item.id == itemTypeFromSystemData.id);
                if (existingItemTypes.Count() == 0)
                {
                    itemTypeList.Add(itemTypeFromSystemData);
                }
            }
        }

        /// <summary>
        /// Gets the generator. It is important for the header of the system characteristics.
        /// </summary>
        /// <returns></returns>
        private GeneratorType GetGenerator()
        {
            return DocumentHelpers.GetDefaultGenerator();            
        }

        /// <summary>
        /// Gets the system info.
        /// The system info are informations about the station that was executed the collect.
        /// </summary>
        /// <param name="probeResult">The probe result.</param>
        /// <returns></returns>
        private SystemInfoType GetSystemInfo(ProbeResult probeResult)
        {
            SystemInfoType systemInfo = new SystemInfoType();
            if (probeResult.SystemInformation != null)
            {
                systemInfo.architecture = probeResult.SystemInformation.Architecture;
                systemInfo.os_name = probeResult.SystemInformation.SystemName;
                systemInfo.os_version = probeResult.SystemInformation.SystemVersion;
                systemInfo.primary_host_name = probeResult.SystemInformation.PrimaryHostName;
                systemInfo.interfaces = new InterfaceType[probeResult.SystemInformation.Interfaces.Count()];
                for (int i = 0; i <= (probeResult.SystemInformation.Interfaces.Count - 1); i++)
                {
                    NetworkInterface networkInterface = probeResult.SystemInformation.Interfaces[i];
                    InterfaceType interfaceType = new InterfaceType();
                    interfaceType.interface_name = networkInterface.Name;
                    interfaceType.ip_address = new EntityItemIPAddressStringType()
                    {
                        datatype = SimpleDatatypeEnumeration.ipv4_address,
                        Value = networkInterface.IpAddress
                    };
                    interfaceType.mac_address = networkInterface.MacAddress;
                    systemInfo.interfaces[i] = interfaceType;
                }
            }
            return systemInfo;
        }

        private SystemInfoType GetSystemInfo(SystemInfo systemInfo)
        {
            SystemInfoType systemInfoType = new SystemInfoType();
            if ( systemInfo != null)
            {
                systemInfoType.architecture = systemInfo.Architecture;
                systemInfoType.os_name = systemInfo.SystemName;
                systemInfoType.os_version = systemInfo.SystemVersion;
                systemInfoType.primary_host_name = systemInfo.PrimaryHostName;
                systemInfoType.interfaces = new InterfaceType[systemInfo.NetworkInterfaces.Count()];
                for (int i = 0; i <= (systemInfo.NetworkInterfaces.Count - 1); i++)
                {
                    NetworkInfo networkInterface = systemInfo.NetworkInterfaces[i];
                    InterfaceType interfaceType = new InterfaceType();
                    interfaceType.interface_name = networkInterface.Name;
                    interfaceType.ip_address = new EntityItemIPAddressStringType() { Value = networkInterface.IpAddress };
                    interfaceType.mac_address = networkInterface.MacAddress;
                    systemInfoType.interfaces[i] = interfaceType;
                }
            }
            return systemInfoType;
        }       

        private oval_system_characteristics CreateSystemCharacteristicsBySpecificSystemInfo(IEnumerable<oval_system_characteristics> systemCharacteristics, SystemInfoType systemInfo)
        {
            SystemCharacteristicsBuilder builder = new SystemCharacteristicsBuilder();
            var generator = (systemCharacteristics.Count() > 0) ? systemCharacteristics.ElementAt(0).generator : DocumentHelpers.GetDefaultGenerator();
            return builder.WithGenerator(generator)
                        .WithSystemInfo(systemInfo)
                        .WithCollectedObjectAndSystemDataByTheSystemCharacteristicList(systemCharacteristics)
                        .Build();

        }
    }
}
