using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class SolarisSvcPropCollector
    {
        public static SolarisSvcPropInfo parsePropInfo(string line)
        {
            SolarisSvcPropInfo retInfo = null;
            char[] elemseps = { ' ', '\t' };
            string[] ffields = line.Split(elemseps, 3, StringSplitOptions.RemoveEmptyEntries);
            if (ffields.Length == 3)
            {
                retInfo = new SolarisSvcPropInfo();
                retInfo.Name = ffields[0];
                retInfo.Type = ffields[1];
                retInfo.Value = ffields[2];
            }
            return retInfo;
        }

        public static Dictionary<string, SolarisSvcPropInfo> getAllProps(SshExec exec, string frmi)
        {
            Dictionary<string, SolarisSvcPropInfo> retList = new Dictionary<string,SolarisSvcPropInfo>();
            string cmdOutput = exec.RunCommand("svcprop '" + frmi + "'");
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                SolarisSvcPropInfo thisInfo = parsePropInfo(line);
                if (thisInfo != null)
                    retList[thisInfo.Name] = thisInfo;
            }

            return retList;
        }

        public static SolarisSmfInfo getSmfInfo(SshExec exec, string frmi)
        {
            SolarisSmfInfo retVal = null;
            string[] svcComps = frmi.Split(new char[] { ':' }, 3);
            if (svcComps.Length < 2)
                return null;
            string[] nameComps = svcComps[1].Split(new char[] { '/' });
            if (nameComps.Length < 1)
                return null;
            retVal = new SolarisSmfInfo();
            retVal.frmi = frmi;
            retVal.ServiceName = nameComps[nameComps.Length - 1];

            SolarisSvcPropInfo thisProp = null;
            Dictionary<string, SolarisSvcPropInfo> props = getAllProps(exec, frmi);

            if (props.TryGetValue("restarter/state", out thisProp))
                retVal.ServiceState = thisProp.Value.ToUpper();

            if (props.TryGetValue("inetd/proto", out thisProp))
                retVal.Protocol = thisProp.Value;

            if (props.TryGetValue("inetd_start/exec", out thisProp) || props.TryGetValue("start/exec", out thisProp))
            {
                int slashSpacePos = thisProp.Value.IndexOf("\\ ");
                if (slashSpacePos < 0)
                    retVal.ServerExecutable = thisProp.Value;
                else
                {
                    retVal.ServerExecutable = thisProp.Value.Substring(0, slashSpacePos);
                    retVal.ServerArgs = thisProp.Value.Substring(slashSpacePos + 2);
                }
            }

            if (props.TryGetValue("inetd_start/user", out thisProp))
                retVal.ExecAsUser = thisProp.Value;

            return retVal;
        }
    }
}
