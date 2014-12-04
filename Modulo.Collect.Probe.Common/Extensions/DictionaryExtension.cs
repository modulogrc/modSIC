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

namespace Modulo.Collect.Probe.Common.Extensions
{
    public static class DictionaryExtension
    {
        public static Dictionary<string, string> AddStringIfItIsNotNullToDictionary(this Dictionary<string, string> dictionary, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                dictionary.Add(key, value);
            
            return dictionary;
        }


        /// <summary>
        /// Returns object value as string.
        /// </summary>
        /// <param name="key">The key value of KeyValuePair item</param>
        /// <returns>Returns object value from Dictionary<string, object> as string. If object value is null, it returns a empty string.</returns>
        public static String GetObjectValueOrDefaultAsString(this Dictionary<string, object> dictionary, string key)
        {
            return GetObjectValueOrDefaultAsString(dictionary, key, string.Empty);
        }

        /// <summary>
        /// Returns object value as string or a default value.
        /// </summary>
        /// <param name="key">The key value of KeyValuePair item</param>
        /// <param name="defaultValue">The default value to return if object value is null.</param>
        /// <returns>Returns object value from Dictionary<string, object> as string. If object value is null, it returns the given default value.</returns>
        public static String GetObjectValueOrDefaultAsString(this Dictionary<string, object> dictionary, string key, string defaultValue)
        {
            object value;
            dictionary.TryGetValue(key, out value);
            return (value == null) ? defaultValue : value.ToString();
        }
    }
}
