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
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Windows.AuditEventPolicy;

namespace Modulo.Collect.Probe.Windows.File.Helpers
{
    public partial class WMIWinACE
    {
        public bool IsDirectory = false;
        public string GuidInheritedObjectType;
        public string GuidObjectType;

        #region ACL

        public UInt32 AccessMask { get; set; }
        public UInt32? AceFlags { get; set; }
        public UInt32 AceType { get; set; }
        public WMIWinTrustee Trustee { get; set; }

        #endregion

        #region Standard Permissions

        public bool ACCESS_SYSTEM_SECURITY { get { return this.AccessSystemSecurity; } set { this.AccessSystemSecurity = value; } }
        public bool DELETE { get { return this.Delete; } set { this.Delete = value; } }
        public bool READ_CONTROL { get { return this.ReadControl; } set { this.ReadControl = value; } }
        public bool WRITE_DAC { get { return this.WriteDAC; } set { this.WriteDAC = value; } }
        public bool WRITE_OWNER { get { return this.WriteOwner; } set { this.WriteOwner = value; } }
        public bool SYNCHRONIZE { get { return this.Syncronize; } set { this.Syncronize = value; } }

        #endregion

        #region Generic Permissions

        public bool GENERIC_EXECUTE { get { return this.GenericExecute; } set { this.GenericExecute = value; } }
        public bool GENERIC_READ { get { return this.GenericRead; } set { this.GenericRead = value; } }
        public bool GENERIC_WRITE { get { return this.GenericWrite; } set { this.GenericWrite = value; } }
        public bool GENERIC_ALL { get { return this.GenericAll; } set { this.GenericAll = value; } }
        
        #endregion

        #region File Permissions

        public bool FILE_READ_DATA { get { return this.FileReadData; } set { this.FileReadData = value; } }
        public bool FILE_WRITE_DATA { get { return this.FileWriteData; } set { this.FileWriteData = value; } }
        public bool FILE_APPEND_DATA { get { return this.FileAppendData; } set { this.FileAppendData = value; } }
        public bool FILE_READ_EA { get { return this.FileReadEA; } set { this.FileReadEA = value; } }
        public bool FILE_WRITE_EA { get { return this.FileWriteEA; } set { this.FileWriteEA = value; } }
        public bool FILE_EXECUTE { get { return this.FileExecute; } set { this.FileExecute = value; } }
        public bool FILE_DELETE_CHILD { get { return this.FileDeleteChild; } set { this.FileDeleteChild = value; } }
        public bool FILE_READ_ATTRIBUTES { get { return this.FileReadAttribute; } set { this.FileReadAttribute = value; } }
        public bool FILE_WRITE_ATTRIBUTES { get { return this.FileWriteAttributes; } set { this.FileWriteAttributes= value; } }
        public bool FILE_LIST_DIRECTORY { get { return this.FileListDirectory; } set { this.FileListDirectory = value; } }
        public bool FILE_ADD_FILE { get { return this.FileAddFile; } set { this.FileAddFile = value; } }
        public bool FILE_ADD_SUBDIRECTORY { get { return this.FileAddSubdirectory; } set { this.FileAddSubdirectory = value; } }
        public bool FILE_TRAVERSE { get { return this.FileTransverse; } set { this.FileTransverse = value; } }

        #endregion

        #region Registry Permissions

        public bool KEY_QUERY_VALUE { get { return this.KeyQueryValue; } set { this.KeyQueryValue = value; } }
        public bool KEY_SET_VALUE { get { return this.KeySetValue; } set { this.KeySetValue = value; } }
        public bool KEY_CREATE_SUB_KEY { get { return this.KeyCreateSubKey; } set { this.KeyCreateSubKey = value; } }
        public bool KEY_ENUMERATE_SUB_KEYS { get { return this.KeyEnumerateSubKeys; } set { this.KeyEnumerateSubKeys = value; } }
        public bool KEY_NOTIFY { get { return this.KeyNotify; } set { this.KeyNotify = value; } }
        public bool KEY_CREATE_LINK { get { return this.KeyCreateLink; } set { this.KeyCreateLink = value; } }
        public bool KEY_WOW64_64KEY { get { return this.KeyWOW64_64Key; } set { this.KeyWOW64_64Key = value; } }
        public bool KEY_WOW64_32KEY { get { return this.KeyWOW64_32Key; } set { this.KeyWOW64_32Key = value; } }
        public bool KEY_WOW64_RES { get { return this.KeyWOW64_Res; } set { this.KeyWOW64_Res = value; } }

        #endregion

        public AuditEventStatus AuditEventPolicy
        {
            get
            {
                switch (this.AceFlags)
                {
                    case 0: return AuditEventStatus.AUDIT_NONE;
                    case 64: return AuditEventStatus.AUDIT_SUCCESS;
                    case 128: return AuditEventStatus.AUDIT_FAILURE;
                    case 192: return AuditEventStatus.AUDIT_SUCCESS_FAILURE;
                    default: return AuditEventStatus.EMPTY;
                }
            }
        }


        public override string ToString()
        {
            string retVal = String.Format("{0}:", this.Trustee);

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

            if (this.KEY_QUERY_VALUE) retVal += " KEY_QUERY_VALUE";
            if (this.KEY_SET_VALUE) retVal += " KEY_SET_VALUE";
            if (this.KEY_CREATE_SUB_KEY) retVal += " KEY_CREATE_SUB_KEY";
            if (this.KEY_ENUMERATE_SUB_KEYS) retVal += " KEY_ENUMERATE_SUB_KEYS";
            if (this.KEY_NOTIFY) retVal += " KEY_NOTIFY";
            if (this.KEY_CREATE_LINK) retVal += " KEY_CREATE_LINK";
            if (this.KEY_WOW64_64KEY) retVal += " KEY_WOW64_64KEY";
            if (this.KEY_WOW64_32KEY) retVal += " KEY_WOW64_32KEY";
            if (this.KEY_WOW64_RES) retVal += " KEY_WOW64_RES";

            return retVal;
        }


    }
}
