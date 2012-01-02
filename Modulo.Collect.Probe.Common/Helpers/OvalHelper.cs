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
using Modulo.Collect.OVAL.Common;

using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Defs = Modulo.Collect.OVAL.Definitions;
using SysCharacteristics = Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.OVAL.Definitions.Windows;


namespace Modulo.Collect.Probe.Common
{
    public class OvalHelper
    {
        public static EntityObjectStringType GetEntityObjectByName(string entityName, object[] allEntities, string[] allEntityNames)
        {
            for (int i = 0; i < allEntityNames.Length; i++)
                if (entityName.Equals(allEntityNames[i].ToString()))
                    return (allEntities[i] as EntityObjectStringType);
            
            return null;

        }
        
        public static EntityObjectIntType GetEntityObjectIntTypeByName(string entityName, object[] allEntities, string[] allEntityNames)
        {
            for (int i = 0; i < allEntityNames.Length; i++)
                if (entityName.Equals(allEntityNames[i].ToString()))
                    return (allEntities[i] as EntityObjectIntType);

            return null;

        }

        public static Dictionary<String, EntityObjectStringType> GetRegistryEntitiesFromObjectType(registry_object registryObject)
        {
            string hiveEntityName = registry_object_ItemsChoices.hive.ToString();
            string keyEntityName = registry_object_ItemsChoices.key.ToString();
            string nameEntityName = registry_object_ItemsChoices.name.ToString();

            object[] allEntities = registryObject.Items.ToArray();
            string[] allEntityNames = registryObject.RegistryObjectItemsElementName.Select(i => i.ToString()).ToArray<String>();

            Dictionary<String, EntityObjectStringType> registryEntities = new Dictionary<String, EntityObjectStringType>();
            registryEntities.Add(hiveEntityName, OvalHelper.GetEntityObjectByName(hiveEntityName, allEntities, allEntityNames));
            registryEntities.Add(keyEntityName, OvalHelper.GetEntityObjectByName(keyEntityName, allEntities, allEntityNames));
            registryEntities.Add(nameEntityName, OvalHelper.GetEntityObjectByName(nameEntityName, allEntities, allEntityNames));

            return registryEntities;
        }

        public static Dictionary<String, EntityObjectStringType> GetFileEntitiesFromObjectType(
            OVAL.Definitions.ObjectType objectType)
        {
            var filenameEntityName = GetFilenameEntityName(objectType);
            var filepathEntityName = GetFilepathEntityName(objectType);
            var pathEntityName = GetPathEntityName(objectType);

            object[] allEntities = GetAllItems(objectType);
            string[] allEntityNames = GetAllElementNames(objectType);

            var fileEntities = new Dictionary<String, EntityObjectStringType>();
            fileEntities.Add(filenameEntityName, OvalHelper.GetEntityObjectByName(filenameEntityName, allEntities, allEntityNames));
            fileEntities.Add(filepathEntityName, OvalHelper.GetEntityObjectByName(filepathEntityName, allEntities, allEntityNames));
            fileEntities.Add(pathEntityName, OvalHelper.GetEntityObjectByName(pathEntityName, allEntities, allEntityNames));

            return fileEntities;
        }

        private static object[] GetAllItems(OVAL.Definitions.ObjectType objectType)
        {
            var windowsFileObjectType = typeof(OVAL.Definitions.Windows.file_object);
            var unixFileObjectType = typeof(OVAL.Definitions.Unix.file_object);

            if (objectType.GetType().Equals(typeof(OVAL.Definitions.Windows.file_object)))
                return ((OVAL.Definitions.Windows.file_object)objectType).Items.ToArray();
            else if (objectType.GetType().Equals(typeof(OVAL.Definitions.Unix.file_object)))
                return ((OVAL.Definitions.Unix.file_object)objectType).Items.ToArray();
            else
                throw new ArgumentException(String.Format("This object type '{0}' is not supported.", objectType.GetType().ToString()));
        }

        private static string[] GetAllElementNames(OVAL.Definitions.ObjectType objectType)
        {
            var windowsFileObjectType = typeof(OVAL.Definitions.Windows.file_object);
            var unixFileObjectType = typeof(OVAL.Definitions.Unix.file_object);

            if (objectType.GetType().Equals(typeof(OVAL.Definitions.Windows.file_object)))
                return ((OVAL.Definitions.Windows.file_object)objectType).FileObjectItemsElementName.Select(i => i.ToString()).ToArray<String>();
            else if (objectType.GetType().Equals(typeof(OVAL.Definitions.Unix.file_object)))
                return ((OVAL.Definitions.Unix.file_object)objectType).ItemsElementName.Select(i => i.ToString()).ToArray<String>();
            else
                throw new ArgumentException(String.Format("This object type '{0}' is not supported.", objectType.GetType().ToString()));
        }

        private static string GetPathEntityName(Defs.ObjectType objectType)
        {
            var windowsFileObjectType = typeof(OVAL.Definitions.Windows.file_object);
            var unixFileObjectType = typeof(OVAL.Definitions.Unix.file_object);

            if (objectType.GetType().Equals(windowsFileObjectType))
                return file_object_ItemsChoices.path.ToString();
            else if (objectType.GetType().Equals(unixFileObjectType))
                return OVAL.Definitions.Unix.ItemsChoiceType3.path.ToString();
            else
                throw new ArgumentException(String.Format("This object type '{0}' is not supported.", objectType.GetType().ToString()));
        }

        private static string GetFilepathEntityName(Defs.ObjectType objectType)
        {
            var windowsFileObjectType = typeof(OVAL.Definitions.Windows.file_object);
            var unixFileObjectType = typeof(OVAL.Definitions.Unix.file_object);

            if (objectType.GetType().Equals(windowsFileObjectType))
                return file_object_ItemsChoices.filepath.ToString();
            else if (objectType.GetType().Equals(unixFileObjectType))
                return OVAL.Definitions.Unix.ItemsChoiceType3.filepath.ToString();
            else
                throw new ArgumentException(String.Format("This object type '{0}' is not supported.", objectType.GetType().ToString()));
        }

        private static string GetFilenameEntityName(OVAL.Definitions.ObjectType objectType)
        {
            var windowsFileObjectType = typeof(OVAL.Definitions.Windows.file_object);
            var unixFileObjectType = typeof(OVAL.Definitions.Unix.file_object);

            if (objectType.GetType().Equals(windowsFileObjectType))
                return file_object_ItemsChoices.filename.ToString();
            else if (objectType.GetType().Equals(unixFileObjectType))
                return OVAL.Definitions.Unix.ItemsChoiceType3.filename.ToString();
            else
                throw new ArgumentException(String.Format("This object type '{0}' is not supported.", objectType.GetType().ToString()));
        }
      

        public static Dictionary<String, EntityObjectStringType> GetUserObjectEntitiesFromObjectType(user_object userObject)
        {
            Dictionary<String, EntityObjectStringType> userEntities = new Dictionary<String, EntityObjectStringType>();
            userEntities.Add("user", (EntityObjectStringType)userObject.User);
            
            return userEntities;
        }

        public static Dictionary<String, EntityObjectStringType> GetFileEffectiveRightsFromObjectType(fileeffectiverights_object fileEffectiveRightsObject)
        {
            string fileNameEntityName = fileeffectiverights_object_ItemsChoices.filename.ToString();
            string pathEntityName = fileeffectiverights_object_ItemsChoices.path.ToString();
            string trusteeEntityName = fileeffectiverights_object_ItemsChoices.trustee_name.ToString();

            object[] allEntities = fileEffectiveRightsObject.Items.ToArray();
            string[] allEntityNames = fileEffectiveRightsObject.FileeffectiverightsObjectItemsElementName.Select(i => i.ToString()).ToArray<String>();

            Dictionary<String, EntityObjectStringType> fileEntities = new Dictionary<String, EntityObjectStringType>();
            fileEntities.Add(fileNameEntityName, OvalHelper.GetEntityObjectByName(fileNameEntityName, allEntities, allEntityNames));
            fileEntities.Add(pathEntityName, OvalHelper.GetEntityObjectByName(pathEntityName, allEntities, allEntityNames));
            fileEntities.Add(trusteeEntityName, OvalHelper.GetEntityObjectByName(trusteeEntityName, allEntities, allEntityNames));

            return fileEntities;
        }

        public static OperationEnumeration GetFileEntityOperation(Dictionary<String, EntityObjectStringType> allEntities)
        {
            EntityObjectStringType fileEntity;

            allEntities.TryGetValue(file_object_ItemsChoices.filepath.ToString(), out fileEntity);
            if (fileEntity == null)
                allEntities.TryGetValue(file_object_ItemsChoices.path.ToString(), out fileEntity);

            if (fileEntity.operation == OperationEnumeration.equals)
                return OperationEnumeration.caseinsensitiveequals;
            
            return fileEntity.operation;
        }


        public static string GetFullFilePathFromObjectType(Defs::ObjectType objectType)
        {
            var allEntities = 
                (objectType is file_object)
                    ? OvalHelper.GetFileEntitiesFromObjectType((file_object)objectType)
                    : OvalHelper.GetFileEffectiveRightsFromObjectType((fileeffectiverights_object)objectType);

            string fullFilePath = string.Empty;
            if (OvalHelper.IsFilePathEntityDefined(allEntities))
                fullFilePath = allEntities[file_object_ItemsChoices.filepath.ToString()].Value;

            if (string.IsNullOrEmpty(fullFilePath))
            {
                string path = allEntities[file_object_ItemsChoices.path.ToString()].Value;
                string filename = allEntities[file_object_ItemsChoices.filename.ToString()].Value;
                fullFilePath = string.Format(@"{0}\{1}", path, filename);
            }

            return fullFilePath;
        }
        
        public static bool IsOvalObjectPatternMatch(EntityObjectStringType ovalObject)
        {
            return (ovalObject.operation == OperationEnumeration.patternmatch);
        }

        public static bool IsFilePathEntityDefined(Dictionary<string, EntityObjectStringType> allFileEntities)
        {
            EntityObjectStringType filepathEntity;
            allFileEntities.TryGetValue(file_object_ItemsChoices.filepath.ToString(), out filepathEntity);

            if (filepathEntity == null)
                return false;

            return ((!string.IsNullOrEmpty(filepathEntity.var_ref)) || (!string.IsNullOrEmpty(filepathEntity.Value)));
        }
       
        public static EntityItemStringType CreateItemEntityWithStringValue(string entityValue)
        {
            return new EntityItemStringType { Value = entityValue };
        }

        public static EntityItemBoolType CreateItemEntityWithBooleanValue(string entityValue)
        {
            return new EntityItemBoolType { Value = entityValue };
        }

        /// <summary>
        /// Creates new Boolean Item Type Entity.
        /// </summary>
        /// <param name="value">The value for new Boolean Item Entity.</param>
        /// <returns>A new item type. If value is null, the entity status will be "not collected" and its value null.</returns>
        public static EntityItemBoolType CreateBooleanEntityItemFromBoolValue(bool? value)
        {
            if (value == null)
                return new EntityItemBoolType() { status = StatusEnumeration.notcollected };
            
            return new EntityItemBoolType() { Value = (bool)value ? "1" : "0" };
        }

        public static EntityItemIntType CreateItemEntityWithIntegerValue(string entityValue)
        {
            return new EntityItemIntType() { datatype = SimpleDatatypeEnumeration.@int, Value = entityValue };
        }

        public static EntityItemAuditType CreateAuditItemTypeWithValue(string auditStatusValue)
        {
            return new EntityItemAuditType() { Value = auditStatusValue };
        }

        public static EntityItemVersionType CreateVersionItemTypeWithValue(string versionValue)
        {
            return new EntityItemVersionType() { Value = versionValue };
        }

        public static EntityItemAnySimpleType CreateEntityItemAnyTypeWithValue(
            string entityValue, SimpleDatatypeEnumeration datatype = SimpleDatatypeEnumeration.@string)
        {
            return new EntityItemAnySimpleType() { Value = entityValue, datatype = datatype };
        }

    }

}
