using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class WMIWinACE
    {
        public bool IsDirectory = false;
        public UInt32 AccessMask;
        public UInt32 AceFlags;
        public UInt32 AceType;
        public string GuidInheritedObjectType;
        public string GuidObjectType;
        public WMIWinTrustee Trustee;

        public bool FILE_READ_DATA { get { return (this.AccessMask & 0x1) != 0; } }
        public bool FILE_WRITE_DATA { get { return (this.AccessMask & 0x2) != 0; } }
        public bool FILE_APPEND_DATA { get { return (this.AccessMask & 0x4) != 0; } }
        public bool FILE_READ_EA { get { return (this.AccessMask & 0x8) != 0; } }
        public bool FILE_WRITE_EA { get { return (this.AccessMask & 0x10) != 0; } }
        public bool FILE_EXECUTE { get { return (this.AccessMask & 0x20) != 0; } }
        public bool FILE_DELETE_CHILD { get { return (this.AccessMask & 0x40) != 0; } }
        public bool FILE_READ_ATTRIBUTES { get { return (this.AccessMask & 0x80) != 0; } }
        public bool FILE_WRITE_ATTRIBUTES { get { return (this.AccessMask & 0x100) != 0; } }
        public bool DELETE { get { return (this.AccessMask & 0x10000) != 0; } }
        public bool READ_CONTROL { get { return (this.AccessMask & 0x20000) != 0; } }
        public bool WRITE_DAC { get { return (this.AccessMask & 0x40000) != 0; } }
        public bool WRITE_OWNER { get { return (this.AccessMask & 0x80000) != 0; } }
        public bool SYNCHRONIZE { get { return (this.AccessMask & 0x100000) != 0; } }

        public bool FILE_LIST_DIRECTORY { get { return (this.AccessMask & 0x1) != 0; } }
        public bool FILE_ADD_FILE { get { return (this.AccessMask & 0x2) != 0; } }
        public bool FILE_ADD_SUBDIRECTORY { get { return (this.AccessMask & 0x4) != 0; } }
        public bool FILE_TRAVERSE { get { return (this.AccessMask & 0x20) != 0; } }

        public override string ToString()
        {
            string retVal;

            retVal = String.Format("{0}:", this.Trustee);
            if (this.IsDirectory)
            {
                if (this.FILE_LIST_DIRECTORY) retVal += " FILE_LIST_DIRECTORY";
                if (this.FILE_ADD_FILE) retVal += " FILE_ADD_FILE";
                if (this.FILE_ADD_SUBDIRECTORY) retVal += " FILE_ADD_SUBDIRECTORY";
                if (this.FILE_TRAVERSE) retVal += " FILE_TRAVERSE";
            }
            else
            {
                if (this.FILE_READ_DATA) retVal += " FILE_READ_DATA";
                if (this.FILE_WRITE_DATA) retVal += " FILE_WRITE_DATA";
                if (this.FILE_APPEND_DATA) retVal += " FILE_APPEND_DATA";
                if (this.FILE_EXECUTE) retVal += " FILE_EXECUTE";
            }
            if (this.FILE_READ_EA) retVal += " FILE_READ_EA";
            if (this.FILE_WRITE_EA) retVal += " FILE_WRITE_EA";
            if (this.FILE_DELETE_CHILD) retVal += " FILE_DELETE_CHILD";
            if (this.FILE_READ_ATTRIBUTES) retVal += " FILE_READ_ATTRIBUTES";
            if (this.FILE_WRITE_ATTRIBUTES) retVal += " FILE_WRITE_ATTRIBUTES";
            if (this.DELETE) retVal += " DELETE";
            if (this.READ_CONTROL) retVal += " READ_CONTROL";
            if (this.WRITE_DAC) retVal += " WRITE_DAC";
            if (this.WRITE_OWNER) retVal += " WRITE_OWNER";
            if (this.SYNCHRONIZE) retVal += " SYNCHRONIZE";
            return retVal;
        }
    }
}
