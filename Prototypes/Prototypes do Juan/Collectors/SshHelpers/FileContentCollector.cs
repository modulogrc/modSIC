using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class FileContentCollector
    {
        /* public static List<TextFileContent> parseMatches(string dir, string fname, string line, string pattern, ref uint instance)
        {
            List<TextFileContent> retList = new List<TextFileContent>();
            Regex myRegex = new Regex(pattern);
            MatchCollection myMatches = myRegex.Matches(line);
            foreach (Match myMatch in myMatches)
            {
                TextFileContent retItem = new TextFileContent();
                retItem.SubExpressions = new List<string>();

                retItem.Line = line;
                retItem.Pattern = pattern;
                retItem.Text = myMatch.ToString();
                retItem.Path = dir;
                retItem.FileName = fname;
                retItem.Instance = ++instance;
                for (int i = 1; i < myMatch.Groups.Count; i++)
                {
                    retItem.SubExpressions.Add(myMatch.Groups[i].ToString());
                }
                retList.Add(retItem);
            }
            return retList;
        } */

        public static List<TextFileContent> getTextFileContent(SshExec exec, string pathspec, string pattern)
        {
            List<TextFileContent> retList = new List<TextFileContent>();
            string cookedPattern = pattern.Replace("\\", "\\\\");
            cookedPattern = cookedPattern.Replace("/", "\\/");
            cookedPattern = cookedPattern.Replace("'", "'\"'\"'");
            string output = exec.RunCommand("awk '/" + cookedPattern + "/ {print}' <" + pathspec);
            char[] lineseps = { '\r', '\n' };
            string[] lines = output.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            string dir, fname;
            uint instance = 0;
            int whereSlash = pathspec.LastIndexOf('/');
            if (whereSlash > 0)
            {
                dir = pathspec.Substring(0, whereSlash);
                fname = pathspec.Substring(whereSlash + 1);
            }
            else if (whereSlash == 0)
            {
                dir = "/";
                fname = pathspec.Substring(1);
            }
            else
            {
                dir = ".";
                fname = pathspec;
            }
            foreach (string line in lines)
            {
                List<TextFileContent> retPartialList = WinNetUtils.parseMatches(dir, fname, line, cookedPattern, ref instance);
                retList.AddRange(retPartialList);
            }

            return retList;
        }
    }
}
