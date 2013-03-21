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
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.OVAL.SystemCharacteristics.Solaris;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Common.Extensions;
using Modulo.Collect.Probe.Unix;

namespace Modulo.Collect.Probe.Solaris.Probes.smf
{
    public class SMFObjectCollector: BaseObjectCollector
    {
        public SshCommandLineRunner CommandRunner { get; set; }

        public SolarisSmfCollector SMFCollector { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            this.CreateSMFCollectorInstance();

            var smfItem = (smf_item)systemItem;

            try
            {
                var collectedSmf = this.TryToCollectSMF(smfItem.fmri.Value);
                smfItem.service_name = OvalHelper.CreateItemEntityWithStringValue(collectedSmf.ServiceName);
                smfItem.service_state = new EntityItemSmfServiceStateType() { Value = collectedSmf.ServiceState };
                smfItem.protocol = new EntityItemSmfProtocolType() { Value = collectedSmf.Protocol };
                smfItem.server_executable = OvalHelper.CreateItemEntityWithStringValue(collectedSmf.ServerExecutable);
                smfItem.server_arguements = OvalHelper.CreateItemEntityWithStringValue(collectedSmf.ServerArgs);
                smfItem.exec_as_user = OvalHelper.CreateItemEntityWithStringValue(collectedSmf.ExecAsUser);
            }
            catch (NoSMFDataException)
            {
                ExecutionLogBuilder.AddInfo("An error occurred while trying to collect smf_object");
                smfItem.status = StatusEnumeration.error;
                smfItem.message = MessageType.FromErrorString("The fmri format is invalid.");
                smfItem.fmri.status = StatusEnumeration.error;
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(smfItem, BuildExecutionLog());
        }

        private SolarisSmfInfo TryToCollectSMF(string fmri)
        {
            var allSmfInfo = SMFCollector.GetSmfInfo(fmri);

            if (allSmfInfo == null)
                throw new NoSMFDataException();

            return allSmfInfo;
        }

        private void CreateSMFCollectorInstance()
        {
            if (this.SMFCollector == null)
                this.SMFCollector = new SolarisSmfCollector() { CommandRunner = this.CommandRunner };
        }
    }
}