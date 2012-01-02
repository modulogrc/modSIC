using System;
using System.Collections.Generic;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;
using MinimalisticTelnet;

namespace FrameworkNG.SshHelpers
{
    public class SysInfoColllector
    {
        private static string tokenAfter(string[] pfields, string keyword)
        {
            for (int i = 0; i < pfields.GetUpperBound(0); i++)
            {
                if (pfields[i] == keyword)
                    return pfields[i + 1];
            }
            return "";
        }

        private static void completeIP(ref InterfaceState.IPInfo curip)
        {
            if (String.IsNullOrEmpty(curip.IPAddr))
                return;

            if (String.IsNullOrEmpty(curip.IPMask) || String.IsNullOrEmpty(curip.IPBcast))
            {
                char[] onlydot = { '.' };
                string[] comps = curip.IPAddr.Split(onlydot);
                if (comps.GetUpperBound(0) == 3)
                {
                    if (String.IsNullOrEmpty(curip.IPMask))
                    {
                        uint firstTerm = uint.Parse(comps[0]);
                        if (firstTerm < 128)
                            curip.IPMask = "255.0.0.0";
                        else if (firstTerm < 192)
                            curip.IPMask = "255.255.0.0";
                        else
                            curip.IPMask = "255.255.255.0";
                    }
                    if (String.IsNullOrEmpty(curip.IPBcast))
                    {
                        string[] compsMask = curip.IPMask.Split(onlydot);
                        if (compsMask.GetUpperBound(0) == 3)
                        {
                            uint bcast0 = uint.Parse(comps[0]) | (0x00FF & ~uint.Parse(compsMask[0]));
                            uint bcast1 = uint.Parse(comps[1]) | (0x00FF & ~uint.Parse(compsMask[1]));
                            uint bcast2 = uint.Parse(comps[2]) | (0x00FF & ~uint.Parse(compsMask[2]));
                            uint bcast3 = uint.Parse(comps[3]) | (0x00FF & ~uint.Parse(compsMask[3]));
                            curip.IPBcast = String.Format("{0}.{1}.{2}.{3}", bcast0, bcast1, bcast2, bcast3);
                        }
                    }
                }
            }
        }

        private static List<string> getAngleFlags(string srcLine)
        {
            List<string> retList = new List<string>();
            int startPos, endPos;

            startPos = srcLine.IndexOf('<');
            if (startPos >= 0)
            {
                endPos = srcLine.IndexOf('>');
                if (endPos > startPos)
                {
                    char[] onlycomma = { ',' };
                    retList.AddRange(srcLine.Substring(startPos + 1, endPos - startPos - 1).Split(onlycomma, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            return retList;
        }

        private static void getInterfacesLinux(SshExec exec, SysInfo mySysInfo)
        {
            uint ifIndex = 0;
            InterfaceState curif = null;
            char[] lineseps = { '\r', '\n' };
            char[] fieldseps = { ' ', '\t' };
            string output = exec.RunCommand("/sbin/ip addr show");
            string[] lines = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if ((line[0] >= '0') && (line[0] <= '9'))
                {
                    if (curif != null)
                        mySysInfo.Interfaces.Add(curif);
                    curif = new InterfaceState();
                    curif.InetAddr = new List<InterfaceState.IPInfo>();
                    curif.Index = ifIndex++;
                    string[] ifields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                    curif.Name = ifields[1].Replace(":", "");
                }
                else
                {
                    string[] pfields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                    if (pfields[0].StartsWith("link/"))
                    {
                        string itype = pfields[0].Remove(0, 5);
                        switch (itype)
                        {
                            case "ether":
                                curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_ETHERNET;
                                curif.IsPhysical = true;
                                break;
                            case "loopback":
                                curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_LOOPBACK;
                                curif.IsPhysical = false;
                                break;
                            case "ppp":
                                curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_PPP;
                                curif.IsPhysical = false;
                                break;
                            case "fddi":
                                curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_FDDI;
                                curif.IsPhysical = true;
                                break;
                            case "tr":
                                curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_TOKENRING;
                                curif.IsPhysical = true;
                                break;
                            case "slip":
                            case "cslip":
                            case "slip6":
                            case "cslip6":
                                curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_SLIP;
                                curif.IsPhysical = false;
                                break;
                            default:
                                curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_OTHER;
                                break;
                        }
                        curif.HWAddr = pfields[1];
                    }
                    else if (pfields[0] == "inet")
                    {
                        char[] sepslash = { '/' };
                        string[] addrAndMask = pfields[1].Split(sepslash);
                        InterfaceState.IPInfo curip;
                        curip.IPAddr = addrAndMask[0];
                        if (addrAndMask[1].Contains("."))
                            curip.IPMask = addrAndMask[1];
                        else
                        {
                            uint hostbits = uint.Parse(addrAndMask[1]);
                            if (hostbits <= 8)
                                curip.IPMask = String.Format("{0}.0.0.0", (0xFF00 >> (int)hostbits) & 0x00FF);
                            else if (hostbits <= 16)
                                curip.IPMask = String.Format("255.{0}.0.0", (0xFF00 >> (int)(hostbits - 8)) & 0x00FF);
                            else if (hostbits <= 24)
                                curip.IPMask = String.Format("255.255.{0}.0", (0xFF00 >> (int)(hostbits - 16)) & 0x00FF);
                            else if (hostbits <= 32)
                                curip.IPMask = String.Format("255.255.{0}.0", (0xFF00 >> (int)(hostbits - 24)) & 0x00FF);
                            else
                                throw new InvalidOperationException(String.Format("Netmask bits in '{0}' ({1}) outside acceptable range", curif.Name, hostbits));
                        }
                        curip.IPBcast = tokenAfter(pfields, "brd");
                        curip.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_PRIMARY;
                        curif.InetAddr.Add(curip);
                    }
                }
            }
            if (curif != null)
                mySysInfo.Interfaces.Add(curif);
        }

        private static void getInterfacesMacOSX(SshExec exec, SysInfo mySysInfo)
        {
            throw new NotImplementedException("Obtaining interfaces for Mac OS not yet implemented");
        }

        private static void getInterfacesBSD(SshExec exec, SysInfo mySysInfo)
        {
            getInterfacesAIX(exec, mySysInfo);
        }

        private static void getMACSolaris(SshExec exec, InterfaceState curif)
        {
            char[] lineseps = { '\r', '\n' };
            char[] fieldseps = { ' ', '\t' };
            string output = exec.RunCommand("/usr/sbin/arp -n -a");
            string[] lines = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string thisMAC = "";
                string[] pfields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                if (pfields[0] == curif.Name)
                {
                    foreach (InterfaceState.IPInfo thisIP in curif.InetAddr)
                    {
                        if (pfields[1] == thisIP.IPAddr)
                        {
                            if (pfields.GetUpperBound(0) == 3)
                                thisMAC = pfields[3];
                            else if (pfields.GetUpperBound(0) >= 4)
                                thisMAC = pfields[4];

                            if (thisMAC.IndexOf(':') >= 0)
                            {
                                curif.HWAddr = thisMAC;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void getInterfacesSolaris(SshExec exec, SysInfo mySysInfo)
        {
            uint ifIndex = 0;
            bool repeated = false;
            InterfaceState curif = null;
            char[] lineseps = { '\r', '\n' };
            char[] fieldseps = { ' ', '\t' };
            string output = exec.RunCommand("/sbin/ifconfig -a");
            string[] lines = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);

            InterfaceState.IPInfo curip = new InterfaceState.IPInfo();
            foreach (string line in lines)
            {
                string varVal;

                if (!char.IsWhiteSpace(line[0]))
                {
                    if (curif != null)
                    {
                        if (!String.IsNullOrEmpty(curip.IPAddr))
                        {
                            completeIP(ref curip);
                            curif.InetAddr.Add(curip);
                            if (!repeated)
                            {
                                getMACSolaris(exec, curif);
                                mySysInfo.Interfaces.Add(curif);
                            }
                        }
                    }

                    curif = new InterfaceState();
                    curif.InetAddr = new List<InterfaceState.IPInfo>();
                    curip = new InterfaceState.IPInfo();
                    string[] ifields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);

                    curif.Name = ifields[0];
                    if (curif.Name.Substring(curif.Name.Length - 1) == ":")
                        curif.Name = curif.Name.Substring(0, curif.Name.Length - 1);
                    int hasColon = curif.Name.IndexOf(':');
                    if (hasColon >= 0)
                        curif.Name = curif.Name.Substring(0, hasColon);

                    // Repeated interface?
                    repeated = false;
                    foreach (InterfaceState alreadyThere in mySysInfo.Interfaces)
                    {
                        if (alreadyThere.Name == curif.Name)
                        {
                            curif = alreadyThere;
                            repeated = true;
                            break;
                        }
                    }

                    varVal = tokenAfter(ifields, "index");
                    if (varVal != "")
                    {
                        curif.Index = uint.Parse(varVal);
                        if (ifIndex <= curif.Index)
                            ifIndex = curif.Index + 1;
                    }
                    else
                        curif.Index = ifIndex++;

                    List<string> iflags = getAngleFlags(ifields[1]);

                    if (iflags.Contains("VIRTUAL"))
                    {
                        curif.IsPhysical = false;
                    }
                    else
                    {
                        curif.IsPhysical = true;
                    }

                    if (iflags.Contains("DHCP") || iflags.Contains("BOOTP"))
                    {
                        curip.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_DYNAMIC;
                    }
                    else
                    {
                        curip.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_PRIMARY;
                    }

                    if (iflags.Contains("LOOPBACK"))
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_LOOPBACK;
                        curif.IsPhysical = false;
                    }
                    else
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_OTHER;
                    }
                }
                else
                {
                    string[] pfields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);

                    varVal = tokenAfter(pfields, "inet");
                    if (varVal != "")
                        curip.IPAddr = varVal;
                    varVal = tokenAfter(pfields, "broadcast");
                    if (varVal != "")
                        curip.IPBcast = varVal;
                    varVal = tokenAfter(pfields, "netmask");
                    if (varVal != "")
                    {
                        if (varVal.Contains("."))
                            curip.IPMask = varVal;
                        else
                        {
                            UInt32 masknum;
                            if (varVal.StartsWith("0x"))
                                masknum = Convert.ToUInt32(varVal.Substring(2), 16);
                            else
                                masknum = Convert.ToUInt32(varVal, 16);
                            curip.IPMask = String.Format("{0}.{1}.{2}.{3}",
                                masknum >> 24, (masknum >> 16) & 0x00FF, (masknum >> 8) & 0x00FF, masknum & 0x00FF);
                        }
                    }

                    // "ether" não aparece se não for root. Se não preencher MAC, usar "arp -n -a".
                    varVal = tokenAfter(pfields, "ether");
                    if (varVal != "")
                        curif.HWAddr = varVal;
                }
            }
            if (curif != null)
            {
                if (!String.IsNullOrEmpty(curip.IPAddr))
                {
                    completeIP(ref curip);
                    curif.InetAddr.Add(curip);
                    mySysInfo.Interfaces.Add(curif);
                }
            }
        }

        private static void getMACAIX(SshExec exec, InterfaceState curif)
        {
            char[] lineseps = { '\r', '\n' };
            char[] fieldseps = { ' ', '\t' };
            char[] onlydot = { '.' };
            string output = exec.RunCommand("netstat -I " + curif.Name);
            string[] lines = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] pfields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                if (pfields.GetUpperBound(0) >= 3)
                {
                    string maybeMAC = pfields[3];
                    string[] bytesMAC = maybeMAC.Split(onlydot, StringSplitOptions.None);
                    if (bytesMAC.GetUpperBound(0) == 5)
                    {
                        try
                        {
                            curif.HWAddr = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                Convert.ToUInt32(bytesMAC[0], 16).ToString("X2"),
                                Convert.ToUInt32(bytesMAC[1], 16).ToString("X2"),
                                Convert.ToUInt32(bytesMAC[2], 16).ToString("X2"),
                                Convert.ToUInt32(bytesMAC[3], 16).ToString("X2"),
                                Convert.ToUInt32(bytesMAC[4], 16).ToString("X2"),
                                Convert.ToUInt32(bytesMAC[5], 16).ToString("X2"));
                            return;
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private static void getInterfacesAIX(SshExec exec, SysInfo mySysInfo)
        {
            /*
            em0: flags=8843<UP,BROADCAST,RUNNING,SIMPLEX,MULTICAST> metric 0 mtu 1500
                    options=9b<RXCSUM,TXCSUM,VLAN_MTU,VLAN_HWTAGGING,VLAN_HWCSUM>
                    ether 08:00:27:fc:20:71
                    inet 172.16.3.206 netmask 0xfffffe00 broadcast 172.16.3.255
                    media: Ethernet autoselect (1000baseT <full-duplex>)
                    status: active
            lo0: flags=8049<UP,LOOPBACK,RUNNING,MULTICAST> metric 0 mtu 16384
                    options=3<RXCSUM,TXCSUM>
                    inet6 fe80::1%lo0 prefixlen 64 scopeid 0x2
                    inet6 ::1 prefixlen 128
                    inet 127.0.0.1 netmask 0xff000000
                    nd6 options=3<PERFORMNUD,ACCEPT_RTADV>
             */
            uint ifIndex = 0;
            InterfaceState curif = null;
            char[] lineseps = { '\r', '\n' };
            char[] fieldseps = { ' ', '\t' };
            string output = exec.RunCommand("ifconfig -a");
            string[] lines = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (!char.IsWhiteSpace(line[0]))
                {
                    if (curif != null)
                    {
                        if (String.IsNullOrEmpty(curif.HWAddr))
                            getMACAIX(exec, curif);
                        mySysInfo.Interfaces.Add(curif);
                    }
                    curif = new InterfaceState();
                    curif.InetAddr = new List<InterfaceState.IPInfo>();
                    curif.Index = ifIndex++;
                    string[] ifields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                    curif.Name = ifields[0];
                    if (curif.Name.Substring(curif.Name.Length - 1) == ":")
                        curif.Name = curif.Name.Substring(0, curif.Name.Length - 1);
                    List<string> iflags = getAngleFlags(ifields[1]);
                    if (iflags.Contains("LOOPBACK"))
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_LOOPBACK;
                        curif.IsPhysical = false;
                    }
                    else if (curif.Name.StartsWith("en") || curif.Name.StartsWith("et"))
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_ETHERNET;
                        curif.IsPhysical = true;
                    }
                    else
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_OTHER;
                    }
                }
                else
                {
                    string[] pfields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                    string varVal;

                    varVal = tokenAfter(pfields, "inet");
                    if (varVal != "")
                    {
                        InterfaceState.IPInfo curip = new InterfaceState.IPInfo();
                        curip.IPAddr = varVal;
                        varVal = tokenAfter(pfields, "broadcast");
                        if (varVal != "")
                            curip.IPBcast = varVal;
                        varVal = tokenAfter(pfields, "netmask");
                        if (varVal != "")
                        {
                            if (varVal.Contains("."))
                                curip.IPMask = varVal;
                            else
                            {
                                UInt32 masknum;
                                if (varVal.StartsWith("0x"))
                                    masknum = Convert.ToUInt32(varVal.Substring(2), 16);
                                else
                                    masknum = Convert.ToUInt32(varVal, 16);
                                curip.IPMask = String.Format("{0}.{1}.{2}.{3}",
                                    masknum >> 24, (masknum >> 16) & 0x00FF, (masknum >> 8) & 0x00FF, masknum & 0x00FF);
                            }
                        }
                        curip.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_PRIMARY;
                        completeIP(ref curip);
                        curif.InetAddr.Add(curip);
                    }

                    // "ether" não aparece se não for root. Se não preencher MAC, usar "arp -n -a".
                    varVal = tokenAfter(pfields, "ether");
                    if (varVal != "")
                    {
                        curif.HWAddr = varVal;
                        curif.IsPhysical = true;
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_ETHERNET;
                    }
                }
            }
            if (curif != null)
                mySysInfo.Interfaces.Add(curif);
        }

        private static void getInterfacesCiscoIOS(TelnetConnection tc, SysInfo mySysInfo)
        {
            /*
            Ethernet0 is up, line protocol is up
              Hardware is QUICC Ethernet, address is 0002.fd65.597c (bia 0002.fd65.597c)
              Internet address is 10.1.0.125/24
              MTU 1500 bytes, BW 10000 Kbit, DLY 1000 usec, rely 255/255, load 1/255
              Encapsulation ARPA, loopback not set, keepalive not set
              ARP type: ARPA, ARP Timeout 04:00:00
              Last input 00:00:00, output 00:00:00, output hang never
              Last clearing of "show interface" counters never
              Queueing strategy: fifo
              Output queue 0/40, 0 drops; input queue 0/75, 0 drops
              5 minute input rate 1000 bits/sec, 2 packets/sec
              5 minute output rate 0 bits/sec, 0 packets/sec
                 1195156 packets input, 116415381 bytes, 0 no buffer
                 Received 1176612 broadcasts, 0 runts, 0 giants, 0 throttles
                 0 input errors, 0 CRC, 0 frame, 0 overrun, 0 ignored, 0 abort
                 0 input packets with dribble condition detected
                 19417 packets output, 1446163 bytes, 0 underruns
                 0 output errors, 3 collisions, 0 interface resets
                 0 babbles, 0 late collision, 12 deferred
                 0 lost carrier, 0 no carrier
                 0 output buffer failures, 0 output buffers swapped out
            Ethernet1 is administratively down, line protocol is down
              Hardware is QUICC Ethernet, address is 0002.fd65.597d (bia 0002.fd65.597d)
              MTU 1500 bytes, BW 10000 Kbit, DLY 1000 usec, rely 252/255, load 1/255
              Encapsulation ARPA, loopback not set, keepalive not set
              ARP type: ARPA, ARP Timeout 04:00:00
              Last input never, output 2w3d, output hang never
              Last clearing of "show interface" counters never
              Queueing strategy: fifo
              Output queue 0/40, 0 drops; input queue 0/75, 0 drops
              5 minute input rate 0 bits/sec, 0 packets/sec
              5 minute output rate 0 bits/sec, 0 packets/sec
                 0 packets input, 0 bytes, 0 no buffer
                 Received 0 broadcasts, 0 runts, 0 giants, 0 throttles
                 0 input errors, 0 CRC, 0 frame, 0 overrun, 0 ignored, 0 abort
                 0 input packets with dribble condition detected
                 1 packets output, 60 bytes, 0 underruns
                 1 output errors, 0 collisions, 0 interface resets
                 0 babbles, 0 late collision, 0 deferred
                 1 lost carrier, 0 no carrier
                 0 output buffer failures, 0 output buffers swapped out
             */
            char promptChar;
            uint ifIndex = 0;
            InterfaceState curif = null;
            char[] lineseps = { '\r', '\n' };
            char[] fieldseps = { ' ', '\t' };
            string output = tc.CiscoCommand("show interfaces", out promptChar);
            string[] lines = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] ifields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                if (!char.IsWhiteSpace(line[0]))
                {
                    if (curif != null)
                        mySysInfo.Interfaces.Add(curif);

                    curif = new InterfaceState();
                    curif.InetAddr = new List<InterfaceState.IPInfo>();
                    curif.Index = ifIndex++;
                    curif.Name = ifields[0];

                    if (ifields[0].StartsWith("LOOPBACK", StringComparison.InvariantCultureIgnoreCase))
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_LOOPBACK;
                        curif.IsPhysical = false;
                    }
                    else if (ifields[0].StartsWith("ETHERNET", StringComparison.InvariantCultureIgnoreCase))
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_ETHERNET;
                        curif.IsPhysical = true;
                    }
                    else if (ifields[0].StartsWith("SERIAL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_PPP;
                        curif.IsPhysical = true;
                    }
                    else
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_OTHER;
                    }
                }
                else
                {
                    if ((ifields.Length >= 4) && (ifields[0] == "Internet") && (ifields[1] == "address") && (ifields[2] == "is"))
                    {
                        InterfaceState.IPInfo curip = new InterfaceState.IPInfo();
                        string[] ipparts = ifields[3].Split('/');
                        curip.IPAddr = ipparts[0];

                        if (ipparts.Length >= 2)
                        {
                            string varVal = ipparts[1];
                            if (varVal.Contains("."))
                                curip.IPMask = varVal;
                            else
                            {
                                UInt32 masknum;
                                if (varVal.StartsWith("0x"))
                                    masknum = Convert.ToUInt32(varVal.Substring(2), 16);
                                else
                                    masknum = Convert.ToUInt32(varVal, 16);
                                curip.IPMask = String.Format("{0}.{1}.{2}.{3}",
                                    masknum >> 24, (masknum >> 16) & 0x00FF, (masknum >> 8) & 0x00FF, masknum & 0x00FF);
                            }
                        }
                        curip.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_PRIMARY;
                        completeIP(ref curip);
                        curif.InetAddr.Add(curip);
                    }

                    if ((ifields.Length >= 6) && (ifields[0] == "Hardware") && (ifields[1] == "is"))
                    {
                        for (int i = 2; i < ifields.Length - 2; i++)
                        {
                            if ((ifields[i] == "address") && (ifields[i + 1] == "is"))
                            {
                                // TODO: Extract MAC from ifields[i + 2]
                                string tmpMAC = "";
                                for (int j = 0; j < ifields[i + 2].Length; j++)
                                {
                                    if ((j != 0) && ((j & 1) == 0))
                                        tmpMAC += ':';
                                    tmpMAC += ifields[i + 2][j];
                                }
                                if (tmpMAC.Length == 17)
                                {
                                    curif.HWAddr = tmpMAC;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (curif != null)
                mySysInfo.Interfaces.Add(curif);
        }

        public static SysInfo getSysInfoCisco(TelnetConnection tc)
        {
            char promptChar;
            SysInfo mySysInfo = new SysInfo();
            char[] lineseps = { '\r', '\n' };
            char[] fieldseps = { ' ', '\t' };
            string output = tc.CiscoCommand("show version", out promptChar);

            CiscoIOSVersion iosVersion = CiscoIOSHelper.CiscoGetVersion(output);
            mySysInfo.OS = "CiscoIOS";
            mySysInfo.OSVersion = iosVersion.VersionString;
            
            string[] versionparts = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in versionparts)
            {
                string[] parts = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                string prevToken = "";
                foreach (string token in parts)
                {
                    if ((token == "processor") && prevToken.StartsWith("(68"))
                        mySysInfo.Architecture = "m68k";
                    prevToken = token;
                }
                if (!String.IsNullOrEmpty(mySysInfo.Architecture))
                    break;
            }
            getInterfacesCiscoIOS(tc, mySysInfo);

            return mySysInfo;
        }

        public static SysInfo getSysInfo(SshExec exec)
        {
            string arch;
            SysInfo mySysInfo = new SysInfo();
            char[] separators = { ' ', '\t' };
            string output = exec.RunCommand("uname -a");
            string[] unameparts = output.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            mySysInfo.OS = unameparts[0];
            mySysInfo.Hostname = unameparts[1];

            mySysInfo.Interfaces = new List<InterfaceState>();
            switch (mySysInfo.OS)
            {
                case "Linux":
                    mySysInfo.OSVersion = unameparts[2];
                    arch = exec.RunCommand("uname -m").Trim();
                    getInterfacesLinux(exec, mySysInfo);
                    break;
                case "Darwin":
                    mySysInfo.OSVersion = unameparts[2];
                    arch = exec.RunCommand("uname -m").Trim();
                    getInterfacesMacOSX(exec, mySysInfo);
                    break;
                case "FreeBSD":
                    mySysInfo.OSVersion = unameparts[2];
                    arch = exec.RunCommand("uname -m").Trim();
                    getInterfacesBSD(exec, mySysInfo);
                    break;
                case "SunOS":
                    mySysInfo.OSVersion = unameparts[2];
                    arch = exec.RunCommand("uname -m").Trim();
                    getInterfacesSolaris(exec, mySysInfo);
                    break;
                case "AIX":
                    mySysInfo.OSVersion = unameparts[3] + "." + unameparts[2];
                    arch = exec.RunCommand("uname -p").Trim();
                    getInterfacesAIX(exec, mySysInfo);
                    break;
                default:
                    throw new CollectorException(String.Format("Unsupported OS {0}", mySysInfo.OS));
            }

            switch (arch)
            {
                case "i386":
                case "i486":
                case "i586":
                case "i686":
                    mySysInfo.Architecture = "INTEL32";
                    break;
                case "x86_64":
                    mySysInfo.Architecture = "INTEL64";
                    break;
                case "sparc":
                    mySysInfo.Architecture = "SPARC";
                    break;
                case "mips":
                    mySysInfo.Architecture = "MIPS";
                    break;
                case "ppc":
                case "powerpc":
                    mySysInfo.Architecture = "POWERPC32";
                    break;
                case "ppc64":
                    mySysInfo.Architecture = "POWERPC64";
                    break;
                case "alpha":
                    mySysInfo.Architecture = "ALPHA32";
                    break;
                case "alpha64":
                    mySysInfo.Architecture = "ALPHA64";
                    break;
                default:
                    mySysInfo.Architecture = "UNKNOWN";
                    break;
            }

            return mySysInfo;
        }
    }
}
