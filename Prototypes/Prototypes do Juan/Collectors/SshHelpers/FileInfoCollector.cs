using System;
using System.Collections.Generic;
using System.Text;
using FrameworkNG;
using Tamir.SharpSsh;

namespace FrameworkNG.SshHelpers
{
    public class FileInfoCollector
    {
        private static string DeDotPath(string origPath)
        {
            char[] pathseps = { '/' };
            List<string> pathComps = new List<string>(origPath.Split(pathseps, StringSplitOptions.None));

            int ndx = 0;
            while (ndx < pathComps.Count)
            {
                if (pathComps[ndx] == ".")
                {
                    pathComps.RemoveAt(ndx);
                }
                else if (pathComps[ndx] == "..")
                {
                    pathComps.RemoveAt(ndx);
                    if (ndx > 0)
                    {
                        pathComps.RemoveAt(ndx - 1);
                        ndx--;
                    }
                }
                else
                    ndx++;
            }

            return String.Join("/", pathComps.ToArray());

            /* string retVal = "";
            foreach (string pathComp in pathComps)
                retVal += "/" + pathComp;
            if (retVal == "")
                retVal = "/";
            return retVal; */
        }

        private static GenericFileInfo parseFileInfo(string lsLine, SshExec exec)
        {
            bool numericDate = true;
            bool isDevice = true;
            int nameField = 7;
            int dateField = 5;
            char[] fieldseps = { ' ', '\t' };

            string[] ffields = lsLine.Split(fieldseps, nameField + 1, StringSplitOptions.RemoveEmptyEntries);
            if (ffields.GetUpperBound(0) < nameField)
                return null;

            if (ffields[4].EndsWith(","))   // Device node
            {
                isDevice = true;
                dateField++;
                nameField++;
                ffields = lsLine.Split(fieldseps, nameField + 1, StringSplitOptions.RemoveEmptyEntries);
                if (ffields.GetUpperBound(0) < nameField)
                    return null;
            }

            char dateIndicator = ffields[dateField][0];
            if ((dateIndicator < '0') || (dateIndicator > '9'))
            {
                numericDate = false;
                nameField++;
                ffields = lsLine.Split(fieldseps, nameField + 1, StringSplitOptions.RemoveEmptyEntries);
                if (ffields.GetUpperBound(0) < nameField)
                    return null;
            }

            if (ffields[0].Length < 10)
                return null;

            UnixFileInfo retVal = new UnixFileInfo();
            retVal.Name = ffields[nameField];
            retVal.Owner = ffields[2];
            retVal.Group = ffields[3];

            int whereSlash = retVal.Name.LastIndexOf('/');
            if (whereSlash < 0)
            {
                retVal.FileName = retVal.Name;
                retVal.Path = ".";
            }
            else
            {
                retVal.FileName = retVal.Name.Substring(whereSlash + 1);
                if (whereSlash > 0)
                    retVal.Path = retVal.Name.Substring(0, whereSlash);
                else
                    retVal.Path = "/";
            }

            char fTypeChar = ffields[0][0];
            switch (fTypeChar)
            {
                case '-':
                    retVal.IsDirectory = false;
                    retVal.IsSpecial = false;
                    break;
                case 'd':
                    retVal.IsDirectory = true;
                    retVal.IsSpecial = false;
                    break;
                case 'l':
                    int arrow = retVal.Name.IndexOf(" -> ");
                    if (arrow > 0)
                    {
                        string linkName = retVal.Name.Substring(0, arrow);
                        string targetName = retVal.Name.Substring(arrow + 4);

                        whereSlash = linkName.LastIndexOf('/');
                        if (whereSlash < 0)
                        {
                            retVal.FileName = linkName;
                            retVal.Path = ".";
                        }
                        else
                        {
                            retVal.FileName = linkName.Substring(whereSlash + 1);
                            if (whereSlash > 0)
                                retVal.Path = linkName.Substring(0, whereSlash);
                            else
                                retVal.Path = "/";
                        }

                        if (!targetName.StartsWith("/"))
                        {
                            if (retVal.Path.EndsWith("/"))
                                targetName = retVal.Path + targetName;
                            else
                                targetName = retVal.Path + "/" + targetName;
                        }
                        targetName = DeDotPath(targetName);
                        retVal.Name = linkName;
                        retVal.LinksTo = targetName;
                    }
                    break;
                default:
                    retVal.IsDirectory = false;
                    retVal.IsSpecial = true;
                    break;
            }
            if (isDevice)
                retVal.FileSize = 0;
            else
                retVal.FileSize = ulong.Parse(ffields[4]);

            uint myMode = 0;
            string perms = ffields[0].Substring(1, 9);
            string lperms = perms.ToLower();
            if (lperms[2] == 's')
                myMode += 0x800;    // suid
            if (lperms[5] == 's')
                myMode += 0x200;    // sgid
            if (lperms[8] == 't')
                myMode += 0x100;    // sticky
            perms.Replace('s', 'x');
            perms.Replace('S', '-');
            perms.Replace('t', 'x');
            perms.Replace('T', '-');
            for (int i = 0; i < 9; i++)
            {
                if (perms[8 - i] != '-')
                    myMode += (uint) (1 << i);
            }
            retVal.Mode = myMode;

            if (numericDate)
                retVal.LastModified = DateTime.ParseExact(ffields[dateField] + " " + ffields[dateField + 1], "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            else if (ffields[dateField + 2].Contains(":"))
            {
                int thisYear = DateTime.Now.Year;
                retVal.LastModified = DateTime.ParseExact(ffields[dateField] + " " + ffields[dateField + 1] + " " + thisYear.ToString() + " " + ffields[dateField + 2], "MMM d yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                if (retVal.LastModified > DateTime.Now)
                    retVal.LastModified = DateTime.ParseExact(ffields[dateField] + " " + ffields[dateField + 1] + " " + (thisYear - 1).ToString() + " " + ffields[dateField + 2], "MMM d yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
                retVal.LastModified = DateTime.ParseExact(ffields[dateField] + " " + ffields[dateField + 1] + " " + ffields[dateField + 2], "MMM d yyyy", System.Globalization.CultureInfo.InvariantCulture);

            // Nojentaço: datas de criação e acesso, links simbólicos

            retVal.Found = true;
            return retVal;
        }

        private static List<GenericFileInfo> getFileInfo(string lsOutput, SshExec exec)
        {
            List<GenericFileInfo> retList = new List<GenericFileInfo>();
            char[] lineseps = { '\r', '\n' };
            string[] lines = lsOutput.Split(lineseps, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                GenericFileInfo thisInfo = parseFileInfo(line, exec);
                if (thisInfo != null)
                    retList.Add(thisInfo);
            }
            return retList;
        }

        public static List<GenericFileInfo> getFileInfo(SshExec exec, string pathspec)
        {
            string output = exec.RunCommand("/bin/ls -ld " + pathspec);
            return getFileInfo(output, exec);
        }
    }
}
