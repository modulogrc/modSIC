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
using Modulo.Collect.Probe.Independent.Common;

using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Independent;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Independent.XmlFileContent
{
    public abstract class
        GenericXmlFileContentProber<ConnectionProviderType> : ProbeBase, IProbe
        where ConnectionProviderType : IConnectionProvider, new()
    {
        public IFileProvider CustomFileProvider { get; set; }
        public FamilyEnumeration Platform { get; private set; }

        protected GenericXmlFileContentProber() { }

        protected GenericXmlFileContentProber(IFileProvider customFileProvider)
            : base()
        {
            CustomFileProvider = customFileProvider;
            SetPlatform();
        }

        private void SetPlatform()
        {
            if (CustomFileProvider != null)
            {
                var fileProviderTypeName = CustomFileProvider.GetType().Name.ToLower();
                Platform = fileProviderTypeName.Contains("unix") ? FamilyEnumeration.unix : FamilyEnumeration.windows;
            }
        }

        protected TargetInfo TargetInfo;

        protected override Definitions.set GetSetElement(Definitions.ObjectType objectType)
        {
            return null;
        }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            base.ConnectionProvider = this.ConnectionManager.Connect<ConnectionProviderType>(connectionContext, target);
        }

        protected abstract void InitializeCustomFileProvider();

        protected override void ExecuteAfterOpenConnectionProvider()
        {
            if (CustomFileProvider == null)
            {
                InitializeCustomFileProvider();
                SetPlatform(); 
            }
        }

        protected override void ConfigureObjectCollector()
        {
            if (base.ObjectCollector == null)
            {
                var newXPathOperator = new XPathOperator() { FileContentProvider = CustomFileProvider };
                var objCollector = new XmlFileContentObjectCollector() { XPathOperator = newXPathOperator, Platform = this.Platform };

                base.ObjectCollector = objCollector;
            }

            if (base.ItemTypeGenerator == null)
            {
                var pathOperatorEvaluator = new PathOperatorEvaluator(CustomFileProvider, Platform);
                var itemTypeGenerator = new XmlFileContentItemTypeGenerator(pathOperatorEvaluator) ;

                base.ItemTypeGenerator = itemTypeGenerator;
            }
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new xmlfilecontent_item() { status = StatusEnumeration.error, message = PrepareErrorMessage(errorMessage)};
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<xmlfilecontent_object>();
        }
    }
}
