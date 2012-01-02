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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Definitions =Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Probe.Independent.Common.File;


namespace Modulo.Collect.Probe.Windows.FileContent.operation
{
    public class FileContentEntityOperationEvaluator
    {
        private BaseObjectCollector systemDataSource;        
        private PathOperatorEvaluator pathOperatorEvaluator;

        public FileContentEntityOperationEvaluator(BaseObjectCollector systemDataSource, IFileProvider fileProvider)
        {
            this.systemDataSource = systemDataSource;
            pathOperatorEvaluator = new PathOperatorEvaluator(fileProvider, FamilyEnumeration.windows);
        }

        public IEnumerable<ItemType> ProcessOperation(IEnumerable<Definitions::ObjectType> fileContentObjects)
        {
            List<ItemType> items = new List<ItemType>();
            foreach (Definitions::ObjectType item in fileContentObjects)
            {
                items.AddRange(ProcessOperation((textfilecontent_object) item));
            }

            return items;
        }

        public IEnumerable<ItemType> ProcessOperation(textfilecontent_object fileContentObject)
        {
            List<ItemType> itemTypes = new List<ItemType>();
            Dictionary<string, EntityObjectStringType> fileEntities = FileContentOvalHelper.GetFileContentEntitiesFromObjectType(fileContentObject);
            
            IEnumerable<string> paths = this.ProcessOperationsPaths(fileEntities);
            IEnumerable<string> fileNames = this.ProcessOperationsFileNames(fileEntities, paths);
            IEnumerable<string> lines = this.ProcessOperationsLine(fileEntities);

            return new FileContentItemTypeFactory().CreateFileItemTypesByCombinationOfEntitiesFrom(paths, fileNames, lines);
        }

        private IEnumerable<string> ProcessOperationsLine(Dictionary<string, EntityObjectStringType> fileEntities)
        {
            EntityObjectStringType line;
            List<string> lines = new List<string>();

            if (fileEntities.TryGetValue("line", out line))
                lines.Add(line.Value);

            return lines;
        }

        private IEnumerable<string> ProcessOperationsPaths(Dictionary<string, EntityObjectStringType> fileEntities)
        {
            return pathOperatorEvaluator.ProcessOperationsPaths(fileEntities);
        }

        private IEnumerable<string> ProcessOperationsFileNames(Dictionary<string, EntityObjectStringType> fileNames, IEnumerable<string> paths)
        {
            return pathOperatorEvaluator.ProcessOperationFileName(fileNames,paths,false);
        }
    }
}
