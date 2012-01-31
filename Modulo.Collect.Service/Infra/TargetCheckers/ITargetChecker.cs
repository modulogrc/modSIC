using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using Modulo.Collect.Service.Contract;

namespace Modulo.Collect.Service.Server.Infra
{
    public interface ITargetChecker
    {
        TargetCheckingResult Check(TargetInfo targetInfo);
    }

}
