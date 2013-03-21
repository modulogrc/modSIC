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
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.SystemCharacteristics.Linux;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Unix.SSHCollectors;

namespace Modulo.Collect.Probe.Linux.RPMInfo
{
    public class RPMInfoObjectCollector : BaseObjectCollector
    {
        public RPMInfoCollector RPMInfosCollector { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            this.CreateRPMInfosCollectorInstance();

            var rpmInfoItem = (rpminfo_item)systemItem;
            try
            {
                var collectedVariableValue = this.TryToCollectRPMInfo(rpmInfoItem.name.Value);
                
                rpmInfoItem.arch = OvalHelper.CreateItemEntityWithStringValue(collectedVariableValue.Arch);
                rpmInfoItem.epoch = new rpminfo_itemEpoch() { Value = collectedVariableValue.Epoch.ToString() };
                rpmInfoItem.release = new rpminfo_itemRelease() { Value = collectedVariableValue.Release };
                rpmInfoItem.version = new rpminfo_itemVersion() { Value = collectedVariableValue.Version};
                rpmInfoItem.evr = new EntityItemEVRStringType() { Value = collectedVariableValue.Evr };
            }
            catch (PackageNotInstalledException)
            {
                base.SetDoesNotExistStatusForItemType(systemItem, rpmInfoItem.name.Value);
            }
            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }


        private LinuxPackageInfo TryToCollectRPMInfo(string packageName)
        {
            LinuxPackageInfo variableValue = this.RPMInfosCollector.GetTargetRPMByPackageName(packageName);

            if (variableValue == null)
                throw new PackageNotInstalledException();

            return variableValue;
        }

        private void CreateRPMInfosCollectorInstance()
        {
            if (this.RPMInfosCollector == null)
                this.RPMInfosCollector = new RPMInfoCollector();
        }
    }
}
