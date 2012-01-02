using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Windows.WMI;
using System.IO;
using Modulo.Collect.Probe.Windows.File.Helpers;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.OVAL.Common.comparators;


namespace Modulo.Collect.Probe.Windows.Probes._New.FileEffectiveRights53
{
    public class FileEffectiveRights53ObjectCollector : BaseObjectCollector
    {
        private WmiDataProvider WmiProvider;
        private WindowsSecurityDescriptorDisassembler DaclDisassembler;
        private Dictionary<string, IEnumerable<fileeffectiverights_item>> Cache;
        private Dictionary<string, fileeffectiverights_item> FlatCache;

        public FileEffectiveRights53ObjectCollector(WmiDataProvider wmiDataProvider)
        {
            this.WmiProvider = wmiDataProvider;
            this.DaclDisassembler = new WindowsSecurityDescriptorDisassembler(SecurityDescriptorType.DACL);
            this.Cache = new Dictionary<string, IEnumerable<fileeffectiverights_item>>();
            this.FlatCache = new Dictionary<string, fileeffectiverights_item>();
        }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<ItemType> CollectItems(string filepath, string trusteeSidPattern, OperationEnumeration operation)
        {
            var cachedItems = this.TryToGetCollectedItemsFromCache(filepath, trusteeSidPattern);
            if (cachedItems != null)
                return cachedItems.Select(item => CloneItemType(item));

            var invokeMethodInfo = CreateInvokeMethodInfo(filepath);
            object managementACEs = null;
            try
            {
                managementACEs = WmiProvider.InvokeMethodByWmiPath(invokeMethodInfo);
            }
            catch (InvalidInvokeMethodException)
            {
                var notExistItem = CreateItemToCollect(filepath, trusteeSidPattern, StatusEnumeration.doesnotexist);
                return new ItemType[] { notExistItem };
            }

            var allUsers = DaclDisassembler.GetAllSecurityDescriptorsFromManagementObject(managementACEs).ToList();

            var usersDACLs = new Dictionary<string, List<WMIWinACE>>();
            foreach (var userACL in allUsers)
            {
                var sid = userACL.Trustee.SIDString;
                if (usersDACLs.ContainsKey(sid))
                    usersDACLs[sid].Add(userACL);
                else
                    usersDACLs.Add(sid, new WMIWinACE[] { userACL }.ToList());
            }

            var ovalComparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            var collectedItems = new List<fileeffectiverights_item>();
            foreach (var dacl in usersDACLs)
            {
                var trusteeSID = dacl.Key;
                var userDACLs = dacl.Value;

                if (ovalComparator.Compare(trusteeSID, trusteeSidPattern, operation))
                {
                    cachedItems = this.TryToGetCollectedItemsFromCache(filepath, trusteeSID);
                    if (cachedItems != null)
                    {
                        collectedItems.AddRange(cachedItems.Select(item => CloneItemType(item)));
                        continue;
                    }

                    var newCollectedItem = this.CreateItemToCollect(filepath, trusteeSID);
                    FillCollectedItem(newCollectedItem, userDACLs);
                    collectedItems.Add(newCollectedItem);
                    this.AddToFlatCache(filepath, trusteeSID, newCollectedItem);
                }
            }

            if (collectedItems.Count == 0)
            {
                var newNotExistsItem = CreateItemToCollect(filepath, trusteeSidPattern, StatusEnumeration.doesnotexist);
                collectedItems.Add(newNotExistsItem);
                this.AddToFlatCache(filepath, trusteeSidPattern, newNotExistsItem);
            }

            this.AddToCache(filepath, trusteeSidPattern, collectedItems);

            return collectedItems;
        }

        private void AddToFlatCache(string filepath, string trustee, fileeffectiverights_item collectedItem)
        {
            var cacheItemKey = String.Format("{0}#{1}", filepath, trustee);
            if (!this.FlatCache.ContainsKey(cacheItemKey))
                this.FlatCache.Add(cacheItemKey, collectedItem);
        }
        
        private void AddToCache(string filepath, string trustee, IEnumerable<fileeffectiverights_item> collectedItem)
        {
            var cacheItemKey = String.Format("{0}#{1}", filepath, trustee);
            if (!this.Cache.ContainsKey(cacheItemKey))
                this.Cache.Add(cacheItemKey, collectedItem);
        }

        private IEnumerable<fileeffectiverights_item> TryToGetCollectedItemsFromCache(string filepath, string trusteeSID)
        {
            var cacheKey = String.Format("{0}#{1}", filepath, trusteeSID);
            if (this.Cache.ContainsKey(cacheKey))
                return this.Cache[cacheKey];

            if (this.FlatCache.ContainsKey(cacheKey))
                return new fileeffectiverights_item[] { this.FlatCache[cacheKey] };

            return null;
        }

        private fileeffectiverights_item CloneItemType(fileeffectiverights_item sourceItem)
        {
            return new fileeffectiverights_item()
            {
                status = sourceItem.status,
                message = CloneItemMessages(sourceItem.message),
                filepath = CloneStringEntity(sourceItem.filepath),
                path = CloneStringEntity(sourceItem.path),
                filename = CloneStringEntity(sourceItem.filename),
                trustee_sid = CloneStringEntity(sourceItem.trustee_sid),
                trustee_name = CloneStringEntity(sourceItem.trustee_name),
                access_system_security = CloneEntity(sourceItem.access_system_security),
                file_append_data = CloneEntity(sourceItem.file_append_data),
                file_delete_child = CloneEntity(sourceItem.file_delete_child),
                file_execute = CloneEntity(sourceItem.file_execute),
                file_read_attributes = CloneEntity(sourceItem.file_read_attributes),
                file_read_data = CloneEntity(sourceItem.file_read_data),
                file_read_ea = CloneEntity(sourceItem.file_read_ea),
                file_write_attributes = CloneEntity(sourceItem.file_write_attributes),
                file_write_data = CloneEntity(sourceItem.file_write_data),
                file_write_ea = CloneEntity(sourceItem.file_write_ea),
                generic_all = CloneEntity(sourceItem.generic_all),
                generic_execute = CloneEntity(sourceItem.generic_execute),
                generic_read = CloneEntity(sourceItem.generic_read),
                generic_write = CloneEntity(sourceItem.generic_write),
                standard_delete = CloneEntity(sourceItem.standard_delete),
                standard_read_control = CloneEntity(sourceItem.standard_read_control),
                standard_synchronize = CloneEntity(sourceItem.standard_synchronize),
                standard_write_dac = CloneEntity(sourceItem.standard_write_dac),
                standard_write_owner = CloneEntity(sourceItem.standard_write_owner)
            };
        }

        private EntityItemStringType CloneStringEntity(EntityItemStringType sourceEntity)
        {
            if (sourceEntity == null)
                return null;

            return new EntityItemStringType()
            {
                datatype = sourceEntity.datatype,
                status = sourceEntity.status,
                mask = sourceEntity.mask,
                Value = sourceEntity.Value
            };
        }

        private MessageType[] CloneItemMessages(MessageType[] sourceMessagesType)
        {
            if (sourceMessagesType == null)
                return null;

            var clonedMessages = new List<MessageType>();
            foreach (var sourceMessage in sourceMessagesType)
                clonedMessages.Add(new MessageType() { level = sourceMessage.level, Value = sourceMessage.Value });
            
            return clonedMessages.ToArray();
        }

        private EntityItemBoolType CloneEntity(EntityItemBoolType sourceEntity)
        {
            if (sourceEntity == null)
                return null;

            return new EntityItemBoolType()
            {
                datatype = sourceEntity.datatype,
                status = sourceEntity.status,
                mask = sourceEntity.mask,
                Value = sourceEntity.Value
            };
        }
        

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            if (systemItem.status == StatusEnumeration.notcollected)
            {
                var filepath = ((fileeffectiverights_item)systemItem).filepath.Value;
                var trusteeSid = ((fileeffectiverights_item)systemItem).trustee_sid.Value;
                systemItem = CollectItems(filepath, trusteeSid, OperationEnumeration.equals).FirstOrDefault();
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }

        private void FillCollectedItemFromUserWinACEs(
            fileeffectiverights_item fileEffectiveRightsItem,
            IEnumerable<WMIWinACE> userDACLs)
        {
            var userTrusteeName = fileEffectiveRightsItem.trustee_name.Value;
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

        private void FillCollectedItem(fileeffectiverights_item item, IEnumerable<WMIWinACE> userDACLs)
        {
            try
            {
                FillCollectedItemFromUserWinACEs(item, userDACLs);
                item.status = StatusEnumeration.exists;
            }
            catch (InvalidInvokeMethodFilterException)
            {
                item.status = StatusEnumeration.doesnotexist;
            }
            catch (ACLNotFoundException)
            {
                item.status = StatusEnumeration.doesnotexist;
            }
            catch (UserNotFoundException)
            {
                item.status = StatusEnumeration.doesnotexist;
            }
            catch (InvalidInvokeMethodException)
            {
                item.status = StatusEnumeration.doesnotexist;
            }
            catch (Exception ex)
            {
                // ConfigureItemWithError(itemToCollect, ex.Message);
            }
        }

        private fileeffectiverights_item CreateItemToCollect(
            string filepath, string trusteeSID, StatusEnumeration status = StatusEnumeration.exists)
        {
            return new fileeffectiverights_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(filepath),
                path = OvalHelper.CreateItemEntityWithStringValue(Path.GetDirectoryName(filepath)),
                filename = OvalHelper.CreateItemEntityWithStringValue(Path.GetFileName(filepath)),
                trustee_name = OvalHelper.CreateItemEntityWithStringValue("sid")
            };
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
}
