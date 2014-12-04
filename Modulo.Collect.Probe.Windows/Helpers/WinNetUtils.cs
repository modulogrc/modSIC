/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
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
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Modulo.Collect.Probe.Windows.FileContent.Helpers;

namespace Modulo.Collect.Probe.Windows.Helpers
{
    public class WinNetConnectionException : Exception
    {
        public WinNetConnectionException(string exceptionMessage) : base(exceptionMessage) { }
    }



    public class WinNetUtils
    {
        #region Consts
        const int RESOURCE_CONNECTED = 0x00000001;
        const int RESOURCE_GLOBALNET = 0x00000002;
        const int RESOURCE_REMEMBERED = 0x00000003;

        const int RESOURCETYPE_ANY = 0x00000000;
        const int RESOURCETYPE_DISK = 0x00000001;
        const int RESOURCETYPE_PRINT = 0x00000002;

        const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
        const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
        const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;

        const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        const int RESOURCEUSAGE_CONTAINER = 0x00000002;

        const int CONNECT_INTERACTIVE = 0x00000008;
        const int CONNECT_PROMPT = 0x00000010;
        const int CONNECT_REDIRECT = 0x00000080;
        const int CONNECT_UPDATE_PROFILE = 0x00000001;
        const int CONNECT_COMMANDLINE = 0x00000800;
        const int CONNECT_CMD_SAVECRED = 0x00001000;
        const int CONNECT_LOCALDRIVE = 0x00000100;
        #endregion

        #region Imports
        [DllImport("Mpr.dll")]
        private static extern int WNetUseConnection(
            IntPtr hwndOwner,
            NETRESOURCE lpNetResource,
            string lpPassword,
            string lpUserID,
            int dwFlags,
            string lpAccessName,
            string lpBufferSize,
            string lpResult
            );

        [DllImport("Mpr.dll")]
        private static extern int WNetCancelConnection2(
            string lpName,
            int dwFlags,
            bool fForce
            );

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public int dwScope = 0;
            public int dwType = 0;
            public int dwDisplayType = 0;
            public int dwUsage = 0;
            public string lpLocalName = "";
            public string lpRemoteName = "";
            public string lpComment = "";
            public string lpProvider = "";
        }
        #endregion

        public static void connectToRemote(string remoteUNC, string username, string password)
        {
            connectToRemote(remoteUNC, username, password, false);
        }

        public static void connectToRemote(string remoteUNC, string username, string password, bool promptUser)
        {
            NETRESOURCE nr = new NETRESOURCE();
            nr.dwType = RESOURCETYPE_DISK;
            nr.lpRemoteName = remoteUNC;

            int ret;
            if (promptUser)
                ret = WNetUseConnection(IntPtr.Zero, nr, "", "", CONNECT_INTERACTIVE | CONNECT_PROMPT, null, null, null);
            else
                ret = WNetUseConnection(IntPtr.Zero, nr, password, username, 0, null, null, null);

            var successfullyConnected = (ret == 0);
            if (!successfullyConnected)
            {
                // 1219 is the code for "Multiple connections to a server or shared resource by the same user, using more than one user name, are not allowed.
                var alreadyConnected = (ret == 1219);
                if (!alreadyConnected)
                {
                    var win32Exception = new System.ComponentModel.Win32Exception(ret);
                    throw new WinNetConnectionException(win32Exception.Message);
                }
            }
        }

        public static void disconnectRemote(string remoteUNC)
        {
            try
            {
                // The API call below returns the code error. If the code is 0 (zero) there is no error.
                // If any error occurrs, you can throw a System.ComponentModel.Win32Exception passing the error code.
                WNetCancelConnection2(remoteUNC, CONNECT_UPDATE_PROFILE, false);
            }
            catch (Exception)
            {
            }
        }

        public static List<FileContentItemSystemData> parseMatches(string dir, string fname, string line, string pattern, ref int instance)
        {
            List<FileContentItemSystemData> retList = new List<FileContentItemSystemData>();
            Regex myRegex = new Regex(pattern);
            MatchCollection myMatches = myRegex.Matches(line);
            foreach (Match myMatch in myMatches)
            {
                FileContentItemSystemData retItem = new FileContentItemSystemData();
                retItem.SubExpressions = new List<string>();
                retItem.Line = line;
                retItem.Pattern = pattern;
                retItem.Text = myMatch.ToString();
                retItem.Path = dir;
                retItem.FileName = fname;
                for (int i = 1; i < myMatch.Groups.Count; i++)
                    retItem.SubExpressions.Add(myMatch.Groups[i].ToString());

                retList.Add(retItem);
            }
            return retList;
        }

        public static List<FileContentItemSystemData> getWinTextFileContent(string remoteUNC, string pathspec, string pattern)
        {
            List<FileContentItemSystemData> retList = new List<FileContentItemSystemData>();

            if ((pathspec[1] != ':') || (pathspec[2] != '\\'))
                throw new ArgumentException("Pathspec must be of the form 'X:\\path\\filename[.ext]'", pathspec);

            char drive = pathspec[0];
            string restofpath = pathspec.Substring(2);
            int whereFile = pathspec.LastIndexOf('\\');
            string dirName, fName;
            if (whereFile > 0)
            {
                dirName = pathspec.Substring(0, whereFile);
                fName = pathspec.Substring(whereFile + 1);
            }
            else
            {
                dirName = "\\";
                fName = pathspec.Substring(1);
            }

            TextReader tr = new StreamReader(remoteUNC + "\\" + drive + "$" + restofpath);
            try
            {
                int instance = 0;

                string linha = tr.ReadLine();
                while (linha != null)
                {
                    retList.AddRange(parseMatches(dirName, fName, linha, pattern, ref instance));
                    linha = tr.ReadLine();
                }
            }
            finally
            {
                tr.Close();
            }

            return retList;
        }
    }
}
