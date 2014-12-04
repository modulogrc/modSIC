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
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.Probe.Common.Test.Checkers;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Independent.Sql57;

namespace Modulo.Collect.Probe.Independent.Tests.Probes.SQL
{
    [TestClass]
    public class SqlItemTypeGeneratorTests
    {
        private const string FAKE_CONNECTION_STRING = "DRIVER={Oracle};DATABASE=database;SERVER=server;UID=username;PWD=password;";
        private const string FAKE_CONNECTION_STRING2 = "DRIVER={SQLOLEDB};DATABASE=database;SERVER=server;UID=username;PWD=password;";
        private const string FAKE_SQL_QUERY = "select id, name from employees";
        private const string FAKE_SQL_QUERY2 = "select column from table";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object()
        {
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5000");
            
            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, VariableHelper.CreateEmptyEvaluatedVariables());

            Assert.IsNotNull(itemsToCollect);
            ItemTypeChecker.DoBasicAssertForItems(itemsToCollect.ToArray(), 1, typeof(sql_item));
            AssertSqlItem((sql_item)itemsToCollect.Single());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_on_engine_entity()
        {
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("5010", "5010", "oracle");
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5010");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            AssertSqlItem((sql_item)itemsToCollect.Single());
        }
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_with_multiple_values_on_engine_entity()
        {
            var fakeVariables = VariableHelper.CreateVariableWithMultiplesValue("5010", "5010", new string[] { "oracle", "sqlserver" });
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5010");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            Assert.AreEqual(2, itemsToCollect.Count());
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(0), "oracle");
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(1), "sqlserver");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_on_version_entity()
        {
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("5020", "5020", "9");
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5020");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            AssertSqlItem((sql_item)itemsToCollect.Single());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_with_multiple_values_on_version_entity()
        {
            var fakeVariables = VariableHelper.CreateVariableWithMultiplesValue("5020", "5020", new string[] { "9", "10" });
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5020");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            Assert.AreEqual(2, itemsToCollect.Count());
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(0), "oracle", "9");
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(1), "oracle", "10");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_on_connectionString_entity()
        {
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("5030", "5030", FAKE_CONNECTION_STRING);
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5030");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            AssertSqlItem((sql_item)itemsToCollect.Single());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_with_multiple_values_on_connectionString_entity()
        {
            var fakeConnectionStrings = new string[] { FAKE_CONNECTION_STRING, FAKE_CONNECTION_STRING2 };
            var fakeVariables = VariableHelper.CreateVariableWithMultiplesValue("5030", "5030", fakeConnectionStrings);
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5030");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            Assert.AreEqual(2, itemsToCollect.Count());
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(0), "oracle", "9", FAKE_CONNECTION_STRING);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(1), "oracle", "9", FAKE_CONNECTION_STRING2);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_on_sql_entity()
        {
            var fakeVariables = VariableHelper.CreateVariableWithOneValue("5040", "5040", FAKE_SQL_QUERY);
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5040");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            AssertSqlItem((sql_item)itemsToCollect.Single());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_with_multiple_values_on_sql_entity()
        {
            var fakeConnectionStrings = new string[] { FAKE_SQL_QUERY, FAKE_SQL_QUERY2 };
            var fakeVariables = VariableHelper.CreateVariableWithMultiplesValue("5040", "5040", fakeConnectionStrings);
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5040");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            Assert.AreEqual(2, itemsToCollect.Count());
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(0), "oracle", "9", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(1), "oracle", "9", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY2);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_in_all_entities()
        {
            var fakeVariableValues = CreateFakeVariableValues();
            var fakeVariables = VariableHelper.CreateEvaluatedVariables("oval:modulo:obj:5050", fakeVariableValues);
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5050");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            Assert.AreEqual(1, itemsToCollect.Count());
            AssertSqlItem((sql_item)itemsToCollect.Single(), "postgres", "8", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY2);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_generate_items_to_collect_from_a_sql_object_with_referenced_variable_with_multiple_values_in_all_entities()
        {
            var fakeVariableValues = CreateFakeMultipleVariableValues();
            var fakeVariables = VariableHelper.CreateEvaluatedVariables("oval:modulo:obj:5050", fakeVariableValues);
            var fakeObject = ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5050");

            var itemsToCollect = new SqlItemTypeGenerator().GetItemsToCollect(fakeObject, fakeVariables);

            Assert.AreEqual(16, itemsToCollect.Count());
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(0), "postgres", "8", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(1), "postgres", "8", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY2);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(2), "postgres", "8", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(3), "postgres", "8", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY2);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(4), "postgres", "2", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(5), "postgres", "2", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY2);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(6), "postgres", "2", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(7), "postgres", "2", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY2);

            AssertSqlItem((sql_item)itemsToCollect.ElementAt(8), "db2", "8", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(9), "db2", "8", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY2);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(10), "db2", "8", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(11), "db2", "8", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY2);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(12), "db2", "2", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(13), "db2", "2", FAKE_CONNECTION_STRING, FAKE_SQL_QUERY2);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(14), "db2", "2", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY);
            AssertSqlItem((sql_item)itemsToCollect.ElementAt(15), "db2", "2", FAKE_CONNECTION_STRING2, FAKE_SQL_QUERY2);
        }


        private static Dictionary<string, IEnumerable<string>> CreateFakeVariableValues()
        {
            var fakeVariableValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariableValues.Add("oval:modulo:var:5010", new string[] { "postgres" });
            fakeVariableValues.Add("oval:modulo:var:5020", new string[] { "8" });
            fakeVariableValues.Add("oval:modulo:var:5030", new string[] { FAKE_CONNECTION_STRING2 });
            fakeVariableValues.Add("oval:modulo:var:5040", new string[] { FAKE_SQL_QUERY2 });

            return fakeVariableValues;
        }

        private static Dictionary<string, IEnumerable<string>> CreateFakeMultipleVariableValues()
        {
            var fakeVariableValues = new Dictionary<string, IEnumerable<string>>();
            fakeVariableValues.Add("oval:modulo:var:5010", new string[] { "postgres", "db2" });
            fakeVariableValues.Add("oval:modulo:var:5020", new string[] { "8", "2" });
            fakeVariableValues.Add("oval:modulo:var:5030", new string[] { FAKE_CONNECTION_STRING, FAKE_CONNECTION_STRING2 });
            fakeVariableValues.Add("oval:modulo:var:5040", new string[] { FAKE_SQL_QUERY, FAKE_SQL_QUERY2 });

            return fakeVariableValues;
        }

        private void AssertSqlItem(
            sql_item sqlItemToAssert,
            string expectedEngineValue = "oracle",
            string expectedVersionValue = "9",
            string expectedConnectionStringValue = FAKE_CONNECTION_STRING,
            string expectedSqlValue = FAKE_SQL_QUERY)
        {
            ItemTypeEntityChecker.AssertItemTypeEntity(sqlItemToAssert.engine, expectedEngineValue);
            ItemTypeEntityChecker.AssertItemTypeEntity(sqlItemToAssert.version, expectedVersionValue);
            ItemTypeEntityChecker.AssertItemTypeEntity(sqlItemToAssert.connection_string, expectedConnectionStringValue);
            ItemTypeEntityChecker.AssertItemTypeEntity(sqlItemToAssert.sql, expectedSqlValue);
        }
    }
}
