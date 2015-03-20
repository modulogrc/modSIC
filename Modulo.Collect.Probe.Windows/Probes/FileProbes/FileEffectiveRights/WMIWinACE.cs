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

namespace Modulo.Collect.Probe.Windows.File.Helpers
{
    public partial class WMIWinACE
    {
        #region Generic Permissions

        private bool GenericExecute;
        private bool GenericRead;
        private bool GenericWrite;
        private bool GenericAll;

        #endregion

        #region Standard Permissions

        private bool AccessSystemSecurity;
        private bool Delete;
        private bool ReadControl;
        private bool WriteDAC;
        private bool WriteOwner;
        private bool Syncronize;

        #endregion

        #region File Permissions

        private bool FileReadData;
        private bool FileWriteData; 
        private bool FileAppendData;
        private bool FileReadEA;
        private bool FileWriteEA; 
        private bool FileExecute;
        private bool FileDeleteChild;
        private bool FileReadAttribute;
        private bool FileWriteAttributes;
        private bool FileListDirectory;
        private bool FileAddFile;
        private bool FileAddSubdirectory;
        private bool FileTransverse;

        #endregion

        #region Registry Key Permissions

        private bool KeyQueryValue;
        private bool KeySetValue;
        private bool KeyCreateSubKey;
        private bool KeyEnumerateSubKeys;
        private bool KeyNotify;
        private bool KeyCreateLink;
        private bool KeyWOW64_64Key;
        private bool KeyWOW64_32Key;
        private bool KeyWOW64_Res;

        #endregion

        
        public void ReversePermissions()
        {
            this.AccessSystemSecurity = !this.AccessSystemSecurity;
            this.FileReadData = !this.FileReadData; ;
            this.FileWriteData = !this.FileWriteData;
            this.FileAppendData = !this.FileAppendData;
            this.FileReadEA = !this.FileReadEA;
            this.FileWriteEA = !this.FileWriteEA;
            this.FileExecute = !this.FileExecute;
            this.FileDeleteChild = !this.FileDeleteChild;
            this.FileReadAttribute = !this.FileReadAttribute;
            this.FileWriteAttributes = !this.FileWriteAttributes;
            this.GenericExecute = !this.GenericExecute;
            this.GenericRead = !this.GenericRead;
            this.GenericWrite = !this.GenericWrite;
            this.GenericAll = !this.GenericAll;
            this.Delete = !this.Delete;
            this.ReadControl = !this.ReadControl;
            this.WriteDAC = !this.WriteDAC;
            this.WriteOwner = !this.WriteOwner;
            this.Syncronize = !this.Syncronize;
            this.FileListDirectory = !this.FileListDirectory;
            this.FileAddFile = !this.FileAddFile;
            this.FileAddSubdirectory = !this.FileAddSubdirectory;
            this.FileTransverse = !this.FileTransverse;

            this.KeyQueryValue = !this.KeyQueryValue;
            this.KeySetValue = !this.KeySetValue;
            this.KeyCreateSubKey = !this.KeyCreateSubKey;
            this.KeyEnumerateSubKeys = !this.KeyEnumerateSubKeys;
            this.KeyNotify = !this.KeyNotify;
            this.KeyCreateLink = !this.KeyCreateLink;
            this.KeyWOW64_64Key = !this.KeyWOW64_64Key;
            this.KeyWOW64_32Key = !this.KeyWOW64_32Key;
        }

        public void CalculateFileAccessRightsFromAccessMask()
        {
            this.calculateGenericPermissions();
            this.calculateStandardPermissions();
            this.calculateFilePermissions();
        }

        public void CalculateRegistryKeyAccessRightsFromAccessMask()
        {
            this.calculateGenericPermissions();
            this.calculateStandardPermissions();
            this.calculateRegistryKeyAccessRightsFromAccessMask();
        }

        private void calculateFilePermissions()
        {
            this.FileReadData = ((this.AccessMask & 0x1) != 0);
            this.FileListDirectory = this.FileReadData;
            this.FileWriteData = ((this.AccessMask & 0x2) != 0);
            this.FileAddFile = this.FileWriteData;
            this.FileAppendData = ((this.AccessMask & 0x4) != 0);
            this.FileAddSubdirectory = this.FileAppendData;
            this.FileExecute = ((this.AccessMask & 0x20) != 0);
            this.FileTransverse = this.FileExecute;
            this.FileReadEA = ((this.AccessMask & 0x8) != 0);
            this.FileWriteEA = ((this.AccessMask & 0x10) != 0);
            this.FileDeleteChild = ((this.AccessMask & 0x40) != 0);
            this.FileReadAttribute = ((this.AccessMask & 0x80) != 0);
            this.FileWriteAttributes = ((this.AccessMask & 0x100) != 0);
        }

        private void calculateRegistryKeyAccessRightsFromAccessMask()
        {
            this.KeyQueryValue = ((this.AccessMask & 0x0001) != 0);
            this.KeySetValue = ((this.AccessMask & 0x0002) != 0);
            this.KeyCreateSubKey = ((this.AccessMask & 0x0004) != 0);
            this.KeyEnumerateSubKeys = ((this.AccessMask & 0x0008) != 0);
            this.KeyNotify = ((this.AccessMask & 0x0010) != 0);
            this.KeyCreateLink = ((this.AccessMask & 0x0020) != 0);
            this.KeyWOW64_64Key = ((this.AccessMask & 0x0100) != 0);
            this.KeyWOW64_32Key = ((this.AccessMask & 0x0200) != 0);
        }

        private void calculateGenericPermissions()
        {
            this.GenericExecute = ((this.AccessMask & 0x20000000) != 0);
            this.GenericRead = ((this.AccessMask & 0x8000000) != 0);
            this.GenericWrite = ((this.AccessMask & 0x40000000) != 0);
            this.GenericAll = ((this.AccessMask & 0x10000000) != 0);
        }

        private void calculateStandardPermissions()
        {
            this.AccessSystemSecurity = ((this.AccessMask & 0x1000000) != 0);
            this.Delete = ((this.AccessMask & 0x010000) != 0);
            this.ReadControl = ((this.AccessMask & 0x020000) != 0);
            this.WriteDAC = ((this.AccessMask & 0x040000) != 0);
            this.WriteOwner = ((this.AccessMask & 0x080000) != 0);
            this.Syncronize = ((this.AccessMask & 0x100000) != 0);
        }

        
    }
}

/* ===================
 * Generic Permissions
 * ===================
 * File Generic Execute = AccessMask & 0x1200A9 != 0
 * File Generic Read = AccessMask & 0x120089) != 0
 * File Generic Write = AccessMask & 0x100116) != 0
 * File All Access = AccessMask & 0x1F01FF) != 0
 */
