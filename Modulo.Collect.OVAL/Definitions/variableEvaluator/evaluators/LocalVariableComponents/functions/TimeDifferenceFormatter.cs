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
using System.Globalization;

namespace Modulo.Collect.OVAL.Definitions.variableEvaluator.evaluators.LocalVariableComponents.functions
{
    public class TimeDifferenceFormatter
    {
        /// <summary>
        /// Gets the date in format specified in the definitions document.
        /// the formats are defined in DateTimeFormatEnumeration 
        /// </summary>
        /// <param name="aDate">A date.</param>
        /// <param name="dateTimeFormatEnumeration">The date time format enumeration.</param>
        /// <returns></returns>
        public DateTime GetDateInFormat(string aDate, DateTimeFormatEnumeration dateTimeFormatEnumeration)
        {
            try
            {
                return ConvertToDateInTheCorrectFormat(aDate, dateTimeFormatEnumeration);
            }
            catch (NotSupportedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new InvalidCastException(string.Format("The value {0} is not a date.", aDate), e);
            }
        }

        private DateTime ConvertToDateInTheCorrectFormat(string aDate, DateTimeFormatEnumeration dateTimeFormatEnumeration)
        {
            if (dateTimeFormatEnumeration == DateTimeFormatEnumeration.year_month_day)
                return this.GetDateInYearMonthDayFormat(aDate);
            else if (dateTimeFormatEnumeration == DateTimeFormatEnumeration.month_day_year)
                return this.GetDateInMonthDayYear(aDate);
            else if (dateTimeFormatEnumeration == DateTimeFormatEnumeration.day_month_year)
                return this.GetDateInDayMonthYear(aDate);
            else if (dateTimeFormatEnumeration == DateTimeFormatEnumeration.seconds_since_epoch)
                return this.GetDateGivenSecondsSinceEpoch(aDate);
            else if (dateTimeFormatEnumeration == DateTimeFormatEnumeration.win_filetime)
                return this.GetDateGivenWinFileTime(aDate);
            else
                throw new NotSupportedException("The dateTimeFormat is not supported.");
        }
       
        private DateTime GetDateInMonthDayYear(string aDate)
        {            
            // not suported yet
            //"NameOfMonth, dd yyyy hh:mm:ss" ,
            //"NameOfMonth, dd yyyy", 
            //"AbreviatedNameOfMonth, dd yyyy hh:mm:ss"
            //"AbreviatedNameOfMonth, dd yyyy"
            string[] formats = new string[] { "MM/dd/yyyy HH:mm:ss", "MM/dd/yyyy", "MM-dd-yyyy HH:mm:ss", "MM-dd-yyyy" };
            return ConvertToDate(aDate, formats);
        }

        private DateTime GetDateInDayMonthYear(string aDate)
        {
            string[] formats = new string[] { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy", "dd-MM-yyyy HH:mm:ss","dd-MM-yyyy" };
            return ConvertToDate(aDate, formats);
        }


        private DateTime GetDateInYearMonthDayFormat(string aDate)
        {            
            string[] formats = new string[] { "yyyyMMdd", "yyyyMMddThhmmss", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd" };
            return ConvertToDate(aDate, formats);
        }

        private static DateTime ConvertToDate(string aDate, string[] formats)
        {
            DateTime formattedDate = DateTime.ParseExact(aDate, formats, System.Globalization.CultureInfo.CurrentCulture, DateTimeStyles.None);
            return formattedDate;
        }

        private DateTime GetDateGivenSecondsSinceEpoch(string aDate)
        {
            double seconds = double.Parse(aDate);
            DateTime baseDate = new DateTime(1970, 01, 01, 0, 0, 0);
            return baseDate.AddSeconds(seconds);
            
        }

        private DateTime GetDateGivenWinFileTime(string aDate)
        {
            long fileTime = long.Parse(aDate);
            return DateTime.FromFileTime(fileTime);
        }

        
        
    }
}

