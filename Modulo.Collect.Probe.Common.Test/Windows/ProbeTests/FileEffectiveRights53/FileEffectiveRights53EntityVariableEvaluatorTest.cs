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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Probe.Windows.FileEffectiveRights53;
using Modulo.Collect.Probe.Windows.Test.helpers;
using Modulo.Collect.Probe.Common.Test.Helpers;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.Windows;

namespace Modulo.Collect.Probe.Windows.Test.FileEffectiveRights53
{
    public static class ConstantHelper
    {
        public static string DEFINITIONS_SIMPLE = "definitionsSimple";

        public static string OBJECT_ID_1000 = "oval:modulo:obj:1000";
        public static string OBJECT_ID_1010 = "oval:modulo:obj:1010";
        public static string OBJECT_ID_1020 = "oval:modulo:obj:1020";
        public static string OBJECT_ID_1030 = "oval:modulo:obj:1030";
        public static string OBJECT_ID_1040 = "oval:modulo:obj:1040";
        public static string OBJECT_ID_1050 = "oval:modulo:obj:1050";
        public static string OBJECT_ID_1060 = "oval:modulo:obj:1060";
        public static string OBJECT_ID_1070 = "oval:modulo:obj:1070";
        public static string OBJECT_ID_1071 = "oval:modulo:obj:1071";
        public static string OBJECT_ID_1080 = "oval:modulo:obj:1080";
        public static string OBJECT_ID_1090 = "oval:modulo:obj:1090";
        
        public static string FilepathEntityName = fileeffectiverights53_object_ItemsChoices.filepath.ToString();
        public static string PathEntityName = fileeffectiverights53_object_ItemsChoices.path.ToString();
        public static string FilenameEntityName = fileeffectiverights53_object_ItemsChoices.filename.ToString();
        public static string TrusteeSIDEntityName = fileeffectiverights53_object_ItemsChoices.trustee_sid.ToString();
    }

    [TestClass]
    public class FileEffectiveRights53EntityVariableEvaluatorTest
    {
        private oval_definitions Definitions;

        public FileEffectiveRights53EntityVariableEvaluatorTest()
        {
            this.Definitions = ProbeHelper.GetFakeOvalDefinitions("definitionsSimple");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variable_for_filepath_entity()
        {
            string variableID;
            var sourceObjectType = this.GetObjectByID(ConstantHelper.OBJECT_ID_1010);
            var variableEvaluator = this.GetVariableEntityEvaluator(sourceObjectType.id, ConstantHelper.FilepathEntityName, out variableID);
            
            var result = variableEvaluator.ProcessVariables(sourceObjectType);

            DoBasicAssertForVariableProcessing(result, 1, typeof(fileeffectiverights53_object));
            AssertObjectEntity(result.First(), ConstantHelper.FilepathEntityName, this.GetVariableValue(variableID));
            AssertObjectEntityFromSourceObject(result.First(), ConstantHelper.TrusteeSIDEntityName, sourceObjectType);
            AssertCorrectnessOfFileEntities((fileeffectiverights53_object)result.First());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variable_for_path_entity()
        {
            string variableID;
            var sourceObjectType = this.GetObjectByID(ConstantHelper.OBJECT_ID_1020);
            var variableEvaluator = this.GetVariableEntityEvaluator(sourceObjectType.id, ConstantHelper.PathEntityName, out variableID);
            
            var result = variableEvaluator.ProcessVariables(sourceObjectType);

            DoBasicAssertForVariableProcessing(result, 1, typeof(fileeffectiverights53_object));
            AssertObjectEntity(result.First(), ConstantHelper.PathEntityName, this.GetVariableValue(variableID));
            AssertObjectEntityFromSourceObject(result.First(), ConstantHelper.FilenameEntityName, sourceObjectType);
            AssertObjectEntityFromSourceObject(result.First(), ConstantHelper.TrusteeSIDEntityName, sourceObjectType);
            AssertCorrectnessOfFileEntities((fileeffectiverights53_object)result.First());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variable_for_filename_entity()
        {
            string variableID;
            var sourceObjectType = this.GetObjectByID(ConstantHelper.OBJECT_ID_1030);
            var variableEvaluator = this.GetVariableEntityEvaluator(sourceObjectType.id, ConstantHelper.FilenameEntityName, out variableID);

            var result = variableEvaluator.ProcessVariables(sourceObjectType);

            DoBasicAssertForVariableProcessing(result, 1, typeof(fileeffectiverights53_object));
            AssertObjectEntity(result.First(), ConstantHelper.FilenameEntityName, this.GetVariableValue(variableID));
            AssertObjectEntityFromSourceObject(result.First(), ConstantHelper.PathEntityName, sourceObjectType);
            AssertObjectEntityFromSourceObject(result.First(), ConstantHelper.TrusteeSIDEntityName, sourceObjectType);
            AssertCorrectnessOfFileEntities((fileeffectiverights53_object)result.First());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variable_for_trusteeSID_entity()
        {
            string variableID;
            var sourceObjectType = this.GetObjectByID(ConstantHelper.OBJECT_ID_1040);
            var variableEvaluator = this.GetVariableEntityEvaluator(sourceObjectType.id, ConstantHelper.TrusteeSIDEntityName, out variableID);

            var result = variableEvaluator.ProcessVariables(sourceObjectType);

            DoBasicAssertForVariableProcessing(result, 1, typeof(fileeffectiverights53_object));
            AssertObjectEntity(result.First(), ConstantHelper.TrusteeSIDEntityName, this.GetVariableValue(variableID));
            AssertObjectEntityFromSourceObject(result.First(), ConstantHelper.FilepathEntityName, sourceObjectType);
            AssertCorrectnessOfFileEntities((fileeffectiverights53_object)result.First());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variable_for_all_entities_at_same_time()
        {
            var sourceObject = ProbeHelper.GetOvalComponentByOvalID(this.Definitions, ConstantHelper.OBJECT_ID_1050);
            var variablesAndValues = this.CreateVariableAndValuesDictionary(sourceObject.id);
            var evaluatedVariables = VariableHelper.CreateEvaluatedVariables(sourceObject.id, variablesAndValues);
            var variableEvaluator = new FileEffectiveRights53EntityVariableEvaluator(evaluatedVariables);

            var evaluationResult = variableEvaluator.ProcessVariables(sourceObject);

            DoBasicAssertForVariableProcessing(evaluationResult, 1, typeof(fileeffectiverights53_object));
            var pathVariableValue = this.GetVariableValue(variablesAndValues.ElementAt(0).Key);
            AssertObjectEntity(evaluationResult.First(), ConstantHelper.PathEntityName, pathVariableValue);
            var filenameVariableValue = this.GetVariableValue(variablesAndValues.ElementAt(1).Key);
            AssertObjectEntity(evaluationResult.First(), ConstantHelper.FilenameEntityName, filenameVariableValue);
            var trusteeSIDVariableValue = this.GetVariableValue(variablesAndValues.ElementAt(2).Key);
            AssertObjectEntity(evaluationResult.First(), ConstantHelper.TrusteeSIDEntityName, trusteeSIDVariableValue);
            AssertCorrectnessOfFileEntities((fileeffectiverights53_object)evaluationResult.First());
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_evaluate_variable_for_path_entity_when_filename_is_nil()
        {
            var sourceObject = ProbeHelper.GetOvalComponentByOvalID(this.Definitions, ConstantHelper.OBJECT_ID_1071);
            var pathVariableID = this.GetVariableReferenceID(sourceObject.id, ConstantHelper.PathEntityName);
            var pathVariableValue = this.GetVariableValue(pathVariableID);
            var variablesAndValues = new Dictionary<String, IEnumerable<String>>();
            variablesAndValues.Add(pathVariableID, new string[] { pathVariableValue });
            var evaluatedVariables = VariableHelper.CreateEvaluatedVariables(sourceObject.id, variablesAndValues);
            var variableEvaluator = new FileEffectiveRights53EntityVariableEvaluator(evaluatedVariables);

            var evaluationResult = variableEvaluator.ProcessVariables(sourceObject);

            Assert.IsNotNull(evaluationResult);
            Assert.AreEqual(1, evaluationResult.Count());
            var entities = evaluationResult.Single().GetEntityBaseTypes();
            Assert.AreEqual(3, entities.Count());
            var pathEntity = entities.ElementAt(0);
            Assert.IsNotNull(pathEntity);
            Assert.AreEqual(GetVariableValue(GetVariableReferenceID(sourceObject.id, "path")), pathEntity.Value);
            var filenameEntity = entities.ElementAt(1);
            Assert.IsNotNull(filenameEntity);
            Assert.AreEqual(string.Empty, filenameEntity.Value);
            var trusteeSidEntity = entities.ElementAt(2);
            Assert.IsNotNull(trusteeSidEntity);
            Assert.AreEqual("S-1-15-500", trusteeSidEntity.Value);
        }

        private void DoBasicAssertForVariableProcessing(IEnumerable<ObjectType> resultToAssert, int expectedResultCount, Type expectedObjectType)
        {
            Assert.IsNotNull(resultToAssert, "The result of variable evaluation cannot be null");
            Assert.AreEqual(expectedResultCount, resultToAssert.Count(), "Unexpected object count was found after variable evaluation.");
            Assert.IsInstanceOfType(resultToAssert.First(), expectedObjectType, "Unexpected object type was found after variable evaluation.");
        }

        private void AssertObjectEntity(ObjectType createdObjectType, string entityName, string expectedEntityValue)
        {
            var entityToAssert = ((fileeffectiverights53_object)createdObjectType).GetAllObjectEntities()[entityName];
            Assert.AreEqual(expectedEntityValue, entityToAssert.Value, string.Format("Unexpected object entity value ('{0}') was found after variable evaluation.", entityName));
        }

        private void AssertObjectEntityFromSourceObject(ObjectType createdObjectType, string entityName, ObjectType sourceObjectType)
        {
            var entityToAssert = ((fileeffectiverights53_object)createdObjectType).GetAllObjectEntities()[entityName];
            var sourceEntity = ((fileeffectiverights53_object)sourceObjectType).GetAllObjectEntities()[entityName];
            Assert.AreEqual(sourceEntity.Value, entityToAssert.Value, string.Format("Unexpected object entity value ('{0}') was found after variable evaluation.", entityName));
        }

        private void AssertCorrectnessOfFileEntities(fileeffectiverights53_object createdObjectType)
        {
            if (createdObjectType.IsFilePathDefined())
            {
                Assert.IsNull(createdObjectType.GetAllObjectEntities()[ConstantHelper.PathEntityName]);
                Assert.IsNull(createdObjectType.GetAllObjectEntities()[ConstantHelper.FilenameEntityName]); 
                return;
            }

            Assert.IsNull(createdObjectType.GetAllObjectEntities()[ConstantHelper.FilepathEntityName]);
        }


        private FileEffectiveRights53EntityVariableEvaluator GetVariableEntityEvaluator(string objectID, string entityName, out string variableID)
        {
            var objectType  = ProbeHelper.GetOvalComponentByOvalID(this.Definitions, objectID);

            variableID = this.GetVariableReferenceID(objectID, entityName);
            var evaluatedVariables = VariableHelper.CreateVariableWithOneValue(objectType.id, variableID, this.GetVariableValue(variableID));
            
            return new FileEffectiveRights53EntityVariableEvaluator(evaluatedVariables);
        }

        private string GetVariableReferenceID(string objectID, string entityName)
        {
            var objectType = (fileeffectiverights53_object)ProbeHelper.GetOvalComponentByOvalID(this.Definitions, objectID);
            return objectType.GetAllObjectEntities()[entityName].var_ref;
        }

        private string GetVariableValue(string variableID)
        {
            return VariableHelper.ExtractVariableValueFromConstantVariable(this.Definitions, variableID);
        }

        private ObjectType GetObjectByID(string objectID)
        {
            return ProbeHelper.GetOvalComponentByOvalID(this.Definitions, objectID);
        }

        private Dictionary<String, IEnumerable<String>> CreateVariableAndValuesDictionary(string objectID)
        {
            var pathVariableID = this.GetVariableReferenceID(objectID, ConstantHelper.PathEntityName);
            var filenameVariableID = this.GetVariableReferenceID(objectID, ConstantHelper.FilenameEntityName);
            var trusteeSIDVariableID = this.GetVariableReferenceID(objectID, ConstantHelper.TrusteeSIDEntityName);

            var dictionaryOfVariables = new Dictionary<String, IEnumerable<String>>();
            dictionaryOfVariables.Add(pathVariableID, new string[] { this.GetVariableValue(pathVariableID) });
            dictionaryOfVariables.Add(filenameVariableID, new string[] { this.GetVariableValue(filenameVariableID) });
            dictionaryOfVariables.Add(trusteeSIDVariableID, new string[] { this.GetVariableValue(trusteeSIDVariableID) });

            return dictionaryOfVariables;
        }
    }
}
