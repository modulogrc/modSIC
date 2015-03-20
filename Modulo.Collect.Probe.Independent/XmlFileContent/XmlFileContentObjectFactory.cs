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
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Independent.XmlFileContent
{
    public class XmlFileContentObjectFactory
    {
        public IEnumerable<ObjectType> CreateObjectTypeByCombinationOfEntities(xmlfilecontent_object objectType,
            string[] filepaths, string[] filenames, string[] paths, string[] xpaths)
        {
            return this.CreateXmlFileContentObjects(objectType, filepaths, filenames, paths, xpaths);
        }

        public IEnumerable<ObjectType> CreateXmlFileContentObjects(xmlfilecontent_object objectType,
            string[] filepaths, string[] filenames, string[] paths, string[] xpaths)
        {
            var objects = new List<ObjectType>();
            if (!objectType.IsFilePathDefined())
                this.CreateFileObjectsWithoutFilePath(objectType, filenames, paths, xpaths, objects);
            else
                this.CreateFileObjectsWithFilePath(objectType, filepaths, xpaths, objects);

            return objects;
        }


        private void CreateFileObjectsWithFilePath(xmlfilecontent_object objectType,
            string[] filepaths, string[] xpaths, List<ObjectType> xmlFileObjects)
        {
            foreach (string filepath in filepaths)
                foreach (string xpath in xpaths)
                    xmlFileObjects.Add(this.CreateObjectTypeFrom(objectType, filepath, null, null, xpath));
        }

        private void CreateFileObjectsWithoutFilePath(xmlfilecontent_object objectType,
            string[] paths, string[] fileNamesOrFilePaths, string[] xpaths, List<ObjectType> xmlFileObjects)
        {
            foreach (string path in paths)
                foreach (string fileName in fileNamesOrFilePaths)
                    foreach (string xpath in xpaths)
                        xmlFileObjects.Add(this.CreateObjectTypeFrom(objectType, null, fileName, path, xpath));
        }

        private ObjectType CreateObjectTypeFrom(xmlfilecontent_object objectType,
            string filepath, string filename, string path, string xpath)
        {
            EntityObjectStringType filePathFrom = null;
            EntityObjectStringType pathFrom = null;
            EntityObjectStringType fileNameFrom = null;
            EntityObjectStringType newFilePath = null;
            EntityObjectStringType newPath = null;
            EntityObjectStringType newFileName = null;

            var xpathFrom = (EntityObjectStringType)objectType.GetItemValue(xmlfilecontent_ItemsChoices.xpath);
            var newTrusteeSID = this.CreateObjectStringTypeFrom(xpathFrom);
            newTrusteeSID.Value = string.IsNullOrEmpty(xpath) ? newTrusteeSID.Value : xpath;

            if (objectType.IsFilePathDefined())
            {
                filePathFrom = (EntityObjectStringType)objectType.GetItemValue(xmlfilecontent_ItemsChoices.filepath);
                newFilePath = this.CreateObjectStringTypeFrom(filePathFrom);
                newFilePath.Value = string.IsNullOrEmpty(filepath) ? newFilePath.Value : filepath;
                return this.CreateFileObject(newFilePath, null, null, newTrusteeSID);
            }
            else
            {
                pathFrom = (EntityObjectStringType)objectType.GetItemValue(xmlfilecontent_ItemsChoices.path);
                fileNameFrom = (EntityObjectStringType)objectType.GetItemValue(xmlfilecontent_ItemsChoices.filename);

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

        private xmlfilecontent_object CreateFileObject(
            EntitySimpleBaseType filePath, EntitySimpleBaseType fileName, EntitySimpleBaseType path, EntitySimpleBaseType xpath)
        {
            var newXmlFileContentObject = new xmlfilecontent_object();
            EntitySimpleBaseType[] items;
            xmlfilecontent_ItemsChoices[] itemChoices;
            if (filePath == null)
            {
                items = new EntitySimpleBaseType[4];
                itemChoices = new xmlfilecontent_ItemsChoices[3];
                itemChoices[0] = xmlfilecontent_ItemsChoices.path;
                itemChoices[1] = xmlfilecontent_ItemsChoices.filename;
                itemChoices[2] = xmlfilecontent_ItemsChoices.xpath;
                items[0] = path;
                items[1] = fileName;
                items[2] = xpath;
            }
            else
            {
                items = new EntitySimpleBaseType[3];
                itemChoices = new xmlfilecontent_ItemsChoices[2];
                itemChoices[0] = xmlfilecontent_ItemsChoices.filepath;
                itemChoices[1] = xmlfilecontent_ItemsChoices.xpath;
                items[0] = filePath;
                items[1] = xpath;
            }

            newXmlFileContentObject.Items = items;
            newXmlFileContentObject.XmlfilecontentItemsElementName = itemChoices;

            return newXmlFileContentObject;
        }

    }
}
