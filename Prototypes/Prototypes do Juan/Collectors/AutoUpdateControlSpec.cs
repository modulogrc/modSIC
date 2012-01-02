using System;

namespace FrameworkNG
{
    public class AutoUpdateControlSpec : WinRegControlSpec
    {
        public AutoUpdateControlSpec()
        {
            Hive = FrameworkNG.WMI.Registry.baseKey.HKEY_LOCAL_MACHINE;
            RegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update";
            RegValue = "AUOptions";
            ValueType = FrameworkNG.WMI.Registry.valueType.DWORD;
        }

        public override bool Decide(CollectResult collectres)
        {
            ulong myval;

            try
            {
                myval = UInt32.Parse((string)collectres.Data);
            }
            catch
            {
                return false;
            }

            return (myval == 4);
        }
    }
}
