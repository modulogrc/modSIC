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
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.Probe.Independent.Tests.Probes.SQL
{
    [TestClass]
    public class SqlObjectTests
    {
        private const string FAKE_CONNECTION_STRING = 
            "DRIVER={Oracle};DATABASE=database;SERVER=server;UID=username;PWD=password;";

        private const string FAKE_SQL = "select id, name from employees";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_sql_object_entities()
        {
            var fakeSqlObject = (sql_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5000");

            var engineEntity = fakeSqlObject.GetObjectEntityByName(sql_object_choices.engine);
            var versionEntity = fakeSqlObject.GetObjectEntityByName(sql_object_choices.version);
            var connectionStringEntity = fakeSqlObject.GetObjectEntityByName(sql_object_choices.connection_string);
            var sqlEntity = fakeSqlObject.GetObjectEntityByName(sql_object_choices.sql);
            
            AssertObjectEntity(engineEntity, "oracle");
            AssertObjectEntity(versionEntity, "9");
            AssertObjectEntity(connectionStringEntity, FAKE_CONNECTION_STRING);
            AssertObjectEntity(sqlEntity, FAKE_SQL);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_sql57_object_entities()
        {
            var fakeSqlObject = (sql57_object)ProbeHelper.GetDefinitionObjectTypeByID("definitionsSimple", "5100");

            var engineEntity = fakeSqlObject.GetObjectEntityByName(sql57_object_choices.engine);
            var versionEntity = fakeSqlObject.GetObjectEntityByName(sql57_object_choices.version);
            var connectionStringEntity = fakeSqlObject.GetObjectEntityByName(sql57_object_choices.connection_string);
            var sqlEntity = fakeSqlObject.GetObjectEntityByName(sql57_object_choices.sql);

            AssertObjectEntity(engineEntity, "oracle");
            AssertObjectEntity(versionEntity, "9");
            AssertObjectEntity(connectionStringEntity, FAKE_CONNECTION_STRING);
            AssertObjectEntity(sqlEntity, FAKE_SQL);

        }

        private void AssertObjectEntity(EntitySimpleBaseType entityToAssert, string expectedEntiyValue)
        {
            Assert.IsNotNull(entityToAssert);
            Assert.IsNotNull(entityToAssert.Value);
            Assert.AreEqual(expectedEntiyValue, entityToAssert.Value);
        }

    }
}
