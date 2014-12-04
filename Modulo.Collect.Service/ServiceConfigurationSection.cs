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
using System.Text;
using System.Configuration;

namespace Modulo.Collect.Service
{
    public class ServiceConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("ravendb", IsDefaultCollection = false, IsRequired = false)]
        public RavenConfiguration ravendb
        {
            get { return (RavenConfiguration)this["ravendb"]; }
        }

        [ConfigurationProperty("schematronValidation", IsDefaultCollection = false, IsRequired = false)]
        public SchematronConfiguration schematronValidation
        {
            get { return (SchematronConfiguration)this["schematronValidation"]; }
        }

        [ConfigurationProperty("scheduler", IsDefaultCollection = false, IsRequired = false)]
        public SchedulerConfiguration scheduler
        {
            get { return (SchedulerConfiguration)this["scheduler"]; }
        }
    }

    public class RavenConfiguration : ConfigurationElement
    {
        public RavenConfiguration(Boolean webUIEnabled, int webUIPort)
        {
            this.WebUIEnabled = webUIEnabled;
            this.WebUIPort = webUIPort;
        }

        public RavenConfiguration() { }

        [ConfigurationProperty("webUIEnabled", IsRequired = true)]
        public Boolean WebUIEnabled
        {
            get { return (Boolean)this["webUIEnabled"]; }
            set { this["webUIEnabled"] = value.ToString(); }
        }

        [ConfigurationProperty("webUIPort", IsRequired = true)]
        public int WebUIPort
        {
            get { return (int)this["webUIPort"]; }
            set { this["webUIPort"] = value; }
        }
    }

    public class SchematronConfiguration : ConfigurationElement
    {
        public SchematronConfiguration(Boolean validationEnabled)
        {
            this.ValidationEnabled = validationEnabled;
        }

        public SchematronConfiguration() { }

        [ConfigurationProperty("ValidationEnabled", IsRequired = true)]
        public Boolean ValidationEnabled
        {
            get { return (Boolean)this["ValidationEnabled"]; }
            set { this["ValidationEnabled"] = value.ToString(); }
        }
    }

    public class SchedulerConfiguration : ConfigurationElement
    {
        public SchedulerConfiguration(Boolean schedulerIntervalEnabled)
        {
            this.SchedulerIntervalEnabled = schedulerIntervalEnabled;
        }

        public SchedulerConfiguration() { }

        [ConfigurationProperty("SchedulerIntervalEnabled", IsRequired = false)]
        public bool SchedulerIntervalEnabled
        {
            get { return (Boolean)this["SchedulerIntervalEnabled"]; }
            set { this["SchedulerIntervalEnabled"] = value.ToString(); }
        }

        [ConfigurationProperty("CollectionTimeoutInMinutes", IsRequired = false)]
        public int CollectionTimeoutInMinutes
        {
            get { return (int)this["CollectionTimeoutInMinutes"]; }
            set { this["CollectionTimeoutInMinutes"] = value; }
        }
    }

    

    public class ModsicConfigurationHelper
    {
        /// <summary>
        /// Check if modSIC was set to validate schematron of Oval Definitions received through collect request.
        /// Section: ServiceConfigurationSection->schematronValidation/ValidationEnabled
        /// <returns>TRUE if schematron validation is set, FALSE otherwise or schematronValidation configuration was not found in config file.</returns>
        public static Boolean IsSchematronValidationSet()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceConfigurationSection = config.GetSection("ServiceConfigurationSection") as ServiceConfigurationSection;
            if (serviceConfigurationSection != null && serviceConfigurationSection.schematronValidation != null)
                return serviceConfigurationSection.schematronValidation.ValidationEnabled;

            return false;
        }

        public static Boolean IsSchedulerIntervalSet()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceConfigurationSection = config.GetSection("ServiceConfigurationSection") as ServiceConfigurationSection;
            if (serviceConfigurationSection != null && serviceConfigurationSection.scheduler != null)
                return serviceConfigurationSection.scheduler.SchedulerIntervalEnabled;

            return false;
        }

        public static int? GetCollectionTimeout()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceConfigurationSection = config.GetSection("ServiceConfigurationSection") as ServiceConfigurationSection;
            var schedulerSectionIsDefined = (serviceConfigurationSection != null && serviceConfigurationSection.scheduler != null);
            if (schedulerSectionIsDefined)
            {
                var timeoutSection = serviceConfigurationSection.scheduler.CollectionTimeoutInMinutes;
                if (timeoutSection > 0)
                    return timeoutSection;
            }

            return null;
        }
    }
}
