using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class SolarisSvcPropInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return String.Format("{0} = [{1}] (type: {2})", this.Name, this.Value, this.Type);
        }
    }
}
