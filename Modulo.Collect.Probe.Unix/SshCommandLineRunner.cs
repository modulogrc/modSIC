using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.SshNet;

namespace Modulo.Collect.Probe.Unix
{
    public class SshCommandLineRunner
    {
        
        public SshClient SshClient { get; private set; }

        public int? LastCommandExitCode { get { return this.Command.ExitStatus; } }

        private SshCommand Command;


        public SshCommandLineRunner()
        {
        }

        public SshCommandLineRunner(string hostAddress, string username, string password, int port)
        {
            this.SshClient = new SshClient(hostAddress, port, username, password);
        }

        public void Connect()
        {
            this.SshClient.Connect();
        }

        public void Disconnect()
        {
            this.SshClient.Disconnect();
        }

        public virtual String ExecuteCommand(string commandText)
        {
            if (!SshClient.IsConnected)
                SshClient.Connect();

            this.Command = SshClient.CreateCommand(commandText);
            return this.Command.Execute();
        }
    }
}
