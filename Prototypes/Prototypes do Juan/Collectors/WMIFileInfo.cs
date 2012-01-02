using System;
using System.Collections.Generic;

namespace FrameworkNG
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
    public class WMIFileInfo : GenericFileInfo
    {
        // Manter: Query, Drive, Extension, DACL, Archive, Compressed, Encrypted, System

        /// <summary>
        /// Gets or sets the WMI query string.
        /// </summary>
        /// <value>WMI query string.</value>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file's archive bit is set.
        /// </summary>
        /// <value><c>true</c> if archive; otherwise, <c>false</c>.</value>
        public bool Archive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is compressed.
        /// </summary>
        /// <value><c>true</c> if compressed; otherwise, <c>false</c>.</value>
        public bool Compressed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is encrypted.
        /// </summary>
        /// <value><c>true</c> if encrypted; otherwise, <c>false</c>.</value>
        public bool Encrypted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is a system file.
        /// </summary>
        /// <value><c>true</c> if system; otherwise, <c>false</c>.</value>
        public bool System { get; set; }

        public string Drive { get; set; }
        public string Extension { get; set; }
        public List<WMIWinACE> DACL { get; set; }
        public List<WMIWinACE> SACL { get; set; }

        public WMIFileInfo() : base ()
        {
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
            retVal += String.Format("Path: {0}\n", Path);
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
