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
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using Modulo.Collect.Probe.Independent.Common.File;

namespace Modulo.Collect.Probe.Independent.XmlFileContent
{
    public class XPathOperator
    {
        public IFileProvider FileContentProvider { get; set; }

        public virtual IEnumerable<String> Apply(string xmlFilepath, string xPathExpression, bool returnOuterXml = false)
        {
            var xmlFileContentLines = this.getFileLinesContent(xmlFilepath);
            var xmlFileContent = string.Join(Environment.NewLine, xmlFileContentLines);

            if (xmlFilepath.Contains("metabase.xml"))
                xmlFileContent = xmlFileContent.Replace("xmlns=\"urn:microsoft-catalog:XML_Metabase_V64_0\"", string.Empty);
            
            if (xmlFilepath.Contains("mbschema.xml"))
                xmlFileContent = xmlFileContent.Replace("xmlns=\"x-urn:microsoft-catalog:MetaData_V7\"", string.Empty);
 
            if (string.IsNullOrWhiteSpace(xmlFileContent))
                throw new XPathNoResultException();

            return this.applyXpath(xmlFileContent, xPathExpression, returnOuterXml);
        }

        private IEnumerable<String> getFileLinesContent(string xmlFilepath)
        {
            return this.FileContentProvider.GetFileLinesContentFromHost(xmlFilepath);
        }

        public string[] applyXpath(string xmlContent, string xpath, bool returnOuterXml = false)
        {
            var result = new List<String>();
            var sr = new StringReader(xmlContent);
            var document = new XPathDocument(sr);
            
            XPathNavigator navigator = document.CreateNavigator();
            XPathExpression expression = XPathExpression.Compile(xpath);
            var valueOf = navigator.Evaluate(expression);

            if (valueOf == null)
                throw new XPathNoResultException();

            switch (expression.ReturnType)
            {
                case XPathResultType.String:
                case XPathResultType.Number:
                    result.Add(valueOf.ToString());
                    break;

                case XPathResultType.Boolean:
                    result.Add(((bool)valueOf).ToString());
                    break;

                case XPathResultType.NodeSet:
                    XPathNodeIterator nodes = navigator.Select(expression);
                    if ((nodes == null) || (nodes.Count <= 0))
                        throw new XPathNoResultException();

                    while (nodes.MoveNext())
                    {
                        var xmlChunk = returnOuterXml ? nodes.Current.OuterXml : nodes.Current.InnerXml;
                        var nodeValue = string.IsNullOrEmpty(xmlChunk) ? nodes.Current.Value : xmlChunk;
                        result.Add(nodeValue);
                    }

                    break;

                case XPathResultType.Error:
                    break;
            }

            sr.Close();
            return result.ToArray();
        }
    }

    public class XPathNoResultException : Exception { }
}
