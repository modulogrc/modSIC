using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Contract;

namespace ModSicApiTests
{
    [TestClass]
    public class AcceptanceTests
    {
        private modSICClient ModSIC;

        public AcceptanceTests()
        {
            this.ModSIC = new modSICClient();



   //          System.Diagnostics.ProcessStartInfo psi =
   //new System.Diagnostics.ProcessStartInfo(@"C:\listfiles.bat");

   //         C:\Projects\NG\RiskManager\Dev\source\Collectors\Modulo.Collect.Service\bin\Debug\Collect.Service.exe
            
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_collect_Microsoft_Windows_XP_English()
        {
            var package = ModSIC.CreatePackage("ModSicApiTests.modulo-XPEN-oval.xml", "10.1.0.170");
            
            var collectRequestID = SendCollect(package);
            
            while (true)
            {
                System.Threading.Thread.Sleep(30000);
                var token = ModSIC.Login();
                
                var collectResult = ModSIC.Service.GetCollectedResultDocument(collectRequestID, token);
                
                if (collectResult.Status.Equals(CollectStatus.Complete))
                {
                    Assert.IsNotNull(collectResult.SystemCharacteristics);
                    break;
                }
                
                if (collectResult.Status.Equals(CollectStatus.Error))
                {
                    var lastError = GetLastErrorMessageFromExecutionLog(collectResult.ExecutionLogs);
                    Assert.Fail(String.Format("An error occurred during collection: '{0}'", lastError));
                }
            }
        }

        private String SendCollect(Package package)
        {
            var token = ModSIC.Login();
            var collectRequest = ModSIC.Service.SendRequest(package, token);
            if (collectRequest.HasErrors)
                Assert.Fail("An error occurred while trying to send collect: " + collectRequest.Message);
            
            return collectRequest.Requests.First().ServiceRequestId;
        }

        private String GetLastErrorMessageFromExecutionLog(ExecutionLog[] executionLog)
        {
            var lastErrorMessage = string.Empty;
            if (executionLog != null && executionLog.Count() > 0)
            {    
                var errors = executionLog.OrderByDescending(log => log.Date);
                foreach (var log in errors)
                    if (log.LogType.ToLower().Contains("error"))
                        lastErrorMessage = log.Message;

                if (string.IsNullOrWhiteSpace(lastErrorMessage))
                    lastErrorMessage = executionLog.Last().Message;
            }

            return lastErrorMessage;
        }

    }
}
