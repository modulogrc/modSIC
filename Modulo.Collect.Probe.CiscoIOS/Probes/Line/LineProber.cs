/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Ios;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Ios;

namespace Modulo.Collect.Probe.CiscoIOS.Probes.Line
{
    [ProbeCapability(OvalObject="line", PlataformName=FamilyEnumeration.ios)]
    public class LineProber: ProbeBase, IProbe
    {
        private TargetInfo TargetInfo;

        protected override set GetSetElement(OVAL.Definitions.ObjectType objectType)
        {
            var itemsElement = ((line_object)objectType).Items;
            return itemsElement.OfType<set>().SingleOrDefault();
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = base.ConnectionManager.Connect<CiscoIOSConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ItemTypeGenerator == null)
                base.ItemTypeGenerator = new LineItemTypeGenerator();

            if (base.ObjectCollector == null)
            {
                var telnetConnection = ((CiscoIOSConnectionProvider)ConnectionProvider).TelnetConnection;
                base.ObjectCollector = new LineObjectCollector(telnetConnection);
            }
        }

        protected override IEnumerable<OVAL.Definitions.ObjectType> GetObjectsOfType(IEnumerable<OVAL.Definitions.ObjectType> objectTypes)
        {
            throw new NotImplementedException();
        }

        protected override OVAL.SystemCharacteristics.ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            var messageType = new MessageType() { level = MessageLevelEnumeration.error, Value = errorMessage };
            return new OVAL.SystemCharacteristics.Ios.line_item() { status = StatusEnumeration.error, message = new MessageType[] { messageType } };
        }
    }
}
