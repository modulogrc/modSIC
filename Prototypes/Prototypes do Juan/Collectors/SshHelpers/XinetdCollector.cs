using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class XinetdCollector
    {
        private enum machineState : uint
        {
            EXPECT_SERVICE,
            EXPECT_OPENBRACE,
            EXPECT_PROPERTY,
        }

        private XinetdInfo tmpInfo;
        private XinetdInfo defaultInfo;
        private SshExec exec;
        private machineState mState;
        private Dictionary<string, int> portName2Num;
        private bool isDefaults;
        private char[] whiteSpc = { ' ', '\t' };
        private char[] lineseps = { '\r', '\n' };

        private string reTokenize(string orig, string op, string values)
        {
            List<string> lOrig = new List<string>();
            lOrig.AddRange(orig.Split(whiteSpc, StringSplitOptions.RemoveEmptyEntries));
            List<string> lNew = new List<string>();
            lNew.AddRange(values.Split(whiteSpc, StringSplitOptions.RemoveEmptyEntries));

            if (op == "=")
                return String.Join(" ", lNew.ToArray());
            else if (op == "-=")
            {
                foreach (string toRemove in lNew)
                {
                    if (lOrig.Contains(toRemove))
                        lOrig.Remove(toRemove);
                }
            }
            else if (op == "+=")
            {
                foreach (string toAdd in lNew)
                {
                    if (!lOrig.Contains(toAdd))
                        lOrig.Add(toAdd);
                }
            }

            return String.Join(" ", lOrig.ToArray());
        }

        private void applyProp(XinetdInfo myInfo, string propName, string propOp, string propVal)
        {
            switch (propName)
            {
                // TODO: Parse defaults
                case "disable":
                    myInfo.Disabled = (propVal == "yes");
                    break;
                case "flags":

                    if ((defaultInfo != null) && !String.IsNullOrEmpty(defaultInfo.Flags))
                        myInfo.Flags = reTokenize(defaultInfo.Flags, propOp, propVal);
                    else
                        myInfo.Flags = reTokenize("", propOp, propVal);
                    break;
                case "no_access":
                    if ((defaultInfo != null) && !String.IsNullOrEmpty(defaultInfo.NoAccess))
                        myInfo.NoAccess = reTokenize(defaultInfo.NoAccess, propOp, propVal);
                    else
                        myInfo.NoAccess = reTokenize("", propOp, propVal);
                    break;
                case "only_from":
                    if ((defaultInfo != null) && !String.IsNullOrEmpty(defaultInfo.OnlyFrom))
                        myInfo.OnlyFrom = reTokenize(defaultInfo.OnlyFrom, propOp, propVal);
                    else
                        myInfo.OnlyFrom = reTokenize("", propOp, propVal);
                    break;
                case "port":
                    myInfo.Port = propVal;
                    break;
                case "protocol":
                    myInfo.Protocol = propVal;
                    if (String.IsNullOrEmpty(myInfo.SocketType))
                    {
                        if (propVal == "tcp")
                            myInfo.SocketType = "stream";
                        else if (propVal == "udp")
                            myInfo.SocketType = "dgram";
                    }
                    break;
                case "server":
                    myInfo.Server = propVal;
                    break;
                case "server_args":
                    myInfo.ServerArgs = propVal;
                    break;
                case "socket_type":
                    myInfo.SocketType = propVal;
                    if (String.IsNullOrEmpty(myInfo.Protocol))
                    {
                        if (propVal == "stream")
                            myInfo.Protocol = "tcp";
                        else if (propVal == "dgram")
                            myInfo.Protocol = "udp";
                    }
                    break;
                case "type":
                    myInfo.Type = propVal;
                    break;
                case "user":
                    myInfo.User = propVal;
                    break;
                case "wait":
                    myInfo.Wait = (propVal == "yes");
                    break;
            }
        }

        private XinetdInfo parseXinetdInfo(string line)
        {
            string[] tokens = line.Split(whiteSpc, StringSplitOptions.RemoveEmptyEntries);
            bool returnSomething = false;

            if ((tokens.Length < 1) || (tokens[0].Substring(0, 1) == "#"))
                return null;

            switch (mState)
            {
                case machineState.EXPECT_SERVICE:
                    if ((tokens.Length >= 2) && (tokens[0] == "service"))
                    {
                        if (defaultInfo != null)
                            tmpInfo = defaultInfo.Clone();
                        else
                            tmpInfo = new XinetdInfo();
                        tmpInfo.ServiceName = tokens[1];
                        mState = machineState.EXPECT_OPENBRACE;
                        isDefaults = false;
                    }
                    else if ((tokens.Length >= 1) && (tokens[0] == "defaults"))
                    {
                        tmpInfo = new XinetdInfo();
                        mState = machineState.EXPECT_OPENBRACE;
                        isDefaults = true;
                    }
                    break;

                case machineState.EXPECT_OPENBRACE:
                    if ((tokens.Length >= 1) && (tokens[0] == "{"))
                    {
                        mState = machineState.EXPECT_PROPERTY;
                    }
                    break;

                case machineState.EXPECT_PROPERTY:
                    if ((tokens.Length >= 1) && (tokens[0] == "}"))
                    {
                        mState = machineState.EXPECT_SERVICE;
                        if (isDefaults)
                        {
                            returnSomething = true;
                            break;
                        }
                        // Check for completeness of tmpInfo
                        if (String.IsNullOrEmpty(tmpInfo.ServiceName))
                            break;
                        if (String.IsNullOrEmpty(tmpInfo.Protocol) || String.IsNullOrEmpty(tmpInfo.SocketType))
                            break;
                        if (String.IsNullOrEmpty(tmpInfo.Port))
                        {
                            if (String.IsNullOrEmpty(tmpInfo.Protocol))
                                break;
                            int defPort = 0;
                            if (portName2Num.TryGetValue(tmpInfo.ServiceName + '/' + tmpInfo.Protocol, out defPort))
                                tmpInfo.Port = defPort.ToString();
                            else
                                break;
                        }
                        if (String.IsNullOrEmpty(tmpInfo.Server) && String.IsNullOrEmpty(tmpInfo.Type))
                            break;
                        returnSomething = true;
                    }
                    else
                    {
                        int whereEqual = line.IndexOf('=');
                        if (whereEqual > 0)
                        {
                            string propOp, propName, propVal;
                            char beforeEqual = line[whereEqual - 1];
                            if ((beforeEqual == '+') || (beforeEqual == '-'))
                            {
                                propOp = beforeEqual + "=";
                                propName = line.Substring(0, whereEqual - 1).Trim();
                            }
                            else
                            {
                                propOp = "=";
                                propName = line.Substring(0, whereEqual).Trim();
                            }
                            propVal = line.Substring(whereEqual + 1).Trim();
                            applyProp(tmpInfo, propName, propOp, propVal);
                        }
                    }
                    break;
            }

            if (returnSomething)
            {
                if (isDefaults)
                {
                    defaultInfo = tmpInfo;
                    return null;
                }
                return tmpInfo.Clone();
            }
            else
                return null;
        }

        public List<XinetdInfo> getXinetdInfo()
        {
            string cmdOutput;
            string[] lines;

            List<XinetdInfo> retList = new List<XinetdInfo>();
            cmdOutput = exec.RunCommand("cat /etc/xinetd.conf /etc/xinetd.d/* 2>/dev/null");
            lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                XinetdInfo thisInfo = parseXinetdInfo(line);
                if (thisInfo != null)
                    retList.Add(thisInfo);
            }

            return retList;
        }

        public XinetdCollector(SshExec exec)
        {
            this.exec = exec;
            tmpInfo = null;
            isDefaults = false;
            mState = machineState.EXPECT_SERVICE;

            string cmdOutput;
            string[] lines;

            portName2Num = new Dictionary<string, int>();
            cmdOutput = exec.RunCommand("cat /etc/services 2>/dev/null");
            lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] tokens = line.Split(whiteSpc, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length >= 2)
                {
                    if ((tokens[0].Substring(0, 1) != "#") && (tokens[1].Substring(0, 1) != "#"))
                    {
                        string proto = tokens[1];
                        int whereSlash = proto.IndexOf('/');
                        if (whereSlash > 0)
                        {
                            int portNum = int.Parse(proto.Substring(0, whereSlash));
                            proto = proto.Substring(whereSlash + 1);

                            for (int i = 0; i < tokens.Length; i++)
                            {
                                if (tokens[i].Substring(0, 1) == "#")
                                    break;
                                if (i == 1)
                                    continue;
                                portName2Num[tokens[0] + '/' + proto] = portNum;
                            }
                        }
                    }
                }
            }
        }
    }
}
