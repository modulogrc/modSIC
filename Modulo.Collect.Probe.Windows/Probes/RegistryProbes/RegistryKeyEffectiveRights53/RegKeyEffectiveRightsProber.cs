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

using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.WMI;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;


namespace Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53
{
    /// <summary>
    /// The regkeyeffectiverights53_object element is used by a registry key effective rights test to define the objects used to evalutate against the specified state. 
    /// Each object extends the standard ObjectType as definied in the oval-definitions-schema and one should refer to the ObjectType description for more information. 
    /// The common set element allows complex objects to be created using filters and set logic.
    /// A regkeyeffectiverights53_object is defined as a combination of a Windows registry and trustee sid. 
    /// The key entity represents the registry key to be evaluated while the trustee sid represents the account (sid) to check effective rights of. 
    /// If multiple files or sids are matched by either reference, then each possible combination of registry key and sid is a matching registry key effective rights object. 
    /// In addition, a number of behaviors may be provided that help guide the collection of objects. 
    /// Please refer to the RegkeyEffectiveRights53Behaviors complex type for more information about specific behaviors.
    /// </summary>
    [ProbeCapability(OvalObject="regkeyeffectiverights53", PlataformName=FamilyEnumeration.windows)]
    public class RegKeyEffectiveRightsProber: ProbeBase, IProbe
    {
        private TargetInfo TargetInfo;


        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            return ((regkeyeffectiverights53_object)objectType).GetSetEntity();
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = base.ConnectionManager.Connect<RegKeyEffectiveRights53ConnectionProvider>(connectionContext, target);            
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var newWmiProvider = new WmiDataProvider(((RegKeyEffectiveRights53ConnectionProvider)base.ConnectionProvider).ConnectionScope);
                base.ObjectCollector = new RegKeyEffectiveRightsObjectCollector() 
                { 
                    TargetInfo = this.TargetInfo, 
                    WmiDataProvider = newWmiProvider,
                    AccessControlListProvider = AccessControlListProvider.CreateInstance()
                };
            }

            if (base.ItemTypeGenerator == null)
                base.ItemTypeGenerator =
                    new RegKeyEffectiveRightsItemTypeGenerator()
                    {
                        OperationEvaluator = new RegKeyEffectiveRightsOperationEvaluator()
                        {
                            SystemDataSource = base.ObjectCollector
                        },

                        ObjectCollector = (RegKeyEffectiveRightsObjectCollector)base.ObjectCollector
                    };
            /* { SystemDataSource = base.ObjectCollector } */;
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<regkeyeffectiverights53_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new regkeyeffectiverights_item() { status = StatusEnumeration.error, message = PrepareErrorMessage(errorMessage) };
        }
    }
}
