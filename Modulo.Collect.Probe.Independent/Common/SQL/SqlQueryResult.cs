/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Independent.Common.Sql
{
    public class SqlQueryResult
    {
        public IList<Dictionary<string, object>> Records { get; private set; }

        public SqlQueryResult()
        {
            this.Records = new List<Dictionary<string, object>>();
        }

        public void AddRecord(Dictionary<string, object> record)
        {
            this.Records.Add(record);
        }

        public IEnumerable<EntityItemAnySimpleType> ToOvalSimpleTypeList()
        {
            if (this.Records.Count <= 0)
                return new EntityItemAnySimpleType[] { new EntityItemAnySimpleType() { status = StatusEnumeration.doesnotexist } };
            
            var result = new List<EntityItemAnySimpleType>();
            foreach (var record in this.Records)
            {
                var newSimpleType = new EntityItemAnySimpleType();
                
                var field = record.FirstOrDefault();
                if ((field.Key == null) || (field.Value == null))
                    newSimpleType.status = StatusEnumeration.doesnotexist;
                else
                    newSimpleType.Value = field.Value.ToString();

                result.Add(newSimpleType);
            }

            return result;

        }

        public IEnumerable<EntityItemRecordType> ToOvalRecordTypeList()
        {
            if (this.Records.Count <= 0)
                return new EntityItemRecordType[] { new EntityItemRecordType() { status = StatusEnumeration.doesnotexist } };

            var result = new List<EntityItemRecordType>();
            foreach (var record in this.Records)
            {
                var newOvalRecordFields = new List<EntityItemFieldType>();
                foreach (var recordField in record)
                {
                    var newRecordField = CreateEntityItemFieldType(recordField.Key, recordField.Value);
                    newOvalRecordFields.Add(newRecordField);
                }

                result.Add(new EntityItemRecordType() { field = newOvalRecordFields.ToArray() });
            }
            
            return result;
        }

        private EntityItemFieldType CreateEntityItemFieldType(string fieldName, object fieldValue)
        {
            var newFieldType = new EntityItemFieldType() { name = fieldName };
            if (fieldValue == null)
                newFieldType.status = StatusEnumeration.doesnotexist;
            else
                newFieldType.Value = fieldValue.ToString();

            return newFieldType;
        }

    }
}
