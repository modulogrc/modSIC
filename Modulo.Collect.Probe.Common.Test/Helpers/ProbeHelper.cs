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
using System.Linq;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common.BasicClasses;
using Definitions = Modulo.Collect.OVAL.Definitions;

namespace Modulo.Collect.Probe.Common.Test.Helpers
{
    public class ProbeHelper
    {

        public static TargetInfo CreateFakeTarget(
            string hostname = null, string domain = null, string username = null, string password = null)
        {
            var fakeUsername = username ?? "joselito";
            var fakePassword = password ?? "123";
            var fakeDomain = domain ?? "D";
            var fakeHostname = hostname ?? "remotehost";
            
            var fakeCredentials = new Credentials(fakeDomain, fakeUsername, fakePassword, null);

            return new TargetInfoFactory(fakeHostname, fakeDomain, fakeUsername, fakePassword).Create();
        }

        public static List<IConnectionProvider> CreateFakeContext()
        {
            return new List<IConnectionProvider>();
        }

        public static oval_definitions GetFakeOvalDefinitions(string fileName)
        {
            if (!fileName.Contains(".xml"))
                fileName += ".xml";

            LoadOvalDocument ovalDocumentLoader = new LoadOvalDocument();
            return ovalDocumentLoader.GetFakeOvalDefinitions(fileName);
        }

        public static Definitions.ObjectType GetOvalComponentByOvalID(oval_definitions definitions, string ovalComponentID)
        {
            string ovalID = ovalComponentID.Contains(":") ? ovalComponentID : string.Format("oval:modulo:obj:{0}", ovalComponentID);
            return definitions.objects.Single(obj => obj.id.Equals(ovalID));
        }

        public static Definitions.ObjectType GetDefinitionObjectTypeByID(string definitionsFileName, string objectTypeID)
        {
            oval_definitions definitions = ProbeHelper.GetFakeOvalDefinitions(definitionsFileName);
            return GetOvalComponentByOvalID(definitions, objectTypeID);
        }

        public static CollectInfo CreateFakeCollectInfo(
            IEnumerable<Definitions.ObjectType> objects, 
            VariablesEvaluated variables = null, 
            oval_system_characteristics systemCharacteristics = null)
        {
            CollectInfo collectInfo = new CollectInfo();
            collectInfo.Variables = (variables == null) ? new VariablesEvaluated(new List<VariableValue>()) : variables;
            collectInfo.ObjectTypes = objects;
            collectInfo.SystemCharacteristics = systemCharacteristics;

            return collectInfo;
        }

        public static CollectedItem CreateFakeCollectedItem(ItemType fakeItemType)
        {
            return new CollectedItem() { ItemType = fakeItemType, ExecutionLog = new List<ProbeLogItem>() };
        }

        public static oval_system_characteristics GetOvalSystemCharacteristicsFromFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;

            if (!filename.Contains(".xml"))
                filename += ".xml";
            
            return new LoadOvalDocument().GetFakeOvalSystemCharacteristics(filename);
        }
    }
}
