using System;

namespace FrameworkNG
{
    /// <summary>
    /// === CollectResult ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: Encapsulates the raw return of a probe. Giving a final assessment based on this is the responsibility of ControlSpec.Decide().
    /// How to Use: Opaque class; is the return type of Collector.Collect(), and will be acted upon by ControlSpec.Decide().
    /// Exceptions: N/A
    /// Hypoteses: Not to be created or manipulated directly.
    /// Example: N/A
    /// </summary>
    public class CollectResult
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public object Data { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
    }
}
