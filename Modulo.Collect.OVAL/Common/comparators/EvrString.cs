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

namespace Modulo.Collect.OVAL.Common.comparators
{
    /// <summary>
    /// It provides a struct to represent a EVR String (RPM Version)
    /// http://www.rpm.org/max-rpm/s1-rpm-inside-tags.html
    /// </summary>
    public class EvrString
    {
        private const int FIRST_POSITION = 0;
        private const int SECOND_POSITION = 1;
        private char[] COLON_SEPARATOR = new char[] { ':' };
        private char[] DASH_SEPARATOR = new char[] { '-' };
        

        public EvrString(String evrAsString)
        {
            var evrParts = evrAsString.Split(COLON_SEPARATOR);
            var hasEpoch = evrParts.Count() > 1;
            var evrWithoutEpoch =  evrParts.ElementAt(hasEpoch ? SECOND_POSITION : FIRST_POSITION);
            var nonEpochEvrParts = evrWithoutEpoch.Split(DASH_SEPARATOR);

            if (nonEpochEvrParts.Count() > 2)
                throw new InvalidEvrStringFormat(String.Format("There are too many dashes in EvrString: '{0}'", evrAsString));

            if (hasEpoch)
            {
                if (evrParts.ElementAt(FIRST_POSITION) != "(none)")
                    this.Epoch = evrParts.ElementAt(FIRST_POSITION);
            }
            
            this.Version = nonEpochEvrParts.ElementAt(FIRST_POSITION);
            this.Release = nonEpochEvrParts.Count() > 1 ? nonEpochEvrParts.ElementAt(SECOND_POSITION) : "0";
        }

        public String Epoch { get; private set; }

        public String Version { get; private set; }

        public String Release { get; private set; }
    }

    public class InvalidEvrStringFormat : Exception 
    { 
        public InvalidEvrStringFormat(String message) : base(message) { }  
    }
}
