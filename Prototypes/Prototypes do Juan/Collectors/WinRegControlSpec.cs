using System;

namespace FrameworkNG
{
    /// <summary>
    /// === WinRegControlSpec ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: Models a control that refers to Windows Registry content.
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: WinRegCollector.Collect() expects an instance of this.
    /// Example: N/A
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public abstract class WinRegControlSpec : ControlSpec
    {
        public WMI.Registry.baseKey Hive { get; protected set; }
        public string RegKey { get; protected set; }
        public string RegValue { get; protected set; }
        public WMI.Registry.valueType ValueType { get; protected set; }
    }
}
