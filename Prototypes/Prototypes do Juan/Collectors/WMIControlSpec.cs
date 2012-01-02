using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    /// <summary>
    /// === WMIControlSpec ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: Models a control that refers to data obtainable through a WMI query.
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: WMIRegCollector.Collect() expects an instance of this.
    /// Example: N/A
    /// </summary>
    public abstract class WMIControlSpec : ControlSpec
    {
        public string WMINamespace { get; protected set; }
        public string WMIQuery { get; protected set; }
    }
}
