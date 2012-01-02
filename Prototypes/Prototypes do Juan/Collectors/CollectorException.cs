using System;
using System.Runtime.Serialization;

namespace FrameworkNG
{
    /// <summary>
    /// === CollectorException ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: Exceptions specific to the RiskManager NG Remote collection infrastructure.
    /// How to Use: throw new CollectorException(string message);
    /// Exceptions: N/A
    /// Hypoteses: To be used only within Risk Manager NG collection infrastructure
    /// Example: throw new CollectorException(String.Format("Invalid KB {0}", kbItem));
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class CollectorException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorException"/> class.
        /// </summary>
        /// <param name="msg">Message</param>
        public CollectorException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorException"/> class.
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="innerexcp">Inner exception</param>
        public CollectorException(string msg, Exception innerexcp)
            : base(msg, innerexcp)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorException"/> class.
        /// </summary>
        public CollectorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected CollectorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
