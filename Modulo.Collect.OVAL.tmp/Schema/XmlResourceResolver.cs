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
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

namespace Modulo.Collect.OVAL.Schema
{
    /// <summary>
    /// Provides resource-based XML Uri resolution.
    /// </summary>
    public class XmlResourceResolver : XmlResolver
    {
        /// <summary>
        /// The container assembly for the resources
        /// </summary>
        private string _Folder;
        private Assembly m_container;

        /// <summary>
        /// Credential set (not supported)
        /// </summary>
        public override ICredentials Credentials
        {
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Creates and initializes a new instance of XmlResourceResolver
        /// </summary>
        /// <param name="container">Container assembly</param>
        /// <param name="container">Schema Assembly folder</param>
        public XmlResourceResolver(string folder)
        {
            _Folder = folder;
            m_container = GetType().Assembly;
        }

        /// <summary>
        /// Creates and returns a stream for the specified Uri
        /// </summary>
        /// <param name="absoluteUri">Absolute Uri to the resource</param>
        /// <param name="role">The resource role (ignored)</param>
        /// <param name="ofObjectToReturn">Type of object to be returned (ignored)</param>
        /// <returns>A stream to the resource data</returns>
        /// <remarks>The Uri is automatically constructed by the base <see cref="XmlResolver"/>;
        /// only the last part (CurrentDocument) is processed.</remarks>
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return GetStream(absoluteUri.Segments[absoluteUri.Segments.GetUpperBound(0)]);
        }

        /// <summary>
        /// Creates a resource stream for the specified resource name
        /// </summary>
        /// <param name="resourceName">Resource name for which to create the stream</param>
        /// <returns>A stream to the resource data</returns>
        public Stream GetStream(string resourceName)
        {
            return m_container.GetManifestResourceStream(string.Format("{0}.resources.{1}.{2}",m_container.GetName().Name,_Folder,resourceName));
        }
    }
}
