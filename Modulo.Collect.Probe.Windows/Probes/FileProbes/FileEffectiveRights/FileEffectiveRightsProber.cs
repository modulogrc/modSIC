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
    /// The fileeffectiverights_object element is used by a file effective rights test to define the objects used to evalutate against the specified state. 
    /// The fileeffectiverights_object will collect directories and all Windows file types (FILE_TYPE_CHAR, FILE_TYPE_DISK, FILE_TYPE_PIPE, FILE_TYPE_REMOTE, and FILE_TYPE_UNKNOWN). 
    /// Each object extends the standard ObjectType as definied in the oval-definitions-schema and one should refer to the ObjectType description for more information. 
    /// The common set element allows complex objects to be created using filters and set logic.
    /// A fileeffectiverights_object is defined as a combination of a Windows file and trustee name. 
    /// The file represents the file to be evaluated while the trustee name represents the account (sid) to check effective rights of. 
    /// If multiple files or sids are matched by either reference, then each possible combination of file and sid is a matching file effective rights object. 
    /// In addition, a number of behaviors may be provided that help guide the collection of objects. 
    /// Please refer to the FileEffectiveRightsBehaviors complex type for more information about specific behaviors.
    /// <oval:deprecated_info>
    ///    <oval:version>5.3</oval:version>
    ///    <oval:reason>
    ///         Replaced by the fileeffectiverights_object. This object uses a trustee_name element for identifying trustees. 
    ///         Trustee names are not unique, and a new object was created to use trustee SIDs, which are unique. See the fileeffectiverights53_object.
    ///    </oval:reason>
    ///    <oval:comment>This object has been deprecated and will be removed in version 6.0 of the language.</oval:comment>
    /// </oval:deprecated_info>
    /// </summary>
    [ProbeCapability(OvalObject = "fileeffectiverights", PlataformName = FamilyEnumeration.windows)]
    public class FileEffectiveRightsProber: ProbeBase, IProbe
    {
        private BaseObjectCollector FileSystemDataSource;

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            base.ConnectionProvider = base.ConnectionManager.Connect<FileConnectionProvider>(connectionContext, target);            
        }

        /// <summary>
        /// This is a temporary code in order to keep unit tests and oval interpreter prototype compatible.
        /// This method should be replace by dependency injection pattern.
        /// </summary>
        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var newWmiProvider = this.CreateWmiProviderFromProberConnection();
                base.ObjectCollector = new FileEffectiveRightsObjectCollector() { WmiDataProvider = newWmiProvider };
                this.FileSystemDataSource = new FileObjectCollector() { WmiDataProvider = newWmiProvider };
            }

            if (base.ItemTypeGenerator == null)
                base.ItemTypeGenerator = new FileEffectiveRightsItemTypeGenerator() { SystemDataSource = base.ObjectCollector, FileDataSource = FileSystemDataSource };
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<fileeffectiverights_object>();
        }

        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            var setElement = ((fileeffectiverights_object)objectType).GetItemValue(fileeffectiverights_object_ItemsChoices.set);
            return (set)setElement;
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            throw new NotImplementedException();
        }


        private WmiDataProvider CreateWmiProviderFromProberConnection()
        {
            var wmiConnectionScope = ((FileConnectionProvider)this.ConnectionProvider).ConnectionScope;
            return new WmiDataProvider(wmiConnectionScope);
        }
    }
}
