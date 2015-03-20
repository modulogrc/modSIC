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
using Modulo.Collect.OVAL.Definitions.helpers;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Independent.Common.File
{
    public class FileObjectTypeFactory
    {
        public IEnumerable<ObjectType> CreateObjectTypeByCombinationOfEntities(file_object fileObject, IEnumerable<string> filePaths, IEnumerable<string> paths, IEnumerable<string> fileNames)
        {
            var fileObjects = new List<ObjectType>();
            if (fileObject.IsFilePathSet())
                fileObjects.AddRange(CreateFileObjectWithFilePath(fileObject, filePaths));
            else
                fileObjects.AddRange(this.CreateFileObjectWithoutFilePath(fileObject, paths, fileNames));

            return fileObjects;
        }

        public IEnumerable<ObjectType> CreateObjectTypeByCombinationOfEntities(OVAL.Definitions.Unix.file_object fileObject, IEnumerable<string> filePaths, IEnumerable<string> paths, IEnumerable<string> fileNames)
        {
            var fileObjects = new List<ObjectType>();
            if (fileObject.IsFilePathSet())
                fileObjects.AddRange(CreateFileObjectWithFilePath(fileObject, filePaths));
            else
                fileObjects.AddRange(this.CreateFileObjectWithoutFilePath(fileObject, paths, fileNames));

            return fileObjects;
        }

        private IEnumerable<ObjectType> CreateFileObjectWithoutFilePath(ObjectType fileObject, IEnumerable<string> paths, IEnumerable<string> fileNames)
        {
            List<ObjectType> fileObjects = new List<ObjectType>();
            foreach (string path in paths)
            {
                foreach (string fileName in fileNames)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        ObjectType newFileObject = this.CreateObjectTypeFrom(fileObject, " ", path, fileName);
                        fileObjects.Add(newFileObject);
                    }
                }
            }
            return fileObjects;
        }

        private IEnumerable<ObjectType> CreateFileObjectWithFilePath(ObjectType fileObject, IEnumerable<string> variablesFromFilePath)
        {
            List<ObjectType> fileObjects = new List<ObjectType>();
            if (variablesFromFilePath == null)
                return fileObjects;

            foreach (string filePath in variablesFromFilePath)
            {
                if (String.IsNullOrWhiteSpace(filePath))
                    continue;
                ObjectType newFileObject = this.CreateObjectTypeFrom(fileObject, filePath, "", "");
                fileObjects.Add(newFileObject);
            }
            return fileObjects;
        }

        private object GetFileEntity(ObjectType fileObject, string entityName)
        {
            if (fileObject is OVAL.Definitions.Unix.file_object)
            {
                var unixFilepath = (OVAL.Definitions.Unix.file_object)fileObject;
                if (entityName.Equals("filepath"))
                    return unixFilepath.GetItemValue(OVAL.Definitions.Unix.ItemsChoiceType3.filepath);
                
                if (entityName.Equals("path"))
                    return unixFilepath.GetItemValue(OVAL.Definitions.Unix.ItemsChoiceType3.path);

                if (entityName.Equals("filename"))
                    return unixFilepath.GetItemValue(OVAL.Definitions.Unix.ItemsChoiceType3.filename);

                return null;
            }

            if (entityName.Equals("filepath"))
                return ((file_object)fileObject).GetItemValue(file_object_ItemsChoices.filepath);

            if (entityName.Equals("path"))
                return ((file_object)fileObject).GetItemValue(file_object_ItemsChoices.path);

            if (entityName.Equals("filename"))
                return ((file_object)fileObject).GetItemValue(file_object_ItemsChoices.filename);

            return null;
        }


        private ObjectType CreateObjectTypeFrom(ObjectType fileObject, string filePath, string path, string fileName)
        {
            //EntityObjectStringType filePathFrom = (EntityObjectStringType) fileObject.GetItemValue(file_object_ItemsChoices.filepath);
            //EntityObjectStringType pathFrom = (EntityObjectStringType) fileObject.GetItemValue(file_object_ItemsChoices.path);
            //EntityObjectStringType fileNameFrom = (EntityObjectStringType)fileObject.GetItemValue(file_object_ItemsChoices.filename);

            var filePathFrom = (EntityObjectStringType)GetFileEntity(fileObject, "filepath");
            var pathFrom = (EntityObjectStringType)GetFileEntity(fileObject, "path");
            var fileNameFrom = (EntityObjectStringType)GetFileEntity(fileObject, "filename");
            
            EntityObjectStringType newfilePath = null;
            EntityObjectStringType newPath = null;
            EntityObjectStringType newFileName = null;
            //if (fileObject.IsFilePathSet())
            if (filePathFrom != null)
            {
                newfilePath = this.CreateObjectStringTypeFrom(filePathFrom);
                newfilePath.Value = !string.IsNullOrEmpty(filePath) ? filePath : newfilePath.Value;
            }
            else 
            {
                newPath = this.CreateObjectStringTypeFrom(pathFrom);
                newPath.Value = !string.IsNullOrEmpty(path) ? path : newPath.Value;

                if (fileNameFrom == null)
                {
                    newFileName = new EntityObjectStringType() { Value = string.Empty };
                }
                else
                {
                    newFileName = this.CreateObjectStringTypeFrom(fileNameFrom);
                    newFileName.Value = fileName == null ? newFileName.Value : fileName;
                }
            }
            
            if (fileObject is OVAL.Definitions.Unix.file_object)
                return this.CreateUnixFileObject(newfilePath, newPath, newFileName);
            
            return this.CreateFileObject(newfilePath,newPath, newFileName);
        }

        private file_object CreateFileObject(EntityObjectStringType filePath, EntityObjectStringType path, EntityObjectStringType fileName)
        {
            file_object fileObject = new file_object();
            EntityObjectStringType[] items;
            file_object_ItemsChoices[] itemChoices;
            if (filePath != null)
            {
                items = new EntityObjectStringType[1];
                itemChoices = new file_object_ItemsChoices[1];
                items[0] = filePath;
                itemChoices[0] = file_object_ItemsChoices.filepath;
            }
            else
            {
                items = new EntityObjectStringType[2];
                itemChoices = new file_object_ItemsChoices[2];
                items[0] = path;
                items[1] = fileName;
                itemChoices[0] = file_object_ItemsChoices.path;
                itemChoices[1] = file_object_ItemsChoices.filename;
            }
            fileObject.Items = items;
            fileObject.FileObjectItemsElementName = itemChoices;
            return fileObject;
        }

        private OVAL.Definitions.Unix.file_object CreateUnixFileObject(EntityObjectStringType filePath, EntityObjectStringType path, EntityObjectStringType fileName)
        {
            var fileObject = new OVAL.Definitions.Unix.file_object();
            EntityObjectStringType[] items;
            OVAL.Definitions.Unix.ItemsChoiceType3[] itemChoices;
            if (filePath != null)
            {
                items = new EntityObjectStringType[1];
                itemChoices = new OVAL.Definitions.Unix.ItemsChoiceType3[1];
                items[0] = filePath;
                itemChoices[0] = OVAL.Definitions.Unix.ItemsChoiceType3.filepath;
            }
            else
            {
                items = new EntityObjectStringType[2];
                itemChoices = new OVAL.Definitions.Unix.ItemsChoiceType3[2];
                items[0] = path;
                items[1] = fileName;
                itemChoices[0] = OVAL.Definitions.Unix.ItemsChoiceType3.path;
                itemChoices[1] = OVAL.Definitions.Unix.ItemsChoiceType3.filename;
            }
            fileObject.Items = items;
            fileObject.ItemsElementName = itemChoices;
            return fileObject;
        }

        private EntityObjectStringType CreateObjectStringTypeFrom(EntityObjectStringType objectStringType)
        {
            EntityBaseTypeFactory factory = new EntityBaseTypeFactory();
            return factory.CreateEntityBasedOn<EntityObjectStringType>(objectStringType);           
        }
    }
}
