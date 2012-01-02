using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modulo.Collect.Service.Tests.Helpers;
using System.IO;

namespace Modulo.Collect.Service.Tests
{
    [TestClass]
    public class OvalDefinitionsValidatorTests
    {
        private string AN_EMPTY_ERROR_LIST_EXPECTED_FAIL_MSG = "An empty error list was expected but it contains {0} items which on the first element is '{1}'";

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_validate_a_valid_OvalDefinitions()
        {
            var validOvalDefinitionsXml = new OvalDocumentLoader().GetFakeOvalDefinitions("oval.org.mitre.oval.def.5368.xml").GetDefinitionsXml();
            
            var ovalDefinitionsValidator = new OvalDefinitionsValidator(validOvalDefinitionsXml);

            Assert.IsNotNull(ovalDefinitionsValidator.Schema, "The Oval Definitions validation result cannot be null.");
            Assert.IsTrue(ovalDefinitionsValidator.Schema.IsValid, "The Oval Definitions Schema validation result must be TRUE.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_validate_an_OvalDefinitions_with_schema_errors()
        {
            var ovalDefinitionsWithInvalidSchemaAsXml = this.GetOvalDefinitionsXml("definitions", "oval.org.mitre.oval.def.5368.invalid.xml");

            var ovalDefinitionsValidator = new OvalDefinitionsValidator(ovalDefinitionsWithInvalidSchemaAsXml);

            Assert.IsNotNull(ovalDefinitionsValidator.Schema, "The Oval Definitions Schema validation result cannot be null.");
            Assert.IsFalse(ovalDefinitionsValidator.Schema.IsValid, "The Oval Definitions validation result must be FALSE.");
            var schemaErrorsList = ovalDefinitionsValidator.Schema.ErrorList;
            Assert.IsNotNull(schemaErrorsList, "The schema errors list must be created. SchemaErrors property cannot be null.");
            Assert.AreEqual(1, schemaErrorsList.Count(), "Unexpected schema errors list length.");
            Assert.IsFalse(String.IsNullOrWhiteSpace(schemaErrorsList.ElementAt(0)), "The schema errors list element should contains the validation error description.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_validate_an_OvalDefinitions_with_invalid_schematron()
        {
            var ovalDefinitionsWithInvalidSchematron = GetOvalDefinitionsXml("definitions", "definitions_with_deprecated_objects.xml");

            var schematronValidationResult = new OvalDefinitionsValidator(ovalDefinitionsWithInvalidSchematron).ValidateSchematron();

            Assert.IsNotNull(schematronValidationResult, "The Oval Definitions Schmetron validation result cannot be null.");
            Assert.IsFalse(schematronValidationResult.IsValid, "The schematron validation should be FALSE.");
            var schematronErrors = schematronValidationResult.ErrorList;
            Assert.IsNotNull(schematronErrors, "The schematron error list SHOULD NOT be null.");
            Assert.AreEqual(2, schematronErrors.Count(), "Unexpected schematron errors list length.");
            Assert.AreEqual("DEPRECATED OBJECT: wmi_object ID: oval:org.mitre.oval:obj:3000", schematronErrors.ElementAt(1), "Unexpected schematron error description was found in errors list.");
        }

        [TestMethod, Owner("lfernandes")]
        public void Should_be_possible_to_validate_an_OvalDefinitions_with_valid_schematron()
        {
            var validOvalDefinitionsXml = new OvalDocumentLoader().GetFakeOvalDefinitions("oval.org.mitre.oval.def.5368.xml").GetDefinitionsXml();
            var validator = new OvalDefinitionsValidator(validOvalDefinitionsXml);
            ValidationResultShouldBeValidFor(validator.Schema);

            var schematronValidationResult = validator.ValidateSchematron();

            ValidationResultShouldBeValidFor(schematronValidationResult);
        }

        private void ValidationResultShouldBeValidFor(OvalDefinitionsValidationResult validationResult)
        {
            Assert.IsTrue(validationResult.IsValid, "The Oval Definitions validation result must be TRUE.");
            var validationErrors = validationResult.ErrorList;
            var emptyErrorList = validationErrors == null || validationErrors.Count() == 0;
            if (!emptyErrorList)
                Assert.Fail(AN_EMPTY_ERROR_LIST_EXPECTED_FAIL_MSG, validationErrors.Count(), validationErrors.First());
        }

        private string GetOvalDefinitionsXml(string resourceFolder, string resouceName)
        {
            var resourcePath = String.Format("{0}.{1}", resourceFolder, resouceName);
            try
            {
                var ovalDefinitionsAsStream = new OvalDocumentLoader().GetStreamFrom(resourcePath);
                if (ovalDefinitionsAsStream == null)
                    Assert.Fail("A resource with name '{0}' could not be found.", resourcePath);

                var ovalDefinitionsAsXml = new StreamReader(ovalDefinitionsAsStream).ReadToEnd();
                Assert.IsNotNull(ovalDefinitionsAsXml, "Could not possible to get Oval Definitions Document from stream.");
                Assert.IsTrue(ovalDefinitionsAsXml.Contains("<oval_definitions "), "The document is not an Oval Definitions file. Missing <oval_definitions> begin tag.");
                Assert.IsTrue(ovalDefinitionsAsXml.Contains("</oval_definitions>"), "The document is not an Oval Definitions file. Missing </oval_definitions> closing tag.");

                return ovalDefinitionsAsXml;
            }
            catch (Exception ex)
            {
                Assert.Fail("An error occurred while trying to load resource '{0}': '{1}'.", resourcePath, ex.Message);
                return null;
            }
        }

        
    }
}
