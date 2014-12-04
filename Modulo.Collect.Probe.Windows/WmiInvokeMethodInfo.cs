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
using System.Linq;
using System.Collections.Generic;

namespace Modulo.Collect.Probe.Windows
{
    public class WmiInvokeMethodOutParameter
    {
        public List<String> ParentObjectNames { get; private set; }
        public List<String> PropertyNames { get; private set; }

        
        public WmiInvokeMethodOutParameter(string fullParentObjectName)
        {
            string[] splittedParentObjectNames = fullParentObjectName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            this.ParentObjectNames = new List<String>(splittedParentObjectNames);
        }

        public void AddPropertyName(string propertyName)
        {
            if (this.PropertyNames == null)
                this.PropertyNames = new List<String>();

            this.PropertyNames.Add(propertyName);
        }
    }

    public class WmiInvokeMethodFilter
    {
        public IList<String> FilterParentObjectNames { get; private set; }
        public String FilterPropertyName { get; private set; }
        public String FilterPropertyValue { get; private set; }

        public WmiInvokeMethodFilter(string fullQualifyParentObjectName, string propertyName, string propertyValue)
        {
            string[] splittedParentObjectNames = fullQualifyParentObjectName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            this.FilterParentObjectNames = new List<String>(splittedParentObjectNames);

            this.FilterPropertyName = propertyName;
            this.FilterPropertyValue = propertyValue;
        }
    }


    public class WmiInvokeMethodInfo
    {
        public string ClassName { get; set; }
        
        public string PathName { get; set; }
        
        public string PathValue { get; set; }
        
        public string MethodName { get; set; }

        public WmiInvokeMethodOutParameter OutParameters { get; private set; }

        public IList<WmiInvokeMethodFilter> Filter { get; set; }





        public void AddOutProperties(string parentObjectName, string[] propertyNames)
        {
            WmiInvokeMethodOutParameter newOutParameter = new WmiInvokeMethodOutParameter(parentObjectName);
            foreach (var propertyName in propertyNames)
                newOutParameter.AddPropertyName(propertyName);

            this.OutParameters = newOutParameter;
        }

        public void AddInvokeMethodFilter(string fullQualifyObjectName, string propertyName, string propertyValue)
        {
            WmiInvokeMethodFilter wmiFilter = new WmiInvokeMethodFilter(fullQualifyObjectName, propertyName, propertyValue);
            if (this.Filter == null)
                this.Filter = new List<WmiInvokeMethodFilter>();
            this.Filter.Add(wmiFilter);

        }

        public bool ShouldBeReturnManagementObjects()
        {
            return 
                ((this.OutParameters == null) || 
                 (this.OutParameters.ParentObjectNames == null) ||
                 (this.OutParameters.ParentObjectNames.Count == 0));
        }
    }
}
