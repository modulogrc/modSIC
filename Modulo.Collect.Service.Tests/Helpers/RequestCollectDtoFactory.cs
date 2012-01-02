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
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Modulo.Collect.Service.Contract;
using System;
using Modulo.Collect.OVAL.Definitions;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Security.Cryptography.X509Certificates;

namespace Modulo.Collect.Service.Tests.Helpers
{
    public class CollectRequestDtoFactory
    {
        

        public static Request CreateCollectRequestDTO(string address)
        {
            Request collectRequest = new Request();
            collectRequest.RequestId = "256";
            collectRequest.Address = address;
            collectRequest.Credential = getCredential();
            collectRequest.TargetParameters = getTargetParameters();       

            return collectRequest;
        }

        public static Package CreateCollectPackageDTO(string address)
        {
            Package collectPackage = CreateCollectPackageDTO();
            collectPackage.CollectRequests = new Request[] { CreateCollectRequestDTO(address) };
            collectPackage.ScheduleInformation = new ScheduleInformation() { ScheduleDate = DateTime.Now };
            collectPackage.Definitions = new DefinitionInfo[] { new DefinitionInfo() { Id = "def1", Text = GetOvalDefinitions()}};
            collectPackage.CollectRequests.First().DefinitionId = "def1";
            return collectPackage;
        }

        public static Package CreateCollectPackageDTO()
        {
            Package collectPackage = new Package();            
            collectPackage.ScheduleInformation = new ScheduleInformation() { ScheduleDate = DateTime.Now };
            return collectPackage;
        }

        private static string getCredential()
        {
            return new CredentialFactory().GetEncryptCredentialInString();
        }

        private static Dictionary<string, string> getTargetParameters()
        {
            Dictionary<string, string> targetParameters = new Dictionary<string, string>();
            targetParameters.Add("instance", "SQLExpress");

            return targetParameters;
        }        

        private static string GetOvalDefinitions()
        {
            StringBuilder ovalDefinitions = new StringBuilder();
            ovalDefinitions.AppendLine("<oval_definitions xsi:schemaLocation=\"http://oval.mitre.org/XMLSchema/oval-definitions-5#windows windows-definitions-schema.xsd http://oval.mitre.org/XMLSchema/oval-definitions-5 oval-definitions-schema.xsd http://oval.mitre.org/XMLSchema/oval-common-5 oval-common-schema.xsd http://oval.mitre.org/XMLSchema/oval-definitions-5#independent independent-definitions-schema.xsd\" xmlns=\"http://oval.mitre.org/XMLSchema/oval-definitions-5\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:oval=\"http://oval.mitre.org/XMLSchema/oval-common-5\" xmlns:oval-def=\"http://oval.mitre.org/XMLSchema/oval-definitions-5\">");
            ovalDefinitions.AppendLine("    <generator>");
            ovalDefinitions.AppendLine("        <oval:product_name>Risk Manager</oval:product_name>");
            ovalDefinitions.AppendLine("        <oval:product_version>7.0</oval:product_version>");
            ovalDefinitions.AppendLine("        <oval:schema_version>5.9</oval:schema_version>");
            ovalDefinitions.AppendLine("        <oval:timestamp>2011-02-24T16:36:26.896-03:00</oval:timestamp>");
            ovalDefinitions.AppendLine("        <vendor>Modulo Security</vendor>");
            ovalDefinitions.AppendLine("    </generator>");
            ovalDefinitions.AppendLine("    <objects>");
            ovalDefinitions.AppendLine("        <registry_object id=\"oval:modulo:obj:9838\" version=\"1\" xmlns=\"http://oval.mitre.org/XMLSchema/oval-definitions-5#windows\">");
            ovalDefinitions.AppendLine("            <hive>HKEY_LOCAL_MACHINE</hive>");
            ovalDefinitions.AppendLine("            <key>System\\CurrentControlSet\\Control\\Lsa</key>");
            ovalDefinitions.AppendLine("            <name>AuditBaseObjects</name>");
            ovalDefinitions.AppendLine("        </registry_object>");
            ovalDefinitions.AppendLine("    </objects>");
            ovalDefinitions.AppendLine("</oval_definitions>");
            return ovalDefinitions.ToString();                        
            
        }     


    }
}
