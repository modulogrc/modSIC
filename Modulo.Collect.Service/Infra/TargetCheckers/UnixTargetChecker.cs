using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Tamir.SharpSsh;

namespace Modulo.Collect.Service.Server.Infra
{
    public class UnixTargetChecker: ITargetChecker
    {
        public TargetCheckingResult Check(TargetInfo targetInfo)
        {
            TargetCheckingResult retVal = new TargetCheckingResult();
            bool success = false;

            try
            {
                string userName = targetInfo.credentials.GetUserName();
                string passWord = targetInfo.credentials.GetPassword();
                string hostName = targetInfo.GetAddress();
                int port = targetInfo.GetPort();
                SshExec testConn = new SshExec(hostName, userName, passWord);
                testConn.Connect(port);
                testConn.Close();
                success = true;
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = ex.Message;
            }
            finally
            {
                retVal.IsTargetAvailable = success;
            }
            return retVal;
        }
    }
}
