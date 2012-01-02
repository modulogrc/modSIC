using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Contract;


namespace ModSicApiTests
{
    [TestClass]
    public class UnitTest1
    {
        private modSICClient ModSicClient;

        public UnitTest1()
        {
            this.ModSicClient = new modSICClient();
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_to_call_Heartbeat_operation()
        {
            try
            {
                this.ModSicClient.Service.Heartbeat();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_login_operation()
        {
            try
            {
                var token = Login();
                Assert.IsFalse(string.IsNullOrWhiteSpace(token));
                this.ModSicClient.Service.Logout(token);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod, Owner("modSIC Team")]
        [ExpectedException(typeof(FaultException))]
        public void Shouldnt_be_possible_call_login_operation_with_null_credentials()
        {
            this.ModSicClient.Service.Login(null, null);
        }

        [TestMethod, Owner("modSIC Team")]
        [ExpectedException(typeof(FaultException))]
        public void Shouldnt_be_possible_call_login_operation_with_invalid_credentials()
        {
            this.ModSicClient.Service.Login("adimin", "Password");
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_get_certificate_operation()
        {
            var certificate = GetCertificate();

            Assert.IsNotNull(certificate);
            Assert.IsFalse(string.IsNullOrWhiteSpace(certificate.PublicKey.ToString()));
            Assert.IsNull(certificate.PrivateKey);
            Assert.AreEqual("B755F89A2EEB37B596F69BE1150CA91CC047C26E", certificate.Thumbprint);
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_logout_operation()
        {
            try
            {
                var token = Login();
                this.ModSicClient.Service.Logout(token);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod, Owner("modSIC Team")]
        [ExpectedException(typeof(FaultException))]
        public void Shouldnt_be_possible_call_an_operation_after_logout()
        {
            var token = Login();
            this.ModSicClient.Service.Logout(token);
            // GetCertificate Operation Calling
            this.ModSicClient.Service.GetCertificate(token);
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_SendRequest_operation()
        {
            var token = Login();
            var package = this.ModSicClient.CreatePackage("ModSicApiTests.oval-sample.xml", "10.1.0.187");

            try
            {
                var requestResult = this.ModSicClient.Service.SendRequest(package, token);
                this.ModSicClient.Service.Logout(token);

                Assert.IsNotNull(requestResult);
                Assert.IsFalse(requestResult.HasErrors);
                Assert.IsTrue(string.IsNullOrEmpty(requestResult.Message));
                Assert.AreEqual(1, requestResult.Requests.Count());

                var expectedClientRequestId = package.CollectRequests.Single().RequestId;
                var returnedRequestInfo = requestResult.Requests.Single();
                Assert.AreEqual(expectedClientRequestId, returnedRequestInfo.ClientRequestId);
                Assert.IsFalse(string.IsNullOrWhiteSpace(returnedRequestInfo.ServiceRequestId));
                Assert.IsTrue(returnedRequestInfo.ServiceRequestId.Contains("collectrequests/"));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_GetCollectedResultDocument_operation()
        {
            var token = Login();
            Result results = null;

            try
            {
                results = ModSicClient.Service.GetCollectedResultDocument("collectrequests/1", token);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsFalse(string.IsNullOrWhiteSpace(results.SystemCharacteristics));
            Assert.AreEqual(CollectStatus.Complete, results.Status);
            Assert.IsTrue(results.ExecutionLogs.Count() > 0);
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_GetOvalResultDocument_operation()
        {
            var token = Login();
            var ovalResultsDocument = this.ModSicClient.Service.GetOvalResultDocument("collectrequests/1", token);
            this.ModSicClient.Service.Logout(token);

            Assert.IsFalse(string.IsNullOrWhiteSpace(ovalResultsDocument));
            Assert.IsTrue(ovalResultsDocument.Contains("<oval_results"));
            Assert.IsTrue(ovalResultsDocument.Contains("</oval_results>"));
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_GetCollectRequestsInExecution_operation()
        {
            CollectInfo[] collectInfo = null;
            
            
            try
            {
                var token = Login();   
                collectInfo = this.ModSicClient.Service.GetCollectRequestsInExecution(token);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsNotNull(collectInfo);
            Assert.IsTrue(collectInfo.Count() > 0);
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_CancelCollect_operation()
        {
            bool cancelOperationResult = true;
            try
            {
                var token = Login();
                cancelOperationResult = ModSicClient.Service.CancelCollect("collectrequests/2", token);
                ModSicClient.Service.Logout(token);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsFalse(cancelOperationResult);
        }

        [TestMethod, Owner("modSIC Team")]
        public void Should_be_possible_call_ScheduleCollection_operation()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var aread = new StreamReader(currentAssembly.GetManifestResourceStream("ModSicApiTests.oval-sample.xml"));
            var ovalDefinitions = aread.ReadToEnd();
            var certificate = GetCertificate();

            var requestResult = this.ModSicClient.ScheduleCollection(ovalDefinitions, certificate);

            Assert.IsNotNull(requestResult);
            Assert.IsFalse(requestResult.HasErrors);
            Assert.IsTrue(string.IsNullOrEmpty(requestResult.Message));
            Assert.AreEqual(1, requestResult.Requests.Count());
        }



        private String Login()
        {
            return this.ModSicClient.Service.Login("admin", "Pa$$w@rd");
        }










        private X509Certificate2 GetCertificate()
        {
            try
            {
                var token = Login();
                var collectorCertificateInBytes = this.ModSicClient.Service.GetCertificate(token);
                Assert.IsNotNull(collectorCertificateInBytes);

                this.ModSicClient.Service.Logout(token);

                return new X509Certificate2(collectorCertificateInBytes);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return null;
            }
        }
    }

}
