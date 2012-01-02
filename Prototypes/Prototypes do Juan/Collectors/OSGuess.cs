using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FrameworkNG
{
    /// <summary>
    /// === OSGuess ===
    /// Author: jcastro
    /// Creation Date: 28/05/2009
    /// Description: Represent one guess about a machine's OS, and the accuracy of such guess in percentage points.
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: N/A
    /// Example: N/A
    /// </summary>
    public class OSGuess
    {
        /// <summary>
        /// Gets or sets the accuracy.
        /// </summary>
        /// <value>The accuracy. (0-100)</value>
        public int Accuracy { get; set; }
        /// <summary>
        /// Gets or sets the vendor.
        /// </summary>
        /// <value>The vendor. ("Microsoft", "Sun", "Cisco"...)</value>
        public string Vendor { get; set; }
        /// <summary>
        /// Gets or sets the OS family.
        /// </summary>
        /// <value>The OS family. ("Linux", "Windows", "Solaris"...)</value>
        public string OSFamily { get; set; }
        /// <summary>
        /// Gets or sets the OS "generation" or version.
        /// </summary>
        /// <value>The OS "generation" or version. ("2.6.x", "Vista", "XP"...)</value>
        public string OSGen { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// Ex.: "Microsoft Windows XP (Accuracy: 100%)"
        /// </returns>
        public override string ToString()
        {
            if (OSGen != null)
                return String.Format("{0} {1} {2} (Accuracy: {3}%)", Vendor, OSFamily, OSGen, Accuracy);
            else
                return String.Format("{0} {1} (Accuracy: {2}%)", Vendor, OSFamily, Accuracy);
        }
    }
}
