using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class InterfaceState
    {
        public enum stateAddrType : uint
        {
            MIB_IPADDR_DELETED,
            MIB_IPADDR_DISCONNECTED,
            MIB_IPADDR_DYNAMIC,
            MIB_IPADDR_PRIMARY,
            MIB_IPADDR_TRANSIENT
        }

        public enum stateInterfaceType : uint
        {
            MIB_IF_TYPE_ETHERNET = 0,
            MIB_IF_TYPE_TOKENRING = 1,
            MIB_IF_TYPE_FDDI = 2,
            MIB_IF_TYPE_LOOPBACK = 1000,
            MIB_IF_TYPE_OTHER = 1001,
            MIB_IF_TYPE_PPP,
            MIB_IF_TYPE_SLIP
        }

        public struct IPInfo
        {
            public string IPAddr;
            public string IPMask;
            public string IPBcast;
            public stateAddrType AddrType;
        }

        public string Name { get; set; }
        public uint Index { get; set; }
        public uint InterfaceIndex { get; set; }
        public stateInterfaceType Type { get; set; }
        public string HWAddr { get; set; }
        public List<IPInfo> InetAddr { get; set; }
        public bool IsPhysical { get; set; }

        public override string ToString()
        {
            if ((HWAddr != null) && (HWAddr != ""))
                return String.Format("{0}: {1} ({2}), HWAddr {3}, {4}", Index, Name, Type, HWAddr, IsPhysical ? "Physical" : "Virtual");
            else
                return String.Format("{0}: {1} ({2}), {3}", Index, Name, Type, IsPhysical ? "Physical" : "Virtual");
        }
    }
}
