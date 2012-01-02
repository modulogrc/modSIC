using System;
using System.Collections.Generic;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class ProcessInfoCollector
    {
        private static long psTime2Secs(string psTime)
        {
            long offset = 0;

            int whereDash = psTime.IndexOf('-');
            if (whereDash >= 0)
            {
                offset = long.Parse(psTime.Substring(0, whereDash)) * 86400;
                psTime = psTime.Substring(whereDash + 1);
            }
            char[] elemseps = { ':' };
            string[] timeParts = psTime.Split(elemseps, StringSplitOptions.None);
            if (timeParts.GetUpperBound(0) < 2)
                return 0;

            offset += long.Parse(timeParts[2]) + long.Parse(timeParts[1]) * 60 + long.Parse(timeParts[0]) * 3600;
            return offset;
        }

        private static UnixProcessInfo parseProcessInfo(string line, DateTime baseTime)
        {
            UnixProcessInfo retInfo = new UnixProcessInfo();
            char[] elemseps = { ' ', '\t' };
            string[] ffields = line.Split(elemseps, 9, StringSplitOptions.RemoveEmptyEntries);
            if (ffields.GetUpperBound(0) < 8)
                return null;
            uint pid, ppid;
            int prio;
            if (!uint.TryParse(ffields[0], out pid))
                return null;
            if (!uint.TryParse(ffields[2], out ppid))
                return null;
            if (!int.TryParse(ffields[3], out prio))
                return null;

            retInfo.Pid = pid;
            retInfo.PPid = ppid;
            retInfo.Command = ffields[8];
            retInfo.Prio = prio;
            if (ffields[4] != ffields[0])
                retInfo.Class = ffields[4];
            else                                    // No support for "class", so we...
                retInfo.Class = "Unclassified";     // called ps with the pid keyword repeated
            retInfo.Tty = ffields[7];
            retInfo.User = ffields[1];
            retInfo.CpuTime = psTime2Secs(ffields[5]);
            retInfo.StartTime = baseTime.AddSeconds(-psTime2Secs(ffields[6]));

            return retInfo;
        }

        public static List<UnixProcessInfo> getProcessInfo(SshExec exec)
        {
            List<UnixProcessInfo> retList = new List<UnixProcessInfo>();
            string cmdOutput = exec.RunCommand("ps -A -o 'pid user ppid pri class time etime tty args' || echo ERROR");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            DateTime baseTime = DateTime.Now;

            if (lines[lines.GetUpperBound(0)] != "ERROR")
            {
                foreach (string line in lines)
                {
                    UnixProcessInfo thisInfo = parseProcessInfo(line, baseTime);
                    if (thisInfo != null)
                        retList.Add(thisInfo);
                }
            }
            else
            {
                // Try without the "class" keyword if error
                cmdOutput = exec.RunCommand("ps -A -o 'pid user ppid pri pid time etime tty args' || echo ERROR");
                lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
                baseTime = DateTime.Now;
                if (lines[lines.GetUpperBound(0)] != "ERROR")
                {
                    foreach (string line in lines)
                    {
                        UnixProcessInfo thisInfo = parseProcessInfo(line, baseTime);
                        if (thisInfo != null)
                            retList.Add(thisInfo);
                    }
                }
            }

            return retList;
        }
    }
}
