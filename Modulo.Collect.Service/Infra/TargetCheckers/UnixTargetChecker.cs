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
            var sshConnectionProvider = new SSHConnectionProvider();
            try
            {
                sshConnectionProvider.Connect(targetInfo);
                return new TargetCheckingResult() { IsTargetAvailable = true };
            }
            catch (Exception ex)
            {
                return new TargetCheckingResult() { IsTargetAvailable = false, ErrorMessage = ex.Message };
            }
            finally
            {
                sshConnectionProvider.Disconnect();
            }
        }
    }
}
