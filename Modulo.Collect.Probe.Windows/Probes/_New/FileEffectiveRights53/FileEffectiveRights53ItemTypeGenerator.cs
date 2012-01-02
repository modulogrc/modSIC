using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.File;

namespace Modulo.Collect.Probe.Windows.Probes._New.FileEffectiveRights53
{
    public class FileEffectiveRights53ItemTypeGenerator: IItemTypeGenerator
    {
        private WindowsFileProvider FileProvider;
        private FileEffectiveRights53ObjectCollector ObjectCollector;

        public FileEffectiveRights53ItemTypeGenerator(
            WindowsFileProvider fileProvider, FileEffectiveRights53ObjectCollector objectCollector)
        {
            this.FileProvider = fileProvider;
            this.ObjectCollector = objectCollector;
        }

        public IEnumerable<ItemType> GetItemsToCollect(OVAL.Definitions.ObjectType objectType, VariablesEvaluated variables)
        {
            var variableEvaluator = new VariableEntityEvaluator(variables);
            var fileEffectiveRightsObject = (fileeffectiverights53_object)objectType;
            
            var paths = new List<String>();
            var filenames = new List<String>();
            if (fileEffectiveRightsObject.IsFilePathDefined())
            {
                var filepathEntity = (EntitySimpleBaseType)fileEffectiveRightsObject.GetItemValue(fileeffectiverights53_object_ItemsChoices.filepath);
                var filepathValues = variableEvaluator.EvaluateVariableForEntity(filepathEntity);
                filepathValues = this.ProcessOperationForFilepathEntity(filepathEntity.operation, filepathValues);


                paths.AddRange(filepathValues.Select(filepath => Path.GetDirectoryName(filepath)).Distinct());
                filenames.AddRange(filepathValues.Select(filepath => Path.GetFileName(filepath)).Distinct());
            }
            else
            {
                var pathEntity = (EntitySimpleBaseType)fileEffectiveRightsObject.GetItemValue(fileeffectiverights53_object_ItemsChoices.path);
                var pathEntityValues = variableEvaluator.EvaluateVariableForEntity(pathEntity);
                pathEntityValues = ProcessOperationForFilepathEntity(pathEntity.operation, pathEntityValues);
                paths.AddRange(pathEntityValues);

                var filenameEntity = (EntitySimpleBaseType)fileEffectiveRightsObject.GetItemValue(fileeffectiverights53_object_ItemsChoices.filename);
                if (filenameEntity == null)
                    filenames.Add(string.Empty);
                else
                {
                    var filenameEntityValues = variableEvaluator.EvaluateVariableForEntity(filenameEntity);
                    filenameEntityValues = ProcessOperationForFilenameEntity(filenameEntity.operation, filenameEntityValues, pathEntityValues); 
                    filenames.AddRange(filenameEntityValues.Distinct());
                }
            }

            var trusteeSidEntity = (EntitySimpleBaseType)fileEffectiveRightsObject.GetItemValue(fileeffectiverights53_object_ItemsChoices.trustee_sid);
            var trusteeSidEntityValues = variableEvaluator.EvaluateVariableForEntity(trusteeSidEntity);

            var itemsToCollect = new List<ItemType>();
            foreach(var path in paths)
                foreach(var filename in filenames)
                    foreach (var trusteeSID in trusteeSidEntityValues)
                    {
                        var newItems = ProcessOperationForTrusteeSidEntity(path, filename, trusteeSID, trusteeSidEntity.operation);
                        itemsToCollect.AddRange(newItems);
                    }

            return itemsToCollect;
        }

        private IEnumerable<ItemType> ProcessOperationForTrusteeSidEntity(
            string path, string filename, string trusteeSID, OperationEnumeration operation)
        {
            if (operation.Equals(OperationEnumeration.equals) || operation.Equals(OperationEnumeration.caseinsensitiveequals))
            {
                var newItemToCollect = this.NewFileEffectiveRightsItem(path, filename, trusteeSID);
                newItemToCollect.status = StatusEnumeration.notcollected;
                return new ItemType[] { newItemToCollect };
            }

            return this.ObjectCollector.CollectItems(Path.Combine(path, filename), trusteeSID, operation);
        }

        private IEnumerable<string> ProcessOperationForFilenameEntity(
            OperationEnumeration filenameEntityOperation, IEnumerable<string> filenameEntityValues, IEnumerable<string> paths)
        {
            if (filenameEntityOperation.Equals(OperationEnumeration.equals) || 
                filenameEntityOperation.Equals(OperationEnumeration.caseinsensitiveequals))
                return filenameEntityValues;

            if (filenameEntityOperation.Equals(OperationEnumeration.patternmatch))
            {
                var newFilenames = new List<String>();
                foreach (var path in paths)
                {
                    var filesFromPath = this.FileProvider.GetChildrenFiles(path);
                    foreach (var filename in filenameEntityValues)
                    {
                        var regex = new Regex(filename);
                        foreach (var fileFromPath in filesFromPath)
                            if (regex.IsMatch(fileFromPath))
                                newFilenames.Add(fileFromPath);
                    }
                }

                return newFilenames;
            }

            return null;
        }

        private IEnumerable<string> ProcessOperationForFilepathEntity(OperationEnumeration operation, IEnumerable<string> filepathValues)
        {
            if (operation.Equals(OperationEnumeration.equals) || operation.Equals(OperationEnumeration.caseinsensitiveequals))
                return filepathValues;

            if (operation.Equals(OperationEnumeration.patternmatch))
            {
                var newFilepaths = new List<String>();
                foreach (var filepath in filepathValues)
                    newFilepaths.AddRange(this.ProcessPatternMatchForFilepath(filepath));
                
                return newFilepaths;
            }

            return null;
        }

        private IEnumerable<string> ProcessPatternMatchForFilepath(string filepathWithPattern)
        {
            var pathParts = filepathWithPattern.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var fixedPart = pathParts.First();
            
            var indexOfPattern = -1;
            var allPathChildren = new List<String>();
            for (int i = 1; i < pathParts.Length; i++)
            {
                if (RegexHelper.IsPathLevelARegexPattern(pathParts[i]))
                {
                    indexOfPattern = i;
                    allPathChildren.AddRange(this.FileProvider.GetChildrenDirectories(fixedPart));
                    break;
                }
                fixedPart += string.Format("\\{0}", pathParts[i]);
            }

            var regex = new Regex(pathParts[indexOfPattern]);
            var newPaths = new List<String>();
            foreach (var children in allPathChildren)
            {
                if (regex.IsMatch(children))
                {
                    pathParts[indexOfPattern] = children;
                    newPaths.Add(string.Join("\\", pathParts));
                }
            }
            
            return newPaths;
        }

        private fileeffectiverights_item NewFileEffectiveRightsItem(string path, string filename, string trusteeSID)
        {
            return new fileeffectiverights_item()
            {
                filepath = OvalHelper.CreateItemEntityWithStringValue(Path.Combine(path, filename)),
                path = OvalHelper.CreateItemEntityWithStringValue(path),
                filename = OvalHelper.CreateItemEntityWithStringValue(filename),
                trustee_sid = OvalHelper.CreateItemEntityWithStringValue(trusteeSID)
            };
        }
        
    }

    
}
