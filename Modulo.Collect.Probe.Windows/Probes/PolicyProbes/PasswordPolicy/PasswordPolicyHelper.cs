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

namespace Modulo.Collect.Probe.Windows.PasswordPolicy
{
    class PasswordPolicyHelper
    {
        [DllImport("netapi32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern uint NetUserModalsGet(
            string server,
            int level,
            out IntPtr BufPtr
        );

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern uint NetApiBufferFree(
          IntPtr bufptr
        );

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_MODALS_INFO_0
        {
            public uint usrmod0_min_passwd_len;
            public uint usrmod0_max_passwd_age;
            public uint usrmod0_min_passwd_age;
            public uint usrmod0_force_logoff;
            public uint usrmod0_password_hist_len;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_MODALS_INFO_1
        {
            public uint usrmod1_role;
            public string usrmod1_primary;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_MODALS_INFO_2
        {
            public string usrmod2_domain_name;
            public IntPtr usrmod2_domain_id;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_MODALS_INFO_3
        {
            public uint usrmod3_lockout_duration;
            public uint usrmod3_lockout_observation_window;
            public uint usrmod3_lockout_threshold;
        };

        public struct UMInfo
        {
            public USER_MODALS_INFO_0 u0;
            public USER_MODALS_INFO_1 u1;
            public USER_MODALS_INFO_2 u2;
            public USER_MODALS_INFO_3 u3;
        }

        public static USER_MODALS_INFO_0 getUserModalsInfo0(string myHost)
        {
            uint retVal;
            IntPtr myBuf;
            USER_MODALS_INFO_0 retData;

            retVal = NetUserModalsGet(myHost, 0, out myBuf);
            if (retVal == 0)
                retData = (USER_MODALS_INFO_0)Marshal.PtrToStructure(myBuf, typeof(USER_MODALS_INFO_0));
            else
                throw new System.ComponentModel.Win32Exception((int)retVal);
            retVal = NetApiBufferFree(myBuf);
            return retData;
        }

        public static USER_MODALS_INFO_1 getUserModalsInfo1(string myHost)
        {
            uint retVal;
            IntPtr myBuf;
            USER_MODALS_INFO_1 retData;

            retVal = NetUserModalsGet(myHost, 1, out myBuf);
            if (retVal == 0)
                retData = (USER_MODALS_INFO_1)Marshal.PtrToStructure(myBuf, typeof(USER_MODALS_INFO_1));
            else
                throw new System.ComponentModel.Win32Exception((int)retVal);
            retVal = NetApiBufferFree(myBuf);
            return retData;
        }

        public static USER_MODALS_INFO_2 getUserModalsInfo2(string myHost)
        {
            uint retVal;
            IntPtr myBuf;
            USER_MODALS_INFO_2 retData;

            retVal = NetUserModalsGet(myHost, 2, out myBuf);
            if (retVal == 0)
                retData = (USER_MODALS_INFO_2)Marshal.PtrToStructure(myBuf, typeof(USER_MODALS_INFO_2));
            else
                throw new System.ComponentModel.Win32Exception((int)retVal);
            retVal = NetApiBufferFree(myBuf);
            return retData;
        }

        public static USER_MODALS_INFO_3 getUserModalsInfo3(string myHost)
        {
            uint retVal;
            IntPtr myBuf;
            USER_MODALS_INFO_3 retData;

            retVal = NetUserModalsGet(myHost, 3, out myBuf);
            if (retVal == 0)
                retData = (USER_MODALS_INFO_3)Marshal.PtrToStructure(myBuf, typeof(USER_MODALS_INFO_3));
            else
                throw new System.ComponentModel.Win32Exception((int)retVal);
            retVal = NetApiBufferFree(myBuf);
            return retData;
        }

        public static UMInfo getLoginPolicies(string myHost)
        {
            UMInfo retData = new UMInfo();

            retData.u0 = getUserModalsInfo0(myHost);
            retData.u1 = getUserModalsInfo1(myHost);
            retData.u2 = getUserModalsInfo2(myHost);
            retData.u3 = getUserModalsInfo3(myHost);

            return retData;
        }
    }
}
