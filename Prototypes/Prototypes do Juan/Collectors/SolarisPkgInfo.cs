using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class SolarisPkgInfo
    {
        public string PkgInst { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Version { get; set; }
        public string Vendor { get; set; }
        public string Description { get; set; }

        public string NameOrDesc
        {
            get
            {
                if (String.IsNullOrEmpty(this.Name))
                    return this.Description;
                else
                    return this.Name;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}{2}{3}{4}", this.PkgInst, this.Version,
                String.IsNullOrEmpty(this.NameOrDesc) ? "" : " (" + this.NameOrDesc + ")",
                String.IsNullOrEmpty(this.Category) ? "" : "; Category " + this.Category,
                String.IsNullOrEmpty(this.Vendor) ? "" : "; By " + this.Vendor
                );
        }
    }
}
