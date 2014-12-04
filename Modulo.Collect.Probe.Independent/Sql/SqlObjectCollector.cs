/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
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
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Exceptions;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Independent.Common.Sql;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Independent.Sql
{
    public class SqlObjectCollector : BaseObjectCollector
    {
        private SqlQueryProvider SqlQueryProvider;

        public SqlObjectCollector(SqlQueryProvider SqlQueryProvider)
        {
            this.SqlQueryProvider = SqlQueryProvider;
        }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var sqlItem = (sql_item)systemItem;
            var dbEngine = new DbEngine(sqlItem.engine.Value).Engine;
            var connectionString = sqlItem.connection_string.Value;
            var sqlQuery = sqlItem.sql.Value;

            try
            {
                var queryResult = this.SqlQueryProvider.ExecuteSQL(dbEngine, connectionString, sqlQuery);
                sqlItem.result = queryResult.ToOvalSimpleTypeList().ToArray();
            }
            catch (SqlNoResultException)
            {
                sqlItem.status = StatusEnumeration.doesnotexist;
            }
            catch (Exception ex)
            {
                sqlItem.status = StatusEnumeration.error;
                sqlItem.message = new MessageType[] { new MessageType() { level = MessageLevelEnumeration.error, Value = ex.Message } };
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(sqlItem, BuildExecutionLog());
        }
    }
}
