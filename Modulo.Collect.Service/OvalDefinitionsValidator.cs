using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using System.IO;

namespace Modulo.Collect.Service
{
    public class OvalDefinitionsValidator
    {
        private string OvalDefinitionsXml;
        private string DefinitionId;

        public OvalDefinitionsValidationResult Schema { get; private set; }

        public OvalDefinitionsValidator(string ovalDefinitionsXml, string definitionId = "0")
        {
            this.DefinitionId = definitionId;
            this.OvalDefinitionsXml = ovalDefinitionsXml;   
            this.ValidateSchema();
        }

        public OvalDefinitionsValidationResult ValidateSchematron()
        {
            string schematronErrors;
            IEnumerable<string> validationResultErrorList = null;
            var validationResult = new Schematron().Validate(this.DefinitionId, this.OvalDefinitionsXml, out schematronErrors);
            if (!validationResult)
                validationResultErrorList = schematronErrors.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            
            return new OvalDefinitionsValidationResult(validationResultErrorList);
        }

        private void ValidateSchema()
        {
            IEnumerable<string> schemaErrors;
            var ovalDefinitionsAsStream = new MemoryStream(Encoding.UTF8.GetBytes(this.OvalDefinitionsXml));

            var ovalDefinitions = oval_definitions.GetOvalDefinitionsFromStream(ovalDefinitionsAsStream, out schemaErrors);

            this.Schema = new OvalDefinitionsValidationResult(schemaErrors);
        }
    }

    public class OvalDefinitionsValidationResult
    {
        public Boolean IsValid { get; private set; }

        public IEnumerable<String> ErrorList { get; private set; }

        public OvalDefinitionsValidationResult(IEnumerable<String> errorList = null)
        {
            this.IsValid = errorList == null || errorList.Count() == 0;
            if (errorList != null)
                this.ErrorList = new List<String>(errorList);
        }
    }
}
