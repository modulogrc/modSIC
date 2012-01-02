using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class RunLevelCollector
    {
        private static RunLevelInfo parseRunLevelInfo(string line, string runLevel)
        {
            RunLevelInfo retInfo = null;

            if (line.Length > 3)
            {
                char cStartKill = line[0];
                char cDigit1 = line[1];
                char cDigit2 = line[2];
                retInfo  = new RunLevelInfo();

                if (char.IsDigit(cDigit1) && char.IsDigit(cDigit2))
                {
                    if (cStartKill == 'S')
                        retInfo.Start = true;
                    if (cStartKill == 'K')
                        retInfo.Kill = true;
                    retInfo.RunLevel = runLevel;
                    retInfo.Service = line.Substring(3);
                }
            }

            return retInfo;
        }

        public static List<RunLevelInfo> getRunLevelInfo(SshExec exec, string runLevel)
        {
            List<RunLevelInfo> retList = new List<RunLevelInfo>();
            string cmdOutput = exec.RunCommand("ls /etc/rc" + runLevel + ".d");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                RunLevelInfo thisInfo = parseRunLevelInfo(line, runLevel);
                if (thisInfo != null)
                    retList.Add(thisInfo);
            }

            return retList;
        }
    }
}
