using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modulo.Collect.Service.Contract
{
    public class TargetCheckingResult
    {
        public Boolean IsTargetAvailable { get; set; }

        public String ErrorMessage { get; set; }
    }
}
