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
        //private bool IsThereCredential(Credentials credentials)
        //{
        //    return ((credentials != null) && (credentials.Count > 0));
        //}

        //private ConnectionOptions GetConnectionOptions(Credentials credentials)
        //{
        //    ConnectionOptions options = new ConnectionOptions();
        //    options.Impersonation = ImpersonationLevel.Impersonate;
        //    options.Authentication = AuthenticationLevel.Default;
        //    options.EnablePrivileges = true;

        //    if (this.IsThereCredential(credentials))
        //    {
        //        options.Username = credentials.GetFullyQualifiedUsername();
        //        options.Password = credentials.GetPassword();
        //    }

        //    return options;
        //}

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
            //TargetCheckingResult retVal = new TargetCheckingResult();
            //bool success = false;

            //try
            //{
            //    var targetAddress = targetInfo.GetAddress();
            //    var credentials = targetInfo.IsLocalTarget() ? null : targetInfo.credentials;
            //    var options = this.GetConnectionOptions(credentials);
            //    var managementPath = new ManagementPath(string.Format(@"\\{0}\root\cimv2", targetAddress));
            //    var connectScope = new ManagementScope() { Path = managementPath, Options = options };

            //    connectScope.Connect();
            //    success = true;
            //}
            //catch (Exception ex)
            //{
            //    retVal.ErrorMessage = ex.Message;
            //}
            //finally
            //{
            //    retVal.IsTargetAvailable = success;
            //}
            //return retVal;
        }
    }
}
