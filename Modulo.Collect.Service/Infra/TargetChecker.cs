using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Service.Server.Infra;

namespace Modulo.Collect.Service.Server
{
    public class TargetChecker
    {
    }

    public class TargetCheckerFactory
    {
        private FamilyEnumeration Family;

        public TargetCheckerFactory(FamilyEnumeration family)
        {
            this.Family = family;
        }

        public ITargetChecker NewTargetFactory()
        {
            switch (this.Family)
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

            throw new NotSupportedException(String.Format("{0} platform is not supported currently.", this.Family.ToString()));
        }
    
    }

    public interface ITargetChecker
    {
        TargetCheckingResult Check();
    }

    public class TargetCheckingResult
    {
        public Boolean IsTargetAvailable;
        public String ErrorMessage;
    }
}


