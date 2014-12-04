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
using System.Management;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Windows.AuditEventPolicy;
using Modulo.Collect.Probe.Windows.File.Helpers;
using Modulo.Collect.Probe.Windows.WMI;
using System.IO;


namespace Modulo.Collect.Probe.Windows.FileAuditedPermissions53
{

    public class FileAuditedPermissionsObjectCollector : BaseObjectCollector
    {
        private const string TRYING_TO_COLLECT_AUDITED_PERMISSIONS_LOG = "Trying to collect audited permissions for user '{0}' on file '{1}'";
        
        public WmiDataProvider WmiDataProvider { get; set; }
        
        public WindowsSecurityDescriptorDisassembler WindowsSecurityDescriptorDisassembler { get; set; }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var filepath   = string.Empty;
            var trusteeSID = string.Empty;

            try
            {
                filepath = ((fileauditedpermissions_item)systemItem).filepath.Value;
                trusteeSID = ((fileauditedpermissions_item)systemItem).trustee_sid.Value;
                base.ExecutionLogBuilder.AddInfo(string.Format(TRYING_TO_COLLECT_AUDITED_PERMISSIONS_LOG, trusteeSID, filepath));

                CreateFileAuditedPermissions53ItemType((fileauditedpermissions_item)systemItem, filepath, trusteeSID);
                var SACLs = this.CollectSecurityDescriptorForFileAndUser(filepath, trusteeSID);
                MapSACLsToFileAuditedPermissionsItem((fileauditedpermissions_item)systemItem, SACLs);
            }
            catch (InvalidInvokeMethodException)
            {
                base.SetDoesNotExistStatusForItemType(systemItem, filepath);
                this.SetAllAuditEntitiesItemToNULL((fileauditedpermissions_item)systemItem);
            }
            catch (ACLNotFoundException)
            {
                base.SetDoesNotExistStatusForItemType(systemItem, trusteeSID);
            }
            catch (Exception ex)
            {
                this.SetAllAuditEntitiesItemToEMPTY((fileauditedpermissions_item)systemItem);
                throw ex;
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private void SetAllAuditEntitiesItemToNULL(fileauditedpermissions_item fileauditedpermissionsItem)
        {
            fileauditedpermissionsItem.access_system_security = null;
            fileauditedpermissionsItem.file_append_data = null;
            fileauditedpermissionsItem.file_delete_child = null;
            fileauditedpermissionsItem.file_execute = null;
            fileauditedpermissionsItem.file_read_attributes = null;
            fileauditedpermissionsItem.file_read_data = null;
            fileauditedpermissionsItem.file_read_ea = null;
            fileauditedpermissionsItem.file_write_attributes = null;
            fileauditedpermissionsItem.file_write_data = null;
            fileauditedpermissionsItem.file_write_ea = null;
            fileauditedpermissionsItem.generic_all = null;
            fileauditedpermissionsItem.generic_execute = null;
            fileauditedpermissionsItem.generic_read = null;
            fileauditedpermissionsItem.generic_write = null;
            fileauditedpermissionsItem.standard_delete = null;
            fileauditedpermissionsItem.standard_read_control = null;
            fileauditedpermissionsItem.standard_synchronize = null;
            fileauditedpermissionsItem.standard_write_dac = null;
            fileauditedpermissionsItem.standard_write_owner = null;
        }

        private void SetAllAuditEntitiesItemToEMPTY(fileauditedpermissions_item item)
        {
            item.access_system_security = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_append_data = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_delete_child = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_execute = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_read_attributes = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_read_data = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_read_ea = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_write_attributes = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_write_data = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.file_write_ea = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.generic_all = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.generic_execute = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.generic_read = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.generic_write = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.standard_delete = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.standard_read_control = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.standard_synchronize = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.standard_write_dac = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
            item.standard_write_owner = OvalHelper.CreateAuditItemTypeWithValue(AuditEventStatus.EMPTY.ToString());
        }

        private string GetCompleteFilepath(fileauditedpermissions_item fileAuditedPermissionsItem)
        {
            if (IsFilePathDefined(fileAuditedPermissionsItem))
                return fileAuditedPermissionsItem.filepath.Value;
            else
                return Path.Combine(fileAuditedPermissionsItem.path.Value, fileAuditedPermissionsItem.filename.Value);
        }

        private bool IsFilePathDefined(fileauditedpermissions_item fileAuditedPermissionsItem)
        {
            var filepathEntity = fileAuditedPermissionsItem.filepath;
            return ((filepathEntity != null) && (!string.IsNullOrEmpty(filepathEntity.Value)));
        }

        private void MapSACLsToFileAuditedPermissionsItem(
            fileauditedpermissions_item collectedItem, IEnumerable<WMIWinACE> SACLs)
        {
            foreach (var sacl in SACLs)
            {
                sacl.CalculateFileAccessRightsFromAccessMask();

                if (sacl.ACCESS_SYSTEM_SECURITY)
                    collectedItem.access_system_security.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.DELETE)
                    collectedItem.standard_delete.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_APPEND_DATA)
                    collectedItem.file_append_data.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_DELETE_CHILD)
                    collectedItem.file_delete_child.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_EXECUTE)
                    collectedItem.file_execute.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_READ_ATTRIBUTES)
                    collectedItem.file_read_attributes.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_READ_DATA)
                    collectedItem.file_read_data.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_READ_EA)
                    collectedItem.file_read_ea.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_WRITE_ATTRIBUTES)
                    collectedItem.file_write_attributes.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_WRITE_DATA)
                    collectedItem.file_write_data.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.FILE_WRITE_EA)
                    collectedItem.file_write_ea.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.GENERIC_ALL)
                    collectedItem.generic_all.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.GENERIC_EXECUTE)
                    collectedItem.generic_execute.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.GENERIC_READ)
                    collectedItem.generic_read.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.GENERIC_WRITE)
                    collectedItem.generic_write.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.READ_CONTROL)
                    collectedItem.standard_read_control.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.SYNCHRONIZE)
                    collectedItem.standard_synchronize.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.WRITE_DAC)
                    collectedItem.standard_write_dac.Value = sacl.AuditEventPolicy.ToString();
                if (sacl.WRITE_OWNER)
                    collectedItem.standard_write_owner.Value = sacl.AuditEventPolicy.ToString();
            }
        }

        private IEnumerable<WMIWinACE> CollectSecurityDescriptorForFileAndUser(string filepath, string trusteeName)
        {
            CheckWmiDataProviderInstance();
            var wmiInvokeMethodInfo = CreateWmiInvokeMethodInfo(filepath);
            var wmiMethodResult = (ManagementBaseObject)this.WmiDataProvider.InvokeMethodByWmiPath(wmiInvokeMethodInfo);

            return this.WindowsSecurityDescriptorDisassembler.GetSecurityDescriptorsFromManagementObject(wmiMethodResult, trusteeName, this.WmiDataProvider);
        }

        private static WmiInvokeMethodInfo CreateWmiInvokeMethodInfo(string filepath)
        {
            return new WmiInvokeMethodInfo()
            {
                ClassName = "Win32_LogicalFileSecuritySetting",
                MethodName = "GetSecurityDescriptor",
                PathName = "path",
                PathValue = filepath
            };
        }

        private void CheckWmiDataProviderInstance()
        {
            if (this.WmiDataProvider == null)
            {
                var newConnectionScope = this.CreateConnectedManagementScope();
                this.WmiDataProvider = new WmiDataProvider(newConnectionScope);
            }

            if (this.WindowsSecurityDescriptorDisassembler == null)
                this.WindowsSecurityDescriptorDisassembler = new WindowsSecurityDescriptorDisassembler(SecurityDescriptorType.SACL);
        }

        private ManagementScope CreateConnectedManagementScope()
        {
            var wmiConnectionScope = new ManagementScope("root\\cimv2");
            wmiConnectionScope.Options = new ConnectionOptions() { EnablePrivileges = true };
            wmiConnectionScope.Connect();
            return wmiConnectionScope;
        }

        private void CreateFileAuditedPermissions53ItemType(fileauditedpermissions_item item, string filepath, string trusteeSID)
        {
            var defaultAuditEntityStatus = AuditEventStatus.AUDIT_NONE.ToString();
            
            item.filepath = OvalHelper.CreateItemEntityWithStringValue(filepath);
            item.path = OvalHelper.CreateItemEntityWithStringValue(Path.GetDirectoryName(filepath));
            item.filename = OvalHelper.CreateItemEntityWithStringValue(Path.GetFileName(filepath));
            item.trustee_sid = OvalHelper.CreateItemEntityWithStringValue(trusteeSID);
            item.access_system_security = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_append_data = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_delete_child = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_execute = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_read_attributes = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_read_data = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_read_ea = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_write_attributes = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_write_data = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.file_write_ea = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.generic_all = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.generic_execute = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.generic_read = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.generic_write = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.standard_delete = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.standard_read_control = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.standard_synchronize = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.standard_write_dac = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
            item.standard_write_owner = OvalHelper.CreateAuditItemTypeWithValue(defaultAuditEntityStatus);
        }
    }
}
