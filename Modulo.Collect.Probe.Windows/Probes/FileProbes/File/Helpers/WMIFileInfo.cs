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
using System.Collections.Generic;

namespace Modulo.Collect.Probe.Windows.File.Helpers
{
    /// <summary>
    /// === WMIFileInfo ===
    /// Author: jcastro
    /// Creation Date: 10/06/2009
    /// Description: Contains information about a file obtained via WMI calls
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: N/A
    /// Example: N/A
    /// </summary>
    public class WMIFileInfo
    {
        /// <summary>
        /// Gets or sets a valueToMatch indicating whether a file is in fact a directory.
        /// </summary>
        /// <valueToMatch><c>true</c> if directory; otherwise, <c>false</c>.</valueToMatch>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// Gets or sets a valueToMatch indicating whether a file was found.
        /// </summary>
        /// <valueToMatch><c>true</c> if found; otherwise, <c>false</c>.</valueToMatch>
        public bool Found { get; set; }

        /// <summary>
        /// Gets or sets the WMI query string.
        /// </summary>
        /// <valueToMatch>WMI query string.</valueToMatch>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <valueToMatch>Error message.</valueToMatch>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// Gets or sets the file owner. Format: DOMAIN\username
        /// </summary>
        /// <valueToMatch>The owner.</valueToMatch>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets the file group. Format: DOMAIN\groupname
        /// </summary>
        /// <valueToMatch>The group.</valueToMatch>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets a valueToMatch indicating whether the file's archive bit is set.
        /// </summary>
        /// <valueToMatch><c>true</c> if archive; otherwise, <c>false</c>.</valueToMatch>
        public bool Archive { get; set; }

        /// <summary>
        /// Gets or sets a valueToMatch indicating whether the file is compressed.
        /// </summary>
        /// <valueToMatch><c>true</c> if compressed; otherwise, <c>false</c>.</valueToMatch>
        public bool Compressed { get; set; }

        /// <summary>
        /// Gets or sets a valueToMatch indicating whether the file is encrypted.
        /// </summary>
        /// <valueToMatch><c>true</c> if encrypted; otherwise, <c>false</c>.</valueToMatch>
        public bool Encrypted { get; set; }

        /// <summary>
        /// Gets or sets a valueToMatch indicating whether the file is hidden.
        /// </summary>
        /// <valueToMatch><c>true</c> if hidden; otherwise, <c>false</c>.</valueToMatch>
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets a valueToMatch indicating whether the file is a system file.
        /// </summary>
        /// <valueToMatch><c>true</c> if system; otherwise, <c>false</c>.</valueToMatch>
        public bool System { get; set; }

        /// <summary>
        /// Gets or sets a valueToMatch indicating whether the file is writeable, i.e., the opposite of "read-only".
        /// </summary>
        /// <valueToMatch><c>true</c> if writeable; otherwise, <c>false</c>.</valueToMatch>
        public bool Writeable { get; set; }

        /// <summary>
        /// Gets or sets the full file name.
        /// </summary>
        /// <valueToMatch>The name.</valueToMatch>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file's eight-dot-three name.
        /// </summary>
        /// <valueToMatch>The file's eight-dot-three name.</valueToMatch>
        public string EightDotThreeFileName { get; set; }

        /// <summary>
        /// Gets or sets the file's type string.
        /// </summary>
        /// <valueToMatch>Type string.</valueToMatch>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the file's manufacturer string.
        /// </summary>
        /// <valueToMatch>Manufacturer string.</valueToMatch>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the file version.
        /// </summary>
        /// <valueToMatch>The file version.</valueToMatch>
        public string Version { get; set; }

        public string Drive { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime InstallDate { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime LastModified { get; set; }

        public ulong FileSize { get; set; }

        public List<WMIWinACE> DACL { get; set; }

        public WMIFileInfo()
        {
            Found = false;
        }

        public override string ToString()
        {
            string retVal = "";
            retVal += String.Format("Found: {0}\n", Found);
            retVal += String.Format("IsDirectory: {0}\n", IsDirectory);
            retVal += String.Format("Query: {0}\n", Query);
            retVal += String.Format("Error Message: {0}\n", ErrorMsg);

            retVal += String.Format("Name: {0}\n", Name);
            retVal += String.Format("EightDotThreeFileName: {0}\n", EightDotThreeFileName);
            retVal += String.Format("FileType: {0}\n", FileType);
            retVal += String.Format("Manufacturer: {0}\n", Manufacturer);
            retVal += String.Format("Version: {0}\n", Version);
            retVal += String.Format("Drive: {0}\n", Drive);
            retVal += String.Format("Extension: {0}\n", Extension);
            retVal += String.Format("FileName: {0}\n", FileName);
            retVal += String.Format("path: {0}\n", Path);
            retVal += String.Format("FileSize: {0}\n", FileSize);

            retVal += String.Format("Owner: {0}\n", Owner);
            retVal += String.Format("Group: {0}\n", Group);

            retVal += String.Format("Archive: {0}\n", Archive);
            retVal += String.Format("Compressed: {0}\n", Compressed);
            retVal += String.Format("System: {0}\n", System);
            retVal += String.Format("Hidden: {0}\n", Hidden);
            retVal += String.Format("Encrypted: {0}\n", Encrypted);
            retVal += String.Format("Writeable: {0}\n", Writeable);

            retVal += String.Format("CreationDate: {0}\n", CreationDate);
            retVal += String.Format("InstallDate: {0}\n", InstallDate);
            retVal += String.Format("LastAccessed: {0}\n", LastAccessed);
            retVal += String.Format("LastModified: {0}", LastModified);

            return retVal;
        }
    }
}
