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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modulo.Collect.OVAL.Definitions.Independent
{
    public partial class xmlfilecontent_object : ObjectType
    {
        public override string ComponentString
        {
            get { return "xmlfilecontent"; }
        }

        public override IEnumerable<EntitySimpleBaseType> GetEntityBaseTypes()
        {
            try
            {
                IEnumerable<EntitySimpleBaseType> entities = this.Items.OfType<EntitySimpleBaseType>();
                return entities;
            }
            catch (Exception ex)
            {
                return new List<EntitySimpleBaseType>();
            }
        }

        public Object GetItemValue(xmlfilecontent_ItemsChoices itemName)
        {
         
            for (int i = 0; i <= this.ItemsElementName.Count() - 1; i++)
                if (this.ItemsElementName[i] == itemName)
                    return this.Items[i];

            return null;
        }

        public bool IsFilePathDefined()
        {
            var filepath = (EntityObjectStringType)this.GetItemValue(xmlfilecontent_ItemsChoices.filepath);
            return filepath != null;
        }



        public Dictionary<string, EntitySimpleBaseType> GetAllObjectEntities()
        {
            var filepathEntity = (EntitySimpleBaseType)this.GetItemValue(xmlfilecontent_ItemsChoices.filepath);
            var pathEntity = (EntitySimpleBaseType)this.GetItemValue(xmlfilecontent_ItemsChoices.path);
            var filenameEntity = (EntitySimpleBaseType)this.GetItemValue(xmlfilecontent_ItemsChoices.filename);
            var xpathEntity = (EntitySimpleBaseType)this.GetItemValue(xmlfilecontent_ItemsChoices.xpath);

            var allEntities = new Dictionary<string, EntitySimpleBaseType>();
            allEntities.Add(xmlfilecontent_ItemsChoices.filepath.ToString(), filepathEntity);
            allEntities.Add(xmlfilecontent_ItemsChoices.path.ToString(), pathEntity);
            allEntities.Add(xmlfilecontent_ItemsChoices.filename.ToString(), filenameEntity);
            allEntities.Add(xmlfilecontent_ItemsChoices.xpath.ToString(), xpathEntity);

            return allEntities;
        }

        /// <summary>
        /// Returns the complete path of file. This methods verifies if has a filepath entity defined
        /// and based on define the correct filepath of object.
        /// </summary>
        /// <returns></returns>
        public string GetCompleteFilepath()
        {
            if (this.IsFilePathDefined())
                return ((EntityObjectStringType)this.GetItemValue(xmlfilecontent_ItemsChoices.filepath)).Value;
            else
            {
                var path = ((EntityObjectStringType)this.GetItemValue(xmlfilecontent_ItemsChoices.path)).Value;
                var fileName = ((EntityObjectStringType)this.GetItemValue(xmlfilecontent_ItemsChoices.filename)).Value;
                return string.Format(@"{0}\{1}", path, fileName);
            }

        }
    }
}
