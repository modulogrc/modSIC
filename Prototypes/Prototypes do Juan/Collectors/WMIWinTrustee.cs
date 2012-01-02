using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class WMIWinTrustee
    {
        public string Domain;
        public string Name;
        public Byte[] SID;
        public UInt32 SidLength;
        public string SIDString;

        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.Domain))
                return this.Name;
            else
                return this.Domain + "\\" + this.Name;
        }
    }
}
