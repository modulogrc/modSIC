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
using System.Text.RegularExpressions;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Probe.Common.BasicClasses;
using Modulo.Collect.Probe.Common.Exceptions;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Independent.Common;
using System.IO;
using System.Text;
using Modulo.Collect.Probe.Independent.Common.File;



namespace Modulo.Collect.Probe.Independent.TextFileContent54
{
    public enum SearchTextFileContentParameters
    {
        filepath,
        pattern,
        instance,
        multiline,
        singleline
    };

    public class TextFileContentObjectCollector : BaseObjectCollector
    {
        public TargetInfo TargetInfo { get; set; }

        public IFileProvider FileContentProvider { get; set; }

        private const string ERROR_MESSAGE = "[TextFileContentObjectCollector] - An error occurred while trying to collect text file contents: '{0}'";


        public static Dictionary<String, Object> GetDictionaryWithParametersToSearchTextFileConten(
            string filepath,
            string pattern,
            int instance,
            bool multiline = true,
            bool singleline = false)
        {
            var parameters = new Dictionary<String, Object>();
            parameters.Add(SearchTextFileContentParameters.filepath.ToString(), filepath);
            parameters.Add(SearchTextFileContentParameters.instance.ToString(), instance);
            parameters.Add(SearchTextFileContentParameters.pattern.ToString(), pattern);
            parameters.Add(SearchTextFileContentParameters.multiline.ToString(), multiline);
            parameters.Add(SearchTextFileContentParameters.singleline.ToString(), singleline);
            return parameters;
        }

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            //try
            //{
                var filepathParamName = SearchTextFileContentParameters.filepath.ToString();
                var instanceParamName = SearchTextFileContentParameters.instance.ToString();
                var patternParamName = SearchTextFileContentParameters.pattern.ToString();
                var multilineParamName = SearchTextFileContentParameters.multiline.ToString();
                var singlelineParamName = SearchTextFileContentParameters.singleline.ToString();
            
                var filepath = parameters[filepathParamName].ToString();
                var pattern = parameters[patternParamName].ToString();
                var instance = (int)parameters[instanceParamName];
                var multiline = (bool)parameters[multilineParamName];
                var singleline = (bool)parameters[singlelineParamName];

                var fileContentLines = this.FileContentProvider.GetFileLinesContentFromHost(filepath);
                

                return this.GetMatchInstances(fileContentLines.ToArray(), pattern, instance, multiline, singleline);
            //}
            //catch (Exception ex)
            //{
            //    try { System.IO.File.WriteAllText("c:\\temp\\err.log", ex.ToString()); }
            //    catch (Exception) { }
            //    throw new ProbeException(string.Format(ERROR_MESSAGE, ex.ToString()));
            //}
        }

        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            base.ExecutionLogBuilder.CollectingDataFrom(this.TargetInfo.GetAddress());
            base.ExecutionLogBuilder.AddInfo(string.Format("Searching contents on '{0}' file", ((textfilecontent_item)systemItem).GetCompleteFilepath()));

            // In this special case, the collect of text file contents was done 
            // during generation of items to collect (see TextFileContentItemTypeGenerator).
            ConfigureFilepathEnities((textfilecontent_item)systemItem);
            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
        }


        private string[] GetMatchInstances(string[] fileContentLines, string patternValue, int instance, bool multiline, bool singleline)
        {
            var regexopt = RegexOptions.None;

            if (multiline)
                regexopt |= RegexOptions.Multiline;

            if (singleline)
                regexopt |= RegexOptions.Singleline;

            var matchLines = new List<String>();
            
            var fileContent = string.Join(Environment.NewLine, fileContentLines);
            var regexResult = new Regex(patternValue, regexopt).Matches(fileContent);
            foreach (Match matchItem in regexResult)
                if (matchItem.Success)
                    matchLines.Add(matchItem.Value);

            if (instance <= 0)
                return matchLines.ToArray();

            var result = new List<String>();
            for (int i = 0; i < matchLines.Count; i++)
                if (i.Equals(instance - 1))
                    result.Add(matchLines.ElementAt(i));

            return result.ToArray();
        }

        private void ConfigureFilepathEnities(textfilecontent_item textFileContentItem)
        {
            var completeFilepath = textFileContentItem.GetCompleteFilepath();
            textFileContentItem.filepath = OvalHelper.CreateItemEntityWithStringValue(completeFilepath);
            if (String.IsNullOrWhiteSpace(completeFilepath))
                return;

            var directory = Path.GetDirectoryName(completeFilepath);
            textFileContentItem.path = OvalHelper.CreateItemEntityWithStringValue(directory);

            var filename = Path.GetFileName(completeFilepath);
            textFileContentItem.filename = OvalHelper.CreateItemEntityWithStringValue(filename);
        }
    }
}
