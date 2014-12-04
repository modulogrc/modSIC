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
using System.Text.RegularExpressions;

namespace Modulo.Collect.OVAL.Common.comparators
{
    public enum ComparisionResults { IsGreaterThan = -1, IsEqual, IsLessThan }

    public class EvrStringComparator: IOvalComparator
    {
        private const string ALL_DIGITS_REGEX_PATTERN = "[0-9]+";

        public bool Compare(string firstElement, string secondElement, OperationEnumeration operation)
        {
            var firstEvrString = new EvrString(firstElement);
            var secondEvrString = new EvrString(secondElement);

            var evrIsEqual = ProcessEvrComparisionForEquals(firstEvrString, secondEvrString);
            var evrIsGreaterThan = ProcessEvrComparisionFor(firstEvrString, secondEvrString, ComparisionResults.IsGreaterThan);
            var evrIsLessThan = ProcessEvrComparisionFor(firstEvrString, secondEvrString, ComparisionResults.IsLessThan);

            switch (operation)
            {
                case OperationEnumeration.equals:
                    return evrIsEqual;
                case OperationEnumeration.notequal:
                    return !evrIsEqual;
                case OperationEnumeration.greaterthan:
                    return evrIsGreaterThan;
                case OperationEnumeration.lessthan:
                    return evrIsLessThan;
                case OperationEnumeration.greaterthanorequal:
                    return evrIsGreaterThan || evrIsEqual;
                case OperationEnumeration.lessthanorequal:
                    return evrIsLessThan || evrIsEqual;
                default:
                    throw new InvalidOvalOperationException();
            }
        }

        private ComparisionResults compareEpoches(String firstEpoch, String secondEpoch)
        {
            var is1stEpochDefined = !String.IsNullOrWhiteSpace(firstEpoch);
            var is2ndEpochDefined = !String.IsNullOrWhiteSpace(secondEpoch);

            if (is1stEpochDefined && (!is2ndEpochDefined))
                return ComparisionResults.IsGreaterThan;

            if (is2ndEpochDefined && (!is1stEpochDefined))
                return ComparisionResults.IsLessThan;

            if (!(is1stEpochDefined || is2ndEpochDefined))
                return ComparisionResults.IsEqual;

            var all1stEpochDigits = Regex.Matches(firstEpoch, ALL_DIGITS_REGEX_PATTERN);
            var all2ndEpochDigits = Regex.Matches(secondEpoch, ALL_DIGITS_REGEX_PATTERN);
            var firstEpochAsInteger = int.Parse(TransformMatchCollectionIntoSingleString(all1stEpochDigits));
            var secondEpochAsInteger = int.Parse(TransformMatchCollectionIntoSingleString(all2ndEpochDigits));

            if (firstEpochAsInteger.Equals(secondEpochAsInteger))
                return ComparisionResults.IsEqual;

            var is1stEpochGreaterThan2ndOne = firstEpochAsInteger > secondEpochAsInteger;
            return is1stEpochGreaterThan2ndOne ? ComparisionResults.IsGreaterThan : ComparisionResults.IsLessThan;
        }

        private string[] breakIntoNumAndNotNum(string evrFrag)
        {
            List<string> retList = new List<string>();

            bool thisSubfragIsNum = true;
            string thisSubFrag = "";
            for (int i = 0; i < evrFrag.Length; i++)
            {
                char c = evrFrag[i];
                bool thisCharIsNum = Char.IsDigit(c);
                if (i == 0)
                {
                    thisSubfragIsNum = thisCharIsNum;
                    thisSubFrag += c;
                }
                else
                {
                    if (thisCharIsNum != thisSubfragIsNum)
                    {
                        retList.Add(thisSubFrag);
                        thisSubfragIsNum = thisCharIsNum;
                        thisSubFrag = c.ToString();
                    }
                    else
                        thisSubFrag += c;
                }
            }
            if (!String.IsNullOrEmpty(thisSubFrag))
                retList.Add(thisSubFrag);

            return retList.ToArray();
        }

        private ComparisionResults compareEvrFrag(String firstEvrFrag, String secondEvrFrag)
        {
            // Quick for easy case
            if (firstEvrFrag == secondEvrFrag)
                return ComparisionResults.IsEqual;

            string[] firstSubFrags = breakIntoNumAndNotNum(firstEvrFrag);
            string[] secondSubFrags = breakIntoNumAndNotNum(secondEvrFrag);

            int nSubFragsFirst = firstSubFrags.Length;
            int nSubFragsSecond = secondSubFrags.Length;
            int nSubFrags = nSubFragsFirst < nSubFragsSecond ? nSubFragsFirst : nSubFragsSecond;

            for (int i = 0; i < nSubFrags; i++)
            {
                string firstSubFrag = firstSubFrags[i];
                string secondSubFrag = secondSubFrags[i];

                if (firstSubFrag != secondSubFrag)
                {
                    // One is empty -> the other wins
                    if (String.IsNullOrEmpty(firstSubFrag))
                        return ComparisionResults.IsLessThan;
                    if (String.IsNullOrEmpty(secondSubFrag))
                        return ComparisionResults.IsGreaterThan;

                    bool firstIsNum = Char.IsDigit(firstSubFrag[0]);
                    bool secondIsNum = Char.IsDigit(secondSubFrag[0]);

                    IComparable firstVal;
                    IComparable secondVal;
                    if (firstIsNum && secondIsNum)
                    {
                        firstVal = long.Parse(firstSubFrag);
                        secondVal = long.Parse(secondSubFrag);
                    }
                    else
                    {
                        firstVal = firstSubFrag;
                        secondVal = secondSubFrag;
                    }

                    int compareSubFrag = firstVal.CompareTo(secondVal);
                    if (compareSubFrag < 0)
                        return ComparisionResults.IsLessThan;
                    if (compareSubFrag > 0)
                        return ComparisionResults.IsGreaterThan;
                }
            }

            // All equal so far, but if somebody has extra subfrags, it wins
            if (nSubFragsFirst > nSubFragsSecond)
                return ComparisionResults.IsGreaterThan;
            else if (nSubFragsFirst < nSubFragsSecond)
                return ComparisionResults.IsLessThan;
            else
                return ComparisionResults.IsEqual;
        }

        private ComparisionResults compareEvrPart(String firstEvrPart, String secondEvrPart)
        {
            // Quick for easy case
            if (firstEvrPart == secondEvrPart)
                return ComparisionResults.IsEqual;

            // Break 'em up
            string[] firstEvrFrags = firstEvrPart.Split(new char[] { '.' });
            string[] secondEvrFrags = secondEvrPart.Split(new char[] { '.' });

            int nFragsFirst = firstEvrFrags.Length;
            int nFragsSecond = secondEvrFrags.Length;
            int nFrags = nFragsFirst < nFragsSecond ? nFragsFirst : nFragsSecond;

            for (int i = 0; i < nFrags; i++)
            {
                ComparisionResults fragResult = compareEvrFrag(firstEvrFrags[i], secondEvrFrags[i]);
                if (fragResult != ComparisionResults.IsEqual)
                    return fragResult;
            }

            // All equal so far, but if somebody has extra frags, it wins
            if (nFragsFirst > nFragsSecond)
                return ComparisionResults.IsGreaterThan;
            else if (nFragsFirst < nFragsSecond)
                return ComparisionResults.IsLessThan;
            else
                return ComparisionResults.IsEqual;
        }

#if false
        private ComparisionResults compareEvrPart(String firstEvrPart, String secondEvrPart)
        {
            var allVersionDigitsInFirstEvrString = Regex.Matches(firstEvrPart, ALL_DIGITS_REGEX_PATTERN);
            var allVersionDigitsInSecondEvrString = Regex.Matches(secondEvrPart, ALL_DIGITS_REGEX_PATTERN);
            if (allVersionDigitsInFirstEvrString.Count != allVersionDigitsInSecondEvrString.Count)
            {

                if (allVersionDigitsInFirstEvrString.Count > allVersionDigitsInSecondEvrString.Count)
                    return ComparisionResults.IsGreaterThan;
                else
                    return ComparisionResults.IsLessThan;
            }

            var allVersionDigitsTogetherFrom1stEvrString = 
                TransformMatchCollectionIntoSingleString(allVersionDigitsInFirstEvrString);
            var allVersionDigitsTogetherFrom2ndEvrString = 
                TransformMatchCollectionIntoSingleString(allVersionDigitsInSecondEvrString);

            var firstVersionAsInteger = int.Parse(allVersionDigitsTogetherFrom1stEvrString);
            var secondVersionAsInteger = int.Parse(allVersionDigitsTogetherFrom2ndEvrString);

            if (firstVersionAsInteger.Equals(secondVersionAsInteger))
                return ComparisionResults.IsEqual;

            var isFirstGreaterThanSecond = firstVersionAsInteger > secondVersionAsInteger;
            return isFirstGreaterThanSecond ? ComparisionResults.IsGreaterThan : ComparisionResults.IsLessThan;
        }
#endif

        private String TransformMatchCollectionIntoSingleString(MatchCollection matchCollection)
        {
            var stringBuilder = new StringBuilder();
            foreach (Match match in matchCollection)
                stringBuilder.Append(match.Value);
            
            return stringBuilder.ToString();
        }

        private bool ProcessEvrComparisionForEquals(EvrString firstEvr, EvrString secondEvr)
        {
            var epochComparisionResult = compareEpoches(firstEvr.Epoch, secondEvr.Epoch);
            if (epochComparisionResult.Equals(ComparisionResults.IsEqual))
            {
                var versionComparisionResult = compareEvrPart(firstEvr.Version, secondEvr.Version);
                if (versionComparisionResult.Equals(ComparisionResults.IsEqual))
                {
                    var releaseComparisionResult = compareEvrPart(firstEvr.Release, secondEvr.Release);
                    return releaseComparisionResult.Equals(ComparisionResults.IsEqual);
                }
            }

            return false;
        }

        private bool ProcessEvrComparisionFor(EvrString firstEvr, EvrString secondEvr, ComparisionResults expectedResult)
        {
            var epochComparisionResult = compareEpoches(firstEvr.Epoch, secondEvr.Epoch);
            if (epochComparisionResult.Equals(expectedResult))
                return true;

            var versionComparisionResult = compareEvrPart(firstEvr.Version, secondEvr.Version);
            if (versionComparisionResult.Equals(expectedResult))
                return true;
            
            var releaseComparisionResult = compareEvrPart(firstEvr.Release, secondEvr.Release);
            if (releaseComparisionResult.Equals(expectedResult))
                return true;

            return false;
        }
    }

    public class InvalidOvalOperationException : Exception
    {
        private const string INVALID_OVAL_OPERATION = "The '{0}' operation is not allowed for EvrString Datatype";

        public InvalidOvalOperationException() : base(INVALID_OVAL_OPERATION) { }
    }
}
