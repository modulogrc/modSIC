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
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Common.comparators;

namespace Modulo.Collect.OVAL.Definitions.validators
{
    public class ExternalVariableValidator
    {
        public IEnumerable<string> ValidateValue(VariablesTypeVariableExternal_variable externalVariable, string value)
        {
            List<string> messages = new List<string>();
            IEnumerable<PossibleValueType> possibleValues = externalVariable.GetPossibleValues();
            if (externalVariable.HasPossibleRestriction())
            {
                messages.AddRange(this.VerifyRestrictions(value, externalVariable));
                if (messages.Count() > 0)
                {
                    if (this.TheValueExistsInPossibleValues(value, possibleValues))
                        messages.Clear();
                }
            }
            else
            {
                messages.AddRange(this.VerifyPossibleValues(value, possibleValues));
            }            
            return messages;
        }

        private IEnumerable<string> VerifyPossibleValues(string value, IEnumerable<PossibleValueType> possibleValues)
        {
            List<string> messages = new List<string>();
            if ((possibleValues.Count() > 0) && (!this.TheValueExistsInPossibleValues(value, possibleValues)))
            {
                messages.Add(string.Format("The value {0} is not in the range the possible values", value));
            }
            return messages;
        }       

        private bool TheValueExistsInPossibleValues(string value, IEnumerable<PossibleValueType> possibleValues)
        {
            return possibleValues.Where(pv => pv.Value.Equals(value)).Count() > 0;
        }

        private IEnumerable<string> VerifyRestrictions(string value, VariablesTypeVariableExternal_variable externalVariable)
        {
            List<string> messages = new List<string>();
            IEnumerable<PossibleRestrictionType> possibleRestrictions = externalVariable.GetPossibleRestrictions();
            foreach (var possibleRestriction in possibleRestrictions)
            {
                foreach (var restriction in possibleRestriction.Items)
                {
                    messages.AddRange(this.ApplyRestriction(restriction, value, externalVariable.datatype, possibleRestriction.hint));
                }
            }


            return messages;  
        }

        private IEnumerable<string> ApplyRestriction(RestrictionType restriction, string value, Common.SimpleDatatypeEnumeration datatypeEnumeration, string restrictionHint)
        {            
            List<string> messages = new List<string>();
            IOvalComparator comparator = new OvalComparatorFactory().GetComparator(datatypeEnumeration);
            if (!comparator.Compare(value, restriction.Value, restriction.operation))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(restrictionHint);
                sb.AppendLine(restriction.GetRestrictionMessage());
                messages.Add(sb.ToString());
            }
            return messages;
        }
    }
}
