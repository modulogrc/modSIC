using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class InetdInfo
    {
        public string ServiceName { get; set; }
        public string EndpointType { get; set; }
        public string Protocol { get; set; }
        public string WaitStatus { get; set; }
        public string ExecAsUser { get; set; }
        public string ServerProgram { get; set; }
        public string ServerArgs { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5} {6}", ServiceName, EndpointType, Protocol, WaitStatus, ExecAsUser, ServerProgram, ServerArgs);
        }
    }
}
