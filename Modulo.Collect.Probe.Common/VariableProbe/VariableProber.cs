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
using Modulo.Collect.Probe.Common.BasicClasses;

using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Common.VariableProbe
{
    [ProbeCapability(OvalObject = "variable", PlataformName = FamilyEnumeration.undefined)]
    public class VariableProber : ProbeBase, IProbe
    {
        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            var variableObject = (variable_object)objectType;
            return variableObject.HasTheSetElement() ? (set)variableObject.Items.OfType<set>().First() : null;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            // is not application for this probe.
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
                base.ObjectCollector = new VariableObjectCollector();

            if (base.ItemTypeGenerator == null)
                base.ItemTypeGenerator = new VariableItemTypeGenerator();
        }

        /// <summary>
        /// Collects the information for variableProbes. The variable prober not needs a real systemDataSource.
        /// The SystemDataSource implementation for this prober, is only for compatibility with the architecture.
        /// Because this, is necessary configure a systemDataSource with the set of variables for the creation of 
        /// Items.
        /// </summary>
        /// <param name="collectInfo">The collect info.</param>
        /// <returns></returns>
        protected override ProbeResultBuilder CollectInformation(CollectInfo collectInfo)
        {
            ((VariableObjectCollector)this.ObjectCollector).VariableEvaluated = collectInfo.Variables;
            return base.CollectInformation(collectInfo);
        }


        private variable_item CreateVariableItem(string variableReference)
        {
            return new variable_item() { var_ref = new EntityItemVariableRefType() { Value = variableReference } };
        }
        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<variable_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new variable_item() { status = StatusEnumeration.error, message = PrepareErrorMessage(errorMessage) };
        }
    }
}
