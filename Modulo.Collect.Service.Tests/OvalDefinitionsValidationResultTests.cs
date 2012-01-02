using System.Linq;
using Modulo.Collect.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Modulo.Collect.Service.Tests
{
    [TestClass]
    public class OvalDefinitionsValidationResultTests
    {
        private const string ERROR_LIST_PROPERTY_SHOULD_BE_NULL = "ErrorList property should be null.";
        private const string ERROR_LIST_PROPERTY_SHOULD_NOT_BE_NULL = "ErrorList property SHOULD NOT be null.";
        private const string NO_ITEMS_ARE_EXPECTED_FAIL_MSG = "No items are expected in 'ErrorList' property.";
        private const string UNEXPECTED_ERROR_ELEMENT_WAS_FOUND = "Unexpected error description was found in error list (ErrorList property).";
        
        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_create_an_instance_of_OvalDefinitionsValidationResult_using_default_constructor()
        {
            var instanceOfOvalDefintionsValidationResult = new OvalDefinitionsValidationResult();

            Assert.IsTrue(instanceOfOvalDefintionsValidationResult.IsValid, "When no error list was passed, the IsValid property must TRUE.");
            Assert.IsNull(instanceOfOvalDefintionsValidationResult.ErrorList, ERROR_LIST_PROPERTY_SHOULD_BE_NULL);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_OvalDefinitionsValidationResult_was_created_passing_an_empty_error_list_the_validation_result_must_be_TRUE()
        {
            var anEmptySchemaErrorsList = new String[] { };

            var instanceOfOvalDefintionsValidationResult = new OvalDefinitionsValidationResult(anEmptySchemaErrorsList);

            Assert.IsTrue((bool)instanceOfOvalDefintionsValidationResult.IsValid, "If an empty error list was passed, the validation result must be TRUE.");
            var schemaErrorsList = instanceOfOvalDefintionsValidationResult.ErrorList;
            Assert.IsNotNull(schemaErrorsList, ERROR_LIST_PROPERTY_SHOULD_BE_NULL);
            Assert.AreEqual(0, schemaErrorsList.Count(), NO_ITEMS_ARE_EXPECTED_FAIL_MSG);
        }

        [TestMethod, Owner("lfernandes")]
        public void If_OvalDefinitionsValidationResult_was_created_passing_a_nonempty_erros_list_the_validation_result_must_be_FALSE()
        {
            var anSchemaErrorsList = new String[] { "Schema Error 1", "Schema Error 2" };

            var instanceOfOvalDefintionsValidationResult = new OvalDefinitionsValidationResult(anSchemaErrorsList);

            Assert.IsFalse((bool)instanceOfOvalDefintionsValidationResult.IsValid, "If a non-empty schema error list was passed, the expected value for IsValid property is FALSE.");
            var schemaErrorsList = instanceOfOvalDefintionsValidationResult.ErrorList;
            Assert.IsNotNull(schemaErrorsList, ERROR_LIST_PROPERTY_SHOULD_NOT_BE_NULL);
            Assert.AreEqual(2, schemaErrorsList.Count(), "Unexpected errors count was found in SchemaErrors property.");
            Assert.AreEqual("Schema Error 1", schemaErrorsList.ElementAt(0), UNEXPECTED_ERROR_ELEMENT_WAS_FOUND);
            Assert.AreEqual("Schema Error 2", schemaErrorsList.ElementAt(1), UNEXPECTED_ERROR_ELEMENT_WAS_FOUND);
        }
    }
}
