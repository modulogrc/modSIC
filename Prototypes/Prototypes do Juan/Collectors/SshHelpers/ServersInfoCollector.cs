using System;
using System.Collections.Generic;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class ServersInfoCollector
    {
        private static InetServerInfo parseServerInfo(string line)
        {
            InetServerInfo retInfo = new InetServerInfo();
            char[] elemseps = { ' ', '\t' };
            string[] ffields = line.Split(elemseps, StringSplitOptions.RemoveEmptyEntries);
            string portAndPid = "";
            retInfo.Protocol = ffields[0];
            if ((retInfo.Protocol == "tcp") && (ffields.GetUpperBound(0) >= 6))
            {
                retInfo.LocalFullAddress = ffields[3];
                retInfo.ForeignFullAddress = ffields[4];
                portAndPid = ffields[6];
            }
            else if ((retInfo.Protocol == "udp") && (ffields.GetUpperBound(0) >= 5))
            {
                retInfo.LocalFullAddress = ffields[3];
                retInfo.ForeignFullAddress = ffields[4];
                portAndPid = ffields[5];
            }
            else
            {
                return null;
            }

            int whereSlash = portAndPid.LastIndexOf('/');
            if (whereSlash <= 0)
                return null;
            retInfo.ProgramName = portAndPid.Substring(whereSlash + 1);
            retInfo.ProcessId = uint.Parse(portAndPid.Substring(0, whereSlash));

            return retInfo;
        }

        public static List<InetServerInfo> getInetServerInfo(SshExec exec)
        {
            List<InetServerInfo> retList = new List<InetServerInfo>();
            string cmdOutput = exec.RunCommand("netstat --inet --listening -n -p");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                InetServerInfo thisInfo = parseServerInfo(line);
                if (thisInfo != null)
                    retList.Add(thisInfo);
            }

            cmdOutput = exec.RunCommand("ps aux");
            lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<uint, string> usersPerPid = new Dictionary<uint, string>();
            char[] elemseps = { ' ', '\t' };
            string[] ffields;
            uint pidnum;
            foreach (string line in lines)
            {
                ffields = line.Split(elemseps, StringSplitOptions.RemoveEmptyEntries);
                if (ffields.GetUpperBound(0) >= 1)
                {
                    if (uint.TryParse(ffields[1], out pidnum))
                        usersPerPid[pidnum] = ffields[0];
                }
            }

            string username;
            foreach (InetServerInfo thisInfo in retList)
            {
                if (usersPerPid.TryGetValue(thisInfo.ProcessId, out username))
                    thisInfo.User = username;
            }

            return retList;
        }
    }
}
