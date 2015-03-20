/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;

namespace Modulo.Collect.Probe.CiscoIOS
{
    public class CiscoIOSVersion
    {
        private int majorVersion;
        private int minorVersion;
        private int release;
        private string mainlineRebuild;
        private string trainIdentifier;
        private int rebuild;
        private string subRebuild;

        public CiscoIOSVersion()
        {
            majorVersion = minorVersion = 0;
            release = rebuild = -1;
            mainlineRebuild = trainIdentifier = subRebuild = "";
        }

        public int MajorVersion { get { return majorVersion; } }
        public int MinorVersion { get { return minorVersion; } }
        public int Release { get { return release; } }
        public string MainlineRebuild { get { return mainlineRebuild; } }
        public string TrainIdentifier { get { return trainIdentifier; } }
        public int Rebuild { get { return rebuild; } }
        public string SubRebuild { get { return subRebuild; } }

        public string HostName { get; set; }
        public string OSName { get; set; }
        public string Architecture { get; set; }

        public override string ToString()
        {
            return VersionString;
        }

        public string VersionString
        {
            get
            {
                if (release >= 0)
                    return TrainNumber + "(" + release.ToString() + mainlineRebuild + ")" + MajorRelease;
                else
                    return TrainNumber;
            }

            set
            {
                int posOpenPar = value.IndexOf('(');
                if (posOpenPar < 0)
                {
                    TrainNumber = value;
                    return;
                }
                TrainNumber = value.Substring(0, posOpenPar);
                int posClosePar = value.IndexOf(')', posOpenPar + 1);
                if (posClosePar > 0)
                {
                    if (posClosePar < value.Length - 1)
                        MajorRelease = value.Substring(posClosePar + 1);
                    string middle = value.Substring(posOpenPar + 1, posClosePar - posOpenPar - 1);
                    if ((middle.Length > 0) && char.IsDigit(middle[0]))
                    {
                        for (int i = 0; i < middle.Length; i++)
                        {
                            if (!char.IsDigit(middle[i]))
                            {
                                mainlineRebuild = middle.Substring(i);
                                middle = middle.Substring(0, i);
                                break;
                            }
                        }
                        release = int.Parse(middle);
                    }
                }
            }
        }

        public string MajorRelease
        {
            get
            {
                string retVal = "";
                if (!String.IsNullOrEmpty(trainIdentifier))
                {
                    retVal = trainIdentifier;
                    if (rebuild >= 0)
                    {
                        retVal += rebuild.ToString();
                        if (!String.IsNullOrEmpty(subRebuild))
                            retVal += subRebuild;
                    }
                }
                return retVal;
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    trainIdentifier = subRebuild = "";
                    rebuild = -1;
                }

                if (char.IsDigit(value[0]))
                    throw new ArgumentException("MajorRelease must not begin with a digit");

                int pos = 0;
                while ((pos < value.Length) && !char.IsDigit(value[pos]))
                    pos++;
                trainIdentifier = value.Substring(0, pos);
                if (pos < value.Length)
                {
                    int iniReb = pos;
                    while ((pos < value.Length) && char.IsDigit(value[pos]))
                        pos++;
                    rebuild = int.Parse(value.Substring(iniReb, pos - iniReb));
                    subRebuild = value.Substring(pos);
                }
            }
        }

        public string TrainNumber
        {
            get
            {
                return majorVersion + "." + minorVersion;
            }

            set
            {
                string[] parts = value.Split('.');
                int maj, min;

                if (parts.Length == 2)
                    if (int.TryParse(parts[0], out maj))
                        if (int.TryParse(parts[1], out min))
                            if ((maj >= 0) && (min >= 0))
                            {
                                majorVersion = maj;
                                minorVersion = min;
                                return;
                            }

                throw new ArgumentException("TrainNumber must be of the form M.N, M and N nonnegative integers");
            }
        }
    }

    public static class CiscoIOSHelper
    {
        public static IList<NetworkInterface> CiscoGetInterfaces(TelnetConnection tc)
        {
            List<NetworkInterface> retList = new List<NetworkInterface>();
            NetworkInterface curif = null;
            char[] lineseps = { '\r', '\n' };
            char[] fieldseps = { ' ', '\t' };
            string output = tc.CiscoCommand("show interfaces");
            string[] lines = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] ifields = line.Split(fieldseps, StringSplitOptions.RemoveEmptyEntries);
                if (!char.IsWhiteSpace(line[0]))
                {
                    if (curif != null)
                    {
                        retList.Add(curif);
                    }
                    curif = new NetworkInterface();
                    curif.Name = ifields[0];
                }
                else
                {
                    if (String.Equals(ifields[0], "Internet", StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(ifields[1], "address", StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(ifields[2], "is", StringComparison.OrdinalIgnoreCase))
                    {
                        string tmpIp = ifields[3];
                        string[] tmpIpParts = tmpIp.Split('/');
                        curif.IpAddress = tmpIpParts[0];
                    }
                    if (String.Equals(ifields[0], "Hardware", StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(ifields[1], "is", StringComparison.OrdinalIgnoreCase))
                    {
                        string tmpMac = "";
                        for (int i = 2; i < ifields.Length - 2; i++)
                        {
                            if (String.Equals(ifields[i], "address", StringComparison.OrdinalIgnoreCase) &&
                                String.Equals(ifields[i + 1], "is", StringComparison.OrdinalIgnoreCase))
                            {
                                tmpMac = ifields[i + 2];
                                break;
                            }
                        }
                        if (tmpMac.Length == 14)    // xxxx.xxxx.xxxx MAC format
                        {
                            tmpMac = tmpMac.Substring(0, 2) + ":" +
                                     tmpMac.Substring(2, 2) + ":" +
                                     tmpMac.Substring(5, 2) + ":" +
                                     tmpMac.Substring(7, 2) + ":" +
                                     tmpMac.Substring(10, 2) + ":" +
                                     tmpMac.Substring(12, 2);
                        }
                        if (!String.IsNullOrEmpty(tmpMac))
                            curif.MacAddress = tmpMac;
                    }
                }
            }
            if (curif != null)
            {
                retList.Add(curif);
            }
            return retList;
        }

        public static CiscoIOSVersion CiscoGetVersion(TelnetConnection tc)
        {
            CiscoIOSVersion retVal = null;
            string output = tc.CiscoCommand("show version");
            string[] lines = output.Split('\r', '\n');
            retVal = new CiscoIOSVersion();
            retVal.VersionString = "0.0";
            retVal.HostName = retVal.Architecture = "undefined";

            foreach (string line in lines)
            {
                if (line.StartsWith("IOS") || line.StartsWith("Cisco IOS"))
                {
                    string verString = " Version ";
                    int posVer = line.IndexOf(verString, StringComparison.OrdinalIgnoreCase);
                    if (posVer >= 0)
                    {
                        posVer += verString.Length;
                        verString = line.Substring(posVer);
                        string[] verStringCut = verString.Split(' ', '\t', ',');
                        retVal.VersionString = verStringCut[0];
                        retVal.OSName = "IOS";
                    }
                }

                if (line.IndexOf(" uptime is ", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    int posSpace = line.IndexOfAny(new char[] { ' ', '\t' });
                    retVal.HostName = posSpace != 0 ? line.Substring(0, posSpace) : "unknown";
                }

                if ((line.IndexOf(" bytes of memory", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    retVal.Architecture = line;
                }
            }

            return retVal;
        }

    }
}
