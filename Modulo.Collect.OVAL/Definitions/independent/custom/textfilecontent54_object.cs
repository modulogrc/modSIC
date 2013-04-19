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


using System.Xml.Serialization;
using Modulo.Collect.OVAL.Common;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Modulo.Collect.OVAL.Definitions.Independent
{
    public partial class textfilecontent54_object : ObjectType
    {
        public override string ComponentString
        {
            get { return "textfilecontent54"; }
        }

        public override IEnumerable<EntitySimpleBaseType> GetEntityBaseTypes()
        {
            try
            {
                IEnumerable<EntitySimpleBaseType> entities = this.Items.OfType<EntitySimpleBaseType>();
                return entities;
            }
            catch (Exception)
            {
                return new List<EntitySimpleBaseType>();
            }
        }

        /// <summary>
        /// Gets the item value from the items array.
        /// This method makes the combination between ItemsElementName and Items array
        /// to the get value.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        public Object GetItemValue(textfilecontent54_ItemsChoices itemName)
        {
            for (int i = 0; i <= this.Textfilecontent54ItemsElementName.Count() - 1; i++)
            {
                if (this.Textfilecontent54ItemsElementName[i] == itemName)
                {
                    return this.Items[i];
                }
            }
            return null;
        }


        public Dictionary<String, EntitySimpleBaseType> GetAllObjectEntities()
        {
            string filepathEntity = textfilecontent54_ItemsChoices.filepath.ToString();
            string pathEntity = textfilecontent54_ItemsChoices.path.ToString();
            string filenameEntity = textfilecontent54_ItemsChoices.filename.ToString();
            string patternEntity = textfilecontent54_ItemsChoices.pattern.ToString();
            string instanceEntity = textfilecontent54_ItemsChoices.instance.ToString();

            string[] allEntityNames = this.Textfilecontent54ItemsElementName.Select(i => i.ToString()).ToArray<String>();

            Dictionary<String, EntitySimpleBaseType> allTextFileContentEntities = new Dictionary<String, EntitySimpleBaseType>();
            allTextFileContentEntities.Add(filepathEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, filepathEntity));
            allTextFileContentEntities.Add(pathEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, pathEntity));
            allTextFileContentEntities.Add(filenameEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, filenameEntity));
            allTextFileContentEntities.Add(patternEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, patternEntity));
            allTextFileContentEntities.Add(instanceEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, instanceEntity));

            return allTextFileContentEntities;
        }

        public bool IsFilePathDefined()
        {
            EntityObjectStringType filepath = (EntityObjectStringType)this.GetItemValue(textfilecontent54_ItemsChoices.filepath);
            return filepath != null;
        }

        public bool HasVariableDefined()
        {
            EntityObjectStringType filePath = null;
            EntityObjectStringType fileName = null;
            EntityObjectStringType path = null;

            EntityObjectStringType pattern = (EntityObjectStringType)this.GetItemValue(textfilecontent54_ItemsChoices.pattern);
            EntityObjectIntType instance = (EntityObjectIntType)this.GetItemValue(textfilecontent54_ItemsChoices.instance);
            if (this.IsFilePathDefined())
            {
                filePath = (EntityObjectStringType)this.GetItemValue(textfilecontent54_ItemsChoices.filepath);
                return ((!string.IsNullOrEmpty(filePath.var_ref)) || (!string.IsNullOrEmpty(pattern.var_ref)) || (!string.IsNullOrEmpty(instance.var_ref)));
            }
            else
            {
                fileName = (EntityObjectStringType)this.GetItemValue(textfilecontent54_ItemsChoices.filename);
                path = (EntityObjectStringType)this.GetItemValue(textfilecontent54_ItemsChoices.path);
                return ((!string.IsNullOrEmpty(path.var_ref)) || (!string.IsNullOrEmpty(fileName.var_ref)) || (!string.IsNullOrEmpty(pattern.var_ref)) || (!string.IsNullOrEmpty(instance.var_ref)));
            }           
        }

        /// <summary>
        /// Returns the complete path of file. This methods verifies if has a filepath entity defined
        /// and based on define the correct filepath of object.
        /// </summary>
        /// <returns></returns>
        public string GetCompleteFilepath()
        {
            if (this.IsFilePathDefined())
                return ((EntityObjectStringType)this.GetItemValue(textfilecontent54_ItemsChoices.filepath)).Value;
            else
            {
                var path = ((EntityObjectStringType)this.GetItemValue(textfilecontent54_ItemsChoices.path)).Value;
                var fileName = ((EntityObjectStringType)this.GetItemValue(textfilecontent54_ItemsChoices.filename)).Value;
                return string.Format(@"{0}\{1}", path, fileName);
            }
            
        }

        public bool IsMultiline()
        {
            foreach (var item in this.Items)
                if (item is Textfilecontent54Behaviors)
                    return ((Textfilecontent54Behaviors)item).multiline;

            return true;
        }

        public bool IsSingleline()
        {
            foreach (var item in this.Items)
                if (item is Textfilecontent54Behaviors)
                    return ((Textfilecontent54Behaviors)item).singleline;

            return true;
        }
    }
}
