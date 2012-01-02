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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.WMI;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common.Exceptions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Windows.Providers;

namespace Modulo.Collect.Probe.Windows.AccessToken
{
    /// <summary>
    /// The accesstoken_object element is used by an access token test to define the object to be evaluated. 
    /// Each object extends the standard ObjectType as definied in the oval-definitions-schema and one should refer to the ObjectType description for more information. 
    /// The common set element allows complex objects to be created using filters and set logic. 
    /// Again, please refer to the description of the set element in the oval-definitions-schema.
    /// 
    /// An accesstoken_object consists of a single security principal that identifies user, group, or computer account that is associated with the token.
    /// </summary>
    [ProbeCapability(OvalObject = "accesstoken", PlataformName = FamilyEnumeration.windows)]
    public class AccessTokenProber : ProbeBase, IProbe
    {
        private const string WMI_OPENING_ERROR_MESSAGE = "[AccessTokenProber] - An error occurred while trying to open WMI Connection: '{0}'\r\nManagement path: '{1}'";

        private TargetInfo TargetInfo;
        
        public WMIConnectionProvider WMIConnectionProvider { get; set; }


        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            var setElement = ((accesstoken_object)objectType).Items.ElementAt(0);
            return setElement is set ? (set)setElement : null;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = base.ConnectionManager.Connect<StraightNetworkConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
                base.ObjectCollector = new AccessTokenObjectCollector() { TargetHostName = TargetInfo.GetAddress() };

            if (this.WMIConnectionProvider == null)
            {
                this.WMIConnectionProvider = new WMIConnectionProvider("cimv2");
                this.OpenWmiConnection();
            }

            this.CreateItemTypeGeneratorInstance();
        }



        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<accesstoken_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new accesstoken_item() { message = PrepareErrorMessage(errorMessage), status = StatusEnumeration.error };
        }


        private void CreateItemTypeGeneratorInstance()
        {
            if (base.ItemTypeGenerator == null)
            {
                var newWmiDataProvider = new WmiDataProvider(this.WMIConnectionProvider.ConnectionScope);
                var newAccountProvider = new WindowsUsersProvider(newWmiDataProvider, this.TargetInfo);
                base.ItemTypeGenerator = new AccessTokenItemTypeGenerator(newAccountProvider);
            }
        }

        private void OpenWmiConnection()
        {
            try
            {
                this.WMIConnectionProvider.Connect(this.TargetInfo);
            }
            catch (Exception ex)
            {
                var managementPath = string.Empty;
                var connectionScope = this.WMIConnectionProvider.ConnectionScope;
                
                var isConnectionScopePathDefined = ((connectionScope != null) && (connectionScope.Path != null) && (!string.IsNullOrEmpty(connectionScope.Path.Path)));
                if (isConnectionScopePathDefined)
                        managementPath = connectionScope.Path.Path;

                throw new ProbeException(string.Format(WMI_OPENING_ERROR_MESSAGE, ex.Message, managementPath));
            }
        }
    }
}
