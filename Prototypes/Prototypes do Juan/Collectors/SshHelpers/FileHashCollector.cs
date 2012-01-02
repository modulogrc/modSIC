using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class FileHashCollector
    {
        private static Dictionary<string, string> parseFileHashes(string cmdOutput)
        {
            char[] lineseps = { '\r', '\n' };
            string[] lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            return parseFileHashes(lines);
        }

        private static Dictionary<string, string> parseFileHashes(string[] lines)
        {
            char[] fieldseps = { ' ', '\t' };

            Dictionary<string, string> retList = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                string[] ffields = line.Split(fieldseps, 2, StringSplitOptions.RemoveEmptyEntries);
                if (ffields.GetUpperBound(0) == 1)
                {
                    if (ffields[1].Substring(0, 1) == "*")
                        ffields[1] = ffields[1].Substring(1);
                    retList[ffields[1]] = ffields[0];
                }
            }
            return retList;
        }

        private static Dictionary<string, string> parseFileHashesOpenSSL(string[] lines)
        {
            char[] fieldseps = { ' ', '\t' };

            Dictionary<string, string> retList = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                int parOpen = line.IndexOf('(');
                int parClose = line.LastIndexOf(')');

                if ((parOpen >= 0) && (parClose > (parOpen + 1)))
                {
                    string fileName = line.Substring(parOpen + 1, parClose - parOpen - 1);
                    string[] ffields = line.Split(fieldseps, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (ffields.GetUpperBound(0) > 0)
                    {
                        retList[fileName] = ffields[ffields.GetUpperBound(0)];
                    }
                }
            }
            return retList;
        }

        private static Dictionary<string, string> parseFileHashes(SshExec exec, string hashType, string pathspec)
        {
            string cmdOutput;
            string[] lines;
            char[] lineseps = { '\r', '\n' };

            cmdOutput = exec.RunCommand(hashType + "sum '" + pathspec + "' || echo ERROR");
            lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);

            if (lines[lines.GetUpperBound(0)] != "ERROR")
                return parseFileHashes(lines);

            cmdOutput = exec.RunCommand("openssl dgst -" + hashType + " '" + pathspec + "' || echo ERROR");
            lines = cmdOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);

            if (lines[lines.GetUpperBound(0)] != "ERROR")
                return parseFileHashesOpenSSL(lines);

            return new Dictionary<string, string>();
        }

        public static List<FileHashInfo> getFileHash(SshExec exec, string pathspec)
        {
            List<FileHashInfo> retList = new List<FileHashInfo>();

            Dictionary<string, string> sha1Sums = parseFileHashes(exec, "sha1", pathspec);
            Dictionary<string, string> md5Sums = parseFileHashes(exec, "md5", pathspec);
            foreach (KeyValuePair<string, string> thisSha1 in sha1Sums)
            {
                FileHashInfo thisHashInfo = new FileHashInfo();
                thisHashInfo.Name = thisSha1.Key;
                thisHashInfo.Sha1 = thisSha1.Value;
                thisHashInfo.Md5 = md5Sums[thisHashInfo.Name];
                retList.Add(thisHashInfo);
            }
            return retList;
        }
    }
}
