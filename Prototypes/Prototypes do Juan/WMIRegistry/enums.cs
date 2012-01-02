using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkNG.WMI.Registry
{
    public enum baseKey : uint
    {
        HKEY_CLASSES_ROOT = 0x80000000,
        HKEY_CURRENT_USER = 0x80000001,
        HKEY_LOCAL_MACHINE = 0x80000002,
        HKEY_USERS = 0x80000003,
        HKEY_CURRENT_CONFIG = 0x80000005
    }
    public enum valueType
    {
        STRING = 1,
        EXPANDED_STRING = 2,
        BINARY = 3,
        DWORD = 4,
        MULTI_STRING = 7
    }
}
