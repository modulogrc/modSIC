using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class FileHashInfo
    {
        public string Name;
        public string Sha1;
        public string Md5;

        public override string ToString()
        {
            return String.Format("{0}: SHA1: {1}; MD5: {2}", this.Name, this.Sha1, this.Md5);
        }
    }
}
