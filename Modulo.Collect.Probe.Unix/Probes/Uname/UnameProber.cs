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
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions.Unix;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Unix;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Unix.SSHCollectors;

namespace Modulo.Collect.Probe.Unix.Probes.Uname
{
    [ProbeCapability(OvalObject="uname", PlataformName=FamilyEnumeration.unix)]
    public class UnameProber: ProbeBase, IProbe
    {
        protected override OVAL.Definitions.set GetSetElement(OVAL.Definitions.ObjectType objectType)
        {
            return null;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            base.ConnectionProvider = base.ConnectionManager.Connect<SSHConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var unameCollector = new UnameCollector() { SSHExec = ((SSHConnectionProvider)ConnectionProvider).SSHExec };
                base.ObjectCollector = new UnameObjectCollector() { UnameCollector = unameCollector };
            }

            if (base.ItemTypeGenerator == null)
                base.ItemTypeGenerator = new UnameItemTypeGenerator();
        }

        protected override IEnumerable<OVAL.Definitions.ObjectType> GetObjectsOfType(IEnumerable<OVAL.Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<uname_object>();
        }

        protected override OVAL.SystemCharacteristics.ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return
                new uname_item()
                {
                    status = StatusEnumeration.error,
                    message = PrepareErrorMessage(errorMessage)
                };
        }

        
    }
}
