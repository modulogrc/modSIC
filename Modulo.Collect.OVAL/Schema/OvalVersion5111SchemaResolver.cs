///* 
// * Modulo Open Distributed SCAP Infrastructure Collector (modSIC) 
// *  
// * Copyright (c) 2011-2015, Modulo Solutions for GRC. 
// * All rights reserved. 
// *  
// * Redistribution and use in source and binary forms, with or without 
// * modification, are permitted provided that the following conditions are met: 
// *  
// * - Redistributions of source code must retain the above copyright notice, 
// *   this list of conditions and the following disclaimer. 
// *    
// * - Redistributions in binary form must reproduce the above copyright  
// *   notice, this list of conditions and the following disclaimer in the 
// *   documentation and/or other materials provided with the distribution. 
// *    
// * - Neither the name of Modulo Security, LLC nor the names of its 
// *   contributors may be used to endorse or promote products derived from 
// *   this software without specific  prior written permission. 
// *    
// * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// * POSSIBILITY OF SUCH DAMAGE. 
// * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;

namespace Modulo.Collect.OVAL.Schema
{
    /// <summary> 
    /// Provides resource-based XML Uri resolution. 
    /// </summary> 
    internal class OvalVersion5111SchemaResolver : BaseResourceSchemaResolver
    {
        public OvalVersion5111SchemaResolver()
            : base("Schema.resources.v5111", "5.11", _exposedSchemas)
        {
        }

        private static readonly string[] _exposedSchemas = new[] {
            "aix-definitions-schema.xsd",
            "aix-system-characteristics-schema.xsd",
            "android-definitions-schema.xsd",
            "android-system-characteristics-schema.xsd",
            "apache-definitions-schema.xsd",
            "apache-system-characteristics-schema.xsd",
            "apple-ios-definitions-schema.xsd",
            "apple-ios-system-characteristics-schema.xsd",
            "asa-definitions-schema.xsd",
            "asa-system-characteristics-schema.xsd",
            "catos-definitions-schema.xsd",
            "catos-system-characteristics-schema.xsd",
            "esx-definitions-schema.xsd",
            "esx-system-characteristics-schema.xsd",
            "freebsd-definitions-schema.xsd",
            "freebsd-system-characteristics-schema.xsd",
            "hpux-definitions-schema.xsd",
            "hpux-system-characteristics-schema.xsd",
            "independent-definitions-schema.xsd",
            "independent-system-characteristics-schema.xsd",
            "ios-definitions-schema.xsd",
            "ios-system-characteristics-schema.xsd",
            "iosxe-definitions-schema.xsd",
            "iosxe-system-characteristics-schema.xsd",
            "junos-definitions-schema.xsd",
            "junos-system-characteristics-schema.xsd",
            "linux-definitions-schema.xsd",
            "linux-system-characteristics-schema.xsd",
            "macos-definitions-schema.xsd",
            "macos-system-characteristics-schema.xsd",
            "netconf-definitions-schema.xsd",
            "netconf-system-characteristics-schema.xsd",
            "oval-common-schema.xsd",
            "oval-definitions-schema.xsd",
            "oval-system-characteristics-schema.xsd",
            "pixos-definitions-schema.xsd",
            "pixos-system-characteristics-schema.xsd",
            "sharepoint-definitions-schema.xsd",
            "sharepoint-system-characteristics-schema.xsd",
            "solaris-definitions-schema.xsd",
            "solaris-system-characteristics-schema.xsd",
            "unix-definitions-schema.xsd",
            "unix-system-characteristics-schema.xsd",
            "windows-definitions-schema.xsd",
            "windows-system-characteristics-schema.xsd",
            "xmldsig-core-schema.xsd",
        };        
    }
}