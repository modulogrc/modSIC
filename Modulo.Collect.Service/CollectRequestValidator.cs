using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Service.Contract;
using Modulo.Collect.Service.Exceptions;

namespace Modulo.Collect.Service
{
    public class CollectRequestValidator
    {
        private DefinitionInfo[] RequestedDefinitions;
        

        public CollectRequestValidator(Package collectPackage)
        {
            this.VerifyIfCollectPackageIsNull(collectPackage);
            this.VerifyIfCollectRequestParameterIsNull(collectPackage.CollectRequests);
            this.VerifyIfExistsANullRequesCollectItemInCollection(collectPackage.CollectRequests);
            this.VerifyIfRequestIdsIsNotNull(collectPackage.CollectRequests);
            this.VerifyIfExistsDuplicatedRequestIDs(collectPackage.CollectRequests);
            
            this.RequestedDefinitions = collectPackage.Definitions;
        }

        public void ValidateOvalDefinitions(bool validateSchematron)
        {
            foreach (var definitionInfo in this.RequestedDefinitions)
            {
                var ovalDefinitionsXml = definitionInfo.Text;
                var ovalDefinitionsValidator = new OvalDefinitionsValidator(ovalDefinitionsXml, definitionInfo.Id);
                if (!ovalDefinitionsValidator.Schema.IsValid)
                    throw new OvalDefinitionsValidationException(ovalDefinitionsValidator.Schema.ErrorList);

                if (validateSchematron)
                {
                    var schematron = ovalDefinitionsValidator.ValidateSchematron();
                    if (!schematron.IsValid)
                        throw new OvalDefinitionsValidationException(schematron.ErrorList);
                }
            }
        }

        private void VerifyIfCollectPackageIsNull(Package collectPackage)
        {
            if (collectPackage == null)
                throw new ArgumentNullException("The CollectPackage parameter of CollectRequest method cannot be null.");
        }

        private void VerifyIfCollectRequestParameterIsNull(Request[] collectRequest)
        {
            if (collectRequest == null)
                throw new ArgumentNullException("The collectRequest parameter of CollectRequest method cannot be null.");
        }

        private void VerifyIfExistsANullRequesCollectItemInCollection(Request[] collectRequest)
        {
            var hasNullItems = collectRequest.Where(x => x == null).Count() > 0;
            if (hasNullItems)
                throw new RequestItemNullException("There are null items in request collect.");
        }

        private void VerifyIfExistsDuplicatedRequestIDs(Request[] collectRequest)
        {
            var total = collectRequest.Count();
            var duplicated = (from x in collectRequest select x.RequestId).Distinct().Count();
            if (total != duplicated)
                throw new DuplicatedRequestIDsException("There are duplicated request identifiers.");
        }

        private void VerifyIfRequestIdsIsNotNull(Request[] collectRequest)
        {
            var element = collectRequest.FirstOrDefault(x => String.IsNullOrEmpty(x.RequestId));
            if (element != null)
                throw new RequestIDNullException("There are CollectRequest with RequestId null.");
        }
    }

    public class OvalDefinitionsValidationException : Exception 
    { 
        public OvalDefinitionsValidationException(IEnumerable<string> validationErrorList)
            : base(string.Join(Environment.NewLine, validationErrorList)) { }
    }
}
