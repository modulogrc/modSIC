using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class ApacheCollector
    {
        public static List<ApacheInfo> getApacheInfo(SshExec exec)
        {
            List<ApacheInfo> retList = new List<ApacheInfo>();
            List<string> pathList = Utils.getPath(exec);
            Utils.AddIfUnique<string>(pathList, "/sbin");
            Utils.AddIfUnique<string>(pathList, "/usr/sbin");
            Utils.AddIfUnique<string>(pathList, "/usr/local/sbin");
            Utils.AddIfUnique<string>(pathList, "/usr/local/apache/bin");
            Utils.AddIfUnique<string>(pathList, "/usr/local/apache/sbin");
            Utils.AddIfUnique<string>(pathList, "/usr/local/apache2/bin");
            Utils.AddIfUnique<string>(pathList, "/usr/local/apache2/sbin");
            Utils.AddIfUnique<string>(pathList, "/usr/local/httpd/bin");
            Utils.AddIfUnique<string>(pathList, "/usr/local/httpd/sbin");

            foreach (string pathComp in pathList)
            {
                ApacheInfo thisInfo = new ApacheInfo { Path = pathComp, BinaryName = "httpd" };
                string cmdOutput = exec.RunCommand(thisInfo.FullBinaryPath + " -v || echo ERROR");
                string[] lines = cmdOutput.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if ((lines.Length >= 2) && (lines[lines.Length - 1] != "ERROR"))
                {
                    string[] versComps = lines[0].Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (versComps.Length == 2)
                    {
                        thisInfo.Version = versComps[1].Trim();
                        retList.Add(thisInfo);
                    }
                }
            }

            return retList;
        }
    }
}
