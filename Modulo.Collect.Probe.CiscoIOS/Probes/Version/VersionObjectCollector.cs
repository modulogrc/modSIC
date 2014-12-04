/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.CiscoIOS;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics.Ios;
using Modulo.Collect.Probe.Common.Helpers;

namespace Modulo.Collect.Probe.CiscoIOS.Probes.Version
{
    public class VersionObjectCollector : BaseObjectCollector
    {
        public TelnetConnection TelnetConnection { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Common.BasicClasses.CollectedItem> collectDataForSystemItem(OVAL.SystemCharacteristics.ItemType systemItem)
        {
            CiscoIOSVersion myVersion = CiscoIOSHelper.CiscoGetVersion(TelnetConnection);

            version_item vItem = (version_item)systemItem;
            vItem.mainline_rebuild = OvalHelper.CreateItemEntityWithStringValue(myVersion.MainlineRebuild);
            vItem.major_release = OvalHelper.CreateItemEntityWithStringValue(myVersion.MajorRelease);
            vItem.major_version = OvalHelper.CreateItemEntityWithIntegerValue(myVersion.MajorVersion.ToString());
            vItem.minor_version = OvalHelper.CreateItemEntityWithIntegerValue(myVersion.MinorVersion.ToString());
            if (myVersion.Rebuild >= 0)
                vItem.rebuild = OvalHelper.CreateItemEntityWithIntegerValue(myVersion.Rebuild.ToString());
            if (myVersion.Release >= 0)
                vItem.release = OvalHelper.CreateItemEntityWithIntegerValue(myVersion.Release.ToString());
            vItem.subrebuild = OvalHelper.CreateItemEntityWithStringValue(myVersion.SubRebuild);
            vItem.train_identifier = OvalHelper.CreateItemEntityWithStringValue(myVersion.TrainIdentifier);
            vItem.train_number = OvalHelper.CreateItemEntityWithStringValue(myVersion.TrainNumber);
            vItem.version_string = new EntityItemIOSVersionType { Value = myVersion.VersionString };

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }
    }
}
