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

using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;


namespace Modulo.Collect.Probe.Windows.LockoutPolicy
{
    /// <summary>
    /// The lockoutpolicy_object element is used by a lockout policy test to define those objects to evaluated based on a specified state. 
    /// There is actually only one object relating to lockout policy and this is the system as a whole. 
    /// Therefore, there are no child entities defined. 
    /// Any OVAL Test written to check lockout policy will reference the same lockoutpolicy_object which is basically an empty object element.
    /// </summary>
    [ProbeCapability(OvalObject = "lockoutpolicy", PlataformName = FamilyEnumeration.windows)]
    class LockoutPolicyProber : ProbeBase, IProbe
    {
        private TargetInfo TargetInfo;


        protected override set GetSetElement(OVAL.Definitions.ObjectType objectType)
        {
            return null;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
                base.ObjectCollector = new LockoutPolicyObjectCollector() { TargetHostName = TargetInfo.GetAddress() };

            if (base.ItemTypeGenerator == null)
                base.ItemTypeGenerator = new LockoutPolicyItemTypeGenerator();
        }


        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<lockoutpolicy_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
