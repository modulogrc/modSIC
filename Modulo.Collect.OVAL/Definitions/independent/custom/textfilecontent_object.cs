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


using System.Xml.Serialization;
using Modulo.Collect.OVAL.Common;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Modulo.Collect.OVAL.Definitions.Independent
{
    public partial class textfilecontent_object : ObjectType
    {
        public override string ComponentString
        {
            get { return "textfilecontent"; }
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
        public Object GetItemValue(textfilecontent_ItemsChoices itemName)
        {
            for (int i = 0; i <= this.TextfilecontentItemsElementName.Count() - 1; i++)
            {
                if (this.TextfilecontentItemsElementName[i] == itemName)
                {
                    return this.Items[i];
                }
            }
            return null;
        }

        /// <summary>
        /// this method verifies if exists a set element in the items array.
        /// </summary>   
        public bool HasTheSetElement()
        {
            Object setElement = this.GetItemValue(textfilecontent_ItemsChoices.set);
            if (setElement != null)
                return true;

            return false;
        }

        public bool HasVariableDefined()
        {            
            EntityObjectStringType fileName = (EntityObjectStringType)this.GetItemValue(textfilecontent_ItemsChoices.filename);
            EntityObjectStringType path = (EntityObjectStringType)this.GetItemValue(textfilecontent_ItemsChoices.path);
            EntityObjectStringType line = (EntityObjectStringType)this.GetItemValue(textfilecontent_ItemsChoices.line);

            return ((!string.IsNullOrEmpty(path.var_ref)) || (!string.IsNullOrEmpty(fileName.var_ref)) || (!string.IsNullOrEmpty(line.var_ref)) );            
        }
    }
}
