using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class AIXOSLevelCollector
    {
        public static string getAIXOSLevel(SshExec exec)
        {
            string cmdOutput = exec.RunCommand("oslevel -r");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, 2, StringSplitOptions.RemoveEmptyEntries);
            if (lines.GetUpperBound(0) >= 0)
                return lines[0];
            else
                return "";
        }
    }
}
