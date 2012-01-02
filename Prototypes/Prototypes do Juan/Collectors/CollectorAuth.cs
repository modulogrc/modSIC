using System;

namespace FrameworkNG
{
    /// <summary>
    /// === CollectorAuth ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: This class encapsulates authentication data in a hopefully architecture-independent fashion.
    ///              Depending on the technology, some fields may not be used, ex. 'Domain' for a telnet-based probe on a UNIX host.
    /// How to Use: new XXXColector(new CollectorAuth(username, password [, domain [, style]]);
    /// Exceptions: N/A
    /// Hypoteses: N/A
    /// Example: grabber = new WMICollector(new CollectorAuth('Administrator', myPass.Password, 'MSS'));
    /// </summary>
    public class CollectorAuth
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain { get; set; }
        /// <summary>
        /// Gets or sets the auth style.
        /// </summary>
        /// <value>The auth style.</value>
        public string AuthStyle { get; set; }

        /// <summary>
        /// CollectorAuth constructor receiving only username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public CollectorAuth(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// CollectorAuth constructor receiving username, password, and domain.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="domain">Domain</param>
        public CollectorAuth(string username, string password, string domain)
        {
            Username = username;
            Password = password;
            Domain = domain;
        }

        /// <summary>
        /// CollectorAuth constructor receiving username, password, and domain.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="domain">Domain</param>
        /// <param name="authstyle">Auth style</param>
        public CollectorAuth(string username, string password, string domain, string authstyle)
        {
            Username = username;
            Password = password;
            Domain = domain;
            AuthStyle = authstyle;
        }
    }
}
