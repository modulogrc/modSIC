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


using System.Collections.Generic;
using System.Linq;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.WMI;

using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Independent;


namespace Modulo.Collect.Probe.Windows.FileContent
{
    /// <summary>
    /// The textfilecontent_object element is used by a text file content test to define the specific line(s) of a file(s) to be evaluated. 
    /// The textfilecontent_object will only collect regular files on UNIX systems and FILE_TYPE_DISK files on Windows systems. 
    /// Each object extends the standard ObjectType as definied in the oval-definitions-schema and one should refer to the ObjectType description for more information. 
    /// The common set element allows complex objects to be created using filters and set logic. 
    /// Again, please refer to the description of the set element in the oval-definitions-schema.
    /// <oval:deprecated_info>
    /// <oval:version>5.4</oval:version>
    /// <oval:reason>
    ///     Replaced by the textfilecontent54_object. Support for multi-line pattern matching and multi-instance matching was added. 
    ///     Therefore, a new object was created to reflect these changes. See the textfilecontent54_object.
    /// </oval:reason>
    /// <oval:comment>This object has been deprecated and will be removed in version 6.0 of the language.</oval:comment>
    /// </summary>
    [ProbeCapability(OvalObject = "textfilecontent", PlataformName = FamilyEnumeration.windows)]
    public class FileContentProber : ProbeBase, IProbe
    {
        private TargetInfo TargetInfo;

        private BaseObjectCollector FileDataSource;
        
        private IConnectionProvider FileConnectionProvider;
        

        public WmiDataProvider WmiDataProvider { get; set; }


        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            this.ConnectionProvider = base.ConnectionManager.Connect<StraightNetworkConnectionProvider>(connectionContext, target);
            this.FileConnectionProvider = base.ConnectionManager.Connect<FileConnectionProvider>(connectionContext, target);            
        }

        protected override void ConfigureObjectCollector()
        {
            this.CreateObjectCollector();
            this.CreateItemTypeGenerator();
        }


        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<textfilecontent_object>();
        }

        protected override set GetSetElement(Definitions.ObjectType objectType) 	
        {
            var setElement = ((textfilecontent_object)objectType).GetItemValue(textfilecontent_ItemsChoices.set);
            return (set)setElement;
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new textfilecontent_item() { status = StatusEnumeration.error, message = PrepareErrorMessage(errorMessage) };
        }


        private void CreateObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                base.ObjectCollector = new FileContentSystemDataSource(TargetInfo.GetAddress());

                var wmiConnectionScope =
                this.FileDataSource = new FileObjectCollector() { WmiDataProvider = CreateWmiDataProvider() };
            }
        }

        private void CreateItemTypeGenerator()
        {
            if (base.ItemTypeGenerator == null)
            {
                base.ItemTypeGenerator = new FileContentItemTypeGenerator()
                {
                    FileContentSystemDataSource = base.ObjectCollector,
                    FileDataSource = this.FileDataSource,
                    WindowsFileProvider = CreateWinFileProvider()
                };
            }
        }


        private WmiDataProvider CreateWmiDataProvider()
        {
            var currentConnectionScope = ((FileConnectionProvider)FileConnectionProvider).ConnectionScope;
            return new WmiDataProvider(currentConnectionScope);
        }

        private WindowsFileProvider CreateWinFileProvider()
        {
            return new WindowsFileProvider(TargetInfo) { WmiDataProvider = CreateWmiDataProvider() };
        }
    }
}
