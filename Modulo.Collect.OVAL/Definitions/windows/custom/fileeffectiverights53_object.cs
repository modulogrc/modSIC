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


using System.Xml.Serialization;
using Modulo.Collect.OVAL.Common;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Modulo.Collect.OVAL.Definitions.Windows
{
    public partial class fileeffectiverights53_object : ObjectType
    {
        public override string ComponentString
        {
            get { return "fileeffectiverights53"; }
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
        public Object GetItemValue(fileeffectiverights53_object_ItemsChoices itemName)
        {
            for (int i = 0; i <= this.Fileeffectiverights53ObjectItemsElementName.Count() - 1; i++)
                if (this.Fileeffectiverights53ObjectItemsElementName[i] == itemName)
                    return this.Items[i];
            
            return null;
        }


        public Dictionary<String, EntitySimpleBaseType> GetAllObjectEntities()
        {
            string filepathEntity = fileeffectiverights53_object_ItemsChoices.filepath.ToString();
            string pathEntity = fileeffectiverights53_object_ItemsChoices.path.ToString();
            string filenameEntity = fileeffectiverights53_object_ItemsChoices.filename.ToString();
            string trusteeSIDEntity = fileeffectiverights53_object_ItemsChoices.trustee_sid.ToString();
            string[] allEntityNames = this.Fileeffectiverights53ObjectItemsElementName.Select(i => i.ToString()).ToArray<String>();

            var allTextFileContentEntities = new Dictionary<String, EntitySimpleBaseType>();
            allTextFileContentEntities.Add(filepathEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, filepathEntity));
            allTextFileContentEntities.Add(pathEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, pathEntity));
            allTextFileContentEntities.Add(filenameEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, filenameEntity));
            allTextFileContentEntities.Add(trusteeSIDEntity, base.GetObjectEntityByName(this.Items.ToArray(), allEntityNames, trusteeSIDEntity));

            return allTextFileContentEntities;
        }

        public bool IsFilePathDefined()
        {
            EntityObjectStringType filepath = (EntityObjectStringType)this.GetItemValue(fileeffectiverights53_object_ItemsChoices.filepath);
            return filepath != null;
        }

        public bool HasVariableDefined()
        {
            if (this.hasEntityVariableReference(fileeffectiverights53_object_ItemsChoices.trustee_sid))
                return true;

            if (this.IsFilePathDefined())
            {
                if (this.hasEntityVariableReference(fileeffectiverights53_object_ItemsChoices.filepath)) 
                    return true;
            }
            else
            {
                if (this.hasEntityVariableReference(fileeffectiverights53_object_ItemsChoices.path))
                    return true;

                if (this.hasEntityVariableReference(fileeffectiverights53_object_ItemsChoices.filename))
                    return true;
            }

            return false;
        }

        private Boolean hasEntityVariableReference(fileeffectiverights53_object_ItemsChoices entityName)
        {
            var entity = (EntityObjectStringType)this.GetItemValue(entityName);
            return ((entity != null) && (!string.IsNullOrEmpty(entity.var_ref)));
        }

        /// <summary>
        /// Returns the complete path of file. This methods verifies if has a filepath entity defined
        /// and based on define the correct filepath of object.
        /// </summary>
        /// <returns></returns>
        public string GetCompleteFilepath()
        {
            if (this.IsFilePathDefined())
                return ((EntityObjectStringType)this.GetItemValue(fileeffectiverights53_object_ItemsChoices.filepath)).Value;
            else
            {
                var path = ((EntityObjectStringType)this.GetItemValue(fileeffectiverights53_object_ItemsChoices.path)).Value;
                var fileName = ((EntityObjectStringType)this.GetItemValue(fileeffectiverights53_object_ItemsChoices.filename)).Value;
                return string.Format(@"{0}\{1}", path, fileName);
            }

        }
    }
}
