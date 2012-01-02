//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Modulo.Collect.Service.Contract;
//using Modulo.Collect.Service.Controllers;
//using Modulo.Collect.Service.Data;
//using DevExpress.Xpo;
//using Modulo.Collect.Service.Entities;
//using Modulo.Collect.Probe.Common;
//using Modulo.Collect.OVAL.Common;
//using System.Reflection;
//using System.Security.Cryptography.X509Certificates;
//using Modulo.Collect.Service.Security;
//using Modulo.Collect.Service.Contract.Security;

//namespace ModuloOvalInterpreter
//{
//    public class InterpreterController
//    {
//        private DataProvider provider;
//        private Session session;

//        public InterpreterController()
//        {
//            //provider = new DataProvider();
//            session = XpoDefault.Session;// provider.GetSession();
//        }

//        public String ExecuteCollect(Package collectPackage)
//        {

//            CollectController collectController = new Col();            
//            Dictionary<string, string> results = collectController.CollectRequest(collectPackage);
//            string id = results.First().Value;
//            //this.Execute(id);
//            return id;
//        }


//        public String ExecuteCollect(string id)
//        {            
//            this.Execute(id);
//            return id;
//        }

//        private void Execute(string id)
//        {

//            Package requestCollect = this.GetRequestCollect(id);

//            //TargetInfo targetInfo = new TargetInfo();
//            //targetInfo.Add("IPAddr", requestCollect.CollectRequests);
//            //targetInfo.credentials = new Credentials();



//            X509Certificate2 certificate = new CertificateFactory().GetCertificate();
//            Credential credential = new CollectServiceCryptoProvider().DecryptCredentialBasedOnCertificateOfServer(/*take bytes of certificate*/,
//                                                                                                                      certificate);

//            targetInfo.credentials.Add("UserName", credential.UserName);
//            targetInfo.credentials.Add("Password", credential.Password);
//            targetInfo.credentials.Add("Domain", "mss");
            
//            CollectExecutionManager executionManager = new CollectExecutionManager(new List<IConnectionProvider>(), targetInfo);
//            IProbeManager probeManager = new ProbeManager();
//            probeManager.LoadProbes("");
//            executionManager.DataProvider = provider;
//            executionManager.ProbeManager = probeManager;

//            executionManager.ExecuteCollect(requestCollect, FamilyEnumeration.windows);

//        }

//        public Request GetRequestCollect(string id)
//        {
//            Request requestCollect = XpoDefault.Session.GetObjectByKey<Request>(new Guid(id));
//            return requestCollect;
//        }

//        public XPCollection<Request> GetAllRequestCollect()
//        {
//            return new XPCollection<Request>(XpoDefault.Session);
//        }

//        public bool RemoveAllRequestCollect() // Refactor to delete with raven
//        {
//            //var requestCollects = new XPCollection(session, typeof(Request));
//            //List<string> idsOfRequestCollect = this.GetListOfIds(requestCollects);
//            //try
//            //{
//            //    session.BeginTransaction();
//            //    foreach (string id in idsOfRequestCollect)
//            //    {
//            //        var collect = this.GetRequestCollect(id);
//            //        collect.Delete();
//            //    }
//            //    session.CommitTransaction();
//            //    return true;

//            //}
//            //catch (Exception ex)
//            //{
//            //    session.RollbackTransaction();
//            //    throw ex;
//            //}
//            return true;
//        }

//        private List<string> GetListOfIds(XPCollection requestCollects)
//        {
//            List<string> ids = new List<string>();
//            foreach (Request requestCollect in requestCollects)
//            {
//                ids.Add(requestCollect.RequestId.ToString());
//            }
//            return ids;
//        }
//    }
//}
