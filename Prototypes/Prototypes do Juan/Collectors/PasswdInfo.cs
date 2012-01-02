using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class PasswdInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Uid { get; set; }
        public string Gid { get; set; }
        public string Gecos { get; set; }
        public string HomeDir { get; set; }
        public string Shell { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1}): uid {2}, gid {3}", UserName, String.IsNullOrEmpty(Gecos) ? "no full name" : Gecos, Uid, Gid);
        }
    }
}
