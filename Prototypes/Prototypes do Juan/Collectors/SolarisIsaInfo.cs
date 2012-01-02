using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class SolarisIsaInfo
    {
        public string Bits { get; set; }
        public string KernelIsa { get; set; }
        public string ApplicationIsa { get; set; }

        public override string ToString()
        {
            return String.Format("{0} bits, Kernel {1}, Application {2}", this.Bits, this.KernelIsa, this.ApplicationIsa);
        }
    }
}
