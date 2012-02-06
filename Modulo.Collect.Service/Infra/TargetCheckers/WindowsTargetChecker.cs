using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using System.Management;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Probe.Windows;

namespace Modulo.Collect.Service.Server.Infra
{
    public class WindowsTargetChecker: ITargetChecker
    {
        public TargetCheckingResult Check(TargetInfo targetInfo)
        {
            var targetAvailable = true;
            string errorMessage = null;
            try
            {
                new WMIConnectionProvider("cimv2").Connect(targetInfo);
            }
            catch (Exception ex)
            {
                targetAvailable = false;
                errorMessage = ex.Message;
            }

            return new TargetCheckingResult() { IsTargetAvailable = targetAvailable, ErrorMessage = errorMessage };
        }
    }
}
