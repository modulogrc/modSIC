using System;
using System.Management;

namespace FrameworkNG
{
    public class GoodBIOSControlSpec : WMIControlSpec
    {
        public GoodBIOSControlSpec()
        {
            WMINamespace = "root\\CIMV2";
            WMIQuery = "SELECT * FROM Win32_BIOS";
            ID = 2469171;
            Description = "Has a BIOS from a manufacturer that's not shady";
        }

        public override bool Decide(CollectResult collectres)
        {
            ManagementObjectCollection wmiRes = (ManagementObjectCollection)collectres.Data;
            foreach (ManagementObject queryObj in wmiRes)
            {
                string maker = queryObj["Name"].ToString().Split(' ')[0].ToLower();
                switch (maker)
                {
                    case "award":
                    case "ami":
                    case "phoenix":
                        continue;

                    default:
                        return false;
                }
            }
            return true;
        }
    }
}
