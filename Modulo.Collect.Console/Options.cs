#region License
/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace Modulo.Collect.ClientConsole
{
    class Options
    {
        #region Standard Option Attribute
        [Option("m", "modsic",
                Required = false,
                HelpText = "modSIC server address and port (address:port)")]
        public string ServerAddress = String.Empty;

        [Option("u", "username",
                Required = false,
                HelpText = "modSIC username")]
        public string ServerUsername = String.Empty;

        [Option("p", "password",
                Required = false,
                HelpText = "modSIC password")]
        public string ServerPassword = String.Empty;

        [Option("t", "target",
                Required = false,
                HelpText = "Target address")]
        public string TargetAddress = String.Empty;

        [Option("y", "target-username",
                Required = false,
                HelpText = "Target username")]
        public string TargetUsername = String.Empty;

        [Option("z", "target-password",
                Required = false,
                HelpText = "Target password")]
        public string TargetPassword = String.Empty;

        [Option("c", "collect",
                HelpText = "Send to collect (asynchronous)")]
        public bool SendCollect = false;

        [Option("s", "collect-sync",
                HelpText = "Send to collect (synchronous). Wait for collect to finish")]
        public bool SendCollectSync = false;

        [Option("g", "get-results",
                Required = false,
                HelpText = "Request collect results")]
        public string RequestId = String.Empty;

        [Option("x", "cancel",
                HelpText = "Cancel collect")]
        public string CancelRequestId = String.Empty;

        [Option("l", "list",
                HelpText = "List collects in execution requested by this client")]
        public bool ListCollectsInExecution = false;

        [Option("q", "query-all",
                HelpText = "List collects in execution requested by all clients")]
        public bool ListAllCollectsInExecution = false;

        [Option("o", "oval-definition",
                Required = false,
                HelpText = "Path to the oval-definitions xml file (default=\"definitions.xml\").")]
        public string OvalDefinitionFilename = String.Empty;

        [Option("v", "external-variable",
                Required = false,
                HelpText = "Path to external variable values file (default=\"external-variables.xml\").")]
        public string ExternalVariableFilename = String.Empty;

        [Option("a", "oval-schema",
                Required = false,
                HelpText = "Path to the directory that contains the OVAL schema (default=\"xml\").")]
        public string OvalSchemaPath = String.Empty;

        [Option("d", "system-characteristics",
        Required = false,
        HelpText = "Save data to the specified XML file (default=\"system-characteristics.xml\").")]
        public string SystemCharacteristicsFilename = String.Empty;

        [Option("r", "result",
                Required = false,
                HelpText = "Save results to the specified XML file (default=\"results.xml\").")]
        public string ResultsFilename = String.Empty;

        [Option("n", "schematron",
                Required = false,
                HelpText = "Perform Schematron validation of the oval-definitions file.")]
        public bool ValidateSchematron = false;

        [Option("e", "preset",
        Required = false,
        HelpText = "Use preset parameters to send a collect.")]
        public string Preset = String.Empty;

        [Option("w", "administrative-password", 
                Required=false, 
                HelpText="Root or super user password")]
        public String TargetAdministrativePassword = null;

        [Option("b", "shh-port",
                Required = false,
                HelpText = "SSH port for Unix systems")]
        public String SSHPort = null;

        [Option("", "verbose",
                Required = false,
                HelpText = "Display additional information in -l and -q")]
        public bool VerboseOutput = false;

        #endregion

        #region Specialized Option Attribute
        [ValueList(typeof(List<string>), MaximumElements = 0)]
        public IList<string> Items = null;
        

        [HelpOption(
                HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            HeadingInfo _headingInfo = new HeadingInfo(programName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var help = new HelpText(_headingInfo);
            help.AdditionalNewLineAfterOption = true;
            help.Copyright = new CopyrightInfo("Modulo Security Solutions", 2011, 2011);
            help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
            help.AddPreOptionsLine("the License <http://modsic.codeplex.com/license>.");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("Usage: {0} -c -m <server> -u <username> -p <password> -t <target> -y <target-username> -z <target-password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} --collect --modsic <server> --username <username> --password <password> --target <target> --target-username <target-username> --target-password <target-password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} -s -m <server> -u <username> -p <password> -t <target> -y <target-username> -z <target-password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} --collect-sync --modsic <server> --username <username> --password <password> --target <target> --target-username <target-username> --target-password <target-password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} -g <id> -m <server> -u <username> -p <password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} --get-results <id> --modsic <server> --username <username> --password <password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} -x <id> -m <server> -u <username> -p <password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} --cancel <id> --modsic <server> --username <username> --password <password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} -l -m <server> -u <username> -p <password>", programName));
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("       {0} --list --modsic <server> --username <username> --password <password>", programName));
            help.AddOptions(this);

            return help;
        }
        #endregion
    }
}
