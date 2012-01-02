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


namespace Modulo.Collect.Probe.Independent.Common.Sql
{
    public class DbEngine
    {
        private const string NOT_SUPPORT_ENGINE_MSG = "[{0}] - There is no support for this DB engine: '{1}'";

        private string EngineAsString;

        public DbEngines Engine { get; private set; }

        public DbEngine(string engineDescription)
        {
            this.EngineAsString = engineDescription;
            this.Engine = this.GetDbEngineFromString();
        }

        private DbEngines GetDbEngineFromString()
        {
            if (string.IsNullOrWhiteSpace(EngineAsString))
                return DbEngines.Empty;

            if (EngineAsString.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
                return DbEngines.MsSqlServer;

            if (EngineAsString.Equals("oracle", StringComparison.InvariantCultureIgnoreCase))
                return DbEngines.Oracle;

            if (EngineAsString.Equals("mysql", StringComparison.InvariantCultureIgnoreCase))
                return DbEngines.MySQL;

            if (EngineAsString.Equals("postgre", StringComparison.InvariantCultureIgnoreCase))
                return DbEngines.Postgre;

            if (EngineAsString.Equals("db2", StringComparison.InvariantCultureIgnoreCase))
                return DbEngines.Db2;

            var errorMessage = String.Format(NOT_SUPPORT_ENGINE_MSG, GetType().Name, EngineAsString);
            throw new NotSupportedException(errorMessage);
        }
    }
}
