using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class RunLevelInfo
    {
        public string Service { get; set; }
        public string RunLevel { get; set; }
        public bool Start { get; set; }
        public bool Kill { get; set; }

        public override string ToString()
        {
            return String.Format(
                "Service '{0}'; Runlevel {1}{2}{3}", Service, RunLevel, Start ? "; Start" : "", Kill ? "; Kill" : "");
        }
    }
}
