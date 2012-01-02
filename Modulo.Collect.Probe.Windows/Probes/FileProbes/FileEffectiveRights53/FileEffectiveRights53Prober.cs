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
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Independent.Common;
using Modulo.Collect.Probe.Windows.File;
using Modulo.Collect.Probe.Windows.WMI;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;
using Modulo.Collect.Probe.Independent.Common.File;


namespace Modulo.Collect.Probe.Windows.FileEffectiveRights53
{
    /// <summary>
    /// The fileeffectiverights53_object element is used by a file effective rights test to define the objects used to evalutate against the specified state. 
    /// The fileeffectiverights53_object will collect directories and all Windows file types (FILE_TYPE_CHAR, FILE_TYPE_DISK, FILE_TYPE_PIPE, FILE_TYPE_REMOTE, and FILE_TYPE_UNKNOWN). 
    /// Each object extends the standard ObjectType as definied in the oval-definitions-schema and one should refer to the ObjectType description for more information. 
    /// The common set element allows complex objects to be created using filters and set logic.
    /// A fileeffectiverights53_object is defined as a combination of a Windows file and trustee sid. 
    /// The file represents the file to be evaluated while the trustee sid represents the account (sid) to check effective rights of. 
    /// If multiple files or sids are matched by either reference, then each possible combination of file and sid is a matching file effective rights object. 
    /// In addition, a number of behaviors may be provided that help guide the collection of objects. 
    /// Please refer to the FileEffectiveRights53Behaviors complex type for more information about specific behaviors.
    /// The set of files to be evaluated may be identified with either a complete filepath or a path and filename. Only one of these options may be selected.
    /// It is important to note that the 'max_depth' and 'recurse_direction' attributes of the 'behaviors' element do not apply to the 
    /// 'filepath' element, only to the 'path' and 'filename' elements.  
    /// This is because the 'filepath' element represents an absolute path to a particular file and it is not possible to recurse over a file.
    /// </summary>
    //[ProbeCapability(OvalObject = "fileeffectiverights53", PlataformName = FamilyEnumeration.windows)]
    public class FileEffectiveRights53Prober : ProbeBase, IProbe
    {
        public FileObjectCollector FileSystemDataSource { get; set; }
        public IFileProvider FileProvider { get; set; }

        private TargetInfo TargetInfo;

        private FileEffectiveRights53EntityOperationEvaluator OperationEvaluator;


        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            var setElementName = fileeffectiverights53_object_ItemsChoices.set;
            var setElement = ((fileeffectiverights53_object)objectType).GetItemValue(setElementName);

            return (set)setElement;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = base.ConnectionManager.Connect<FileConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            CreateObjectCollector();
            CreateFileSystemDataSource();
            CreateItemTypeGenerator();
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<fileeffectiverights53_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new fileeffectiverights_item() { status = StatusEnumeration.error, message = PrepareErrorMessage(errorMessage) };
        }

        private void CreateObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var wmiConnectionScope = ((FileConnectionProvider)this.ConnectionProvider).ConnectionScope;
                var newWmiProvider = new WmiDataProvider(wmiConnectionScope);
                base.ObjectCollector = new FileEffectiveRights53ObjectCollector() { WmiDataProvider = newWmiProvider };
            }
        }

        private void CreateFileSystemDataSource()
        {
            if (this.FileSystemDataSource == null)
                this.FileSystemDataSource =
                    new FileObjectCollector() { WmiDataProvider = GetCurrentWmiDataProvider() };

            if (this.FileProvider == null)
                this.FileProvider =
                    new WindowsFileProvider(TargetInfo) { WmiDataProvider = GetCurrentWmiDataProvider() };

        }

        private void CreateItemTypeGenerator()
        {
            if (this.OperationEvaluator == null)
                this.OperationEvaluator =
                    new FileEffectiveRights53EntityOperationEvaluator(
                        (FileEffectiveRights53ObjectCollector)base.ObjectCollector, this.FileProvider);

            if (this.ItemTypeGenerator == null)
            {
                this.ItemTypeGenerator =
                    new FileEffectiveRights53ItemTypeGenerator()
                    {
                        OperationEvaluator = this.OperationEvaluator
                    };
            }
        }

        private WmiDataProvider GetCurrentWmiDataProvider()
        {
            if (ObjectCollector == null)
                return null;

            return ((FileEffectiveRights53ObjectCollector)ObjectCollector).WmiDataProvider;
        }
    }
}
