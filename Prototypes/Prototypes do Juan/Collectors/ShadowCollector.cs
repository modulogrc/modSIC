using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG
{
    public class ShadowCollector
    {
        private static ShadowInfo parseShadowInfo(string line)
        {
            ShadowInfo retInfo = null;
            char[] elemseps = { ':' };
            string[] ffields = line.Split(elemseps);
            if (ffields.GetUpperBound(0) == 8)
            {
                retInfo = new ShadowInfo();
                retInfo.UserName = ffields[0];
                retInfo.Password = ffields[1];
                retInfo.ChangeLast = ffields[2];
                retInfo.ChangeAllow = ffields[3];
                retInfo.ChangeRequire = ffields[4];
                retInfo.ExpireWarning = ffields[5];
                retInfo.ExpireInactivity = ffields[6];
                retInfo.ExpireDate = ffields[7];
                retInfo.FlagReserved = ffields[8];
            }
            return retInfo;
        }

        public static List<ShadowInfo> getShadowInfo(SshExec exec)
        {
            List<ShadowInfo> retList = new List<ShadowInfo>();
            string cmdOutput = exec.RunCommand("cat /etc/shadow");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                ShadowInfo thisInfo = parseShadowInfo(line);
                if (thisInfo != null)
                    retList.Add(thisInfo);
            }

            return retList;
        }
    }
}
