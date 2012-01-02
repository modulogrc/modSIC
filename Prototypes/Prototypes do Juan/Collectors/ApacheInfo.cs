using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class ApacheInfo
    {
        public string Path { get; set; }
        public string BinaryName { get; set; }
        public string Version { get; set; }

        public string FullBinaryPath
        {
            get
            {
                if (this.Path.EndsWith("/"))
                    return this.Path + this.BinaryName;
                else
                    return this.Path + "/" + this.BinaryName;
            }
        }

        public override string ToString()
        {
            return this.FullBinaryPath + " Version: " + (String.IsNullOrEmpty(this.Version) ? "unknown" : this.Version);
        }
    }
}
