using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public static class Utils
    {
        public static void AddIfUnique<T>(List<T> myList, T myItem)
        {
            if (!myList.Contains(myItem))
                myList.Add(myItem);
        }

        public static List<string> getPath(SshExec exec)
        {
            List<string> retVal = new List<string>();
            string cmdOutput = exec.RunCommand("echo $PATH");
            string[] lines = cmdOutput.Split(new char[] { '\r', '\n' }, 2, StringSplitOptions.RemoveEmptyEntries);
            retVal.AddRange(lines[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries));
            return retVal;
        }
    }
}
