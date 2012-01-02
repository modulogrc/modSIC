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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Independent.Sql;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Rhino.Mocks;
using Modulo.Collect.Probe.Independent.Sql57;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common.Sql;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Probe.Independent.Tests.Probes.SQL
{
    [TestClass]
    public class SqlObjectCollectorTests
    {
        private const string FAKE_CONNECTION_STRING = "DRIVER={PostgreSQL};DATABASE=database;SERVER=server;UID=username;PWD=password;";
        private const string FAKE_SQL_QUERY = "select id from employees";


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_sql_item()
        {
            var fakeSqlItem = sql_item.CreateSqlItem(string.Empty, string.Empty, FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            var sqlObjectCollector = CreateSqlObjectCollectorWithBehavior(CreateFakeQueryResult(), false);

            var collectedItems = sqlObjectCollector.CollectDataForSystemItem(fakeSqlItem);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(sql_item), true);
            var collectedSqlItem = (sql_item)collectedItems.First().ItemType;

            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSqlItem.connection_string, FAKE_CONNECTION_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSqlItem.sql, FAKE_SQL_QUERY);
            
            var queryResult = collectedSqlItem.result;
            Assert.AreEqual(2, queryResult.Count());
            Assert.AreEqual("John", queryResult.First().Value);
            Assert.AreEqual("David", queryResult.Last().Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_a_sql57_item()
        {
            var fakeSqlItem = sql57_item.CreateSqlItem(string.Empty, string.Empty, FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            var sqlObjectCollector = CreateSqlObjectCollectorWithBehavior(CreateFakeQueryResult(), true);

            var collectedItems = sqlObjectCollector.CollectDataForSystemItem(fakeSqlItem);

            ItemTypeChecker.DoBasicAssertForCollectedItems(collectedItems, 1, typeof(sql57_item), true);
            var collectedSqlItem = (sql57_item)collectedItems.First().ItemType;

            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSqlItem.connection_string, FAKE_CONNECTION_STRING);
            ItemTypeEntityChecker.AssertItemTypeEntity(collectedSqlItem.sql, FAKE_SQL_QUERY);

            var queryResult = collectedSqlItem.result;
            Assert.AreEqual(2, queryResult.Count());
            AssertRecordField(queryResult.First().field, new string[] { "John", "1", "True" });
            AssertRecordField(queryResult.Last().field, new string[] { "David", "2", "False" });
            

        }


        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_error_during_item_collection()
        {
            var fakeSqlItem = sql_item.CreateSqlItem(string.Empty, string.Empty, FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            var sqlObjectCollector = CreateSqlObjectCollectorWithError();

            var collectedItems = sqlObjectCollector.CollectDataForSystemItem(fakeSqlItem);

            Assert.AreEqual(1, collectedItems.Count());
            var collectedSqlItem = collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.error, collectedSqlItem.status, "Unexpected item status.");
            Assert.IsNotNull(collectedSqlItem.message);
            Assert.AreEqual(1, collectedSqlItem.message.Count());
            Assert.AreEqual(MessageLevelEnumeration.error, collectedSqlItem.message.First().level);
            var messageString = collectedSqlItem.message.First().Value;
            Assert.IsFalse(string.IsNullOrWhiteSpace(messageString));
            Assert.AreEqual("Test Exception", messageString);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_handle_empty_sql_result()
        {
            var fakeSqlItem = sql_item.CreateSqlItem(string.Empty, string.Empty, FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            var sqlObjectCollector = CreateSqlObjectCollectorWithError(true);

            var collectedItems = sqlObjectCollector.CollectDataForSystemItem(fakeSqlItem);

            Assert.AreEqual(1, collectedItems.Count());
            var collectedSqlItem = collectedItems.Single().ItemType;
            Assert.AreEqual(StatusEnumeration.doesnotexist, collectedSqlItem.status, "Unexpected item status.");
        }

        [TestMethod]
        public void a()
        {
           // System.Data.

        }

        private SqlQueryResult CreateFakeQueryResult()
        {
            var fakeColumnNames = new string[] { "name", "id", "enabled" };
            
            var fakeQueryResult = new SqlQueryResult();
            fakeQueryResult.AddRecord(CreateFakeRecord(fakeColumnNames, new object[] { "John", 1, true}));
            fakeQueryResult.AddRecord(CreateFakeRecord(fakeColumnNames, new object[] { "David", 2, false }));
            return fakeQueryResult;
        }

        private Dictionary<string, object> CreateFakeRecord(string[] columnNames, object[] columnValues)
        {
            var fakeRecord = new Dictionary<string, object>();
            for(int i = 0; i < columnNames.Length; i++)
                fakeRecord.Add(columnNames[i], columnValues[i]);
            
            return fakeRecord;
        }

        private BaseObjectCollector CreateSqlObjectCollectorWithBehavior(
            SqlQueryResult fakeSqlQueryResultToReturn, bool forSql57)
        {
            var mocks = new MockRepository();
            var fakeSqlQueryProvider = mocks.DynamicMock<SqlQueryProvider>();
            Expect.Call(
                fakeSqlQueryProvider
                    .ExecuteSQL(DbEngines.MsSqlServer, null, null))
                        .IgnoreArguments()
                        .Return(fakeSqlQueryResultToReturn);
            
            mocks.ReplayAll();

            if (forSql57)
                return new Sql57ObjectCollector(fakeSqlQueryProvider);
            
            return new SqlObjectCollector(fakeSqlQueryProvider);
        }

        private BaseObjectCollector CreateSqlObjectCollectorWithError(bool forNoQueryResult = false)
        {
            var mocks = new MockRepository();
            var fakeSqlQueryProvider = mocks.DynamicMock<SqlQueryProvider>();
            var exception = forNoQueryResult ? new SqlNoResultException() : new Exception("Test Exception"); 
            Expect.Call(
                fakeSqlQueryProvider
                    .ExecuteSQL(DbEngines.MsSqlServer, null, null))
                        .IgnoreArguments()
                        .Throw(exception);

            mocks.ReplayAll();

            return new SqlObjectCollector(fakeSqlQueryProvider);

        }

        private void AssertRecordField(EntityItemFieldType[] entityFieldsToAssert, string[] expectedFieldValues)
        {
            Assert.AreEqual(3, entityFieldsToAssert.Count());
            AssertFieldType(entityFieldsToAssert.ElementAt(0), "name", expectedFieldValues.ElementAt(0));
            AssertFieldType(entityFieldsToAssert.ElementAt(1), "id", expectedFieldValues.ElementAt(1));
            AssertFieldType(entityFieldsToAssert.ElementAt(2), "enabled", expectedFieldValues.ElementAt(2));
        }

        private void AssertFieldType(
            EntityItemFieldType fieldToAssert, 
            string expectedFieldName, 
            string expectedFieldValue)
        {
            Assert.AreEqual(expectedFieldName, fieldToAssert.name);
            Assert.AreEqual(expectedFieldValue, fieldToAssert.Value);

        }

    }
}
