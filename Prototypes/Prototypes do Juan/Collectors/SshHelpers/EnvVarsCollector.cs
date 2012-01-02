using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class EnvVarsCollector
    {
        private static EnvVarInfo parseEnvVarInfo(string line)
        {
            EnvVarInfo retInfo = null;
            char[] elemseps = { '=' };
            string[] ffields = line.Split(elemseps, 2);
            if (ffields.GetUpperBound(0) == 1)
            {
                retInfo = new EnvVarInfo();
                retInfo.Name = ffields[0];
                retInfo.Value = ffields[1];
            }

            return retInfo;
        }

        public static Dictionary<string, string> getEnvVarsInfo(SshExec exec)
        {
            Dictionary<string, string> retList = new Dictionary<string, string>();
            string cmdOutput = exec.RunCommand("set");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                EnvVarInfo thisInfo = parseEnvVarInfo(line);
                if (thisInfo != null)
                    retList[thisInfo.Name] = thisInfo.Value;
            }

            return retList;
        }
    }
}
