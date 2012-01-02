using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class InetServerInfo
    {
        public string Protocol { get; set; }
        public string LocalAddress { get; set; }
        public uint LocalPort { get; set; }
        public string ForeignAddress { get; set; }
        public uint ForeignPort { get; set; }
        public string ProgramName { get; set; }
        public uint ProcessId { get; set; }
        public string User { get; set; }

        private void spliceAddr(string addrAndPort, out string addr, out uint port)
        {
            int whereColon = addrAndPort.IndexOf(':');
            string tmpPort;
            if (whereColon > 0)
            {
                addr = addrAndPort.Substring(0, whereColon);
                tmpPort = addrAndPort.Substring(whereColon + 1);
            }
            else if (whereColon == 0)
            {
                addr = "0.0.0.0";
                tmpPort = addrAndPort.Substring(whereColon + 1);
            }
            else
            {
                addr = addrAndPort;
                tmpPort = "*";
            }
            if (tmpPort == "*")
                port = 0;
            else
                port = uint.Parse(tmpPort);
        }

        public string LocalFullAddress
        {
            get
            {
                if (this.LocalPort > 0)
                    return String.Format("{0}:{1}", this.LocalAddress, this.LocalPort);
                else
                    return String.Format("{0}:*", this.LocalAddress);
            }
            set
            {
                string addr;
                uint port;
                spliceAddr(value, out addr, out port);
                this.LocalAddress = addr;
                this.LocalPort = port;
            }
        }

        public string ForeignFullAddress
        {
            get
            {
                if (this.ForeignPort > 0)
                    return String.Format("{0}:{1}", this.ForeignAddress, this.ForeignPort);
                else
                    return String.Format("{0}:*", this.ForeignAddress);
            }
            set
            {
                string addr;
                uint port;
                spliceAddr(value, out addr, out port);
                this.ForeignAddress = addr;
                this.ForeignPort = port;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}: Local {1}, Foreign {2}, PID {3}/{4}, User {5}",
                Protocol, LocalFullAddress, ForeignFullAddress, ProcessId, ProgramName, User);
        }
    }
}
