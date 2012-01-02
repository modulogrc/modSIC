using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class UnameInfoCollector
    {
        public static UnameInfo getUnameInfo(SshExec exec)
        {
            UnameInfo myInfo = new UnameInfo();
            char[] separators = { '\r', '\n' };
            string output = exec.RunCommand("uname -s ; uname -n ; uname -r ; uname -v ; uname -m ; uname -p");
            string[] unameparts = output.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            myInfo.OSName = unameparts[0];
            myInfo.NodeName = unameparts[1];
            myInfo.OSRelease = unameparts[2];
            myInfo.OSVersion = unameparts[3];
            myInfo.MachineClass = unameparts[4];
            myInfo.ProcessorType = unameparts[5];

            return myInfo;
        }
    }
}
