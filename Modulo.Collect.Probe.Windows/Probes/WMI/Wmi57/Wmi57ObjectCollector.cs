/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Windows.Probes.WMI.Wmi57
{
    public class Wmi57ObjectCollector: BaseObjectCollector
    {
        private TargetInfo TargetInfo;

        /// <summary>
        /// This property usage is only for unit tests porposes.
        /// </summary>
        public WmiDataProvider WmiDataProvider { get; set; }

        public Wmi57ObjectCollector(TargetInfo targetInfo)
        {
            this.TargetInfo = targetInfo;
        }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var wmiItemType = ((wmi57_item)systemItem);
            var wmiDataProvider = this.GetWmiDataProvider(wmiItemType.@namespace.Value);
            var wmiResult = wmiDataProvider.ExecuteWQL(wmiItemType.wql.Value);

            if (wmiResult.Count() == 0)
            {
                wmiItemType.status = StatusEnumeration.doesnotexist;
                wmiItemType.result = new[] { new EntityItemRecordType() { status = StatusEnumeration.doesnotexist } };
            }
            else
                this.ConfigureWmiItemResult(wmiItemType, wmiResult);

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(wmiItemType, BuildExecutionLog());
        }


        private void ConfigureWmiItemResult(wmi57_item wmiItemType, IEnumerable<WmiObject> wmiResult)
        {
            wmiItemType.result = new EntityItemRecordType[wmiResult.Count()];

            int i = 0;
            foreach (var wmiObj in wmiResult)
            {
                var fields = wmiObj.GetValues();
                wmiItemType.result[i] = new EntityItemRecordType();
                wmiItemType.result[i].field = new EntityItemFieldType[fields.Count];
                int j = 0;
                foreach (var field in fields)
                {
                    var fieldName = string.IsNullOrWhiteSpace(field.Key) ? string.Empty : field.Key;
                    var fieldValue = field.Value == null ? string.Empty : field.Value.ToString();

                    wmiItemType.result[i].field[j] = new EntityItemFieldType() { name = fieldName, Value = fieldValue };
                    j++;
                }

                i++;
            }
        }
        private WmiDataProvider GetWmiDataProvider(string @namespace)
        {
            if (this.WmiDataProvider != null)
                return this.WmiDataProvider;

            var wmiNamespace = String.Format(@"\\{0}\{1}", this.TargetInfo.GetAddress(), @namespace);
            var wmiNamespacePath = new System.Management.ManagementPath(wmiNamespace);
            var wmiConnectionOptions = WmiDataProviderFactory.CreateConnectionOptions(this.TargetInfo);
            var wmiManagementScope = new System.Management.ManagementScope(wmiNamespacePath, wmiConnectionOptions);
            wmiManagementScope.Connect();

            return new WmiDataProvider(wmiManagementScope);
        }

    }
}
