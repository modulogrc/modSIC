using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Service.Contract;

namespace Modulo.Collect.Service.Server.Infra
{
    public class TargetCheckerFactory
    {
        public ITargetChecker NewTargetFactoryForFamily(TargetPlatforms family)
        {
            switch (family)
            {
                case TargetPlatforms.windows:
                    return new WindowsTargetChecker();
                case TargetPlatforms.unix:
                    return new UnixTargetChecker();
                case TargetPlatforms.ios:
                    return new CiscoIosChecker();
                case TargetPlatforms.undefined:
                    return new UndefinedTargetChecker();
                case TargetPlatforms.catos:
                case TargetPlatforms.macos:
                case TargetPlatforms.pixos:
                case TargetPlatforms.vmware_infrastructure:
                    break;
            }

            throw new NotSupportedException(String.Format("{0} platform is not supported currently.", family.ToString()));
        }

    }
}
