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
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using Modulo.Collect.OVAL.Plugins;

namespace Modulo.Collect.OVAL.Schema
{
    public class ExtensibleXmlResourceResolver : XmlResolver
    {
        private IEnumerable<IOvalSchemaResolver> SchemaResolvers;
        public ExtensibleXmlResourceResolver()
        {
            var _container = PluginContainer.GetOvalCompositionContainer();
            SchemaResolvers = _container.GetExportedValues<IOvalSchemaResolver>()
                .OrderBy(x => x.GetType().Name)
                .ToList();
        }

        /// <summary>
        /// Credential set (not supported)
        /// </summary>
        public override ICredentials Credentials
        {
            set { throw new NotImplementedException(); }
        }

        private string GetVersion(Uri absoluteUri)
        {
            return absoluteUri.Segments.FirstOrDefault(s => s.Contains("version"));
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            var version = GetVersion(absoluteUri);
            var resourceName = absoluteUri.Segments[absoluteUri.Segments.GetUpperBound(0)];

            if (!string.IsNullOrEmpty(version))
                return GetStream(resourceName, version);

            return GetStream(resourceName);
        }

        private IOvalSchemaResolver GetResolver(string resourceName)
        {
            return SchemaResolvers.FirstOrDefault(x => x.GetExposedSchemas().Contains(resourceName));
        }

        private IOvalSchemaResolver GetResolver(string resourceName, string version)
        {
            var resolver = SchemaResolvers.FirstOrDefault(x => version.Contains(x.SchemaVersion) && x.GetExposedSchemas().Contains(resourceName));

            if (resolver == null)
                return GetResolver(resourceName);

            return resolver;
        }

        public Stream GetStream(string resourceName, string version)
        {
            return GetResolver(resourceName, version).GetResourceStreamForSchema(resourceName);
        }

        public Stream GetStream(string resourceName)
        {
            return GetResolver(resourceName).GetResourceStreamForSchema(resourceName);
        }
    }
}
