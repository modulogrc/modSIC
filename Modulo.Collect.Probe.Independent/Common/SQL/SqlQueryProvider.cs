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
using System.Data.Odbc;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.OracleClient;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data.OleDb;


namespace Modulo.Collect.Probe.Independent.Common.Sql
{
    public enum DbEngines { MsSqlServer, Oracle, Empty, MySQL, Postgre, Db2 }

    public class SqlQueryProvider
    {
        private IDbConnection DbConnection;
        private IDbCommand DbCommand;
        private IDbDataAdapter DbDataAdapter;

        public virtual SqlQueryResult ExecuteSQL(DbEngines dbEngine, string connectionString, string sqlQuery)
        {
            ConfigureDependencies(dbEngine);
            this.DbConnection.ConnectionString = connectionString;
            this.DbConnection.Open();

            var dataReader = GetDataReader(this.DbConnection, sqlQuery);
            var sqlResult = new SqlQueryResult();
            try
            {
                while (dataReader.Read())
                {
                    var currentRecord = new Dictionary<string, object>();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        var columnName = dataReader.GetName(i);
                        var columnValue = dataReader[columnName];
                        currentRecord.Add(columnName, columnValue);
                    }
                    sqlResult.AddRecord(currentRecord);
                }

                return sqlResult;
            }
            finally
            {
                dataReader.Close();
                this.DbConnection.Close();
            }
        }

        private void ConfigureDependencies(DbEngines dbEngine)
        {
            switch (dbEngine)
            {
                case DbEngines.MsSqlServer:
                    this.DbConnection = new SqlConnection();
                    this.DbCommand = new SqlCommand();
                    this.DbDataAdapter = new SqlDataAdapter();
                    break;
                case DbEngines.Oracle:
                    this.DbConnection = new OracleConnection();
                    this.DbCommand = new OracleCommand();
                    this.DbDataAdapter = new OracleDataAdapter();
                    break;
                case DbEngines.MySQL:
                    this.DbConnection = new MySqlConnection();
                    this.DbCommand = new MySqlCommand();
                    this.DbDataAdapter = new MySqlDataAdapter();
                    break;
                case DbEngines.Postgre:
                    this.DbConnection = new NpgsqlConnection();
                    this.DbCommand = new NpgsqlCommand();
                    this.DbDataAdapter = new NpgsqlDataAdapter();
                    break;
                case DbEngines.Db2:
                    this.DbConnection = new OleDbConnection();
                    this.DbCommand = new OleDbCommand();
                    this.DbDataAdapter = new OleDbDataAdapter();
                    break;
                case DbEngines.Empty:
                    this.DbConnection = new OdbcConnection();
                    this.DbCommand = new OdbcCommand();
                    this.DbDataAdapter = new OdbcDataAdapter();
                    break;
                default:
                    throw new ArgumentException(String.Format("[SqlQueryProvider] - This engine ('{0}') is not supportted.", dbEngine.ToString()));
            }
        }

        private IDataReader GetDataReader(IDbConnection connection, string sqlQuery)
        {
            this.DbCommand.Connection = connection;
            this.DbCommand.CommandText = sqlQuery;
            this.DbDataAdapter.SelectCommand = this.DbCommand;
            
            var dataReader = this.DbCommand.ExecuteReader();
            if (((DbDataReader)dataReader).HasRows)
                return dataReader;
            else
                throw new SqlNoResultException();
        }
    }

    public class SqlNoResultException : Exception { }
}
