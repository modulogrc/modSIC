﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modulo.Collect.Service.Server.Infra
{
    public class UndefinedTargetChecker: ITargetChecker
    {
        public TargetCheckingResult Check()
        {
            throw new NotImplementedException();
        }
    }
}
