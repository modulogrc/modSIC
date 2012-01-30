using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Common;

namespace Modulo.Collect.Service.Server.Infra
{
    public class TargetCheckerFactory
    {
        public ITargetChecker NewTargetFactoryForFamily(FamilyEnumeration family)
        {
            switch (family)
            {
                case FamilyEnumeration.windows:
                    return new WindowsTargetChecker();
                case FamilyEnumeration.unix:
                    return new UnixTargetChecker();
                case FamilyEnumeration.ios:
                    return new CiscoIosChecker();
                case FamilyEnumeration.undefined:
                    return new UndefinedTargetChecker();
                case FamilyEnumeration.catos:
                case FamilyEnumeration.macos:
                case FamilyEnumeration.pixos:
                case FamilyEnumeration.vmware_infrastructure:
                    break;
            }

            throw new NotSupportedException(String.Format("{0} platform is not supported currently.", family.ToString()));
        }

    }
}
