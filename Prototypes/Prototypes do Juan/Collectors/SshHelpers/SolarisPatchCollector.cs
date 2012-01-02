using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class SolarisPatchCollector
    {
        public static SolarisPatchInfo parsePatchInfo(string myLine)
        {
            SolarisPatchInfo retVal = new SolarisPatchInfo();
            bool gotToPatch = false;
            string patchName = "";

            foreach (char c in myLine)
            {
                bool isPatchChar = (((c >= '0') && (c <= '9')) || (c == '-'));

                if (gotToPatch && !isPatchChar)
                    break;

                if (isPatchChar)
                {
                    gotToPatch = true;
                    patchName += c;
                }
            }

            string[] comps = patchName.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            retVal.Base = ulong.Parse(comps[0]);
            if (comps.Length > 1)
                retVal.Version = ulong.Parse(comps[1]);
            else
                retVal.Version = 0;

            return retVal;
        }

        public static Dictionary<ulong, SolarisPatchInfo> getPatchInfo(SshExec exec)
        {
            Dictionary<ulong, SolarisPatchInfo> retList = new Dictionary<ulong, SolarisPatchInfo>();
            string cmdOutput = exec.RunCommand("showrev -p");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                SolarisPatchInfo thisInfo = parsePatchInfo(line);
                if (thisInfo != null)
                    retList[thisInfo.Base] = thisInfo;
            }

            return retList;
        }
    }
}
