using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class LinuxPackageInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Arch { get; set; }
        public string Release { get; set; }
        public uint Epoch { get; set; }
        public string Signature { get; set; }

        public string Revision
        {
            get
            {
                return this.Release;
            }
            set
            {
                this.Release = value;
            }
        }

        public string Evr
        {
            get
            {
                if (this.Epoch > 0)
                    return String.Format("{0}:{1}-{2}", this.Epoch, this.Version, this.Release);
                else
                    return String.Format("{1}-{2}", this.Version, this.Release);
            }
            set
            {
                string verAndRelease;

                int whereEpoch = value.IndexOf(':');
                if (whereEpoch >= 0)
                {
                    this.Epoch = uint.Parse(value.Substring(0, whereEpoch));
                    verAndRelease = value.Substring(whereEpoch + 1);
                }
                else
                {
                    this.Epoch = 0;
                    verAndRelease = value;
                }

                int whereRel = verAndRelease.IndexOf('-');
                if (whereRel >= 0)
                {
                    this.Version = verAndRelease.Substring(0, whereRel);
                    this.Release = verAndRelease.Substring(whereRel + 1);
                }
                else
                {
                    this.Version = verAndRelease;
                    this.Release = "0";
                }
            }
        }

        public LinuxPackageInfo()
        {
            this.Epoch = 0;
        }

        public override string ToString()
        {
            string stringize = String.Format("Name <{0}> Version <{1}> Arch <{2}> Release <{3}>", Name, Version, Arch, Release);
            if (Epoch > 0)
                stringize += String.Format(" Epoch <{0}>", Epoch);
            return stringize;
        }
    }
}
