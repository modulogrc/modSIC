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
using System.Linq;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Windows.FileEffectiveRights;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Independent.Common.Operators;
using Modulo.Collect.Probe.Windows.File;


namespace Modulo.Collect.Probe.Windows.FileEffectiveRights53
{
    public class FileEffectiveRights53EntityOperationEvaluator
    {
        private FileEffectiveRightsObjectCollector FileEffectiveRights53SystemDataSource;
        private IFileProvider FileProvider;
        private PathOperatorEvaluator PathOperatorEvaluator { get; set; }

        private string[] allUsersSID = null;


        public FileEffectiveRights53EntityOperationEvaluator(
            FileEffectiveRightsObjectCollector objectCollector, IFileProvider fileProvider)
        {
            this.FileEffectiveRights53SystemDataSource = objectCollector;
            this.FileProvider = fileProvider;
            this.PathOperatorEvaluator = new PathOperatorEvaluator(fileProvider, FamilyEnumeration.windows);
        }

        public IEnumerable<ItemType> ProcessOperation(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            List<ItemType> items = new List<ItemType>();
            foreach (Definitions.ObjectType objectType in objectTypes)
            {
                try
                {
                    items.AddRange(this.ProcessOperation((fileeffectiverights53_object)objectType));
                }
                catch (Exception ex)
                {
                    items.Add(this.CreateFileEffectiveRightsItemTypeWithError(objectType, ex.Message));
                }


            }

            return items;
        }

        private ItemType CreateFileEffectiveRightsItemTypeWithError(Definitions.ObjectType objectType, string message)
        {
            var fileobject = (fileeffectiverights53_object)objectType;
            var path = string.Empty;
            var filepath = string.Empty;
            var filename = string.Empty;
            var trustee = fileobject.GetItemValue(fileeffectiverights53_object_ItemsChoices.trustee_sid).ToString();

            if (fileobject.IsFilePathDefined())
            {
                filepath = fileobject.GetItemValue(fileeffectiverights53_object_ItemsChoices.filepath).ToString();
                path = System.IO.Path.GetDirectoryName(filepath);
                filename = System.IO.Path.GetFileName(filepath);
            }
            else
            {
                path = fileobject.GetItemValue(fileeffectiverights53_object_ItemsChoices.path).ToString();
                filename = fileobject.GetItemValue(fileeffectiverights53_object_ItemsChoices.filename).ToString();
            }
            
            return 
                CreateFileEffectiveRightsItemTypeFactory()
                    .CreateFileItemTypesWithError(path, filename, trustee, message, StatusEnumeration.error);
        }

        public IEnumerable<ItemType> ProcessOperation(fileeffectiverights53_object objectToCollect)
        {
            IEnumerable<string> filenames = null;
            IEnumerable<string> paths = null;

            var allEntities = objectToCollect.GetAllObjectEntities();
            var trusteeSID = ((EntityObjectStringType)objectToCollect.GetItemValue(fileeffectiverights53_object_ItemsChoices.trustee_sid)).Value;
            
            if (objectToCollect.IsFilePathDefined())
            {
                var filepath = this.GetDictionaryWithElement(fileeffectiverights53_object_ItemsChoices.filepath, allEntities);
                var filePathOperationResult = this.PathOperatorEvaluator.ProcessOperationFilePath(filepath);
                paths = filePathOperationResult.Paths;

                filenames = new List<String>();
                foreach (var completeFilepath in filePathOperationResult.FileNames)
                    ((List<String>)filenames).Add(System.IO.Path.GetFileName(completeFilepath));
            }
            else
            {
                paths = this.ProcessOperationsPaths(allEntities);
                filenames = this.ProcessOperationsFileNames(allEntities, paths);
            }

            var trusteeSIDNames = this.ProcessOperationsTrusteeSID(objectToCollect);
            return
                CreateFileEffectiveRightsItemTypeFactory()
                    .CreateFileItemTypesByCombinationOfEntitiesFrom(paths, filenames, trusteeSIDNames);
        }

        private FileEffectiveRightsItemTypeFactory CreateFileEffectiveRightsItemTypeFactory()
        {
            return 
                new FileEffectiveRightsItemTypeFactory(
                    SourceObjectTypes.FileEffectiveRights53, FileEffectiveRights53SystemDataSource);
        }

        private Dictionary<string, EntityObjectStringType> GetDictionaryWithElement(
            fileeffectiverights53_object_ItemsChoices entityName, Dictionary<string, EntitySimpleBaseType> allEntities)
        {
            var dictionary = new Dictionary<String, EntityObjectStringType>();
            dictionary.Add(entityName.ToString(), (EntityObjectStringType)allEntities[entityName.ToString()]);

            return dictionary;
        }

        private IEnumerable<string> ProcessOperationsPaths(Dictionary<string, EntitySimpleBaseType> allEntities)
        {
            var paths = this.GetDictionaryWithElement(fileeffectiverights53_object_ItemsChoices.path, allEntities);
            return PathOperatorEvaluator.ProcessOperationsPaths(paths);
        }

        private IEnumerable<string> ProcessOperationsFileNames(
            Dictionary<string, EntitySimpleBaseType> allEntities, 
            IEnumerable<string> paths)
        {
            var fileNames = this.GetDictionaryWithElement(fileeffectiverights53_object_ItemsChoices.filename, allEntities);
            return PathOperatorEvaluator.ProcessOperationFileName(fileNames, paths, false);
        }

        private IEnumerable<String> ProcessOperationsTrusteeSID(fileeffectiverights53_object objectToCollect)
        {
            var trusteeSIDEntityName = fileeffectiverights53_object_ItemsChoices.trustee_sid.ToString();
            var trusteeSIDEntity = objectToCollect.GetAllObjectEntities()[trusteeSIDEntityName];
            var derivedTrusteeSIDs = new List<String>();

            if (trusteeSIDEntity.operation == OperationEnumeration.equals)
                derivedTrusteeSIDs.Add(trusteeSIDEntity.Value);
            else
            {
                this.searchAllUsersOnTarget();
                derivedTrusteeSIDs = 
                    new MultiLevelPatternMatchOperation(FamilyEnumeration.windows)
                        .applyPatternMatch(trusteeSIDEntity.Value, allUsersSID).ToList();
            }

            return derivedTrusteeSIDs;
        }

        private void searchAllUsersOnTarget()
        {
            if (this.allUsersSID == null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("ReturnSID", null);

                this.allUsersSID = this.FileEffectiveRights53SystemDataSource.GetValues(parameters).ToArray();
            }
        }
    }
}
