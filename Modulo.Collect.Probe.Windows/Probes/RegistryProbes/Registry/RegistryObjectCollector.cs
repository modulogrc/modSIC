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
using System.Linq;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Windows.Registry.Exceptions;
using Modulo.Collect.Probe.Windows.WMI;
using System.IO;
using Modulo.Collect.OVAL.Definitions.Windows;
using System.Text;
using System.Text.RegularExpressions;

namespace Modulo.Collect.Probe.Windows.Registry
{
    public class RegistryObjectCollector: BaseObjectCollector
    {
        private const string OBJECT_COLLECTED_SUCCESSFULLY = "The Key, which fullPath is '{0}', was collected sucessfully.";
        private const string OBJECT_NOT_FOUND = "The Key, which fullPath is '{0}', was not found.";
        private const string OBJECT_COULD_NOT_BE_COLLECTED = "The Key, which fullPath is '{0}', could not be collected.\r\nError message: '{1}";
        
        public WmiDataProvider WmiDataProvider { get; set; }
        public registry_object RegistryObject { get; set; }
        public TargetInfo TargetInfo { get; set; }
       
        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            var hiveName = parameters[registry_object_ItemsChoices.hive.ToString()].ToString();
            var keyName = parameters[registry_object_ItemsChoices.key.ToString()].ToString();
            
            var methodName = this.resolveEnumerateMethodName(parameters);
            var inParams = RegistryHelper.GetInputParametersForWmiEnumValuesMethod(hiveName, keyName);
            
            return (IList<String>)this.WmiDataProvider.InvokeMethod(methodName, inParams, "sNames");
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var registryItem = (registry_item)systemItem;
            var fullKeyPath = Path.Combine(registryItem.hive.Value, registryItem.key.Value, registryItem.name.Value);
            var itemBuilder = new RegistryItemTypeBuilder(registryItem);
            
            try
            {
                base.ExecutionLogBuilder.CollectingDataFrom(fullKeyPath);
                if (this.IsAllEntitiesIsSet(systemItem))
                {
                    var collectedSystemData = this.collectSystemDataForRegistryItem(registryItem);
                    itemBuilder.FillItemTypeWithData(collectedSystemData);
                }
            }
            catch (RegistryKeyNotFoundException)
            {
                base.ExecutionLogBuilder.Warning(string.Format("The registry key {0} is not found ", registryItem.key.Value));
                itemBuilder.SetRegistryKeyStatus(StatusEnumeration.doesnotexist);
                itemBuilder.SetItemTypeStatus(StatusEnumeration.doesnotexist, null);
                itemBuilder.ClearRegistryName();
            }
            catch (RegistryNameNotFoundException)
            {
                base.ExecutionLogBuilder.Warning(string.Format("The registry name {0} is not found ", registryItem.name.Value));
                itemBuilder.SetRegistryNameStatus(StatusEnumeration.doesnotexist);
                itemBuilder.SetItemTypeStatus(StatusEnumeration.doesnotexist, null);
            }

            itemBuilder.BuildItemType();
            itemBuilder.ClearEntitiesWithEmptyValueIfStatusDoesNotExists();
            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(itemBuilder.BuiltItemType, BuildExecutionLog());
        }


        private bool IsAllEntitiesIsSet(ItemType systemItem)
        {
            var registryItem = (registry_item)systemItem;
            return (!string.IsNullOrEmpty(registryItem.hive.Value)) &&
                   (!string.IsNullOrEmpty(registryItem.key.Value)) &&
                   (!string.IsNullOrEmpty(registryItem.name.Value));
                    
        }

        private string resolveEnumerateMethodName(Dictionary<string, object> parameters)
        {
            object nameEntity;
            parameters.TryGetValue(registry_object_ItemsChoices.name.ToString(), out nameEntity);

            var isEnumerateValues = ((nameEntity != null) && (!string.IsNullOrEmpty(nameEntity.ToString())));
            return isEnumerateValues ? "EnumValues" : "EnumKey";
        }

        private RegistryItemSystemData collectSystemDataForRegistryItem(registry_item item)
        {
            object dataValue = null;
            
            var valueTypeID = this.getValueTypeIDFromRegistry(item.hive.Value, item.key.Value, item.name.Value);
            if (valueTypeID == eValueTypes.DWORD_LITTLE_ENDIAN)
            {
                var credentials = TargetInfo.credentials;
                Helpers.WinNetUtils.connectToRemote(TargetInfo.GetRemoteUNC(), credentials.GetUserName(), credentials.GetPassword());
                var address = TargetInfo.GetAddress();
                var hive = RegistryHelper.GetRegistryHiveFromHiveName(item.hive.Value);
                var key = item.key.Value;
                var name = item.name.Value;
                dataValue = Microsoft.Win32.RegistryKey.OpenRemoteBaseKey(hive, address).OpenSubKey(key).GetValue(name);
            }
            else
            {
                var sGetValueMethodName = RegistryHelper.ConvertToGetValueMethodNameFromValueType(valueTypeID);
                var inParameters = this.getInParametersForGetValueMethod(item.hive.Value, item.key.Value, item.name.Value);
                var getValueResult = this.WmiDataProvider.InvokeMethod(sGetValueMethodName, inParameters);
                dataValue = this.getCollectedValueFromGetValueMethodResult(getValueResult);
            }

            if (valueTypeID == eValueTypes.STRING)
                dataValue = RemoveInvalidChars(dataValue.ToString());

            return new RegistryItemSystemData(valueTypeID, dataValue);
        }

        private string RemoveInvalidChars(string dataValue)
        {
            string inputString = dataValue.ToString();
            var sb = new StringBuilder();

            for (int i = 0; i < inputString.Length; i++)
            {
                if (inputString[i] == 0x09 || inputString[i] == 0x0A || inputString[i] == 0x0D || 
                    (inputString[i] >= 0x20 && inputString[i] <= 0xD7FF) ||
                    (inputString[i] >= 0xE000 && inputString[i] <= 0xFFFD))
                {
                    sb.Append(inputString[i]);
                }
            }

            dataValue = sb.ToString();
            return dataValue;
        }

        private eValueTypes getValueTypeIDFromRegistry(string hiveName, string keyName, string valueName)
        {
            var valueAndTypes = (Dictionary<String, int>)this.getValueNamesFromKeyName(hiveName, keyName, true);
            return this.getValueTypeIdFromDictionary(valueAndTypes, valueName);
        }

        private eValueTypes getValueTypeIdFromDictionary(Dictionary<string, int> dictionary, string valueName)
        {
            var registryNotFoundExceptionMessage = String.Format("The registry name {0} does not exist", valueName);
            if ((dictionary == null) || (dictionary.Count == 0))
                throw new RegistryNameNotFoundException(registryNotFoundExceptionMessage);

            var valueTypeID = 0;
            dictionary.TryGetValue(valueName.ToLower(), out valueTypeID);
            if (valueTypeID == 0)
                throw new RegistryNameNotFoundException(registryNotFoundExceptionMessage);
            
            return (eValueTypes)valueTypeID;
        }

        private Object getValueNamesFromKeyName(string hiveName, string key, bool withTypes)
        {
            var inParams = RegistryHelper.GetInputParametersForWmiEnumValuesMethod(hiveName, key);
            var enumValuesResult = this.WmiDataProvider.InvokeMethod("EnumValues", inParams);


            if ((uint)enumValuesResult["ReturnValue"] == 2)
                throw new RegistryKeyNotFoundException(String.Format("The key {0} does not exist", key));

            if (withTypes)
                return this.getKeyValueAndTypesFromWmiResult(enumValuesResult);
            else
                return (enumValuesResult["sNames"] as String[]).ToList<String>();
        }

        private Dictionary<String, Int32> getKeyValueAndTypesFromWmiResult(Dictionary<String, Object> enumValuesResult)
        {
            string[] valuesNames = (enumValuesResult["sNames"] as string[]);
            int[] valuesTypes = (enumValuesResult["Types"] as int[]);

            Dictionary<String, Int32> valueAndTypes = new Dictionary<String, Int32>();
            if (valuesNames != null)
            {
                for (int i = 0; i < valuesNames.Length; i++)
                    valueAndTypes.Add(valuesNames[i].ToLower(), valuesTypes[i]); // for registry collect is case insensitive
            }
            return valueAndTypes;
        }

        private Dictionary<String, String> getInParametersForGetValueMethod(string hiveName, string keyName, string valueName)
        {
            var inParameters = new Dictionary<String, String>();
            inParameters.Add("hDefKey", RegistryHelper.GetEnumValueFromHiveName(hiveName));
            inParameters.Add("sSubKeyName", keyName);
            inParameters.Add("sValueName", valueName);
            return inParameters;
        }

        private Object getCollectedValueFromGetValueMethodResult(Dictionary<String, Object> getValueResult)
        {
            object collectedValue = null;
            if (!getValueResult.TryGetValue("sValue", out collectedValue))
                getValueResult.TryGetValue("uValue", out collectedValue);

            if (collectedValue == null)
                throw new RegistryKeyNotFoundException(String.Format("The key {0} does not exist", ""));
            else
                return collectedValue;
        }
    }
}
