using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class WMIWinPatchPackage
    {
        public string HotFixID;
        public string Caption;
        public string Description;
        public string FixComments;
        public string InstalledBy;
        public string InstalledOn;

        public DateTime InstallDate
        {
            get
            {
                if (InstalledOn.Length == 16)
                    return DateTime.FromFileTime(long.Parse(InstalledOn, System.Globalization.NumberStyles.HexNumber));
                else if (InstalledOn.Length == 8)
                    return DateTime.ParseExact(InstalledOn, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                else
                    return DateTime.MinValue;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} <{1}>: {2} (Installed {3} by '{4}')", HotFixID, Caption, Description, InstallDate, InstalledBy);
        }
    }
}
