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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Windows.Registry;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.Operators;

namespace Modulo.Collect.Probe.Windows.RegistryKeyEffectiveRights53
{
    public class RegKeyEffectiveRightsOperationEvaluator
    {
        private EntityObjectStringType HiveObjectEntity = null;
        private EntityObjectStringType KeyObjectEntity = null;
        private EntityObjectStringType TrusteeSIDObjectEntity = null;

        public BaseObjectCollector SystemDataSource { get; set; }


        public virtual IEnumerable<string> ProcessOperationForKeyEntity(ObjectType sourceObject, string[] KeysAlreadyProcessed)
        {
            this.ConfigureObjectEntities((regkeyeffectiverights53_object)sourceObject);
            KeysAlreadyProcessed = this.GetEntityValuesAlreadyProcessed(KeysAlreadyProcessed, this.KeyObjectEntity.Value);

            List<String> operationResult = new List<String>();
            foreach (var key in KeysAlreadyProcessed)
                operationResult.AddRange(this.processKey(key));

            return operationResult;
        }

        public virtual IEnumerable<String> ProcessOperationForTrusteeSidEntity(
            ObjectType sourceObject, string[] TrusteeSIDsAlreadyProcessed)
        {
            this.ConfigureObjectEntities((regkeyeffectiverights53_object)sourceObject);
            TrusteeSIDsAlreadyProcessed = 
                GetEntityValuesAlreadyProcessed(TrusteeSIDsAlreadyProcessed, TrusteeSIDObjectEntity.Value);

            List<String> operationResult = new List<String>();
            foreach (var trusteeSID in TrusteeSIDsAlreadyProcessed)
            {
                if (this.TrusteeSIDObjectEntity.operation == OperationEnumeration.equals)
                {
                    operationResult.Add(trusteeSID);
                    continue;
                }

                operationResult = this.processTrusteeSID(trusteeSID, operationResult.ToArray());
            }

            return operationResult;
        }


        private IEnumerable<string> processKey(string entityValue)
        {
            if (this.KeyObjectEntity.operation == OperationEnumeration.patternmatch)
                return this.getSubKeysApplyingPatternMatch(this.HiveObjectEntity.Value, entityValue);

            if (this.KeyObjectEntity.operation == OperationEnumeration.equals)
                return new List<String>(new string[] { entityValue });

            string startKeyPath = this.RemoveLastLevelOfKeyPath(entityValue);
            var valuesToMatch = this.EnumerateSubKeys(startKeyPath);
            return NonPatternMatchOperationEvaluator
                .EvaluateOperationsDifferentsOfPatternMatch(this.KeyObjectEntity.operation, entityValue, valuesToMatch);
        }

        private List<String> processTrusteeSID(string trusteeSID, string[] valuesToComparer)
        {
            var objectCollector = ((RegKeyEffectiveRightsObjectCollector)this.SystemDataSource);
            if ((valuesToComparer == null) || (valuesToComparer.Count() == 0))
                valuesToComparer = objectCollector.SearchUserTrusteeSIDs().ToArray();
            
            var result = new List<String>();
            var comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            foreach (var sidToCompare in valuesToComparer)
            {
                //var hasRegKeyUserDACL = 
                //    objectCollector.IsThereDACLOnRegistryKeyForUser(
                //        HiveObjectEntity.Value, KeyObjectEntity.Value, sidToCompare);

                //if (!hasRegKeyUserDACL)
                //    continue;

                if (comparator.Compare(sidToCompare, trusteeSID, this.TrusteeSIDObjectEntity.operation))
                    result.Add(sidToCompare);
            }

            return result;
        }

        private string RemoveLastLevelOfKeyPath(string registryKeyPath)
        {
            int indexOfLastSlash = registryKeyPath.LastIndexOf("\\");
            int characterCountToRemove = registryKeyPath.Length - indexOfLastSlash;

            return registryKeyPath.Remove(indexOfLastSlash, characterCountToRemove);
        }

        private IEnumerable<string> getSubKeysApplyingPatternMatch(string hiveName, string key)
        {
            string[] allKeyLevels = key.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            List<String> derivedKeys = new List<String>();
            foreach (var levelKey in allKeyLevels)
            {
                if (RegexHelper.IsPathLevelARegexPattern(levelKey))
                {
                    derivedKeys = this.DeriveSubKeysForLevel(derivedKeys, levelKey);
                    if (derivedKeys.Count == 0)
                        break;
                }
                else if (derivedKeys.Count == 0)
                    derivedKeys.Add(levelKey);
                else
                    for (int i = 0; i < derivedKeys.Count; i++)
                        derivedKeys[i] += string.Format("\\{0}", levelKey);
            }

            return derivedKeys;
        }

        private List<string> DeriveSubKeysForLevel(List<String> actualDerivedKeys, string levelPattern)
        {
            List<String> newDerivedKeys = new List<String>();
            foreach (var actualKey in actualDerivedKeys)
            {
                string[] subKeysToMatch = this.EnumerateSubKeys(actualKey);
                IList<String> hitKeys = this.matchSubKeysToPatternMatch(subKeysToMatch, levelPattern);
                foreach (var hitKey in hitKeys)
                {
                    string newDerivedKey = string.Format(@"{0}\{1}", actualKey, hitKey);
                    newDerivedKeys.Add(newDerivedKey);
                }
            }

            return newDerivedKeys;
        }

        private IList<String> matchSubKeysToPatternMatch(string[] subKeyNames, string subKeyPattern)
        {
            var regexHelper = new RegexHelper(subKeyPattern, false);
            return regexHelper.MatchPathNamesToPattern(subKeyNames, subKeyPattern);
        }

        private string[] EnumerateSubKeys(string startKeyPath)
        {
            var searchSubKeysParameters = new Dictionary<string, object>();
            searchSubKeysParameters.Add("RegistryHive", RegistryHelper.GetRegistryHiveFromHiveName(this.HiveObjectEntity.Value));
            searchSubKeysParameters.Add("RegistryKey", startKeyPath);

            return this.SystemDataSource.GetValues(searchSubKeysParameters).ToArray();
        }

        private string getRootKeyPath(string[] keyLevels)
        {
            string rootKeyPath = string.Empty;
            foreach (var level in keyLevels)
            {
                if (RegexHelper.IsPathLevelARegexPattern(level))
                    break;

                rootKeyPath += string.Format("\\{0}", level);
            }

            return rootKeyPath.Remove(0, 1);
        }

        private void ConfigureObjectEntities(regkeyeffectiverights53_object regKeyEffectiveRightsObject)
        {
            var allObjectEntities = regKeyEffectiveRightsObject.GetAllObjectEntities();

            this.HiveObjectEntity = allObjectEntities[regkeyeffectiverights53_object_ItemsChoices.hive.ToString()];
            this.KeyObjectEntity = allObjectEntities[regkeyeffectiverights53_object_ItemsChoices.key.ToString()];
            this.TrusteeSIDObjectEntity = allObjectEntities[regkeyeffectiverights53_object_ItemsChoices.trustee_sid.ToString()];

        }

        private string[] GetEntityValuesAlreadyProcessed(string[] valuesAlreadyProcessed, string defaultEntityValue)
        {
            bool AreThereValuesAlreadyProcessed = (!(((valuesAlreadyProcessed == null) || (valuesAlreadyProcessed.Count() == 0))));
            return AreThereValuesAlreadyProcessed ? valuesAlreadyProcessed : new string[] { defaultEntityValue };
        }
    }
}