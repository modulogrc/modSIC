#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
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

namespace Modulo.Collect.GraphicalConsole
{
    public class ConfigurationSection : System.Configuration.ConfigurationSection
    {
        [ConfigurationProperty("server", IsDefaultCollection = false, IsRequired = false)]
        public ServerConfigurationElement server
        {
            get
            {
                return (ServerConfigurationElement)this["server"];
            }
        }

        [ConfigurationProperty("target", IsDefaultCollection = false, IsRequired = false)]
        public TargetConfigurationElement target
        {
            get
            {
                return (TargetConfigurationElement)this["target"];
            }
        }

        [ConfigurationProperty("file", IsDefaultCollection = false, IsRequired = false)]
        public FileConfigurationElement file
        {
            get
            {
                return (FileConfigurationElement)this["file"];
            }
        }
    }    

    public class ServerConfigurationElement : ConfigurationElement
    {
        public ServerConfigurationElement(string address, string username, string password, string port)
        {
            this.Address = address;
            this.Username = username;
            this.Password = password;
            this.Port = port;
        }

        public ServerConfigurationElement() { }

        [ConfigurationProperty("Address", IsRequired = true)]
        public string Address
        {
            get
            {
                return (string)this["Address"];
            }
            set
            {
                this["Address"] = value.ToString();
            }
        }

        [ConfigurationProperty("Username", IsRequired = true)]
        public string Username
        {
            get
            {
                return (string)this["Username"];
            }
            set
            {
                this["Username"] = value.ToString();
            }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
            set
            {
                this["Password"] = value.ToString();
            }
        }

        [ConfigurationProperty("Port", IsRequired = true)]
        public string Port
        {
            get
            {
                return (string)this["Port"];
            }
            set
            {
                this["Port"] = value.ToString();
            }
        }
    }

    public class TargetConfigurationElement : ConfigurationElement
    {
        public TargetConfigurationElement(string address, string username, string password, string administrativePassword, string SSHPort = null)
        {
            this.Address = address;
            this.Username = username;
            this.Password = password;
            this.AdministrativePassword = administrativePassword;
            this.SSHPort = SSHPort;
        }

        public TargetConfigurationElement() { }

        [ConfigurationProperty("Address", IsRequired = true)]
        public string Address
        {
            get
            {
                return (string)this["Address"];
            }
            set
            {
                this["Address"] = value.ToString();
            }
        }

        [ConfigurationProperty("Username", IsRequired = true)]
        public string Username
        {
            get
            {
                return (string)this["Username"];
            }
            set
            {
                this["Username"] = value.ToString();
            }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
            set
            {
                this["Password"] = value.ToString();
            }
        }

        [ConfigurationProperty("AdministrativePassword", IsRequired = true)]
        public string AdministrativePassword
        {
            get
            {
                return (string)this["AdministrativePassword"];
            }
            set
            {
                this["AdministrativePassword"] = value.ToString();
            }
        }

        [ConfigurationProperty("SSHPort", IsRequired = false)]
        public string SSHPort
        {
            get
            {
                return (string)this["SSHPort"];
            }
            set
            {
                this["SSHPort"] = value.ToString();
            }
        }
    }

    public class FileConfigurationElement : ConfigurationElement
    {
        public FileConfigurationElement(string saveFolder, string definitionFilename)
        {
            this.SaveFolder = saveFolder;
            this.DefinitionFilename = definitionFilename;   
        }

        public FileConfigurationElement() { }

        [ConfigurationProperty("SaveFolder", IsRequired = true)]
        public string SaveFolder
        {
            get
            {
                return (string)this["SaveFolder"];
            }
            set
            {
                this["SaveFolder"] = value.ToString();
            }
        }

        [ConfigurationProperty("DefinitionFilename", IsRequired = false)]
        public string DefinitionFilename
        {
            get
            {
                return (string)this["DefinitionFilename"];
            }
            set
            {
                this["DefinitionFilename"] = value.ToString();
            }
        }
    }
}
