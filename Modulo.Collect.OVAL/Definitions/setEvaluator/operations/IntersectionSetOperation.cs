﻿/*
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
using Modulo.Collect.OVAL.SystemCharacteristics;

namespace Modulo.Collect.OVAL.Definitions.setEvaluator.operations
{
    /// <summary>
    /// This class represents the intersetion operation in sets.
    /// The intersection of multiple sets in OVAL results in a set that 
    /// contains everything that belongs every set in the collection, but 
    /// nothing else.  If A and B are sets, then the intersection of A and B 
    /// contains all the elements of A that also belong to B, but no other 
    /// elements.  You may take the intersection of more than two sets. The 
    /// intersection of A, B, and C, for example, contains all the elements 
    /// that exist in each of the sets A, B, and C, and nothing else. 
    /// </summary>
    public class IntersectionSetOperation : SetOperation
    {
        public IntersectionSetOperation()
        {
            this.LoadMatrixCombinationFlags();
        }

        public override IEnumerable<string> Execute(IEnumerable<string> firstReferences, IEnumerable<string> secondReferences)
        {
            var intersection = firstReferences.Intersect(secondReferences);
            return intersection;
        }

        private void LoadMatrixCombinationFlags()
        {

            this.matrixOfCombinationFlags = new Dictionary<string, FlagEnumeration>();
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.error, FlagEnumeration.error), FlagEnumeration.error);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.error, FlagEnumeration.complete), FlagEnumeration.error);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.error, FlagEnumeration.incomplete), FlagEnumeration.error);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.error, FlagEnumeration.doesnotexist), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.error, FlagEnumeration.notcollected), FlagEnumeration.error);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.error, FlagEnumeration.notapplicable), FlagEnumeration.error);

            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.complete, FlagEnumeration.error), FlagEnumeration.error);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.complete, FlagEnumeration.complete), FlagEnumeration.complete);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.complete, FlagEnumeration.incomplete), FlagEnumeration.incomplete);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.complete, FlagEnumeration.doesnotexist), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.complete, FlagEnumeration.notcollected), FlagEnumeration.notcollected);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.complete, FlagEnumeration.notapplicable), FlagEnumeration.complete);

            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.incomplete, FlagEnumeration.error), FlagEnumeration.error);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.incomplete, FlagEnumeration.complete), FlagEnumeration.incomplete);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.incomplete, FlagEnumeration.incomplete), FlagEnumeration.incomplete);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.incomplete, FlagEnumeration.doesnotexist), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.incomplete, FlagEnumeration.notcollected), FlagEnumeration.notcollected);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.incomplete, FlagEnumeration.notapplicable), FlagEnumeration.incomplete);

            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.doesnotexist, FlagEnumeration.error), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.doesnotexist, FlagEnumeration.complete), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.doesnotexist, FlagEnumeration.incomplete), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.doesnotexist, FlagEnumeration.doesnotexist), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.doesnotexist, FlagEnumeration.notcollected), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.doesnotexist, FlagEnumeration.notapplicable), FlagEnumeration.doesnotexist);

            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notcollected, FlagEnumeration.error), FlagEnumeration.error);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notcollected, FlagEnumeration.complete), FlagEnumeration.notcollected);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notcollected, FlagEnumeration.incomplete), FlagEnumeration.notcollected);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notcollected, FlagEnumeration.doesnotexist), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notcollected, FlagEnumeration.notcollected), FlagEnumeration.notcollected);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notcollected, FlagEnumeration.notapplicable), FlagEnumeration.notapplicable);

            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notapplicable, FlagEnumeration.error), FlagEnumeration.error);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notapplicable, FlagEnumeration.complete), FlagEnumeration.complete);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notapplicable, FlagEnumeration.incomplete), FlagEnumeration.incomplete);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notapplicable, FlagEnumeration.doesnotexist), FlagEnumeration.doesnotexist);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notapplicable, FlagEnumeration.notcollected), FlagEnumeration.notcollected);
            this.matrixOfCombinationFlags.Add(this.GetNameOfFlagEnumerationCombined(FlagEnumeration.notapplicable, FlagEnumeration.notapplicable), FlagEnumeration.notapplicable);

        }
        
    }
}
