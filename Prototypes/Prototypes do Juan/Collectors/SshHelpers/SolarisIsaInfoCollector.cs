using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class SolarisIsaInfoCollector
    {
        public static SolarisIsaInfo getIsaInfo(SshExec exec)
        {
            SolarisIsaInfo retVal = null;
            string cmdOutput = exec.RunCommand("isainfo -b && isainfo -k && isainfo -n");
            string[] comps = cmdOutput.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (comps.Length == 3)
            {
                retVal = new SolarisIsaInfo();
                retVal.Bits = comps[0];
                retVal.KernelIsa = comps[1];
                retVal.ApplicationIsa = comps[2];
            }
            return retVal;
        }
    }
}
