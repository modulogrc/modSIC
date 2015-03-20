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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Windows.File.Helpers;
using Modulo.Collect.Probe.Windows.WMI;

namespace Modulo.Collect.Probe.Windows.File
{
    public enum AccessTypes : uint { ALLOWED = 0, DENIED = 1, ALLOWED_OBJECT = 5, DENIED_OBJECT = 6 };

    public class FileEffectiveRightsObjectCollector : BaseObjectCollector
    {
        public WmiDataProvider WmiDataProvider { get; set; }

        public WindowsSecurityDescriptorDisassembler daclDisassembler { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            var users = this.WmiDataProvider.SearchWmiObjects("Win32_Account", null);

            var values = new List<String>();
            foreach (var user in users)
            {
                var wmiProperty = parameters.ContainsKey("ReturnSID") ? "SID" : "Caption";
                var sidProperty = user.GetValueOf(wmiProperty);
                var sidValue = (sidProperty == null) ? string.Empty : sidProperty.ToString();

                values.Add(sidValue);
            }

            return values;
        }

        public virtual bool IsThereUserACLInFileSecurityDescriptor(string path, string filename, string userSID)
        {
            var filepath = Path.Combine(path, filename);
            var invokeMethodInfo = CreateInvokeMethodInfo(filepath);
            object managementACEs = null;
            try
            {
                managementACEs = this.WmiDataProvider.InvokeMethodByWmiPath(invokeMethodInfo);
            }
            catch
            {
                return false;
            }

            if (managementACEs == null)
                return false;

            var item = new fileeffectiverights_item() { trustee_name = OvalHelper.CreateItemEntityWithStringValue(userSID) };
            try
            {
                FillCollectedItemFromUserWinACEs(item, managementACEs);
                return true;
            }
            catch (ACLNotFoundException)
            {
                return false;
            }
            catch (UserNotFoundException)
            {
                return false;
            }
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var fileEffectiveRightsItem = (fileeffectiverights_item)systemItem;
            var filepath = Path.Combine(fileEffectiveRightsItem.path.Value, fileEffectiveRightsItem.filename.Value);            
            var trusteeName = fileEffectiveRightsItem.trustee_name.Value;
            var invokeMethodInfo = this.CreateInvokeMethodInfo(filepath);

            try
            {
                var collectedFileEffectiveUserRights = this.WmiDataProvider.InvokeMethodByWmiPath(invokeMethodInfo);
                this.FillCollectedItemFromUserWinACEs(fileEffectiveRightsItem, collectedFileEffectiveUserRights);
            }
            catch (InvalidInvokeMethodFilterException)
            {
                SetDoesNotExistStatusForItemType((fileeffectiverights_item)systemItem, filepath);
            }
            catch (ACLNotFoundException)
            {
                SetDoesNotExistStatusForItemType((fileeffectiverights_item)systemItem, trusteeName);
            }
            catch (UserNotFoundException)
            {
                SetDoesNotExistStatusForItemType((fileeffectiverights_item)systemItem, trusteeName);
            }
            catch (InvalidInvokeMethodException)
            {
                SetDoesNotExistStatusForItemType((fileeffectiverights_item)systemItem, filepath);
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }


        private WmiInvokeMethodInfo CreateInvokeMethodInfo(string completeFilePath)
        {
            var filepath = completeFilePath.Trim();
            if (filepath.Last().Equals('\\'))
                filepath = filepath.Substring(0, filepath.Length - 1);

            return new WmiInvokeMethodInfo()
            {
                ClassName = "Win32_LogicalFileSecuritySetting",
                MethodName = "GetSecurityDescriptor",
                PathName = "path",
                PathValue = filepath
            };
        }

        private void FillCollectedItemFromUserWinACEs(
            fileeffectiverights_item fileEffectiveRightsItem, 
            object managementWinACEs)
        {
            var userTrusteeName = fileEffectiveRightsItem.trustee_name.Value;
            var daclDisassembler = new WindowsSecurityDescriptorDisassembler(SecurityDescriptorType.DACL);
            var userDACLs = daclDisassembler.GetSecurityDescriptorsFromManagementObject(managementWinACEs, userTrusteeName, this.WmiDataProvider);
            var userEffectiveRights = this.CalculateUserEffectiveRightsForItem(userDACLs);
            if (userEffectiveRights == null)
                throw new UserNotFoundException();
            
            this.AdjustGenericRights(userEffectiveRights);
            
            fileEffectiveRightsItem.trustee_name = null;
            fileEffectiveRightsItem.trustee_sid = OvalHelper.CreateItemEntityWithStringValue(userEffectiveRights.Trustee.SIDString);
            #region Setting File Effective Rights Entities
            fileEffectiveRightsItem.access_system_security = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.ACCESS_SYSTEM_SECURITY);
            fileEffectiveRightsItem.file_append_data = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_APPEND_DATA);
            fileEffectiveRightsItem.file_delete_child = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_DELETE_CHILD);
            fileEffectiveRightsItem.file_execute = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_EXECUTE);
            fileEffectiveRightsItem.file_read_attributes = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_READ_ATTRIBUTES);
            fileEffectiveRightsItem.file_read_data = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_READ_DATA);
            fileEffectiveRightsItem.file_read_ea = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_READ_EA);
            fileEffectiveRightsItem.file_write_attributes = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_WRITE_ATTRIBUTES);
            fileEffectiveRightsItem.file_write_data = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_WRITE_DATA);
            fileEffectiveRightsItem.file_write_ea = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.FILE_WRITE_EA);
            fileEffectiveRightsItem.generic_all = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.GENERIC_ALL);
            fileEffectiveRightsItem.generic_execute = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.GENERIC_EXECUTE);
            fileEffectiveRightsItem.generic_read = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.GENERIC_READ);
            fileEffectiveRightsItem.generic_write = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.GENERIC_WRITE);
            fileEffectiveRightsItem.standard_delete = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.DELETE);
            fileEffectiveRightsItem.standard_read_control = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.READ_CONTROL);
            fileEffectiveRightsItem.standard_synchronize = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.SYNCHRONIZE);
            fileEffectiveRightsItem.standard_write_dac = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.WRITE_DAC);
            fileEffectiveRightsItem.standard_write_owner = OvalHelper.CreateBooleanEntityItemFromBoolValue(userEffectiveRights.WRITE_OWNER);
            #endregion
        }

        private void AdjustGenericRights(WMIWinACE userEffectiveRights)
        {
            userEffectiveRights.GENERIC_READ =
                userEffectiveRights.READ_CONTROL ||
                userEffectiveRights.FILE_READ_ATTRIBUTES ||
                userEffectiveRights.FILE_READ_DATA ||
                userEffectiveRights.FILE_READ_EA;

            userEffectiveRights.GENERIC_WRITE =
                userEffectiveRights.WRITE_OWNER ||
                userEffectiveRights.WRITE_DAC ||
                userEffectiveRights.FILE_WRITE_ATTRIBUTES ||
                userEffectiveRights.FILE_WRITE_DATA || 
                userEffectiveRights.FILE_APPEND_DATA ||
                userEffectiveRights.FILE_WRITE_EA;

            userEffectiveRights.GENERIC_EXECUTE = userEffectiveRights.FILE_EXECUTE;

            userEffectiveRights.GENERIC_ALL =
                userEffectiveRights.GENERIC_READ ||
                userEffectiveRights.GENERIC_WRITE ||
                userEffectiveRights.GENERIC_EXECUTE;
        }

        private WMIWinACE CalculateUserEffectiveRightsForItem(IEnumerable<WMIWinACE> userDACLs)
        {
            var denyRightDACL = userDACLs.Where(dacl => dacl.AceType == (uint)AccessTypes.DENIED).FirstOrDefault();
            var grantRightDACL = userDACLs.Where(dacl => dacl.AceType == (uint)AccessTypes.ALLOWED).FirstOrDefault();
            WMIWinACE userEffectiveRights;

            var thereIsNoDACL = ((denyRightDACL == null) && (grantRightDACL == null));
            if (thereIsNoDACL)
                return null;

            if (!((denyRightDACL == null) || (grantRightDACL == null)))
            {
                userEffectiveRights = this.CreateEffectiveRightsFromGrantAndDenyDACLsCombination(denyRightDACL, grantRightDACL);
                userEffectiveRights.Trustee = this.CreateWinTrusteeFromSource(grantRightDACL.Trustee);
            }
            else if (denyRightDACL == null)
            {
                userEffectiveRights = grantRightDACL;
                userEffectiveRights.Trustee = this.CreateWinTrusteeFromSource(grantRightDACL.Trustee);
            }
            else
            {
                denyRightDACL.ReversePermissions();
                userEffectiveRights = denyRightDACL;
                userEffectiveRights.Trustee = this.CreateWinTrusteeFromSource(denyRightDACL.Trustee);
            }

            return userEffectiveRights;
        }

        private WMIWinACE CreateEffectiveRightsFromGrantAndDenyDACLsCombination(WMIWinACE denyDACL, WMIWinACE grantDACL)
        {
            var effectiveDACL = new WMIWinACE();
            effectiveDACL.ACCESS_SYSTEM_SECURITY = denyDACL.ACCESS_SYSTEM_SECURITY ? false : grantDACL.ACCESS_SYSTEM_SECURITY;
            effectiveDACL.DELETE = denyDACL.DELETE ? false : grantDACL.DELETE;
            effectiveDACL.FILE_ADD_FILE = denyDACL.FILE_ADD_FILE ? false : grantDACL.FILE_ADD_FILE;
            effectiveDACL.FILE_ADD_SUBDIRECTORY = denyDACL.FILE_ADD_SUBDIRECTORY ? false : grantDACL.FILE_ADD_SUBDIRECTORY;
            effectiveDACL.FILE_APPEND_DATA = denyDACL.FILE_APPEND_DATA ? false : grantDACL.FILE_APPEND_DATA;
            effectiveDACL.FILE_DELETE_CHILD = denyDACL.FILE_DELETE_CHILD ? false : grantDACL.FILE_DELETE_CHILD;
            effectiveDACL.FILE_EXECUTE = denyDACL.FILE_EXECUTE ? false : grantDACL.FILE_EXECUTE;
            effectiveDACL.GENERIC_EXECUTE = denyDACL.GENERIC_EXECUTE ? false : grantDACL.GENERIC_EXECUTE;
            effectiveDACL.GENERIC_READ = denyDACL.GENERIC_READ ? false : grantDACL.GENERIC_READ;
            effectiveDACL.GENERIC_WRITE = denyDACL.GENERIC_WRITE ? false : grantDACL.GENERIC_WRITE;
            effectiveDACL.GENERIC_ALL = denyDACL.GENERIC_ALL ? false : grantDACL.GENERIC_ALL;
            effectiveDACL.FILE_LIST_DIRECTORY = denyDACL.FILE_LIST_DIRECTORY ? false : grantDACL.FILE_LIST_DIRECTORY;
            effectiveDACL.FILE_READ_ATTRIBUTES = denyDACL.FILE_READ_ATTRIBUTES ? false : grantDACL.FILE_READ_ATTRIBUTES;
            effectiveDACL.FILE_READ_DATA = denyDACL.FILE_READ_DATA ? false : grantDACL.FILE_READ_DATA;
            effectiveDACL.FILE_READ_EA = denyDACL.FILE_READ_EA ? false : grantDACL.FILE_READ_EA;
            effectiveDACL.FILE_TRAVERSE = denyDACL.FILE_TRAVERSE ? false : grantDACL.FILE_TRAVERSE;
            effectiveDACL.FILE_WRITE_ATTRIBUTES = denyDACL.FILE_WRITE_ATTRIBUTES ? false : grantDACL.FILE_WRITE_ATTRIBUTES;
            effectiveDACL.FILE_WRITE_DATA = denyDACL.FILE_WRITE_DATA ? false : grantDACL.FILE_WRITE_DATA;
            effectiveDACL.FILE_WRITE_EA = denyDACL.FILE_WRITE_EA ? false : grantDACL.FILE_WRITE_EA;
            effectiveDACL.READ_CONTROL = denyDACL.READ_CONTROL ? false : grantDACL.READ_CONTROL;
            effectiveDACL.SYNCHRONIZE = denyDACL.SYNCHRONIZE ? false : grantDACL.SYNCHRONIZE;
            effectiveDACL.WRITE_DAC = denyDACL.WRITE_DAC ? false : grantDACL.WRITE_DAC;
            effectiveDACL.WRITE_OWNER = denyDACL.WRITE_OWNER ? false : grantDACL.WRITE_OWNER;
            // Like OvalDI, the "Generic All" permission is equal to "File Read Data" permission. It needs to be reviewed.

            return effectiveDACL;
        }

        private WMIWinTrustee CreateWinTrusteeFromSource(WMIWinTrustee sourceTrustee)
        {
            return new WMIWinTrustee()
            {
                Domain = sourceTrustee.Domain,
                Name = sourceTrustee.Name,
                SID = sourceTrustee.SID,
                SidLength = sourceTrustee.SidLength,
                SIDString = sourceTrustee.SIDString
            };
        }
    }

    public class UserNotFoundException : Exception { }
}
