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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Independent.Sql;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Independent.Common.Sql;

namespace Modulo.Collect.Probe.Independent.Probes.SQL.Tests
{
    [TestClass]
    public class SqlQueryResultTests
    {
        private SqlQueryResult FakeSqlQueryResult;

        public SqlQueryResultTests()
        {
            var fakeColumnNames = new string[] { "name", "id", "enabled" };
            var fakeRecord1 = CreateFakeRecord(fakeColumnNames, new object[] { "John Doe", 10, true });
            var fakeRecord2 = CreateFakeRecord(fakeColumnNames, new object[] { "Barack Obama", 11, false });

            this.FakeSqlQueryResult = new SqlQueryResult();
            this.FakeSqlQueryResult.AddRecord(fakeRecord1);
            this.FakeSqlQueryResult.AddRecord(fakeRecord2);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_represent_the_result_of_a_SQL_query()
        {
            Assert.IsNotNull(FakeSqlQueryResult.Records);
            Assert.AreEqual(2, FakeSqlQueryResult.Records.Count);

            var firstRecord = FakeSqlQueryResult.Records[0];
            AssertRecordItem(firstRecord, 10, "John Doe", true);
            
            var secondRecord = FakeSqlQueryResult.Records[1];
            AssertRecordItem(secondRecord, 11, "Barack Obama", false);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_transform_a_sqlQueryResult_into_a_list_of_ovalSimpleAnyType()
        {
            var ovalSimpleTypes = FakeSqlQueryResult.ToOvalSimpleTypeList();

            Assert.IsNotNull(ovalSimpleTypes);
            Assert.AreEqual(2, ovalSimpleTypes.Count());
            Assert.AreEqual("John Doe", ovalSimpleTypes.ElementAt(0).Value);
            Assert.AreEqual("Barack Obama", ovalSimpleTypes.ElementAt(1).Value);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_transform_a_sqlQueryResult_with_no_result_into_a_list_of_ovalSimpleAnyType()
        {
            var emptySqlQueryResult = new SqlQueryResult();

            var ovalSimpleTypes = emptySqlQueryResult.ToOvalSimpleTypeList();
            
            Assert.IsNotNull(ovalSimpleTypes);
            Assert.AreEqual(1, ovalSimpleTypes.Count());
            Assert.AreEqual(StatusEnumeration.doesnotexist, ovalSimpleTypes.ElementAt(0).status);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_transform_a_sqlQueryResult_with_empty_field_into_a_list_of_ovalSimpleAnyType()
        {
            var fakeSqlQueryResult = new SqlQueryResult();
            fakeSqlQueryResult.AddRecord(CreateFakeRecord(new string[] { "name" }, new object[] { "John" }));
            fakeSqlQueryResult.AddRecord(CreateFakeRecord(new string[] { "name" }, new object[] { null }));
            fakeSqlQueryResult.AddRecord(CreateFakeRecord(new string[] { "name" }, new object[] { "Mark" }));

            var ovalSimpleTypes = fakeSqlQueryResult.ToOvalSimpleTypeList();

            Assert.IsNotNull(ovalSimpleTypes);
            Assert.AreEqual(3, ovalSimpleTypes.Count());
            Assert.AreEqual("John", ovalSimpleTypes.ElementAt(0).Value);
            Assert.AreEqual("Mark", ovalSimpleTypes.ElementAt(2).Value);
            var nullField = ovalSimpleTypes.ElementAt(1);
            Assert.IsNull(nullField.Value);
            Assert.AreEqual(StatusEnumeration.doesnotexist, nullField.status);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_transform_a_sqlQueryResult_into_a_list_of_ovalRecordType()
        {
            var ovalRecords = FakeSqlQueryResult.ToOvalRecordTypeList();

            Assert.IsNotNull(ovalRecords);
            Assert.AreEqual(2, ovalRecords.Count());

            AssertRecordFieldNames(
                ovalRecords.ElementAt(0).field,
                new string[] { "John Doe", "10", "True" });

            AssertRecordFieldNames(
                ovalRecords.ElementAt(1).field,
                new string[] { "Barack Obama", "11", "False" });
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_transform_a_sqlQueryResult_with_no_result_into_a_list_of_ovalRecordType()
        {
            var emptySqlQueryResult = new SqlQueryResult();

            var ovalRecordTypes = emptySqlQueryResult.ToOvalRecordTypeList();

            Assert.IsNotNull(ovalRecordTypes);
            Assert.AreEqual(1, ovalRecordTypes.Count());
            Assert.AreEqual(StatusEnumeration.doesnotexist, ovalRecordTypes.ElementAt(0).status);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_transform_a_sqlQueryResult_with_empty_field_into_a_list_of_ovalRecordType()
        {
            var fakeSqlQueryResult = new SqlQueryResult();
            fakeSqlQueryResult.AddRecord(CreateFakeRecord(new string[] { "id", "name" }, new object[] { null, null }));
            fakeSqlQueryResult.AddRecord(CreateFakeRecord(new string[] { "id", "name" }, new object[] { 1, null }));
            fakeSqlQueryResult.AddRecord(CreateFakeRecord(new string[] { "id", "name" }, new object[] { null, "John" }));
            fakeSqlQueryResult.AddRecord(CreateFakeRecord(new string[] { "id", "name" }, new object[] { 2, "Michael" }));
            
            var ovalRecords = fakeSqlQueryResult.ToOvalRecordTypeList();

            Assert.IsNotNull(ovalRecords);
            Assert.AreEqual(4, ovalRecords.Count());

            var firstRecord = ovalRecords.ElementAt(0);
            AssertField(firstRecord.field.First(), "id", null);
            AssertField(firstRecord.field.Last(), "name", null);
            
            var secondRecord = ovalRecords.ElementAt(1);
            AssertField(secondRecord.field.First(), "id", "1");
            AssertField(secondRecord.field.Last(), "name", null);

            var thirdRecord = ovalRecords.ElementAt(2);
            AssertField(thirdRecord.field.First(), "id", null);
            AssertField(thirdRecord.field.Last(), "name", "John");
            
            var fourthRecord = ovalRecords.ElementAt(3);
            AssertField(fourthRecord.field.First(), "id", "2");
            AssertField(fourthRecord.field.Last(), "name", "Michael");
        }


        private void AssertRecordFieldNames(EntityItemFieldType[] recordFieldsToAssert, string[] expectedValues)
        {
            Assert.AreEqual(3, recordFieldsToAssert.Count());
            AssertField(recordFieldsToAssert.ElementAt(0), "name", expectedValues[0]);
            AssertField(recordFieldsToAssert.ElementAt(1), "id", expectedValues[1]);
            AssertField(recordFieldsToAssert.ElementAt(2), "enabled", expectedValues[2]);
        }

        private void AssertField(EntityItemFieldType fieldToAssert, string expectedFieldName, string expectedFieldValue)
        {
            Assert.AreEqual(expectedFieldName, fieldToAssert.name);
            
            if (expectedFieldValue == null)
            {
                Assert.IsNull(fieldToAssert.Value);
                Assert.AreEqual(StatusEnumeration.doesnotexist, fieldToAssert.status);
            }
            else
            {
                Assert.AreEqual(expectedFieldValue, fieldToAssert.Value);
            }
        }

        private Dictionary<string, object> CreateFakeRecord(string[] columns, object[] values)
        {
            var newRecord = new Dictionary<string, object>();
            for(int i = 0; i < columns.Length; i++)
                newRecord.Add(columns[i], values[i]);
            
            return newRecord;
        }

        private void AssertRecordItem(
            Dictionary<string, object> recordToAssert, 
            int expectedId, 
            string expectedName, 
            bool expectedEnabled)
        {
            Assert.AreEqual(3, recordToAssert.Count);
            Assert.AreEqual(expectedId, recordToAssert["id"]);
            Assert.AreEqual(expectedName, recordToAssert["name"]);
            Assert.AreEqual(expectedEnabled, recordToAssert["enabled"]);
        }
    }
}
