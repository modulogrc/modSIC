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
using Modulo.Collect.Probe.Independent.Common.Sql;

namespace Modulo.Collect.Probe.Independent.Tests.Probes.SQL
{
    [TestClass]
    public class DbEngineTests
    {
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_oracle_engine()
        {
            Assert.AreEqual(DbEngines.Oracle, new DbEngine("oracle").Engine);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_MsSqlServer_engine()
        {
            Assert.AreEqual(DbEngines.MsSqlServer, new DbEngine("sqlserver").Engine);
        }

        [TestMethod, Owner("dgomes")]
        public void Should_be_possible_to_get_mysql_engine()
        {
            Assert.AreEqual(DbEngines.MySQL, new DbEngine("mysql").Engine);
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_get_generic_db_engine()
        {
            Assert.AreEqual(DbEngines.Empty, new DbEngine(string.Empty).Engine);
        }

        [TestMethod, Owner("lfernandes")]
        [ExpectedException(typeof(NotSupportedException))]
        public void Should_be_possible_to_throw_a_NotSupportedException_when_unsupported_engine_was_given()
        {
            new DbEngine("postgres");
        }


    }
}
