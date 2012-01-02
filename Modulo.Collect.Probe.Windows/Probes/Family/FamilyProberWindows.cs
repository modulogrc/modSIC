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


using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Family;
using Modulo.Collect.Probe.Windows.WMI;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Windows.Family
{
    /// <summary>
    /// The family_object element is used by a family test to define those objects to evaluate based on a specified state. 
    /// There is actually only one object relating to family and this is the system as a whole. 
    /// Therefore, there are no child entities defined. 
    /// Any OVAL Test written to check the family will reference the same family_object which is basically an empty object element.
    /// </summary>
    [ProbeCapability(OvalObject="family", PlataformName=FamilyEnumeration.windows)]
    public class FamilyProberWindows : ProbeBase, IProbe
    {
        public TargetInfo TargetInfo { get; set; }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = this.ConnectionManager.Connect<FileConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            if (ObjectCollector == null)
                ObjectCollector = new FamilyObjectCollector() { FamilyCollector = CreateFamilyCollectorForWindows()};

            if (ItemTypeGenerator == null)
                ItemTypeGenerator = new FamilyItemTypeGenerator();
               
        }

        private IFamilyCollector CreateFamilyCollectorForWindows()
        {
            var wmiConnectionScope = ((FileConnectionProvider)ConnectionProvider).ConnectionScope;
            return new FamilyCollectorWindows() { WmiDataProvider = new WmiDataProvider(wmiConnectionScope) };
        }
                
        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            return null;
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<family_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new family_item() { status = StatusEnumeration.error, message = PrepareErrorMessage(errorMessage) };
        }
    }
}
