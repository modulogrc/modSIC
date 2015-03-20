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
using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.WMI;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.Probe.Independent.TextFileContent54;
using Modulo.Collect.OVAL.Definitions.Independent;

namespace Modulo.Collect.Probe.Windows.TextFileContent54
{
    /// <summary>
    /// The textfilecontent54_object element is used by a textfilecontent_test to define the specific block(s) of text of a file(s) to be evaluated.
    /// The textfilecontent54_object will only collect regular files on UNIX systems and FILE_TYPE_DISK files on Windows systems.
    /// Each object extends the standard ObjectType as definied in the oval-definitions-schema and one should refer to the ObjectType description for more information.
    /// The common set element allows complex objects to be created using filters and set logic. Again, please refer to the description of the set element in the oval-definitions-schema.
    /// The set of files to be evaluated may be identified with either a complete filepath or a path and filename. Only one of these options may be selected.
    /// It is important to note that the 'max_depth' and 'recurse_direction' attributes of the 'behaviors' 
    /// element do not apply to the 'filepath' element, only to the 'path' and 'filename' elements.  
    /// This is because the 'filepath' element represents an absolute path to a particular file and it is not possible to recurse over a file.
    /// </summary>
    
    [ProbeCapability(OvalObject = "textfilecontent54", PlataformName = FamilyEnumeration.windows)]
    public class TextFileContentProberWindows: ProbeBase, IProbe
    {
        private TargetInfo TargetInfo = null;
        private BaseObjectCollector FileDataSource;
        private IConnectionProvider FileConnectionProvider;
        
        
        
        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            var setElement = ((textfilecontent54_object)objectType).GetItemValue(textfilecontent54_ItemsChoices.set);
            return (Definitions.set)setElement;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = this.ConnectionManager.Connect<StraightNetworkConnectionProvider>(connectionContext, target);
            this.FileConnectionProvider = this.ConnectionManager.Connect<FileConnectionProvider>(connectionContext, target);    
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var connectionScope = ((FileConnectionProvider)FileConnectionProvider).ConnectionScope;
                var newWmiDataProvider = new WmiDataProvider(connectionScope);
                var newFileProvider = new WindowsFileProvider(TargetInfo) { WmiDataProvider = newWmiDataProvider };

                ObjectCollector = new TextFileContentObjectCollector() { FileContentProvider = newFileProvider, TargetInfo = TargetInfo };
                FileDataSource = new FileObjectCollector() { WmiDataProvider = newWmiDataProvider };
            }

            if (base.ItemTypeGenerator == null)
            {
                var newFileProvider =
                    new WindowsFileProvider(this.TargetInfo)
                    {
                        WmiDataProvider = 
                            WmiDataProviderFactory
                                .CreateWmiDataProviderForFileSearching(this.TargetInfo)
                    };

                var newOperationEvaluator =
                    new TextFileContentEntityOperationEvaluator(ObjectCollector, newFileProvider, FamilyEnumeration.windows);
                
                ItemTypeGenerator = new TextFileContentItemTypeGenerator() 
                { 
                    OperationEvaluator = newOperationEvaluator 
                };
            }
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<textfilecontent54_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new textfilecontent_item() { status = StatusEnumeration.error, message = PrepareErrorMessage(errorMessage) };
        }
    }
}
