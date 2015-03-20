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
using Modulo.Collect.OVAL.Common.XmlSignatures;
using System;
using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.Definitions.EntityOperations;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.OVAL.Definitions
{
    [System.ComponentModel.Composition.InheritedExport]
    public partial class ObjectType
    {
        public virtual string ComponentString { 
            get 
            { 
                var typeName = GetType().Name;
                return typeName.Substring(0, typeName.IndexOf("_object"));
            }
        }

        public virtual IEnumerable<EntitySimpleBaseType> GetEntityBaseTypes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the object type has some entity that references a variable id.
        /// </summary>
        /// <param name="variableId">The variable id.</param>
        /// <returns></returns>
        public virtual bool HasReferenceForVariable(string variableId)
        {
            IEnumerable<EntitySimpleBaseType> entities = this.GetEntityBaseTypes();
            EntitySimpleBaseType entity = entities.Where<EntitySimpleBaseType>(e => e.var_ref == variableId).FirstOrDefault<EntitySimpleBaseType>();
            return entity != null;
        }

        public IEnumerable<String> ApplyOperationForEntity(OperationEnumeration entityOperation, Dictionary<string, object> operationParameters)
        {
            OvalEntityOperationBase operation = OvalEntityOperationFactory.CreateOperationForOvalEntity(entityOperation);
            return operation.Apply(operationParameters);
        }

        public EntitySimpleBaseType GetObjectEntityByName(object[] allEntities, string[] allEntityNames, string entityName)
        {
            for (int i = 0; i < allEntityNames.Length; i++)
                if (entityName.Equals(allEntityNames[i].ToString()))
                    return (allEntities[i] as EntitySimpleBaseType);

            return null;
        }
    }
}
