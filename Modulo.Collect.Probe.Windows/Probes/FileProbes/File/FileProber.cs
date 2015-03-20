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
using System.Management;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Windows.WMI;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.File
{
    /// <summary>
    /// The file_object element is used by a file test to define the specific file(s) to be evaluated. 
    /// The file_object will collect directories and all Windows file types (FILE_TYPE_CHAR, FILE_TYPE_DISK, FILE_TYPE_PIPE, FILE_TYPE_REMOTE, and FILE_TYPE_UNKNOWN). 
    /// Each object extends the standard ObjectType as definied in the oval-definitions-schema and one should refer to the ObjectType description for more information. 
    /// The common set element allows complex objects to be created using filters and set logic. 
    /// Again, please refer to the description of the set element in the oval-definitions-schema.
    /// A file object defines the path and filename or complete filepath of the file(s). In addition, a number of behaviors may be provided that help guide the collection of objects. 
    /// Please refer to the FileBehaviors complex type for more information about specific behaviors.
    /// The set of files to be evaluated may be identified with either a complete filepath or a path and filename. Only one of these options may be selected.
    /// It is important to note that the 'max_depth' and 'recurse_direction' attributes of the 'behaviors' element do not apply to the 
    /// 'filepath' element, only to the 'path' and 'filename' elements.  
    /// This is because the 'filepath' element represents an absolute path to a particular file and it is not possible to recurse over a file.
    /// </summary>
    [ProbeCapability(OvalObject = "file", PlataformName = FamilyEnumeration.windows)]
    public class FileProber : ProbeBase, IProbe
    {

        private TargetInfo TargetInfo;

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = this.ConnectionManager.Connect<FileConnectionProvider>(connectionContext, target);            
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var wmiConnectionScope = ((FileConnectionProvider)this.ConnectionProvider).ConnectionScope;
                base.ObjectCollector = new FileObjectCollector() { WmiDataProvider = new WmiDataProvider(wmiConnectionScope) };
            }

            this.CreateItemTypeGenerator();
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<file_object>();
        }

        protected override set GetSetElement(Modulo.Collect.OVAL.Definitions.ObjectType objectType)
        {
            var setElement = ((file_object)objectType).GetItemValue(file_object_ItemsChoices.set);
            return (set)setElement;
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new file_item() { message = PrepareErrorMessage(errorMessage), status = StatusEnumeration.error };
        }

        private void CreateItemTypeGenerator()
        {
            if (base.ItemTypeGenerator == null)
            {
                var wmiDataProvider = ((FileObjectCollector)base.ObjectCollector).WmiDataProvider;
                var windowsFileProvider = new WindowsFileProvider(this.TargetInfo, wmiDataProvider);
                base.ItemTypeGenerator = new FileItemTypeGenerator()
                {
                    SystemDataSource = base.ObjectCollector,
                    FileProvider = windowsFileProvider
                };
            }
        }
    }
}
