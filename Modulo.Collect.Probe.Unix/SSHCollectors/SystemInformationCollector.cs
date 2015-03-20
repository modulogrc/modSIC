/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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
using System.Text;
using Modulo.Collect.Probe.Common.Extensions;

namespace Modulo.Collect.Probe.Unix.SSHCollectors
{
    public class SystemInformationCollector
    {
        private static string TokenFollowing(string[] tokens, string myToken)
        {
            bool nowIsTheTime = false;
            foreach (string thisToken in tokens)
            {
                if (nowIsTheTime)
                    return thisToken;
                if (thisToken == myToken)
                    nowIsTheTime = true;
            }
            return null;
        }

        private static void getInterfacesLinux(SshCommandLineRunner commandRunner, SysInfo mySysInfo)
        {
            uint ifIndex = 0;
            InterfaceState curif = null;
            char[] fieldseps = { ' ', '\t' };
            
            var output = commandRunner.ExecuteCommand("/sbin/ip addr show").SplitStringByDefaultNewLine();
            foreach (var line in output)
            {
                if ((line[0] >= '0') && (line[0] <= '9'))
                {
                    if (curif != null)
                        mySysInfo.Interfaces.Add(curif);
                    curif = new InterfaceState();
                    curif.InetAddr = new List<InterfaceState.IPInfo>();
                    curif.Index = ifIndex++;
                    var ifields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
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
                        curip.IPBcast = TokenFollowing(pfields, "brd");
                        curip.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_PRIMARY;
                        curif.InetAddr.Add(curip);
                    }
                }
            }
            if (curif != null)
                mySysInfo.Interfaces.Add(curif);
        }

        private static void getInterfacesMacOSX(SshCommandLineRunner commandRunner, SysInfo mySysInfo)
        {
            throw new NotImplementedException("Obtaining interfaces for Mac OS not yet implemented");
        }

        private static void getInterfacesBSD(SshCommandLineRunner commandRunner, SysInfo mySysInfo)
        {
            throw new NotImplementedException("Obtaining interfaces for BSD not yet implemented");
        }

        private static void getInterfacesSolaris(SshCommandLineRunner commandRunner, SysInfo mySysInfo)
        {
            uint ifIndex = 0;
            InterfaceState curif = null;
            char[] fieldseps = { ' ', '\t' };
            var commandOutputLines = commandRunner.ExecuteCommand("/sbin/ifconfig -a").SplitStringByDefaultNewLine();

            foreach (var line in commandOutputLines)
            {
                if (!char.IsWhiteSpace(line[0]))
                {
                    if (curif != null)
                    {
                        getMACAIX(commandRunner, curif);
                        mySysInfo.Interfaces.Add(curif);
                    }
                    curif = new InterfaceState();
                    curif.InetAddr = new List<InterfaceState.IPInfo>();
                    curif.Index = ifIndex++;
                    string[] ifields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                    curif.Name = ifields[0].Replace(":", "");
                    string iflags = ifields[1].Replace("<", ",").Replace(">", ",");
                    if (iflags.Contains(",LOOPBACK,"))
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_LOOPBACK;
                        curif.IsPhysical = false;
                    }
                    else if (iflags.Contains(",VIRTUAL,"))
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_OTHER;
                        curif.IsPhysical = false;
                    }
                    else
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_OTHER;
                        curif.IsPhysical = true;
                    }
                }
                else
                {
                    string[] pfields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                    if (pfields[0] == "ether")
                    {
                        curif.Type = InterfaceState.stateInterfaceType.MIB_IF_TYPE_ETHERNET;
                        string[] macParts = pfields[1].Split(':');
                        if (macParts.Length == 6)
                        {
                            for (int i = 0; i < macParts.Length; i++)
                            {
                                if (macParts[i].Length == 1)
                                    macParts[i] = "0" + macParts[i];
                            }
                            curif.HWAddr = String.Join(":", macParts);
                        }
                    }
                    else if (pfields[0] == "inet")
                    {
                        InterfaceState.IPInfo curip = new InterfaceState.IPInfo();
                        curip.IPAddr = pfields[1];
                        if ((pfields.Length > 5) && (pfields[4] == "broadcast"))
                            curip.IPBcast = pfields[5];
                        else
                            curip.IPBcast = "";
                        if ((pfields.Length > 3) && (pfields[2] == "netmask"))
                        {
                            if (pfields[3].Contains("."))
                                curip.IPMask = pfields[3];
                            else
                            {
                                if (pfields[3].StartsWith("0x"))
                                    pfields[3] = pfields[3].Substring(2);
                                UInt32 masknum = Convert.ToUInt32(pfields[3], 16);
                                curip.IPMask = String.Format("{0}.{1}.{2}.{3}",
                                    masknum >> 24, (masknum >> 16) & 0x00FF, (masknum >> 8) & 0x00FF, masknum & 0x00FF);
                            }
                        }
                        else
                            curip.IPMask = "";
                        curip.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_PRIMARY;
                        curif.InetAddr.Add(curip);
                    }
                }
            }
            if (curif != null)
                mySysInfo.Interfaces.Add(curif);
        }

        private static void getMACAIX(SshCommandLineRunner commandRunner, InterfaceState curif)
        {
            char[] fieldseps = { ' ', '\t' };
            char[] onlydot = { '.' };
            var commandOutputLines = commandRunner.ExecuteCommand("netstat -I " + curif.Name).SplitStringByDefaultNewLine();
            if (commandOutputLines == null)
                return;
            foreach (var line in commandOutputLines)
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

        private static void getInterfacesAIX(SshCommandLineRunner commandRunner, SysInfo mySysInfo)
        {
            uint ifIndex = 0;
            InterfaceState curif = null;
            char[] fieldseps = { ' ', '\t' };
            var output = commandRunner.ExecuteCommand("ifconfig -a").SplitStringByDefaultNewLine();
            foreach (var line in output)
            {
                if (!char.IsWhiteSpace(line[0]))
                {
                    if (curif != null)
                    {
                        getMACAIX(commandRunner, curif);
                        mySysInfo.Interfaces.Add(curif);
                    }
                    curif = new InterfaceState();
                    curif.InetAddr = new List<InterfaceState.IPInfo>();
                    curif.Index = ifIndex++;
                    string[] ifields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                    curif.Name = ifields[0].Replace(":", "");
                    string iflags = ifields[1].Replace("<", ",").Replace(">", ",");
                    if (iflags.Contains(",LOOPBACK,"))
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
                    if (pfields[0] == "inet")
                    {
                        InterfaceState.IPInfo curip = new InterfaceState.IPInfo();
                        curip.IPAddr = pfields[1];
                        if ((pfields.Length > 5) && (pfields[4] == "broadcast"))
                            curip.IPBcast = pfields[5];
                        else
                            curip.IPBcast = "";
                        if ((pfields.Length > 3) && (pfields[2] == "netmask"))
                        {
                            if (pfields[3].Contains("."))
                                curip.IPMask = pfields[3];
                            else if (pfields[3].StartsWith("0x"))
                            {
                                UInt32 masknum = Convert.ToUInt32(pfields[3].Substring(2), 16);
                                curip.IPMask = String.Format("{0}.{1}.{2}.{3}",
                                    masknum >> 24, (masknum >> 16) & 0x00FF, (masknum >> 8) & 0x00FF, masknum & 0x00FF);
                            }
                        }
                        else
                            curip.IPMask = "";
                        curip.AddrType = InterfaceState.stateAddrType.MIB_IPADDR_PRIMARY;
                        curif.InetAddr.Add(curip);
                    }
                }
            }
            if (curif != null)
                mySysInfo.Interfaces.Add(curif);
        }

        public static SysInfo getSysInfo(SshCommandLineRunner commandRunner)
        {
            string arch;
            var mySysInfo = new SysInfo();
            
            var output = commandRunner.ExecuteCommand("uname -a");
            var unameparts = output.Split(new[] { ' ', '\t' } , StringSplitOptions.RemoveEmptyEntries);
            mySysInfo.OS = unameparts[0];
            mySysInfo.Hostname = unameparts[1];

            mySysInfo.Interfaces = new List<InterfaceState>();
            switch (mySysInfo.OS)
            {
                case "Linux":
                    mySysInfo.OSVersion = unameparts[2];
                    arch = commandRunner.ExecuteCommand("uname -m").Trim();
                    getInterfacesLinux(commandRunner, mySysInfo);
                    break;
                case "Darwin":
                    mySysInfo.OSVersion = unameparts[2];
                    arch = commandRunner.ExecuteCommand("uname -m").Trim();
                    getInterfacesMacOSX(commandRunner, mySysInfo);
                    break;
                case "FreeBSD":
                    mySysInfo.OSVersion = unameparts[2];
                    arch = commandRunner.ExecuteCommand("uname -m").Trim();
                    getInterfacesBSD(commandRunner, mySysInfo);
                    break;
                case "SunOS":
                    mySysInfo.OSVersion = unameparts[2];
                    arch = commandRunner.ExecuteCommand("uname -p").Trim();
                    getInterfacesSolaris(commandRunner, mySysInfo);
                    break;
                case "AIX":
                    mySysInfo.OSVersion = unameparts[3] + "." + unameparts[2];
                    arch = commandRunner.ExecuteCommand("uname -p").Trim();
                    getInterfacesAIX(commandRunner, mySysInfo);
                    break;
                default:
                    throw new Exception(String.Format("Unsupported OS {0}", mySysInfo.OS));
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

    public class SysInfo
    {
        public string Hostname { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string Architecture { get; set; }
        public List<InterfaceState> Interfaces { get; set; }

        public override string ToString()
        {
            return Hostname + ": " + OS + "; Version " + OSVersion + "; " + Architecture;
        }
    }
}
