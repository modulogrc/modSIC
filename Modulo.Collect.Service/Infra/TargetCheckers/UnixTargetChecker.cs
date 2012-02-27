using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Tamir.SharpSsh;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Probe.Unix;

namespace Modulo.Collect.Service.Server.Infra
{
    public class UnixTargetChecker: ITargetChecker
    {
        public TargetCheckingResult Check(TargetInfo targetInfo)
        {
            string errorMessage = null;
            var targetAvailable = false;
            var sshConnectionProvider = new SSHConnectionProvider();
            try
            {
                sshConnectionProvider.Connect(targetInfo);
                targetAvailable = true;
            }
            catch (SshConnectingException sshException)
            {
                errorMessage = sshException.Message;
            }
            catch (Exception genericException)
            {
                errorMessage =
                    string.Format(
                        "An unknown error occurred while trying to connect to target machine ({0}): {1}.",
                        targetInfo.GetAddress(), genericException.Message);
            }
            finally
            {
                sshConnectionProvider.Disconnect();
            }

            return new TargetCheckingResult() { IsTargetAvailable = targetAvailable, ErrorMessage = errorMessage };
        }
    }
}
