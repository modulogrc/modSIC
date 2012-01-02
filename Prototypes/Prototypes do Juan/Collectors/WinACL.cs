using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FrameworkNG
{
    public static class WinACL
    {
        public const Byte ACL_REVISION = 2;

        public const Byte ACCESS_ALLOWED_ACE_TYPE = 0;
        public const Byte ACCESS_DENIED_ACE_TYPE  = 1;
        public const Byte SYSTEM_AUDIT_ACE_TYPE   = 2;
        public const Byte SYSTEM_ALARM_ACE_TYPE   = 3;

        public const Byte OBJECT_INHERIT_ACE         = 0x01;
        public const Byte CONTAINER_INHERIT_ACE      = 0x02;
        public const Byte NO_PROPAGATE_INHERIT_ACE   = 0x04;
        public const Byte INHERIT_ONLY_ACE           = 0x08;
        public const Byte INHERITED_ACE              = 0x10;
        public const Byte SUCCESSFUL_ACCESS_ACE_FLAG = 0x40;
        public const Byte FAILED_ACCESS_ACE_FLAG     = 0x80;

        public enum TRUSTEE_TYPE
        {
            TRUSTEE_IS_UNKNOWN,
            TRUSTEE_IS_USER,
            TRUSTEE_IS_GROUP,
            TRUSTEE_IS_DOMAIN,
            TRUSTEE_IS_ALIAS,
            TRUSTEE_IS_WELL_KNOWN_GROUP,
            TRUSTEE_IS_DELETED,
            TRUSTEE_IS_INVALID,
            TRUSTEE_IS_COMPUTER
        } 

        public enum TRUSTEE_FORM
        {
            TRUSTEE_IS_SID,
            TRUSTEE_IS_NAME,
            TRUSTEE_BAD_FORM,
            TRUSTEE_IS_OBJECTS_AND_SID,
            TRUSTEE_IS_OBJECTS_AND_NAME
        }

        public enum MULTIPLE_TRUSTEE_OPERATION
        {
            NO_MULTIPLE_TRUSTEE,
            TRUSTEE_IS_IMPERSONATE
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TRUSTEE
        {
            public IntPtr pMultipleTrustee;
            public MULTIPLE_TRUSTEE_OPERATION MultipleTrusteeOperation;
            public TRUSTEE_FORM TrusteeForm;
            public TRUSTEE_TYPE TrusteeType;
            public IntPtr ptstrName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ACL
        {
            public Byte AclRevision;
            public Byte Sbz1;
            public UInt16 AclSize;
            public UInt16 AceCount;
            public UInt16 Sbz2;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ACE_HEADER
        {
            public Byte AceType;
            public Byte AceFlags;
            public UInt16 AceSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SYSTEM_AUDIT_ACE
        {
            public ACE_HEADER Header;
            public UInt32 Mask;
            public UInt32 SidStart;
        }

        [DllImport("kernel32.dll")]
        extern static int GetLastError();

#if false
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        public static extern IntPtr GlobalAlloc(
            UInt16 uFlags,
            UInt32 dwBytes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        public static extern IntPtr GlobalFree(
            IntPtr hMem);
#endif

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern UInt32 GetAuditedPermissionsFromAcl(
            IntPtr pAcl,
            ref TRUSTEE pTrustee,
            out UInt32 pSuccessfulAuditedRights,
            out UInt32 pFailedAuditedRights);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        public static extern bool InitializeAcl(
            IntPtr pAcl,
            UInt32 nAclLength,
            UInt32 dwAclRevision);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
        public static extern bool AddAuditAccessAceEx(
            IntPtr pAcl,
            UInt32 dwAclRevision,
            UInt32 AceFlags,
            UInt32 dwAccessMask,
            IntPtr pSid,
            Boolean bAuditSuccess,
            Boolean bAuditFailure);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool ConvertStringSidToSid(
            String StringSid,
            out IntPtr ptrSid);

        public static IntPtr MakeAuditACL(int nAces)
        {
            IntPtr retPtr = IntPtr.Zero;
            ACL myAcl = new ACL();
            SYSTEM_AUDIT_ACE myAce = new SYSTEM_AUDIT_ACE();
            UInt32 dwFiller = 0;

            long szAcl = Marshal.SizeOf(myAcl) + (100 /* Gariba */ + Marshal.SizeOf(myAce) - Marshal.SizeOf(dwFiller)) * nAces;
            szAcl = (szAcl + Marshal.SizeOf(dwFiller) - 1) & 0xFFFFFFFC;
            retPtr = Marshal.AllocHGlobal((int)szAcl);
            if (retPtr == IntPtr.Zero)
            {
                int causingError = GetLastError();
                throw new System.ComponentModel.Win32Exception(causingError);
            }

            if (!InitializeAcl(retPtr, (UInt32)szAcl, ACL_REVISION))
            {
                int causingError = GetLastError();
                Marshal.FreeHGlobal(retPtr);
                throw new System.ComponentModel.Win32Exception(causingError);
            }

            return retPtr;
        }

        public static void AddACEToAuditACL(IntPtr pacl, UInt32 aceFlags, UInt32 accessMask, String sid)
        {
            IntPtr pSid;

            if (!ConvertStringSidToSid(sid, out pSid))
            {
                int causingError = GetLastError();
                throw new System.ComponentModel.Win32Exception(causingError);
            }

            if (!AddAuditAccessAceEx(pacl, ACL_REVISION, aceFlags, accessMask, pSid, (aceFlags & SUCCESSFUL_ACCESS_ACE_FLAG) != 0, (aceFlags & FAILED_ACCESS_ACE_FLAG) != 0))
            {
                int causingError = GetLastError();
                Marshal.FreeHGlobal(pSid);
                throw new System.ComponentModel.Win32Exception(causingError);
            }

            Marshal.FreeHGlobal(pSid);
        }

        public static void GetAuditedPermissions(IntPtr pAcl, String sid, out UInt32 successAudit, out UInt32 failAudit)
        {
            IntPtr pSid;
            TRUSTEE myTrustee = new TRUSTEE();

            if (!ConvertStringSidToSid(sid, out pSid))
            {
                int causingError = GetLastError();
                throw new System.ComponentModel.Win32Exception(causingError);
            }

            myTrustee.pMultipleTrustee = IntPtr.Zero;
            myTrustee.MultipleTrusteeOperation = MULTIPLE_TRUSTEE_OPERATION.NO_MULTIPLE_TRUSTEE;
            myTrustee.TrusteeForm = TRUSTEE_FORM.TRUSTEE_IS_SID;
            myTrustee.TrusteeType = TRUSTEE_TYPE.TRUSTEE_IS_UNKNOWN;
            myTrustee.ptstrName = pSid;

            UInt32 winError = GetAuditedPermissionsFromAcl(pAcl, ref myTrustee, out successAudit, out failAudit);

            if (winError != 0)
            {
                Marshal.FreeHGlobal(pSid);
                throw new System.ComponentModel.Win32Exception((int)winError);
            }
        }
    }
}
