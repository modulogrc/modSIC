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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Registry
{
    /// <summary>
    /// This class is an wrapper for oval definitions registry_object. 
    /// Provides a set of methods for the make the manipulation these object more simple.
    /// 
    /// </summary>
    public class RegistryObject
    {    
        private Dictionary<String, EntityObjectStringType> allRegistryEntities;

        public RegistryObject(Dictionary<String, EntityObjectStringType> allRegistryEntities)
        {
            this.allRegistryEntities = allRegistryEntities;
        }      
        

        public string Hive 
        {
            get 
            {
                return this.GetValueOfEntity(registry_object_ItemsChoices.hive.ToString());                
            }
        }
        
        public string Key 
        {
            get 
            {
                return this.GetValueOfEntity(registry_object_ItemsChoices.key.ToString());
            }
        }

        public string Name 
        {
            private set { }
            get 
            { 
                return this.GetValueOfEntity(registry_object_ItemsChoices.name.ToString());
            }
        }

        /// <summary>
        /// Gets the key operation. If  not exists operation defined for key, the operation default
        /// is OperationEnumeration.equals
        /// For more information see RegistryObjectTest.
        /// </summary>
        /// <returns></returns>
        public OperationEnumeration GetKeyOperation()
        { 
             return allRegistryEntities[registry_object_ItemsChoices.key.ToString()].operation;
        }

        /// <summary>
        /// Gets the Name operation. If  not exists operation defined for Name, the operation default
        /// is OperationEnumeration.equals
        /// For more information see RegistryObjectTest.
        /// </summary>
        /// <returns></returns>
        public OperationEnumeration GetNameOperation()
        {
            return allRegistryEntities[registry_object_ItemsChoices.name.ToString()].operation; 
        }

        /// <summary>
        /// Gets the variable id by the entityName.
        /// The entityName can be Key,Hive,Name
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns></returns>
        public string GetVariableId(string entityName)
        {
            if (allRegistryEntities[entityName] != null)
            {
                string value = allRegistryEntities[entityName].var_ref;
                if (value != null)
                    return value;
                else return "";
            }
            return "";
        }

        public string GetValueOfEntity(string entityName)
        {
            if (allRegistryEntities[entityName] != null)
            {
                string value = allRegistryEntities[entityName].Value;
                if (value != null)
                    return value;
                else return "";
            }
            return "";
        }

        public OperationEnumeration GetOperationOfEntity(string entityName)
        {
            if (allRegistryEntities[entityName] != null)
                return allRegistryEntities[entityName].operation;
            else
                return OperationEnumeration.equals;
        }

        public EntitySimpleBaseType GetEntity(string entityName)
        {
            return allRegistryEntities[entityName];
        }


        public void ClearNameEntity()
        {
            this.Name = null;
        }
    }
}
