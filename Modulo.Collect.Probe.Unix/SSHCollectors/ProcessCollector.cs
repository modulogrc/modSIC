using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common.Extensions;


namespace Modulo.Collect.Probe.Unix.SSHCollectors
{
    public class UnixProcessInfo
    {
        public uint Pid { get; set; }
        public uint PPid { get; set; }
        public string Command { get; set; }
        public string User { get; set; }
        public int Prio { get; set; }
        public string Tty { get; set; }
        public string Class { get; set; }
        public DateTime StartTime { get; set; }
        public long CpuTime { get; set; }

        public override string ToString()
        {
            return String.Format(
                "PID {0}; PPID {1}; User {2}; Prio {3}; TTY {4}; Class {5}; StartTime {6}; ExecTime {7}; {8}",
                Pid, PPid, User, Prio, Tty, Class, StartTime, CpuTime, Command);
        }
    }

    public class ProcessInfoCollector
    {
        private SshCommandLineRunner CommandLineRunner;

        public ProcessInfoCollector(SshCommandLineRunner commandLineRunner)
        {
            this.CommandLineRunner = commandLineRunner;
        }

        public virtual IEnumerable<UnixProcessInfo> GetProcessInfo()
        {

            var allTargetProcesses = TryToGetProcessInfo(true);
            if (allTargetProcesses == null)
            {
                // Try without the "class" keyword if error
                allTargetProcesses = TryToGetProcessInfo(false);
            }

            return allTargetProcesses;
        }


        private IEnumerable<UnixProcessInfo> TryToGetProcessInfo(Boolean withClassArgument)
        {
            //"ps -A -o 'pid uid ppid pri pid time etime tty args' || echo ERROR"
            var argument = withClassArgument ? "class" : "pid";
            var commandText = String.Format("ps -A -o 'pid uid ppid pri {0} time etime tty args' || echo ERROR", argument);
            var cmdOutput = CommandLineRunner.ExecuteCommand(commandText);
            var commandLines = cmdOutput.SplitStringByDefaultNewLine();
            var baseTime = DateTime.Now;
            
            if (commandLines.Last() == "ERROR")
                return null;
            
            var targetProcessInfo = commandLines.Select(cmdLine => parseProcessInfo(cmdLine, baseTime));
            return targetProcessInfo.Where(procInfo => procInfo != null).ToList();
        }

        private long psTime2Secs(string psTime)
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

        private UnixProcessInfo parseProcessInfo(string line, DateTime baseTime)
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
    }
}
