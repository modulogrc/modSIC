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
using Modulo.Collect.Probe.Independent.XmlFileContent;

namespace Modulo.Collect.Probe.Windows.Probes.Metabase
{
    public class MetabaseSessionConfiguration
    {
        public string Name { get; private set; }
        public string ID { get; private set; }
        public string Value { get; set; }
        public string Type { get; private set; }
        public string UserType { get; private set; }
        
        public MetabaseSessionConfiguration(string name, string id, string type, string userType)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("The metabase ID cannot be null or empty.");

            this.Name = name; 
            this.ID = id; 
            this.Type = type;
            this.UserType = userType;
        }
    }


    public class MetabaseFragment
    {
        private string XmlMetabaseContent;
        private string XmlMBSchema;

        public MetabaseFragment(string xmlMetabaseContent, string xmlMBSchema)
        {
            this.XmlMetabaseContent = "<root>" + xmlMetabaseContent + "</root>";
            this.XmlMBSchema = xmlMBSchema;
        }

        public MetabaseSessionConfiguration GetMetabaseSessionByID(string ID)
        {
            var metabaseSession = GetMetabasePropertyFromMBSchema(ID);
            if (metabaseSession == null)
                throw new XPathNoResultException();
            
            metabaseSession.Value = TryToGetAttributeValueFromMetabaseFragment(XmlMetabaseContent, metabaseSession.Name, true);
            if (metabaseSession.Value == null)
                throw new XPathNoResultException();
            
            CheckFlaggedProperties(metabaseSession);
            
            return metabaseSession;
        }

        private void CheckFlaggedProperties(MetabaseSessionConfiguration metabaseSession)
        {
            if (string.IsNullOrWhiteSpace(metabaseSession.Value))
                return;

            if (this.IsFlaggedProperty(metabaseSession))
            {
                int propertyValue = 0;
                var flags = metabaseSession.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var flag in flags)
                {
                    var xPathFlagNode =  string.Format("//*[@InternalName='{0}']", flag.Trim());
                    var flagNode = new XPathOperator().applyXpath(this.XmlMBSchema, xPathFlagNode, true);
                    var flagValue = int.Parse(new XPathOperator().applyXpath(flagNode.First(), "//*/@Value", false).First());
                    propertyValue += flagValue;
                }

                metabaseSession.Value = propertyValue.ToString();
            }
        }

        private bool IsFlaggedProperty(MetabaseSessionConfiguration metabaseSession)
        {
            var propertyID = metabaseSession.ID;

            return (propertyID.Equals("6030") ||
                    propertyID.Equals("6016") ||
                    propertyID.Equals("6000") ||
                    propertyID.Equals("6005") ||
                    propertyID.Equals("4013") ||
                    propertyID.Equals("2068") ||
                    propertyID.Equals("6031") ||
                    propertyID.Equals("2044") ||
                    propertyID.Equals("1027") ||
                    propertyID.Equals("7044") ||
                    propertyID.Equals("9037")
                    
                    );
        }

        private MetabaseSessionConfiguration GetMetabasePropertyFromMBSchema(string id)
        {
            var xpathExpression = string.Format("//*[@ID='{0}']", id);
            try
            {
                var propertyNode = new XPathOperator().applyXpath(XmlMBSchema, xpathExpression, true).FirstOrDefault();
                if (!string.IsNullOrEmpty(propertyNode))
                {
                    var name = TryToGetAttributeValueFromMetabaseFragment(propertyNode, "InternalName", true);
                    var type = TryToGetAttributeValueFromMetabaseFragment(propertyNode, "Type", true);
                    var userType = TryToGetAttributeValueFromMetabaseFragment(propertyNode, "UserType", true);
                    return new MetabaseSessionConfiguration(name, id, type, userType);
                }

                return null;
            }
            catch (XPathNoResultException)
            {
                try
                {
                    var propertyNode = new XPathOperator().applyXpath(XmlMetabaseContent, xpathExpression, true).FirstOrDefault();
                    var name = TryToGetAttributeValueFromMetabaseFragment(propertyNode, "Name", false);
                    var type = TryToGetAttributeValueFromMetabaseFragment(propertyNode, "Type", false);
                    var userType = TryToGetAttributeValueFromMetabaseFragment(propertyNode, "UserType", false);
                    return new MetabaseSessionConfiguration(name, id, type, userType);
                }
                catch (XPathNoResultException)
                {
                    return null;
                }
            }
        }

        private string TryToGetAttributeValueFromMetabaseFragment(string metabaseXmlFragment, string attributeName, bool retryToGetValue)
        {
            var xPathOperator = new XPathOperator();
            try
            {
                var xpathExpression = string.Format("//*/@{0}", attributeName);
                return xPathOperator.applyXpath(metabaseXmlFragment, xpathExpression).FirstOrDefault();
            }
            catch (XPathNoResultException)
            {
                try
                {
                    if (retryToGetValue)
                        return xPathOperator.applyXpath(metabaseXmlFragment, "//*/@Value").FirstOrDefault();
                    
                    return null;
                }
                catch (XPathNoResultException)
                {
                    return null;
                }
            }
        }
    }
}
