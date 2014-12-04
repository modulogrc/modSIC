/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Modulo.Collect.Probe.Windows.Family
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
        /// <valueToMatch>The accuracy. (0-100)</valueToMatch>
        public int Accuracy { get; set; }
        /// <summary>
        /// Gets or sets the vendor.
        /// </summary>
        /// <valueToMatch>The vendor. ("Microsoft", "Sun", "Cisco"...)</valueToMatch>
        public string Vendor { get; set; }
        /// <summary>
        /// Gets or sets the OS family.
        /// </summary>
        /// <valueToMatch>The OS family. ("Linux", "Windows", "Solaris"...)</valueToMatch>
        public string OSFamily { get; set; }
        /// <summary>
        /// Gets or sets the OS "generation" or version.
        /// </summary>
        /// <valueToMatch>The OS "generation" or version. ("2.6.x", "Vista", "XP"...)</valueToMatch>
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
                return String.Format("{0} {1} {2}", Vendor, OSFamily, OSGen, Accuracy);
            else
                return String.Format("{0} {1}", Vendor, OSFamily, Accuracy);
        }
    }
}
