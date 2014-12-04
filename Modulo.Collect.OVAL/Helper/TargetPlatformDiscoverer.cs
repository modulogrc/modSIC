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
using System.Xml.Serialization;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.OVAL.Helper
{
    /// <summary>
    /// It discovers the platform of a target from its IP Address or host name.
    /// </summary>
    public class TargetPlatformDiscoverer
    {
        private List<String> OvalDefinitionsObjectNamepaces;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ovalDefinitionsObjects">The oval objects list from Oval Definitions.</param>
        public TargetPlatformDiscoverer(ObjectType[] ovalDefinitionsObjects)
        {
            this.OvalDefinitionsObjectNamepaces = new List<String>();
            if (ovalDefinitionsObjects != null)
                foreach (var @object in ovalDefinitionsObjects)
                    foreach (var attribute in @object.GetType().GetCustomAttributes(typeof(XmlRootAttribute), false))
                        if (!this.OvalDefinitionsObjectNamepaces.Contains((attribute as XmlRootAttribute).Namespace))
                            this.OvalDefinitionsObjectNamepaces.Add((attribute as XmlRootAttribute).Namespace);
        }

        /// <summary>
        /// It runs the target platform discovering for a target address passed in constructor.
        /// </summary>
        /// <returns>The system family.</returns>
        public FamilyEnumeration Discover()
        {
            if (this.IsTargetWindowsSystem())
                return FamilyEnumeration.windows;

            if (this.IsTargetUnixSystem())
                return FamilyEnumeration.unix;

            if (this.IsTargetCiscoIOSSystem())
                return FamilyEnumeration.ios;

            return FamilyEnumeration.undefined;
            
            
        }

        private bool IsTargetCiscoIOSSystem()
        {
            return (OvalDefinitionsObjectNamepaces.Where(ns => ns.Contains("#ios")).Count() > 0);
        }

        private bool IsTargetWindowsSystem()
        {
            return (OvalDefinitionsObjectNamepaces.Where(ns => ns.Contains("#windows")).Count() > 0);
        }

        private bool IsTargetUnixSystem()
        {
            var hasUnixNamespace = OvalDefinitionsObjectNamepaces.Where(ns => ns.Contains("#unix")).Count() > 0;
            var hasLinuxNamespace = OvalDefinitionsObjectNamepaces.Where(ns => ns.Contains("#linux")).Count() > 0;
            var hasSolarisNamespace = OvalDefinitionsObjectNamepaces.Where(ns => ns.Contains("#solaris")).Count() > 0;

            return (hasUnixNamespace || hasLinuxNamespace || hasSolarisNamespace);
        }
    }
}
