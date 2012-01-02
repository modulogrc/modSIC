using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class AIXFileset
    {
        public string Name;
        public string Level;
        public string State;
        public string Description;

        public string StateVerbose
        {
            get {
                switch (State)
                {
                    case "A":
                        return "Applied";
                    case "B":
                        return "Broken";
                    case "C":
                        return "Committed";
                    case "E":
                        return "EFIX Locked";
                    case "O":
                        return "Obsolete";
                    case "?":
                        return "Inconsistent";
                    default:
                        return "Unknown";
                }
            }
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.Description))
                return String.Format("{0} {1} ({2})", this.Name, this.Level, this.StateVerbose);
            else
                return String.Format("{0} {1} ({2}) - {3}", this.Name, this.Level, this.StateVerbose, this.Description);
        }
    }
}
