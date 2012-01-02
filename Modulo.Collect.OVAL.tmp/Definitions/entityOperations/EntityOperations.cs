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
using Modulo.Collect.OVAL.Definitions;
using System.Text.RegularExpressions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions.Helpers;

namespace Modulo.Collect.OVAL.Definitions.EntityOperations
{
    public enum EntityOperationParameters { EntityValue, ValuesToApply };

    public abstract class OvalEntityOperationBase
    {       

        public abstract IEnumerable<string> Apply(Dictionary<string, object> entityParameters);

        protected virtual string GetEntityValueAsString(Dictionary<string, object> entityParameters)
        {
            return entityParameters[EntityOperationParameters.EntityValue.ToString()].ToString();
        }

        protected virtual IEnumerable<string> GetValuesToApply(Dictionary<string, object> entityParameters)
        {
            return (IEnumerable<string>)entityParameters[EntityOperationParameters.ValuesToApply.ToString()];
        }
    }



    public class EqualsEntityOperation: OvalEntityOperationBase
    {
        public override IEnumerable<string> Apply(Dictionary<string, object> entityParameters)
        {
            string entityValue = base.GetEntityValueAsString(entityParameters);
            string valueToApply = base.GetValuesToApply(entityParameters).ElementAt(0);

            IEnumerable<String> operationResult = new List<String>();
            if (OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.equals, valueToApply, entityValue))
                ((List<String>)operationResult).Add(entityValue);

            return operationResult;
        }
    }

    public class PatternMatchEntityOperation : OvalEntityOperationBase
    {
        public override IEnumerable<string> Apply(Dictionary<string, object> entityParameters)
        {
            string entityValue = base.GetEntityValueAsString(entityParameters);
            IEnumerable<string> valuesToApply = base.GetValuesToApply(entityParameters);

            return this.applyPatternMatch(entityValue, valuesToApply);
        }

        private IEnumerable<string> applyPatternMatch(string entityValue, IEnumerable<string> valuesToMatch)
        {
            IEnumerable<string> result = new List<string>();
            if (valuesToMatch != null)
            {
                foreach (var valueToMatch in valuesToMatch)
                {
                    RegexHelper regexHelper = new RegexHelper(entityValue);
                    IList<string> matchValues = regexHelper.GetMatchPathNamesFromCurrentPathPattern(new string[] { valueToMatch });
                    ((List<string>)result).AddRange(matchValues);
                }
            }

            return result;
        }
    }

    public class NotEqualsEntityOperation: OvalEntityOperationBase
    {
        public override IEnumerable<string> Apply(Dictionary<string, object> entityParameters)
        {
            string entityValue = base.GetEntityValueAsString(entityParameters);
            IEnumerable<string> valuesToApply = base.GetValuesToApply(entityParameters);

            return (from v in valuesToApply where v != entityValue select v);
        }
    }

    public class CaseInsentiveEqualsOperation : OvalEntityOperationBase
    {
        public override IEnumerable<string> Apply(Dictionary<string, object> entityParameters)
        {
            string entityValue = base.GetEntityValueAsString(entityParameters);
            IEnumerable<string> valuesToApply = base.GetValuesToApply(entityParameters);

            IEnumerable<String> operationResult = new List<String>();
            foreach (var value in valuesToApply)
                if (OvalEntityComparer.IsEntityValuesEquals(OperationEnumeration.caseinsensitiveequals, entityValue, value))
                    ((List<String>)operationResult).Add(value);


            return operationResult;
        }
    }

    public class CaseInsensitiveNotEqualsOperation : OvalEntityOperationBase
    {
        public override IEnumerable<string> Apply(Dictionary<string, object> entityParameters)
        {
            string entityValue = base.GetEntityValueAsString(entityParameters);
            IEnumerable<string> valuesToApply = base.GetValuesToApply(entityParameters);

            IEnumerable<string> operationResult = new List<String>();
            foreach (var value in valuesToApply)
                if (OvalEntityComparer.IsEntityValuesNotEqual(OperationEnumeration.caseinsensitivenotequal, entityValue, value))
                    ((List<String>)operationResult).Add(value);
            
            return operationResult;
        }
    }




    
        




}
