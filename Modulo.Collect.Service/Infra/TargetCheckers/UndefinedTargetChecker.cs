using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;

namespace Modulo.Collect.Service.Server.Infra
{
    public class UndefinedTargetChecker: ITargetChecker
    {
        public TargetCheckingResult Check(TargetInfo targetInfo)
        {
            throw new NotImplementedException();
        }
    }
}
