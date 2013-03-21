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
using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Unix.SSHCollectors;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions.VariableEvaluators;
using Modulo.Collect.OVAL.Common.comparators;
using Modulo.Collect.OVAL.Definitions.Independent;
using System.Text;

namespace Modulo.Collect.Probe.Unix.TextFileContent54
{
    public class TemporaryItemTypeGenerator: IItemTypeGenerator
    {

        public FileContentCollector FileContentCollector { get; set; }
        public  TargetInfo TargetInfo { get; set; }

        public IEnumerable<ItemType> GetItemsToCollect(Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var fileObject = ((textfilecontent54_object)objectType);
            var variableEvaluator = new VariableEntityEvaluator(variables);

            IList<string> filepaths = new List<string>();
            IList<string> paths = new List<string>();
            IList<string> filenames = new List<string>();

            if (fileObject.IsFilePathDefined())
            {
                filepaths =
                    variableEvaluator
                        .EvaluateVariableForEntity(
                            (EntitySimpleBaseType)fileObject.GetItemValue(
                                textfilecontent54_ItemsChoices.filepath)).ToList();

                foreach (var filepath in filepaths)
                {
                    var indexOfLastSlash = filepath.LastIndexOf("/");
                    var path = filepath.Substring(0, indexOfLastSlash + 1);
                    var filename = filepath.Replace(path, string.Empty);

                    paths.Add(path);
                    filenames.Add(filename);
                }
            }
            else
            {
                paths =
                    variableEvaluator
                        .EvaluateVariableForEntity(
                            (EntitySimpleBaseType)fileObject.GetItemValue(
                                textfilecontent54_ItemsChoices.path)).ToList();
                filenames =
                    variableEvaluator
                        .EvaluateVariableForEntity(
                            (EntitySimpleBaseType)fileObject.GetItemValue(
                                textfilecontent54_ItemsChoices.filename)).ToList();

                foreach (var path in paths)
                    foreach (var filename in filenames)
                        filepaths.Add(string.Format("{0}/{1}", path, filename).Replace("//", "/"));
            }

            var entityPattern = (EntitySimpleBaseType)fileObject.GetItemValue(textfilecontent54_ItemsChoices.pattern);
            var pattern = variableEvaluator.EvaluateVariableForEntity(entityPattern).SingleOrDefault();

            var entityInstance = (EntitySimpleBaseType)fileObject.GetItemValue(textfilecontent54_ItemsChoices.instance);
            var instance = variableEvaluator.EvaluateVariableForEntity(entityInstance).SingleOrDefault();

            var itemsToCollect = new List<ItemType>();
            foreach (var filepath in filepaths)
            {
                if (string.IsNullOrWhiteSpace(pattern))
                    continue;

                var fileContentSearchParameters =
                    TextFileContentObjectCollector
                        .GetDictionaryWithParametersToSearchTextFileConten(
                            filepath, pattern, int.Parse(instance), fileObject.IsMultiline());

                var matchLines = ObjectCollector.GetValues(fileContentSearchParameters);
                if (matchLines == null || matchLines.Count <= 0)
                {
                    var newNotExistsItem = CreateTextFileContentItem(filepath, "", "", pattern, instance, null);
                    newNotExistsItem.status = StatusEnumeration.doesnotexist;
                    itemsToCollect.Add(newNotExistsItem);
                }
                else
                {
                    var newCollectedItem = CreateTextFileContentItem(filepath, "", "", pattern, instance, null);
                    var result = string.Join(System.Environment.NewLine, matchLines);
                    ((textfilecontent_item)newCollectedItem).text = new EntityItemAnySimpleType() { Value = result };
                    itemsToCollect.Add(newCollectedItem);
                }
            }           

            //IList<ItemType> itemsToCollect = new List<ItemType>();
            //for (int i = 0; i < filepaths.Count; i++)
            //{
            //    if ((filepaths[i] != null) && (paths[i] != null) && (filenames[i] != null) && (pattern != null) && (instance != null))
            //    {
            //        var fileContents = this.FileContentCollector.GetTextFileFullContentInLines(filepaths[i]); //this.FileContentCollector.GetTextFileContent(filepaths[i], pattern);
            //        if ((fileContents == null) || (fileContents.Count() <= 0))
            //        {
            //            var newFileContentItem = CreateTextFileContentItem(filepaths[i], "", "", "", "", "");
            //            newFileContentItem.status = StatusEnumeration.doesnotexist;
            //            itemsToCollect.Add(newFileContentItem);
            //            continue;
            //        }
                    
            //        var matchLines = new Dictionary<int, string>();
            //        for (int currentInstance = 1; currentInstance <= fileContents.Count(); currentInstance++)
            //        {
            //            var comparator = new OvalComparatorFactory().GetComparator(entityInstance.datatype);
            //            if (comparator.Compare(instance, currentInstance.ToString(), entityInstance.operation))
            //            {
            //                var instanceNumber = currentInstance - 1;
            //                var matchLine = fileContents[instanceNumber].Text;
            //                matchLines.Add(currentInstance, matchLine);
            //            }
            //        }

            //        foreach (var matchLine in matchLines)
            //        {
            //            var newTextFileContent =
            //                CreateTextFileContentItem(
            //                    filepaths[i],
            //                    paths[i],
            //                    filenames[i],
            //                    pattern,
            //                    matchLine.Key.ToString(),
            //                    matchLine.Value);

            //            itemsToCollect.Add(newTextFileContent);
            //        }
            //    }
            //}

            return itemsToCollect;
        }

            
        private ItemType CreateTextFileContentItem(
            string filepath, string path, string filename, string pattern, string instance, string text)
        {
            return new textfilecontent_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(filepath),
                path = OvalHelper.CreateItemEntityWithStringValue(path),
                filename = OvalHelper.CreateItemEntityWithStringValue(filename),
                pattern = OvalHelper.CreateItemEntityWithStringValue(pattern),
                instance = OvalHelper.CreateItemEntityWithIntegerValue(instance),
                text = OvalHelper.CreateEntityItemAnyTypeWithValue(text)
            };
        }

        public BaseObjectCollector ObjectCollector { get; set; }
    }

    [ProbeCapability(OvalObject="textfilecontent54", PlataformName=FamilyEnumeration.unix)]
    public class TextFileContentProberUnix : ProbeBase, IProbe
    {
        private TargetInfo TargetInfo;

        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            var setElement = ((textfilecontent54_object)objectType).GetItemValue(textfilecontent54_ItemsChoices.set);
            return (Definitions.set)setElement;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = base.ConnectionManager.Connect<SSHConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var commandRunner = ((SSHConnectionProvider)ConnectionProvider).SshCommandLineRunner;
                var newFileContentCollector = new FileContentCollector(commandRunner);
                var newFileCollector = new FileCollector() { LsCommand = new LsCommand(commandRunner) };
                var newUnixFileProvider = new UnixFileProvider(newFileContentCollector, newFileCollector);
                
                base.ObjectCollector = 
                    new TextFileContentObjectCollector()
                    {
                        FileContentProvider = newUnixFileProvider,
                        TargetInfo = this.TargetInfo
                    };

                base.ItemTypeGenerator = 
                    new TemporaryItemTypeGenerator() 
                    {
                        ObjectCollector = base.ObjectCollector, 
                        TargetInfo  = this.TargetInfo
                    };
            }
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<textfilecontent54_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new textfilecontent_item() { status = StatusEnumeration.error, message = PrepareErrorMessage(errorMessage) };
        }
    }
}
