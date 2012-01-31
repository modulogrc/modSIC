using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Probe.CiscoIOS;

namespace Modulo.Collect.Service.Server.Infra
{
    public class CiscoIosChecker: ITargetChecker
    {
        public TargetCheckingResult Check(TargetInfo targetInfo)
        {
            var ciscoConnectionProvider = new CiscoIOSConnectionProvider();
            try
            {
                ciscoConnectionProvider.Connect(targetInfo);
                return new TargetCheckingResult() { IsTargetAvailable = true };
            }
            catch (Exception ex)
            {
                return new TargetCheckingResult() { IsTargetAvailable = false, ErrorMessage = ex.Message };
            }
            finally
            {
                ciscoConnectionProvider.Disconnect();
            }
        }
    }
}
