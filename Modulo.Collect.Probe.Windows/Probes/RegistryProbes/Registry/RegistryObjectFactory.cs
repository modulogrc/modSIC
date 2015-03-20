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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Registry
{
    public class RegistryObjectFactory
    {

        /// <summary>
        /// Creates the registry object given the oval Registry Object.
        /// </summary>
        /// <param name="ovalRegistryObject">The oval registry object.</param>
        /// <returns></returns>
        public static RegistryObject CreateRegistryObject(registry_object ovalRegistryObject)
        {
            Dictionary<String, EntityObjectStringType> allRegistryEntities = OvalHelper.GetRegistryEntitiesFromObjectType(ovalRegistryObject);
            return new RegistryObject(allRegistryEntities);

        }

        /// <summary>
        /// Creates the registry objects by combination of entities.
        /// This method combines the entities for the create multiples RegistryObjects given a other RegistryObject as source. 
        /// Ex. We can have the hives [HKEY_LOCAL_MACHINE], the keys [Microsoft\Windows,Microsoft\Windows NT] 
        /// and the names [CurrentVersion,CurrentBuild]. 
        /// With these wqlParameters will be created 4 registryObjects:
        ///  1 - RegistryObject {hive = HKEY_LOCAL_MACHINE, key = Microsoft\Windows, name = CurrentVersion}
        ///  2 - RegistryObject {hive = HKEY_LOCAL_MACHINE, key = Microsoft\Windows, name = CurrentBuild}
        ///  3 - RegistryObject {hive = HKEY_LOCAL_MACHINE, key = Microsoft\Windows NT, name = CurrentVersion}
        ///  4 - RegistryObject {hive = HKEY_LOCAL_MACHINE, key = Microsoft\Windows NT, name = CurrentBuild}
        ///  The information of Variables and Operations for each entity, will be copied from source RegistryObject.
        /// </summary>
        /// <param name="hives">The hives.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="names">The names.</param>
        /// <param name="registryObjectSource"> the source registry object </param>
        /// <returns></returns>
        public static IEnumerable<RegistryObject> CreateRegistryObjectsByCombinationOfEntitiesFrom(IEnumerable<string> hives, 
                                                                                                   IEnumerable<string> keys, 
                                                                                                   IEnumerable<string> names,
                                                                                                   RegistryObject registryObjectSource)
        {
            List<RegistryObject> registryObjects = CreateResgistryObjectByCombination(hives, keys, names, registryObjectSource);
            return registryObjects;
        }

        private static List<RegistryObject> CreateResgistryObjectByCombination(
                                                        IEnumerable<string> hives, 
                                                        IEnumerable<string> keys, 
                                                        IEnumerable<string> names, 
                                                        RegistryObject registryObjectSource)
        {
            List<RegistryObject> registryObjects = new List<RegistryObject>();
            foreach (string hiveValue in hives)
            {
                foreach (string keyValue in keys)
                {
                    foreach (string nameValue in names)
                    {
                        RegistryObject registry = RegistryObjectFactory.CreateRegistryObjectFrom(hiveValue, keyValue, nameValue, registryObjectSource);
                        registryObjects.Add(registry);
                    }
                }
            }
            return registryObjects;
        }

        private static RegistryObject CreateRegistryObjectFrom(string hive, string key, string name, RegistryObject source)
        {
            Dictionary<string, EntityObjectStringType> registryEntities = new Dictionary<string, EntityObjectStringType>();
            registryEntities.Add(registry_object_ItemsChoices.hive.ToString(), CreateEntityObjectStringType(registry_object_ItemsChoices.hive.ToString(),hive,source));
            registryEntities.Add(registry_object_ItemsChoices.key.ToString(), CreateEntityObjectStringType(registry_object_ItemsChoices.key.ToString(), key, source));
            registryEntities.Add(registry_object_ItemsChoices.name.ToString(), CreateEntityObjectStringType(registry_object_ItemsChoices.name.ToString(), name, source));
            return new RegistryObject(registryEntities);

        }

        private static EntityObjectStringType CreateEntityObjectStringType(string entityName, string value, RegistryObject source)
        {
            return new EntityObjectStringType()
            {
                Value = value,
                var_ref = source.GetVariableId(entityName),
                operation = source.GetOperationOfEntity(entityName)
            };
        }

    }
}
