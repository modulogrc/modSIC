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
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Windows.File.Helpers;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.Probe.Windows.Registry;
using Modulo.Collect.OVAL.Common;
using System.Text.RegularExpressions;

namespace Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53
{
    public class RegKeyEffectiveRightsObjectCollector: BaseObjectCollector
    {
        private const string LOG_START_COLLECTING = "Collecting effective rights for {0} user on {1} registry key.";
        private WindowsSecurityDescriptorDisassembler DaclDisassembler;

        public TargetInfo TargetInfo { get; set; }

        public WmiDataProvider WmiDataProvider { get; set; }

        public AccessControlListProvider AccessControlListProvider { get; set; }


        public RegKeyEffectiveRightsObjectCollector()
        {
            this.DaclDisassembler = new WindowsSecurityDescriptorDisassembler(SecurityDescriptorType.DACL);
        }

        /// <summary>
        /// Enumerates the sub keys of a given registry key.
        /// </summary>
        /// <param name="parameters">A Dictionary that must be contains the follow Key Pairs:
        /// [RegistryHive] = an object of Microsoft.Win32.RegistryHive
        /// [RegistryKey] = a string with the path (without hive) of registry key which the sub keys will be returned. 
        /// </param>
        /// <returns>Returns a list of string with the names of found sub keys. Each item does not contain the path of subkey, only the name is returned.</returns>
        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            try
            {
                RegistryHive hive = (RegistryHive)parameters["RegistryHive"];
                string registryKey = parameters["RegistryKey"].ToString();

                RegistryKey remoteKey = RegistryKey.OpenRemoteBaseKey(hive, this.TargetInfo.GetAddress()).OpenSubKey(registryKey);
                return remoteKey.GetSubKeyNames();
            }
            catch (Exception ex)
            {
                string ERROR_MSG_TEMPLATE = "[RegKeyEffectiveRightsSystemDataSource] - An error occurred while trying to connect on remote registry key: '{0}'";
                throw new Exception(string.Format(ERROR_MSG_TEMPLATE, ex.Message));
            }
        }

        public virtual IEnumerable<String> SearchUserTrusteeSIDs()
        {
            var wqlResult = this.WmiDataProvider.ExecuteWQL(new WQLBuilder().WithWmiClass("Win32_Account").Build());
            return wqlResult.Select(x => x.GetValueOf("SID").ToString());
        }

        public virtual bool IsThereDACLOnRegistryKeyForUser(string registryHive, string registryKey, string userSID)
        {
            var hiveID = ((RegistryHive)RegistryHelper.GetHiveKeyIdFromHiveName(registryHive)).ToString();
            return
                this.AccessControlListProvider
                    .IsThereDACLOnRegistryKeyForUser(this.TargetInfo, hiveID, registryKey, userSID);
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            if (systemItem.status.Equals(StatusEnumeration.notcollected))
            {
                var itemTypeBuilder = new RegKeyEffectiveRightsItemTypeBuilder((regkeyeffectiverights_item)systemItem);
                var hive = ((regkeyeffectiverights_item)systemItem).hive;
                var registryKey = ((regkeyeffectiverights_item)systemItem).key;
                var trusteeSID = ((regkeyeffectiverights_item)systemItem).trustee_sid;

                try
                {
                    base.ExecutionLogBuilder.AddInfo(string.Format(LOG_START_COLLECTING, trusteeSID.Value, registryKey.Value));
                    var collectedDACL = this.GetRegistryKeyACLForUser(hive.Value, registryKey.Value, trusteeSID.Value);
                    itemTypeBuilder.FillItemTypeWithData(collectedDACL);

                    systemItem.status = StatusEnumeration.exists;
                }
                catch (UserDACLNotFoundOnRegistryException)
                {
                    SetDoesNotExistStatusForItemType(systemItem, trusteeSID.Value);
                    itemTypeBuilder.SetItemTypeEntityStatus(trusteeSID, StatusEnumeration.doesnotexist);
                }
                catch (RegistryKeyEffectiveRightsNotFoundException)
                {
                    SetDoesNotExistStatusForItemType(systemItem, Path.Combine(hive.Value, registryKey.Value));
                    itemTypeBuilder.SetItemTypeEntityStatus(hive, StatusEnumeration.doesnotexist);
                    itemTypeBuilder.SetItemTypeEntityStatus(registryKey, StatusEnumeration.doesnotexist);
                }
                catch (RegistryKeyEffectiveRightsAccessDenied regKeyAccessDeniedException)
                {
                    itemTypeBuilder.SetItemTypeEntityStatus(hive, StatusEnumeration.error);
                    itemTypeBuilder.SetItemTypeEntityStatus(registryKey, StatusEnumeration.error);
                    throw new Exception(regKeyAccessDeniedException.Message);
                }

                itemTypeBuilder.BuildItemType();
                return new ItemTypeHelper().CreateCollectedItemsWithOneItem(itemTypeBuilder.BuiltItemType, BuildExecutionLog());
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        public IEnumerable<ItemType> CollectItemsApplyingOperation(string regHive, string regKey, string sidEntityValue, OperationEnumeration sidEntityOperation)
        {
            Dictionary<string, uint> allUsersDACL = null;
            try
            {
                var hiveID = RegistryHelper.GetRegistryHiveFromHiveName(regHive);//(RegistryHive)RegistryHelper.GetHiveKeyIdFromHiveName(regHive);
                allUsersDACL = AccessControlListProvider.GetRegKeyDACLs(this.TargetInfo, hiveID.ToString(), regKey);
            }
            catch (RegistryKeyEffectiveRightsNotFoundException)
            {
                var newNotExistsItem = new regkeyeffectiverights_item() { status = StatusEnumeration.doesnotexist };
                newNotExistsItem.hive = new EntityItemRegistryHiveType() { Value = regHive };
                newNotExistsItem.key = new EntityItemStringType() { Value = regKey, status = StatusEnumeration.doesnotexist };
                return new ItemType[] { newNotExistsItem }; 
            }
            catch (RegistryKeyEffectiveRightsAccessDenied regKeyAccessDeniedException)
            {
                var messageType = new MessageType() { level = MessageLevelEnumeration.error, Value = regKeyAccessDeniedException.Message };
                var newErrorItem =
                    new regkeyeffectiverights_item()
                    {
                        hive = new EntityItemRegistryHiveType() { Value = regHive },
                        key = new EntityItemStringType() { Value = regKey },
                        status = StatusEnumeration.error,
                        message = new MessageType[] { messageType }
                    };

                return new ItemType[] { newErrorItem }; 
            }


            var collectedItems = new List<ItemType>();
            foreach (var userDacl in allUsersDACL)
            {
                var userSid = userDacl.Key;
                var dacl = userDacl.Value;

                if (ProcessSidEntityOperation(sidEntityValue, userSid, sidEntityOperation))
                {
                    var winACE = this.DaclDisassembler.GetSecurityDescriptorFromAccessMask(dacl);
                    var newCollectedItem = CreateItemTypeFromWinACE(winACE, regHive, regKey, userSid);
                    collectedItems.Add(newCollectedItem);
                }
            }

            return collectedItems;
        }
        

        private bool ProcessSidEntityOperation(string entityValue, string collectedUser, OperationEnumeration entityOperation)
        {
            if (entityOperation == OperationEnumeration.patternmatch)
                return Regex.IsMatch(collectedUser, entityValue);

            if (entityOperation == OperationEnumeration.notequal || entityOperation == OperationEnumeration.caseinsensitivenotequal)
                return !collectedUser.Equals(entityValue, StringComparison.InvariantCultureIgnoreCase);

            return false;
        }
        


        private WMIWinACE GetRegistryKeyACLForUser(string hive, string key, string trusteeSID)
        {
            var hiveID = (RegistryHive)RegistryHelper.GetHiveKeyIdFromHiveName(hive);

            var collectedUserDACL =
                AccessControlListProvider
                    .GetRegistryKeyEffectiveRights(this.TargetInfo, hiveID, key, trusteeSID);

            var daclDissambler = new WindowsSecurityDescriptorDisassembler(SecurityDescriptorType.DACL);
            return daclDissambler.GetSecurityDescriptorFromAccessMask(collectedUserDACL);
        }

        private regkeyeffectiverights_item CreateItemTypeFromWinACE(object collectedData, string regHive, string regKey, string collectedSid)
        {
            WMIWinACE systemData = (WMIWinACE)collectedData;
            return new regkeyeffectiverights_item()
            {
                status = StatusEnumeration.exists,
                hive = new EntityItemRegistryHiveType() { Value = regHive },
                key = OvalHelper.CreateItemEntityWithStringValue(regKey),
                trustee_sid = OvalHelper.CreateItemEntityWithStringValue(collectedSid),
                access_system_security = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.ACCESS_SYSTEM_SECURITY),
                standard_delete = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.DELETE),
                standard_read_control = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.READ_CONTROL),
                standard_synchronize = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.SYNCHRONIZE),
                standard_write_dac = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.WRITE_DAC),
                standard_write_owner = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.WRITE_OWNER),
                generic_all = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.GENERIC_ALL),
                generic_execute = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.GENERIC_EXECUTE),
                generic_read = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.GENERIC_READ),
                generic_write = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.GENERIC_WRITE),
                key_create_link = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_CREATE_LINK),
                key_create_sub_key = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_CREATE_SUB_KEY),
                key_enumerate_sub_keys = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_ENUMERATE_SUB_KEYS),
                key_notify = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_NOTIFY),
                key_query_value = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_QUERY_VALUE),
                key_set_value = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_SET_VALUE),
                key_wow64_32key = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_WOW64_32KEY),
                key_wow64_64key = OvalHelper.CreateBooleanEntityItemFromBoolValue(systemData.KEY_WOW64_64KEY)
            };

        }


    }
}
