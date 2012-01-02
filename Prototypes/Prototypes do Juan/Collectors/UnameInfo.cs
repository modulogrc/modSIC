using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class UnameInfo
    {
        public string OSName { get; set; }
        public string NodeName { get; set; }
        public string OSRelease { get; set; }
        public string OSVersion { get; set; }
        public string MachineClass { get; set; }
        public string ProcessorType { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5}", OSName, NodeName, OSRelease, OSVersion, MachineClass, ProcessorType);
        }
    }
}
