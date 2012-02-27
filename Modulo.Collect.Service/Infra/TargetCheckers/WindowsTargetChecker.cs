using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using System.Management;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Probe.Windows;
using Modulo.Collect.Probe.Common.Exceptions;

namespace Modulo.Collect.Service.Server.Infra
{
    public class WindowsTargetChecker: ITargetChecker
    {
        public TargetCheckingResult Check(TargetInfo targetInfo)
        {
            var targetAvailable = false;
            string errorMessage = null;
            try
            {
                new WMIConnectionProvider("cimv2").Connect(targetInfo);
                targetAvailable = true;
            }
            catch (UnauthorizedAccessException)
            {
                errorMessage =
                    string.Format(
                        "Unable to connect to host. The target machine ({0}) denied access to the user ({1}).",
                        targetInfo.GetAddress(), targetInfo.credentials.GetFullyQualifiedUsername());
            }
            catch (CannotConnectToHostException ex)
            {
                errorMessage =
                    string.Format(
                        "Unable to connect to host. The target machine ({0}) returned the following error: {1}.",
                        targetInfo.GetAddress(), ex.Message);
            }
            catch (Exception ex1)
            {
                errorMessage =
                    string.Format(
                        "An unknown error occurred while trying to connect to host ({0}): {1}.",
                        targetInfo.GetAddress(), ex1.Message);
            }

            return new TargetCheckingResult() { IsTargetAvailable = targetAvailable, ErrorMessage = errorMessage };
        }
    }
}
