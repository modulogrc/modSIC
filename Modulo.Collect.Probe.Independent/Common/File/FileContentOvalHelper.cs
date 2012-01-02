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
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Independent.Common.File
{
    public class FileContentOvalHelper
    {
        public static Dictionary<string, EntityObjectStringType> GetFileContentEntitiesFromObjectType(textfilecontent_object fileContentObject)
        {
            string fileNameEntityName = textfilecontent_ItemsChoices.filename.ToString();
            string lineEntityName = textfilecontent_ItemsChoices.line.ToString();
            string pathEntityName = textfilecontent_ItemsChoices.path.ToString();

            object[] allEntities = fileContentObject.Items.ToArray();
            string[] allEntityNames = fileContentObject.TextfilecontentItemsElementName.Select(i => i.ToString()).ToArray<String>();

            Dictionary<String, EntityObjectStringType> fileContentEntities = new Dictionary<string, EntityObjectStringType>();
            fileContentEntities.Add(fileNameEntityName, OvalHelper.GetEntityObjectByName(fileNameEntityName, allEntities, allEntityNames));
            fileContentEntities.Add(lineEntityName, OvalHelper.GetEntityObjectByName(lineEntityName, allEntities, allEntityNames));
            fileContentEntities.Add(pathEntityName, OvalHelper.GetEntityObjectByName(pathEntityName, allEntities, allEntityNames));

            return fileContentEntities;
        }

        public static Dictionary<string, EntitySimpleBaseType> GetFileContentEntitiesFromObjectType(textfilecontent54_object textFileContentObject)
        {
            string fileName = textfilecontent54_ItemsChoices.filename.ToString();
            string filePath = textfilecontent54_ItemsChoices.filepath.ToString();
            string path = textfilecontent54_ItemsChoices.path.ToString();
            string pattern = textfilecontent54_ItemsChoices.pattern.ToString();
            string instance = textfilecontent54_ItemsChoices.instance.ToString();

            object[] allEntities = textFileContentObject.Items.ToArray();
            string[] allEntityNames = textFileContentObject.Textfilecontent54ItemsElementName.Select(i => i.ToString()).ToArray<String>();

            Dictionary<String, EntitySimpleBaseType> fileContentEntities = new Dictionary<string, EntitySimpleBaseType>();
            if (textFileContentObject.IsFilePathDefined())
            {
                fileContentEntities.Add(filePath, OvalHelper.GetEntityObjectByName(filePath, allEntities, allEntityNames));
            }
            else
            {
                fileContentEntities.Add(fileName, OvalHelper.GetEntityObjectByName(fileName, allEntities, allEntityNames));
                fileContentEntities.Add(path, OvalHelper.GetEntityObjectByName(path, allEntities, allEntityNames));
            }

            fileContentEntities.Add(pattern, OvalHelper.GetEntityObjectByName(pattern, allEntities, allEntityNames));
            fileContentEntities.Add(instance, OvalHelper.GetEntityObjectIntTypeByName(instance, allEntities, allEntityNames));
            return fileContentEntities;

        }
    }
}
