using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class AIXFilesetCollector
    {
        private static AIXFileset parseAIXFileset(string line)
        {
            AIXFileset retInfo = null;
            char[] elemseps = { ':' };
            string[] ffields = line.Split(elemseps);
            if ((ffields.GetUpperBound(0) >= 7) && (ffields[0].Substring(0, 1) != "#"))
            {
                int lvlInName;

                retInfo = new AIXFileset();
                retInfo.Description = ffields[7];
                retInfo.Level = ffields[2];
                retInfo.State = ffields[5];
                retInfo.Name = ffields[1];
                lvlInName = retInfo.Name.IndexOf("-" + retInfo.Level);
                if (lvlInName >= 0)
                    retInfo.Name = ffields[0];
            }

            return retInfo;
        }

        public static List<AIXFileset> getAIXFileset(SshExec exec)
        {
            List<AIXFileset> retList = new List<AIXFileset>();
            string cmdOutput = exec.RunCommand("lslpp -L -c");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                AIXFileset thisFileSet = parseAIXFileset(line);
                if (thisFileSet != null)
                    retList.Add(thisFileSet);
            }

            return retList;
        }
    }
}
