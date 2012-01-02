using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameworkNG
{
    public class SolarisSmfInfo
    {
        public string frmi { get; set; }
        public string ServiceName { get; set; }
        public string ServiceState { get; set; }
        public string Protocol { get; set; }
        public string ServerExecutable { get; set; }
        public string ServerArgs { get; set; }
        public string ExecAsUser { get; set; }

        public string CommandLine
        {
            get
            {
                if (String.IsNullOrEmpty(this.ServerExecutable) || String.IsNullOrEmpty(this.ServerArgs))
                    return this.ServerExecutable;
                else
                    return this.ServerExecutable + ' ' + this.ServerArgs;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}{2}{3}{4}", this.ServiceName,
                                    this.ServiceState,
                                    String.IsNullOrEmpty(this.Protocol) ? "" : ", Proto " + this.Protocol,
                                    String.IsNullOrEmpty(this.CommandLine) ? "" : ", Cmd {" + this.CommandLine + "}",
                                    String.IsNullOrEmpty(this.ExecAsUser) ? "" : ", Run as " + this.ExecAsUser
                                    );
        }
    }
}
