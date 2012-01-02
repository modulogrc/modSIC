using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class SysInfo
    {
        public string Hostname { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string Architecture { get; set; }
        public List<InterfaceState> Interfaces { get; set; }

        public override string ToString()
        {
            return Hostname + ": " + OS + "; Version " + OSVersion + "; " + Architecture;
        }
    }
}
