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
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Modulo.Collect.OVAL.Plugins
{
    public static class PluginContainer
    {
        private static string _pluginDirectoryPath = string.Empty;

        static List<Assembly> _ovalAssemblies = new List<Assembly>();
        static List<Assembly> _probeAssemblies = new List<Assembly>();

        public static void RegisterOvalAssembly(Assembly assembly)
        {
            if(!_ovalAssemblies.Contains(assembly))
                _ovalAssemblies.Add(assembly);
        }

        public static void RegisterProbeAssembly(Assembly assembly)
        {
            if (!_probeAssemblies.Contains(assembly))
                _probeAssemblies.Add(assembly);
        }

        public static void SetPluginDirectoryPath(string path)
        {
            _pluginDirectoryPath = path;
        }

        public static CompositionContainer GetOvalCompositionContainer()
        {
            var catalog = new AggregateCatalog();
            var thisAssembly = typeof (PluginContainer).Assembly;
            if(!_ovalAssemblies.Contains(thisAssembly))
                catalog.Catalogs.Add(new AssemblyCatalog(thisAssembly));
            foreach (var ovalAssembly in _ovalAssemblies)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(ovalAssembly));
            }
            
            if(!string.IsNullOrEmpty(_pluginDirectoryPath))
                catalog.Catalogs.Add(new DirectoryCatalog(_pluginDirectoryPath));

            var _container = new CompositionContainer(catalog);
            return _container;
        }
        public static CompositionContainer GetProbeCompositionContainer()
        {
            var catalog = new AggregateCatalog();
            var thisAssembly = typeof(PluginContainer).Assembly;
            if (!_probeAssemblies.Contains(thisAssembly))
                catalog.Catalogs.Add(new AssemblyCatalog(thisAssembly));
            foreach (var probeAssembly in _probeAssemblies)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(probeAssembly));
            }
            
            if (!string.IsNullOrEmpty(_pluginDirectoryPath))
                catalog.Catalogs.Add(new DirectoryCatalog(_pluginDirectoryPath));
            
            var _container = new CompositionContainer(catalog);
            return _container;
        }
    }
}
