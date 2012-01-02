using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class PasswdCollector
    {
        private static PasswdInfo parsePasswdInfo(string line)
        {
            PasswdInfo retInfo = null;
            char[] elemseps = { ':' };
            string[] ffields = line.Split(elemseps);
            if (ffields.GetUpperBound(0) == 6)
            {
                retInfo = new PasswdInfo();
                retInfo.UserName = ffields[0];
                retInfo.Password = ffields[1];
                retInfo.Uid = ffields[2];
                retInfo.Gid = ffields[3];
                retInfo.Gecos = ffields[4];
                retInfo.HomeDir = ffields[5];
                retInfo.Shell = ffields[6];
            }
            return retInfo;
        }

        public static List<PasswdInfo> getPasswdInfo(SshExec exec)
        {
            List<PasswdInfo> retList = new List<PasswdInfo>();
            string cmdOutput = exec.RunCommand("cat /etc/passwd");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                PasswdInfo thisInfo = parsePasswdInfo(line);
                if (thisInfo != null)
                    retList.Add(thisInfo);
            }

            return retList;
        }
    }
}
