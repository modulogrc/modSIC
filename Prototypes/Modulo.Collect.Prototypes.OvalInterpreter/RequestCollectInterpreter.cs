using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Service.Contract;

namespace ModuloOvalInterpreter
{
    public class RequestCollectInterpreter // : IRequestCollect
    {

        private string address;
        private string ovalDefinitions;
        private string externalVariables;
        private string requestId;
        private string credential;
        private Dictionary<string, string> targetParameters;


        #region IRequestCollect Members

        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

        public string OvalDefinitions
        {
            get
            {
                return ovalDefinitions;
            }
            set
            {
                ovalDefinitions = value;
            }
        }

        public string ExternalVariables
        {
            get
            {
                return externalVariables;
            }
            set
            {
                externalVariables = value;
            }
        }
        
        public string RequestId
        {
            get
            {
                return requestId;
            }
            set
            {
                requestId = value;
            }
        }

        public string Credential
        {
            get
            {
                return credential;
            }
            set
            {
                credential = value;
            }
        }

        public Dictionary<string, string> TargetParameters
        {
            get
            {
                return targetParameters;

            }
            set
            {
                targetParameters = value;
            }
        }

        #endregion
    }
}
