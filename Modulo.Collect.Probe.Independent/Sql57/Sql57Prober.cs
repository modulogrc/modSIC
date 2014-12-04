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
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Sql57;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.Independent.Sql
{
    [ProbeCapability(OvalObject="sql57", PlataformName=FamilyEnumeration.undefined)]
    public class Sql57Prober: ProbeBase, IProbe
    {
        protected override set GetSetElement(OVAL.Definitions.ObjectType objectType)
        {
            return ((sql57_object)objectType).GetSetElement();
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            // For this probe the connection logic must be done in Object Collector.
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
                base.ObjectCollector = new Sql57ObjectCollector(new Common.Sql.SqlQueryProvider());

            if (base.ItemTypeGenerator == null)
                base.ItemTypeGenerator = new Sql57ItemTypeGenerator();
        }

        protected override IEnumerable<OVAL.Definitions.ObjectType> GetObjectsOfType(
            IEnumerable<OVAL.Definitions.ObjectType> objectTypes)
        {
            throw new NotImplementedException();
        }

        protected override OVAL.SystemCharacteristics.ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            var messageType = new MessageType() { level = MessageLevelEnumeration.error, Value = errorMessage };
            return new sql57_item() { status = StatusEnumeration.error, message = new MessageType[] { messageType  } };
        }
    }
}
