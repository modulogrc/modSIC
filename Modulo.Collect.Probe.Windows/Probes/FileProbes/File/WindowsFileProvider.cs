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


using System.Collections.Generic;
using System.IO;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.Extensions;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Windows.WMI;
using System;
using System.Linq;
using Modulo.Collect.Probe.Independent.Common.File;
using Modulo.Collect.Probe.Windows.Helpers;


namespace Modulo.Collect.Probe.Windows.File
{
    public class WindowsFileProvider: IFileProvider
    {
        public WmiDataProvider WmiDataProvider { get; set; }

        private const string FILENAME_IN_ADMINISTRATIVE_SHARE_FORMAT = @"\\{0}\{1}{2}";
        private const string WMI_FILE_CLASS = "CIM_LogicalFile";
        private TargetInfo TargetInfo;

        public WindowsFileProvider(TargetInfo targetInfo) 
        {
            this.TargetInfo = targetInfo;
        }

        public WindowsFileProvider(TargetInfo targetInfo, WmiDataProvider wmiDataProvider) : this(targetInfo)
        {
            this.WmiDataProvider = wmiDataProvider;
        }

        public virtual IEnumerable<String> GetFileLinesContentFromHost(string localFilepath)
        {
            var adminShareFilePath = this.GetAdministrativeSharePathFromLocalFilepath(TargetInfo.GetAddress(), localFilepath);
            if (string.IsNullOrWhiteSpace(adminShareFilePath))
                return new string[] { };
            
            return System.IO.File.ReadAllLines(adminShareFilePath);
        }

        public IEnumerable<string> GetFileChildren(string parentDirectory)
        {
            var wqlParameters = this.CreateWmiParameters(parentDirectory);
            var targetFiles = this.WmiDataProvider.SearchWmiObjects(WMI_FILE_CLASS, wqlParameters);

            var fullFileNames = new List<String>();
            foreach (var file in targetFiles)
            {
                var fileType = file.GetValueOf("FileType").ToString();
                var filename = file.GetValueOf("Name").ToString();
                string path = fileType.Equals("File Folder") ? string.Format("{0}\\", filename) : filename;

                fullFileNames.Add(path);
            }

            return fullFileNames;
        }

        public string GetAdministrativeSharePathFromLocalFilepath(string hostname, string localFilepath)
        {
            if (!localFilepath.Contains(":"))
            {
                if (localFilepath.ToLower().Contains("%systemdrive%"))
                    localFilepath = localFilepath.ToLower().Replace("%systemdrive%", "c:");
                else
                    return string.Empty;
            }
            
            var rootPath = Path.GetPathRoot(localFilepath);
            var remoteDrivePath = rootPath.Replace(":", "$");
            string filepathWithoutDrivePath = localFilepath.Replace(rootPath, string.Empty);

            return string.Format(FILENAME_IN_ADMINISTRATIVE_SHARE_FORMAT, hostname, remoteDrivePath, filepathWithoutDrivePath);
        }

        public bool FileExists(string localFilepath)
        {
            /* IMPORTANT
             * BEFORE CHANGING OR REFACTORING THIS METHOD, YOU MUST READ THIS NOTICE.
             * 
             * The code below sounds confusing, but there is a reason to be like that.
             * The first issue is that the System.IO.File.Exists and Directory.Exists behavior (see Remarks Session in http://msdn.microsoft.com/en-us/library/system.io.file.exists.aspx).
             * If an error such as "Acess Denied" or "Invalid Path" occurs during those methods calling,
             * .NET API will return FALSE instead of throwing an exception. Thus, when the return is false,
             * maybe the file or directory exists, but some error occurred.
             * The best way to find out if a file or directory exists is trying to open them. The methods
             * that open a file (or directory) will throw an exception if it does not exist.
             * 
             * Now, we have a second issue:
             * To do the above, it is necessary that the administrative share on the remote machine is enabled.
             * It´s very common that is not enabled.
             * The solution to this issue is to make another attempt in order to check file (or directory) existence.
             * You can make that through a WMI query.
             * 
             * So, we have three ways to check File Existence. Why not use only WMI ?
             * Because, we have a third issue: performance.
             * In some scenarios, this WMI Query can be really slow.
             * OK, but why the code below still using File.Exists and Directory.Exists methods?
             * When those methods return TRUE, we can safely say that file (or directory) really exists.
             * Besides, those methods are very fast. Hence, we can stop the method if one of those methods return TRUE.
             */

            try
            {
                var windowsConnectionProvider = new StraightNetworkConnectionProvider();
                var adminShareFilePath = GetAdministrativeSharePathFromLocalFilepath(TargetInfo.GetAddress(), localFilepath);
                try
                {
                    // To use Administrative Share resource, we need open a straight connection to remote machine.
                    windowsConnectionProvider.Connect(TargetInfo);
                    try
                    {
                        // If one of these methods return TRUE, we can return this result.
                        if (System.IO.File.Exists(adminShareFilePath) || System.IO.Directory.Exists(adminShareFilePath))
                            return true;

                        // If both methods above return FALSE, we CAN NOT rely on in this.
                        try
                        {
                            // So, we will try to open the file...
                            System.IO.File.Open(adminShareFilePath, FileMode.Open);
                            // If we could open it, the file exists.
                            return true;
                        }
                        catch (FileNotFoundException)
                        {
                            // obviously we can return FALSE if File.Open thrown FileNotFoundException.
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            // If any else Exception was thrown, maybe the passed path is a directory...
                            // So, we will try to open it.
                            System.IO.Directory.EnumerateFiles(adminShareFilePath, "*");
                            return true;
                        }
                        catch(FileNotFoundException)
                        {
                            return false;
                        }
                    }
                }
                finally
                {
                    windowsConnectionProvider.Disconnect();
                }
            }
            catch (Exception)
            {
                // At last, if it was not possible to check file (or directory) existence due to any error,
                // we will try to find this information out through WMI.
                var wmiParametersForFileSearching = this.CreateWmiParameters(localFilepath);
                var wqlForFileSearching = new WQLBuilder().WithWmiClass("CIM_LogicalFile").AddParameters(wmiParametersForFileSearching).Build();
                var wmiQueryResult = this.WmiDataProvider.ExecuteWQL(wqlForFileSearching);
                
                return ((wmiQueryResult != null) && (wmiQueryResult.Count() > 0));
            }
        }

        /// <summary>
        /// This method is public for unit tests porposes.
        /// </summary>
        /// <param name="filepath">Filepath</param>
        /// <returns></returns>
        public Dictionary<string, string> CreateWmiParameters(string filepath)
        {
            var fileLogicUnit = Path.GetPathRoot(filepath);
            var fileDirectory = "";
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(filepath)))
                fileDirectory = 
                    Path.GetDirectoryName(filepath)
                        .Replace(fileLogicUnit, string.Empty)
                        .Replace(@"\", @"\\");

            var fileName = Path.GetFileNameWithoutExtension(filepath);
            var fileExtension = Path.GetExtension(filepath).Replace(".", string.Empty);

            var parameters = new Dictionary<string, string>();
            parameters.AddStringIfItIsNotNullToDictionary(WqlFileParameters.Drive.ToString(), fileLogicUnit.Replace(@"\", string.Empty));
            parameters.AddStringIfItIsNotNullToDictionary(WqlFileParameters.Path.ToString(), string.Format(@"\\{0}\\", fileDirectory).Replace(@"\\\\", @"\\"));
            parameters.AddStringIfItIsNotNullToDictionary(WqlFileParameters.FileName.ToString(), fileName);
            parameters.AddStringIfItIsNotNullToDictionary(WqlFileParameters.Extension.ToString(), fileExtension);

            return parameters;
        }



        public virtual IEnumerable<String> GetChildrenDirectories(string fixedPart)
        {
            var wqlFilter = GeWqlFilter(fixedPart);
            
            var wql = new WQLBuilder().WithWmiClass("Win32_Directory").AddParameters(wqlFilter).Build();

            return this.WmiDataProvider.ExecuteWQL(wql).Select(row => row.GetFieldValueAsString("FileName"));
        }

        public virtual IEnumerable<String> GetChildrenFiles(string parentDirectory)
        {
            var wqlFilter = GeWqlFilter(parentDirectory);

            var wql = new WQLBuilder().WithWmiClass("CIM_DataFile").AddParameters(wqlFilter).Build();

            return this.WmiDataProvider.ExecuteWQL(wql).Select(row => row.GetFieldValueAsString("FileName"));
        }

        private Dictionary<string, string> GeWqlFilter(string directoryPath)
        {
            var wqlFilter = new Dictionary<string, string>();

            if (!directoryPath.EndsWith("\\"))
                directoryPath += "\\";

            var logicUnit = Path.GetPathRoot(directoryPath);
            wqlFilter.Add(WqlFileParameters.Drive.ToString(), logicUnit.Replace(@"\", string.Empty));
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(directoryPath)))
            {
                var directory =
                    Path.GetDirectoryName(directoryPath)
                        .Replace(logicUnit, string.Empty)
                        .Replace(@"\", @"\\");

                wqlFilter.Add(WqlFileParameters.Path.ToString(), string.Format(@"\\{0}\\", directory).Replace(@"\\\\", @"\\"));
            }
            else
                wqlFilter.Add(WqlFileParameters.Path.ToString(), @"\\");


            return wqlFilter;
        }
    }
}
