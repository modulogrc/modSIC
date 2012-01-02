using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class ShadowInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ChangeLast { get; set; }
        public string ChangeAllow { get; set; }
        public string ChangeRequire { get; set; }
        public string ExpireWarning { get; set; }
        public string ExpireInactivity { get; set; }
        public string ExpireDate { get; set; }
        public string FlagReserved { get; set; }

        public override string ToString()
        {
            return String.Format("{0}{1}", UserName, ((!String.IsNullOrEmpty(Password)) && (Password.Length > 2)) ? " (has password)" : "");
        }
    }
}
