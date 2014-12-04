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

namespace Modulo.Collect.OVAL.Tests.helpers
{
    public class InvalidOvalSchemaException : Exception
    {
        public IEnumerable<String> OnDocumentLoadingErrors { get; private set; }

        public InvalidOvalSchemaException(IEnumerable<string> errors)
        {
            this.OnDocumentLoadingErrors = errors;
        }
    }

    public class OnLoadingOvalDocumentException : Exception
    {
        public OnLoadingOvalDocumentException(string exceptionMessage) : base(exceptionMessage) { }
    }

    public class OvalDocumentLoader
    {

        public oval_definitions GetFakeOvalDefinitions(string fileName)
        {
            string fileNameOnly = fileName.Replace("oval_definitions.", string.Empty);
            var sampleDoc = GetStreamFrom(fileNameOnly,"oval_definitions");

            IEnumerable<string> errors;
            oval_definitions ovalDefinitions;
            
            try
            {
                ovalDefinitions = oval_definitions.GetOvalDefinitionsFromStream(sampleDoc, out errors);
            }
            catch (Exception ex)
            {
                throw new OnLoadingOvalDocumentException(ex.Message);
            }

            if (errors.Count() > 0)
                throw new InvalidOvalSchemaException(errors);

            return ovalDefinitions;
        }

        public oval_system_characteristics GetFakeOvalSystemCharacteristics(string fileName)
        {
            string fileNameOnly = fileName.Replace("system_characteristics.", string.Empty);
            var sampleDoc = this.GetStreamFrom(fileNameOnly,"system_characteristics");
            IEnumerable<string> errors;
            oval_system_characteristics ovalSystemCharacteristics;

            try
            {
                ovalSystemCharacteristics = oval_system_characteristics.GetOvalSystemCharacteristicsFromStream(sampleDoc, out errors);
            }
            catch (Exception ex)
            {
                throw new OnLoadingOvalDocumentException(ex.Message);
            }

            if (errors.Count() > 0)
                throw new InvalidOvalSchemaException(errors);
            
            return ovalSystemCharacteristics;
        }

        private System.IO.Stream GetStreamFrom(string fileNameOnly,string directoryName)
        {
            string pathFile = string.Format("{0}.{1}.{2}", GetType().Assembly.GetName().Name, directoryName, fileNameOnly);
            var sampleDoc = GetType().Assembly.GetManifestResourceStream(pathFile);
            return sampleDoc;
        }

    }
}
