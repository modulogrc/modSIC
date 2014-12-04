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
using Modulo.Collect.OVAL.Definitions.helpers;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.FileEffectiveRights
{
    public class FileEffectiveRightsObjectTypeFactory
    {
        private EntityBaseTypeFactory entityBaseTypeFactory = new EntityBaseTypeFactory();

        public IEnumerable<ObjectType> CreateObjectTypeByCombinationOfEntities(fileeffectiverights_object fileEffectiveRights,IEnumerable<string> paths, IEnumerable<string> fileNames, IEnumerable<string> trustee_names)
        {
            List<ObjectType> fileEffectiveRightsObjects = new List<ObjectType>();

            foreach (string path in paths)
            {
                foreach (string fileName in fileNames)
                    foreach (string trustee_name in trustee_names)
                        fileEffectiveRightsObjects.Add(this.CreateObjectTypeFrom(fileEffectiveRights, path, fileName, trustee_name));
            }

            return fileEffectiveRightsObjects;               
        }


        private ObjectType CreateObjectTypeFrom(fileeffectiverights_object fileEffectiveRights, string path, string fileName, string trustee_name)
        {
            EntityObjectStringType fileNameFrom = (EntityObjectStringType)fileEffectiveRights.GetItemValue(fileeffectiverights_object_ItemsChoices.filename);
            EntityObjectStringType pathFrom = (EntityObjectStringType)fileEffectiveRights.GetItemValue(fileeffectiverights_object_ItemsChoices.path);
            EntityObjectStringType trustee_nameFrom = (EntityObjectStringType)fileEffectiveRights.GetItemValue(fileeffectiverights_object_ItemsChoices.trustee_name);

            EntityObjectStringType newFileName = entityBaseTypeFactory.CreateEntityBasedOn<EntityObjectStringType>(fileNameFrom);
            newFileName.Value = !string.IsNullOrEmpty(fileName) ? fileName : newFileName.Value;
            EntityObjectStringType newPath = entityBaseTypeFactory.CreateEntityBasedOn<EntityObjectStringType>(pathFrom);
            newPath.Value = !string.IsNullOrEmpty(path) ? path : newPath.Value;
            EntityObjectStringType newTrustee_name = entityBaseTypeFactory.CreateEntityBasedOn<EntityObjectStringType>(trustee_nameFrom);
            newTrustee_name.Value = !string.IsNullOrEmpty(trustee_name) ? trustee_name : newTrustee_name.Value;

            return this.CreateFileEffectiveRights(newFileName, newPath, newTrustee_name);

        }

        private ObjectType CreateFileEffectiveRights(EntityObjectStringType newFileName, EntityObjectStringType newPath, EntityObjectStringType newTrustee_name)
        {
            List<EntityObjectStringType> items = new List<EntityObjectStringType>();
            List<fileeffectiverights_object_ItemsChoices> choices = new List<fileeffectiverights_object_ItemsChoices>();

            items.Add(newFileName);
            choices.Add(fileeffectiverights_object_ItemsChoices.filename);
            items.Add(newPath);
            choices.Add(fileeffectiverights_object_ItemsChoices.path);
            items.Add(newTrustee_name);
            choices.Add(fileeffectiverights_object_ItemsChoices.trustee_name);

            fileeffectiverights_object newFileEffectiveRights = new fileeffectiverights_object();
            newFileEffectiveRights.Items = items.ToArray();
            newFileEffectiveRights.FileeffectiverightsObjectItemsElementName = choices.ToArray();

            return newFileEffectiveRights;
        }
    }
}
