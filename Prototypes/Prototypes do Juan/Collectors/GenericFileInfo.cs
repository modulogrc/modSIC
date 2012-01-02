using System;
using System.Collections.Generic;

namespace FrameworkNG
{
    /// <summary>
    /// === FileInfo ===
    /// Author: jcastro
    /// Creation Date: 21/09/2009
    /// Description: Contains information about a file obtained via remote collection
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: N/A
    /// Example: N/A
    /// </summary>
    public abstract class GenericFileInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether a file is in fact a directory.
        /// </summary>
        /// <value><c>true</c> if directory; otherwise, <c>false</c>.</value>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file was found.
        /// </summary>
        /// <value><c>true</c> if found; otherwise, <c>false</c>.</value>
        public bool Found { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>Error message.</value>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// Gets or sets the file owner. Format for Windows: DOMAIN\username
        /// </summary>
        /// <value>The owner.</value>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets the file group. Format for Windows: DOMAIN\groupname
        /// </summary>
        /// <value>The group.</value>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is hidden.
        /// </summary>
        /// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
        public virtual bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is writeable, i.e., the opposite of "read-only".
        /// </summary>
        /// <value><c>true</c> if writeable; otherwise, <c>false</c>.</value>
        public virtual bool Writeable { get; set; }

        /// <summary>
        /// Gets or sets the full file name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file's eight-dot-three name, if applicable.
        /// </summary>
        /// <value>The file's eight-dot-three name.</value>
        public string EightDotThreeFileName { get; set; }

        /// <summary>
        /// Gets or sets the file's type string, if applicable.
        /// </summary>
        /// <value>Type string.</value>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the file's manufacturer string, if applicable.
        /// </summary>
        /// <value>Manufacturer string.</value>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the file version, if applicable.
        /// </summary>
        /// <value>The file version.</value>
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
            Found = false;
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
}
