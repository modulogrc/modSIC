using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class SolarisPkgInfoCollector
    {
        public static Dictionary<string, string> getAllPkgProps(SshExec exec, string pkg)
        {
            Dictionary<string, string> retList = new Dictionary<string, string>();
            string cmdOutput = exec.RunCommand("pkginfo -l '" + pkg + "'");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] comps = line.Split(new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (comps.Length == 2)
                {
                    if (comps[0].EndsWith(":"))
                        comps[0] = comps[0].Remove(comps[0].Length - 1);
                    retList[comps[0]] = comps[1];
                }
            }

            return retList;
        }

        public static SolarisPkgInfo getPkgInfo(SshExec exec, string pkg)
        {
            string tmpVal;
            Dictionary<string, string> allProps = getAllPkgProps(exec, pkg);
            SolarisPkgInfo retVal = new SolarisPkgInfo() { PkgInst = pkg };
            allProps.TryGetValue("NAME", out tmpVal);
            retVal.Name = tmpVal;
            allProps.TryGetValue("CATEGORY", out tmpVal);
            retVal.Category = tmpVal;
            allProps.TryGetValue("VERSION", out tmpVal);
            retVal.Version = tmpVal;
            allProps.TryGetValue("VENDOR", out tmpVal);
            retVal.Vendor = tmpVal;
            allProps.TryGetValue("DESC", out tmpVal);
            retVal.Description = tmpVal;
            return retVal;
        }
    }
}
