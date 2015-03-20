/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Common.Test;
using Modulo.Collect.Probe.Independent.Sql;
using Modulo.Collect.Probe.Independent.Sql57;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Test.Helpers;

namespace Modulo.Collect.Probe.Independent.Tests.Probes.SQL
{
    [TestClass]
    public class SqlProberTests: ProberTestBase
    {
        private const string FAKE_DB_ENGINE = "oracle";
        private const string FAKE_DB_VERSION = "9";
        private const string FAKE_CONNECTION_STRING = "DRIVER={oracle};DATABASE=database;SERVER=server;UID=username;PWD=password;";
        private const string FAKE_SQL_QUERY = "select id, name from employees";
        
        private sql_item FakeSqlItem;
        private sql_item FakeCollectedSqlItem;

        private sql57_item FakeSql57Item;
        private sql57_item FakeCollectedSql57Item;

        public SqlProberTests()
        {
            this.FakeSqlItem = sql_item.CreateSqlItem(FAKE_DB_ENGINE, FAKE_DB_VERSION, FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            this.FakeCollectedSqlItem = sql_item.CreateSqlItem(FAKE_DB_ENGINE, FAKE_DB_VERSION, FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            this.FakeCollectedSqlItem.result = new EntityItemAnySimpleType[] { new EntityItemAnySimpleType() { Value = "10" }, new EntityItemAnySimpleType() { Value = "11" } };

            this.FakeSql57Item = sql57_item.CreateSqlItem(FAKE_DB_ENGINE, FAKE_DB_VERSION, FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            this.FakeCollectedSql57Item = sql57_item.CreateSqlItem(FAKE_DB_ENGINE, FAKE_DB_VERSION, FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            this.FakeCollectedSql57Item.result =
                new EntityItemRecordType[] 
                { 
                    new EntityItemRecordType() 
                    { 
                        field  = new EntityItemFieldType[] 
                        { 
                            new EntityItemFieldType() { name = "id", Value = "10" }, 
                            new EntityItemFieldType() { name = "name", Value = "John" } 
                        }
                    }
                };
        }

        [TestMethod, Owner("lfernades")]
        public void Should_be_possible_to_collect_sql_object()
        {
            var sqlProber = new SqlProber();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    sqlProber,
                    new ItemType[] { FakeSqlItem },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(FakeCollectedSqlItem) });

            var probeResult = sqlProber.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:5000"));

            DoAssertForSingleCollectedObject(probeResult, typeof(sql_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned_for_sql_objects()
        {
            var sqlProber = new SqlProber();
            var fakeCollectedInfo = GetFakeCollectInfo("oval:modulo:obj:5000");
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(sqlProber);

            var proberExecutionResult = sqlProber.Execute(FakeContext, FakeTargetInfo, fakeCollectedInfo);

            DoAssertForExecutionWithErrors(proberExecutionResult, typeof(sql_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_sql57_object()
        {
            var sqlProber = new Sql57Prober();
            ProberBehaviorCreator
                .CreateBehaviorForNormalFlowExecution(
                    sqlProber,
                    new ItemType[] { FakeSql57Item },
                    new CollectedItem[] { ProbeHelper.CreateFakeCollectedItem(FakeCollectedSql57Item) });

            var probeResult = sqlProber.Execute(FakeContext, FakeTargetInfo, GetFakeCollectInfo("oval:modulo:obj:5100"));

            DoAssertForSingleCollectedObject(probeResult, typeof(sql57_item));
        }

        [TestMethod, Owner("lfernandes")]
        public void If_any_occurs_while_item_type_creation_an_item_with_error_status_must_be_returned_for_sql57_objects()
        {
            var sqlProber = new Sql57Prober();
            var fakeCollectedInfo = GetFakeCollectInfo("oval:modulo:obj:5100");
            ProberBehaviorCreator.CreateBehaviorWithExceptionThrowing(sqlProber);

            var proberExecutionResult = sqlProber.Execute(FakeContext, FakeTargetInfo, fakeCollectedInfo);

            DoAssertForExecutionWithErrors(proberExecutionResult, typeof(sql57_item));
        }
    }
}
