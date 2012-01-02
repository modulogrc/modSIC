/*
 * This software is licensed under the terms of Blah.
 * Use of this software is conditional to saying "Blah".
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace FrameworkNG
{
    public class SQLCollector
    {
        public static List<object[]> sqlDoSelect(string driverName, string hostName, string dbName, string userName, string passWord, string sqlQuery, out string[] fldNames)
        {
            string connStr = String.Format("DRIVER={{{0}}};SERVER={1};DATABASE={2};UID={3};PWD={4}",
                driverName, hostName, dbName, userName, passWord);
            return sqlDoSelect(connStr, sqlQuery, out fldNames);
        }

        public static List<object[]> sqlDoSelect(string connStr, string sqlQuery, out string[] fldNames)
        {
            List<object[]> retList = new List<object[]>();
            OdbcConnection myConn = new OdbcConnection();
            myConn.ConnectionString = connStr;
            myConn.Open();

            OdbcCommand myCmd = new OdbcCommand();
            myCmd.Connection = myConn;
            myCmd.CommandText = sqlQuery;

            OdbcDataAdapter myAdapter = new OdbcDataAdapter();
            myAdapter.SelectCommand = myCmd;

            OdbcDataReader myReader = myCmd.ExecuteReader();
            while (myReader.Read())
            {
                object[] thisRow = new object[myReader.FieldCount];
                myReader.GetValues(thisRow);
                retList.Add(thisRow);
            }

            fldNames = new string[myReader.FieldCount];
            for (int i = 0; i < myReader.FieldCount; i++)
            {
                fldNames[i] = myReader.GetName(i);
            }

            myReader.Close();
            myConn.Close();
            return retList;
        }
    }
}
