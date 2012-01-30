using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Service.Security;
using Modulo.Collect.Service.Contract.Security;

namespace Modulo.Collect.Service.Server
{
    public class TargetInfoExtractor
    {
        public TargetInfo GetTargetInformation(byte[] credentialInfo, string targetAddress)
        {
            var certificate = new CertificateFactory().GetCertificate();

            var deserializedCredentials =
                new CollectServiceCryptoProvider()
                    .DecryptCredentialBasedOnCertificateOfServer(credentialInfo, certificate);

            return
                new TargetInfoFactory(
                    targetAddress,
                    deserializedCredentials.Domain,
                    deserializedCredentials.UserName,
                    deserializedCredentials.Password,
                    deserializedCredentials.AdministrativePassword).Create();
        }
    }
}
