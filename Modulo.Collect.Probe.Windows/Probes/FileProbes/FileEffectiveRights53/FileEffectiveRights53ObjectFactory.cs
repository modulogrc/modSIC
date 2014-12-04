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
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;


namespace Modulo.Collect.Probe.Windows.FileEffectiveRights53
{
    public class FileEffectiveRights53ObjectFactory
    {
        public IEnumerable<ObjectType> CreateObjectTypeByCombinationOfEntities(fileeffectiverights53_object objectType,
            IEnumerable<string> filepaths, IEnumerable<string> filenames, IEnumerable<string> paths, IEnumerable<string> trusteeSIDs)
        {
            List<ObjectType> fileContentObjects = new List<ObjectType>();
            fileContentObjects.AddRange(this.CreateFileObjects(objectType, filepaths, filenames, paths, trusteeSIDs));
            return fileContentObjects;
        }

        public IEnumerable<ObjectType> CreateFileObjects(fileeffectiverights53_object objectType,
            IEnumerable<string> filepaths, IEnumerable<string> filenames, IEnumerable<string> paths, IEnumerable<string> trusteeSIDs)
        {
            List<ObjectType> fileEffectiveRightsObjects = new List<ObjectType>();
            if (!objectType.IsFilePathDefined())
                this.CreateFileObjectsWithoutFilePath(objectType, filenames, paths, trusteeSIDs, fileEffectiveRightsObjects);
            else
                this.CreateFileObjectsWithFilePath(objectType, filepaths, trusteeSIDs, fileEffectiveRightsObjects);


            return fileEffectiveRightsObjects;
        }

        private void CreateFileObjectsWithFilePath(fileeffectiverights53_object objectType,
            IEnumerable<string> filepaths, IEnumerable<string> trusteeSIDs, List<ObjectType> fileEffectiveRightsObjects)
        {
            foreach (string filepath in filepaths)
                foreach (string trusteeSID in trusteeSIDs)
                    fileEffectiveRightsObjects.Add(this.CreateObjectTypeFrom(objectType, filepath, null, null, trusteeSID));
        }

        private void CreateFileObjectsWithoutFilePath(
            fileeffectiverights53_object objectType,
            IEnumerable<string> paths, IEnumerable<string> fileNamesOrFilePaths, IEnumerable<string> trusteeSIDs, 
            List<ObjectType> fileEffectiveRightsObjects)
        {
            foreach (string path in paths)
                foreach (string fileName in fileNamesOrFilePaths)
                    foreach (string trusteeSID in trusteeSIDs)
                        fileEffectiveRightsObjects.Add(this.CreateObjectTypeFrom(objectType, null, fileName, path, trusteeSID));
        }

        private ObjectType CreateObjectTypeFrom(fileeffectiverights53_object objectType, 
            string filepath, string filename, string path, string trusteeSID)
        {
            EntityObjectStringType filePathFrom = null;
            EntityObjectStringType pathFrom = null;
            EntityObjectStringType fileNameFrom = null;
            EntityObjectStringType newFilePath = null;
            EntityObjectStringType newPath = null;
            EntityObjectStringType newFileName = null;

            var trusteeSIDFrom = (EntityObjectStringType)objectType.GetItemValue(fileeffectiverights53_object_ItemsChoices.trustee_sid);
            var newTrusteeSID = this.CreateObjectStringTypeFrom(trusteeSIDFrom);
            newTrusteeSID.Value = string.IsNullOrEmpty(trusteeSID) ? newTrusteeSID.Value : trusteeSID;

            if (objectType.IsFilePathDefined())
            {
                filePathFrom = (EntityObjectStringType)objectType.GetItemValue(fileeffectiverights53_object_ItemsChoices.filepath);
                newFilePath = this.CreateObjectStringTypeFrom(filePathFrom);
                newFilePath.Value = string.IsNullOrEmpty(filepath) ? newFilePath.Value : filepath;
                return this.CreateFileObject(newFilePath, null, null, newTrusteeSID);
            }
            else
            {
                pathFrom = (EntityObjectStringType)objectType.GetItemValue(fileeffectiverights53_object_ItemsChoices.path);
                fileNameFrom = (EntityObjectStringType)objectType.GetItemValue(fileeffectiverights53_object_ItemsChoices.filename);
                
                newPath = this.CreateObjectStringTypeFrom(pathFrom);
                newPath.Value = string.IsNullOrEmpty(path) ? newPath.Value: path;

                var isNil = ((fileNameFrom == null) && filename.Trim().Equals(string.Empty));
                newFileName = this.CreateObjectStringTypeFrom(fileNameFrom, isNil);
                newFileName.Value = string.IsNullOrEmpty(filename) ? newFileName.Value : filename;
                
                return this.CreateFileObject(null, newFileName, newPath, newTrusteeSID);
            }

        }

        private EntityObjectStringType CreateObjectStringTypeFrom(EntityObjectStringType objectStringType, bool isNilEntity = false)
        {
            if (isNilEntity)
                return new EntityObjectStringType() { Value = string.Empty };

            return new EntityObjectStringType()
            {
                datatype = objectStringType.datatype,
                mask = objectStringType.mask,
                operation = objectStringType.operation,
                var_ref = objectStringType.var_ref,
                Value = objectStringType.Value
            };
        }


        private fileeffectiverights53_object CreateFileObject(EntityObjectStringType filePath,
                                                        EntityObjectStringType fileName,
                                                        EntityObjectStringType path,
                                                        EntityObjectStringType trusteeSID)
        {
            var newFileEffectiveRights53Object = new fileeffectiverights53_object();
            var items = new List<object>();
            var  itemChoices = new List<fileeffectiverights53_object_ItemsChoices>();
            if (filePath == null)
            {
                itemChoices.Add(fileeffectiverights53_object_ItemsChoices.path);
                itemChoices.Add(fileeffectiverights53_object_ItemsChoices.filename);
                itemChoices.Add(fileeffectiverights53_object_ItemsChoices.trustee_sid);
                items.Add(path);
                items.Add(fileName);
                items.Add(trusteeSID);
            }
            else
            {
                itemChoices.Add(fileeffectiverights53_object_ItemsChoices.filepath);
                itemChoices.Add(fileeffectiverights53_object_ItemsChoices.trustee_sid);
                items.Add(filePath);
                items.Add(trusteeSID);
            }
            
            newFileEffectiveRights53Object.Items = items.ToArray();
            newFileEffectiveRights53Object.Fileeffectiverights53ObjectItemsElementName = itemChoices.ToArray();

            return newFileEffectiveRights53Object;
        }

    }
}
