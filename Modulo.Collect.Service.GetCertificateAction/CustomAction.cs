/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Diagnostics;
using System.Reflection;

namespace Modulo.Collect.Service.CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult UpdateSummary(Session session)
        {
            try
            {
                var text = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fswiss\fcharset0 Arial;}}
{\*\generator Msftedit 5.41.21.2510;}\viewkind4\uc1\pard\f0\fs16 modSIC installation completed successfully\par
\par
The service is running under\par
Address: #0#\par\par
Please take note of the following username and password to access the service\par
Username: admin\par
Password: Pa\$\$w@rd\par
}";
                text = text.Replace("#0#", session["BASEADDRESS"]);
                var sql = String.Format("INSERT INTO `Control` (`Dialog_`, `Control`, `Type`, `X`, `Y`, `Width`, `Height`, `Attributes`, `Text`) VALUES('ExitDlg', 'txtSummary', 'ScrollableText', 11, 56, 350, 166, 3, '{0}') TEMPORARY", text);
                session.Database.Execute(sql);
            }
            catch(Exception ex)
            {
                session.Log(ex.Message);
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult UpdateFQDN(Session session)
        {
            string hostName;
            string fqdn = "localhost";

            try
            {
                session["ADDRESS"] = "";

                fqdn = hostName = Dns.GetHostName();
                string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;

                if (!String.IsNullOrEmpty(domainName) && !hostName.Contains(domainName))
                {
                    IPAddress[] addresslist = Dns.GetHostAddresses(hostName + "." + domainName);
                    if (addresslist.Length > 0)
                        fqdn = hostName + "." + domainName;
                }
                else
                {
                    fqdn = hostName;
                }
            }
            catch (Exception ex)
            {
                session.Log(ex.Message);
            }
            finally
            {
                session["ADDRESS"] = fqdn.ToLower();
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CheckPort(Session session)
        {
            try
            {
                int port = 0;

                if(Int32.TryParse(session["SERVICEPORT"].Trim(), out port))
                {
                    session["SERVICEPORT"] = port.ToString();
                }
                if(port < 1024 || port > 65535)
                {
                    MessageBox.Show("Please, choose a port from the 49152–65535 range. If you really need to choose a port within the range 1024–49151 make sure you pick an \"unassigned\" port",
                                session["ProductName"] ?? "modSIC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    session["VALIDPORT"] = "0";
                }
                else
                {
                    session["VALIDPORT"] = "1";
                }

            }
            catch(Exception ex)
            {
                session["VALIDPORT"] = "0";
                session.Log(ex.Message);
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CheckAddress(Session session)
        {
            try
            {
                session["ADDRESS"] = session["ADDRESS"].Trim();

                string re = @"^[a-zA-Z0-9][a-zA-Z0-9\-._]*$";
                Regex r = new Regex(re, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = r.Match(session["ADDRESS"]);

                if(string.IsNullOrEmpty(session["ADDRESS"]) || !m.Success)
                {
                    MessageBox.Show("Please, check the address", session["ProductName"] ?? "modSIC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    session["VALIDADDRESS"] = "0";
                }
                else
                {
                    session["VALIDADDRESS"] = "1";
                }

            }
            catch(Exception ex)
            {
                session.Log(ex.Message);
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult GetThumbPrint(Session session)
        {
            var certpath = session["CERTPATH"].ToString();
            var password = session["CERTPASS"].ToString();

            if(string.IsNullOrEmpty(certpath.Trim()))
            {
                MessageBox.Show("Please, select the certificate file path correctly.",
                               session["ProductName"] ?? "modSIC",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error
                              );
                return ActionResult.Success;
            }

            try
            {
                X509Certificate certificate = new X509Certificate(certpath, password);
                session["CERTHASHCODE"] = certificate.GetCertHashString();
                session["VALIDCERTIFICATE"] = "1";
                return ActionResult.Success;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, session["ProductName"] ?? "modSIC", MessageBoxButtons.OK, MessageBoxIcon.Error);

                session.Log(ex.Message);
                session["VALIDCERTIFICATE"] = "0";
                return ActionResult.Success;
            }
        }

        [CustomAction]
        public static ActionResult SelectCertificate(Session session)
        {
            try
            {
                FileBrowser fileBrowser = new FileBrowser(session);

                Thread worker = new Thread(fileBrowser.Run);
                worker.SetApartmentState(ApartmentState.STA);
                worker.Start();
                worker.Join();

                return ActionResult.Success;
            }
            catch(Exception ex)
            {
                session.Log(ex.Message);
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult InstallCertificate(Session session)
        {
            //if (System.Diagnostics.Debugger.IsAttached)
            //    System.Diagnostics.Debugger.Break();
            //else
            //    System.Diagnostics.Debugger.Launch();
          
            try
            {
                var serviceRuntimeCertificateStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                serviceRuntimeCertificateStore.Open(OpenFlags.ReadWrite);

                var tokens = session["CustomActionData"].Split("|".ToCharArray());
                var path = tokens[0];
                var password = tokens[1];

                X509Certificate2 certificate = null;
                if (path == "sample-08C0FB88-9ABF-4AB5-ADDF-221477CD2B23")
                {
                    using (var stream = GetResourceStream("modSICtestCert.pfx"))
                    {
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, (int)stream.Length);

                        certificate = new X509Certificate2(buffer, password, X509KeyStorageFlags.PersistKeySet);
                    }
                }
                else
                {
                    certificate = new X509Certificate2(path, password, X509KeyStorageFlags.PersistKeySet);
                }

                AddAccessToCertificate(certificate);

                if(!serviceRuntimeCertificateStore.Certificates.Contains(certificate))
                {
                    serviceRuntimeCertificateStore.Add(certificate);
                }
                serviceRuntimeCertificateStore.Close();
                
                return ActionResult.Success;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, session["ProductName"] ?? "modSIC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                session.Log(ex.Message);
                return ActionResult.Failure;
            }
        }

        private static Stream GetResourceStream(string filename)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Modulo.Collect.Service.CustomAction.{0}", filename));
            return stream;
        }

        private static void AddAccessToCertificate(X509Certificate2 cert)
        {
            RSACryptoServiceProvider rsa = cert.PrivateKey as RSACryptoServiceProvider;
            if (rsa == null)
            {
                return;
            }

            string keyfilepath = FindKeyLocation(rsa.CspKeyContainerInfo.UniqueKeyContainerName);

            FileInfo file = new FileInfo(System.IO.Path.Combine(keyfilepath, rsa.CspKeyContainerInfo.UniqueKeyContainerName));

            FileSecurity fs = file.GetAccessControl();

            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            fs.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.Read, AccessControlType.Allow));
            file.SetAccessControl(fs);
        }

        private static string FindKeyLocation(string keyFileName)
        {
            string pathCommAppData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Microsoft\Crypto\RSA\MachineKeys");
            string[] textArray = Directory.GetFiles(pathCommAppData, keyFileName);
            if (textArray.Length > 0) return pathCommAppData;

            string pathAppData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Crypto\RSA\");
            textArray = Directory.GetDirectories(pathAppData);
            if (textArray.Length > 0)
            {
                foreach (string str in textArray)
                {
                    textArray = Directory.GetFiles(str, keyFileName);
                    if (textArray.Length != 0) return str;
                }
            }
            return "Private key exists but is not accessible";
        }
    }
}
