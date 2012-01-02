using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinimalisticTelnet;

namespace FrameworkNG
{
    public class CiscoIOSVersion
    {
        private int majorVersion;
        private int minorVersion;
        private int release;
        private string mainlineRebuild;
        private string trainIdentifier;
        private int rebuild;
        private string subRebuild;

        public CiscoIOSVersion()
        {
            majorVersion = minorVersion = 0;
            release = rebuild = -1;
            mainlineRebuild = trainIdentifier = subRebuild = "";
        }

        public int MajorVersion { get { return majorVersion; } }
        public int MinorVersion { get { return minorVersion; } }
        public int Release { get { return release; } }
        public string MainlineRebuild { get { return mainlineRebuild; } }
        public string TrainIdentifier { get { return trainIdentifier; } }
        public int Rebuild { get { return rebuild; } }
        public string SubRebuild { get { return subRebuild; } }

        public override string ToString()
        {
            return VersionString;
        }

        public string VersionString
        {
            get
            {
                if (release >= 0)
                    return TrainNumber + "(" + release.ToString() + mainlineRebuild + ")" + MajorRelease;
                else
                    return TrainNumber;
            }

            set
            {
                int posOpenPar = value.IndexOf('(');
                if (posOpenPar < 0)
                {
                    TrainNumber = value;
                    return;
                }
                TrainNumber = value.Substring(0, posOpenPar);
                int posClosePar = value.IndexOf(')', posOpenPar + 1);
                if (posClosePar > 0)
                {
                    if (posClosePar < value.Length - 1)
                        MajorRelease = value.Substring(posClosePar + 1);
                    string middle = value.Substring(posOpenPar + 1, posClosePar - posOpenPar - 1);
                    if ((middle.Length > 0) && char.IsDigit(middle[0]))
                    {
                        for (int i = 0; i < middle.Length; i++)
                        {
                            if (!char.IsDigit(middle[i]))
                            {
                                mainlineRebuild = middle.Substring(i);
                                middle = middle.Substring(0, i);
                                break;
                            }
                        }
                        release = int.Parse(middle);
                    }
                }
            }
        }

        public string MajorRelease
        {
            get
            {
                string retVal = "";
                if (!String.IsNullOrEmpty(trainIdentifier))
                {
                    retVal = trainIdentifier;
                    if (rebuild >= 0)
                    {
                        retVal += rebuild.ToString();
                        if (!String.IsNullOrEmpty(subRebuild))
                            retVal += subRebuild;
                    }
                }
                return retVal;
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    trainIdentifier = subRebuild = "";
                    rebuild = -1;
                }

                if (char.IsDigit(value[0]))
                    throw new ArgumentException("MajorRelease must not begin with a digit");

                int pos = 0;
                while ((pos < value.Length) && !char.IsDigit(value[pos]))
                    pos++;
                trainIdentifier = value.Substring(0, pos);
                if (pos < value.Length)
                {
                    int iniReb = pos;
                    while ((pos < value.Length) && char.IsDigit(value[pos]))
                        pos++;
                    rebuild = int.Parse(value.Substring(iniReb, pos - iniReb));
                    subRebuild = value.Substring(pos);
                }
            }
        }

        public string TrainNumber
        {
            get
            {
                return majorVersion + "." + minorVersion;
            }

            set
            {
                string[] parts = value.Split('.');
                int maj, min;

                if (parts.Length == 2)
                    if (int.TryParse(parts[0], out maj))
                        if (int.TryParse(parts[1], out min))
                            if ((maj >= 0) && (min >= 0))
                            {
                                majorVersion = maj;
                                minorVersion = min;
                                return;
                            }

                throw new ArgumentException("TrainNumber must be of the form M.N, M and N nonnegative integers");
            }
        }
    }

    public static class CiscoIOSHelper
    {
        public static CiscoIOSVersion CiscoGetVersion(TelnetConnection tc)
        {
            char promptChar;
            string output = tc.CiscoCommand("show version", out promptChar);
            return CiscoGetVersion(output);
        }

        public static CiscoIOSVersion CiscoGetVersion(string output)
        {
            CiscoIOSVersion retVal = null;
            string[] lines = output.Split('\r', '\n');
            foreach (string line in lines)
            {
                if (line.StartsWith("IOS"))
                {
                    string verString = " Version ";
                    int posVer = line.IndexOf(verString, StringComparison.OrdinalIgnoreCase);
                    if (posVer >= 0)
                    {
                        posVer += verString.Length;
                        verString = line.Substring(posVer);
                        string[] verStringCut = verString.Split(' ', '\t', ',');
                        retVal = new CiscoIOSVersion();
                        retVal.VersionString = verStringCut[0];
                        break;
                    }
                }
            }
            return retVal;
        }
    }
}
