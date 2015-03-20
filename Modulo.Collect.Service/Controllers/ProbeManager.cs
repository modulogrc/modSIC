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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Plugins;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Services;
using Modulo.Collect.Service.Probes;

namespace Modulo.Collect.Service.Controllers
{
    public class ProbeManager : Modulo.Collect.Service.Controllers.IProbeManager
    {

        private const string PLUGINS_DIRECTORY_NAME = "plugins";

        [ImportMany(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public Lazy<IProbe, IProbeCapabilities>[] probes;

        [ImportMany(RequiredCreationPolicy = CreationPolicy.NonShared)]
        public Lazy<ISystemInformationService, ISystemInformationServicePlataform>[] systemInformationServices;

        /// <summary>
        /// This method load all probes available. 
        /// </summary>
        public void LoadProbes()
        {
            if (this.probes != null)
                return;

            PluginContainer.RegisterProbeAssembly(Assembly.GetExecutingAssembly());
            PluginContainer.RegisterProbeAssembly(typeof(ProbeBase).Assembly);
            PluginContainer.RegisterProbeAssembly(typeof(Probe.Windows.Registry.RegistryProber).Assembly);
            PluginContainer.RegisterProbeAssembly(typeof(Probe.Unix.Family.FamilyProberUnix).Assembly);
            PluginContainer.RegisterProbeAssembly(typeof(Probe.Solaris.Probes.smf.SMFProber).Assembly);
            PluginContainer.RegisterProbeAssembly(typeof(Probe.Independent.Sql.SqlProber).Assembly);
            PluginContainer.RegisterProbeAssembly(typeof(Probe.CiscoIOS.Probes.Version.VersionProber).Assembly);

            var container = PluginContainer.GetProbeCompositionContainer();

            container.ComposeParts(this);
        }


        /// <summary>
        /// Return a probe given a probeCapabilities. If exists more that one probe with these capabilities,
        /// it return a first probe.
        /// </summary>
        /// <param name="probeCapabilities">The probe capabilities.</param>
        /// <returns></returns>
        public IProbe GetProbe(IProbeCapabilities probeCapabilities)
        {
            LoadProbes();

            var probe = 
                this.probes.SingleOrDefault(
                    p => 
                        (p.Metadata.OvalObject.Equals(probeCapabilities.OvalObject) &&
                         (p.Metadata.PlataformName.Equals(probeCapabilities.PlataformName) ||
                          p.Metadata.PlataformName.Equals(FamilyEnumeration.undefined))));
            if (probe != null)
            {
                return probe.Value;
            }
            return null;
        }


        /// <summary>
        /// Returns all capabilities of Collect Service.
        /// </summary>        
        /// <returns>the list of all capabilities of the Collect Service</returns>
        public IList<ProbeCapabilities> GetCapabilities()
        {
            var allProbeCapabilities =
                        (from x in this.probes
                         orderby x.Metadata.PlataformName.ToString()
                         select x.Metadata
                        );

            IList<ProbeCapabilities> lstProbesCapabilities = new List<ProbeCapabilities>();
            foreach (var probeCapabilities in allProbeCapabilities)
            {
                lstProbesCapabilities.Add(
                    new ProbeCapabilities
                    {
                        PlataformName = probeCapabilities.PlataformName,
                        OvalObject = probeCapabilities.OvalObject
                    }
                );
            }
            return lstProbesCapabilities;
        }


        /// <summary>        
        /// Obtain the probes for ObjectTypes informed. 
        /// This method checks which probes will be used to run a group of objectTypes.
        /// </summary>
        /// <param name="objectTypes">The object types.</param>
        /// <param name="plataform">The plataform.</param>
        /// <returns></returns>
        public IEnumerable<SelectedProbe> GetProbesFor(IEnumerable<ObjectType> objectTypes, FamilyEnumeration plataform)
        {
            List<SelectedProbe> selectedProbes = new List<SelectedProbe>();

            var objectTypesByComponentString =
                    from t in objectTypes
                    group t by t.ComponentString into types
                    select new { Capability = types.Key, objectsTypes = types };

            LoadProbes();

            foreach (var type in objectTypesByComponentString)
            {
                IProbeCapabilities capability = this.CreateProbeCapability(type.Capability, plataform);
                IProbe probe = this.GetProbe(capability);
                if (probe != null)
                {
                    SelectedProbe selectedProbe = new SelectedProbe(probe, type.objectsTypes, (ProbeCapabilities)capability);
                    selectedProbes.Add(selectedProbe);
                }
            }
            return selectedProbes;
        }

        public IEnumerable<SelectedProbe> GetNotSupportedObjects(IEnumerable<ObjectType> objectTypes, FamilyEnumeration plataform)
        {
            List<SelectedProbe> selectedProbes = new List<SelectedProbe>();

            var objectTypesByComponentString =
                    from t in objectTypes
                    group t by t.ComponentString into types
                    select new { Capability = types.Key, objectsTypes = types };

            foreach (var type in objectTypesByComponentString)
            {
                IProbeCapabilities capability = this.CreateProbeCapability(type.Capability, plataform);
                IProbe probe = this.GetProbe(capability);
                if (probe == null)
                {
                    probe = this.probes.First().Value;
                    SelectedProbe selectedProbe = new SelectedProbe(probe, type.objectsTypes, (ProbeCapabilities)capability);
                    selectedProbes.Add(selectedProbe);
                }
            }
            return selectedProbes;
        }

        /// <summary>
        /// Gets the system information service by the plataform name.
        /// </summary>
        /// <param name="plataformName">Name of the plataform.</param>
        /// <returns></returns>
        public ISystemInformationService GetSystemInformationService(FamilyEnumeration plataformName)
        {
            var systemInformationService = this.systemInformationServices.SingleOrDefault(systemInfo => systemInfo.Metadata.PlataformName == plataformName);
            if (systemInformationService != null)
            {
                return systemInformationService.Value;
            }
            return null;
        }

        private IProbeCapabilities CreateProbeCapability(String capabilityName, FamilyEnumeration plataform)
        {
            ProbeCapabilities probeCapability = new ProbeCapabilities();
            probeCapability.OvalObject = capabilityName;
            probeCapability.PlataformName = plataform;
            return probeCapability;
        }
    }
}
