using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace FrameworkNG
{
    /// <summary>
    /// === OSFPCollector ===
    /// Author: jcastro
    /// Creation Date: 28/05/2009
    /// Description: Collects OS data on remote machines via Nmap's Active OS fingerprinting.
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: Requires the Nmap executable. You can pass an alternate executable name.
    /// Example: N/A
    /// </summary>
    public class OSFPCollector : Collector
    {
        /// <summary>
        /// Gets or sets the host name.
        /// </summary>
        /// <value>The host name.</value>
        public string Hostname { get; set; }
        /// <summary>
        /// Gets or sets the nmap executable file name.
        /// </summary>
        /// <value>File name. May be a naked command name (ex. "nmap476") to be looked on the PATH,
        /// or a complete path and file name. (ex. @"C:\download\nmap-4.76\nmap.exe")</value>
        public string NmapExecutable { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OSFPCollector"/> class with default values.
        /// </summary>
        public OSFPCollector()
        {
            NmapExecutable = "nmap";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OSFPCollector"/> class.
        /// </summary>
        /// <param name="authinfo">Auth info. Won't be used in this particular subclass.
        /// This is for compatibility with the superclass.</param>
        public OSFPCollector(CollectorAuth authinfo)
            : base(authinfo)
        {
            NmapExecutable = "nmap";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OSFPCollector"/> class.
        /// </summary>
        /// <param name="nmapExecutable">Nmap executable. May be a naked command name (ex. "nmap476") to be looked on the PATH,
        /// or a complete path and file name. (ex. @"C:\download\nmap-4.76\nmap.exe")</param>
        public OSFPCollector(string nmapExecutable)
        {
            NmapExecutable = nmapExecutable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OSFPCollector"/> class.
        /// </summary>
        /// <param name="authinfo">Auth info. Won't be used in this particular subclass.
        /// This is for compatibility with the superclass.</param>
        /// <param name="nmapExecutable">Nmap executable. May be a naked command name (ex. "nmap476") to be looked on the PATH,
        /// or a complete path and file name. (ex. @"C:\download\nmap-4.76\nmap.exe")</param>
        public OSFPCollector(CollectorAuth authinfo, string nmapExecutable)
            : base(authinfo)
        {
            NmapExecutable = nmapExecutable;
        }

        /// <summary>
        /// Conect to a host in order to perform data collections. This method is mostly for compatibility with the
        /// base class's semantics. Doesn't actually connect anything; only stores the hostname in an instance property.
        /// </summary>
        /// <param name="hostname">Hostname or IP of machine to be probed.</param>
        public override void Connect(string hostname)
        {
            Hostname = hostname;
        }

        /// <summary>
        /// Performs a collection. This method is mostly for compatibility with the base class's semantics.
        /// Just calls CollectOS() with the hostname it was fed in Connect().
        /// </summary>
        /// <param name="spec">Collection spec. Not used in this version.</param>
        /// <returns>
        /// Raw result of probe. The "Data" property has a <see cref="OSScanResult"/> object.
        /// </returns>
        public override CollectResult Collect(ControlSpec spec)
        {
            CollectResult myRes = new CollectResult { Data = CollectOS(Hostname) };
            return myRes;
        }

        /// <summary>
        /// === CollectOS ===
        /// Autor: jcastro
        /// Data de Criação: 28/05/2009
        /// Descrição: Queries a host for OS information through active OS fingerprinting.
        /// Modo de Utilizar: N/A
        /// Estados ao Entrar: N/A
        /// Estados ao Sair: N/A
        /// Exceções: N/A
        /// Hipóteses: Nmap has to be installed on the local machine.
        /// Exemplo: myosguess = myOSCollector.CollectOS("10.24.69.171").Best.ToString();
        /// </summary>
        /// <param name="hostname">The host name or IP address of the target machine.</param>
        /// <returns><see cref="OSScanResult"/> object containing the OS guesses for that machine.</returns>
        public OSScanResult CollectOS(string hostname)
        {
            string tmpFile = String.Format("{0}\\osprobe-{1}.xml", System.IO.Path.GetTempPath(), Process.GetCurrentProcess().Id);
            ProcessStartInfo startinfo = new ProcessStartInfo(NmapExecutable, String.Format("-F -O -oX {0} -v {1}", tmpFile, hostname))
            {
                WindowStyle = ProcessWindowStyle.Hidden 
            };
            Process nmapper = Process.Start(startinfo);
            nmapper.WaitForExit();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(tmpFile);
            new FileInfo(tmpFile).Delete();
            XmlNodeList matches = xmldoc.DocumentElement.SelectNodes("/nmaprun/host/os/osclass");
            int bestaccuracy = 0;
            OSScanResult myResults = new OSScanResult();
            foreach (XmlNode match in matches)
            {
                OSGuess matchObj = new OSGuess {
                    Accuracy = int.Parse(match.Attributes["accuracy"].Value),
                    Vendor = match.Attributes["vendor"].Value,
                    OSFamily = match.Attributes["osfamily"].Value
                };
                if (match.Attributes["osgen"] != null)
                    matchObj.OSGen = match.Attributes["osgen"].Value;
                else
                    matchObj.OSGen = null;

                myResults.Guesses.Add(matchObj);
                if (matchObj.Accuracy > bestaccuracy)
                {
                    myResults.Best = matchObj;
                    bestaccuracy = matchObj.Accuracy;
                }
            }
            return myResults;
        }
    }
}
