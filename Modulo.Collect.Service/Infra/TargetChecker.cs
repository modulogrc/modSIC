using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.Service.Server.Infra;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Service.Contract;

namespace Modulo.Collect.Service.Server
{

    public class TargetChecker
    {
        public TargetCheckingResult Check(TargetPlatforms family, TargetInfo targetInfo)
        {
            var targetChecker = new TargetCheckerFactory().NewTargetFactoryForFamily(family);
            return targetChecker.Check(targetInfo);
        }
    }


}


