using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class TextFileContent
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Pattern { get; set; }
        public uint Instance { get; set; }
        public string Line { get; set; }
        public string Text { get; set; }
        public List<string> SubExpressions { get; set; }

        public override string ToString()
        {
            string retVal = String.Format("{0}: {1}\n    Found {2}\n", Instance, Line, Text);
            foreach (string subExpr in SubExpressions)
            {
                retVal += "    Subexpression: " + subExpr;
            }
            return retVal;
        }
    }
}
