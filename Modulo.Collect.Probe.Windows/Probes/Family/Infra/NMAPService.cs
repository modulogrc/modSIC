/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
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
using System.Diagnostics;
using System.IO;
using System.Xml;


namespace Modulo.Collect.Probe.Windows.Family
{
    public class NMAPService
    {
        private string TargetHostName;
        private string NMAPExecutableName;

        public NMAPService(string targetHostname, string nmapExecutableName)
        {
            this.TargetHostName = targetHostname;
            this.NMAPExecutableName = nmapExecutableName;
        }

        public virtual string GetOSInformation()
        {
            string tempFilePath = this.getTempFilePath();
            this.startNMapProcess(tempFilePath).WaitForExit();
            XmlNodeList OSClassNodeList = this.getOSClassNodeList(tempFilePath);
            OSScanResult OSScanResults = getBestAccurrancy(OSClassNodeList);

            return this.GetSimpleOSName(OSScanResults.Best.ToString());
        }


        private string getTempFilePath()
        {
            return String.Format("{0}\\osprobe-{1}.xml", Path.GetTempPath(), Process.GetCurrentProcess().Id);
        }

        private Process startNMapProcess(string tempFilePath)
        {
            string arguments = String.Format("-F -O -oX {0} -v {1}", tempFilePath, this.TargetHostName);
            var processInfo = new ProcessStartInfo(this.NMAPExecutableName, arguments) { WindowStyle = ProcessWindowStyle.Hidden };
            return Process.Start(processInfo);
        }

        private XmlNodeList getOSClassNodeList(string tempFilePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(tempFilePath);

            new FileInfo(tempFilePath).Delete();

            return xmlDocument.DocumentElement.SelectNodes("/nmaprun/host/os/osclass");
        }        

        public string GetSimpleOSName(string foundOSName)
        {
            bool isWindowsFamily = foundOSName.IndexOf("windows", StringComparison.InvariantCultureIgnoreCase) >= 0;
            return isWindowsFamily ? "windows" : foundOSName;
        }

        private OSScanResult getBestAccurrancy(XmlNodeList OSClassNodeList)
        {
            int bestAccuracy = 0;
            OSScanResult OSScanResults = new OSScanResult();

            foreach (XmlNode OSClassNode in OSClassNodeList)
            {
                var accuracy = int.Parse(OSClassNode.Attributes["accuracy"].Value);
                var vendor = OSClassNode.Attributes["vendor"].Value;
                var family = OSClassNode.Attributes["osfamily"].Value;
                var OSGuess = new OSGuess() { Accuracy = accuracy, Vendor = vendor, OSFamily = family };
                bestAccuracy = this.calculateBestAccurancy(OSScanResults, OSClassNode, OSGuess, bestAccuracy);
            }

            return OSScanResults;
        }

        private int calculateBestAccurancy(OSScanResult OSScanResults, XmlNode OSClassNode, OSGuess OSGuess, int bestAccuracy)
        {
            XmlAttribute attOSGen = OSClassNode.Attributes["osgen"];
            OSGuess.OSGen = (attOSGen == null) ? null : attOSGen.Value;
            OSScanResults.Guesses.Add(OSGuess);

            if (OSGuess.Accuracy > bestAccuracy)
            {
                OSScanResults.Best = OSGuess;
                return OSGuess.Accuracy;
            }

            return bestAccuracy;

        }
    }
}
