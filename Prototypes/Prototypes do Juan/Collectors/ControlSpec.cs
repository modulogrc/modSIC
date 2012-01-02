using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    /// <summary>
    /// === ControlSpec ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: Models a control. This class should be refactored to conform to the OVAL standard.
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: Abstract class; concrete classes should be subclasses of technology specs, which should be subclasses of this one.
    /// Example: N/A
    /// </summary>
    public abstract class ControlSpec
    {
        public long ID { get; protected set; }
        public string Description { get; protected set; }

        /// <summary>
        /// Calculates the standard-compliant result of a probe based on the raw data returned by the communications classes.
        /// </summary>
        /// <param name="collectres">Raw result of a probe</param>
        /// <returns></returns>
        public abstract bool Decide(CollectResult collectres);
    }
}
