using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Modulo.Collect.Probe.Windows.PasswordPolicy
{
    public sealed class PasswordPolicySamServer : IDisposable
    {
        private IntPtr _handle;

        public PasswordPolicySamServer(string name, SERVER_ACCESS_MASK access)
        {
            Name = name;
            Check(SamConnect(new UNICODE_STRING(name), out _handle, access, IntPtr.Zero));
        }

        public string Name { get; private set; }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                SamCloseHandle(_handle);
                _handle = IntPtr.Zero;
            }
        }

        public static DomainPasswordInformation? GetDomainPasswordInformation(string computerName)
        {
            using (PasswordPolicySamServer server = new PasswordPolicySamServer(computerName,
                SERVER_ACCESS_MASK.SAM_SERVER_ENUMERATE_DOMAINS |
                SERVER_ACCESS_MASK.SAM_SERVER_LOOKUP_DOMAIN))
            {
                foreach (string domain in server.EnumerateDomains())
                {
                    if (string.Compare(domain, "Builtin", true) == 0)
                        continue;

                    var sid = server.GetDomainSid(domain);
                    return server.GetDomainPasswordInformation(sid);
                }

                return null;
            }
        }

        public DomainPasswordInformation GetDomainPasswordInformation(SecurityIdentifier domainSid)
        {
            if (domainSid == null)
                throw new ArgumentNullException("domainSid");

            byte[] sid = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(sid, 0);

            IntPtr domain;
            Check(SamOpenDomain(_handle, DOMAIN_ACCESS_MASK.DOMAIN_READ_PASSWORD_PARAMETERS, sid, out domain));
            IntPtr info = IntPtr.Zero;
            try
            {
                Check(SamQueryInformationDomain(domain, DOMAIN_INFORMATION_CLASS.DomainPasswordInformation, out info));
                return (DomainPasswordInformation)Marshal.PtrToStructure(info, typeof(DomainPasswordInformation));
            }
            finally
            {
                SamFreeMemory(info);
                SamCloseHandle(domain);
            }
        }

        public SecurityIdentifier GetDomainSid(string domain)
        {
            if (domain == null)
                throw new ArgumentNullException("domain");

            IntPtr sid;
            Check(SamLookupDomainInSamServer(_handle, new UNICODE_STRING(domain), out sid));
            return new SecurityIdentifier(sid);
        }

        public IEnumerable<string> EnumerateDomains()
        {
            int cookie = 0;
            while (true)
            {
                IntPtr info;
                int count;
                var status = SamEnumerateDomainsInSamServer(_handle, ref cookie, out info, 1, out count);
                if (status != NTSTATUS.STATUS_SUCCESS && status != NTSTATUS.STATUS_MORE_ENTRIES)
                    Check(status);

                if (count == 0)
                    break;

                UNICODE_STRING us = (UNICODE_STRING)Marshal.PtrToStructure(info + 8, typeof(UNICODE_STRING));
                SamFreeMemory(info);
                yield return us.ToString();
            }
        }

        private enum DOMAIN_INFORMATION_CLASS
        {
            DomainPasswordInformation = 1,
        }

        [Flags]
        public enum PASSWORD_PROPERTIES
        {
            DOMAIN_PASSWORD_COMPLEX = 0x00000001,
            DOMAIN_PASSWORD_NO_ANON_CHANGE = 0x00000002,
            DOMAIN_PASSWORD_NO_CLEAR_CHANGE = 0x00000004,
            DOMAIN_LOCKOUT_ADMINS = 0x00000008,
            DOMAIN_PASSWORD_STORE_CLEARTEXT = 0x00000010,
            DOMAIN_REFUSE_PASSWORD_CHANGE = 0x00000020,
        }

        [Flags]
        private enum DOMAIN_ACCESS_MASK
        {
            DOMAIN_READ_PASSWORD_PARAMETERS = 0x00000001,
            DOMAIN_WRITE_PASSWORD_PARAMS = 0x00000002,
            DOMAIN_READ_OTHER_PARAMETERS = 0x00000004,
            DOMAIN_WRITE_OTHER_PARAMETERS = 0x00000008,
            DOMAIN_CREATE_USER = 0x00000010,
            DOMAIN_CREATE_GROUP = 0x00000020,
            DOMAIN_CREATE_ALIAS = 0x00000040,
            DOMAIN_GET_ALIAS_MEMBERSHIP = 0x00000080,
            DOMAIN_LIST_ACCOUNTS = 0x00000100,
            DOMAIN_LOOKUP = 0x00000200,
            DOMAIN_ADMINISTER_SERVER = 0x00000400,
            DOMAIN_ALL_ACCESS = 0x000F07FF,
            DOMAIN_READ = 0x00020084,
            DOMAIN_WRITE = 0x0002047A,
            DOMAIN_EXECUTE = 0x00020301
        }

        [Flags]
        public enum SERVER_ACCESS_MASK
        {
            SAM_SERVER_CONNECT = 0x00000001,
            SAM_SERVER_SHUTDOWN = 0x00000002,
            SAM_SERVER_INITIALIZE = 0x00000004,
            SAM_SERVER_CREATE_DOMAIN = 0x00000008,
            SAM_SERVER_ENUMERATE_DOMAINS = 0x00000010,
            SAM_SERVER_LOOKUP_DOMAIN = 0x00000020,
            SAM_SERVER_ALL_ACCESS = 0x000F003F,
            SAM_SERVER_READ = 0x00020010,
            SAM_SERVER_WRITE = 0x0002000E,
            SAM_SERVER_EXECUTE = 0x00020021
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DomainPasswordInformation
        {
            public short MinPasswordLength;
            public short PasswordHistoryLength;
            public PASSWORD_PROPERTIES PasswordProperties;
            private long _maxPasswordAge;
            private long _minPasswordAge;

            public TimeSpan MaxPasswordAge
            {
                get
                {
                    return -new TimeSpan(_maxPasswordAge);
                }
                set
                {
                    _maxPasswordAge = value.Ticks;
                }
            }

            public TimeSpan MinPasswordAge
            {
                get
                {
                    return -new TimeSpan(_minPasswordAge);
                }
                set
                {
                    _minPasswordAge = value.Ticks;
                }
            }

            public bool PasswordComplex
            {
                get { return Convert.ToBoolean(PasswordProperties & PASSWORD_PROPERTIES.DOMAIN_PASSWORD_COMPLEX); }
            }

            public bool PasswordReversibleEncryption
            {
                get { return Convert.ToBoolean(PasswordProperties & PASSWORD_PROPERTIES.DOMAIN_PASSWORD_STORE_CLEARTEXT); }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UNICODE_STRING : IDisposable
        {
            public ushort Length;
            public ushort MaximumLength;
            private IntPtr Buffer;

            public UNICODE_STRING(string s)
                : this()
            {
                if (s != null)
                {
                    Length = (ushort)(s.Length * 2);
                    MaximumLength = (ushort)(Length + 2);
                    Buffer = Marshal.StringToHGlobalUni(s);
                }
            }

            public void Dispose()
            {
                if (Buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(Buffer);
                    Buffer = IntPtr.Zero;
                }
            }

            public override string ToString()
            {
                return Buffer != IntPtr.Zero ? Marshal.PtrToStringUni(Buffer) : null;
            }
        }

        private static void Check(NTSTATUS err)
        {
            if (err == NTSTATUS.STATUS_SUCCESS)
                return;

            throw new Win32Exception("Error " + err + " (0x" + ((int)err).ToString("X8") + ")");
        }

        private enum NTSTATUS
        {
            STATUS_SUCCESS = 0x0,
            STATUS_MORE_ENTRIES = 0x105,
            STATUS_INVALID_HANDLE = unchecked((int)0xC0000008),
            STATUS_INVALID_PARAMETER = unchecked((int)0xC000000D),
            STATUS_ACCESS_DENIED = unchecked((int)0xC0000022),
            STATUS_OBJECT_TYPE_MISMATCH = unchecked((int)0xC0000024),
            STATUS_NO_SUCH_DOMAIN = unchecked((int)0xC00000DF),
        }

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamConnect(UNICODE_STRING ServerName, out IntPtr ServerHandle, SERVER_ACCESS_MASK DesiredAccess, IntPtr ObjectAttributes);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamCloseHandle(IntPtr ServerHandle);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamFreeMemory(IntPtr Handle);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamOpenDomain(IntPtr ServerHandle, DOMAIN_ACCESS_MASK DesiredAccess, byte[] DomainId, out IntPtr DomainHandle);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamLookupDomainInSamServer(IntPtr ServerHandle, UNICODE_STRING name, out IntPtr DomainId);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamQueryInformationDomain(IntPtr DomainHandle, DOMAIN_INFORMATION_CLASS DomainInformationClass, out IntPtr Buffer);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamSetInformationDomain(IntPtr DomainHandle, DOMAIN_INFORMATION_CLASS DomainInformationClass, IntPtr Buffer);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamEnumerateDomainsInSamServer(IntPtr ServerHandle, ref int EnumerationContext, out IntPtr EnumerationBuffer, int PreferedMaximumLength, out int CountReturned);
    }
}
