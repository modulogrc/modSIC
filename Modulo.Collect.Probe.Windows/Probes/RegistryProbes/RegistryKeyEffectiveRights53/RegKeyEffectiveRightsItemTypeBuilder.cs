/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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

using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.File.Helpers;

namespace Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53
{
    public class RegKeyEffectiveRightsItemTypeBuilder: ItemTypeBuilderBase
    {

        public RegKeyEffectiveRightsItemTypeBuilder(ItemType newItemType): base(newItemType) { }

        public override void FillItemTypeWithData(object collectedData)
        {
            WMIWinACE systemData = (WMIWinACE)collectedData;
            regkeyeffectiverights_item buildingItemType = (regkeyeffectiverights_item)base.BuildingItemType;
            buildingItemType.access_system_security = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.ACCESS_SYSTEM_SECURITY);
            buildingItemType.standard_delete = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.DELETE);
            buildingItemType.standard_read_control = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.READ_CONTROL);
            buildingItemType.standard_synchronize = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.SYNCHRONIZE);
            buildingItemType.standard_write_dac = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.WRITE_DAC);
            buildingItemType.standard_write_owner = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.WRITE_OWNER);
            buildingItemType.generic_all = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.GENERIC_ALL);
            buildingItemType.generic_execute = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.GENERIC_EXECUTE);
            buildingItemType.generic_read = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.GENERIC_READ);
            buildingItemType.generic_write = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.GENERIC_WRITE);
            buildingItemType.key_create_link = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_CREATE_LINK);
            buildingItemType.key_create_sub_key = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_CREATE_SUB_KEY);
            buildingItemType.key_enumerate_sub_keys = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_ENUMERATE_SUB_KEYS);
            buildingItemType.key_notify = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_NOTIFY);
            buildingItemType.key_query_value = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_QUERY_VALUE);
            buildingItemType.key_set_value = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_SET_VALUE);
            buildingItemType.key_wow64_32key = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_WOW64_32KEY);
            buildingItemType.key_wow64_64key = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_WOW64_64KEY);
        }

        public void SetItemTypeEntityStatus(EntityItemStringType entityItemStringType, StatusEnumeration statusEnumeration)
        {
            entityItemStringType.status = statusEnumeration;
        }
    }
}
