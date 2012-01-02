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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.EntityOperations;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Windows.Registry.Exceptions;
using Modulo.Collect.Probe.Windows.WMI;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.Operators;


namespace Modulo.Collect.Probe.Windows.Registry.operation
{
    public class RegistryEntityOperationEvaluator
    {
        private BaseObjectCollector systemDataSource;
        private WmiDataProvider wmiDataProvider;
        private OperatorHelper operatorHelper = new OperatorHelper();

        public RegistryEntityOperationEvaluator(BaseObjectCollector systemDataSource, WmiDataProvider wmiDataProvider)
        {
            this.systemDataSource = systemDataSource;
            this.wmiDataProvider = wmiDataProvider;
        }

        public IEnumerable<ItemType> ProcessOperation(IEnumerable<RegistryObject> registryObjects)
        {
            List<ItemType> registryObjectsProcessed = new List<ItemType>();
            foreach (RegistryObject registryObject in registryObjects)
            {
                registryObjectsProcessed.AddRange(this.processOperationInRegistryObject(registryObject));
            }

            return registryObjectsProcessed;

        }

        private IEnumerable<ItemType> processOperationInRegistryObject(RegistryObject registryObject)
        {
            List<string> hives = new List<string>() { registryObject.Hive };
            RegistryItemTypeFactory itemTypeFactory = new RegistryItemTypeFactory();

            List<ItemType> items = new List<ItemType>();
            List<string> keys = new List<string>();
            List<string> names = new List<string>();
            try
            {
                var registryNameValue = registryObject.GetValueOfEntity("name");
                keys.AddRange(this.processOperationInEntity(registry_object_ItemsChoices.key, registryObject));

                var derivedRegistryObjects =
                    RegistryObjectFactory.CreateRegistryObjectsByCombinationOfEntitiesFrom(hives, keys, new string[] { registryNameValue }, registryObject);

                if (registryObject.GetNameOperation() != OperationEnumeration.equals)
                {
                    foreach (var newRegistryObject in derivedRegistryObjects)
                    {
                        if (!string.IsNullOrEmpty(registryNameValue))
                        {
                            names = this.processOperationInEntity(registry_object_ItemsChoices.name, newRegistryObject).ToList();
                            items.AddRange(itemTypeFactory.CreateRegistryItemTypesByCombinationOfEntitiesFrom(hives, new string[] { newRegistryObject.Key }, names, registryObject));
                        }
                    }
                }
                else
                {
                    names.AddRange(this.processOperationInEntity(registry_object_ItemsChoices.name, registryObject));
                    items.AddRange(itemTypeFactory.CreateRegistryItemTypesByCombinationOfEntitiesFrom(hives, keys, names, registryObject));
                }
            }
            catch (RegistryKeyNotFoundException)
            {
                items = itemTypeFactory.CreateRegistryItemTypesByCombinationOfEntitiesFrom(hives, null, null, registryObject).ToList();
            }
            catch (RegistryItemNotFoundException)
            {
                items = new List<ItemType>() { itemTypeFactory.CreateRegistryItem("", "", "", StatusEnumeration.doesnotexist) };
            }
            catch (Exception ex)
            {
                registry_item registry = itemTypeFactory.CreateRegistryItem(registryObject.Hive, registryObject.Key, registryObject.Name, StatusEnumeration.error);
                registry.message = MessageType.FromErrorString(ex.Message);
                items = new List<ItemType>() { registry };
            }

            return items;
        }

        private IEnumerable<string> processOperationInEntity(registry_object_ItemsChoices entity, RegistryObject registryObject)
        {
            string entityValue = registryObject.GetValueOfEntity(entity.ToString());
            OperationEnumeration entityOperation = registryObject.GetOperationOfEntity(entity.ToString());
            bool isKeyEntity = (entity == registry_object_ItemsChoices.key);
            if (operatorHelper.IsEqualsOperation(entityOperation))
            {
                return new List<string>() { this.processEqualsOperations(entity, registryObject, isKeyEntity) };
            }
            else
            {
                return this.EvaluateOperation(entityOperation, entityValue, registryObject, isKeyEntity);
            }
        }

        private string processEqualsOperations(registry_object_ItemsChoices entity, RegistryObject registryObject, bool isKey)
        {
            string hive = registryObject.Hive;
            string key = registryObject.Key;
            Dictionary<string, string> parameters = RegistryHelper.GetInputParametersForWmiEnumValuesMethod(hive, key);
            if (isKey)
            {
                Dictionary<string, object> result = this.wmiDataProvider.InvokeMethod("EnumKey", parameters);
                if ((uint)result["ReturnValue"] == 2)
                    throw new RegistryKeyNotFoundException(String.Format("The key {0} does not exist", key));
                return key;
            }
            else
            {
                return registryObject.GetValueOfEntity(entity.ToString());
            }

        }


        private IEnumerable<string> getSubKeysRecursive(string hiveValue, string rootKey)
        {
            string keyFixedPart;
            int patternLevelCount = this.getPatternLevelCount(rootKey, out keyFixedPart);
            List<string> subKeys = this.getSubKeys(hiveValue, keyFixedPart).ToList<string>();
            List<string> resultSubKeys = new List<string>();


            if (patternLevelCount == 1)
                foreach (var subKey in subKeys)
                    resultSubKeys.Add(string.Format("{0}{1}", keyFixedPart, subKey));
            else
                for (int i = 1; i < patternLevelCount; i++)
                {
                    List<string> newSubKeys = new List<string>();
                    foreach (var subKey in subKeys)
                        foreach (var subSubKey in this.getSubKeys(hiveValue, subKey))
                            newSubKeys.Add(string.Format("{0}\\{1}", subKey, subSubKey));

                    resultSubKeys.AddRange(newSubKeys);
                }


            return resultSubKeys;
        }

        private IEnumerable<string> getSubKeys(string hiveValue, string keyValue)
        {
            Dictionary<string, object> searchSubKeysParameters = this.getEntityParamsForSearchValues(hiveValue, keyValue, null);
            IList<string> subKeys = this.systemDataSource.GetValues(searchSubKeysParameters);
            if (subKeys == null)
                subKeys = new List<string>();
            return subKeys;

        }

        private IEnumerable<string> getKeyValues(RegistryObject registryObject)
        {
            string hive = registryObject.Hive;
            string key = registryObject.Key;
            string name = registryObject.Name;

            Dictionary<string, object> searchEntityParameters = this.getEntityParamsForSearchValues(hive, key, name);
            return this.systemDataSource.GetValues(searchEntityParameters);
        }

        private Dictionary<string, object> getEntityParamsForSearchValues(string hiveValue, string keyValue, string nameValue)
        {
            Dictionary<string, object> searchEntityParameters = new Dictionary<string, object>();
            searchEntityParameters.Add(registry_object_ItemsChoices.hive.ToString(), hiveValue);
            searchEntityParameters.Add(registry_object_ItemsChoices.key.ToString(), keyValue);
            if (!string.IsNullOrEmpty(nameValue))
                searchEntityParameters.Add(registry_object_ItemsChoices.name.ToString(), nameValue);

            return searchEntityParameters;
        }

        private Dictionary<string, object> getEntityOperationParameters(string entityValue, IEnumerable<string> valuesToMatch)
        {
            Dictionary<string, object> operationParameters = new Dictionary<string, object>();
            operationParameters.Add(EntityOperationParameters.EntityValue.ToString(), entityValue);
            operationParameters.Add(EntityOperationParameters.ValuesToApply.ToString(), valuesToMatch);
            return operationParameters;
        }

        private int getPatternLevelCount(string pattern, out string keyFixedPart)
        {
            keyFixedPart = string.Empty;
            List<String> allLevels = pattern.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).ToList<String>();

            int patternCount = 0;
            foreach (var level in allLevels)
            {
                if (RegexHelper.IsPathLevelARegexPattern(level))
                    patternCount++;
                else
                    if (patternCount == 0)
                        keyFixedPart += string.Format("{0}\\", level);
            }

            return patternCount;
        }

        private IEnumerable<string> EvaluateOperation(OperationEnumeration operation, string entityValue, RegistryObject registryObject, bool isKeyEntity)
        {
            IEnumerable<string> valuesToMatch = null;
            if (operatorHelper.IsRegularExpression(operation))
            {
                if (isKeyEntity)
                    valuesToMatch = this.getSubKeysRecursive(registryObject.Hive, registryObject.Key);
                else
                {
                    registryObject.ClearNameEntity();
                    valuesToMatch = this.getKeyValues(registryObject);
                }

                return new MultiLevelPatternMatchOperation(FamilyEnumeration.windows).applyPatternMatch(entityValue, valuesToMatch);
            }
            else
            {
                valuesToMatch = this.GetValuesToMatchForOperationsDifferentsOfPatternMatch(isKeyEntity, registryObject);
                return NonPatternMatchOperationEvaluator.
                    EvaluateOperationsDifferentsOfPatternMatch(operation, entityValue, valuesToMatch);
            }
        }

        private IEnumerable<string> GetValuesToMatchForOperationsDifferentsOfPatternMatch(bool isKeyEntity, RegistryObject registryObject)
        {
            if (isKeyEntity)
            {
                return GetValuesForKeyEntity(registryObject, new List<string>() { registryObject.Hive });
            }
            else
            {
                return this.getKeyValues(registryObject);
            }
        }

        private IEnumerable<string> GetValuesForKeyEntity(RegistryObject registryObject, IEnumerable<string> elements)
        {
            List<string> values = new List<string>();
            foreach (string startElement in elements)
            {
                List<string> elementsOfKey = startElement.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                string key = "";
                if (elementsOfKey.Count() > 1)
                {
                    elementsOfKey.Remove(elementsOfKey.First());
                    key = this.ConvertAStringWithSeparator(elementsOfKey, "\\");
                }
                IEnumerable<string> valuesToMatch = this.getSubKeys(registryObject.Hive, key);
                if (valuesToMatch == null)
                {
                    values.Add(startElement);
                }
                else
                {
                    IEnumerable<string> valuesConcatenated = this.ConcatTheValuesOfKey(valuesToMatch, startElement);
                    values.AddRange(this.GetValuesForKeyEntity(registryObject, valuesConcatenated));
                }
            }
            return values;
        }

        private IEnumerable<string> ConcatTheValuesOfKey(IEnumerable<string> valuesToMatch, string key)
        {
            List<string> concatenatedValues = new List<string>();
            foreach (string value in valuesToMatch)
            {
                string concatenatedValue = string.Format("{0}\\{1}", key, value);
                concatenatedValues.Add(concatenatedValue);
            }
            return concatenatedValues;
        }

        private string ConvertAStringWithSeparator(IEnumerable<string> elements, string separator)
        {
            string stringWithSeparator = elements.First();
            for (int i = 1; i <= elements.Count() - 1; i++)
                stringWithSeparator += string.Format("\\{0}", elements.ElementAt(i));
            
            return stringWithSeparator;
        }
    }
}
