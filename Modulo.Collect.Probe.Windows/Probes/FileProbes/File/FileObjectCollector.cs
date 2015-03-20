/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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
using System.Globalization;
using System.IO;
using System.Linq;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Extensions;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Windows.WMI;
using System.Text;

namespace Modulo.Collect.Probe.Windows.File
{



    public class FileObjectCollector : BaseObjectCollector
    {
        private const string WMI_FILE_CLASS = "CIM_LogicalFile";

        public WmiDataProvider WmiDataProvider { get; set; }

        public override IList<String> GetValues(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException("FileObjectCollector.GetValues() is deprecated now. Use WindowsFileProvider.GetFileChildren() instead.");
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            var fullFilePath = this.GetCompleteFilePath((file_item)systemItem);
            try
            {
                base.ExecutionLogBuilder.CollectingDataFrom(fullFilePath);
                var collectedSystemData = this.CollectFileItemSystemData((file_item)systemItem);
                new FileItemTypeBuilder().FillItemTypeWithData((file_item)systemItem, collectedSystemData);
            }
            catch (KeyNotFoundException)
            {
                SetDoesNotExistStatusForItemType(systemItem, fullFilePath);
            }

            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }



        private FileItemSystemData CollectFileItemSystemData(file_item fileItem)
        {
            WmiObject wmiLogicalFile = this.OpenWmiLogicalFileClass(fileItem);
            string fullFilePath = this.GetCompleteFilePath(fileItem);


            var fileItemSystemData = new FileItemSystemData();
            this.FillFileItemSystemData(wmiLogicalFile.GetValues(), fileItemSystemData);
            return fileItemSystemData;
        }

        private void FillFileItemSystemData(Dictionary<string, object> wmiLogicalFileData, FileItemSystemData fileSystemData)
        {
            fileSystemData.Size = Int32.Parse(wmiLogicalFileData.GetObjectValueOrDefaultAsString("FileSize", "-1"));
            fileSystemData.CTime = this.GetFileTime(wmiLogicalFileData.GetObjectValueOrDefaultAsString("CreationDate"));
            fileSystemData.MTime = this.GetFileTime(wmiLogicalFileData.GetObjectValueOrDefaultAsString("LastModified"));
            fileSystemData.ATime = this.GetFileTime(wmiLogicalFileData.GetObjectValueOrDefaultAsString("LastAccessed"));
            fileSystemData.Version = wmiLogicalFileData.GetObjectValueOrDefaultAsString("Version");
            fileSystemData.Type = wmiLogicalFileData.GetObjectValueOrDefaultAsString("FileType");
            fileSystemData.Company = wmiLogicalFileData.GetObjectValueOrDefaultAsString("Manufacturer");
        }

        private WmiInvokeMethodInfo CreateInvokeMethodInfoToGetFileSecurity(string fullFilePath)
        {
            return new WmiInvokeMethodInfo()
            {
                ClassName = "Win32_LogicalFileSecuritySetting",
                MethodName = "GetSecurityDescriptor",
                PathName = "path",
                PathValue = fullFilePath
            };
        }

        private void FillItemWithApplicationAttributes(FileItemSystemData fileSystemData, string path)
        {
            WmiInvokeMethodInfo invokeMethodInfo = this.CreateInvokeMethodInfoToGetFileSecurity(path);
            #region Not Implemented
            /* #ToDo Review this method.
            //ManagementBaseObject secutiryDescriptor = (ManagementBaseObject)this.WmiDataProvider.InvokeMethodByWmiPath(invokeMethodInfo);
            //ManagementBaseObject descriptor = (ManagementBaseObject)secutiryDescriptor.Properties["Descriptor"].Value;
            //ManagementBaseObject owner = (ManagementBaseObject)descriptor.Properties["Owner"].Value;
            //fileSystemData.Owner = String.Format("{0}\\{1}", owner.Properties["Domain"].Value, owner.Properties["Name"].Value);
            
            //fileSystemData.MS_Checksum = "[NOT IMPLEMENTED]";
            //fileSystemData.DevelopmentClass = "[NOT IMPLEMENTED]";
            //fileSystemData.InternalName = "[NOT IMPLEMENTED]";
            //fileSystemData.Language = "[NOT IMPLEMENTED]";
            //fileSystemData.OriginalFilename = "[NOT IMPLEMENTED]";
            //fileSystemData.ProductName = "[NOT IMPLEMENTED]";
            //fileSystemData.ProductVersion = "[NOT IMPLEMENTED]";
            */
            #endregion
        }

        private string GetCompleteFilePath(file_item fileItem)
        {
            bool isFilePathDefined = ((fileItem.filepath != null) && (!string.IsNullOrEmpty(fileItem.filepath.Value)));
            string completeFilePath = isFilePathDefined ? fileItem.filepath.Value : ConcatFilePathAndFileName(fileItem);

            if (string.IsNullOrEmpty(Path.GetExtension(completeFilePath)) && (!completeFilePath[completeFilePath.Length - 1].Equals(@"\")))
                return string.Format(@"{0}\", completeFilePath);

            return completeFilePath;
        }

        private string ConcatFilePathAndFileName(file_item fileItem)
        {
            if (fileItem.path.Value.EndsWith("\\"))
            {
                return string.Format("{0}{1}", fileItem.path.Value, fileItem.filename.Value);
            }
            else
            {
                return string.Format("{0}\\{1}", fileItem.path.Value, fileItem.filename.Value);
            }
        }

        private long GetFileTime(string dateTimeInWmiFormat)
        {
            if (string.IsNullOrEmpty(dateTimeInWmiFormat))
                return -1;

            return DateTime.ParseExact(dateTimeInWmiFormat.ToString().Substring(0, 21), "yyyyMMddHHmmss.ffffff", CultureInfo.InvariantCulture).ToFileTime();
        }

        private WmiObject OpenWmiLogicalFileClass(file_item fileItem)
        {
            var fullFilePath = this.GetCompleteFilePath(fileItem);
            var isDirectory = 
                ((fileItem.path != null) && 
                    ((fileItem.filename == null) || 
                    (string.IsNullOrWhiteSpace(fileItem.filename.Value))));

            IEnumerable<WmiObject> result = null;
            if (isDirectory)
            {
                fullFilePath = fullFilePath.Replace("\\\\", "\\");
                var wql = GetWqlForSearchDirectory(fullFilePath);
                result = this.WmiDataProvider.ExecuteWQL(wql);
            }
            else
            {
                var wmiInParameters = this.CreateWmiParameters(fullFilePath);
                result = this.WmiDataProvider.SearchWmiObjects(WMI_FILE_CLASS, wmiInParameters);
            }

            return ExtractResultFromWmiReturn(result);
        }

        private WmiObject ExtractResultFromWmiReturn(IEnumerable<WmiObject> wmiReturn)
        {
            if (wmiReturn.Count() == 0)
                throw new KeyNotFoundException();

            return wmiReturn.First();
        }

        private string GetWqlForSearchDirectory(string fullPath)
        {
            // select * from Win32_Directory where drive = 'c:' and filename ='definitions' and path = '\\temp\\' 
            var fileLogicUnit = Path.GetPathRoot(fullPath);
            // c:\temp\definitions
            var pathParts = fullPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var pathBuilder = new StringBuilder(@"\\");
            for (int i = 1; i < pathParts.Count() - 1; i++)
                pathBuilder.Append(pathParts.ElementAt(i) + @"\\");
            
            var drive = fileLogicUnit.Replace(@"\", string.Empty);
            var filename = pathParts.Last();
            var path = pathBuilder.ToString();
            return 
                new WQLBuilder()
                    .WithWmiClass("Win32_Directory")
                        .AddParameter("drive", drive)
                        .AddParameter("filename", filename)
                        .AddParameter("path", path)
                    .Build();
        }



        private Dictionary<string, string> CreateWmiParameters(string completeFilePath)
        {
            var fileLogicUnit = Path.GetPathRoot(completeFilePath);
            var fileDirectory = "";
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(completeFilePath)))
                fileDirectory = Path.GetDirectoryName(completeFilePath).Replace(fileLogicUnit, string.Empty).Replace(@"\", @"\\");

            var fileName = Path.GetFileNameWithoutExtension(completeFilePath);
            var fileExtension = Path.GetExtension(completeFilePath).Replace(".", string.Empty);

            var parameters = new Dictionary<string, string>();
            parameters.AddStringIfItIsNotNullToDictionary(WqlFileParameters.Drive.ToString(), fileLogicUnit.Replace(@"\", string.Empty));
            parameters.AddStringIfItIsNotNullToDictionary(WqlFileParameters.Path.ToString(), string.Format(@"\\{0}\\", fileDirectory).Replace(@"\\\\", @"\\"));
            parameters.AddStringIfItIsNotNullToDictionary(WqlFileParameters.FileName.ToString(), fileName);
            parameters.AddStringIfItIsNotNullToDictionary(WqlFileParameters.Extension.ToString(), fileExtension);

            return parameters;
        }
    }

    public enum WqlFileParameters { Drive, Path, FileName, Extension, FileType };

    public enum GeneratedFileItemAttributes
    {
        FileSize,
        CreationDate, LastModified, LastAccessed,
        FileType,
        Version,
        Manufacturer
    };
}
