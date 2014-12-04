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
using Modulo.Collect.OVAL.Definitions;
using System.Xml.Serialization;
using Modulo.Collect.OVAL.Schema;
using System.Xml;
using System.Xml.Schema;
using Modulo.Collect.OVAL.SystemCharacteristics;
using System.IO;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Variables;

namespace Modulo.Collect.OVAL.Results
{
    public partial class oval_results
    {
        public const int schemaMajorVersion = 5;

        public const int schemaMinorVersion = 10;

        public oval_variables EvaluatedExternalVariables { get; set; }

        public static oval_results GetOvalResultsFromStream(
            Stream resultDocument, out IEnumerable<string> schemaErrors)
        {
            var _schemaErrors = new List<string>();

            var ScOverrides = oval_system_characteristics.GetExportedScOverrides();
            oval_definitions.GetExportedDefinitionsOverrides(ScOverrides);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(oval_results), ScOverrides);
            var resolver = new ExtensibleXmlResourceResolver();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.XmlResolver = resolver;
            settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.AllowXmlAttributes;
            settings.ValidationEventHandler += (o, args) => { if (args.Severity == XmlSeverityType.Error) _schemaErrors.Add(args.Message); };
            XmlReader reader = XmlReader.Create(resultDocument, settings);
            oval_results result = xmlSerializer.Deserialize(reader) as oval_results;
            reader.Close();

            if (_schemaErrors.Count > 0)
                result = null;

            schemaErrors = _schemaErrors;
            return result;
        }

        /// <summary>
        /// Negates the Given Criteria Result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="negate">if set to <c>true</c> [negate].</param>
        public static ResultEnumeration NegateResult(ResultEnumeration result, bool negate)
        {
            if (negate)
            {
                if (result == ResultEnumeration.@true)
                    return ResultEnumeration.@false;
                else if (result == ResultEnumeration.@false)
                    return ResultEnumeration.@true;
                else
                    return result;
            }
            else
            {
                return result;
            }
        }

        public static ResultEnumeration CombineResultsByOperator(
            IEnumerable<ResultEnumeration> results, OperatorEnumeration op)
        {
            ResultEnumeration combinedResult = ResultEnumeration.unknown;

            if (results.Count() > 0)
            {
                // Get counts of result values
                int trueCount = 0;
                int falseCount = 0;
                int unknownCount = 0;
                int errorCount = 0;
                int notEvaluatedCount = 0;
                int notApplicableCount = 0;

                foreach (ResultEnumeration result in results)
                {
                    switch (result)
                    {
                        case ResultEnumeration.@true:
                            trueCount++;
                            break;
                        case ResultEnumeration.@false:
                            falseCount++;
                            break;
                        case ResultEnumeration.unknown:
                            unknownCount++;
                            break;
                        case ResultEnumeration.error:
                            errorCount++;
                            break;
                        case ResultEnumeration.notevaluated:
                            notEvaluatedCount++;
                            break;
                        case ResultEnumeration.notapplicable:
                            notApplicableCount++;
                            break;

                    }
                }

                // first check for a possible Not Applicable result
                if (notApplicableCount > 0 && notEvaluatedCount == 0 && falseCount == 0 && errorCount == 0 && unknownCount == 0 && trueCount == 0)
                {
                    return ResultEnumeration.notapplicable;
                }

                // Set the combined result
                if (op == OperatorEnumeration.AND)
                {
                    if (trueCount > 0 && falseCount == 0 && errorCount == 0 && unknownCount == 0 && notEvaluatedCount == 0)
                    {
                        combinedResult = ResultEnumeration.@true;
                    }
                    else if (falseCount > 0)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if (falseCount == 0 && errorCount > 0)
                    {
                        combinedResult = ResultEnumeration.error;
                    }
                    else if (unknownCount > 0 && falseCount == 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.unknown;
                    }
                    else if (notEvaluatedCount > 0 && falseCount == 0 && errorCount == 0 && unknownCount == 0)
                    {
                        combinedResult = ResultEnumeration.notevaluated;
                    }
                }
                else if (op == OperatorEnumeration.ONE)
                {

                    if (trueCount == 1 && falseCount >= 0 && errorCount == 0 && unknownCount == 0 && notEvaluatedCount == 0 && notApplicableCount >= 0)
                    {
                        combinedResult = ResultEnumeration.@true;
                    }
                    else if (trueCount >= 2 && falseCount >= 0 && errorCount >= 0 && unknownCount >= 0 && notEvaluatedCount >= 0 && notApplicableCount >= 0)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if (trueCount == 0 && falseCount >= 0 && errorCount == 0 && unknownCount == 0 && notEvaluatedCount == 0 && notApplicableCount >= 0)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if ((trueCount == 0 || trueCount == 1) && falseCount >= 0 && errorCount >= 1 && unknownCount >= 0 && notEvaluatedCount >= 0 && notApplicableCount >= 0)
                    {
                        combinedResult = ResultEnumeration.error;
                    }
                    else if ((trueCount == 0 || trueCount == 1) && falseCount >= 0 && errorCount == 0 && unknownCount >= 1 && notEvaluatedCount >= 0 && notApplicableCount >= 0)
                    {
                        combinedResult = ResultEnumeration.unknown;
                    }
                    else if ((trueCount == 0 || trueCount == 1) && falseCount >= 0 && errorCount == 0 && unknownCount == 0 && notEvaluatedCount >= 1 && notApplicableCount >= 0)
                    {
                        combinedResult = ResultEnumeration.notevaluated;
                    }
                }
                else if (op == OperatorEnumeration.OR)
                {
                    if (trueCount > 0)
                    {
                        combinedResult = ResultEnumeration.@true;
                    }
                    else if (falseCount > 0 && trueCount == 0 && unknownCount == 0 && errorCount == 0 && notEvaluatedCount == 0)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if (errorCount > 0 && trueCount == 0)
                    {
                        combinedResult = ResultEnumeration.error;
                    }
                    else if (unknownCount > 0 && trueCount == 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.unknown;
                    }
                    else if (notEvaluatedCount > 0 && unknownCount == 0 && trueCount == 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.notevaluated;
                    }
                }
                else if (op == OperatorEnumeration.XOR)
                {
                    if (trueCount % 2 == 1 && notEvaluatedCount == 0 && unknownCount == 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.@true;
                    }
                    else if (trueCount % 2 == 0 && notEvaluatedCount == 0 && unknownCount == 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if (errorCount > 0)
                    {
                        combinedResult = ResultEnumeration.error;
                    }
                    else if (unknownCount > 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.unknown;
                    }
                    else if (notEvaluatedCount > 0 && unknownCount == 0 && errorCount == 00)
                    {
                        combinedResult = ResultEnumeration.notevaluated;
                    }
                }

            }

            return combinedResult;
        }

        public static ResultEnumeration CombineResultsByCheck(
            IEnumerable<ResultEnumeration> results, CheckEnumeration check)
        {
            ResultEnumeration combinedResult = ResultEnumeration.unknown;

            if (results.Count() > 0)
            {
                // Get counts of result values
                int trueCount = 0;
                int falseCount = 0;
                int unknownCount = 0;
                int errorCount = 0;
                int notEvaluatedCount = 0;
                int notApplicableCount = 0;

                foreach (ResultEnumeration result in results)
                {

                    if (result == ResultEnumeration.@true)
                    {
                        trueCount++;
                    }
                    else if (result == ResultEnumeration.@false)
                    {
                        falseCount++;
                    }
                    else if (result == ResultEnumeration.unknown)
                    {
                        unknownCount++;
                    }
                    else if (result == ResultEnumeration.error)
                    {
                        errorCount++;
                    }
                    else if (result == ResultEnumeration.notevaluated)
                    {
                        notEvaluatedCount++;
                    }
                    else if (result == ResultEnumeration.notapplicable)
                    {
                        notApplicableCount++;
                    }
                }

                // first check for a possible Not Applicable result
                if (notApplicableCount > 0 && notEvaluatedCount == 0 && falseCount == 0 && errorCount == 0 && unknownCount == 0 && trueCount == 0)
                {
                    return ResultEnumeration.notapplicable;
                }

                // Set the combined result
                if (check == CheckEnumeration.all)
                {
                    if (trueCount > 0 && falseCount == 0 && errorCount == 0 && unknownCount == 0 && notEvaluatedCount == 0)
                    {
                        combinedResult = ResultEnumeration.@true;
                    }
                    else if (falseCount > 0)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if (falseCount == 0 && errorCount > 0)
                    {
                        combinedResult = ResultEnumeration.error;
                    }
                    else if (unknownCount > 0 && falseCount == 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.unknown;
                    }
                    else if (notEvaluatedCount > 0 && falseCount == 0 && errorCount == 0 && unknownCount == 0)
                    {
                        combinedResult = ResultEnumeration.notevaluated;
                    }
                }
                else if (check == CheckEnumeration.atleastone)
                {
                    if (trueCount > 0)
                    {
                        combinedResult = ResultEnumeration.@true;
                    }
                    else if (falseCount > 0 && trueCount == 0 && unknownCount == 0 && errorCount == 0 && notEvaluatedCount == 0)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if (errorCount > 0 && trueCount == 0)
                    {
                        combinedResult = ResultEnumeration.error;
                    }
                    else if (unknownCount > 0 && trueCount == 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.unknown;
                    }
                    else if (notEvaluatedCount > 0 && unknownCount == 0 && trueCount == 0 && errorCount == 0)
                    {
                        combinedResult = ResultEnumeration.notevaluated;
                    }
                }
                else if (check == CheckEnumeration.nonesatisfy)
                {
                    if (trueCount > 0)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if (errorCount > 0 && trueCount == 0)
                    {
                        combinedResult = ResultEnumeration.error;
                    }
                    else if (unknownCount > 0 && errorCount == 0 && trueCount == 0)
                    {
                        combinedResult = ResultEnumeration.unknown;
                    }
                    else if (notEvaluatedCount > 0 && unknownCount == 0 && errorCount == 0 && trueCount == 0)
                    {
                        combinedResult = ResultEnumeration.notevaluated;
                    }
                    else if (falseCount > 0 && notEvaluatedCount == 0 && unknownCount == 0 && errorCount == 0 && trueCount == 0)
                    {
                        combinedResult = ResultEnumeration.@true;
                    }
                }
                else if (check == CheckEnumeration.onlyone)
                {
                    if (trueCount == 1 && unknownCount == 0 && errorCount == 0 && notEvaluatedCount == 0)
                    {
                        combinedResult = ResultEnumeration.@true;
                    }
                    else if (trueCount > 1)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                    else if (errorCount > 0 && trueCount < 2)
                    {
                        combinedResult = ResultEnumeration.error;
                    }
                    else if (unknownCount > 0 && errorCount == 0 && trueCount < 2)
                    {
                        combinedResult = ResultEnumeration.unknown;
                    }
                    else if (notEvaluatedCount > 0 && unknownCount == 0 && errorCount == 0 && trueCount < 2)
                    {
                        combinedResult = ResultEnumeration.notevaluated;
                    }
                    else if (falseCount > 0 && trueCount != 1)
                    {
                        combinedResult = ResultEnumeration.@false;
                    }
                }
            }

            return combinedResult;
        }

        public static oval_results CreateFromDocuments(
            Definitions.oval_definitions definitions, 
            oval_system_characteristics systemcharacteristics,
            oval_variables evaluatedExternalVariables)
        {
            oval_results newResult = new oval_results();
            newResult.generator = DocumentHelpers.GetDefaultGenerator();
            newResult.oval_definitions = definitions;
            newResult.results =  new List<SystemType> { new SystemType() };
            newResult.results[0].oval_system_characteristics = systemcharacteristics;
            newResult.EvaluatedExternalVariables = evaluatedExternalVariables;

            // Generate new Definition Result entries
            foreach (var definition in definitions.definitions)
            {
                if ((definition.criteria == null) || (definition.criteria.Items == null) || (definition.criteria.Items.Count() < 1))
                    continue;
                var newDefinitionResult = new DefinitionType(definition);
                newResult.results[0].definitions.Add(newDefinitionResult);
            }
            // Generate new Test Result entries
            foreach (var test in definitions.tests)
            {
                var newTestResult = new TestType(test);
                newResult.results[0].tests.Add(newTestResult);
            }
            return newResult;
        }

        public string GetResultsXML()
        {
            string resultXML;

            var ScOverrides = oval_system_characteristics.GetExportedScOverrides();
            oval_definitions.GetExportedDefinitionsOverrides(ScOverrides);

            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(oval_results), ScOverrides);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xmlSerializer.Serialize(xmlTextWriter, this);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            resultXML = new System.Text.UTF8Encoding().GetString(memoryStream.ToArray());
            return resultXML;
        }

        public void Analyze()
        {
            foreach (var definition in this.results[0].definitions)
            {
                definition.Analyze(this);
            }

            foreach (var test in this.results[0].tests.Where(t => t.result == ResultEnumeration.unknown))
            {
                test.Analyze(this);
            }
        }
    }
}
