using System.Configuration;
using System.Linq;
using mattgwagner.SharpSsh.wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SharpSSH.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        string host = ConfigurationManager.AppSettings["SSH_Host"];
        string user = ConfigurationManager.AppSettings["SSH_User"];
        string pass = ConfigurationManager.AppSettings["SSH_Pass"];

        [TestMethod]
        public void TestListFiles()
        {
            var ssh = new SFTPUtil(host, user, pass);

           var list = ssh.ListFiles("/home/mattgwagner").ToList();

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void TestGetFile()
        {
            var ssh = new SFTPUtil(host, user, pass);

            ssh.GetFile("/home/mattgwagner/.bash_profile", "c://");
        }

        [TestMethod]
        public void TestDeleteFile()
        {
            var ssh = new SFTPUtil(host, user, pass);

            ssh.DeleteFile("/home/mattgwagner/Test.TXT");
        }

        [TestMethod]
        public void TestPutFile()
        {
            var ssh = new SFTPUtil(host, user, pass);

            ssh.PutFile("c://Test.txt", "/home/mattgwagner/Test.TXT");
        }

        [TestMethod]
        public void TestGetLotsOfFiles()
        {
            var ssh = new SFTPUtil(host, user, pass);

            var remote = "/home/mattgwagner/main/public/wp-admin/images/";

            ssh.GetLotsOfFiles(remote, @"z:\");
        }
    }
}
