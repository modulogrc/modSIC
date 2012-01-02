using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    /// <summary>
    /// === Collector ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: Abstract base class of all collector communications technology class.
    /// How to Use: [TODO]
    /// Exceptions: [TODO]
    /// Hypoteses: [TODO]
    /// Example: [TODO]
    /// </summary>
    public abstract class Collector
    {
        /// <summary>
        /// Gets or sets error information for this instance.
        /// </summary>
        /// <value>Error message</value>
        public string ErrMsg { get; protected set; }

        /// <summary>
        /// Gets or sets error information for this instance.
        /// </summary>
        /// <value>Error type</value>
        public string ErrType { get; protected set; }

        /// <summary>
        /// Authentication data for the Collector instance. Depending on technology, not all
        /// the fields of the CollectorAuth object may be needed.
        /// </summary>
        public CollectorAuth Auth { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        public Collector()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class passing an authentication object.
        /// </summary>
        /// <param name="authinfo">Authentication info.</param>
        public Collector(CollectorAuth authinfo)
        {
            Auth = authinfo;
        }

        /// <summary>
        /// Conect to a host in order to perform data collections.
        /// </summary>
        /// <param name="hostname">Hostname or IP of machine to be probed</param>
        public abstract void Connect(string hostname);

        /// <summary>
        /// Performs a collection. This instance must have valid authentication data filled in.
        /// </summary>
        /// <param name="spec">Collection spec, i.e., which Registry key to probe, which 'expect' script to run etc.</param>
        /// <returns>Raw result of probe, i.e., output of the 'expect script, or value of the probed registry key etc.</returns>
        public abstract CollectResult Collect(ControlSpec spec);
    }
}
