/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tamir.SharpSsh;

namespace Modulo.Collect.Probe.Unix.SSHCollectors
{
    public enum ShortMonths { jan, fev, mar, apr, mai, jun, jul, ago, set, @out, nov, dez }

    public abstract class GenericFileInfo
    {
        private Dictionary<string, string> ShortMonthsTable;
        public bool IsDirectory { get; set; }
        public bool Found { get; set; }
        public string ErrorMsg { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public virtual bool Hidden { get; set; }
        public virtual bool Writeable { get; set; }
        public string Name { get; set; }
        public string EightDotThreeFileName { get; set; }
        public string FileType { get; set; }
        public string Manufacturer { get; set; }
        public string Version { get; set; }

        public string FileName { get; set; }
        public string Path { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime InstallDate { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime LastModified { get; set; }

        public ulong FileSize { get; set; }

        public GenericFileInfo()
        {
            ShortMonthsTable = new Dictionary<string, string>();
            for (int i = 1; i <= 12; i++)
            {
                var monthAsNumeric = i < 10 ? String.Format("0{0}", i) : i.ToString();
                var monthAsShortString = Enum.GetNames(typeof(ShortMonths)).ElementAt(i-1);
                ShortMonthsTable.Add(monthAsShortString, monthAsNumeric);
            }

            Found = false;
        }

        public String GetMonthNumberByShortMonth(string shortMonth)
        {
            String monthAsNumeric = null;
            ShortMonthsTable.TryGetValue(shortMonth.ToLower(), out monthAsNumeric);
            if (String.IsNullOrWhiteSpace(monthAsNumeric))
                throw new Exception(string.Format("[File Collector]: Invalid short month: '{0}'", shortMonth));

            return monthAsNumeric;
        }

        public override string ToString()
        {
            string retVal = "";
            retVal += String.Format("Found: {0}\n", Found);
            retVal += String.Format("IsDirectory: {0}\n", IsDirectory);
            retVal += String.Format("Error Message: {0}\n", ErrorMsg);

            retVal += String.Format("Name: {0}\n", Name);
            retVal += String.Format("EightDotThreeFileName: {0}\n", EightDotThreeFileName);
            retVal += String.Format("FileType: {0}\n", FileType);
            retVal += String.Format("Manufacturer: {0}\n", Manufacturer);
            retVal += String.Format("Version: {0}\n", Version);
            retVal += String.Format("FileName: {0}\n", FileName);
            retVal += String.Format("Path: {0}\n", Path);
            retVal += String.Format("FileSize: {0}\n", FileSize);

            retVal += String.Format("Owner: {0}\n", Owner);
            retVal += String.Format("Group: {0}\n", Group);

            retVal += String.Format("Hidden: {0}\n", Hidden);
            retVal += String.Format("Writeable: {0}\n", Writeable);

            retVal += String.Format("CreationDate: {0}\n", CreationDate);
            retVal += String.Format("InstallDate: {0}\n", InstallDate);
            retVal += String.Format("LastAccessed: {0}\n", LastAccessed);
            retVal += String.Format("LastModified: {0}", LastModified);

            return retVal;
        }
    }

    public class FileInfo : GenericFileInfo
    {
        public uint Mode { get; set; }
        public bool IsSpecial { get; set; }
        public string LinksTo { get; set; }

        public bool IsSymLink
        {
            get
            {
                return (!String.IsNullOrEmpty(this.LinksTo));
            }
        }

        public string FullPath
        {
            get
            {
                if (this.Path.EndsWith("/"))
                    return this.Path + this.FileName;
                else
                    return this.Path + "/" + this.FileName;
            }
        }

        public override bool Hidden
        {
            get
            {
                return ((this.FileName != null) && (this.FileName[0] == '.'));
            }
        }

        public override bool Writeable
        {
            get
            {
                return ((this.Mode & 0x0092) != 0);
            }
        }

        public FileInfo()
            : base()
        {
            LinksTo = null;
        }

        public override string ToString()
        {
            string retVal = "";
            retVal += String.Format("Found: {0}\n", Found);
            retVal += String.Format("IsDirectory: {0}\n", IsDirectory);
            retVal += String.Format("IsSpecial: {0}\n", IsSpecial);
            retVal += String.Format("Error Message: {0}\n", ErrorMsg);

            retVal += String.Format("Name: {0}\n", Name);
            if (IsSymLink)
                retVal += String.Format("LinksTo: {0}\n", LinksTo);
            retVal += String.Format("EightDotThreeFileName: {0}\n", EightDotThreeFileName);
            retVal += String.Format("FileType: {0}\n", FileType);
            retVal += String.Format("Manufacturer: {0}\n", Manufacturer);
            retVal += String.Format("Version: {0}\n", Version);
            retVal += String.Format("FileName: {0}\n", FileName);
            retVal += String.Format("Path: {0}\n", Path);
            retVal += String.Format("FileSize: {0}\n", FileSize);

            retVal += String.Format("Owner: {0}\n", Owner);
            retVal += String.Format("Group: {0}\n", Group);
            retVal += String.Format("Mode: {0:X4}\n", Mode);

            retVal += String.Format("Hidden: {0}\n", Hidden);
            retVal += String.Format("Writeable: {0}\n", Writeable);

            retVal += String.Format("CreationDate: {0}\n", CreationDate);
            retVal += String.Format("InstallDate: {0}\n", InstallDate);
            retVal += String.Format("LastAccessed: {0}\n", LastAccessed);
            retVal += String.Format("LastModified: {0}", LastModified);

            return retVal;
        }
    }

    public class FileCollector
    {
        public LsCommand LsCommand { get; set; }
        //public SshExec SSHExec { get; private set; }

        //public FileCollector(SshExec sshexec)
        //{
        //    this.SSHExec = sshexec;
        //}

        private string DeDotPath(string origPath)
        {
            char[] pathseps = { '/' };
            List<string> pathComps = new List<string>(origPath.Split(pathseps, StringSplitOptions.None));

            int ndx = 0;
            while (ndx < pathComps.Count)
            {
                if (pathComps[ndx] == ".")
                {
                    pathComps.RemoveAt(ndx);
                }
                else if (pathComps[ndx] == "..")
                {
                    pathComps.RemoveAt(ndx);
                    if (ndx > 0)
                    {
                        pathComps.RemoveAt(ndx - 1);
                        ndx--;
                    }
                }
                else
                    ndx++;
            }

            return String.Join("/", pathComps.ToArray());
        }

        private FileInfo parseFileInfo(string lsLine)
        {
            bool numericDate = true;
            bool isDevice = false;
            int nameField = 7;
            int dateField = 5;
            char[] fieldseps = { ' ', '\t' };

            string[] ffields = lsLine.Split(fieldseps, nameField + 1, StringSplitOptions.RemoveEmptyEntries);
            if (ffields.GetUpperBound(0) < nameField)
                return null;

            if (ffields[4].EndsWith(","))   // Device node
            {
                isDevice = true;
                dateField++;
                nameField++;
                ffields = lsLine.Split(fieldseps, nameField + 1, StringSplitOptions.RemoveEmptyEntries);
                if (ffields.GetUpperBound(0) < nameField)
                    return null;
            }

            char dateIndicator = ffields[dateField][0];
            if ((dateIndicator < '0') || (dateIndicator > '9'))
            {
                numericDate = false;
                nameField++;
                ffields = lsLine.Split(fieldseps, nameField + 1, StringSplitOptions.RemoveEmptyEntries);
                if (ffields.GetUpperBound(0) < nameField)
                    return null;
            }

            if (ffields[0].Length < 10)
                return null;

            FileInfo retVal = new FileInfo();
            retVal.Name = ffields[nameField];
            retVal.Owner = ffields[2];
            retVal.Group = ffields[3];

            int whereSlash = retVal.Name.LastIndexOf('/');
            if (whereSlash < 0)
            {
                retVal.FileName = retVal.Name;
                retVal.Path = ".";
            }
            else
            {
                retVal.FileName = retVal.Name.Substring(whereSlash + 1);
                if (whereSlash > 0)
                    retVal.Path = retVal.Name.Substring(0, whereSlash);
                else
                    retVal.Path = "/";
            }

            char fTypeChar = ffields[0][0];
            switch (fTypeChar)
            {
                // This is the file's type: regular file (regular), directory, named pipe (fifo), symbolic link, socket or block special.
                case '-':
                    retVal.FileType = "regular";
                    retVal.IsDirectory = false;
                    retVal.IsSpecial = false;
                    break;
                case 'd':
                    retVal.FileType = "directory";
                    retVal.IsDirectory = true;
                    retVal.IsSpecial = false;
                    break;
                case 'l':
                    retVal.FileType = "symbolic link";
                    int arrow = retVal.Name.IndexOf(" -> ");
                    if (arrow > 0)
                    {
                        string linkName = retVal.Name.Substring(0, arrow);
                        string targetName = retVal.Name.Substring(arrow + 4);

                        whereSlash = linkName.LastIndexOf('/');
                        if (whereSlash < 0)
                        {
                            retVal.FileName = linkName;
                            retVal.Path = ".";
                        }
                        else
                        {
                            retVal.FileName = linkName.Substring(whereSlash + 1);
                            if (whereSlash > 0)
                                retVal.Path = linkName.Substring(0, whereSlash);
                            else
                                retVal.Path = "/";
                        }

                        if (!targetName.StartsWith("/"))
                        {
                            if (retVal.Path.EndsWith("/"))
                                targetName = retVal.Path + targetName;
                            else
                                targetName = retVal.Path + "/" + targetName;
                        }
                        targetName = DeDotPath(targetName);
                        retVal.Name = linkName;
                        retVal.LinksTo = targetName;
                    }
                    break;
                case 'b':
                    retVal.FileType = "block special";
                    retVal.IsDirectory = false;
                    retVal.IsSpecial = true;
                    break;
                case 'c':
                    retVal.FileType = "character special";
                    retVal.IsDirectory = false;
                    retVal.IsSpecial = true;
                    break;
                case 'p':
                    retVal.FileType = "fifo";
                    retVal.IsDirectory = false;
                    retVal.IsSpecial = true;
                    break;
                case 's':
                    retVal.FileType = "socket";
                    retVal.IsDirectory = false;
                    retVal.IsSpecial = true;
                    break;
                default:
                    retVal.IsDirectory = false;
                    retVal.IsSpecial = true;
                    break;
            }
            if (isDevice)
                retVal.FileSize = 0;
            else
                retVal.FileSize = ulong.Parse(ffields[4]);

            uint myMode = 0;
            string perms = ffields[0].Substring(1, 9);
            string lperms = perms.ToLower();
            if (lperms[2] == 's')
                myMode += 0x800;    // suid
            if (lperms[5] == 's')
                myMode += 0x200;    // sgid
            if (lperms[8] == 't')
                myMode += 0x100;    // sticky
            perms = perms.Replace('s', 'x');
            perms = perms.Replace('S', '-');
            perms = perms.Replace('t', 'x');
            perms = perms.Replace('T', '-');
            for (int i = 0; i < 9; i++)
            {
                if (perms[8 - i] != '-')
                    myMode += (uint)(1 << i);
            }
            retVal.Mode = myMode;

            try
            {
                if (numericDate)
                    retVal.LastModified = DateTime.ParseExact(ffields[dateField] + " " + ffields[dateField + 1], "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                else if (ffields[dateField + 2].Contains(":"))
                {
                    int thisYear = DateTime.Now.Year;
                    var dateAsString = ffields[dateField] + " " + ffields[dateField + 1] + " " + thisYear.ToString() + " " + ffields[dateField + 2];
                    retVal.LastModified = DateTime.ParseExact(dateAsString, "MMM d yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    if (retVal.LastModified > DateTime.Now)
                        retVal.LastModified = DateTime.ParseExact(ffields[dateField] + " " + ffields[dateField + 1] + " " + (thisYear - 1).ToString() + " " + ffields[dateField + 2], "MMM d yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                    retVal.LastModified = DateTime.ParseExact(ffields[dateField] + " " + ffields[dateField + 1] + " " + ffields[dateField + 2], "MMM d yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                // Just in case anything goes wrong with DateTime.ParseExact()
                retVal.LastModified = DateTime.MinValue;
            }

            retVal.Found = true;
            return retVal;
        }

        

        public virtual Dictionary<string, FileInfo> GetTargetFileInfo(string pathspec)
        {
            var lsOutput = this.LsCommand.Run(pathspec);
            var retList = new Dictionary<string, FileInfo>();
            char[] lineseps = { '\r', '\n' };
            string[] lines = lsOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                FileInfo thisInfo = parseFileInfo(line);
                if ((thisInfo != null) && IsValidFile(thisInfo.FileName))
                    retList[thisInfo.FullPath] = thisInfo;
            }
            return retList;
        }

        private bool IsValidFile(string filename)
        {
            return (!(filename.Trim().Equals(".") || filename.Trim().Equals("..")));
        }
    }
}
