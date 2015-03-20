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
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Windows.Registry
{
    public class RegistryItemTypeFactory
    {

        /// <summary>
        /// Creates the registry item types from the list of RegistryObjects.
        /// </summary>
        /// <param name="registryObjects">The registry objects.</param>
        /// <returns></returns>
        public IEnumerable<registry_item> CreateRegistryItemTypesFrom(IEnumerable<RegistryObject> registryObjects)
        {
            List<registry_item> registryItems = new List<registry_item>();
            foreach (RegistryObject registryObject in registryObjects)
            {
                registry_item registryItem = this.CreateRegistryItem(registryObject.Hive, registryObject.Key, registryObject.Name);
                registryItems.Add(registryItem);
            }
            return registryItems;
        }

        public registry_item CreateRegistryItem(string hive, string key, string name)
        {
            registry_item newRegistryItem = new registry_item();
            newRegistryItem.hive = new EntityItemRegistryHiveType() { Value = hive };            
            newRegistryItem.key = this.CreateEntityItemStringType(key);
            newRegistryItem.name = this.CreateEntityItemStringType(name);

            return newRegistryItem;
        }

        public registry_item CreateRegistryItem(string hive, string key, string name, StatusEnumeration status)
        {
            registry_item newRegistryItem = this.CreateRegistryItem(hive, key, name);
            newRegistryItem.status = status;
            return newRegistryItem;
        }

         private EntityItemStringType CreateEntityItemStringType(string entityValue)
        {
            EntityItemStringType newEntityItem = new EntityItemStringType();
            newEntityItem.Value = entityValue;             
            return newEntityItem;
        }


        public IEnumerable<ItemType> CreateRegistryItemTypesByCombinationOfEntitiesFrom(
                                                                   IEnumerable<string> hives, 
                                                                   IEnumerable<string> keys, 
                                                                   IEnumerable<string> names, RegistryObject registryObjectSource)
        {
            List<ItemType> registryObjects = new List<ItemType>();
                
            registryObjects.AddRange(this.VerifyErrorsInTheListOfRegistryEntities(hives,keys,names,registryObjectSource));
            if (registryObjects.Count == 0)
                foreach (string hiveValue in hives)
                    foreach (string keyValue in keys)
                        foreach (string nameValue in names)
                            registryObjects.Add(this.CreateRegistryItem(hiveValue, keyValue, nameValue));
                        
            
            return registryObjects;
        }

        private IEnumerable<ItemType> VerifyErrorsInTheListOfRegistryEntities(
            IEnumerable<string> hives, IEnumerable<string> keys, IEnumerable<string> names, RegistryObject registryObjectSource)
        {
            List<ItemType> registryObjects = new List<ItemType>();
            if ((hives == null) || (hives.Count() == 0))
            {
                registry_item registry = this.CreateRegistryItem(registryObjectSource.Hive, "", "",StatusEnumeration.doesnotexist);                
                registry.hive.status = StatusEnumeration.doesnotexist;
                registryObjects.Add(registry);
                return registryObjects;
            }
            if ((keys == null) || (keys.Count() == 0))
            {
                registry_item registry = this.CreateRegistryItem(registryObjectSource.Hive, registryObjectSource.Key, "", StatusEnumeration.doesnotexist);                
                registry.key.status = StatusEnumeration.doesnotexist;
                registryObjects.Add(registry);
                return registryObjects;
            }
            if ((names == null) || (names.Count() == 0))
            {
                registry_item registry = this.CreateRegistryItem(registryObjectSource.Hive, registryObjectSource.Key, registryObjectSource.Name,StatusEnumeration.doesnotexist);                
                registry.name.status = StatusEnumeration.doesnotexist;
                registryObjects.Add(registry);
                return registryObjects;
            }

            return registryObjects;
        }


    }
}
