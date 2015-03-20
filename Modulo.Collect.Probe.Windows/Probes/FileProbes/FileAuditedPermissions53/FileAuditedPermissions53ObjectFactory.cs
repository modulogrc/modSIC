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
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.FileAuditedPermissions53
{
    public class FileAuditedPermissions53ObjectFactory
    {
        public IEnumerable<ObjectType> CreateFileAuditedPermissions53Objects(
            fileauditedpermissions53_object objectType, 
            string[] filepaths, string[] paths, string[] filenames, string[] trusteeSIDs)
        {
            var objects = new List<ObjectType>();
            if (!objectType.IsFilePathDefined())
                this.CreateFileObjectsWithoutFilePath(objectType, filenames, paths, trusteeSIDs, objects);
            else
                this.CreateFileObjectsWithFilePath(objectType, filepaths, trusteeSIDs, objects);

            return objects;
        }

        private void CreateFileObjectsWithFilePath(fileauditedpermissions53_object objectType,
            string[] filepaths, string[] trusteeSIDs, List<ObjectType> fileAuditedObjects)
        {
            foreach (string filepath in filepaths)
                foreach (string trusteeSID in trusteeSIDs)
                    fileAuditedObjects.Add(this.CreateObjectTypeFrom(objectType, filepath, null, null, trusteeSID));
        }

        private void CreateFileObjectsWithoutFilePath(fileauditedpermissions53_object objectType,
            string[] paths, string[] fileNamesOrFilePaths, string[] trusteeSIDs, List<ObjectType> fileAuditedObjects)
        {
            foreach (string path in paths)
                foreach (string fileName in fileNamesOrFilePaths)
                    foreach (string trusteeSID in trusteeSIDs)
                        fileAuditedObjects.Add(this.CreateObjectTypeFrom(objectType, null, fileName, path, trusteeSID));
        }


        private ObjectType CreateObjectTypeFrom(fileauditedpermissions53_object objectType,
            string filepath, string filename, string path, string trusteeSID)
        {
            EntityObjectStringType filePathFrom = null;
            EntityObjectStringType pathFrom = null;
            EntityObjectStringType fileNameFrom = null;
            EntityObjectStringType newFilePath = null;
            EntityObjectStringType newPath = null;
            EntityObjectStringType newFileName = null;

            var trusteeSIDfrom = (EntityObjectStringType)objectType.GetItemValue(fileauditedpermissions53_objectItemsChoices.trustee_sid);
            var newTrusteeSID = this.CreateObjectStringTypeFrom(trusteeSIDfrom);
            newTrusteeSID.Value = string.IsNullOrEmpty(trusteeSID) ? newTrusteeSID.Value : trusteeSID;

            if (objectType.IsFilePathDefined())
            {
                filePathFrom = (EntityObjectStringType)objectType.GetItemValue(fileauditedpermissions53_objectItemsChoices.filepath);
                newFilePath = this.CreateObjectStringTypeFrom(filePathFrom);
                newFilePath.Value = string.IsNullOrEmpty(filepath) ? newFilePath.Value : filepath;
                return this.CreateFileObject(newFilePath, null, null, newTrusteeSID);
            }
            else
            {
                pathFrom = (EntityObjectStringType)objectType.GetItemValue(fileauditedpermissions53_objectItemsChoices.path);
                fileNameFrom = (EntityObjectStringType)objectType.GetItemValue(fileauditedpermissions53_objectItemsChoices.filename);

                newPath = this.CreateObjectStringTypeFrom(pathFrom);
                newPath.Value = string.IsNullOrEmpty(path) ? newPath.Value : path;

                newFileName = this.CreateObjectStringTypeFrom(fileNameFrom);
                newFileName.Value = string.IsNullOrEmpty(filename) ? newFileName.Value : filename;

                return this.CreateFileObject(null, newFileName, newPath, newTrusteeSID);
            }

        }

        private EntityObjectStringType CreateObjectStringTypeFrom(EntityObjectStringType objectStringType)
        {
            return new EntityObjectStringType()
            {
                datatype = objectStringType.datatype,
                mask = objectStringType.mask,
                operation = objectStringType.operation,
                var_ref = objectStringType.var_ref,
                Value = objectStringType.Value
            };
        }

        private fileauditedpermissions53_object CreateFileObject(
            EntitySimpleBaseType filePath, EntitySimpleBaseType fileName, EntitySimpleBaseType path, EntitySimpleBaseType xpath)
        {
            
            EntitySimpleBaseType[] items;
            fileauditedpermissions53_objectItemsChoices[] itemChoices;
            if (filePath == null)
            {
                items = new EntitySimpleBaseType[4];
                itemChoices = new fileauditedpermissions53_objectItemsChoices[3];
                itemChoices[0] = fileauditedpermissions53_objectItemsChoices.path;
                itemChoices[1] = fileauditedpermissions53_objectItemsChoices.filename;
                itemChoices[2] = fileauditedpermissions53_objectItemsChoices.trustee_sid;
                items[0] = path;
                items[1] = fileName;
                items[2] = xpath;
            }
            else
            {
                items = new EntitySimpleBaseType[3];
                itemChoices = new fileauditedpermissions53_objectItemsChoices[2];
                itemChoices[0] = fileauditedpermissions53_objectItemsChoices.filepath;
                itemChoices[1] = fileauditedpermissions53_objectItemsChoices.trustee_sid;
                items[0] = filePath;
                items[1] = xpath;
            }

            return new fileauditedpermissions53_object() { Items = items, Fileauditedpermissions53ObjectItemsElementName = itemChoices };
        }
    }
}
