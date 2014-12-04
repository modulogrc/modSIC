#region License
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Modulo.Collect.Service.Contract;

namespace Modulo.Collect.ClientConsole
{
    public class ServerSection : ConfigurationSection
    {
        [ConfigurationProperty("modSIC", IsDefaultCollection = false, IsRequired = false)]
        public modsicConfiguration Server
        {
            get
            {
                return (modsicConfiguration)this["modSIC"];
            }
        }

        [ConfigurationProperty("collects", IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof(CollectsCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public CollectsCollection Collects
        {
            get
            {
                return (CollectsCollection)this["collects"];
            }
        }
    }

    public class CollectsCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CollectElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((CollectElement)element).Name;
        }

        public CollectElement this[int index]
        {
            get
            {
                return (CollectElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public CollectElement this[string Name]
        {
            get
            {
                return (CollectElement)BaseGet(Name);
            }
        }

        public int IndexOf(CollectElement user)
        {
            return BaseIndexOf(user);
        }

        public void Add(CollectElement user)
        {
            BaseAdd(user);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(CollectElement url)
        {
            if (BaseIndexOf(url) >= 0)
                BaseRemove(url.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    public class CollectElement : ConfigurationElement
    {
        public CollectElement() { }

        public CollectElement(string name, string target, string username, string password, string definitions)
        {
            this.Name = name;
            this.Target = target;
            this.Username = username;
            this.Password = password;
            this.Definitions = definitions;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("target", IsRequired = true)]
        public string Target
        {
            get
            {
                return (string)this["target"];
            }

            set
            {
                this["target"] = value;
            }
        }

        [ConfigurationProperty("username", IsRequired = true)]
        public string Username
        {
            get
            {
                return (string)this["username"];
            }

            set
            {
                this["username"] = value;
            }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }

            set
            {
                this["password"] = value;
            }
        }

        [ConfigurationProperty("definitions", IsRequired = true)]
        public string Definitions
        {
            get
            {
                return (string)this["definitions"];
            }

            set
            {
                this["definitions"] = value;
            }
        }   

    }

    public class modsicConfiguration : ConfigurationElement
    {
        public modsicConfiguration() { }

        public modsicConfiguration(string address, string username, string password)
        {
            this.address    = address;
            this.username   = username;
            this.password   = password;
        }
        
        [ConfigurationProperty("address", IsRequired = true)]
        public string address
        {
            get
            {
                return (string)this["address"];
            }
            set
            {
                this["address"] = value.ToString();
            }
        }

        [ConfigurationProperty("username", IsRequired = true)]
        public string username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = value.ToString();
            }
        }
        
        [ConfigurationProperty("password", IsRequired = true)]
        public string password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value.ToString();
            }
        }
    }
}
