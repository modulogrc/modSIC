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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Independent.Common.Operators;
using Modulo.Collect.Probe.Windows.File;


namespace Modulo.Collect.Probe.Windows.FileEffectiveRights
{
    public class FileEffectiveRightsOperationEvaluator
    {
        private BaseObjectCollector systemDataSource;
        private PathOperatorEvaluator pathOperatorEvaluator;
        private OperatorHelper operatorHelper = new OperatorHelper();

        public FileEffectiveRightsOperationEvaluator(BaseObjectCollector sytemDataSource, IFileProvider fileProvider)
        {
            this.systemDataSource = sytemDataSource;
            this.pathOperatorEvaluator = new PathOperatorEvaluator(fileProvider, FamilyEnumeration.windows);
        }

        public IEnumerable<ItemType> ProcessOperation(IEnumerable<Definitions::ObjectType> objectTypes)
        {
            List<ItemType> itemTypes = new List<ItemType>();
            foreach (var objectType in objectTypes)
            {
                itemTypes.AddRange(this.ProcessOperation((fileeffectiverights_object)objectType));
            }
            return itemTypes;
        }

        public IEnumerable<ItemType> ProcessOperation(fileeffectiverights_object fileEffectiveRights)
        {
            Dictionary<String, EntityObjectStringType> entities = OvalHelper.GetFileEffectiveRightsFromObjectType(fileEffectiveRights);

            IEnumerable<string> paths = this.processOperationPath(entities);
            IEnumerable<string> fileNames = this.processOperationFileName(entities, paths);
            IEnumerable<string> trustNames = this.processOperationTrustName(entities);

            var fileEffectiveRightsItemTypeFactory = 
                new FileEffectiveRightsItemTypeFactory(
                    SourceObjectTypes.FileEffectiveRights, (FileEffectiveRightsObjectCollector)systemDataSource);
            return fileEffectiveRightsItemTypeFactory.CreateFileItemTypesByCombinationOfEntitiesFrom(paths, fileNames, trustNames);
        }

        private IEnumerable<string> processOperationTrustName(Dictionary<string, EntityObjectStringType> entities)
        {
            List<string> trustNames = new List<string>();
            EntityObjectStringType trustName;
            if (entities.TryGetValue("trustee_name", out trustName))
            {
                if (operatorHelper.IsEqualsOperation(trustName.operation))
                {
                    trustNames.Add(trustName.Value);
                }
                else if (operatorHelper.IsRegularExpression(trustName.operation))
                {
                    trustNames.AddRange(this.processOperationPatternMatchForTrustName(trustName));
                }
                else if (operatorHelper.IsNotEqualsOperation(trustName.operation))
                {
                    string[] valuesToApply = this.searchUsers();
                    trustNames.AddRange(this.EvaluateOperationsDifferentsOfPatternMatch(trustName.operation, trustName.Value, valuesToApply));
                }
            }
            return trustNames;
        }

        private IEnumerable<string> EvaluateOperationsDifferentsOfPatternMatch(OperationEnumeration operation, string entityValue, IEnumerable<string> valuesToMatch)
        {
            IOvalComparator comparator = new OvalComparatorFactory().GetComparator(SimpleDatatypeEnumeration.@string);
            List<string> values = new List<string>();
            foreach (string valueToMatch in valuesToMatch)
            {
                if (comparator.Compare(entityValue, valueToMatch, operation))
                    values.Add(valueToMatch);
            }
            return values;
        }

        private IEnumerable<string> processOperationPatternMatchForTrustName(EntityObjectStringType trustName)
        {
            var operationResult = new List<string>();
            string[] valuesToApply = this.searchUsers();
            var multiLevelOperation = new MultiLevelPatternMatchOperation(FamilyEnumeration.windows);
            operationResult.AddRange(multiLevelOperation.applyPatternMatch(trustName.Value, valuesToApply));
            return operationResult;
        }

        private IEnumerable<string> processOperationFileName(Dictionary<string, EntityObjectStringType> entities, IEnumerable<string> paths)
        {
            return this.pathOperatorEvaluator.ProcessOperationFileName(entities, paths, false);
        }

        private IEnumerable<string> processOperationPath(Dictionary<string, EntityObjectStringType> entities)
        {
            return this.pathOperatorEvaluator.ProcessOperationsPaths(entities);
        }

        private string[] searchUsers()
        {
            return this.systemDataSource.GetValues(null).ToArray();
        }

    }
}
