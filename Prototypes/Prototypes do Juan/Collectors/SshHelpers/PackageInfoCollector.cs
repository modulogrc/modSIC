using System;
using System.Collections.Generic;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class PackageInfoCollector
    {
        public static bool DetectSlackware(SshExec exec)
        {
            string grepOutput = exec.RunCommand("grep '^PACKAGE NAME: ' /var/log/packages/kernel-*-`uname -r | tr '-' '_' 2>/dev/null`-*-* 2>/dev/null");
            return grepOutput.Contains("PACKAGE NAME");
        }

        public static bool DetectRedHat(SshExec exec)
        {
            string grepOutput = exec.RunCommand("rpm -qfl /etc/redhat-release 2>/dev/null");
            return grepOutput.Contains("/etc/redhat-release");
        }

        public static bool DetectDebian(SshExec exec)
        {
            string grepOutput = exec.RunCommand("dpkg -L base-files 2>/dev/null | grep '^/etc/debian_version$'");
            return grepOutput.Contains("/etc/debian_version");
        }

        private static LinuxPackageInfo parseDebianPackage(string line)
        {
            char[] elemseps = { '\t' };
            string[] ffields = line.Split(elemseps, StringSplitOptions.None);
            if (ffields.GetUpperBound(0) < 2)
                return null;
            LinuxPackageInfo retInfo = new LinuxPackageInfo();
            retInfo.Name = ffields[0];
            retInfo.Arch = ffields[2];
            retInfo.Evr = ffields[1];
            return retInfo;
        }

        private static LinuxPackageInfo parseRedHatPackage(string line)
        {
            char[] elemseps = { '\t' };
            string[] ffields = line.Split(elemseps, StringSplitOptions.None);
            if (ffields.GetUpperBound(0) < 4)
                return null;
            LinuxPackageInfo retInfo = new LinuxPackageInfo();
            retInfo.Name = ffields[0];
            retInfo.Version = ffields[1];
            retInfo.Release = ffields[2];
            retInfo.Arch = ffields[3];
            if ((ffields[4][0] >= '0') && (ffields[4][0] <= '9'))
                retInfo.Epoch = uint.Parse(ffields[4]);
            return retInfo;
        }

        private static LinuxPackageInfo parseSlackwarePackage(string line)
        {
            char[] elemseps = { '-' };
            string[] ffields = line.Split(elemseps, StringSplitOptions.None);
            int lastFld = ffields.GetUpperBound(0);
            if (lastFld < 3)
                return null;
            LinuxPackageInfo retInfo = new LinuxPackageInfo();
            retInfo.Revision = ffields[lastFld];
            retInfo.Arch = ffields[lastFld - 1];
            retInfo.Version = ffields[lastFld - 2];
            retInfo.Name = String.Join("-", ffields, 0, lastFld - 2);
            return retInfo;
        }

        public static LinuxPackageInfo getPackageInfo(SshExec exec, string packageName)
        {
            if (DetectDebian(exec))
            {
                string lsOutput = exec.RunCommand("dpkg-query -W -f='${Package}\t${Version}\t${Architecture}\n' " + packageName);
                char[] lineseps = { '\r', '\n' };
                string[] lines = lsOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    LinuxPackageInfo thisInfo = parseDebianPackage(line);
                    if (thisInfo != null)
                        if (thisInfo.Name == packageName)
                            return thisInfo;
                }
            }
            else if (DetectRedHat(exec))
            {
                string lsOutput = exec.RunCommand("rpm -q --qf '%{NAME}\t%{VERSION}\t%{RELEASE}\t%{ARCH}\t%{EPOCH}\n' " + packageName);
                char[] lineseps = { '\r', '\n' };
                string[] lines = lsOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    LinuxPackageInfo thisInfo = parseRedHatPackage(line);
                    if (thisInfo != null)
                        if (thisInfo.Name == packageName)
                            return thisInfo;
                }
            }
            else if (DetectSlackware(exec))
            {
                string lsOutput = exec.RunCommand("cd /var/log/packages 2>/dev/null && /bin/ls " + packageName + "*");
                char[] lineseps = { '\r', '\n' };
                string[] lines = lsOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    LinuxPackageInfo thisInfo = parseSlackwarePackage(line);
                    if (thisInfo != null)
                        if (thisInfo.Name == packageName)
                            return thisInfo;
                }
            }
            return null;
        }

        public static List<LinuxPackageInfo> getPackageInfo(SshExec exec)
        {
            List<LinuxPackageInfo> retList = new List<LinuxPackageInfo>();

            if (DetectDebian(exec))
            {
                string lsOutput = exec.RunCommand("dpkg-query -W -f='${Package}\t${Version}\t${Architecture}\n'");
                char[] lineseps = { '\r', '\n' };
                string[] lines = lsOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    LinuxPackageInfo thisInfo = parseDebianPackage(line);
                    if (thisInfo != null)
                        retList.Add(thisInfo);
                }
            }
            else if (DetectRedHat(exec))
            {
                string lsOutput = exec.RunCommand("rpm -qa --qf '%{NAME}\t%{VERSION}\t%{RELEASE}\t%{ARCH}\t%{EPOCH}\n'");
                char[] lineseps = { '\r', '\n' };
                string[] lines = lsOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    LinuxPackageInfo thisInfo = parseRedHatPackage(line);
                    if (thisInfo != null)
                        retList.Add(thisInfo);
                }
            }
            else if (DetectSlackware(exec))
            {
                string lsOutput = exec.RunCommand("/bin/ls /var/log/packages");
                char[] lineseps = { '\r', '\n' };
                string[] lines = lsOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    LinuxPackageInfo thisInfo = parseSlackwarePackage(line);
                    if (thisInfo != null)
                        retList.Add(thisInfo);
                }
            }
            return retList;
        }
    }
}
