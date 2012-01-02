using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
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
}
