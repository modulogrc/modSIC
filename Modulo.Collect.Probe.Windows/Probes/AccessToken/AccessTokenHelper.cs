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
using System.Runtime.InteropServices;

namespace Modulo.Collect.Probe.Windows.AccessToken
{
    public class AccessTokenHelper
    {
        #region Consts
        public const int POLICY_VIEW_LOCAL_INFORMATION = 0x1;
        public const int POLICY_LOOKUP_NAMES = 0x00000800;
        public const int DELETE = 0x00010000;
        public const int READ_CONTROL = 0x00020000;
        public const int WRITE_DAC = 0x00040000;
        public const int WRITE_OWNER = 0x00080000;
        public const int SYNCHRONIZE = 0x00100000;
        public const int STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const int STANDARD_RIGHTS_READ = READ_CONTROL;
        public const int STANDARD_RIGHTS_WRITE = READ_CONTROL;
        public const int STANDARD_RIGHTS_EXECUTE = READ_CONTROL;
        public const int STANDARD_RIGHTS_ALL = 0x001F0000;
        public const int SPECIFIC_RIGHTS_ALL = 0x0000FFFF;
        #endregion

        #region LSA Imports
        [DllImport("kernel32.dll")]
        extern static int GetLastError();

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern UInt32 LsaNtStatusToWinError(UInt32 Status);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        public static extern bool ConvertStringSidToSid(string StringSid, out IntPtr pSid);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        public static extern bool LookupAccountName(
            string lpSystemName, string lpAccountName, IntPtr psid, ref int cbsid, 
            StringBuilder domainName, ref int cbdomainLength, ref int use);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern UInt32 LsaOpenPolicy(
            ref LSA_UNICODE_STRING SystemName, ref LSA_OBJECT_ATTRIBUTES ObjectAttributes, Int32 DesiredAccess, out IntPtr PolicyHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern UInt32 LsaClose(IntPtr PolicyHandle);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        public static extern UInt32 LsaEnumerateAccountRights(IntPtr PolicyHandle, IntPtr AccountSid, out IntPtr UserRights, out UInt32 CountOfRights);
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LSA_UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LSA_OBJECT_ATTRIBUTES
        {
            public IntPtr RootDirectory;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
            public LSA_UNICODE_STRING ObjectName;
            public UInt32 Attributes;
            public UInt32 Length;
        }
        #endregion

        public static LSA_UNICODE_STRING string2LSAUS(string myString)
        {
            LSA_UNICODE_STRING retStr = new LSA_UNICODE_STRING();
            retStr.Buffer = Marshal.StringToHGlobalUni(myString);
            retStr.Length = (UInt16)(myString.Length * UnicodeEncoding.CharSize);
            retStr.MaximumLength = (UInt16)((myString.Length + 1) * UnicodeEncoding.CharSize);
            return retStr;
        }

        public static string LSAUS2string(LSA_UNICODE_STRING lsaus)
        {
            char[] cvt = new char[lsaus.Length / UnicodeEncoding.CharSize];
            Marshal.Copy(lsaus.Buffer, cvt, 0, lsaus.Length / UnicodeEncoding.CharSize);
            return new string(cvt);
        }

        public static List<string> getAccessTokens(string targetHost, string entityName)
        {
            List<string> retList = new List<string>();

            IntPtr sid = IntPtr.Zero;
            int sidSize = 0;
            StringBuilder domainName = new StringBuilder();
            int nameSize = 0;
            int accountType = 0;
            UInt32 lretVal;
            uint retVal;

            LookupAccountName("\\\\" + targetHost, entityName, sid, ref sidSize,
                domainName, ref nameSize, ref accountType);
            domainName = new StringBuilder(nameSize);
            sid = Marshal.AllocHGlobal(sidSize);

            bool result = LookupAccountName("\\\\" + targetHost, entityName, sid, ref sidSize,
                domainName, ref nameSize, ref accountType);

            if (!result)
            {
                Marshal.FreeHGlobal(sid);
                throw new System.ComponentModel.Win32Exception(GetLastError());
            }

            LSA_UNICODE_STRING systemName = string2LSAUS("\\\\" + targetHost);
            IntPtr policyHandle = IntPtr.Zero;
            LSA_OBJECT_ATTRIBUTES objAttrs = new LSA_OBJECT_ATTRIBUTES();
            lretVal = LsaOpenPolicy(ref systemName, ref objAttrs,
                                POLICY_LOOKUP_NAMES | POLICY_VIEW_LOCAL_INFORMATION, out policyHandle);
            retVal = LsaNtStatusToWinError(lretVal);

            if (retVal != 0)
            {
                Marshal.FreeHGlobal(sid);
                throw new System.ComponentModel.Win32Exception((int)retVal);
            }

            IntPtr rightsArray = IntPtr.Zero;
            UInt32 rightsCount = 0;
            lretVal = LsaEnumerateAccountRights(policyHandle, sid, out rightsArray, out rightsCount);
            retVal = LsaNtStatusToWinError(lretVal);
            Marshal.FreeHGlobal(sid);

            if (retVal == 2)
            {
                LsaClose(policyHandle);
                return retList;
            }
            if (retVal != 0)
            {
                LsaClose(policyHandle);
                throw new System.ComponentModel.Win32Exception((int)retVal);
            }

            LSA_UNICODE_STRING myLsaus = new LSA_UNICODE_STRING();
            for (ulong i = 0; i < rightsCount; i++)
            {
                IntPtr itemAddr = new IntPtr(rightsArray.ToInt64() + (long)(i * (ulong)Marshal.SizeOf(myLsaus)));
                myLsaus = (LSA_UNICODE_STRING)Marshal.PtrToStructure(itemAddr, myLsaus.GetType());
                string thisRight = LSAUS2string(myLsaus);
                retList.Add(thisRight);
            }

            lretVal = LsaClose(policyHandle);
            retVal = LsaNtStatusToWinError(lretVal);
            if (retVal != 0)
                throw new System.ComponentModel.Win32Exception((int)retVal);

            return retList;
        }
    }
}
