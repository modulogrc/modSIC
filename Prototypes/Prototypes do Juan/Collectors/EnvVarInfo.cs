using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class EnvVarInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return String.Format("{0}=\"{1}\"", this.Name, this.Value);
        }
    }
}
