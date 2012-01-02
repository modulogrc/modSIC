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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Probe.Windows.File.Helpers
{
    public class FileMethod
    {
        private static ManagementObjectSearcher MyGetSearcher(ManagementScope scope, string myquery)
        {
            EnumerationOptions options = new EnumerationOptions();
            options.Rewindable = false;
            options.ReturnImmediately = true;

            return new ManagementObjectSearcher(scope, new ObjectQuery(myquery), options);
        }

        public static WMIFileInfo CollectFileInfo(ManagementScope scope, string path)
        {
            WMIFileInfo retVal = new WMIFileInfo();

            // Atributes from from CIM_DataFile (creation date, size, etc)
            string pathDrive = Path.GetPathRoot(path);
            string pathPath = Path.GetDirectoryName(path);
            string pathFilename = Path.GetFileNameWithoutExtension(path);
            string pathExtension = Path.GetExtension(path);

            if (pathDrive[1] != ':')
                throw new CollectorException(String.Format("Invalid path '{0}': must be a full path with drive letter", path));
            pathDrive = pathDrive.Substring(0, 2);
            pathPath = pathPath.Substring(2);
            if (pathPath[pathPath.Length - 1] != '\\')
            {
                pathPath += '\\';
            }
            pathPath = pathPath.Replace("\\", "\\\\");
            if (Path.HasExtension(path))
                pathExtension = pathExtension.Substring(1);

            try
            {
                ManagementObject queryObj = null;
                retVal.Query = String.Format("SELECT * FROM CIM_LogicalFile WHERE Drive = '{0}' AND path = '{1}' AND FileName = '{2}' AND Extension = '{3}'", pathDrive, pathPath, pathFilename, pathExtension);
                ManagementObjectSearcher searcher = MyGetSearcher(scope, retVal.Query);
                foreach (ManagementObject tempQueryObj in searcher.Get())
                {
                    queryObj = tempQueryObj;
                    break;
                }
                if (queryObj == null)
                {
                    retVal.Query = String.Format("SELECT * FROM CIM_LogicalFile WHERE Drive = '{0}' AND path = '{1}' AND FileName = '{2}.{3}' AND Extension = ''", pathDrive, pathPath, pathFilename, pathExtension);
                    searcher = MyGetSearcher(scope, retVal.Query);
                    foreach (ManagementObject tempQueryObj in searcher.Get())
                    {
                        queryObj = tempQueryObj;
                        break;
                    }
                }

                if (queryObj == null)
                {
                    retVal.Found = false;
                    retVal.ErrorMsg = "File not found";
                }
                else
                {
                    retVal.Found = true;

                    retVal.Drive = (string)queryObj["Drive"];
                    retVal.Path = (string)queryObj["path"];
                    retVal.Extension = (string)queryObj["Extension"];
                    retVal.FileName = (string)queryObj["FileName"];
                    retVal.Name = (string)queryObj["Name"];

                    retVal.Archive = (bool)queryObj["Archive"];
                    retVal.Compressed = (bool)queryObj["Compressed"];
                    retVal.EightDotThreeFileName = (string)queryObj["EightDotThreeFileName"];
                    retVal.Encrypted = (bool)queryObj["Encrypted"];
                    retVal.FileType = (string)queryObj["FileType"];
                    retVal.Hidden = (bool)queryObj["Hidden"];
                    retVal.System = (bool)queryObj["System"];

                    retVal.Writeable = (bool)queryObj["Writeable"];
                    retVal.CreationDate = DateTime.ParseExact(((string)queryObj["CreationDate"]).Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);
                    retVal.InstallDate = DateTime.ParseExact(((string)queryObj["InstallDate"]).Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);
                    retVal.LastAccessed = DateTime.ParseExact(((string)queryObj["LastAccessed"]).Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);
                    retVal.LastModified = DateTime.ParseExact(((string)queryObj["LastModified"]).Substring(0, 21), "yyyyMMddHHmmss.ffffff", System.Globalization.CultureInfo.InvariantCulture);

                    string wtfIsThis = queryObj.ClassPath.ClassName;
                    switch (wtfIsThis)
                    {
                        case "Win32_Directory":
                            retVal.IsDirectory = true;
                            break;
                        case "CIM_DataFile":
                            retVal.IsDirectory = false;
                            retVal.FileSize = (ulong)queryObj["FileSize"];
                            retVal.Manufacturer = (string)queryObj["Manufacturer"];
                            retVal.Version = (string)queryObj["Version"];
                            break;
                        default:
                            retVal.IsDirectory = false;
                            retVal.ErrorMsg = "Unexpected obect type '" + queryObj.ClassPath.ClassName + "'";
                            break;
                    }
                }

                // Attributes from Win32_LogicalFileSecuritySetting (Owner, Group, ACLs)
                ManagementObject mgmt = new ManagementObject(scope, new ManagementPath(String.Format("Win32_LogicalFileSecuritySetting.path='{0}'", path)), null);
                ManagementBaseObject secDesc = mgmt.InvokeMethod("GetSecurityDescriptor", null, null);
                ManagementBaseObject descriptor = secDesc.Properties["Descriptor"].Value as ManagementBaseObject;

                ManagementBaseObject owner = descriptor.Properties["Owner"].Value as ManagementBaseObject;
                retVal.Owner = String.Format("{0}\\{1}", owner.Properties["Domain"].Value, owner.Properties["Name"].Value);

                ManagementBaseObject group = descriptor.Properties["Group"].Value as ManagementBaseObject;
                retVal.Group = String.Format("{0}\\{1}", group.Properties["Domain"].Value, group.Properties["Name"].Value);

                retVal.DACL = new List<WMIWinACE>();
                ManagementBaseObject[] acls = descriptor.Properties["DACL"].Value as ManagementBaseObject[];
                if (acls != null)
                {
                    foreach (ManagementBaseObject thisacl in acls)
                    {
                        WMIWinACE thisace = new WMIWinACE();
                        thisace.IsDirectory = retVal.IsDirectory;
                        thisace.AccessMask = (UInt32)thisacl.Properties["AccessMask"].Value;
                        thisace.AceFlags = (UInt32)thisacl.Properties["AceFlags"].Value;
                        thisace.AceType = (UInt32)thisacl.Properties["AceType"].Value;
                        thisace.GuidInheritedObjectType = thisacl.Properties["GuidInheritedObjectType"].Value as string;
                        thisace.GuidObjectType = thisacl.Properties["GuidObjectType"].Value as string;

                        thisace.Trustee = new WMIWinTrustee();
                        ManagementBaseObject trustee = thisacl.Properties["Trustee"].Value as ManagementBaseObject;
                        thisace.Trustee.Domain = trustee.Properties["Domain"].Value as string;
                        thisace.Trustee.Name = trustee.Properties["Name"].Value as string;
                        thisace.Trustee.SID = trustee.Properties["SID"].Value as Byte[];
                        thisace.Trustee.SidLength = (UInt32)trustee.Properties["SidLength"].Value;
                        thisace.Trustee.SIDString = trustee.Properties["SIDString"].Value as string;

                        retVal.DACL.Add(thisace);
                    }
                }
            }
            catch (Exception excp)
            {
                retVal.ErrorMsg = String.Format("{0}: {1}", excp.GetType(), excp.Message);
            }

            return retVal;
        }
    }
}
