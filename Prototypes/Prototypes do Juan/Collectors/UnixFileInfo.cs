using System;
using System.Collections.Generic;

namespace FrameworkNG
{
    /// <summary>
    /// === UnixFileInfo ===
    /// Author: jcastro
    /// Creation Date: 10/06/2009
    /// Description: Contains information about a file in a UNIX file system
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: N/A
    /// Example: N/A
    /// </summary>
    public class UnixFileInfo : GenericFileInfo
    {
        /// <summary>
        /// Gets or sets the UNIX permission bitfield.
        /// </summary>
        /// <value>WMI query string.</value>
        public uint Mode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file is a special file (pipe, dev...).
        /// </summary>
        /// <value><c>true</c> if special file; otherwise, <c>false</c>.</value>
        public bool IsSpecial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file is a symbolic link.
        /// </summary>
        /// <value><c>true</c> if symlink; otherwise, <c>false</c>.</value>
        public bool IsSymLink
        {
            get
            {
                return (!String.IsNullOrEmpty(this.LinksTo));
            }
        }

        public string LinksTo { get; set; }

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

        public UnixFileInfo()
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
}
