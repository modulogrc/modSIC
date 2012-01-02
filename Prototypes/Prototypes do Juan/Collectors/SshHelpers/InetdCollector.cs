using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class InetdCollector
    {
        private static InetdInfo parseInetdInfo(string line)
        {
            InetdInfo retInfo = null;
            char[] elemseps = { '\t', ' ' };
            string[] ffields = line.Split(elemseps, 7, StringSplitOptions.RemoveEmptyEntries);
            if ((ffields.GetUpperBound(0) == 6) && (ffields[0].Substring(0, 1) != "#"))
            {
                retInfo = new InetdInfo();
                retInfo.ServiceName = ffields[0];
                retInfo.EndpointType = ffields[1];
                retInfo.Protocol = ffields[2];
                retInfo.WaitStatus = ffields[3];
                retInfo.ExecAsUser = ffields[4];
                retInfo.ServerProgram = ffields[5];
                retInfo.ServerArgs = ffields[6];
            }
            return retInfo;
        }

        public static List<InetdInfo> getInetdInfo(SshExec exec)
        {
            List<InetdInfo> retList = new List<InetdInfo>();
            string cmdOutput = exec.RunCommand("cat /etc/inetd.conf");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                InetdInfo thisInfo = parseInetdInfo(line);
                if (thisInfo != null)
                    retList.Add(thisInfo);
            }

            return retList;
        }
    }
}
