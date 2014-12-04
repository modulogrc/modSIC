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
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.Probe.Common.Test.Helpers
{
    public class LoadOvalDocument
    {
        private const string OVAL_DEFINITIONS_PROJECT_FILEPATH = "Repository.oval_definitions";
        private const string OVAL_SYS_CHARACTERISTICS_PROJECT_FILEPATH = "Repository.system_characteristics";

        public oval_definitions GetFakeOvalDefinitions(string fileName)
        {
            var fileNameOnly = fileName.Replace(OVAL_DEFINITIONS_PROJECT_FILEPATH + ".", string.Empty);
            var sampleDoc = GetStreamFrom(fileNameOnly, OVAL_DEFINITIONS_PROJECT_FILEPATH);

            IEnumerable<string> errors;
            return oval_definitions.GetOvalDefinitionsFromStream(sampleDoc, out errors);
        }

        public oval_system_characteristics GetFakeOvalSystemCharacteristics(string fileName)
        {
            var fileNameOnly = fileName.Replace(OVAL_SYS_CHARACTERISTICS_PROJECT_FILEPATH + ".", string.Empty);
            var sampleDoc = this.GetStreamFrom(fileNameOnly, OVAL_SYS_CHARACTERISTICS_PROJECT_FILEPATH);
            IEnumerable<string> errors;
            return oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);
        }

        private System.IO.Stream GetStreamFrom(string fileNameOnly,string directoryName)
        {
            var assemblyName = GetType().Assembly.GetName().Name;
            var pathFile = string.Format("{0}.{1}.{2}", assemblyName, directoryName, fileNameOnly);
            
            return GetType().Assembly.GetManifestResourceStream(pathFile);
        }

    }
}
