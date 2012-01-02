using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG.SshHelpers
{
    public class SolarisPatchInfo
    {
        public ulong Base { get; set; }
        public ulong Version { get; set; }

        public override string ToString()
        {
            return String.Format("{0}-{1:D2}", this.Base, this.Version);
        }
    }
}
