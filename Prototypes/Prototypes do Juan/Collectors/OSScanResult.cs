using System;
using System.Collections.Generic;

namespace FrameworkNG
{
    /// <summary>
    /// === OSScanResult ===
    /// Author: jcastro
    /// Creation Date: 28/05/2009
    /// Description: Encapsulates a set of OS guesses for a machine.
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: N/A
    /// Example: N/A
    /// </summary>
    public class OSScanResult
    {
        /// <summary>
        /// Gets or sets the best OS guess..
        /// </summary>
        /// <value>The best OS guess.</value>
        public OSGuess Best { get; set; }
        /// <summary>
        /// All possible OS's the mnachine may have, each with a percent accuracy.
        /// </summary>
        public List<OSGuess> Guesses;

        /// <summary>
        /// Initializes a new instance of the <see cref="OSScanResult"/> class.
        /// </summary>
        public OSScanResult()
        {
            Guesses = new List<OSGuess>();
            Best = null;
        }
    }
}
