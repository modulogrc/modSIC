using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class XinetdInfo
    {
        public string ServiceName { get; set; }
        public string Protocol { get; set; }
        public string Flags { get; set; }
        public string NoAccess { get; set; }
        public string OnlyFrom { get; set; }
        public string Port { get; set; }
        public string Server { get; set; }
        public string ServerArgs { get; set; }
        public string SocketType { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
        public bool Wait { get; set; }
        public bool Disabled { get; set; }

        public XinetdInfo Clone()
        {
            XinetdInfo retItem = new XinetdInfo();
            retItem.ServiceName = this.ServiceName;
            retItem.Protocol = this.Protocol;
            retItem.Flags = this.Flags;
            retItem.NoAccess = this.NoAccess;
            retItem.OnlyFrom = this.OnlyFrom;
            retItem.Port = this.Port;
            retItem.Server = this.Server;
            retItem.ServerArgs = this.ServerArgs;
            retItem.SocketType = this.SocketType;
            retItem.Type = this.Type;
            retItem.User = this.User;
            retItem.Wait = this.Wait;
            retItem.Disabled = this.Disabled;
            return retItem;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1} port {2}): {3}", ServiceName, Protocol, Port, String.IsNullOrEmpty(Server) ? Type : Server);
        }
    }
}
