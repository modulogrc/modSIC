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
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Service.Entities.Factories
{
    public class SystemCharacteristicsBuilder
    {
        private oval_system_characteristics systemCharacteristics;
        private int sequence;
        private List<ObjectType> newObjectTypes;
        private List<ItemType> newItemTypes;
        private List<String> addedItemsId;
        public SystemCharacteristicsBuilder()
        {
            systemCharacteristics = new oval_system_characteristics();
            sequence = 1;
            newObjectTypes = new List<ObjectType>();
            newItemTypes = new List<ItemType>();
            addedItemsId = new List<String>();

        }

        public SystemCharacteristicsBuilder WithGenerator(GeneratorType generator)
        {
            systemCharacteristics.generator = generator;
            return this;
        }

        public SystemCharacteristicsBuilder WithSystemInfo(SystemInfoType systemInfo)
        {
            systemCharacteristics.system_info = systemInfo;
            return this;
        }

        public SystemCharacteristicsBuilder WithCollectedObjectAndSystemDataByTheSystemCharacteristicList(IEnumerable<oval_system_characteristics> systemCharacteristics)
        {
            foreach(oval_system_characteristics sourceSystemCharacteristics in systemCharacteristics)
            {
                this.AddCollectedObjectsWithSystemData(sourceSystemCharacteristics.collected_objects,
                                                          sourceSystemCharacteristics.system_data);
            }                
            return this;
        }

        public SystemCharacteristicsBuilder AddCollectedObjectsWithSystemData(IEnumerable<ObjectType> objectTypes, IEnumerable<ItemType> itemTypes)
        {
            AddObjectTypeWithItemTypesInNewSequence(objectTypes, itemTypes);           
            return this;
        }

        /// <summary>
        /// Return the oval_system_characteristics that was constructed.
        /// </summary>
        /// <returns></returns>
        public oval_system_characteristics Build()
        {
            systemCharacteristics.system_data = newItemTypes.ToArray();
            systemCharacteristics.collected_objects = newObjectTypes.ToArray();

            return systemCharacteristics;
        }

        /// <summary>
        /// This method organize the sequence of itemtypes in the new systemcharacteristics.
        /// </summary>
        /// <param name="objectTypes">The object types.</param>
        /// <param name="itemTypes">The item types.</param>
        private void AddObjectTypeWithItemTypesInNewSequence(IEnumerable<ObjectType> objectTypes, IEnumerable<ItemType> itemTypes)
        {
            Dictionary<String, ItemType> itemsCache = new Dictionary<string, ItemType>();
            List<ItemType> itemTypesList = new List<ItemType>(itemTypes);

            foreach (var sourceObjectType in objectTypes)
            {
                SetReferenceTypesInTheObjectType(itemTypesList, itemsCache, sourceObjectType);
                newObjectTypes.Add(sourceObjectType);
            }
            
        }

        /// <summary>
        /// Sets the type of the reference types in the objectType.
        /// </summary>
        /// <param name="itemTypes">the item types of system characteristics.</param>
        /// <param name="itemsCache">The items cache is the list of item already created.</param>
        /// <param name="sourceObjectType">Type of the source object.</param>        
        private void SetReferenceTypesInTheObjectType(IList<ItemType> itemTypes, Dictionary<String, ItemType> itemsCache,
                                                      ObjectType sourceObjectType)
        {
            if (sourceObjectType.reference != null)
            {
                for (int j = 0; j <= (sourceObjectType.reference.Count() - 1); j++)
                {
                    string sourceReferenceItemId = sourceObjectType.reference[j].item_ref;
                    if (!itemsCache.ContainsKey(sourceReferenceItemId))
                    {
                        ItemType itemType = this.GetItemTypeById(sourceReferenceItemId, itemTypes);
                        itemTypes.Remove(itemType);
                        itemType.id = sequence.ToString();
                        sourceObjectType.reference[j].item_ref = itemType.id;
                        this.newItemTypes.Add(itemType);
                        itemsCache.Add(sourceReferenceItemId, itemType);
                        sequence = sequence + 1;
                    }
                    else
                    {
                        sourceObjectType.reference[j].item_ref = itemsCache[sourceReferenceItemId].id;
                    }
                }
            }
        }

        private bool ThisItemExists(ItemType item)
        {
            return (addedItemsId.Where((x) => x == item.id).Count() > 0);
        }       

        private ItemType GetItemTypeById(string id, IList<ItemType> itemTypes)
        {
            ItemType sourceItemType = itemTypes.Where(i => i.id == id).SingleOrDefault();                
            return sourceItemType;
        }
    }
}
